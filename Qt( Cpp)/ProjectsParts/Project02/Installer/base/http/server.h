/*
 * Bittorrent Client using Qt and libtorrent.
 */


#ifndef HTTP_SERVER_H
#define HTTP_SERVER_H

#include <QSslCertificate>
#include <QSslKey>
#include <QTcpServer>

namespace Http
{
    class IRequestHandler;
    class Connection;

    class Server : public QTcpServer
    {
        Q_OBJECT
        Q_DISABLE_COPY(Server)

    public:
        explicit Server(IRequestHandler *requestHandler, QObject *parent = nullptr);

        bool setupHttps(const QByteArray &certificates, const QByteArray &privateKey);
        void disableHttps();

    private slots:
        void dropTimedOutConnection();

    private:
        void incomingConnection(qintptr socketDescriptor);

        IRequestHandler *m_requestHandler;
        QList<Connection *> m_connections;  // for tracking persistent connections

        bool m_https;
        QList<QSslCertificate> m_certificates;
        QSslKey m_key;
    };
}

#endif // HTTP_SERVER_H
