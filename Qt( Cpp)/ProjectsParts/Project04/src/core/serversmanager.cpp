#include "serversmanager.h"

#include <QDebug>
#include <QFile>
#include <QJsonArray>
#include <QJsonDocument>
#include <QJsonObject>
#include <QObject>
#include "../general/settingsstorage.h"
#include "configcontroller.h"

namespace Core {

ServersManager *ServersManager::_instance{};

ServersManager::ServersManager(QObject *parent)
		: QObject(parent)
{
	load();
}

void ServersManager::initInstance(QObject *parent)
{
	if (!_instance)
		_instance = new ServersManager(parent);
}

void ServersManager::freeInstance()
{
	if (_instance) {
		delete _instance;
		_instance = nullptr;
	}
}

ServersManager *ServersManager::instance()
{
	return _instance;
}

QJsonObject ServersManager::getOneServer(QString serverKey, QString name, QString url)
{
	QJsonObject jNewobject{};
	jNewobject["id"] = serverKey;
	jNewobject["name"] = name;

	if (General::SettingsStorage::instance()->hashValue(serverKey)) {
		qDebug() << "Old data " + serverKey;
		QByteArray data = General::SettingsStorage::instance()->loadValue(serverKey).toByteArray();
		QJsonObject jObj = QJsonDocument::fromJson(data).object();
		jNewobject["url"] = jObj.value("server").toString();
		jNewobject["isMakePreview"] = jObj.value("isMakePreview").toBool();
		jNewobject["isRenderStreaming"] = jObj.value("isRenderStreaming").toBool();
		jNewobject["renderStreamingUrl"] = jObj.value("renderStreamingUrl").toString();
		jNewobject["widgetUpdatePeriod"] = jObj.value("widgetUpdatePeriod").toInt();
		General::SettingsStorage::instance()->remove(serverKey);
	} else {
		jNewobject["url"] = url;
		jNewobject["isMakePreview"] = true;
		jNewobject["isRenderStreaming"] = true;
		jNewobject["renderStreamingUrl"] = url + "/render-streaming/";
		jNewobject["widgetUpdatePeriod"] = 0;
	}
	return jNewobject;
}

void ServersManager::load()
{
	QJsonArray jData = ConfigController::getArray("servers");
	QJsonArray config = loadConfig();

	if (jData.count() > 0) {
		mergeData(jData, config);
	} else
		jData = config;

	if (jData.count() <= 0) {
	}

	readData(jData);
}

void ServersManager::save()
{
	QJsonArray jArray{};

	for (auto elem : *_serverList) {
		jArray.append(elem->getJson());
	}

	ConfigController::setData("servers", jArray);
}

QJsonArray ServersManager::loadConfig()
{
	QByteArray serversData;

	QFile file(":/servers/resources/servers.json");

	try {
		file.open(QFile::ReadOnly);
		serversData = file.readAll();
		file.isOpen();
	} catch (...) {
	}
	if (file.isOpen())
		file.close();

	if (serversData.isEmpty())
		return QJsonArray();

	return QJsonDocument::fromJson(serversData).array();
}

void ServersManager::parseData(QByteArray source)
{
	_serverList = new QList<Server *>();
	auto jArray = QJsonDocument::fromJson(source).array();
	readData(jArray);
}

void ServersManager::readData(QJsonArray jArray)
{
	_serverList = new QList<Server *>();

	for (int i = 0; i < jArray.count(); i++) {
		auto jObj = jArray[i].toObject();
		_serverList->append(new Server(jObj));
	}
	emit serversLoadSignal();
}

void ServersManager::mergeData(QJsonArray &source, QJsonArray &config)
{
	for (int i = 0; i < config.count(); i++) {
		auto configItem = config[i].toObject();

		for (int ii = 0; ii < source.count(); ii++) {
			auto sourceItem = source[ii].toObject();
			if (sourceItem["id"] != configItem["id"])
				continue;

			bool esistsChnage = false;
			for (int iii = 0; iii < configItem.keys().count(); iii++) {
				auto key = configItem.keys()[iii];
				if (!sourceItem.contains(key)) {
					sourceItem.insert(key, configItem[key]);
					esistsChnage = true;
				}
			}

			if (esistsChnage) {
				source.removeAt(ii);
				source.insert(ii, sourceItem);
			}
		}
	}
}

void ServersManager::createServer(QString name)
{
	Server *srv = new Server(name);
	_serverList->append(srv);
	save();
}

void ServersManager::removeServer(QString serverId)
{
	int index{-1};

	for (int i = 0; i < _serverList->length(); i++) {
		if (_serverList->value(i)->id() == serverId)
			index = i;
	}
	if (index != -1) {
		_serverList->removeAt(index);
	}
	save();
}

Server *ServersManager::getServerByIndex(int index)
{
	return _serverList->value(index);
}

Server *ServersManager::getServerById(QString serverId)
{
	for (auto elem : *_serverList)
		if (elem->id() == serverId)
			return elem;
	return nullptr;
}

QList<Server *> *ServersManager::getServers()
{
	return _serverList;
}

QVariantList ServersManager::getServersToQml()
{
	QVariantList list;

	for (auto elem : *_serverList) {
		list.append(QVariant::fromValue(elem));
	}
	return list;
}
} // namespace Core
