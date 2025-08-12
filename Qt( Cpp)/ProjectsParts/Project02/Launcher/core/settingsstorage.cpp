#include "settingsstorage.h"

using namespace Core;

SettingsStorage *SettingsStorage::m_instance = nullptr;

SettingsStorage::SettingsStorage(QObject *parent) : QObject(parent),
    m_settings{new QSettings("pzlauncher", "launcher")}
{
}

void SettingsStorage::initInstance()
{
    if (!m_instance)
        m_instance = new SettingsStorage;
}

void SettingsStorage::freeInstance()
{
    if (m_instance) {
        delete m_instance;
        m_instance = nullptr;
    }
}

SettingsStorage *SettingsStorage::instance()
{
    return m_instance;
}



QVariant SettingsStorage::loadValue(const QString &key, const QVariant &defaultValue) const
{
    return m_settings->value(key,defaultValue);
}

void SettingsStorage::storeValue(const QString &key, const QVariant &value)
{
    m_dirty = true;
    m_settings->setValue(key,value);
}

bool SettingsStorage::hashValue(const QString &key)
{
    return m_settings->contains(key);
}

void SettingsStorage::remove(const QString &key)
{
    m_settings->remove(key);
}

void SettingsStorage::saveSync()
{
    qDebug() << "Save";
    m_dirty = false;
    m_settings->sync();
}

bool SettingsStorage::isDirty()
{
    return m_dirty;
}

bool SettingsStorage::save()
{
    saveSync();
    return !m_dirty;
}
