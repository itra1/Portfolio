#ifndef PLACE_H
#define PLACE_H

#include <QObject>
#include "sourceinfo.h"
#include "placestate.h"

namespace Core{

    class Place : public QObject
    {
        Q_OBJECT
    public:

        explicit Place(QObject *parent = nullptr);
        ~Place();

        virtual bool isGame() = 0;
        virtual void checkState() = 0;
        virtual void update() = 0;

        void init();

        QString getStreamName() const;
        void setStreamName(const QString &streamName);

        void findLocalSource();
        void downloadInfo();
        void downloadTorrent();

        PlaceState::State getState() const;
        void setState(const PlaceState::State &state);
        bool isProcessState();

        QString torrentName();
        QString typeToString();

        SourceInfo *getSourceLocal() const;
        void setSourceLocal(SourceInfo *sourceLocal);

        SourceInfo *getSourceServer() const;
        void setSourceServer(SourceInfo *sourceServer);

        void installComplete();

        QString getGuid() const;
        void setGuid(const QString &guid);

    signals:
        void onStateChange(PlaceState::State state);

    public slots:
        void handleDownloadInfoComplete(const QString &url, const QByteArray &data);
        void handleDownloadInfoFailed(const QString &url, const QString &reason);
        void handleDownloadTorrentComplete(const QString &url, const QByteArray &data);
        void handleDownloadTorrentFailed(const QString &url, const QString &reason);

    private:
        virtual QString getSourceUrl() = 0;

    protected:
        QString m_streamName;
        QString m_guid;
        SourceInfo *m_sourceLocal;
        SourceInfo *m_sourceServer;
        PlaceState::State m_state;
        bool m_firstInit;

        QString getInfoUrl();
        QString getTorrentUrl();

    };
}

#endif // PLACE_H
