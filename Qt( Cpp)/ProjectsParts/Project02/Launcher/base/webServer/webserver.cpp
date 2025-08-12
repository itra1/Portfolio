#include "webserver.h"

#include <QHostInfo>
#include <QNetworkInterface>
#include <QDebug>
#include <QTcpSocket>
#include <QNetworkProxy>
#include <QDateTime>
#include "core/usersession.h"
#include "core/logging.h"

WebServer *WebServer::m_instance = nullptr;

quint16 usePort = 6358;

WebServer::WebServer(QObject *parent) : QTcpServer(parent),
    m_isStarted(false)
{
    start();
}

void WebServer::initInstance()
{
    if(!m_instance)
        m_instance = new WebServer();
}

void WebServer::freeInstance()
{
    if (m_instance) {
        delete m_instance;
        m_instance = nullptr;
    }
}

WebServer *WebServer::instance()
{
    return m_instance;
}

void WebServer::incomingConnection(qintptr handle)
{
    qDebug() << "incomingConnetion";
    QTcpSocket *socket = new QTcpSocket();
    socket->setSocketDescriptor(handle);

    connect(socket,&QTcpSocket::readyRead,this,&WebServer::readyReadHandle);
    connect(socket,&QTcpSocket::disconnected,this,&WebServer::disconnectedHandle);
}

void WebServer::start()
{
    if(m_isStarted)
        return;

    startServer(usePort);

}

QString WebServer::serverUrl()
{
    qDebug() << QString("http://localhost:%1").arg(usePort);
    return QString("http://localhost:%1").arg(usePort);
}

void WebServer::readyReadHandle()
{
    QTcpSocket *socket = qobject_cast<QTcpSocket*>(sender());
    QString dataAll = socket->readAll();
    QStringList dataList = dataAll.split(" ");
    qDebug() << dataAll;

    if(dataList[1].contains("token")){
        QString token = dataList[1].split("=")[1];
        Core::UserSession::instance()->setToken(token);
    }

    QString response = "";
    response += "HTTP/1.1 200 OK\r\n";
    response += "Refresh: 0; url=https://account.pzonline.com/auth/pz/success\r\n";
    socket->write(response.toUtf8());
    socket->disconnectFromHost();
}

void WebServer::disconnectedHandle()
{
    QTcpSocket *socket = qobject_cast<QTcpSocket*>(sender());
    socket->close();
    socket->deleteLater();
}

void WebServer::startServer(quint16 port)
{
    qDebug() << "Try start web server wish port " + QString::number(port);
    if(listen(QHostAddress::Any,port)){
        qDebug() << "Web server listening port " + QString::number(port) + "...";
        m_isStarted = true;
    }else{
        qDebug() << "Error init webserver: " << errorString();
        startServer(++port);

    }
}
