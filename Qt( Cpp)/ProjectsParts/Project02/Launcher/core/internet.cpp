#include "internet.h"

#include <QTimer>
#include <QNetworkAccessManager>
#include <QNetworkReply>
#include <QEventLoop>

#include <QDebug>

using namespace Core;

Internet::Internet(QObject *parent) : QObject(parent)
  ,m_wait(false)
  ,m_exists(false)
{
    checkInternet();
}

Internet *Internet::m_instance = nullptr;

void Internet::initInstance()
{
    if(!m_instance)
        m_instance = new Internet();
}

void Internet::freeInstance()
{
    if (m_instance) {
        delete m_instance;
        m_instance = nullptr;
    }
}

Internet *Internet::instance()
{
    return m_instance;
}

void Internet::checkInternet()
{
    if(m_wait){
        waitCheckInternet();
        return;
    }

    m_wait = true;
    QNetworkAccessManager nam;
    QNetworkRequest req(QUrl("http://pzonline.com"));
    QNetworkReply *reply = nam.get(req);
    QEventLoop *loop = new QEventLoop();
    connect(reply, SIGNAL(finished()), loop, SLOT(quit()));

    QTimer::singleShot(15000,[=]{
        if(!m_wait)
            return;
        loop->quit();
    });
    loop->exec();
    if(reply->bytesAvailable())
        setExists(true);
    else
        setExists(false);
    m_wait =false;

}

bool Internet::exists() const
{
    return m_exists;
}

void Internet::setExists(bool exists)
{
    m_exists = exists;
    qDebug() << "Iternet " << m_exists;
    emit onInternetChange(m_exists);
    waitCheckInternet();
}

void Internet::waitCheckInternet()
{
    QTimer::singleShot(m_exists ? 20000 : 10000,this,&Internet::checkInternet);
}
