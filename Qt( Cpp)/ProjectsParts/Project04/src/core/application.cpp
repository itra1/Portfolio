#include "application.h"
#include <QDir>
#include <QGuiApplication>
#include <QQmlApplicationEngine>
#include <QString>
#include "../config/config.h"
#include "../general/appbase.h"
#include "../general/authorization.h"
#include "../general/settingsstorage.h"
#include "../ui/form.h"
#include "apphase.h"
#include "releasemanager.h"
#include "serversmanager.h"
#include "session.h"
#include "settings.h"
#include "updater.h"
#include "wall/clientlog.h"

namespace Core {

Application *Application::_instance = nullptr;

Application::Application(QQmlApplicationEngine *engine, QObject *parent)
    : General::AppBase(parent)
    , _engine(engine)
    , _logger(new Logger())
{
    auto isUpdate{false};
    auto isLog{true};
    _argvInputLenght = 0;

    const QStringList args = QCoreApplication::arguments();

    for (const QString &arg : args.mid(1)) {
        _argvInputLenght++;

        if (arg == "-update") {
            isUpdate = true;
        }
        if (arg == "-disableLog") {
            isLog = false;
        }
        if (arg == Config::getStringValue(ConfigKeys::sumAdaptive_runKey)) {
            Session::instance()->setIsPresentationMode(true);
        }
    }

    _argvInput = new QString[_argvInputLenght];
    int index = -1;
    for (const QString &arg : args.mid(1)) {
        _argvInput[++index] = arg;
    }

    if (isLog)
        _logger->init();

    qDebug() << "App path : " << qApp->applicationDirPath();

    _releaseLoader = new Core::ReleaseLoader();
    Core::ServersManager::initInstance();
    ReleaseManager::initInstance(_releaseLoader);
    Updater::initInstance(_releaseLoader);
    ClientLog::init();

    if (isUpdate)
        Updater::instance()->setCopyUpdate();
    else
        Updater::instance()->clearTempData();

    General::Authorization::initInstance();

    setAppState(Core::AppPhase::State::Authorization);

    _browserManager = new Core::BrowserManager(_releaseLoader, Core::ReleaseManager::instance());
    connect(Core::ReleaseManager::instance(),
            &Core::ReleaseManager::releaseLoadSignal,
            _browserManager,
            &Core::BrowserManager::ReleasesLoaded);

    connect(Core::Session::instance(), SIGNAL(authChange(bool)), SLOT(authChange(bool)));
    connect(General::Authorization::instance(),
            SIGNAL(isAuthStart(bool)),
            SLOT(setIsAuthProcess(bool)));
}

void Application::authChange(bool isLogin)
{
    if (!isLogin) {
        qDebug() << "Application logout";
        setAppState(AppPhase::Authorization);
        return;
    } else {
        UI::Form::instance()->setInfoText("login complete");
        ReleaseManager::instance()->autoPlayReleaseWithSumlite();
        setAppState(AppPhase::Releases);
    }
}

void Application::initInstance(QQmlApplicationEngine *engine, QObject *parent)
{
    if (!_instance)
        _instance = new Application(engine, parent);
}

void Application::freeInstance()
{
    if (_instance) {
        delete _instance;
        _instance = nullptr;
    }
}

Application *Application::instance()
{
    return _instance;
}

void Application::setIsAuthProcess(bool newIsAuthProcess)
{
    isAuthProcess = newIsAuthProcess;
    emit onAuthProcess();
}

void Application::quit()
{
    qDebug() << "Application quit";
    UI::Form::freeInstance();
    General::SettingsStorage::freeInstance();
    QGuiApplication::quit();
}

void Application::settingsButtonTouch()
{
    setSettingsOpen(!getSettingsOpen());
}

void Application::allReleaseDownload()
{
    if (_appState == AppPhase::Authorization)
        return;
    setAppState(AppPhase::Releases);
}

void Application::initinitForm()
{
    UI::Form::initInstance(_engine, nullptr);
}

Core::AppPhase::State Application::appState() const
{
    return _appState;
}

void Application::setAppState(AppPhase::State newAppState)
{
    qDebug() << "set phase " << newAppState;
    _appState = newAppState;
    emit appPhaseChange();
}

void Application::updateComplete()
{
    if (Core::Session::instance()->isPresentationMode())
        General::Authorization::instance()->authorizationStart();
}

QString Application::getCachePath()
{
    return qgetenv("USERPROFILE") + "/AppData/LocalLow/CNP/VideoWall";
}

QString Application::getCacheBrowser()
{
    return getCachePath() + "/Vuplex.WebView";
}

void Application::clearBrowserCache()
{
    QDir(getCacheBrowser()).removeRecursively();
}

void Application::clearFullCache()
{
    QDir(getCachePath()).removeRecursively();
}

bool Application::getSettingsOpen() const
{
    return _settingsOpen;
}

void Application::setSettingsOpen(bool newSettingsOpen)
{
    _settingsOpen = newSettingsOpen;
    emit isSettingsChange();
}

int Application::getArgvInputLenght() const
{
    return _argvInputLenght;
}

QString *Application::getArgvInput() const
{
    return _argvInput;
}

} // namespace Core
