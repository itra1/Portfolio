#ifndef WEBSERVER_H
#define WEBSERVER_H

#include <QObject>
#include <QTcpServer>

class WebServer : public QTcpServer
{
    Q_OBJECT

    explicit WebServer(QObject *parent = nullptr);

public:

    static void initInstance();
    static void freeInstance();
    static WebServer *instance();
    void incomingConnection(qintptr handle);

    void start();
    QString serverUrl();

signals:

public slots:

private slots:
    void readyReadHandle();
    void disconnectedHandle();

private:
    static WebServer *m_instance;
    bool m_isStarted;

    void startServer(quint16 port);
};

#endif // WEBSERVER_H
