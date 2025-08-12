#include "place.h"

#include <QTimer>
#include "core/sourcemanager.h"
#include "config.h"
#include "base/net/downloadhandler.h"
#include "base/utils/fs.h"
#include "update/inieditor.h"
#include "core/launcher.h"
#include "base/bittorrent/session.h"

using namespace Core;

Place::Place(QObject *parent)
    : QObject(parent)
    ,m_sourceLocal(nullptr)
    ,m_sourceServer(nullptr)
    ,m_firstInit(true)
{

}

void Place::init()
{
    findLocalSource();
    downloadInfo();
}

void Place::findLocalSource()
{
    m_sourceLocal = new SourceInfo(this);

    QStringList subdirList = QDir(m_sourceLocal->savePath()).entryList(QDir::Dirs);

    bool exists = false;

    for(QString subdir : subdirList){

        if(subdir == "." || subdir == "..")
            continue;

        QFile sourceFile(Utils::Fs::toNativePath(m_sourceLocal->savePath()+ "/"+subdir+"/source.ini"));
        QFile torrentFile(Utils::Fs::toNativePath(m_sourceLocal->savePath()+ "/"+subdir+"/source.res"));

        if(!sourceFile.exists() || !torrentFile.exists()){
            continue;
        }

        IniEditor ini(sourceFile.fileName());

        m_sourceLocal->setIsDev(ini.read("develop",0).toInt() == 1);

        if(isGame() && getStreamName() != ini.read("name").toString())
            continue;

        exists = true;

        m_sourceLocal->setVersion(ini.read("version").toString());
        m_sourceLocal->setNoteUrl(ini.read("note","").toString());

        torrentFile.open(QIODevice::ReadOnly);
        m_sourceLocal->setByteArray(torrentFile.readAll());
        torrentFile.close();

        m_sourceLocal->setTorrentFile(torrentFile.fileName());

        m_sourceLocal->setRunFile(ini.read("runFile").toString() );
        m_sourceLocal->setIsInstalled(true);

        if(ini.hash("state"))
            m_sourceLocal->setInstallState(static_cast<SourceInfo::InstallState>(ini.read("state").toInt()));
        else {
            m_sourceLocal->setInstallState(SourceInfo::Install);
        }

    }

    if(m_sourceLocal != nullptr && !exists){
        BitTorrent::Session::instance()->deleteTorrent(m_sourceLocal->torrentInfo().hash(),false);
    }
    if(!exists){
        m_sourceLocal = nullptr;
    }

    if(!m_firstInit)
        checkState();
}

void Place::downloadInfo()
{
    qDebug() << "downloadInfo " + getInfoUrl();

    Net::DownloadHandler *handler = Net::DownloadManager::instance()->download(Net::DownloadRequest(getInfoUrl()));
    connect(handler, static_cast<void (Net::DownloadHandler::*)(const QString &, const QByteArray &)>(&Net::DownloadHandler::downloadFinished)
            , this, &Place::handleDownloadInfoComplete);
    connect(handler, &Net::DownloadHandler::downloadFailed, this, &Place::handleDownloadInfoFailed);
}

void Place::handleDownloadInfoComplete(const QString &url, const QByteArray &data)
{
    m_sourceServer = new SourceInfo(this);

    QString filePath = QString("%1/%2-%3").arg(Config::TMP_PATH_ROOT).arg(typeToString()).arg(getStreamName());

    if(!QDir().exists(filePath))
        QDir().mkpath(filePath);

    QString writeFile = Utils::Fs::toNativePath(filePath + "/source.ini");

    QFile saveFile(writeFile);

    if(saveFile.exists())
       saveFile.remove();

    saveFile.open(QIODevice::WriteOnly);
    saveFile.write(data);
    saveFile.close();

    IniEditor ini(writeFile);

    m_sourceServer->setVersion(ini.read("version").toString());
    m_sourceServer->setNoteUrl(ini.read("note","").toString());

    m_sourceServer->setIsInstalled(false);

    m_sourceServer->setRunFile(ini.read("runFile").toString().replace("\\","\\"));
    m_sourceServer->setInstallState(SourceInfo::Source);

    downloadTorrent();
}

void Place::handleDownloadInfoFailed(const QString &url, const QString &reason)
{
    QTimer::singleShot(5000,this,&Place::downloadInfo);
    if(m_firstInit){
        m_firstInit = false;
        checkState();
    }
}

void Place::downloadTorrent()
{
    Net::DownloadHandler *handler = Net::DownloadManager::instance()->download(Net::DownloadRequest(getTorrentUrl()));
    connect(handler, static_cast<void (Net::DownloadHandler::*)(const QString &, const QByteArray &)>(&Net::DownloadHandler::downloadFinished)
            , this, &Place::handleDownloadTorrentComplete);
    connect(handler, &Net::DownloadHandler::downloadFailed, this, &Place::handleDownloadTorrentFailed);
}
void Place::handleDownloadTorrentComplete(const QString &url, const QByteArray &data)
{
    m_sourceServer->setByteArray(data);

    m_firstInit = false;
    checkState();
}

void Place::handleDownloadTorrentFailed(const QString &url, const QString &reason)
{
    QTimer::singleShot(5000,this,&Place::downloadTorrent);
    if(m_firstInit){
        m_firstInit = false;
        checkState();
    }
}

QString Place::getGuid() const
{
    return m_guid;
}

void Place::setGuid(const QString &guid)
{
    m_guid = guid;
}

QString Place::getStreamName() const
{
    return m_streamName;
}

void Place::setStreamName(const QString &streamName)
{
    m_streamName = streamName;
}

Place::~Place()
{
    if(m_sourceLocal != nullptr && m_sourceLocal->torrentInfo().hash() != nullptr)
        BitTorrent::Session::instance()->deleteTorrent(m_sourceLocal->torrentInfo().hash());
    if(m_sourceServer != nullptr && m_sourceServer->torrentInfo().hash() != nullptr)
        BitTorrent::Session::instance()->deleteTorrent(m_sourceServer->torrentInfo().hash());
}

SourceInfo *Place::getSourceServer() const
{
    return m_sourceServer;
}

void Place::setSourceServer(SourceInfo *sourceServer)
{
    m_sourceServer = sourceServer;
}

void Place::installComplete()
{
    m_sourceLocal->setIsInstalled(true);
    m_sourceLocal->setInstallState(SourceInfo::InstallState::Complete);
    findLocalSource();
}

SourceInfo *Place::getSourceLocal() const
{
    return m_sourceLocal;
}

void Place::setSourceLocal(SourceInfo *sourceLocal)
{
    m_sourceLocal = sourceLocal;
}

PlaceState::State Place::getState() const
{
    return m_state;
}

void Place::setState(const PlaceState::State &state)
{
    m_state = state;
    emit onStateChange(m_state);
    Core::Launcher::instance()->setState(isGame(),getGuid(),m_state);
}

bool Place::isProcessState()
{
    return (m_state & PlaceState::Process) > 0;
}

QString Place::torrentName()
{
    return typeToString()+"-"+getGuid();
}

QString Place::typeToString()
{
    return isGame() ? "game" : "launcher";
}

QString Place::getInfoUrl()
{
    return QString("%1/%2").arg(getSourceUrl()).arg(Config::SERVER_FILE_INFO);
}

QString Place::getTorrentUrl()
{
    return QString("%1/%2").arg(getSourceUrl()).arg(Config::SERVER_FILE_TORRENT);
}
