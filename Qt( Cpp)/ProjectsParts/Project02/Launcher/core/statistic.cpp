#include "statistic.h"

#include <QNetworkAccessManager>
#include "core/usersession.h"
#include "core/sourcemanager.h"

#include <QCryptographicHash>

using namespace Core;

Statistic *Statistic::m_instance = nullptr;

Statistic::Statistic(QString deviceInfo, QObject *parent) : QObject(parent)
{
    QCryptographicHash *hash = new QCryptographicHash(QCryptographicHash::Sha1);
    hash->addData(deviceInfo.toUtf8());
    m_hardwareId = QString(hash->result());
}

void Statistic::initInstance(QString deviceInfo)
{
    if(!m_instance)
        m_instance = new Statistic(deviceInfo);
}

void Statistic::freeInstance()
{
    if (m_instance) {
        delete m_instance;
        m_instance = nullptr;
    }
}

Statistic *Statistic::instance()
{
    return m_instance;
}

void Statistic::send(QString action)
{
    QNetworkAccessManager manager;
    QNetworkReply *reply;
    QUrl apiUrl;
    QByteArray requestString;

    requestString += "action=" + action;
    requestString += "&token=" + Core::UserSession::instance()->token();
    requestString += "&hardware_id=" + m_hardwareId;
    requestString += "&version=" + Core::SourceManager::instance()->getSourceLauncher()->getSourceLocal()->version();

    QNetworkRequest request(server);
    reply = manager.post(request, requestString);
    //connect(reply, SIGNAL(finished()),this, SLOT(getReplyFinished()));
    //connect(reply, SIGNAL(readyRead()), this, SLOT(readyReadReply()));
}
