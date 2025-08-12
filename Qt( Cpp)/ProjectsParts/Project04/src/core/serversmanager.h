#ifndef SERVERSMANAGER_H
#define SERVERSMANAGER_H

#include <QObject>
#include <QQmlApplicationEngine>
#include <QQmlContext>
#include "settings/settingsitem.h"
#include "structs/server.h"

namespace Core {

class ServersManager : public QObject
{
	Q_OBJECT
	public:
	ServersManager(const ServersManager &) = delete;
	ServersManager &operator=(const ServersManager &) = delete;
	// void readConfigFile();

	explicit ServersManager(QObject *parent = nullptr);
	static void initInstance(QObject *parent = nullptr);
	static void freeInstance();
	static ServersManager *instance();

	static QJsonObject getOneServer(QString serverKey, QString name, QString url);

	void load();
	void save();

	QJsonArray loadConfig();
	void parseData(QByteArray source);
	void readData(QJsonArray array);
	void mergeData(QJsonArray &source, QJsonArray &config);
	Q_INVOKABLE void createServer(QString name);
	Q_INVOKABLE void removeServer(QString serverId);
	Q_INVOKABLE QVariantList getServersToQml();
	Server *getServerByIndex(int index);
	Server *getServerById(QString serverId);
	QList<Core::Server *> *getServers();

	signals:
	void serversLoadSignal();

	private:
	static ServersManager *_instance;
	// QList<Core::SettingsItem *> *_keyList;
	QList<Core::Server *> *_serverList;
};
} // namespace Core
#endif // SERVERSMANAGER_H
