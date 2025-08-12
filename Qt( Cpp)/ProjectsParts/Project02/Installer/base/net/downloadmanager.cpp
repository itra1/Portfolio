/*
 * Bittorrent Client using Qt and libtorrent.
 */

#include "downloadmanager.h"

#include <algorithm>

#include <QDateTime>
#include <QDebug>
#include <QNetworkCookie>
#include <QNetworkCookieJar>
#include <QNetworkProxy>
#include <QNetworkReply>
#include <QNetworkRequest>
#include <QSslError>
#include <QUrl>

#include "base/global.h"
#include "base/preferences.h"
#include "downloadhandler.h"

// Spoof Firefox 38 user agent to avoid web server banning
const char DEFAULT_USER_AGENT[] = "Mozilla/5.0 (X11; Linux i686; rv:38.0) Gecko/20100101 Firefox/38.0";

namespace
{
    class NetworkCookieJar : public QNetworkCookieJar
    {
    public:

        using QNetworkCookieJar::allCookies;
        using QNetworkCookieJar::setAllCookies;

    };

    QNetworkRequest createNetworkRequest(const Net::DownloadRequest &downloadRequest)
    {
        QNetworkRequest request {downloadRequest.url()};

        if (downloadRequest.userAgent().isEmpty())
            request.setRawHeader("User-Agent", DEFAULT_USER_AGENT);
        else
            request.setRawHeader("User-Agent", downloadRequest.userAgent().toUtf8());

        // Spoof HTTP Referer to allow adding torrent link from Torcache/KickAssTorrents
        request.setRawHeader("Referer", request.url().toEncoded().data());
        // Accept gzip
        request.setRawHeader("Accept-Encoding", "gzip");

        return request;
    }
}

Net::DownloadManager *Net::DownloadManager::m_instance = nullptr;

Net::DownloadManager::DownloadManager(QObject *parent)
    : QObject(parent)
{
#ifndef QT_NO_OPENSSL
    connect(&m_networkManager, &QNetworkAccessManager::sslErrors, this, &Net::DownloadManager::ignoreSslErrors);
#endif
    connect(&m_networkManager, &QNetworkAccessManager::finished, this, &DownloadManager::handleReplyFinished);
}

void Net::DownloadManager::initInstance()
{
    if (!m_instance)
        m_instance = new DownloadManager;
}

void Net::DownloadManager::freeInstance()
{
    if (m_instance) {
        delete m_instance;
        m_instance = nullptr;
    }
}

Net::DownloadManager *Net::DownloadManager::instance()
{
    return m_instance;
}

Net::DownloadHandler *Net::DownloadManager::download(const DownloadRequest &downloadRequest)
{
    // Process download request
    const QNetworkRequest request = createNetworkRequest(downloadRequest);
    const ServiceID id = ServiceID::fromURL(request.url());
    const bool isSequentialService = m_sequentialServices.contains(id);
    if (!isSequentialService || !m_busyServices.contains(id)) {
        qDebug("Downloading %s...", qUtf8Printable(downloadRequest.url()));
        if (isSequentialService)
            m_busyServices.insert(id);
        return new DownloadHandler {
            m_networkManager.get(request), this, downloadRequest};
    }

    auto *downloadHandler = new DownloadHandler {nullptr, this, downloadRequest};
    connect(downloadHandler, &DownloadHandler::destroyed, this, [this, id, downloadHandler]()
    {
        m_waitingJobs[id].removeOne(downloadHandler);
    });
    m_waitingJobs[id].enqueue(downloadHandler);
    return downloadHandler;
}

void Net::DownloadManager::registerSequentialService(const Net::ServiceID &serviceID)
{
    m_sequentialServices.insert(serviceID);
}

QList<QNetworkCookie> Net::DownloadManager::cookiesForUrl(const QUrl &url) const
{
    return m_networkManager.cookieJar()->cookiesForUrl(url);
}

bool Net::DownloadManager::setCookiesFromUrl(const QList<QNetworkCookie> &cookieList, const QUrl &url)
{
    return m_networkManager.cookieJar()->setCookiesFromUrl(cookieList, url);
}

QList<QNetworkCookie> Net::DownloadManager::allCookies() const
{
    return static_cast<NetworkCookieJar *>(m_networkManager.cookieJar())->allCookies();
}

void Net::DownloadManager::setAllCookies(const QList<QNetworkCookie> &cookieList)
{
    static_cast<NetworkCookieJar *>(m_networkManager.cookieJar())->setAllCookies(cookieList);
}

bool Net::DownloadManager::deleteCookie(const QNetworkCookie &cookie)
{
    return static_cast<NetworkCookieJar *>(m_networkManager.cookieJar())->deleteCookie(cookie);
}

bool Net::DownloadManager::hasSupportedScheme(const QString &url)
{
    const QStringList schemes = instance()->m_networkManager.supportedSchemes();
    return std::any_of(schemes.cbegin(), schemes.cend(), [&url](const QString &scheme)
    {
        return url.startsWith((scheme + QLatin1Char(':')), Qt::CaseInsensitive);
    });
}

void Net::DownloadManager::handleReplyFinished(QNetworkReply *reply)
{
    const ServiceID id = ServiceID::fromURL(reply->url());
    auto waitingJobsIter = m_waitingJobs.find(id);
    if ((waitingJobsIter == m_waitingJobs.end()) || waitingJobsIter.value().isEmpty()) {
        m_busyServices.remove(id);
        return;
    }

    DownloadHandler *handler = waitingJobsIter.value().dequeue();
    qDebug("Downloading %s...", qUtf8Printable(handler->m_downloadRequest.url()));
    handler->assignNetworkReply(m_networkManager.get(createNetworkRequest(handler->m_downloadRequest)));
    handler->disconnect(this);
}

#ifndef QT_NO_OPENSSL
void Net::DownloadManager::ignoreSslErrors(QNetworkReply *reply, const QList<QSslError> &errors)
{
    Q_UNUSED(errors)
    // Ignore all SSL errors
    reply->ignoreSslErrors();
}
#endif

Net::DownloadRequest::DownloadRequest(const QString &url)
    : m_url {url}
{
}

QString Net::DownloadRequest::url() const
{
    return m_url;
}

Net::DownloadRequest &Net::DownloadRequest::url(const QString &value)
{
    m_url = value;
    return *this;
}

QString Net::DownloadRequest::userAgent() const
{
    return m_userAgent;
}

Net::DownloadRequest &Net::DownloadRequest::userAgent(const QString &value)
{
    m_userAgent = value;
    return *this;
}

qint64 Net::DownloadRequest::limit() const
{
    return m_limit;
}

Net::DownloadRequest &Net::DownloadRequest::limit(qint64 value)
{
    m_limit = value;
    return *this;
}

bool Net::DownloadRequest::saveToFile() const
{
    return m_saveToFile;
}

Net::DownloadRequest &Net::DownloadRequest::saveToFile(bool value)
{
    m_saveToFile = value;
    return *this;
}

bool Net::DownloadRequest::handleRedirectToMagnet() const
{
    return m_handleRedirectToMagnet;
}

Net::DownloadRequest &Net::DownloadRequest::handleRedirectToMagnet(bool value)
{
    m_handleRedirectToMagnet = value;
    return *this;
}

Net::ServiceID Net::ServiceID::fromURL(const QUrl &url)
{
    return {url.host(), url.port(80)};
}

uint Net::qHash(const ServiceID &serviceID, uint seed)
{
    return ::qHash(serviceID.hostName, seed) ^ serviceID.port;
}

bool Net::operator==(const ServiceID &lhs, const ServiceID &rhs)
{
    return ((lhs.hostName == rhs.hostName) && (lhs.port == rhs.port));
}
