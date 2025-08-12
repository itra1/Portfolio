#ifndef APPLICATION_H
#define APPLICATION_H

#include <QObject>
#include <QApplication>
#include "base/bittorrent/torrentinfo.h"

namespace BitTorrent{
   class TorrentHandle;
}

class MainWindow;
class CompleteDialog;
class InstallDialog;
class QProcess;
class CopyTheards;

class Application : public QObject
{
    Q_OBJECT
    explicit Application(QObject *parent = nullptr);

public:
    static void initInstance();
    static void freeInstance();
    static Application *instance();

    int initiate(int argc, char *argv[]);

    void downloadIni();
    void downloadTorrent();
    bool load() const;
    void play();

    BitTorrent::TorrentInfo torrent() const;

    void install(QString path);
    void complete();

    void quit();

    QString installPath() const;
    void setInstallPath(const QString &installPath);

    QString runFile() const;
    void setRunFile(const QString &runFile);

    void createIcons();

signals:
    void onLoadTorrent();
    void onProgressCopy(double val);

public slots:
    void handleTorrentAdded(BitTorrent::TorrentHandle *const torrent);
    void handleTorrentFinished(BitTorrent::TorrentHandle *const torrent);
    void handleTorrentFinishedChecking(BitTorrent::TorrentHandle *const torrent);
    void setProgress(double progress);
    void handleTheardFinished();
    void handleStartCopy();


private:

    MainWindow *m_mainDlg;
    InstallDialog *m_installDlg;
    CompleteDialog *m_completeDlg;

    static Application *m_instance;
    BitTorrent::TorrentInfo m_torrent;
    QByteArray m_byteTorrent;
    bool m_load;
    bool m_install;
    QString m_installPath;
    QString m_runFile;
    QProcess *m_runProcess;

    bool m_isLoadIni;
    bool m_isLoadTorrent;

    QThread *m_copyTheard;
    CopyTheards *m_copyTheardObject;

    void handleDownloadIniComplete(const QString &url, const QByteArray &data);
    void handleDownloadIniFailed(const QString &url, const QString &reason);
    void handleDownloadTorrentComplete(const QString &url, const QByteArray &data);
    void handleDownloadTorrentFailed(const QString &url, const QString &reason);
    void playTorrent();
    void checkMoveIni();
    bool SetPrivilege(
        HANDLE hToken,          // access token handle
        LPCTSTR lpszPrivilege,  // name of privilege to enable/disable
        BOOL bEnablePrivilege   // to enable or disable privilege
        );
    void setPermission();
};

#endif // APPLICATION_H
