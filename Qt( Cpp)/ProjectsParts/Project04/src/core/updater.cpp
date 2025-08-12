#include "updater.h"
#include "../config/config.h"
#include "../general/settingsstorage.h"
#include "application.h"
#include "helpers/releasehelper.h"
#include "quazip/JlCompress.h"
#include "releasemanager.h"
#include "session.h"

namespace Core
{

Updater *Updater::_instance = nullptr;
Updater::Updater(Core::ReleaseLoader *loader)
    : QObject(nullptr)
    , _releaseLoader{loader}
{
	connect(ReleaseManager::instance(), SIGNAL(releaseLoadSignal()), SLOT(releaseLoaded()));
}

void Updater::initInstance(Core::ReleaseLoader *loader)
{
    if (!_instance)
        _instance = new Updater(loader);
}

void Updater::freeInstance()
{
    if (_instance) {
        delete _instance;
        _instance = nullptr;
    }
	}

	Updater *Updater::instance()
	{
		return _instance;
	}

	void Updater::update()
	{
		qDebug() << "update";

		Core::Application::instance()->setAppState(Core::AppPhase::State::UpdateCheck);

		auto release = Core::ReleaseManager::instance()->getReleaseById(_updateVersion);
        release->download(
            [&, release](Release *releas) {
                qDebug() << "Update file Load";
                downloadUpdate(release);
            },
            nullptr);
    }

    void Updater::checkUpdate()
    {
		// В режиме презентации отключено обновление
		if (Core::Session::instance()->isPresentationMode()) {
			//Application::instance()->updateComplete();
			return;
		}

		setInfoText(QString("Проверка наличия обновления"));

		QString version = APP_VERSION;
		Release *rec = nullptr;

		auto releaseList = ReleaseManager::instance()->getReleasesList();

		for (int i = 0; i < releaseList->count(); i++) {
            if (releaseList->value(i)->type == Config::getStringValue(ConfigKeys::launcherType)
                && Core::ReleaseHelper::MaxVersion(releaseList->value(i)->version.split(
                                                       QLatin1Char('_'))[0],
                                                   version.split(QLatin1Char('_'))[0])) {
                version = releaseList->value(i)->version;
				rec = releaseList->value(i);
            }
        }

		if (version != APP_VERSION && rec != nullptr) {
			setInfoText(QString("Загрузка обновления"));
			qDebug() << "Загрузка обновления " + version;
			_updateVersion = version;
			setUpdateReady(true);
			return;
		} else
			Application::instance()->updateComplete();
	}

	void Updater::qmlLoaded() { _visibleInterface = true; }

	void Updater::downloadUpdate(Release *item)
	{
		setInfoText(QString("Распаковка"));

		QString extractPath = Config::getPath(ConfigKeys::updatePath);

		qDebug() << extractPath;
		qDebug() << item->zipFile();

		// Распаковка
		QString path(extractPath);
		JlCompress::extractDir(item->zipFile(), path);

        auto currentLauncherPathExe = Config::currentPath()
                                      + Config::getStringValue(ConfigKeys::launcherExe);
        qDebug() << currentLauncherPathExe;

		// Копирование
		if (QFile::exists(currentLauncherPathExe))
			QFile::remove(currentLauncherPathExe);

        // Запуск
        QProcess *pr = new QProcess();
		General::SettingsStorage::instance()->storeValue(
			"appPath", Config::currentPath());
		General::SettingsStorage::instance()->storeValue("updateSource", path);
		General::SettingsStorage::instance()->save();
		QStringList ott = {};
		ott << "-update";
		ott << Config::currentPath();
		ott << path;
		if (QFile::exists(path + Config::getStringValue(ConfigKeys::launcherExe))) {
			pr->startDetached(
				path + Config::getStringValue(ConfigKeys::launcherExe), ott);
		}
		Application::instance()->quit();
	}

	void Updater::setCopyUpdate()
	{
		QString source =
			General::SettingsStorage::instance()->loadValue("appPath").toString();
		QString target = General::SettingsStorage::instance()
											 ->loadValue("updateSource")
											 .toString();

		setCopyUpdate(source, target);
	}

	void Updater::setCopyUpdate(QString sourcePath, QString targetPath)
	{
		_sourcePath = targetPath;
		_targetPath = sourcePath;
        StartUpdate();
    }

    void Updater::StartUpdate()
    {
        setInfoText(QString("Копирование"));
        qDebug() << Config::currentPath();

        QTime dieTime = QTime::currentTime().addSecs(5);
		while (QTime::currentTime() < dieTime)
			QCoreApplication::processEvents(QEventLoop::AllEvents, 100);

		auto targetLauncherPathExe =
			_targetPath + Config::getStringValue(ConfigKeys::launcherExe);

		if (QFile::exists(targetLauncherPathExe))
			QFile::remove(targetLauncherPathExe);

		qDebug() << "Source path " << _sourcePath;

		copyDir(_sourcePath, _targetPath);

		QProcess *pr = new QProcess();
		QStringList ott = {};
		if (QFile::exists(targetLauncherPathExe))
			pr->startDetached(targetLauncherPathExe, ott);

		Application::instance()->quit();
    }

    void Updater::copyDir(QString source, QString target)
    {
        QDir dir(source);

		QDir tDir(target);
		if (!tDir.exists())
			tDir.mkdir(target);

		QFileInfoList list = dir.entryInfoList();
		for (int i = 0; i < list.size(); ++i) {
			QFileInfo fileInfo = list.at(i);

			if (fileInfo.fileName() == "." || fileInfo.fileName() == "..")
				continue;

			if (fileInfo.isDir()) {
				copyDir(
					source + "/" + fileInfo.fileName(),
					target + "/" + fileInfo.fileName());
				continue;
			}

			if (QFile::exists(target + "/" + fileInfo.fileName()))
				QFile::remove(target + "/" + fileInfo.fileName());

			setInfoText(QString("copy " + target + "/" + fileInfo.fileName()));
			qDebug() << "copy " << target + "/" + fileInfo.fileName();
			QFile::copy(
				source + "/" + fileInfo.fileName(), target + "/" + fileInfo.fileName());
		}
    }

    void Updater::clearTempData()
	{
		auto currentLauncherPathExe =
			Config::currentPath() + Config::getStringValue(ConfigKeys::launcherExe);
		if (QFile::exists(currentLauncherPathExe))
			QFile::remove(currentLauncherPathExe);

		auto updatePath = Config::getPath(ConfigKeys::updatePath);

		QDir dir(updatePath);

		if (dir.exists(updatePath))
			dir.removeRecursively();
	}

	void Updater::releaseLoaded()
	{
		qDebug() << "Releases loaded";

		if (_sourcePath.length() > 0) {
            qDebug() << "StartProcess";
            return;
        }

        checkUpdate();
	}

	QString Updater::updateVersion() const
	{
		return _updateVersion;
	}

	void Updater::setUpdateVersion(const QString &newUpdateVersion)
	{
		_updateVersion = newUpdateVersion;
	}

	bool Updater::updateReady() const
	{
		return _updateReady;
	}

	void Updater::setUpdateReady(bool newUpdateReady)
	{
		_updateReady = newUpdateReady;
		emitUpdateReadyChange();
	}

	void Updater::emitUpdateReadyChange()
	{
		emit updateReadyChange();
	}

	void Updater::setInfoText(QString newInfoText)
	{
		_infoText = newInfoText;
		emit infoChangeSignal();
	}

	const QString Updater::getInfoText() { return _infoText; }

} // namespace Core
