#include "runrelease.h"
#include <QException>
#include <QObject>
#include <QTimer>
#include "../../config/config.h"
#include "../../general/authorization.h"
#include "../application.h"
#include "../serversmanager.h"
#include "../session.h"
#include "../settings.h"
#include "../wall/clientlog.h"
#include "windows.h"

namespace Core {
RunRelease::RunRelease()
    : Release()
{}

RunRelease::RunRelease(QJsonObject jObject)
    : Release(jObject)
{}

void RunRelease::run()
{
    _playProcess = new QProcess();

    bool isPresentationMode = Core::Session::instance()->isPresentationMode();

    setState(ReleaseState::StartPlayed);

    QString path = exePath();
    qDebug() << "Run wall " << path;

    QStringList ott = type == Config::getStringValue(ConfigKeys::crossType)
                          ? makeArgumentsCross()
                          : makeArgumentsDefault();

    qDebug() << ott.join(" ");

    //return;

    ClientLog::saveReady();

    connect(_playProcess,
            SIGNAL(finished(int, QProcess::ExitStatus)),
            this,
            SLOT(playComplete(int, QProcess::ExitStatus)));

    Sleep(100);

    _playProcess->start(path, ott, QProcess::NotOpen);
    checkState();
    connect(_playProcess, SIGNAL(readyReadStandardOutput()), this, SLOT(playOutput()));
    if (isPresentationMode) {
        _timeRunApp = QDateTime::currentDateTime();
        if (_restartTimer != nullptr)
            _restartTimer = nullptr;
        _restartTimer = new QTimer(this);
        connect(_restartTimer, SIGNAL(timeout()), SLOT(onPresentationTime()));
        _restartTimer->start(60000);
    }
}

void RunRelease::install()
{
    setState(ReleaseState::Unpack);
    QTime dieTime = QTime::currentTime().addSecs(1);
    while (QTime::currentTime() < dieTime)
        QCoreApplication::processEvents(QEventLoop::AllEvents, 100);
    unpack();
    afterUnzip();
    checkState();
}

bool RunRelease::checkState()
{
    if (_playProcess != nullptr) {
        setState(ReleaseState::Played);
        return true;
    }
    return Release::checkState();
}

void RunRelease::afterUnzip() {}

void RunRelease::unpack(Release *parentRelease)
{
    Release::unpack(parentRelease);
}

bool RunRelease::isRunned()
{
    return true;
}

void RunRelease::onPresentationTime()
{
    auto timeLive = Config::getIntValue(ConfigKeys::sumAdaptive_minTimeLiveClient);
    auto hourInDay = Config::getIntValue(ConfigKeys::sumAdaptive_hourInDayToReload);

    if ((_timeRunApp.secsTo(QDateTime::currentDateTime()) > timeLive)
        && QDateTime::currentDateTime().time().hour() == hourInDay) {
        _isTimerRestart = true;
        _playProcess->close();
    }
}

void RunRelease::readOutput()
{
    while (_playProcess->canReadLine()) {
        qDebug() << _playProcess->readLine();
    }
}

void RunRelease::playComplete(int exitCode, QProcess::ExitStatus exitStatus)
{
    bool isCrash = exitStatus == QProcess::ExitStatus::CrashExit;
    ClientLog::saveLog(isCrash);
    if (_restartTimer != nullptr)
        _restartTimer = nullptr;
    _playProcess = nullptr;
    checkState();

    auto isPresentationMode = Core::Session::instance()->isPresentationMode();
    auto baseSettings = Core::Settings::instance()->getBaseSettings();
    auto isAutoRestart = baseSettings->value("autoRestart").toBool();

    if (_isTimerRestart || ((isPresentationMode || isAutoRestart) && isCrash)) {
        _isTimerRestart = false;
        run();
    }
}

void RunRelease::playOutput()
{
    while (_playProcess->canReadLine()) {
        qDebug() << "out " << _playProcess->readLine();
    }
}

QStringList RunRelease::makeArgumentsDefault()
{
    QStringList ott = {};

    try {
        for (int i = 1; i < Core::Application::instance()->getArgvInputLenght(); i++) {
            QString p = Core::Application::instance()->getArgvInput()[i];
            ott << p;
        }
    } catch (QException ex) {
    }

    auto serverId = General::Authorization::instance()->serverId();
    auto baseSettings = Core::Settings::instance()->getBaseSettings();
    auto server = Core::ServersManager::instance()->getServerById(serverId);

    auto authToken = Core::Session::instance()->authToken();
    auto resolutionX = baseSettings->getTargetResolutionX();
    auto resolutionY = baseSettings->getTargetResolutionY();
    auto borderUp = baseSettings->getScreenBorderUp();
    auto borderRight = baseSettings->getScreenBorderRight();
    auto borderDown = baseSettings->getScreenBorderDown();
    auto borderLeft = baseSettings->getScreenBorderLeft();

    ott << "-token" << authToken;

    ott << "-srv" << server->url();

    baseSettings->makeRunWallKey(ott, false);
    server->makeRunWallKey(ott, false);

    ott << "-resolution" << QString::number(resolutionX) << QString::number(resolutionY);

    ott << "-borders" << QString::number(borderUp) << QString::number(borderRight)
        << QString::number(borderDown) << QString::number(borderLeft);

    return ott;
}

QStringList RunRelease::makeArgumentsCross()
{
    QStringList ott = {};

    // try {
    //     for (int i = 1; i < Core::Application::instance()->getArgvInputLenght(); i++)
    //     {
    //         QString p = Core::Application::instance()->getArgvInput()[i];
    //         ott << "-" + p;
    //     }
    // } catch (QException ex) {
    // }

    auto serverId = General::Authorization::instance()->serverId();
    auto baseSettings = Core::Settings::instance()->getBaseSettings();
    auto server = Core::ServersManager::instance()->getServerById(serverId);

    auto authToken = Core::Session::instance()->authToken();
    auto resolutionX = baseSettings->getTargetResolutionX();
    auto resolutionY = baseSettings->getTargetResolutionY();
    auto borderUp = baseSettings->getScreenBorderUp();
    auto borderRight = baseSettings->getScreenBorderRight();
    auto borderDown = baseSettings->getScreenBorderDown();
    auto borderLeft = baseSettings->getScreenBorderLeft();

    ott << "--token=" + authToken;

    ott << "--srv=" + server->url();

    baseSettings->makeRunWallKey(ott, true);
    server->makeRunWallKey(ott, true);

    // ott << "--resolution=" + QString::number(resolutionX)
    //            + "_" + QString::number(resolutionY);

    // ott << "--borders=" + QString::number(borderUp)
    //            + "_" + QString::number(borderRight)
    //            + "_" + QString::number(borderDown)
    //            + "_" + QString::number(borderLeft);

    return ott;
}

} // namespace Core
