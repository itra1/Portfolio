#include "application.h"

#include <QDebug>
#include <QDir>
#include <QLockFile>
#include <QMessageBox>
#include <QQmlApplicationEngine>

#include "core/logging.h"

#include "config.h"
#include "gui/mainwindow.h"
#include "core/usersession.h"
#include "core/statistic.h"
#include "core/sourcemanager.h"
#include "base/profile.h"
#include "base/bittorrent/session.h"
#include "core/launcher.h"
#include "core/internet.h"


Application::Application(QObject *parent) : QObject(parent)
{
}

int Application::Initiate(int argc, char *argv[], QString deviceInfo){

    QApplication app(argc, argv);

    //qInstallMessageHandler(Core::Logging::logHandler);

    QLockFile lockFile(QDir::temp().absoluteFilePath("pz.lock"));

    if(!lockFile.tryLock(100)){
        QMessageBox msgBox;
        msgBox.setIcon(QMessageBox::Warning);
        msgBox.setText("Приложение уже запущено.\n"
                       "Разрешено запускать только один экземпляр приложения.");
        msgBox.exec();
        return 1;
    }

    Core::Internet::initInstance();
    QQmlApplicationEngine *engine = new QQmlApplicationEngine();
    Core::SettingsStorage::initInstance();
    Core::Launcher::initInstance();
    Gui::MainWindow::initInstance(engine,nullptr);

    QTimer::singleShot(10,this,[=]{
        try {
            if(argc > 1){
                QString key = argv[1];
                QString val = argv[2];
                if(key == "setRunPath")
                    QDir::setCurrent(val);
            }
        } catch (QException ex) {

        }

        QString profileDir = QDir::currentPath();

        Profile::initialize(Config::TMP_PATH_ROOT, nullptr,true);

        Core::UserSession::initInstance();
        Core::Statistic::initInstance(deviceInfo);
        Core::SourceManager::initInstance();
        Core::Launcher::instance()->initialization(engine);
        Core::SourceManager::instance()->initialization();
        Core::UserSession::instance()->init();

        Gui::MainWindow::instance()->loadMainWindow();
    });

    if (engine->rootObjects().isEmpty())
        return -1;

    app.exec();
}
