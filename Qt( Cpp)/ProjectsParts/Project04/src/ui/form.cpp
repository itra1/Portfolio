#include "form.h"
#include <QDesktopServices>
#include <QObject>
#include <QQmlApplicationEngine>
#include <QQmlContext>
#include <QStandardPaths>
#include <QtDebug>
#include "../config/config.h"
#include "../core/apphase.h"
#include "../core/application.h"
#include "../core/errorcontroller.h"
#include "../core/releasemanager.h"
#include "../core/releases/releasestate.h"
#include "../core/releases/runrelease.h"
#include "../core/serversmanager.h"
#include "../core/session.h"
#include "../core/settings.h"
#include "../core/settings/commonsettings.h"
#include "../core/structs/server.h"
#include "../core/updater.h"
#include "../core/users/user.h"
#include "../general/authorization.h"

using namespace UI;

Form *Form::_instance = nullptr;
QQmlApplicationEngine *Form::_qmlEngine = nullptr;

Form::Form(QQmlApplicationEngine *engine, QObject *parent)
		: QObject(parent)
{
	_qmlEngine = engine;
    _qmlEngine->addImportPath(":/forms");
    qmlRegisterType<Core::AppPhase>("AppPhase", 1, 0, "AppPhase");
    qmlRegisterType<Core::ReleaseState>("ReleaseState", 1, 0, "ReleaseState");
    qmlRegisterType<Core::RunRelease>("RunRelease", 1, 0, "RunRelease");
    qmlRegisterType<Core::Server>("Server", 1, 0, "Server");
    qmlRegisterType<Core::CommonSettings>("BaseSettings", 1, 0, "BaseSettings");
	qmlRegisterType<Core::User>("AppUser", 1, 0, "AppUser");
	qmlRegisterType<Core::SettingsItem>("SettingsItem", 1, 0, "SettingsItem");

    qmlRegisterSingletonType(getTheme(), "Theme", 1, 0, "Theme");
    qmlRegisterSingletonType(QUrl(QStringLiteral(
                                 "qrc:/forms/LauncherQml/src/ui/main/Config.qml")),
                             "Config",
                             1,
                             0,
                             "Config");

    engine->rootContext()->setContextProperty("Manager", this);
    engine->rootContext()->setContextProperty("App", Core::Application::instance());
    engine->rootContext()->setContextProperty("ServersManager", Core::ServersManager::instance());
    engine->rootContext()->setContextProperty("ReleaseManager", Core::ReleaseManager::instance());
    engine->rootContext()->setContextProperty("UpdaterManager", Core::Updater::instance());
    engine->rootContext()->setContextProperty("SettingsController", Core::Settings::instance());
    engine->rootContext()->setContextProperty("Authorization", General::Authorization::instance());
    engine->rootContext()->setContextProperty("ErrorController", Core::ErrorController::instance());
    engine->rootContext()->setContextProperty("Session", Core::Session::instance());

    connect(Core::Application::instance(), SIGNAL(onAuthProcess()), SLOT(authProcess()));
    connect(General::Authorization::instance(),
            SIGNAL(authError(QString)),
            SLOT(loginError(QString)));

    _qmlEngine->load("qrc:/forms/LauncherQml/src/ui/main/Base.qml");
}

QUrl Form::getTheme()
{
#ifdef LUK_THEME
    return QUrl(QStringLiteral("qrc:/forms/LauncherQml/src/ui/themes/lukoil/Theme.qml"));
#else
    return QUrl(QStringLiteral("qrc:/forms/LauncherQml/src/ui/themes/cnp/Theme.qml"));
#endif
}

void Form::initInstance(QQmlApplicationEngine *engine, QObject *parent)
{
	if (!_instance)
		_instance = new Form(engine, parent);
}

void Form::freeInstance()
{
	if (_instance) {
		delete _qmlEngine->rootContext();
		delete _instance;
		_instance = nullptr;
	}
}

Form *Form::instance()
{
	return _instance;
}

void Form::setInfoText(QString newText)
{
	_infoText = newText;
	emit onInfoText();
}

QString Form::getInfoText()
{
	return _infoText;
}

bool Form::isloginError()
{
	return _loginError.length() > 0;
}

QString Form::getloginErrorText()
{
	return _loginError;
}

bool Form::isPlayReady()
{
	return false;
}

bool Form::isAuthProcess()
{
	return Core::Application::instance()->isAuthProcess;
}

Core::AppPhase::State Form::appState()
{
	return Core::Application::instance()->appState();
}

bool Form::getIsPresentationMove()
{
	return Core::Session::instance()->isPresentationMode();
}

void Form::clearBrowserCache()
{
	Core::Application::instance()->clearBrowserCache();
}

void Form::clearAllCache()
{
	Core::Application::instance()->clearFullCache();
}

void Form::quit()
{
	Core::Application::instance()->quit();
}

QString Form::appVersion()
{
	return APP_VERSION;
}

void Form::openAppLog()
{
	QString path = Config::currentPath() + "/";
	QDir dir(path);
	if (dir.exists()) {
		QDesktopServices::openUrl(QUrl::fromLocalFile(path));
	} else {
		QDesktopServices::openUrl(Config::currentPath());
	}
}

void Form::openClientLog()
{
	QDesktopServices::openUrl(Config::videoWallLogFolder());
}

void Form::loginError(QString error)
{
	_loginError = error;
	emit onloginErrorChange();
}

void Form::authProcess()
{
	emit onAuthProcess();
}
