#ifndef MAINWINDOW_H
#define MAINWINDOW_H

#include <QApplication>
#include <QObject>
#include <QDebug>
#include <QApplication>
//#include <QtWebView>
#include <QtWebEngine>
#include <QQmlApplicationEngine>
#include <QSystemTrayIcon>
#include <QStyle>
#include <QFileDialog>

#include "core/launcher.h"
#include "core/internet.h"
#include "core/usersession.h"
#include "base/bittorrent/session.h"
#include "base/bittorrent/torrenthandle.h"
#include "core/installstate.h"
#include "core/sourceinfo.h"

#include "base/utils/misc.h"

namespace Core{
    class Launcher;
    class Internet;
    class Place;
    class GamePlace;
    class SettingsStorage;
    class UserSession;
}
namespace BitTorrent{
    class Session;
}

namespace Gui{

    class MainWindow : public QObject
    {

        Q_OBJECT
    public:

        explicit MainWindow(QQmlApplicationEngine *engine, QObject *parent = nullptr);

        static void initInstance(QQmlApplicationEngine *engine, QObject *parent);
        static void freeInstance();
        static MainWindow *instance();

        void loadMainWindow();
        Q_INVOKABLE void mainWindowsLoaded();

        Q_INVOKABLE void closeApplication();
        Q_PROPERTY(Core::PlaceState::State installState READ getState NOTIFY onInstallStateChange)

        Core::PlaceState::State getState();

        Q_INVOKABLE void playButton();
        Q_INVOKABLE void installButton();
        Q_INVOKABLE void updateGame();

        //Installed
        Q_PROPERTY(QString updateVersion READ getUpdateVersion)
        Q_PROPERTY(QString updateSpaceNeed READ getUpdateSpaceNeed)
        Q_PROPERTY(QString versionInstallReady READ getVersionInstallReady)
        Q_PROPERTY(QString noteUrl READ getNoteUrl)

        QString getUpdateVersion();
        QString getUpdateSpaceNeed();
        QString getVersionInstallReady();
        QString getNoteUrl();

        Q_PROPERTY(double progress READ getProgress NOTIFY onInstallUpdate)
        Q_PROPERTY(bool isCheckingProgress READ isCheckingProgress NOTIFY onInstallUpdate)
        Q_PROPERTY(QString readySize READ getReadySize NOTIFY onInstallUpdate)
        Q_PROPERTY(QString fullSize READ getFullSize NOTIFY onInstallUpdate)
        Q_PROPERTY(QString speedLoad READ getSpeedLoad NOTIFY onInstallUpdate)
        Q_INVOKABLE QStringList getGameVersionsList();
        Q_INVOKABLE int getGameVersionsIndex();
        Q_INVOKABLE void setGameVersionsView(int index);

        Q_INVOKABLE void installPathFileDialog();

        double getProgress();
        bool isCheckingProgress();
        QString getReadySize();
        QString getFullSize();
        QString getSpeedLoad();

        Q_PROPERTY(bool internetAvalable READ internetAvalable)
        bool internetAvalable();

        Q_PROPERTY(bool isProcess READ isProcess NOTIFY onStateChangeAny)
        bool isProcess();

        Q_PROPERTY(bool avalableInstallVersion READ avalableInstallVersion NOTIFY onStateChangeAny)
        bool avalableInstallVersion();

        // Settings
        Q_INVOKABLE void settingsButton();
        Q_INVOKABLE void saveButton();
        Q_INVOKABLE void restoreDefaultSettings();
        Q_INVOKABLE void checkGameCacheButton();
        Q_INVOKABLE void setInstallPath(QUrl path);
        Q_INVOKABLE void setLanguageView(int index);
        Q_INVOKABLE QStringList getLanguageList();
        Q_INVOKABLE int getLanguageIndex();

        Q_PROPERTY(bool isSettings READ getIsSettings NOTIFY onIsSettings)
        Q_PROPERTY(bool isAutomaticUpdate READ getIsAutomaticUpdate WRITE setIsAutomaticUpdate NOTIFY onIsAutomaticUpdateChange)
        Q_PROPERTY(bool isP2pDownload READ getIsP2pDownload WRITE setIsP2pDownload NOTIFY onIsP2pDownloadChange)
        Q_PROPERTY(QString installationPath READ getInstallPath NOTIFY onInstallPathChange)
        Q_PROPERTY(bool isCorrectPath READ getIsCorrectInstallPath NOTIFY onInstallPathChange)
        Q_PROPERTY(bool isDirty READ getIsDirty NOTIFY onIsDirtyChange)
        Q_PROPERTY(bool isPlay READ getIsPlayGame NOTIFY onPlayGame)
        Q_PROPERTY(QString mainPageUrl READ getMainPageUrl NOTIFY onMainPageUrlChange)

        Q_INVOKABLE void checkInstallPath(QString path);

        bool isUserAuth();

        bool getIsSettings();
        bool getIsAutomaticUpdate();
        void setIsAutomaticUpdate(const bool isAutomaticUpdate);
        bool getIsP2pDownload();
        void setIsP2pDownload(const bool isP2pDownload);
        QString getInstallPath();
        void setInstallPath(QString newDict);
        bool getIsDirty();
        void setIsDirty(const bool isDirty);
        QString getMainPageUrl();

        bool getIsPlayGame();

        void setLaunchUpdateReady(bool launchUpdateReady);

        bool getIsCorrectInstallPath();

        Q_INVOKABLE void agreementConfirm(bool isConfirm);

        Q_INVOKABLE void loadAgreement();
        Q_INVOKABLE QString getAgreement();

        Q_INVOKABLE void pauseDownload();
        Q_PROPERTY(bool isPauseTorrent READ getIsPauseTorrent NOTIFY onPauseTorrent)
        bool getIsPauseTorrent();

        void setIsPauseTorrent(bool isPauseTorrent);

        QString getLanguage() const;
        void setLanguage(const QString &language);

        // Authorization
        Q_INVOKABLE void logIn();
        Q_INVOKABLE void logOut();
        Q_PROPERTY(bool isUserAuth READ isUserAuth NOTIFY onUserAuthChange)
        Q_PROPERTY(QString userName READ userName NOTIFY onUserAuthChange)
        QString userName();
        Q_PROPERTY(QString userLink READ userLink NOTIFY onUserAuthChange)
        QString userLink();

        // Warning message
        Q_PROPERTY(bool showWarningMessage READ getShowWarningMessage NOTIFY onWarningMessage)
        Q_INVOKABLE void setShowWarningMessage(bool showWarningMessage);
        Q_INVOKABLE QString getWarningMessage();
        bool getShowWarningMessage() const;

        Q_PROPERTY(bool launcherNeedUpdate READ getLauncherNeedUpdate NOTIFY onNeedLauncherUpdate)
        bool getLauncherNeedUpdate() const;
        void setLauncherNeedUpdate(bool launcherNeedUpdate);

    signals:

        void onLoadComplete();

        //// QML
        // Install State
        void onInstallStateChange();

        void onInstallUpdate();
        void onStateChange();
        void onStateChangeAny();
        void onInternetActive();

        // Settings
        void onIsSettings();
        void onIsAutomaticUpdateChange();
        void onIsP2pDownloadChange();
        void onInstallPathChange();
        void onIsDirtyChange();
        void onPlayGame();

        void onInstallRequestShow();
        void onInstallRequestFalse();
        void onLoadAgreement();
        void onPauseTorrent();

        void onUserAuthChange();

        void onWarningMessage();
        void onChangeVersion();
        void onNeedLauncherUpdate();

        void onMainPageUrlChange();

    public slots:
        void stateChange(Core::PlaceState::State state);
        void updateProgress(BitTorrent::TorrentHandle *torrentHandle);
        void handlePlayGame(bool isPlay);
        void handleInstallRequest();

    private:
        static MainWindow *m_instance;
        bool m_isSettingsEnable;

        // Settings
        bool m_isDirty;
        bool m_isP2pDownload;
        bool m_isAutomaticUpdate;
        QString m_installPath;
        bool m_isPauseTorrent;

        QSystemTrayIcon *trayIcon;

        void startSettingsParametrs();

        QString m_agreemetText;
        QQmlApplicationEngine *m_engine;
        QString m_language;

        bool m_showWarningMessage;
        QString m_warningMessage;

        bool m_launcherNeedUpdate;
        bool m_internetStatus;

    };
}

#endif // MAINWINDOW_H
