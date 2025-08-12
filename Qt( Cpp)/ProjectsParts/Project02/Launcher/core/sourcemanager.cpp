#include "sourcemanager.h"

#include "core/usersession.h"
#include "core/launcher.h"
#include "core/place.h"
#include "core/launcherplace.h"
#include "core/gameplace.h"
#include "base/bittorrent/session.h"
#include "base/bittorrent/torrentinfo.h"
#include "base/net/downloadhandler.h"
#include <QDir>
#include <QFile>
#include "base/utils/fs.h"
#include "QTimer"
#include "config.h"
#include "core/update/inieditor.h"

using namespace Core;

SourceManager *SourceManager::m_instance = nullptr;

SourceManager::SourceManager(QObject *parent) : QObject(parent)
   , m_torrentLoad(new QHash<QString, SourceInfo*>),
    m_placeList(new QList<Place*>() )
{
    connect(Core::UserSession::instance(),&Core::UserSession::onAuthChange,this,[=](bool isAuth){
        if(isAuth)
            initLocaPlace();
        else
            clearLocalSpace();
    });
}

SourceManager::~SourceManager()
{
}

QList<Place *> *SourceManager::getPlaceList() const
{
    return m_placeList;
}

void SourceManager::setPlaceList(QList<Place *> *placeList)
{
    m_placeList = placeList;
}

void SourceManager::initInstance()
{
    if(!m_instance)
        m_instance = new SourceManager;
}

void SourceManager::freeInstance()
{
    if (m_instance) {
        delete m_instance;
        m_instance = nullptr;
    }
}

SourceManager *SourceManager::instance()
{
    return m_instance;
}

void SourceManager::initLocaPlace()
{
    getPlaceList()->clear();

    LauncherPlace *lPlace = new LauncherPlace();
    lPlace->setStreamName("release");
    lPlace->setGuid("release");
    lPlace->init();
    getPlaceList()->append(lPlace);

    QList<UserSession::Version> * userVersions =  UserSession::instance()->versions();

    for(UserSession::Version elem : *userVersions){
        GamePlace *rPlace = new GamePlace();
        rPlace->setStreamName(elem.stream);
        rPlace->setTorrentUrl(elem.url);
        rPlace->setGuid(elem.guid);
        rPlace->init();
        getPlaceList()->append(rPlace);
    }

}

void SourceManager::clearLocalSpace()
{
    for(Place * pl : *getPlaceList()){
        delete pl;
    }
    getPlaceList()->clear();
}

void SourceManager::initialization()
{
}

void SourceManager::checkLocalGamePlace()
{
    for(Place * pl : *getPlaceList()){
        if(pl->isGame())
            (dynamic_cast<GamePlace*>(pl))->refindLocal();
    }
}

bool SourceManager::isPlay()
{
    for(Place * pl : *getPlaceList()){
        if(pl->isGame() && (dynamic_cast<GamePlace*>(pl))->isPlay())
            return true;
    }
    return false;
}

bool SourceManager::existsUpdateProcessGame()
{
    for(Place * pl : *getPlaceList()){
        if(pl->isGame() && (dynamic_cast<GamePlace*>(pl))->getState() == PlaceState::UpdateProcess)
            return true;
    }
    return false;
}

bool SourceManager::existsInstallProcessGame()
{
    for(Place * pl : *getPlaceList()){
        if(pl->isGame() && (dynamic_cast<GamePlace*>(pl))->getState() == PlaceState::InstallProcess)
            return true;
    }
    return false;
}

bool SourceManager::existsHashCkeckingProcessGame()
{
    for(Place * pl : *getPlaceList()){
        if(pl->isGame() && (dynamic_cast<GamePlace*>(pl))->getState() == PlaceState::HashChecking)
            return true;
    }
    return false;
}

QList<Place *> SourceManager::getGameAvailableVersions()
{
    if(getPlaceList() == nullptr && getPlaceList()->length() == 0)
        return QList<Place *>();

    QList<Place *> list;

    for(Place *pl : *getPlaceList())
        if(pl->isGame())
            list.append(pl);

    return list;
}

Place* SourceManager::getSource(QString guid, bool isGame)
{
    for(Place *source : *getPlaceList()){
        if((source->getGuid() == guid) && (source->isGame() == isGame)){
            return source;
        }
    }
    return nullptr;
}

Place* SourceManager::getSourceLauncher()
{
    for(Place *source : *getPlaceList()){
        if(!source->isGame()){
            return source;
        }
    }
    return nullptr;
}

void SourceManager::sourceDownloadComplete(QString name)
{
    for(Place *source : *getPlaceList()){
        if(source->torrentName() == name){
            source->installComplete();
        }
    }

}

void SourceManager::handleStartInstallGame(SourceInfo *si)
{
    emit onInstallGameProgress();
}
