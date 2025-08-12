/*
 * Контроллер файла конфигурации
 */

#ifndef CONFIGCONTROLLER_H
#define CONFIGCONTROLLER_H

#include <QList>
#include <QJsonDocument>
#include <QJsonObject>
#include "structs/server.h"

namespace Core{

  class ConfigController
  {
  public:
    ConfigController(const ConfigController&) = delete;
    ConfigController& operator=(const ConfigController&) = delete;

    ConfigController();
    static void load();
    static void save();

    static void setData(QString key, QJsonObject data);
    static void setData(QString key, QJsonArray data);
    static QJsonValue getValue(QString key);
    static QJsonObject getObject(QString key);
    static QJsonArray getArray(QString key);

  private:
    static QJsonObject _document;

  };
}
#endif // CONFIGCONTROLLER_H
