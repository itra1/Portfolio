#include "application.h"
#include <QApplication>
#include <QQmlApplicationEngine>
#include <QFile>
#include <QDir>
#include <QDebug>
#include <QTimer>
#include <QThread>
#include <QTimer>

#include "core/logging.h"
#include "global.h"

namespace Updater{

    namespace{

        qint64 minValue(qint64 one, qint64 two ){
            if(one < two)
                return one;
            else two;
        }

    }

    Application::Application(QGuiApplication &app, QQmlApplicationEngine &engine,
                             QString fromPath, QString toPath,QString afterRun, QObject *parent)
        : QObject(parent)
        , m_state(InstalState::Init)
        , m_afterRun(afterRun)
    {

        qInstallMessageHandler(Core::Logging::logHandler);

        qmlRegisterType<InstalState>("InstalState",1,0,"InstalState");
        engine.rootContext()->setContextProperty("model",this);
        engine.load(QUrl(QStringLiteral("qrc:/qml/main.qml")));

        QTimer::singleShot(5000, this, SLOT(startCopy()));

        m_copyTheard = new QThread(this);
        m_copyTheardObject = new CopyTheards(fromPath,toPath);

        connect(m_copyTheard,&QThread::started,m_copyTheardObject,&CopyTheards::run);
        connect(m_copyTheard,&QThread::finished,this,&Application::handleTheardFinished);
        connect(m_copyTheardObject,&CopyTheards::onFinished,m_copyTheard,&QThread::terminate);
        connect(m_copyTheardObject,&CopyTheards::onProgress,this,&Application::setProgress);
        connect(m_copyTheardObject,&CopyTheards::onStartCopy,this,&Application::handleStartCopy);

        m_copyTheardObject->moveToThread(m_copyTheard);
    }

    double Application::getProgress()
    {
        return m_progress;
    }

    InstalState::State Application::getState() const
    {
        return m_state;
    }

    void Application::setState(InstalState::State state)
    {
        if(state != m_state){
            m_state = state;
            emit onStateChange();
        }

    }

    void Application::startCopy()
    {
        m_copyTheard->start();
    }

    void Application::handleTheardFinished()
    {

        qDebug() << "Run: " + m_afterRun;
        QProcess::startDetached("\"" + m_afterRun + "\"");
//         system(("\"" + m_afterRun + "\"").toLatin1().constData());
//        m_afterRunProcess = new QProcess();
//        m_afterRunProcess->startDetached(m_afterRun);
        QTimer::singleShot(2000, this, SLOT(handleAfterCopy()));

    }

    void Application::handleStartCopy()
    {
        setState(InstalState::Process);
    }

    void Application::handleAfterCopy()
    {
        QApplication::exit();
    }

    void Application::setProgress(double progress)
    {
        m_progress = progress;
        emit onProgressChange();
    }

}
