#ifndef STORAGE_H
#define STORAGE_H

#include <QObject>
#include <QSettings>
#include <QDebug>

class Storage : public QObject
{
  Q_OBJECT
  explicit Storage(QObject *parent = nullptr);

public:
  static void initInstance();
  static void freeInstance();
  static Storage *instance();

  QVariant loadValue(const QString &key, const QVariant &defaultValue = QVariant()) const;
  void storeValue(const QString &key, const QVariant &value);
  bool hashValue(const QString &key);
  void remove(const QString &key);

  void saveSync();

  bool isDirty();

signals:

public slots:
  bool save();

private:
  bool m_dirty;
  static Storage *_instance;
  QSettings *m_settings;
};

#endif // STORAGE_H
