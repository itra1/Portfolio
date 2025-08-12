#include "mainwindow.h"
#include "core/settingsstorage.h"

#include <QQmlApplicationEngine>
#include <QQmlContext>
#include <QtWebEngine>
#include <QQmlEngine>
#include <QList>

#include "core/usersession.h"
#include "core/place.h"
#include "core/gameplace.h"
#include "core/placestate.h"
#include "base/net/downloadmanager.h"
#include "base/net/downloadhandler.h"

using namespace Gui;

static const char *traslateTextMainWindow[] = {
    QT_TRANSLATE_NOOP("Warning", "NoOpenSettingIfPlay"),
    QT_TRANSLATE_NOOP("Warning", "NetworkUnavailable")
};

static const char *urls[] = {
    QT_TRANSLATE_NOOP("MainWindow", "mainUrl"),
    QT_TRANSLATE_NOOP("MainWindow", "agreementUrl"),
    QT_TRANSLATE_NOOP("MainWindow", "profileUrl")
};

MainWindow *MainWindow::m_instance = nullptr;

MainWindow::MainWindow(QQmlApplicationEngine *engine, QObject *parent)
    : QObject(parent),
    m_isSettingsEnable(false),
    m_engine(engine),
    m_showWarningMessage(false),
    m_launcherNeedUpdate(false),
    m_internetStatus(Core::Internet::instance()->exists())
{

    qmlRegisterType<Core::PlaceState>("PlaceState",1,0,"PlaceState");
    m_engine->rootContext()->setContextProperty("model",this);
    // Произошел конфликт имен, потому пришлось дублировать
    m_engine->rootContext()->setContextProperty("mainModel",this);
    engine->load(QUrl(QStringLiteral("qrc:/forms/Loader.qml")));
}

void MainWindow::initInstance(QQmlApplicationEngine *engine, QObject *parent)
{
    if(!m_instance)
        m_instance = new MainWindow(engine,parent);
}

void MainWindow::freeInstance()
{
    if (m_instance) {
        delete m_instance;
        m_instance = nullptr;
    }
}

MainWindow *MainWindow::instance()
{
    return m_instance;
}


void MainWindow::loadMainWindow()
{
    QtWebEngine::initialize();

    setIsPauseTorrent(false);
    //QtWebView::initialize();

    //m_engine = engine;
    m_engine->load(QUrl(QStringLiteral("qrc:/forms/main.qml")));

    connect(Core::Launcher::instance(),&Core::Launcher::onLauncherUpdateReady,[this](){
        setLauncherNeedUpdate(true);
    });
    connect(Core::Launcher::instance(),&Core::Launcher::onMessage,[this](QString message){
        m_warningMessage = message;
        setShowWarningMessage(true);
    });
    connect(Core::Launcher::instance(),&Core::Launcher::onChangeSelectVersion,[this](){
        stateChange(Core::Launcher::instance()->getState());
    });
    connect(Core::Launcher::instance(),&Core::Launcher::onStateChange,this,&MainWindow::stateChange);
    connect(Core::Launcher::instance(),&Core::Launcher::onStateChangeAny,this,[=](){
        emit onStateChangeAny();
    });
    connect(Core::Launcher::instance(),&Core::Launcher::onUpdateTorrent,this,&MainWindow::updateProgress);
    connect(Core::Launcher::instance(),&Core::Launcher::onPlay,this,&MainWindow::handlePlayGame);
    connect(Core::Launcher::instance(),&Core::Launcher::onInstallRequest,this,&MainWindow::handleInstallRequest);
    connect(Core::Launcher::instance(),&Core::Launcher::onLanguageChange,this,[=](){
        m_engine->retranslate();
    });

    connect(Core::Internet::instance(),&Core::Internet::onInternetChange,[=](bool internetAvalable){

        bool visible = m_internetStatus && !internetAvalable;
        bool internetActivate = !m_internetStatus && internetAvalable;

        m_internetStatus = internetAvalable;

        if(visible){
            m_warningMessage = QCoreApplication::translate("Warning",  traslateTextMainWindow[1]);
            setShowWarningMessage(true);
        }

        if(internetActivate)
            emit onInternetActive();

    });

    connect(Core::UserSession::instance(),&Core::UserSession::onAuthChange,this,[=](bool isAuth){
        emit onUserAuthChange();
    });

    connect(BitTorrent::Session::instance(),&BitTorrent::Session::torrentPaused,[=](BitTorrent::TorrentHandle *const torrent){
        if(torrent->name().contains("game"))
            setIsPauseTorrent(true);
    });
    connect(BitTorrent::Session::instance(),&BitTorrent::Session::torrentResumed,[=](BitTorrent::TorrentHandle *const torrent){
        if(torrent->name().contains("game"))
            setIsPauseTorrent(false);
    });

    if(m_internetStatus)
        emit onInternetActive();

    stateChange(Core::Launcher::instance()->getState());

    loadAgreement();
}

void MainWindow::mainWindowsLoaded()
{
    QTimer::singleShot(1000,this,[=]{
        emit onLoadComplete();
    });

}

void MainWindow::closeApplication()
{
    Core::Launcher::instance()->quit();
}

Core::PlaceState::State MainWindow::getState()
{
    return Core::Launcher::instance()->getState();
}

void MainWindow::playButton()
{
    if(getIsPauseTorrent()){
        pauseDownload();
        return;
    };

    Core::Launcher::instance()->handlePlayButton();
}

void MainWindow::installButton()
{
    Core::Launcher::instance()->handleInstallButton();
}

void MainWindow::updateGame()
{
    Core::Launcher::instance()->handleUpdateButton();
}

QString MainWindow::getUpdateVersion()
{
    if(Core::Launcher::instance()->getNeedLauncherUpdate()){
        Core::Place* pl = Core::Launcher::instance()->getReadyLauncherVersion();

        if(pl == nullptr)
            return Core::SourceInfo::displayVersionNull();
        else
            return pl->getSourceServer()->displayVersion();
    }


    Core::Place* pl = Core::Launcher::instance()->getReadyGameVersion();

    if(pl == nullptr)
        return Core::SourceInfo::displayVersionNull();
    else
        return pl->getSourceServer()->displayVersion();

}

QString MainWindow::getUpdateSpaceNeed()
{

    Core::Place* pl = Core::Launcher::instance()->getReadyGameVersion();

    if(pl->getSourceLocal() != nullptr && pl->getSourceServer() != nullptr){
        return Utils::Misc::friendlyUnit(pl->getSourceLocal()->torrentInfo().totalSize()
                                         - pl->getSourceServer()->torrentInfo().totalSize());
    }
    if(pl->getSourceLocal() == nullptr && pl->getSourceServer() != nullptr){

        if(pl->getSourceServer()->torrentInfo().totalSize() == -1)
            return "0";
        else
            return Utils::Misc::friendlyUnit(pl->getSourceServer()->torrentInfo().totalSize());
    }
    if(pl->getSourceLocal() != nullptr && pl->getSourceServer() == nullptr){

        if(pl->getSourceLocal()->torrentInfo().totalSize() == -1)
            return  "0";
        else
            return Utils::Misc::friendlyUnit(pl->getSourceLocal()->torrentInfo().totalSize());
    }
    return Utils::Misc::friendlyUnit(0);

}

QString MainWindow::getVersionInstallReady()
{
    Core::Place* pl = Core::Launcher::instance()->getReadyGameVersion();

    if(pl->getSourceLocal() == nullptr)
        return Core::SourceInfo::displayVersionNull();
    else
        return pl->getSourceLocal()->displayVersion();

}

QString MainWindow::getNoteUrl()
{
    Core::Place* pl = Core::Launcher::instance()->getReadyGameVersion();

    return pl->getSourceLocal() != nullptr
            ? pl->getSourceLocal()->noteUrl()
            : "";

}

QStringList MainWindow::getGameVersionsList()
{
    QList<Core::Place *> placeList = Core::SourceManager::instance()->getGameAvailableVersions();

    QStringList listLang;

    for(Core::Place *elem : placeList){
        listLang.append(elem->getStreamName());
    }

    return listLang;

}

int MainWindow::getGameVersionsIndex()
{
    QList<Core::Place *> placeList = Core::SourceManager::instance()->getGameAvailableVersions();

    for(int i = 0 ; i < placeList.length() ; i++){
        if(placeList[i]->getGuid() == Core::Launcher::instance()->getSelectGameGuid())
            return i;
    }

    return 0;
}

void MainWindow::setGameVersionsView(int index)
{
    qDebug() << "setGameVersionsView " << index;
    QList<Core::Place *> placeList = Core::SourceManager::instance()->getGameAvailableVersions();
    Core::Launcher::instance()->setSelectGameGuid(placeList[index]->getGuid());

}

void MainWindow::pauseDownload()
{
    Core::Launcher::instance()->pauseTorrent();
    emit onPauseTorrent();
}

void MainWindow::installPathFileDialog()
{
    QString dir = QFileDialog::getExistingDirectory(nullptr, tr("Open Directory"),
                                                    m_installPath,
                                                    QFileDialog::ShowDirsOnly
                                                    | QFileDialog::DontResolveSymlinks);
    if(dir!= "")
        setInstallPath(dir);

}

double MainWindow::getProgress()
{
    BitTorrent::TorrentHandle *torrentData =  Core::Launcher::instance()->getTorrentSelectVersion();
    if(torrentData == nullptr)
        return 0;

    return torrentData->progress();
}

bool MainWindow::isCheckingProgress()
{
    BitTorrent::TorrentHandle *torrentData =  Core::Launcher::instance()->getTorrentSelectVersion();

    if(torrentData == nullptr)
        return false;
    try {
        return (torrentData->state() == BitTorrent::TorrentState::CheckingDownloading
                || torrentData->state() == BitTorrent::TorrentState::CheckingUploading
                || torrentData->state() == BitTorrent::TorrentState::CheckingResumeData);
    } catch (std::exception ex) {
        return false;
    }
}

QString MainWindow::getReadySize()
{
    BitTorrent::TorrentHandle *torrentData =  Core::Launcher::instance()->getTorrentSelectVersion();
    if(torrentData == nullptr)
        return Utils::Misc::friendlyUnit(0);

    return Utils::Misc::friendlyUnit(torrentData->totalSize() - torrentData->completedSize());

}

QString MainWindow::getFullSize()
{
    BitTorrent::TorrentHandle *torrentData =  Core::Launcher::instance()->getTorrentSelectVersion();
    if(torrentData == nullptr)
        return Utils::Misc::friendlyUnit(0);

    return Utils::Misc::friendlyUnit(torrentData->totalSize());
}

QString MainWindow::getSpeedLoad()
{
    BitTorrent::TorrentHandle *torrentData =  Core::Launcher::instance()->getTorrentSelectVersion();
    if(torrentData == nullptr)
        return Utils::Misc::friendlyUnit(0,true);

    return Utils::Misc::friendlyUnit(torrentData->downloadPayloadRate(),true);
}

bool MainWindow::internetAvalable()
{
    return m_internetStatus;
}

bool MainWindow::isProcess()
{
    QList<Core::Place *> placeList = Core::SourceManager::instance()->getGameAvailableVersions();

    for(Core::Place *elem : placeList){
        if((elem->getState() == Core::PlaceState::State::PlayProcess)
        || (elem->getState() == Core::PlaceState::State::UpdateProcess)
        || (elem->getState() == Core::PlaceState::State::InstallProcess)
        || (elem->getState() == Core::PlaceState::State::HashChecking))
            return true;
    }
    return false;

}

bool MainWindow::avalableInstallVersion()
{
    return Core::Launcher::instance()->avalableInstallVersion();
}

void MainWindow::settingsButton()
{
    if(!m_isSettingsEnable){
        if(Core::Launcher::instance()->isPlay()){
            Core::Launcher::instance()->showWarningMessage(0);
            return;
        }
    }


    m_isSettingsEnable = !m_isSettingsEnable;
    if(m_isSettingsEnable)
        startSettingsParametrs();
    emit onIsSettings();
}

void MainWindow::saveButton()
{
    if(!getIsCorrectInstallPath())
        return;

    bool isPathChange = Core::Launcher::instance()->getInstallPath() != m_installPath;

    Core::Launcher::instance()->setAutoUpdateEnable(m_isAutomaticUpdate);
    Core::Launcher::instance()->setP2pEnable(m_isP2pDownload);
    Core::Launcher::instance()->setInstallPath(m_installPath);
    Core::Launcher::instance()->setLanguage(m_language);
    Core::Launcher::instance()->save();
    setIsDirty(false);

    if(isPathChange)
        Core::Launcher::instance()->gameInstallPathChange();
}

void MainWindow::restoreDefaultSettings()
{
    Core::Launcher::instance()->restoreDefaultSettings();
    startSettingsParametrs();
}

void MainWindow::checkGameCacheButton()
{
    if(isProcess())
        return;

    Core::Launcher::instance()->checkGameCache();

    if(m_isSettingsEnable){
        settingsButton();
        return;
    }
}

void MainWindow::setInstallPath(QUrl path)
{
    setInstallPath(path.toString().replace(0,8,""));
    setIsDirty(true);
}

void MainWindow::setLanguageView(int index)
{
    m_language = Core::Launcher::instance()->getLang(index).code;
    setIsDirty(true);
}

QStringList MainWindow::getLanguageList()
{
    QStringList listLang;

    for(Core::LanguageStruct elem : Core::Launcher::instance()->getAllLang())
        listLang.append(elem.name);
    return listLang;
}

int MainWindow::getLanguageIndex()
{
    for(int i = 0 ; i < Core::Launcher::instance()->getAllLang().length() ; i++){
        if(Core::Launcher::instance()->getAllLang()[i].code == m_language)
            return i;
    }
    return 0;
}

void MainWindow::logIn()
{
    Core::UserSession::instance()->login();
}

void MainWindow::logOut()
{
    Core::UserSession::instance()->logOut();
}

void MainWindow::checkInstallPath(QString path)
{
    setInstallPath(path);
}

bool MainWindow::isUserAuth()
{
    return Core::UserSession::instance()->isAuth();
}

void MainWindow::agreementConfirm(bool isConfirm)
{
    if(isConfirm)
        Core::Launcher::instance()->handleInstallAgreenmentConfirm();

        emit onInstallRequestFalse();
}

void MainWindow::loadAgreement()
{
    QString url = QCoreApplication::translate("MainWindow",  urls[1]);

    Net::DownloadHandler *handler = Net::DownloadManager::instance()->download(Net::DownloadRequest(url));
    connect(handler, static_cast<void (Net::DownloadHandler::*)(const QString &, const QByteArray &)>(&Net::DownloadHandler::downloadFinished)
            , [=](const QString &url, const QByteArray &data){

        qDebug() << "loadAgreement complete";
        m_agreemetText = data;
        emit onLoadAgreement();
    });

    connect(handler, &Net::DownloadHandler::downloadFailed, [=](const QString &url, const QString &reason){

        qDebug() << "loadAgreement false";
        qDebug() << reason;

        QTimer::singleShot(500,this, &MainWindow::loadAgreement);
    });
}

QString MainWindow::getAgreement()
{
    return m_agreemetText;
}

bool MainWindow::getIsPauseTorrent()
{
    return m_isPauseTorrent;
}

bool MainWindow::getIsSettings()
{
    return m_isSettingsEnable;
}

bool MainWindow::getIsAutomaticUpdate()
{
    return m_isAutomaticUpdate;
}

void MainWindow::setIsAutomaticUpdate(const bool isAutomaticUpdate)
{
    m_isAutomaticUpdate = isAutomaticUpdate;
    setIsDirty(true);
    emit onIsAutomaticUpdateChange();
}

bool MainWindow::getIsP2pDownload()
{
    return m_isP2pDownload;
}

void MainWindow::setIsP2pDownload(const bool isP2pDownload)
{
    m_isP2pDownload = isP2pDownload;
    setIsDirty(true);
    emit onIsP2pDownloadChange();
}

QString MainWindow::getInstallPath()
{
    return m_installPath;
}

void MainWindow::setInstallPath(QString newDict)
{
    m_installPath = newDict;
    emit onInstallPathChange();
}

bool MainWindow::getIsDirty()
{
    return m_isDirty;
}

void MainWindow::setIsDirty(const bool isDirty)
{
    m_isDirty = isDirty;
    emit onIsDirtyChange();
}

QString MainWindow::getMainPageUrl()
{
    return QCoreApplication::translate("MainWindow",  urls[0]);
}

bool MainWindow::getIsPlayGame()
{
    return Core::Launcher::instance()->isPlay();
}

void MainWindow::stateChange(Core::PlaceState::State state)
{
    emit onInstallStateChange();
    emit onStateChange();
}

void MainWindow::updateProgress(BitTorrent::TorrentHandle *torrentHandle)
{

    Core::Place* pl = Core::Launcher::instance()->getReadyGameVersion();

    qDebug() << "onInstallUpdate";
    if(pl != nullptr && pl->torrentName() != torrentHandle->name())
        return;

    if(torrentHandle->state() == BitTorrent::TorrentState::Downloading
            || torrentHandle->state() == BitTorrent::TorrentState::StalledDownloading
            || torrentHandle->state() == BitTorrent::TorrentState::CheckingDownloading
            || torrentHandle->state() == BitTorrent::TorrentState::CheckingUploading
            || torrentHandle->state() == BitTorrent::TorrentState::CheckingResumeData){
        emit onInstallUpdate();
    }
}

void MainWindow::handlePlayGame(bool isPlay)
{
    emit onPlayGame();
}

void MainWindow::handleInstallRequest()
{
    emit onInstallRequestShow();
}

void MainWindow::setIsPauseTorrent(bool isPauseTorrent)
{
    m_isPauseTorrent = isPauseTorrent;
    emit onPauseTorrent();
}

void MainWindow::startSettingsParametrs()
{
    setIsAutomaticUpdate(Core::Launcher::instance()->getAutoUpdateEnable());
    setIsP2pDownload(Core::Launcher::instance()->getP2pEnable());
    setInstallPath(Core::Launcher::instance()->getInstallPath());
    setLanguage(Core::Launcher::instance()->getLanguage());
    setIsDirty(true);
}

bool MainWindow::getLauncherNeedUpdate() const
{
    return m_launcherNeedUpdate;
}

void MainWindow::setLauncherNeedUpdate(bool launcherNeedUpdate)
{
    m_launcherNeedUpdate = launcherNeedUpdate;
    if(m_launcherNeedUpdate)
        emit onNeedLauncherUpdate();
}

bool MainWindow::getShowWarningMessage() const
{
    return m_showWarningMessage;
}

void MainWindow::setShowWarningMessage(bool showWarningMessage)
{
    m_showWarningMessage = showWarningMessage;
    emit onWarningMessage();
}

QString MainWindow::getWarningMessage()
{
    return m_warningMessage;
}

QString MainWindow::getLanguage() const
{
    return m_language;
}

void MainWindow::setLanguage(const QString &language)
{
    m_language = language;
}

QString MainWindow::userName()
{
    return Core::UserSession::instance()->userName();
}

QString MainWindow::userLink()
{
    return QCoreApplication::translate("MainWindow",  urls[2]);
}

bool MainWindow::getIsCorrectInstallPath()
{
    return QDir(m_installPath).exists();
}
