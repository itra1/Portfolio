#ifndef SOURCEINFO_H
#define SOURCEINFO_H

#include <QString>
#include "base/bittorrent/torrentinfo.h"

namespace Core{

    class Place;

    class SourceInfo
    {
    public:

        enum InstallState {
            Source = 0,
            Complete = 1,
            Install = 2,
            Update = 3,
            HashChecking = 4
        };

        explicit SourceInfo(Core::Place *place);

        QString version() const;
        QStringList versionBlocks() const;
        void setVersion(const QString &version);

        QString runFile() const;
        void setRunFile(const QString &runFile);
        bool isInstalled() const;
        void setIsInstalled(bool isInstalled);
        InstallState installState() const;
        void setInstallState(const InstallState &installState);
        QString torrentFile() const;
        void setTorrentFile(const QString &torrentFile);
        QByteArray byteArray() const;
        void setByteArray(const QByteArray &byteArray);
        BitTorrent::TorrentInfo torrentInfo() const;
        void setTorrentInfo(const BitTorrent::TorrentInfo &torrentInfo);

        bool installGame(bool isUpdate = false);
        void playTorrent(bool isResume = false);
        void update();
        QString savePath();
        QString exeFile();

        void remove();

        SourceInfo* clone();

        static QString displayVersionNull();
        QString displayVersion();

        static bool firstGreatOrEqualsSecond(SourceInfo *first, SourceInfo *second, int versionBlock = 0);
        static bool firstGreatSecond(SourceInfo *first, SourceInfo *second, int versionBlock = 0);
        static bool firstGreatSecond(SourceInfo *first, QString second, int versionBlock = 0);

        QString runFileFullPath();

        bool isDev() const;
        void setIsDev(const bool &isDev);

        QString noteUrl() const;
        void setNoteUrl(const QString &noteUrl);

        QString rootFolder();

        void checkState();
        InstallState getState();

        Core::Place *getPlace() const;
        void setPlace(Core::Place *place);

    private:
        Core::Place *m_place;
        QString m_version;
        QString m_runFile;
        QString m_torrentFile;
        QString m_noteUrl;
        bool m_isDev;
        InstallState m_installState;
        bool m_isInstalled;
        QByteArray m_byteArray;
        BitTorrent::TorrentInfo m_torrentInfo;
        InstallState m_state;
        QString m_strimeName;

        void writeVersionFile(QString dir);
    };

}

#endif // SOURCEINFO_H
