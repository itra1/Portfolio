#ifndef API_NETWORK_H
#define API_NETWORK_H

#include "base/requesttype.h"
#include <QNetworkAccessManager>
#include <QNetworkReply>
#include <QObject>
#include <QtNetwork>

namespace Api {

  class Network {
  public:
    static void Request(QString url, QString token, RequestType requestType,
                        QJsonDocument form,
                        const std::function<void(QNetworkReply*)>& callback);
    static void Request(QString url, QString token, RequestType requestType,
                        QJsonDocument form,
                        const std::function<void(QNetworkReply*)>& callback,
                        const std::function<void(qint64, qint64)>& download);
    static void Request(QString url, QString token, RequestType requestType,
                        QUrlQuery form,
                        const std::function<void(QNetworkReply*)>& callback);
    static void Send(QString url, QString token, RequestType requestType,
                     QJsonDocument form, QObject* targetCallback,
                     void(result)(QNetworkReply*),
                     void(download)(qint64, qint64));
  };

} // namespace Api

#endif // API_NETWORK_H
