#ifndef RELEASE_H
#define RELEASE_H

#include <QJsonDocument>
#include <QJsonObject>
#include <QList>
#include <QNetworkReply>
#include <QProcess>
#include <QString>
#include "../../general/appbase.h"
#include "../../general/tag.h"
#include "fileinfo.h"
#include "releasestate.h"
#include "releasetag.h"

namespace Core {
class Release : public QObject
{
    Q_OBJECT
public:
    Release();
    Release(QJsonObject jObject);

    QJsonObject jObject;
    qint64 id;
    QString type;
    QString version;
    QString checksum;
    QString description;
    QDateTime createAt;
    FileInfo file;
    QList<int> tagsIds;
    QList<Core::ReleaseTag *> tags;
    QString data;
    QJsonObject propertyData;
    bool IsLocalOnly = false;

    //! Путь к файлу архива
    const QString zipFile() const;
    virtual const QString installPath() const;
    const QString logPath() const;
    virtual const QString exePath() const;
    const QString versionFile();
    const QString infoFilePath() const;
    virtual bool isRunned();

    static void downloadStreaminCallback(QNetworkReply *reply);
    static void progressCallback(double value, double all);
    static void setIsDowloadProcess(bool newIsDowloadProcess);

    static void setAppBase(General::AppBase *newAppBase);

    QString toJsonString();
    Release *getStreamingAssets();
    virtual const bool existsDisk();
    const bool existsZip();
    const bool isRunning();
    const bool isVideowall();
    const bool isLauncher();
    const bool isCross();
    virtual void download(const std::function<void(Release *)> &onComplete = nullptr,
                          const std::function<void(qint64, qint64)> &progress = nullptr,
                          const std::function<void(QString)> &onError = nullptr);
    virtual const bool isDownload();
    void setDownloadProgress(double progress);
    void downloadFinish(QNetworkReply *reply);
    void recordZipFile(QByteArray byte);
    void emitDownloadProgress();
    virtual void unpack(Release *parentRelease = nullptr);
    virtual void afterUnpack();
    virtual bool checkState();
    void recordInfoFile();
    void setIsDownloadProcess(bool newDownloadProcess);
    void recordVersionFile();
    void readVersionFile();
    QDateTime birthTime();
    bool existsTag(QString tag);
    virtual void afterCreate();

    Q_INVOKABLE const bool isStreamingAsset();
    Q_INVOKABLE void remove();
    Q_INVOKABLE void openFolder();
    Q_INVOKABLE void openLogFolder();
    Q_INVOKABLE const int getId();
    Q_INVOKABLE const QString getVersion();
    Q_INVOKABLE const QString getDescription();
    Q_INVOKABLE void favoriteToggle();
    Q_INVOKABLE bool downloadProcess() const;

    Q_PROPERTY(Core::ReleaseState::State state READ getState NOTIFY stateChangeSignal)
    Q_PROPERTY(double fullDownloadProgress READ getFullDownloadProgress NOTIFY downloadProgressSignal)
    Q_PROPERTY(double isFavorite READ getIsFavorite NOTIFY switchfavoriteSignal)

    //! Возвращает состояние
    Core::ReleaseState::State getState() const;
    virtual float getFullDownloadProgress();
    float getDownloadProgress();

    bool getIsFavorite() const;
    void setIsFavorite(bool newIsFavorite);
    bool isForSumLite() const;

private:
    //void downloadAnyCallback();
    void savePropertyData();

signals:

    void onLoadStart();
    void onLoadComplete();
    void downloadProgressSignal();
    void emitDownloadError(QString error);
    void stateChangeSignal();
    void switchfavoriteSignal();

public slots:
    void unpackOutput();

protected:
    float _downloadProgress = 0;
    QProcess *_unpackProcess;
    //! Установка нового состояния
    void setState(ReleaseState::State newState);

private:
    bool _isDownloadProcess = false;
    ReleaseState::State _state;

    static Release *_dowloadItem;
    static QList<General::Tag> *_tagList;
    static bool _isDowloadProcess;
};

} // namespace Core
// Q_DECLARE_METATYPE(Core::Release)
#endif // RELEASE_H
