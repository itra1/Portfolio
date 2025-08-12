#include "../network/network.h"
#include <QNetworkRequest>

using namespace Core;

Network::Network(QObject *parent) : QObject(parent) {}

void Network::Request(QString url, QString token, RequestType requestType, QJsonDocument form, const std::function<void(QNetworkReply*)>& callback)
{

    QNetworkAccessManager *manager = new QNetworkAccessManager();
    QNetworkRequest req(QUrl(url, QUrl::ParsingMode::TolerantMode));

    qDebug() << "Request: " << url;
    QSslConfiguration config = QSslConfiguration::defaultConfiguration();
		config.setPeerVerifyMode(QSslSocket::VerifyNone);
		req.setSslConfiguration(config);

		req.setHeader(QNetworkRequest::ContentTypeHeader, "application/json");

		QNetworkReply *reply;

		if (!token.isEmpty())
			req.setRawHeader(
				"Authorization", (new QString("Bearer " + token))->toUtf8());

		connect(
			manager, &QNetworkAccessManager::finished,
			[=](QNetworkReply *result) { callback(result); });

		if (requestType == RequestType::Get) {
			reply = manager->get(req);
		}
		if (requestType == RequestType::Post) {
			QString strJson(form.toJson(QJsonDocument::Compact));
			reply = manager->post(req, strJson.toUtf8());
		}
		if (requestType == RequestType::Patch) {
			QString strJson(form.toJson(QJsonDocument::Compact));
			QBuffer *buffer = new QBuffer();
			buffer->open((QBuffer::ReadWrite));
			buffer->write(strJson.toUtf8());
			buffer->seek(0);
			reply = manager->sendCustomRequest(req, "PATCH", buffer);
		}

		//connect(manager, &QNetworkAccessManager::finished, result);
}

void Network::Request(QString url, QString token, RequestType requestType, QJsonDocument form, const std::function<void(QNetworkReply*)>& callback, const std::function<void(qint64, qint64)>& download)
{

    QNetworkAccessManager *manager = new QNetworkAccessManager();
    QNetworkRequest req(QUrl(url, QUrl::ParsingMode::TolerantMode));

    qDebug() << "Request: " << url;

    req.setHeader( QNetworkRequest::ContentTypeHeader, "application/json" );
    QSslConfiguration config = QSslConfiguration::defaultConfiguration();
    config.setPeerVerifyMode(QSslSocket::VerifyNone);
    req.setSslConfiguration(config);

    QNetworkReply* reply;

    if(!token.isEmpty())
        req.setRawHeader( "Authorization", (new QString("Bearer " + token))->toUtf8());

    if(requestType == RequestType::Get){
        reply = manager->get(req);
    }
    if(requestType == RequestType::Post){
        QString strJson(form.toJson(QJsonDocument::Compact));
				reply = manager->post(req, strJson.toUtf8());
    }

    connect(reply, &QNetworkReply::downloadProgress, [=](qint64 receit, qint64 total){
      download(receit,total);
      });
    connect(manager, &QNetworkAccessManager::finished, [=](QNetworkReply *result){
      callback(result);
      });
}

void Network::Request(QString url, QString token, RequestType requestType, QUrlQuery form, const std::function<void(QNetworkReply*)>& callback)
{

    QNetworkAccessManager *manager = new QNetworkAccessManager();
    QNetworkRequest req(QUrl(url, QUrl::ParsingMode::TolerantMode));

    qDebug() << "Request: " << url;

    QSslConfiguration config = QSslConfiguration::defaultConfiguration();
    config.setPeerVerifyMode(QSslSocket::VerifyNone);
    req.setSslConfiguration(config);

    if(!token.isEmpty())
        req.setRawHeader( "Authorization", (new QString("Bearer " + token))->toUtf8());

    if(requestType == RequestType::Get){
        manager->get(req);
    }
    if(requestType == RequestType::Post){
        manager->post(req,form.toString(QUrl::FullyEncoded).toUtf8());
    }

    connect(manager, &QNetworkAccessManager::finished, [=](QNetworkReply *result){
      callback(result);
      });

    //connect(manager, &QNetworkAccessManager::finished, result);
}
