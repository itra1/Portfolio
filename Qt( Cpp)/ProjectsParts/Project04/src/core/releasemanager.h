#ifndef RELEASEMANAGER_H
#define RELEASEMANAGER_H

#include <QBasicTimer>
#include <QObject>
#include <QQueue>
#include "releaseloader.h"
#include "releases/release.h"

namespace Core {
class ReleaseManager : public QObject
{
    Q_OBJECT
public:
    ReleaseManager(const ReleaseManager &) = delete;
    ReleaseManager &operator=(const ReleaseManager &) = delete;

    explicit ReleaseManager(Core::ReleaseLoader *releaseLoader);
    static void initInstance(Core::ReleaseLoader *releaseLoader);
    static void freeInstance();
    static ReleaseManager *instance();

    void ReadInstallReleases();
    void ReadInstallDirRelease(QString folderName);
    void afterCreate();
    void checkAllStates();
    void autoPlayReleaseWithSumlite();
    void clear();
    void addTagIfNeed(Core::ReleaseTag *tag);
    Core::Release *getReleaseByType(QJsonObject jObject);
    Core::Release *getReleaseByType(QString type);
    int noLocalOnlyReleases();

    Q_INVOKABLE QVariantList getVideowallList();
    Q_INVOKABLE QVariantList getVideowallList(bool isFavorite, QString tag);
    Q_INVOKABLE QVariantList getTagsList();
    Q_INVOKABLE void downloadItem(Release *item);
    Q_PROPERTY(QString errorMessage READ getErrorMessage NOTIFY onErrorMessageSignal)

    //static bool maxVersion(QString v1, QString v2, bool orEquals = false);

    QList<Core::Release *> *getReleasesList() const;
    QList<Core::Release *> *getReleasesList(QString type) const;
    Core::Release *getReleaseById(QString version);

    const QString &getErrorMessage() const;
    void setErrorMessage(const QString &newErrorMessage);

public slots:
    void loadReleases();
    void authChange(bool isLogin);

signals:
    void releaseLoadSignal();
    void onErrorMessageSignal();
    void chackAuthComplete(bool result, QString message);

private:
    static ReleaseManager *_instance;
    Core::ReleaseLoader *_releaseLoader;
    QList<Core::Release *> *_releasesList;
    QList<Core::ReleaseTag *> *_releaseTags;
    QString _errorMessage;

    void afterLoadReleases();
    void emitErrorMessage();

    Core::Release *getPresentationRelease();
};
} // namespace Core
#endif // RELEASEMANAGER_H
