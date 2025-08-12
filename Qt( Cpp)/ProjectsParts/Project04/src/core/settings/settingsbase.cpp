#include "settingsbase.h"
#include "../settingsjsoncontroller.h"
#include "settingsitemsfactory.h"

namespace Core {
SettingsBase::SettingsBase(QObject *parent) {}

SettingsBase::SettingsBase(QJsonObject jObject, QObject *parent)
    : QObject{parent}
{}

QVariantList SettingsBase::optionsQmlList()
{
    QVariantList list;

    for (auto elem : *_keyList) {
        if (elem != nullptr && !elem->type().contains("hidden"))
            list.append(QVariant::fromValue(elem));
    }
    return list;
}

void SettingsBase::subscribeChange()
{
    for (auto *key : *_keyList) {
        if (key == nullptr)
            continue;

        connect(key, SIGNAL(valueChange(SettingsItem *)), SLOT(itemChange(SettingsItem *)));
    }
}

void SettingsBase::readConfigFile(QString key)
{
    auto docArr = Core::SettingsJsonController::getArray(key);

    _keyList = new QList<Core::SettingsItem *>();

    for (auto elem : docArr) {
        _keyList->append(Core::SettingsItemsFactory::getItem(elem.toObject()));
    }
}

void SettingsBase::loadJsonObject(QJsonObject jObj)
{
    for (auto *key : *_keyList) {
        if (key == nullptr)
            continue;
        auto propKey = key->key();
        if (jObj.contains(propKey))
            key->setValueAsVariant(jObj.value(propKey).toVariant());
    }
    subscribeChange();
}

void SettingsBase::setValue(QString keyName, QVariant value)
{
    for (auto *key : *_keyList) {
        if (key == nullptr)
            continue;
        auto propKey = key->key();
        if (propKey == keyName)
            key->setValueAsVariant(value);
    }
}

QVariant SettingsBase::value(QString keyName)
{
    for (auto *key : *_keyList) {
        if (key == nullptr)
            continue;
        auto propKey = key->key();
        if (propKey == keyName)
            return key->valueAsVariant();
    }
    return QVariant();
}

QJsonObject SettingsBase::makeJsonObject()
{
    QJsonObject jObj;

    for (auto *key : *_keyList) {
        if (key == nullptr)
            continue;
        auto propKey = key->key();
        jObj[propKey] = key->valueAsJsonValue();
    }
    return jObj;
}

QString SettingsBase::getRunWallKeys()
{
    QString runKeys = "";
    for (auto *key : *_keyList) {
        if (key != nullptr)
            runKeys.append(key->runProperty());
    }
    return runKeys;
    // return "";
}

void SettingsBase::makeRunWallKey(QStringList &opt, bool cross)
{
    for (auto *key : *_keyList) {
        if (key != nullptr) {
            auto keyRun = key->runProperty();
            if (keyRun.length() > 0) {
                if (cross && key->type() == "toggle")
                    opt << (cross ? "-" : "") + keyRun + "=true";
                else
                    opt << (cross ? "-" : "") + keyRun.replace(";","_");
            }
        }
    }
}

void SettingsBase::itemChange(SettingsItem *item)
{
    save();
}
} // namespace Core
