#ifndef SERVERS_H
#define SERVERS_H

#include <QObject>
#include <QJsonObject>
#include <QJsonArray>
#include <QByteArray>

class ServersData{

public:
  ServersData(){ };
  QByteArray sources;
  QJsonObject jObject;
  QList<QString> Game;
  QList<QString> Delivery;
  QList<QString> Apps;

  static ServersData *Parce(QJsonObject jDoc, QByteArray sourceBytes){
    ServersData *servers = new ServersData();
    servers->jObject = jDoc;
    servers->sources = sourceBytes;
    QJsonObject dt = jDoc.value("servers").toObject();
    QJsonArray gameArray = dt.value("game").toArray();
    QJsonArray deliveryArray = dt.value("delivery").toArray();
    QJsonArray appsArray = dt.value("apps").toArray();
    servers->Game.clear();
    servers->Delivery.clear();
    servers->Apps.clear();

    for(int i = 0 ; i < gameArray.count();i++)
      servers->Game.append(gameArray[i].toString());

    for(int i = 0 ; i < deliveryArray.count();i++)
      servers->Delivery.append(deliveryArray[i].toString());

    for(int i = 0 ; i < appsArray.count();i++)
      servers->Apps.append(appsArray[i].toString());

    return servers;
  };
};

class Servers : public QObject
{
  Q_OBJECT
public:
  explicit Servers(QObject *parent = nullptr);
  static void initInstance();
  static void freeInstance();
  static Servers *instance();
  void readUrlLinks();
  void parseServersData(QByteArray data);
  QByteArray getServersDataBase64();
  QString server();
  QString serverApi();
  bool serversLoaded();

signals:


private:
  static Servers *_instance;
  ServersData* servers;
  void EmitSetServers();


signals:
  void OnSetServer();

};

#endif // SERVERS_H
