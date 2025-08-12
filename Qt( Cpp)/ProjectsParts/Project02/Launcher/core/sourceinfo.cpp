#include "sourceinfo.h"

#include "core/place.h"
#include "base/bittorrent/addtorrentparams.h"
#include "launcher.h"
#include "core/update/inieditor.h"
#include "config.h"
#include "base/bittorrent/session.h"
#include "base/utils/fs.h";
#include "core/installstate.h"
#include "base/bittorrent/infohash.h"

using namespace Core;

SourceInfo::SourceInfo(Core::Place *place)
    : m_place(place)
    , m_isDev(false)
{}

QString SourceInfo::runFile() const
{
    return m_runFile;
}

void SourceInfo::setRunFile(const QString &runFile)
{
    m_runFile = runFile;
}

bool SourceInfo::isInstalled() const
{
    return m_isInstalled;
}

void SourceInfo::setIsInstalled(bool isInstalled)
{
    m_isInstalled = isInstalled;
}

SourceInfo::InstallState SourceInfo::installState() const
{
    return m_installState;
}

void SourceInfo::setInstallState(const InstallState &installState)
{
    m_installState = installState;

    if(installState != InstallState::Source)
        writeVersionFile(savePath());
}

QString SourceInfo::torrentFile() const
{
    return m_torrentFile;
}

void SourceInfo::setTorrentFile(const QString &torrentFile)
{
    m_torrentFile = torrentFile;
}

QByteArray SourceInfo::byteArray() const
{
    return m_byteArray;
}

void SourceInfo::setByteArray(const QByteArray &byteArray)
{
    setTorrentInfo(BitTorrent::TorrentInfo::load(byteArray));
    m_byteArray = byteArray;
}

BitTorrent::TorrentInfo SourceInfo::torrentInfo() const
{
    return m_torrentInfo;
}

void SourceInfo::setTorrentInfo(const BitTorrent::TorrentInfo &torrentInfo)
{
    m_torrentInfo = torrentInfo;
}

bool SourceInfo::installGame(bool isUpdate)
{
    Utils::Fs::removeNoListFile(savePath(),m_torrentInfo.rootFolder(),torrentInfo().filePaths(), QStringList() << Config::GAME_INI_FILE << Config::GAME_TORRENT_FILE );

    setIsInstalled(true);
    setInstallState(isUpdate ? InstallState::Update : InstallState::Install);
    playTorrent();

    if(getPlace()->isGame())
        SourceManager::instance()->handleStartInstallGame(this);

    return true;
}

void SourceInfo::playTorrent(bool isResume)
{

    if(BitTorrent::Session::instance()->findTorrent(torrentInfo().hash()) != nullptr)
        return;

    BitTorrent::AddTorrentParams params;
    params.name = getPlace()->torrentName();
    params.version = GetVersion();
    params.savePath = savePath();
    params.sequential = true;
    params.skipChecking = isResume;

    BitTorrent::Session::instance()->deleteTorrent(torrentInfo().hash(),false);
    BitTorrent::Session::instance()->addTorrent(params,byteArray());
}

void SourceInfo::update()
{
    //SourceManager::instance()->updateLocalVersion(this);
    installGame(true);
}


void SourceInfo::writeVersionFile(QString dir)
{
    IniEditor ini(dir+"/" + m_torrentInfo.rootFolder()+ "/" + Config::GAME_INI_FILE);
    ini.write("name",getPlace()->getStreamName());
    ini.write("runFile",runFile());
    ini.write("version",version());
    ini.write("state",installState());
    ini.write("note",noteUrl());
    ini.sync();
}

QString SourceInfo::savePath()
{
    return getPlace()->isGame()
            ? Launcher::instance()->getInstallPath()
            : Config::TMP_PATH_ROOT + "/launcher";
}

QString SourceInfo::exeFile()
{
    return savePath() + "/" +torrentInfo().rootFolder() + runFile();
}

void SourceInfo::remove()
{
    BitTorrent::Session::instance()->deleteTorrent(torrentInfo().hash());

    QFile tr(torrentFile());
    if(tr.exists())
        tr.remove();
}

SourceInfo *SourceInfo::clone()
{
    SourceInfo* source = new SourceInfo(getPlace());
    source->setVersion(version());
    source->setRunFile(runFile());
    source->setTorrentFile(torrentFile());
    source->setNoteUrl(noteUrl());
    source->setIsDev(isDev());
    source->setInstallState(installState());
    source->setIsInstalled(isInstalled());
    source->setByteArray(byteArray());
    source->setTorrentInfo(torrentInfo());

    return source;
}

QString SourceInfo::displayVersionNull()
{
    return QString("0.0.0");
}

QString SourceInfo::displayVersion()
{
    return version();
}

bool SourceInfo::firstGreatOrEqualsSecond(SourceInfo *first, SourceInfo *second, int versionBlock)
{
    QStringList firstVersions = first->versionBlocks();
    QStringList secondVersions = second->versionBlocks();

    if(firstVersions.length() > versionBlock && secondVersions.length() > versionBlock){
        if(firstVersions[versionBlock].toLong() != secondVersions[versionBlock].toLong())
            return firstVersions[versionBlock].toLong() >= secondVersions[versionBlock].toLong();
        else
            return SourceInfo::firstGreatOrEqualsSecond(first, second, ++versionBlock);
    }else
        return firstVersions.length() >= secondVersions.length();
}

bool SourceInfo::firstGreatSecond(SourceInfo *first, SourceInfo *second, int versionBlock)
{
    QStringList firstVersions = first->versionBlocks();
    QStringList secondVersions = second->versionBlocks();

    if(firstVersions.length() > versionBlock && secondVersions.length() > versionBlock){
        if(firstVersions[versionBlock].toLong() != secondVersions[versionBlock].toLong())
            return firstVersions[versionBlock].toLong() > secondVersions[versionBlock].toLong();
        else
            return SourceInfo::firstGreatSecond(first, second, ++versionBlock);
    }else
        return firstVersions.length() > secondVersions.length();
}

bool SourceInfo::firstGreatSecond(SourceInfo *first, QString second, int versionBlock)
{
    QStringList firstVersions = first->versionBlocks();
    QStringList secondVersions = second.split(".");

    if(firstVersions.length() > versionBlock && secondVersions.length() > versionBlock){
        if(firstVersions[versionBlock].toLong() != secondVersions[versionBlock].toLong())
            return firstVersions[versionBlock].toLong() > secondVersions[versionBlock].toLong();
        else
            return SourceInfo::firstGreatSecond(first, second, ++versionBlock);
    }else
        return firstVersions.length() > secondVersions.length();
}

QString SourceInfo::runFileFullPath()
{
    return savePath() + "/" + m_torrentInfo.rootFolder() + runFile();
}

bool SourceInfo::isDev() const
{
    return m_isDev;
}

void SourceInfo::setIsDev(const bool &isDev)
{
    m_isDev = isDev;
}

QString SourceInfo::noteUrl() const
{
    return m_noteUrl;
}

void SourceInfo::setNoteUrl(const QString &noteUrl)
{
    m_noteUrl = noteUrl;
}

QString SourceInfo::rootFolder()
{
    return savePath() + "/" + m_torrentInfo.rootFolder();
}

void SourceInfo::checkState()
{

}

SourceInfo::InstallState SourceInfo::getState()
{
    return m_state;
}

Core::Place *SourceInfo::getPlace() const
{
    return m_place;
}

void SourceInfo::setPlace(Core::Place *place)
{
    m_place = place;
}

QString SourceInfo::version() const
{
    return m_version;
}

QStringList SourceInfo::versionBlocks() const
{
    return version().split(".");
}

void SourceInfo::setVersion(const QString &version)
{
    m_version = version;
}
