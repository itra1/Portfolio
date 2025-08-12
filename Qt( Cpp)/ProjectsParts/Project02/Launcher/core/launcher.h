#ifndef LAUNCHER_H
#define LAUNCHER_H

#include <QObject>
#include <QProcess>
#include <QDebug>
#include <QApplication>
#include <QQmlApplicationEngine>
#include <QFile>
#include <QDir>
#include <QTranslator>

#include "sourcemanager.h"
#include "sourceinfo.h"
#include "installstate.h"

namespace BitTorrent{
    class Session;
    class InfoHash;
    class TorrentHandle;
}

namespace Gui {
    class MainWindow;
}
namespace Core{
    class SourceInfo;
}

namespace Core{

    struct LanguageStruct     //Создаем структуру!
    {
        QString code;
        QString file;
        QString name;
        QLocale locale;
    };

    class Launcher : public QObject
    {
        Q_OBJECT

        Launcher(QObject *parent = nullptr);

    public:

        static void initInstance();
        static void freeInstance();
        static Launcher *instance();

        void initialization(QQmlApplicationEngine *engine);

        void setState(bool isGame, QString guid, PlaceState::State state);
        PlaceState::State getState();

        void checkUpdateComplete();
        void checkUpdateFailed();

        void quit();

        Place *getReadyLauncherVersion();
        Place* getReadyGameVersion();

        //Settings
        void setInstallPath(QString dictionary);
        void setAutoUpdateEnable(bool autoUpdate);
        void setP2pEnable(bool p2pEnable);
        QString getInstallPath();
        bool getAutoUpdateEnable();
        bool getP2pEnable();
        void checkGameCache();
        void restoreDefaultSettings();
        void save();

        void pauseTorrent();

        bool isPlay();
        BitTorrent::TorrentHandle *getTorrentByName(QString name);
        BitTorrent::TorrentHandle *getTorrentSelectVersion();

        QString getLanguage() const;
        void setLanguage(const QString &language);

        void initLanguage();

        QString getSelectGameGuid() const;
        void setSelectGameGuid(const QString &selectGameVersion);

        void showWarningMessage(QString message);

        bool getNeedLauncherUpdate() const;
        void setNeedLauncherUpdate(bool needLauncherUpdate);

        bool existsOtherProcess();
        QString getTranslateName(int num);
        LanguageStruct getLang(int num);
        QList<Core::LanguageStruct> getAllLang();

        bool avalableInstallVersion();

    signals:
        void onSave();
        void onPlay(bool isPlay);
        void onStateChange(PlaceState::State instalState);
        void onStateChangeAny(PlaceState::State instalState);
        void onUpdateTorrent(BitTorrent::TorrentHandle *torrentHandle);
        void onInstallRequest();
        void onLanguageChange();
        void onMessage(QString message);
        void onChangeSelectVersion();
        void onLauncherUpdateReady();

    public slots:
        // Handle base buttons
        void handlePlayButton();
        void handleInstallButton();
        void handleUpdateButton();

        // Handle torrent events
        void torrentAdded(BitTorrent::TorrentHandle *const torrent);
        void torrentFinished(BitTorrent::TorrentHandle *const torrent);
        void torrentFinishedChecking(BitTorrent::TorrentHandle *const torrent);
        void torrentRemoved();
        void torrentsUpdated();

        void handleCheckProcess();

        void handleInstallAgreenmentConfirm();

        void gameInstallPathChange();

    private:
        static Launcher *m_instance;

        bool m_checkUpdateComplete;

        QTimer *m_processCheckTimer;

        QString m_installPath;
        bool m_autoUpdateEnable;
        bool m_p2pEnable;
        QString m_language;
        PlaceState::State m_state;
        bool m_needLauncherUpdate;

        QHash<BitTorrent::InfoHash, BitTorrent::TorrentHandle *> m_torrents;

        QTranslator m_translator;

        QString m_selectGameGuid = "";

        void confirmSettings();

        void updateGame();
        void updateLauncher();
    };
}

#endif // LAUNCHER_H
