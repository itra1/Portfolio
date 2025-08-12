#include "iohelpers.h"
#include "config/config.h"

#include <QDir>
#include <QGuiApplication>

QString IOHelpers::macosPath = QString();

IOHelpers::IOHelpers() {}

QString IOHelpers::currentPath() {
#ifdef TARGET_OS_MAC
  return QGuiApplication::applicationDirPath() + "/../.."; //+ "/.."; // QDir::currentPath(); // + "/../..";
#else
  return QDir::currentPath();
#endif
}
