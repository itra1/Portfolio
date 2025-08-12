#ifndef UPDATER_H
#define UPDATER_H

#include <QObject>
#include "releaseloader.h"
#include "releases/release.h"

namespace Core {

class Updater : public QObject
{
	Q_OBJECT
	public:
	Updater(const Updater &) = delete;
	Updater &operator=(const Updater &) = delete;

    explicit Updater(Core::ReleaseLoader *loader);
    static void initInstance(Core::ReleaseLoader *loader);
    static void freeInstance();
    static Updater *instance();

    Q_INVOKABLE void update();
	Q_INVOKABLE void checkUpdate();
	Q_INVOKABLE void qmlLoaded();

	Q_PROPERTY(QString infoText READ getInfoText NOTIFY infoChangeSignal)
	Q_PROPERTY(bool updateReady READ updateReady WRITE setUpdateReady NOTIFY updateReadyChange FINAL)
	Q_PROPERTY(QString updateVersion READ updateVersion WRITE setUpdateVersion NOTIFY
								 updateReadyChange FINAL)

	void downloadUpdate(Release *item);
	void setCopyUpdate();
	void setCopyUpdate(QString sourcePath, QString targetPath);
    void StartUpdate();
    void copyDir(QString source, QString target);
	void clearTempData();

	const QString getInfoText();

	void setInfoText(QString newInfoText);

	bool updateReady() const;
	void setUpdateReady(bool newUpdateReady);

	QString updateVersion() const;
	void setUpdateVersion(const QString &newUpdateVersion);

	signals:
	void infoChangeSignal();
	void updateReadyChange();

	public slots:
	void releaseLoaded();

	private:
	static Updater *_instance;

    Core::ReleaseLoader *_releaseLoader;
    bool _visibleInterface;
    bool _updateReady{false};
    QString _infoText;
    QString _sourcePath;
	QString _targetPath;
	QString _updateVersion;

	void emitUpdateReadyChange();
};
} // namespace Core
#endif // UPDATER_H
