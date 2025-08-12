#ifndef SOURCEMANAGER_H
#define SOURCEMANAGER_H

#include <QObject>
#include <QList>
#include <QHash>
#include <QString>

#include "sourceinfo.h"
#include "place.h"


namespace Core {

    class SourceInfo;
    class IniEditor;

    class SourceManager : public QObject
    {
        Q_OBJECT
    public:

        static void initInstance();
        static void freeInstance();
        static SourceManager *instance();

        void initLocaPlace();
        void clearLocalSpace();
        void initialization();
        void checkLocalGamePlace();
        bool isPlay();
        bool existsUpdateProcessGame();
        bool existsInstallProcessGame();
        bool existsHashCkeckingProcessGame();

        QList<Place *> getGameAvailableVersions();

        Place* getSource(QString guid, bool isGame = true);
        Place* getSourceLauncher();

        void sourceDownloadComplete(QString name);

        QList<Place *> *getPlaceList() const;
        void setPlaceList(QList<Place *> *placeList);

    signals:
        void onAddNewSource(SourceInfo*);

        void onNullGameReady();
        void onInstallGameReady();
        void onInstallGameProgress();
        void onUpdateGameReady();
        void onPlayGameReady();

        void onLauncherUpdateReady();

    public slots:

        void handleStartInstallGame(SourceInfo *si);

    private:
        explicit SourceManager(QObject *parent = nullptr);
        ~SourceManager();
        static SourceManager *m_instance;
        QHash<QString, SourceInfo*> *m_torrentLoad;
        QList<Place*> *m_placeList;

        void checkStateGame();
        void checkStateLauncher();
    };
}

#endif // SOURCEMANAGER_H
