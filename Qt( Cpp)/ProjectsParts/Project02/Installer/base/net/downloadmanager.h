/*
 * Bittorrent Client using Qt and libtorrent.
 */

#ifndef NET_DOWNLOADMANAGER_H
#define NET_DOWNLOADMANAGER_H

#include <QHash>
#include <QNetworkAccessManager>
#include <QNetworkRequest>
#include <QObject>
#include <QQueue>
#include <QSet>

class QNetworkReply;
class QNetworkCookie;
class QSslError;
class QUrl;

namespace Net
{
    class DownloadHandler;

    class DownloadRequest
    {
    public:
        DownloadRequest(const QString &url);
        DownloadRequest(const DownloadRequest &other) = default;

        QString url() const;
        DownloadRequest &url(const QString &value);

        QString userAgent() const;
        DownloadRequest &userAgent(const QString &value);

        qint64 limit() const;
        DownloadRequest &limit(qint64 value);

        bool saveToFile() const;
        DownloadRequest &saveToFile(bool value);

        bool handleRedirectToMagnet() const;
        DownloadRequest &handleRedirectToMagnet(bool value);

    private:
        QString m_url;
        QString m_userAgent;
        qint64 m_limit = 0;
        bool m_saveToFile = false;
        bool m_handleRedirectToMagnet = false;
    };

    struct ServiceID
    {
        QString hostName;
        int port;

        static ServiceID fromURL(const QUrl &url);
    };

    class DownloadManager : public QObject
    {
        Q_OBJECT
        Q_DISABLE_COPY(DownloadManager)

    public:
        static void initInstance();
        static void freeInstance();
        static DownloadManager *instance();

        DownloadHandler *download(const DownloadRequest &downloadRequest);

        void registerSequentialService(const ServiceID &serviceID);

        QList<QNetworkCookie> cookiesForUrl(const QUrl &url) const;
        bool setCookiesFromUrl(const QList<QNetworkCookie> &cookieList, const QUrl &url);
        QList<QNetworkCookie> allCookies() const;
        void setAllCookies(const QList<QNetworkCookie> &cookieList);
        bool deleteCookie(const QNetworkCookie &cookie);

        static bool hasSupportedScheme(const QString &url);

    private slots:
    #ifndef QT_NO_OPENSSL
        void ignoreSslErrors(QNetworkReply *, const QList<QSslError> &);
    #endif

    private:
        explicit DownloadManager(QObject *parent = nullptr);

        void handleReplyFinished(QNetworkReply *reply);

        static DownloadManager *m_instance;
        QNetworkAccessManager m_networkManager;

        QSet<ServiceID> m_sequentialServices;
        QSet<ServiceID> m_busyServices;
        QHash<ServiceID, QQueue<DownloadHandler *>> m_waitingJobs;
    };

    uint qHash(const ServiceID &serviceID, uint seed);
    bool operator==(const ServiceID &lhs, const ServiceID &rhs);
}

#endif // NET_DOWNLOADMANAGER_H
