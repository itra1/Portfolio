#include "usersession.h"
#include <QDebug>
#include <QDesktopServices>
#include <QUrl>
#include <QUrlQuery>
#include <QUuid>

#include "config.h"
#include "base/webServer/webserver.h"
#include "base/settingvalue.h"
#include "base/net/downloadmanager.h"
#include "base/net/downloadhandler.h"
#include "core/launcher.h"
#include "core/logging.h"

#include <QJsonDocument>
#include <QJsonObject>
#include <QJsonArray>

using namespace Core;

const QString KS_Token = "token";

UserSession *UserSession::m_instance = nullptr;

UserSession::UserSession(QObject *parent) : QObject(parent),
    m_isAuth(false),
    m_token(SettingsStorage::instance()->loadValue(KS_Token,"").toString())
{
}

void UserSession::initInstance()
{
    if(!m_instance)
        m_instance = new UserSession();
}

void UserSession::freeInstance()
{
    if (m_instance) {
        delete m_instance;
        m_instance = nullptr;
    }
}

UserSession *UserSession::instance()
{
    return m_instance;
}

void UserSession::init()
{
    if(m_token != "")
        downloadUserInfo();

}

bool UserSession::isAuth()
{
    return m_isAuth;
}

QString UserSession::userName() const
{
    return m_userName;
}

QString UserSession::token() const
{
    return m_token;
}

void UserSession::setToken(const QString &token)
{
    m_token = token;
    qDebug() << "Set token: " + token;

    if(token != "")
        downloadUserInfo();
    save();
}

QList<UserSession::Version>* UserSession::versions() const
{
    return m_versions;
}

QString UserSession::authUrl()
{
    return Config::SING_IN_URL;
}

QString UserSession::redirectUrl()
{
    return Config::SING_IN_URL_SUCCESS;
}

QString UserSession::userDataUrl()
{
    return Config::USER_INFO_URL.arg(token());
}

void UserSession::login()
{
    WebServer::instance()->start();
    qDebug() << QString(Config::SING_IN_URL).arg(QString(QUrl::toPercentEncoding(WebServer::instance()->serverUrl())));
    QDesktopServices::openUrl(QUrl(QString(Config::SING_IN_URL).arg(QString(QUrl::toPercentEncoding(WebServer::instance()->serverUrl())))));
}

void UserSession::logOut()
{
    m_token = "";
    setIsAuth(false);
    save();
}

void UserSession::downloadUserInfo()
{
    qDebug() << "Download User Info: " + userDataUrl();
    Net::DownloadHandler *handler = Net::DownloadManager::instance()->download(Net::DownloadRequest(userDataUrl()));
    //Net::DownloadHandler *handler = Net::DownloadManager::instance()->download(Net::DownloadRequest(userDataUrl()));
    connect(handler, static_cast<void (Net::DownloadHandler::*)(const QString &, const QByteArray &)>(&Net::DownloadHandler::downloadFinished)
            ,[this](const QString &url, const QByteArray &data){

        qDebug() << "Download User Info ok";
        parceUserData(data);

    });
    connect(handler, &Net::DownloadHandler::downloadFailed, this, [=](const QString &url, const QString &reason){
        qDebug() << "Download User Info error: " + reason;
    });
}

void UserSession::parceUserData(QByteArray data)
{
    try{

        QJsonDocument document = QJsonDocument::fromJson(data);
        QJsonObject root = document.object();

        if(root.value("status").toString() != "ok"){
            logOut();
            return;
        }
        qDebug() << "Parcer user data " + root.value("status").toString();

        QJsonValue dataVal = root.value("data");
        QJsonObject dataObj =  dataVal.toObject();
        m_userName = dataObj.value("username").toString();
        QJsonValue versionsVal = dataObj.value("versions");
        QJsonArray versionsArr = versionsVal.toArray();

        m_versions = new QList<Version>();

        for(int i = 0; i < versionsArr.count(); i++){
            QJsonObject oneVers = versionsArr.at(i).toObject();
            Version vr;
            vr.stream = oneVers.value("name").toString();
            vr.url = oneVers.value("url").toString();

            if(vr.url[vr.url.length()-1] == "/")
                vr.url = vr.url.left(vr.url.length()-1);

            vr.guid = QUrl::toPercentEncoding(vr.stream + vr.url);
            m_versions->append(vr);
        }
        setIsAuth(true);

    }catch( std::exception &ex){
        qDebug() << "User info parcer error";
        logOut();
    }
}

void UserSession::setIsAuth(bool isAuth)
{
    qDebug() << "User auth";
    m_isAuth = isAuth;
    onAuthChange(m_isAuth);
}

void UserSession::save()
{
    SettingsStorage::instance()->storeValue(KS_Token,m_token);
    SettingsStorage::instance()->saveSync();
}
