#ifndef NETWORK_H
#define NETWORK_H

#include <QObject>
#include <QtNetwork>
#include <QNetworkAccessManager>
#include <QNetworkReply>

enum RequestType {
    Get,
    Post
};

    class Network : public QObject
    {
        Q_OBJECT
    public:
        explicit Network(QObject *parent = nullptr);
        static void Request(QString url, QString token, RequestType requestType, QJsonDocument form, const std::function<void(QNetworkReply*)>& callback);
        static void Request(QString url, QString token, RequestType requestType, QJsonDocument form, QObject *targetCallback, const std::function<void(QNetworkReply*)>& callback, void (download)(qint64, qint64));
        static void Request(QString url, QString token, RequestType requestType, QJsonDocument form, const std::function<void(QNetworkReply*)>& callback, const std::function<void(qint64, qint64)>& onProgress);
        static void Request(QString url, QString token, RequestType requestType, QUrlQuery form, const std::function<void(QNetworkReply*)>& callback);
        static void Send(QString url, QString token, RequestType requestType, QJsonDocument form, QObject *targetCallback, void (result)(QNetworkReply *), void (download)(qint64, qint64));

    signals:

    private:

    };

#endif // NETWORK_H
