/*
 * Bittorrent Client using Qt and libtorrent.
 */

#include "server.h"

#include <algorithm>

#include <QMutableListIterator>
#include <QNetworkProxy>
#include <QSslCipher>
#include <QSslSocket>
#include <QStringList>
#include <QTimer>

#include "base/utils/net.h"
#include "connection.h"

namespace
{
    const int KEEP_ALIVE_DURATION = 7 * 1000;  // milliseconds
    const int CONNECTIONS_LIMIT = 500;
    const int CONNECTIONS_SCAN_INTERVAL = 2;  // seconds

    QList<QSslCipher> safeCipherList()
    {
        const QStringList badCiphers {"idea", "rc4"};
        const QList<QSslCipher> allCiphers {QSslSocket::supportedCiphers()};
        QList<QSslCipher> safeCiphers;
        std::copy_if(allCiphers.cbegin(), allCiphers.cend(), std::back_inserter(safeCiphers), [&badCiphers](const QSslCipher &cipher)
        {
            return std::none_of(badCiphers.cbegin(), badCiphers.cend(), [&cipher](const QString &badCipher)
            {
                return cipher.name().contains(badCipher, Qt::CaseInsensitive);
            });
        });
        return safeCiphers;
    }
}

using namespace Http;

Server::Server(IRequestHandler *requestHandler, QObject *parent)
    : QTcpServer(parent)
    , m_requestHandler(requestHandler)
    , m_https(false)
{
    setProxy(QNetworkProxy::NoProxy);
    QSslSocket::setDefaultCiphers(safeCipherList());

    QTimer *dropConnectionTimer = new QTimer(this);
    connect(dropConnectionTimer, &QTimer::timeout, this, &Server::dropTimedOutConnection);
    dropConnectionTimer->start(CONNECTIONS_SCAN_INTERVAL * 1000);
}

void Server::incomingConnection(qintptr socketDescriptor)
{
    if (m_connections.size() >= CONNECTIONS_LIMIT) return;

    QTcpSocket *serverSocket;
    if (m_https)
        serverSocket = new QSslSocket(this);
    else
        serverSocket = new QTcpSocket(this);

    if (!serverSocket->setSocketDescriptor(socketDescriptor)) {
        delete serverSocket;
        return;
    }

    if (m_https) {
        static_cast<QSslSocket *>(serverSocket)->setProtocol(QSsl::SecureProtocols);
        static_cast<QSslSocket *>(serverSocket)->setPrivateKey(m_key);
        static_cast<QSslSocket *>(serverSocket)->setLocalCertificateChain(m_certificates);
        static_cast<QSslSocket *>(serverSocket)->setPeerVerifyMode(QSslSocket::VerifyNone);
        static_cast<QSslSocket *>(serverSocket)->startServerEncryption();
    }

    Connection *c = new Connection(serverSocket, m_requestHandler, this);
    m_connections.append(c);
}

void Server::dropTimedOutConnection()
{
    QMutableListIterator<Connection *> i(m_connections);
    while (i.hasNext()) {
        auto connection = i.next();
        if (connection->isClosed() || connection->hasExpired(KEEP_ALIVE_DURATION)) {
            delete connection;
            i.remove();
        }
    }
}

bool Server::setupHttps(const QByteArray &certificates, const QByteArray &privateKey)
{
    const QList<QSslCertificate> certs {Utils::Net::loadSSLCertificate(certificates)};
    const QSslKey key {Utils::Net::loadSSLKey(privateKey)};

    if (certs.isEmpty() || key.isNull()) {
        disableHttps();
        return false;
    }

    m_key = key;
    m_certificates = certs;
    m_https = true;
    return true;
}

void Server::disableHttps()
{
    m_https = false;
    m_certificates.clear();
    m_key.clear();
}
