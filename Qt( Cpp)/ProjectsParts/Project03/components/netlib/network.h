#ifndef NETWORK_H
#define NETWORK_H

#include <QNetworkReply>
#include <QObject>
#include <QUrlQuery>

namespace Netlib {
enum RequestType { Get, Post, Patch };

class Network : public QObject
{
    Q_OBJECT
public:
    static void Request(QString url,
                        QString token,
                        RequestType requestType,
                        QJsonDocument form,
                        const std::function<void(QNetworkReply *)> &callback);
    static void Request(QString url,
                        QString token,
                        RequestType requestType,
                        QJsonDocument form,
                        const std::function<void(QNetworkReply *)> &callback,
                        const std::function<void(qint64, qint64)> &download);
    static void Request(QString url,
                        QString token,
                        RequestType requestType,
                        QUrlQuery form,
                        const std::function<void(QNetworkReply *)> &callback);
        
    };
    }  // namespace Netlib
#endif // NETWORK_H
