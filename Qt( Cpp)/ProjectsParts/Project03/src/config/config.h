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

    static const QString getPath(QString key) { return currentPath() + getStringValue(key); }
    static QString getStringValue(QString key);
    static int getIntValue(QString key);

private:
    static QList<Configs::ConfigItem *> *_keys;

private:
    void readConfigFile();
};

#endif // CONFIG_H
