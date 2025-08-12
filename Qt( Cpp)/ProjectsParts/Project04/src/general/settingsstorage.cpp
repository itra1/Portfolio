#include "settingsstorage.h"

using namespace General;

SettingsStorage *SettingsStorage::_instance = nullptr;

SettingsStorage::SettingsStorage(QObject *parent)
		: QObject(parent), _settings{new QSettings("kc", "launcher")}
{
}

void SettingsStorage::initInstance()
{
	if (!_instance)
		_instance = new SettingsStorage;
}

void SettingsStorage::freeInstance()
{
	if (_instance) {
		delete _instance;
		_instance = nullptr;
	}
}

SettingsStorage *SettingsStorage::instance() { return _instance; }

QVariant SettingsStorage::loadValue(const QString &key, const QVariant &defaultValue) const
{
	return _settings->value(key, defaultValue);
}

void SettingsStorage::storeValue(const QString &key, const QVariant &value)
{
    _dirty = true;
		_settings->setValue(key, value);
}

bool SettingsStorage::hashValue(const QString &key)
{
	return _settings->contains(key);
}

void SettingsStorage::remove(const QString &key) { _settings->remove(key); }

void SettingsStorage::saveSync()
{
    _dirty = false;
		_settings->sync();
}

bool SettingsStorage::isDirty()
{
    return _dirty;
}

bool SettingsStorage::save()
{
    saveSync();
    return !_dirty;
}
