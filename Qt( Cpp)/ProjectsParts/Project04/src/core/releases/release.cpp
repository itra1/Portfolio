#include "release.h"
#include <QDateTime>
#include <QDesktopServices>
#include <QDir>
#include <QFile>
#include <QJsonArray>
#include "../../config/config.h"
#include "../../general/tag.h"
#include "../api.h"
#include "../helpers/releasehelper.h"
#include "../releasemanager.h"
#include "quazip/JlCompress.h"
#include "releasestate.h"

using namespace Core;

QList<General::Tag> *Release::_tagList;
Release *Release::_dowloadItem;
bool Release::_isDowloadProcess;

Release::Release()
    : QObject(nullptr)
{}

Release::Release(QJsonObject jObject)
    : QObject(nullptr)
    , _state(ReleaseState::None)
{
    this->jObject = jObject;
    this->id = jObject.value("id").toInt();
    this->version = jObject.value("version").toString();
    this->checksum = jObject.value("checksum").toString();
    this->description = jObject.value("description").toString();
    this->type = jObject.value("type").toString();
    this->createAt = QDateTime::fromString(jObject.value("createdAt").toString(), Qt::ISODate);

    auto file{jObject.value("file").toObject()};

    this->file.fieldname = file.value("fieldname").toString();
    this->file.originalname = file.value("originalname").toString();
    this->file.encoding = file.value("encoding").toString();
    this->file.mimetype = file.value("mimetype").toString();
    this->file.destination = file.value("destination").toString();
    this->file.filename = file.value("filename").toString();
    this->file.size = file.value("size").toInt();
    this->file.url = file.value("url").toString();
    this->file.src = file.value("src").toString();
    this->file.title = file.value("title").toString();

    QJsonArray jArr{jObject.value("tags_ids").toArray()};

    tagsIds.clear();
    for (int i = 0; i < jArr.count(); i++) {
        tagsIds.append(jArr[i].toInt());
    }

    QJsonArray jTagsArr{jObject.value("tags").toArray()};

    tags.clear();
    for (int i = 0; i < jTagsArr.count(); i++) {
        Core::ReleaseTag *releaseTag = new Core::ReleaseTag();
        releaseTag->parse(jTagsArr[i].toObject());
        tags.append(releaseTag);
    }

    propertyData = jObject.value("data").isString()
                       ? QJsonDocument::fromJson(jObject.value("data").toString().toLatin1()).object()
                       : jObject.value("data").toObject();
}

const QString Release::getDescription()
{
    return description;
}

const QString Release::getVersion()
{
    return version;
}

const int Release::getId()
{
    return id;
}
const QString Release::zipFile() const
{
    return Config::getPath(ConfigKeys::downloadPath) + QString("/%1_%2.zip").arg(type).arg(version);
}

const QString Release::installPath() const
{
    return Config::getPath(ConfigKeys::installPath) + QString("/%1_%2").arg(type).arg(version);
}

const QString Release::logPath() const
{
    return installPath() + "\\log";
}

const QString Release::exePath() const
{
    return installPath() + "/" + Config::getStringValue(ConfigKeys::videoWallExe);
}

const QString Release::versionFile()
{
    return installPath() + "/" + (isStreamingAsset() ? "streamings" : "version");
}

const QString Release::infoFilePath() const
{
    return installPath() + "/releaseInfo";
}

bool Release::isRunned()
{
    return false;
}

// void Release::downloadAnyCallback()
// {
//     if (isDownload()) {
//         _releaseLoader->SetIsDownloadProcess(false);
//         _releaseLoader->NextDownload();
//     }
// }

void Release::downloadStreaminCallback(QNetworkReply *reply)
{
    _dowloadItem->downloadFinish(reply);
}

void Release::setIsDowloadProcess(bool newIsDowloadProcess)
{
    _isDowloadProcess = newIsDowloadProcess;
}

QString Release::toJsonString()
{
    return QString(QJsonDocument(jObject).toJson(QJsonDocument::Compact));
}

Release *Release::getStreamingAssets()
{
    Release *item = nullptr;

    auto releases = ReleaseManager::instance()->getReleasesList();

    for (int i = 0; i < releases->count(); i++) {
        Release *view = releases->value(i);

        if (!view->isStreamingAsset())
            continue;
        if (!Core::ReleaseHelper::MaxVersion(this->version.split(QLatin1Char('_'))[0],
                                             view->version.split(QLatin1Char('_'))[0],
                                             true))
            continue;

        if ((item == nullptr
             || Core::ReleaseHelper::MaxVersion(view->version.split(QLatin1Char('_'))[0],
                                                item->version.split(QLatin1Char('_'))[0],
                                                true)))
            item = view;
    }
    return item;
}

const bool Release::existsDisk()
{
    return QFile::exists(exePath());
}

const bool Release::existsZip()
{
    return QFile::exists(zipFile());
}

const bool Release::isRunning()
{
    return getState() == ReleaseState::Played;
}

const bool Release::isStreamingAsset()
{
    return type == Config::getStringValue(ConfigKeys::streamingAssetsType);
}

const bool Release::isVideowall()
{
    return type == Config::getStringValue(ConfigKeys::videoWallType);
}

const bool Release::isLauncher()
{
    return type == Config::getStringValue(ConfigKeys::launcherType);
}

const bool Release::isCross()
{
    return type == Config::getStringValue(ConfigKeys::crossType);
}

void Release::download(const std::function<void(Release*)> &onComplete,
                       const std::function<void(qint64, qint64)> &progress,
                       const std::function<void(QString)> &onError)
{
    QString targetUrl = file.url;
    qDebug() << "Start download " << targetUrl;
    _isDownloadProcess = true;
    setDownloadProgress(0.0);

    if (existsZip()) {
        setDownloadProgress(1.0);
        onComplete(this);
        return;
    }
    setState(ReleaseState::Loading);

    Core::Api::loadRelease(
        targetUrl,
        [=](bool complete, QNetworkReply *reply) {
            if (!complete){
                onError(reply->errorString());
                return;
            }
            downloadFinish(reply);
            //downloadAnyCallback();
            checkState();
            onComplete(this);
        },
        [=](qint64 p1, qint64 p2) {
            setDownloadProgress((double) p1 / (double) p2);
            if (progress != nullptr)
                progress((double) p1, (double) p2);
        });
}

const bool Release::isDownload()
{
    return existsZip();
}

void Release::setDownloadProgress(double progress)
{
    _downloadProgress = progress;
    emitDownloadProgress();
}

void Release::downloadFinish(QNetworkReply *reply)
{
    if (reply->error()) {
        emit emitDownloadError("Ошибка загрузки файла");
        qDebug() << "Ошибка загрузки файла";
        return;
    }

    recordZipFile(reply->readAll());
}

void Release::recordZipFile(QByteArray byte)
{
    QString pathWrite(Config::getPath(ConfigKeys::downloadPath));

    if (!QDir(pathWrite).exists())
        QDir(pathWrite).mkdir(pathWrite);

    QFile *file = new QFile(zipFile());
    // Создаём файл или открываем его на перезапись ...
    if (file->open(QFile::WriteOnly)) {
        file->write(byte); // ... и записываем всю информацию со страницы в файл
        file->close(); // закрываем файл
        qDebug() << "Record complete";
    }
}

void Release::emitDownloadProgress()
{
    emit downloadProgressSignal();
}

void Release::unpack(Release *parentRelease)
{
    setState(ReleaseState::Unpack);
    QString path(installPath());
    JlCompress::extractDir(zipFile(), path);
    afterUnpack();
}

void Release::afterUnpack() {}

bool Release::checkState()
{
    if (existsDisk()) {
        setState(ReleaseState::Installed);
        return true;
    }
    if (isDownload()) {
        setState(ReleaseState::Downloaded);
        return true;
    }
    if (getState() == ReleaseState::Loading) {
        setState(ReleaseState::Loading);
        return true;
    }

    setState(ReleaseState::None);
    return false;
}

bool Release::isForSumLite() const
{
    auto key = Config::getStringValue(ConfigKeys::sumAdaptive_buildKey);

    return propertyData.contains(key) ? propertyData.value(key).toBool() : false;
}

void Release::remove()
{
    if (QDir(installPath()).removeRecursively())
        checkState();
}

void Release::openFolder()
{
    QDesktopServices::openUrl(QUrl::fromLocalFile(installPath()));
}

void Release::openLogFolder()
{
    QDir dir(logPath());
    if (dir.exists()) {
        QDesktopServices::openUrl(QUrl::fromLocalFile(logPath()));
    } else {
        openFolder();
    }
}

void Release::favoriteToggle()
{
    setIsFavorite(!getIsFavorite());
}

void Release::recordInfoFile()
{
    QFile *file = new QFile(infoFilePath());
    QJsonDocument doc(jObject);
    // Создаём файл или открываем его на перезапись ...
    if (file->open(QFile::WriteOnly)) {
        file->write(doc.toJson(QJsonDocument::Compact)); // ... и записываем всю информацию со
                                                         // страницы в файл
        file->close();
    }
}

bool Release::downloadProcess() const
{
    return _isDownloadProcess;
}

void Release::setIsDownloadProcess(bool newDownloadProcess)
{
    _isDownloadProcess = newDownloadProcess;
}

void Release::recordVersionFile()
{
    if (QFile(versionFile()).exists(versionFile()))
        return;

    QFile *file = new QFile(versionFile());
    if (file->open(QFile::WriteOnly)) {
        file->write(toJsonString().toUtf8());
        file->close();
    }
}

void Release::readVersionFile()
{
    if (!QFile(versionFile()).exists(versionFile()))
        return;
    QFile *file = new QFile(versionFile());
    if (file->open(QFile::ReadOnly)) {
        QJsonDocument document = QJsonDocument::fromJson(file->readAll());
        file->close();
    }
}

QDateTime Release::birthTime(){
    return QFileInfo(exePath()).fileTime(QFile::FileBirthTime).toUTC();
}

bool Release::existsTag(QString tag)
{
    for (int i = 0; i < tags.count(); i++) {
        if (tags.value(i)->name() == tag)
            return true;
    }
    return false;
}

void Release::afterCreate() {}

Core::ReleaseState::State Release::getState() const
{
    return _state;
}

void Release::setState(ReleaseState::State newState)
{
    _state = newState;
    emit stateChangeSignal();
}

void Release::unpackOutput() {}

bool Release::getIsFavorite() const
{
    auto key = Config::getStringValue(ConfigKeys::favoriteBuildKey);
    return propertyData.contains(key) ? propertyData.value(key).toBool() : false;
}

void Release::setIsFavorite(bool newIsFavorite)
{
    propertyData[Config::getStringValue(ConfigKeys::favoriteBuildKey)] = newIsFavorite;

    savePropertyData();
    emit switchfavoriteSignal();
}

float Release::getDownloadProgress()
{
    return _downloadProgress;
}

float Release::getFullDownloadProgress()
{
    return _downloadProgress;
}

void Release::savePropertyData()
{
    QJsonDocument jDocument(propertyData);

    QString url = "/releases/data/" + QString::number(id);

    // Core::Network::Request(Core::Application::serverAuthUrl(url),
    // 											 General::Authorization::getTokenAuth(),
    // 											 RequestType::Patch,
    // 											 jDocument,
    // 											 [](QNetworkReply *reply) {});
}
