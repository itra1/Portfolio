#ifndef CONFIG_H
#define CONFIG_H

#include <QCoreApplication>
#include <QDir>
#include <QStandardPaths>
#include <QString>

#include "configItem.h"
#include "configKeys.h"

class Config
{
public:
  explicit Config();

  static QString currentPath() { return QCoreApplication::applicationDirPath(); }
  static const QString cachePath()
  {
    return qgetenv("USERPROFILE") + getStringValue(ConfigKeys::cachePath);
  }
  static const QString getPath(QString key) { return currentPath() + getStringValue(key); }
  static const QString cacheBrowserPath()
  {
    return cachePath() + getStringValue(ConfigKeys::cacheBrowserPath);
  }
  static const QString distribPath()
  {
    return currentPath() + getStringValue(ConfigKeys::distribFolder);
  }
  static const QString videoWallLogFolder()
  {
    return QStandardPaths::standardLocations(QStandardPaths::HomeLocation).constLast()
	   + getStringValue(ConfigKeys::cachePath);
  }
  static const QString configFile()
  {
    return currentPath() + getStringValue(ConfigKeys::clientConfig);
  }
  static QString getStringValue(QString key);
  static int getIntValue(QString key);

private:
  static QList<Configs::ConfigItem *> *_keys;

private:
  void readConfigFile();
};

#endif // CONFIG_H
