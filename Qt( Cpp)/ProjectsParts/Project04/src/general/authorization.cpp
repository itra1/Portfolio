#include "authorization.h"
#include <QEventLoop>
#include <QNetworkAccessManager>
#include <QNetworkReply>
#include <QVariant>
#include <QtNetwork>
#include "../core/api.h"
#include "./../config/config.h"
#include "./../core/serversmanager.h"
#include "./../core/session.h"
#include "./../core/structs/server.h"
#include "./../general/settingsstorage.h"
#include "./../network/network.h"

namespace General {

Authorization *Authorization::_instance = nullptr;

Authorization::Authorization(QObject *parent)
		: QObject(parent)
		, _remember(false)
{
	_userName = SettingsStorage::instance()->loadValue("login", "").toString();
	_password = SettingsStorage::instance()->loadValue("password", "").toString();
	_remember = SettingsStorage::instance()->loadValue("remember", false).toBool();
	_serverId = SettingsStorage::instance()->loadValue("serverId", false).toString();
}

void Authorization::initInstance()
{
	if (!_instance)
		_instance = new Authorization();
}

void Authorization::freeInstance()
{
	if (_instance) {
		delete _instance;
		_instance = nullptr;
	}
}

Authorization *Authorization::instance()
{
	return _instance;
}

void Authorization::login(QString userName, QString password, bool isRemember, QString serverId)
{
	if (_isAuthProcess)
		return;

	setIsAuthProcess(true);

	if (userName.length() == 0 || password.length() == 0) {
		emit authError("Форма не заполнена");
		return;
	}

	_serverId = serverId;
	_userName = userName;
	_password = password;
	_remember = isRemember;

	auth();
}

void Authorization::authorizationStart()
{
	if (!_remember)
		return;
	auth();
}

void Authorization::auth()
{
	auto server = Core::ServersManager::instance()->getServerById(_serverId);

	if (server == nullptr)
		return;

	_server = server->url();
	QString targetServer = _server;

	_server = targetServer + Config::getStringValue(ConfigKeys::apiUrl);

	if (_remember) {
		SettingsStorage::instance()->storeValue("login", _userName);
		SettingsStorage::instance()->storeValue("password", _password);
		SettingsStorage::instance()->storeValue("remember", _remember);
		SettingsStorage::instance()->storeValue("serverId", _serverId);
		SettingsStorage::instance()->saveSync();
	} else {
		SettingsStorage::instance()->remove("login");
		SettingsStorage::instance()->remove("password");
		SettingsStorage::instance()->remove("remember");
		SettingsStorage::instance()->remove("serverId");
		SettingsStorage::instance()->saveSync();
	}

	authStart();
}

void Authorization::authStart()
{
	Core::Api::authorization(_userName, _password, [&](bool complete, QNetworkReply *reply) {
		loginAuthFinish(reply);
	});
}

void Authorization::loginAuthFinish(QNetworkReply *reply)
{
    setIsAuthProcess(false);
    if (reply->error()) {
		// Сообщаем об этом и показываем информацию об ошибках
        qDebug() << reply->errorString();
        auto stringError = reply->errorString().split(" - ")[1].split(":")[1].trimmed();
        _instance->setIsAuthProcess(false);
        emit _instance->authError(stringError);
		return;
	}

	QByteArray res(reply->readAll());
	QString str(res);

	QJsonDocument document = QJsonDocument::fromJson(res);
	QJsonObject root = document.object();

	auto token = root.find("token").value().toString();

	Core::User newUser{root.value("user").toObject()};
	Core::Session::instance()->setUser(newUser);
	Core::Session::instance()->setAuthToken(token);
	Core::Session::instance()->setIsAuth(true);
    setIsAuthProcess(false);
}

QString Authorization::userName()
{
	return _userName;
}

QString Authorization::password()
{
	return _password;
}

QString Authorization::serverId()
{
	return _serverId;
}

QString Authorization::serverName()
{
	return Core::ServersManager::instance()->getServerById(_serverId)->name();
}

void Authorization::setServer(const QString &newServer)
{
	_server = newServer;
}

void Authorization::setIsAuthProcess(bool isAuthprocess)
{
	_isAuthProcess = isAuthprocess;
	emit isAuthStart(_isAuthProcess);
}

const QString Authorization::server() const
{
	return _server;
}

const bool Authorization::remember() const
{
	return _remember;
}

void Authorization::setRemember(bool newRemember)
{
	_remember = newRemember;
}
} // namespace General
