#ifndef APPLICATION_H
#define APPLICATION_H

#include <QObject>
#include <QApplication>
#include <QGuiApplication>

#include "core/settingsstorage.h"
#include "gui/mainwindow.h"

namespace Core{
    class Logging;
    class Internet;
    class Statistic;
}

class Application : public QObject
{
    Q_OBJECT
public:
    explicit Application(QObject *parent = nullptr);

    int Initiate(int argc, char *argv[], QString deviceInfo);

signals:

public slots:

private:
    QTimer* m_timerUpdateTorrentStatus;

private:

};

#endif // APPLICATION_H
