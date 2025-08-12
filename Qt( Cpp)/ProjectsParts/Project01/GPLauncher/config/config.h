#ifndef CONFIG_H
#define CONFIG_H

#include <QString>
#include <QDir>
#include "helpers/iohelpers.h"

namespace Config {

  //static QString server = "https://apps.eximkaubad.com";
  //static QString serverApi = server + "/api/v1";

  static QString downloadPath() { return IOHelpers::currentPath() + "/download";};
  static QString unpackPath() { return IOHelpers::currentPath() + "/temp";};
  static QString gamePath() { return IOHelpers::currentPath() + "/game";};
  static QString gamePathExe() { return gamePath() + "/GarillaPoker.exe";};
  static QString gamePathMacExe() { return gamePath() + "/GarillaPoker.app/Contents/MacOS/GarillaPoker";};
  static QString gameInfoPath() { return gamePath() + "/release";};
  static QString launcerListUrlWindows = "/getLauncherList";
  static QString launcerListUrlMac = "/getLauncherList?os=macos";
  static QString gameListUrlWindows = "/getGameList";
  static QString gameListUrlMac = "/getGameList?os=macos";
}

#endif // CONFIG_H
