#ifndef APPLICATION_H
#define APPLICATION_H

#include "copytheards.h"

#include <QGuiApplication>
#include <QObject>
#include <QQmlApplicationEngine>
#include <QQmlContext>
#include <QProcess>

namespace Updater{

    class InstalState: public QObject{

        Q_OBJECT

    public:
        enum State{
            Init,
            Process
        };

        Q_ENUM(State)

    };


    class Application : public QObject
    {
        Q_OBJECT

        Q_PROPERTY(double progress READ getProgress NOTIFY onProgressChange)
        Q_PROPERTY(Updater::InstalState::State state READ getState NOTIFY onStateChange)

    public:
        explicit Application(QGuiApplication &app, QQmlApplicationEngine &engine,
                             QString fromPath, QString toPath,
                             QString afterRun, QObject *parent = nullptr);

        double getProgress();

        InstalState::State getState() const;
        void setState(InstalState::State state);

    signals:
        void onProgressChange();
        void onStateChange();

    public slots:
        void setProgress(double progress);
        void startCopy();
        void handleTheardFinished();
        void handleStartCopy();

        void handleAfterCopy();

    private:
        InstalState::State m_state;
        QString m_afterRun;
        double m_progress;
        QProcess *m_afterRunProcess;

        QThread *m_copyTheard;
        CopyTheards *m_copyTheardObject;

    };
}

#endif // APPLICATION_H
