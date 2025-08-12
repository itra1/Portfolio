/*
 * Bittorrent Client using Qt and libtorrent.
 */

#ifndef NET_DOWNLOADHANDLER_H
#define NET_DOWNLOADHANDLER_H

#include <QNetworkReply>
#include <QObject>

#include "downloadmanager.h"

class QUrl;

namespace Net
{
    class DownloadManager;

    class DownloadHandler : public QObject
    {
        Q_OBJECT
        Q_DISABLE_COPY(DownloadHandler)

        friend class DownloadManager;

        DownloadHandler(QNetworkReply *reply, DownloadManager *manager, const DownloadRequest &downloadRequest);

    public:
        ~DownloadHandler() override;

        QString url() const;

    signals:
        void downloadFinished(const QString &url, const QByteArray &data);
        void downloadFinished(const QString &url, const QString &filePath);
        void downloadFailed(const QString &url, const QString &reason);
        void redirectedToMagnet(const QString &url, const QString &magnetUri);

    private slots:
        void processFinishedDownload();
        void checkDownloadSize(qint64 bytesReceived, qint64 bytesTotal);

    private:
        void assignNetworkReply(QNetworkReply *reply);
        void handleRedirection(QUrl newUrl);

        static QString errorCodeToString(QNetworkReply::NetworkError status);

        QNetworkReply *m_reply;
        DownloadManager *m_manager;
        const DownloadRequest m_downloadRequest;
    };
}

#endif // NET_DOWNLOADHANDLER_H
