#include "config.h"
#include <QJsonArray>
#include <QJsonDocument>
#include <QJsonObject>
#include "src/iohelper.h"

QList<Configs::ConfigItem *> *Config::_keys = new QList<Configs::ConfigItem *>();

Config::Config()
{
  readConfigFile();
}

void Config::readConfigFile()
{
    auto readData = QtSystemLib::IOHelper::TextFileRead(":/ConfigResources/resources/config.json");

    auto jArray = QJsonDocument::fromJson(readData).array();

    for (auto jItem : jArray) {
        auto elem = jItem.toObject();

        auto inst = new Configs::ConfigItem();

        inst->key = elem["key"].toString();
        inst->value = elem["value"];
        inst->description = elem["description"].toString();

        _keys->append(inst);
	}
}

QString Config::getStringValue(QString key)
{
  for (auto *jItem : *_keys) {
    if (jItem->key == key)
      return jItem->value.toString();
  }
  return nullptr;
}

int Config::getIntValue(QString key)
{
  for (auto *jItem : *_keys) {
    if (jItem->key == key)
      return jItem->value.toInt(0);
  }
  return 0;
}
