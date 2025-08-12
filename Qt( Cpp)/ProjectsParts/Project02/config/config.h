#ifndef CONFIG_H
#define CONFIG_H

#include <QString>
#include <QDir>

namespace Config {

    //Group: General
    const char PEER_ID[] = "qpzl";
    const char RESUME_FOLDER[] = "qpzl_backup";
    const char USER_AGENT[] = "qpzlauncher/" PZL_VERSION;

    const QString SERVER_NAME = "http://pzonline.com";
    const QString SING_IN_URL = "https://account.pzonline.com/ru/auth/pz?redirect_url=%1";
    const QString SING_IN_URL_SUCCESS = "https://account.pzonline.com/auth/pz/success";
    const QString USER_INFO_URL = "https://account.pzonline.com/api/launcher/auth/pz?token=%1";

    // Extension for torrents files (alternative *.torrent)
    const QString TORRENT_EXTENSION = "res";
    const int MAX_TORRENT_CONNECTIONS_PEER = 10;
    const int MAX_TORRENT_UPLOAD_SPEED = 999999999;
    const int TORRENT_PORT = 8736;

    //Group: Game
    // Start path to install game, if not set userPath
    const QString GAME_INSTALL_DEFAULT_PATH = QDir::currentPath() + "/apps";
    // File 'ini' for record download state
    const QString GAME_INI_FILE = "source.ini";
    const QString GAME_TORRENT_FILE = "source.res";

    //Group: Updater
    const QString UPDATER_PATH = QDir::currentPath() + "/update";
    const QString TMP_PATH_ROOT = QDir::tempPath() + "/pzLancher";

    const QString SERVER_ROOT = "http://files.netarchitect.ru/pzLauncher/torrents/";

//    const QString SERVER_GAME_RELEASE_TORRENT = "http://files.netarchitect.ru/pzLauncher/torrents/game/release";
//    const QString SERVER_LAUNCHER_DEV_TORRENT = "http://files.netarchitect.ru/pzLauncher/torrents/game/dev";
//    const QString SERVER_GAME_ALPHA_TORRENT = "http://files.netarchitect.ru/pzLauncher/torrents/game/alpha";
//    const QString SERVER_GAME_BETA_TORRENT = "http://files.netarchitect.ru/pzLauncher/torrents/game/beta";
//    const QString SERVER_LAUNCHER_RELEASE_TORRENT = "http://files.netarchitect.ru/pzLauncher/torrents/launcher";

    const QString SERVER_LAUNCHER_RELEASE_TORRENT = "https://dl.pzonline.com/launcher";
    const QString SERVER_LAUNCHER_DEV_TORRENT = "https://dl.pzonline.com/launcher-dev";
    const QString SERVER_GAME_RELEASE_TORRENT = "https://dl.pzonline.com/release";
    const QString SERVER_GAME_ALPHA_TORRENT = "https://dl.pzonline.com/alpha";
    const QString SERVER_GAME_BETA_TORRENT = "https://dl.pzonline.com/beta";

    const QString SERVER_FILE_INFO = "latest.ini";
    const QString SERVER_FILE_TORRENT = "latest.res";

}

#endif // CONFIG_H
