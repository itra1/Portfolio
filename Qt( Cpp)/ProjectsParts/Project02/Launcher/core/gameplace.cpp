#include "gameplace.h"

#include <QDebug>
#include "sourceinfo.h"
#include "config.h"
#include "core/launcher.h"
#include "core/usersession.h"
#include "base/utils/fs.h"
#include "base/bittorrent/torrenthandle.h"
#include <QProcess>
#include "base/bittorrent/session.h"
#include "base/bittorrent/torrentinfo.h"
#include "base/bittorrent/infohash.h"

using namespace Core;

static const char *traslateTextGamePlace[] = {
    QT_TRANSLATE_NOOP("GamePlace", "NoExistsGame")
};

GamePlace::GamePlace(QObject *parent)
    : Place(parent)
{
    qDebug() << "GamePlace";
}

bool GamePlace::isGame()
{
    return true;
}

QString GamePlace::getSourceUrl()
{
    return m_torrentUrl;

//    switch (m_version) {
//        case SourceInfo::Alpha:
//            return Config::SERVER_GAME_ALPHA_TORRENT;
//        case SourceInfo::Beta:
//            return Config::SERVER_GAME_BETA_TORRENT;
//        case SourceInfo::Release:
//        default:
//            return Config::SERVER_GAME_RELEASE_TORRENT;
//    }

}

void GamePlace::setTorrentUrl(const QString &torrentUrl)
{
    m_torrentUrl = torrentUrl;
}

void GamePlace::checkState()
{
    if(m_sourceLocal == nullptr && m_sourceServer == nullptr){
        setState(PlaceState::InstallReady);
        return;
    }

    if(m_sourceLocal == nullptr && m_sourceServer != nullptr){
        setState(PlaceState::InstallReady);
        return;
    }

    if(m_sourceLocal != nullptr && m_sourceServer == nullptr){

        SourceInfo::InstallState stateInstall = m_sourceLocal->installState();

        if(stateInstall == SourceInfo::Complete){
            setState(PlaceState::PlayReady);
        }else{
            setState(PlaceState::InstallProcess);
        }
        return;
    }

    if(m_sourceLocal != nullptr && m_sourceServer != nullptr){

        SourceInfo::InstallState stateInstall = m_sourceLocal->installState();

        if(SourceInfo::firstGreatSecond(m_sourceServer,m_sourceLocal)){

            if(stateInstall != SourceInfo::Complete || Core::Launcher::instance()->getAutoUpdateEnable()){
                install();
                setState(PlaceState::UpdateProcess);
            }else{
                setState(PlaceState::UpdateReady);
            }

        }else{
            if(stateInstall == SourceInfo::Install){
                m_sourceLocal->playTorrent(false);
                setState(PlaceState::InstallProcess);
            }else if(stateInstall == SourceInfo::Update){
                m_sourceLocal->playTorrent(true);
                setState(PlaceState::UpdateProcess);
            }else if(stateInstall == SourceInfo::HashChecking){
                m_sourceLocal->playTorrent(false);
                setState(PlaceState::HashChecking);
            }else if(stateInstall == SourceInfo::Complete){
                m_sourceLocal->playTorrent(true);
                setState(PlaceState::PlayReady);
            }else
                setState(PlaceState::None);

        }
        return;
    }
}

void GamePlace::checkCache()
{
    BitTorrent::TorrentHandle *torrentGame = Core::Launcher::instance()->getTorrentByName(torrentName());

    if(torrentGame == nullptr)
        return;


    findLocalSource();

    if(m_sourceLocal == nullptr){
        Launcher::instance()->showWarningMessage(QCoreApplication::translate("GamePlace",  traslateTextGamePlace[0]));
        return;
    }

    torrentGame->forceRecheck();

    if(m_sourceLocal == nullptr)
        return;

    Utils::Fs::removeNoListFile(m_sourceLocal->savePath()
                                ,m_sourceLocal->torrentInfo().rootFolder()
                                ,m_sourceLocal->torrentInfo().filePaths()
                                , QStringList() << Config::GAME_INI_FILE << Config::GAME_TORRENT_FILE );


    m_sourceLocal->setInstallState(SourceInfo::HashChecking);
    setState(PlaceState::HashChecking);
    //handleInstallGameProgress();
}

bool GamePlace::isCheckChache()
{
    return getState() == PlaceState::State::HashChecking;
}

void GamePlace::refindLocal()
{
    BitTorrent::TorrentHandle *torrentGame = Core::Launcher::instance()->getTorrentByName(torrentName());

    if(torrentGame != nullptr){
        BitTorrent::Session::instance()->deleteTorrent(torrentGame->info().hash(),false);
    };

    findLocalSource();
}

void GamePlace::play()
{
    if(m_sourceLocal == nullptr)
        return;

    findLocalSource();

    if(m_sourceLocal == nullptr){
        Launcher::instance()->showWarningMessage(QCoreApplication::translate("GamePlace",  traslateTextGamePlace[0]));
        return;
    }

    if(!QFile(Utils::Fs::toNativePath(m_sourceLocal->exeFile())).exists()){

        checkCache();
        return;
    }

    if(!isPlay()){
        m_gameProcess.start("\""+ m_sourceLocal->exeFile()+ "\"",QStringList()
                            <<"-gt"
                            <<Core::UserSession::instance()->token()
                            <<"-gs"
                            <<getStreamName()
                            );

        setState(PlaceState::Play);
        emit onPlay(true);
    }
}

bool GamePlace::isPlay()
{
    return m_gameProcess.processId() != 0;
}

bool GamePlace::isPlayReady()
{
    return getState() == PlaceState::State::PlayReady;
}

void GamePlace::update()
{
    m_sourceLocal = m_sourceServer->clone();
    m_sourceLocal->update();
    setState(PlaceState::UpdateProcess);
}

void GamePlace::install()
{
    if(m_sourceServer == nullptr){
        qDebug() << "No exists version ";
        return;
    }

    BitTorrent::TorrentHandle *torrentGame = Core::Launcher::instance()->getTorrentByName(torrentName());

    if(torrentGame != nullptr)
        BitTorrent::Session::instance()->deleteTorrent(torrentGame->info().hash(),false);

    m_sourceLocal = m_sourceServer->clone();
    if(m_sourceLocal->installGame()){
        setState(PlaceState::InstallProcess);
    }
}
