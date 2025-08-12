#include "storage.h"

Storage *Storage::_instance = nullptr;

Storage::Storage(QObject *parent)
    : QObject(parent), m_settings{new QSettings("Garilla", "Launcher")} {}

void Storage::initInstance() {
  if (!_instance)
    _instance = new Storage;
}

void Storage::freeInstance() {
  if (_instance) {
    delete _instance;
    _instance = nullptr;
  }
}

Storage *Storage::instance() { return _instance; }

QVariant Storage::loadValue(const QString &key,
                            const QVariant &defaultValue) const {
  return m_settings->value(key, defaultValue);
}

void Storage::storeValue(const QString &key, const QVariant &value) {
  m_dirty = true;
  m_settings->setValue(key, value);
}

bool Storage::hashValue(const QString &key) {
  return m_settings->contains(key);
}

void Storage::remove(const QString &key) { m_settings->remove(key); }

void Storage::saveSync() {
  m_dirty = false;
  m_settings->sync();
}

bool Storage::isDirty() { return m_dirty; }

bool Storage::save() {
  saveSync();
  return !m_dirty;
}
