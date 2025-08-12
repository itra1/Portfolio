#ifndef APPLICATION_H
#define APPLICATION_H
#include <QGuiApplication>
#include <QObject>
#include <QProcess>
#include <QQmlApplicationEngine>
#include "../general/appbase.h"
#include "apphase.h"
#include "browsermanager.h"
#include "logger.h"
#include "releaseloader.h"

namespace Core {

class Application : public General::AppBase
{
	Q_OBJECT
	public:
	explicit Application(const Application &) = delete;
	Application &operator=(const Application &) = delete;

    explicit Application(QQmlApplicationEngine *engine, QObject *parent = nullptr);
    static void initInstance(QQmlApplicationEngine *engine, QObject *parent);
    static void freeInstance();
	static Application *instance();

	bool isAuthProcess = false;
	static bool loginFile;
	static bool isEditor;

	static QString getCachePath();
	static QString getCacheBrowser();

	void initinitForm();
	void clearBrowserCache();
	void clearFullCache();
	void quit();

	Q_INVOKABLE void settingsButtonTouch();

	Q_PROPERTY(bool isSettings READ getSettingsOpen WRITE setSettingsOpen NOTIFY isSettingsChange)
	Q_PROPERTY(Core::AppPhase::State appState READ appState NOTIFY appPhaseChange);

	void allReleaseDownload();

	void setAppState(AppPhase::State newAppState);
	void updateComplete();

	AppPhase::State appState() const;

	QString *getArgvInput() const;
	int getArgvInputLenght() const;

	bool getSettingsOpen() const;
	void setSettingsOpen(bool newSettingsOpen);

	signals:
	void releasesDownloadStartSignal();
	void releasesDownloadCompleteSignal();
	void appPhaseChange();
	void onAuthProcess();
	void developMode(bool mode);
	void isSettingsChange();

	public slots:
	void setIsAuthProcess(bool newIsAuthProcess);
	void authChange(bool isLogin);

	private:
	static Application *_instance;
    Core::ReleaseLoader *_releaseLoader;
    Core::BrowserManager *_browserManager;

    int _argvInputLenght;
    bool _settingsOpen{false};
	QString *_argvInput;
	QString _selectedVersion;
	AppPhase::State _appState;
	QQmlApplicationEngine *_engine;
	Logger *_logger;
};
} // namespace Core

#endif // APPLICATION_H
