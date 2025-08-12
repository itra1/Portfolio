#include "launcher.h"

#include "settingsstorage.h"
#include "core/place.h"
#include "core/usersession.h"
#include "core/gameplace.h"
#include "config.h"

#include "base/webServer/webserver.h"
#include "base/bittorrent/session.h"
#include "base/bittorrent/infohash.h"
#include "base/bittorrent/torrenthandle.h"

#include "gui/mainwindow.h"
#include "base/utils/fs.h"
#include "base/profile.h"
#include "Processthreadsapi.h"

using namespace Core;

static const char *traslateTextLauncher[] = {
    QT_TRANSLATE_NOOP("LauncherTranslate", "gameIsPlay"),
    QT_TRANSLATE_NOOP("LauncherTranslate", "thisInstallProcess"),
    QT_TRANSLATE_NOOP("LauncherTranslate", "existsInstallProcess"),
    QT_TRANSLATE_NOOP("LauncherTranslate", "thisUpdateProcess"),
    QT_TRANSLATE_NOOP("LauncherTranslate", "existsUpdateProcess"),
    QT_TRANSLATE_NOOP("LauncherTranslate", "versionNotAvalable"),
    QT_TRANSLATE_NOOP("LauncherTranslate", "thisHashChecking"),
    QT_TRANSLATE_NOOP("LauncherTranslate", "existsHashChecking")
};

static QList<Core::LanguageStruct> languageNames = {
    {"ru","launcher_ru_RU",QT_TRANSLATE_NOOP("Language", "Russian"),QLocale::Russian}
   ,{"en","launcher_en_EN",QT_TRANSLATE_NOOP("Language", "English"),QLocale::English}
};

const QString KS_InstallString = "installPath";
const QString KS_isAutoUpdate = "isUpdate";
const QString KS_isP2p = "isP2p";
const QString KS_language = "language";
const QString KS_selectStream = "stream";

Launcher *Launcher::m_instance = nullptr;

Launcher::Launcher(QObject *parent)
    : QObject(parent),
      m_p2pEnable(SettingsStorage::instance()->loadValue(KS_isP2p,false).toBool()),
      m_autoUpdateEnable(SettingsStorage::instance()->loadValue(KS_isAutoUpdate,false).toBool()),
      m_installPath(SettingsStorage::instance()->loadValue(KS_InstallString,QDir::currentPath() + "/apps").toString()),
      m_checkUpdateComplete(false),
      m_state(PlaceState::None),
      m_needLauncherUpdate(false),
      m_selectGameGuid(SettingsStorage::instance()->loadValue(KS_selectStream,"").toString())
{


}

void Launcher::initInstance()
{
    if(!m_instance)
        m_instance = new Launcher();
}

void Launcher::freeInstance()
{
    if (m_instance) {
        delete m_instance;
        m_instance = nullptr;
    }
}

Launcher *Launcher::instance()
{
    return m_instance;
}

void Launcher::initialization(QQmlApplicationEngine *engine)
{
    WebServer::initInstance();

    m_torrents = QHash<BitTorrent::InfoHash, BitTorrent::TorrentHandle *>();
    BitTorrent::Session::initInstance();

    connect(BitTorrent::Session::instance(),&BitTorrent::Session::torrentAdded,
            this,&Launcher::torrentAdded);
    connect(BitTorrent::Session::instance(),&BitTorrent::Session::torrentRemoved,
            this,&Launcher::torrentRemoved);
    connect(BitTorrent::Session::instance(),&BitTorrent::Session::torrentsUpdated,
            this,&Launcher::torrentsUpdated);
    connect(BitTorrent::Session::instance(),&BitTorrent::Session::torrentFinished,
            this,&Launcher::torrentFinished);
    connect(BitTorrent::Session::instance(),&BitTorrent::Session::torrentFinishedChecking,
            this,&Launcher::torrentFinishedChecking);

    connect(Core::UserSession::instance(),&Core::UserSession::onAuthChange,this,[=](bool isAuth){
        if(isAuth){
            QList<UserSession::Version> * userVersions =  UserSession::instance()->versions();
            bool exists = false;
            for(UserSession::Version elem : *userVersions){
                if(m_selectGameGuid == elem.guid)
                    exists = true;
            }
            if(!exists && userVersions->length() > 0)
                setSelectGameGuid(userVersions->at(0).guid);
        }
    });

    m_processCheckTimer = new QTimer();
    m_processCheckTimer->setInterval(2000);
    connect(m_processCheckTimer,&QTimer::timeout,
            this,&Launcher::handleCheckProcess);
    m_processCheckTimer->start();

    if (m_installPath == Config::GAME_INSTALL_DEFAULT_PATH ){
        QDir dir(m_installPath);
        if(!dir.exists())
           dir.mkpath(m_installPath);
    }

    confirmSettings();
    initLanguage();

}

void Launcher::setState(bool isGame, QString guid, PlaceState::State state)
{
    if(isGame)
        emit onStateChangeAny(m_state);

    if(isGame && m_selectGameGuid != guid){
        return;
    }

    if(!isGame){
        if(state == PlaceState::UpdateReady){
            setNeedLauncherUpdate(true);
            if(getAutoUpdateEnable())
                handleUpdateButton();
        }
    }else{
        m_state = state;
        emit onStateChange(m_state);
    }

}

PlaceState::State Launcher::getState()
{
    return m_state;
}

void Launcher::handlePlayButton()
{
    if(existsOtherProcess())
        return;

    Place *pl = SourceManager::instance()->getSource(m_selectGameGuid,true);

    if(pl != nullptr)
        (dynamic_cast<GamePlace*>(pl))->play();

}

void Launcher::handleInstallButton()
{
    if(existsOtherProcess())
        return;

    if(!avalableInstallVersion()){
        showWarningMessage(QCoreApplication::translate("LauncherTranslate",  traslateTextLauncher[5]));
        return;
    }

    emit onInstallRequest();
}

void Launcher::handleUpdateButton()
{
    if(getNeedLauncherUpdate()){
        if(Core::SourceManager::instance()->isPlay()){
            showWarningMessage(0);
            return;
        }
        updateLauncher();
    }else if(SourceManager::instance()->getSource(m_selectGameGuid,true)->getState() == PlaceState::UpdateReady){
        if(existsOtherProcess())
            return;
        updateGame();
    }
}

void Launcher::checkUpdateComplete()
{
    m_checkUpdateComplete = true;
}

void Launcher::checkUpdateFailed()
{
    m_checkUpdateComplete = false;
}

void Launcher::quit()
{
    BitTorrent::Session::freeInstance();
    QApplication::exit();
}

Place *Launcher::getReadyLauncherVersion()
{
    Place *pl = SourceManager::instance()->getSourceLauncher();
    return pl;
}

Place *Launcher::getReadyGameVersion()
{
    Place *pl = SourceManager::instance()->getSource(m_selectGameGuid,true);
    return pl;
}

void Launcher::setInstallPath(QString dictionary)
{
    bool isChange = m_installPath != dictionary;
    m_installPath = dictionary;

    if(isChange)
        Core::SourceManager::instance()->checkLocalGamePlace();
}

void Launcher::setAutoUpdateEnable(bool autoUpdate)
{
    m_autoUpdateEnable = autoUpdate;
}

void Launcher::setP2pEnable(bool p2pEnable)
{
    m_p2pEnable = p2pEnable;
}

QString Launcher::getInstallPath()
{
    return m_installPath;
}

bool Launcher::getAutoUpdateEnable()
{
    return m_autoUpdateEnable;
}

bool Launcher::getP2pEnable()
{
    return m_p2pEnable;
}

void Launcher::checkGameCache()
{
    Place *pl = SourceManager::instance()->getSource(m_selectGameGuid,true);

    if(pl != nullptr)
        (dynamic_cast<GamePlace*>(pl))->checkCache();

}

void Launcher::restoreDefaultSettings()
{
    m_p2pEnable = true;
    m_autoUpdateEnable = false;

    setInstallPath(QDir::currentPath() + "/apps");

    if (getInstallPath() == QDir::currentPath() + "/apps" ){
        QDir dir(getInstallPath());
        if(!dir.exists())
           dir.mkpath(getInstallPath());
    }
}

void Launcher::save()
{
    SettingsStorage::instance()->storeValue(KS_isP2p,m_p2pEnable);
    SettingsStorage::instance()->storeValue(KS_isAutoUpdate,m_autoUpdateEnable);
    SettingsStorage::instance()->storeValue(KS_InstallString,getInstallPath());
    SettingsStorage::instance()->storeValue(KS_language,m_language);
    SettingsStorage::instance()->saveSync();

    confirmSettings();

    emit onSave();
}

void Launcher::confirmSettings(){

    auto session = BitTorrent::Session::instance();

    if(m_p2pEnable)
        session->setMaxUploads(10);
    else
        session->setMaxUploads(0);
    session->configure();
}

void Launcher::updateGame()
{
    Place *pl = SourceManager::instance()->getSource(m_selectGameGuid,true);

    if(pl != nullptr)
        pl->update();
}

void Launcher::updateLauncher()
{
    Place *pl = SourceManager::instance()->getSourceLauncher();

    if(pl != nullptr)
        pl->update();
}

void Launcher::pauseTorrent()
{

    Place *pl = SourceManager::instance()->getSource(m_selectGameGuid,true);

    if(pl == nullptr)
        return;

    BitTorrent::TorrentHandle *torrentGame = getTorrentByName(pl->torrentName());

    if(torrentGame == nullptr)
        return;

    if(!torrentGame->isPaused())
        torrentGame->pause();
    else
        torrentGame->resume();
}

void Launcher::torrentAdded(BitTorrent::TorrentHandle * const torrent)
{
    qDebug() << "Torrent Added " + torrent->name();
    m_torrents = BitTorrent::Session::instance()->torrents();

}

void Launcher::torrentFinished(BitTorrent::TorrentHandle * const torrent)
{
    qDebug() << "Torrent Finished " + torrent->name();

    SourceManager::instance()->sourceDownloadComplete(torrent->name());

}

void Launcher::torrentFinishedChecking(BitTorrent::TorrentHandle * const torrent)
{
    qDebug() << "Torrent Finished Checking " + torrent->name();

    if(torrent->progress() >= 1){
        SourceManager::instance()->sourceDownloadComplete(torrent->name());
    }

}

void Launcher::torrentRemoved()
{
    m_torrents = BitTorrent::Session::instance()->torrents();
}

void Launcher::torrentsUpdated()
{

    for (QHash<BitTorrent::InfoHash, BitTorrent::TorrentHandle *>::iterator it = m_torrents.begin(); it != m_torrents.end(); ++it)
    {
        qDebug() << it.value()->name() + " : " + QString::number(it.value()->progress());
        if(it.value()->name().contains("game")){
            emit onUpdateTorrent(it.value());
        }
    }
}

void Launcher::handleCheckProcess()
{
    //if(m_isPlay != isPlay()){
    //    m_isPlay = isPlay();
        emit onPlay(isPlay());
    //}
}

void Launcher::handleInstallAgreenmentConfirm()
{
    Place *pl = SourceManager::instance()->getSource(m_selectGameGuid,true);

    if(pl != nullptr)
        (dynamic_cast<GamePlace*>(pl))->install();

}

void Launcher::gameInstallPathChange()
{
    qDebug() << "gameInstallPathChange";
    //setState(InstallState::None);
    //SourceManager::instance()->checkLocalVersion(true);
}

bool Launcher::getNeedLauncherUpdate() const
{
    return m_needLauncherUpdate;
}

void Launcher::setNeedLauncherUpdate(bool needLauncherUpdate)
{
    m_needLauncherUpdate = needLauncherUpdate;
    emit onLauncherUpdateReady();
}

bool Launcher::existsOtherProcess()
{
    Place *pl = SourceManager::instance()->getSource(m_selectGameGuid,true);

    if(pl == nullptr)
        return false;

    // Game is playing
    if(Core::SourceManager::instance()->isPlay()){
        showWarningMessage(QCoreApplication::translate("LauncherTranslate",  traslateTextLauncher[0]));
        return true;
    }
    if((dynamic_cast<GamePlace*>(pl))->getState() == PlaceState::InstallProcess){
        showWarningMessage(QCoreApplication::translate("LauncherTranslate",  traslateTextLauncher[1]));
        return true;
    }
    if((dynamic_cast<GamePlace*>(pl))->getState() == PlaceState::UpdateProcess){
        showWarningMessage(QCoreApplication::translate("LauncherTranslate",  traslateTextLauncher[3]));
        return true;
    }
    if((dynamic_cast<GamePlace*>(pl))->getState() == PlaceState::HashChecking){
        showWarningMessage(QCoreApplication::translate("LauncherTranslate",  traslateTextLauncher[6]));
        return true;
    }
    if(Core::SourceManager::instance()->existsInstallProcessGame()){
        showWarningMessage(QCoreApplication::translate("LauncherTranslate",  traslateTextLauncher[2]));
        return true;
    }
    if(Core::SourceManager::instance()->existsUpdateProcessGame()){
        showWarningMessage(QCoreApplication::translate("LauncherTranslate",  traslateTextLauncher[4]));
        return true;
    }
    if(Core::SourceManager::instance()->existsHashCkeckingProcessGame()){
        showWarningMessage(QCoreApplication::translate("LauncherTranslate",  traslateTextLauncher[7]));
        return true;
    }

    return false;
}

QString Launcher::getTranslateName(int num)
{
    return traslateTextLauncher[num];
}

LanguageStruct Launcher::getLang(int num)
{
    return languageNames[num];
}

QList<LanguageStruct> Launcher::getAllLang()
{
    return languageNames;
}

bool Launcher::avalableInstallVersion()
{
    Place *pl = SourceManager::instance()->getSource(m_selectGameGuid,true);
    return pl->getSourceServer() != nullptr;
}

QString Launcher::getSelectGameGuid() const
{
    return m_selectGameGuid;
}

void Launcher::setSelectGameGuid(const QString &selectGameVersion)
{
    qDebug() << "setSelectGameVersion" << selectGameVersion;
    m_selectGameGuid = selectGameVersion;

    SettingsStorage::instance()->storeValue(KS_selectStream,m_selectGameGuid);

    Place *pl = SourceManager::instance()->getSource(m_selectGameGuid,true);
    m_state = pl->getState();
    emit onChangeSelectVersion();
}

void Launcher::showWarningMessage(QString message)
{
    emit onMessage(message);
}

QString Launcher::getLanguage() const
{
    return m_language;
}

void Launcher::setLanguage(const QString &language)
{
    m_language = language;

    QString fileLocal = "";

    for(LanguageStruct elem : languageNames){
        if(elem.code == language)
            fileLocal = elem.file;
    }

    m_translator.load(fileLocal,"./localization/");

    //m_translator.load(":/QmlLanguage_" + translation, "."); // Загружаем перевод
    qApp->installTranslator(&m_translator);                 // Устанавливаем его в приложение
    emit onLanguageChange();                                 // Сигнализируем об изменении текущего перевода


}

void Launcher::initLanguage()
{
    bool existsLang = SettingsStorage::instance()->hashValue(KS_language);

    if(!existsLang){

        bool cont = false;

        for(LanguageStruct elem :languageNames){
            if(elem.locale == QLocale::system().language()){
                m_language = elem.code;
                cont = true;
            }
        }

        if(!cont)
            m_language = languageNames[1].code;

    }else{
        m_language = SettingsStorage::instance()->loadValue(KS_language,languageNames[0].code).toString();
    }

    setLanguage(m_language);
}

bool Launcher::isPlay()
{
    return Core::SourceManager::instance()->isPlay();
}

BitTorrent::TorrentHandle *Launcher::getTorrentByName(QString name)
{
    for (QHash<BitTorrent::InfoHash, BitTorrent::TorrentHandle *>::iterator it = m_torrents.begin(); it != m_torrents.end(); ++it)
    {
        if(it.value()->name() == name){
            return  it.value();
        }
    }
    return nullptr;
}

BitTorrent::TorrentHandle *Launcher::getTorrentSelectVersion()
{
    Place *pl = SourceManager::instance()->getSource(m_selectGameGuid,true);

    if(pl == nullptr)
        return nullptr;

    getTorrentByName(pl->torrentName());
}
