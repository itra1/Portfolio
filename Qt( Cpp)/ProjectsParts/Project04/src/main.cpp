#include <QDir>
#include <QFile>
#include <QGuiApplication>
#include <QLoggingCategory>
#include <QQmlApplicationEngine>
#include <QScopedPointer>
#include <QTextStream>
#include "config/config.h"
#include "core/application.h"
#include "core/configcontroller.h"
#include "core/errorcontroller.h"
#include "core/session.h"
#include "core/settings.h"
#include "core/settingsjsoncontroller.h"
#include "general/settingsstorage.h"

int main(int argc, char *argv[])
{
	Config cnf;
	QGuiApplication app(argc, argv);
	QQmlApplicationEngine *engine = new QQmlApplicationEngine();

	//app.setOrganizationName("CnpOS");
	//app.setOrganizationDomain("cnpos.ac.gov.ru");

	General::SettingsStorage::initInstance();
	Core::SettingsJsonController::readConfig();
	Core::Settings::initInstance();
	Core::Session::initInstance();

	Core::ConfigController cc;
	Core::ErrorController::initInstance();
	
	Core::ConfigController::load();
	Core::Settings::instance()->load();
	
	Core::Application::initInstance(engine, nullptr);
	
	qDebug() << "Application version " << APP_VERSION;

	Core::Application::instance()->initinitForm();

	return app.exec();
}
