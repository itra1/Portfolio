#include "api.h"
#include <QDebug>
#include <QJsonDocument>
#include <QNetworkAccessManager>
#include <QNetworkReply>
#include <QtNetwork>
#include "../general/authorization.h"
#include "../network/network.h"
#include "session.h"

namespace Core {

void Api::authorization(QString userName,
												QString password,
												const std::function<void(bool, QNetworkReply *)> &onComplete)
{
	QJsonObject jObject;
	jObject["username"] = userName;
	jObject["password"] = password;
	jObject["role"] = "VideoWall";

	qDebug() << "Authorization " << userName;

	Core::Network::Request(General::Authorization::instance()->server() + "/auth/login",
												 "",
												 RequestType::Post,
												 QJsonDocument(jObject),
												 [=](QNetworkReply *reply) {
													 if (reply->error()) {
														 qDebug() << "Error " << reply->error();
														 onComplete(false, reply);
														 return;
													 }
													 onComplete(true, reply);
												 });
}

void Api::loadReleases(const std::function<void(QJsonArray)> &onComplete,
											 const std::function<void(QString)> &onError)
{
	Core::Network::Request(General::Authorization::instance()->server() + "/releases",
												 Core::Session::instance()->authToken(),
												 RequestType::Get,
												 QJsonDocument(),
												 [=](QNetworkReply *reply) {
													 if (reply->error()) {
														 qDebug() << "Error " << reply->error();
														 onError(reply->readAll());
														 return;
													 }

													 QByteArray response = reply->readAll();
													 onComplete(QJsonDocument::fromJson(response).array());
												 });
}

void Api::loadRelease(QString url,
											const std::function<void(bool, QNetworkReply *)> &onComplete,
											const std::function<void(qint64, qint64)> &onProgress)
{
	Core::Network::Request(
			General::Authorization::instance()->server()
					//+ Config::getStringValue(ConfigKeys::apiUrl)
					+ url,
			Core::Session::instance()->authToken(),
			RequestType::Get,
			QJsonDocument(),
			[=](QNetworkReply *reply) {
				if (reply->error()) {
					onComplete(false, reply);
					qDebug() << "Error " << reply->error();
					return;
				}

				onComplete(true, reply);
			},
			[=](qint64 p1, qint64 p2) { onProgress(p1, p2); });
}

} // namespace Core
