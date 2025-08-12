#include "server.h"
#include "../serversmanager.h"
#include <QJsonDocument>
#include <QJsonObject>
#include <QRegExp>
#include <QUuid>

namespace Core
{

	Server::Server(QObject *parent) : SettingsBase(parent)
	{
		readConfigFile("serverSettings");
	}

	Server::Server(QString name, QObject *parent) : SettingsBase(parent)
	{
		readConfigFile("serverSettings");

		setValue("id", QUuid::createUuid().toString());
		setValue("name", name);
	}

	Server::Server(QJsonObject jObj, QObject *parent) : SettingsBase(parent)
	{
		readConfigFile("serverSettings");

		loadJsonObject(jObj);
	}

	bool Server::isValid()
	{
		if (url().length() == 0)
			return false;

		QRegExp rx("^http[s]?://");
		return rx.indexIn(url()) >= 0;
	}

	QJsonObject Server::getJson()
	{
		QJsonObject jObj = makeJsonObject();
		return jObj;
	}

	void Server::save() { Core::ServersManager::instance()->save(); }

	QString Server::id() { return value(QString("id")).toString(); }

	QString Server::url() { return value(QString("url")).toString(); }

	QString Server::name() { return value(QString("name")).toString(); }

} // namespace Core
