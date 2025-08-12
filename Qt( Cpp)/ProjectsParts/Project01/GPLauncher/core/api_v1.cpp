#include "api_v1.h"
#include "config/config.h"
#include "network.h"
#include "servers.h"

QString Api_v1::getUrl(QString url) {
  return QString(Servers::instance()->serverApi() + url);
}

QString Api_v1::appId() { return "5535fg34fd"; }

void Api_v1::launcherReleasesGet(
    const std::function<void(QJsonDocument)> &onComplete,
    const std::function<void(QString)> &onError) {

#if TARGET_OS_MAC
  QString url = Config::launcerListUrlMac;
#else
  QString url = Config::launcerListUrlWindows;
#endif

  Network::Request(getUrl(url), appId(), RequestType::Get, QJsonDocument(),
                   [=](QNetworkReply *reply) {
                     if (reply->error()) {
                       qDebug() << "Error request launcher releases "
                                << reply->error();
                       onError(reply->readAll());
                       return;
                     }
                     QString resultStr = reply->readAll();

                     qDebug() << resultStr;

                     QJsonDocument document =
                         QJsonDocument::fromJson(resultStr.toUtf8());
                     bool err = document.object().value("error").toBool();

                     if (err) {
                       onError("Ошибка получения обновления");
                       return;
                     }

                     onComplete(document);
                   });
}

void Api_v1::gameReleasesGet(
    const std::function<void(QJsonDocument)> &onComplete,
    const std::function<void(QString)> &onError) {

#if TARGET_OS_MAC
  QString url = Config::gameListUrlMac;
#else
  QString url = Config::gameListUrlWindows;
#endif

  Network::Request(getUrl(url), appId(), RequestType::Get, QJsonDocument(),
                   [=](QNetworkReply *reply) {
                     if (reply->error()) {
                       qDebug()
                           << "Error request game releases " << reply->error();
                       onError(reply->readAll());
                       return;
                     }
                     QString resultStr = reply->readAll();

                     QJsonDocument document =
                         QJsonDocument::fromJson(resultStr.toUtf8());
                     bool err = document.object().value("error").toBool();

                     if (err) {
                       onError("Ошибка получения обновления");
                       return;
                     }
                     onComplete(document);
                   });
}

void Api_v1::downloadRelease(
    QString url, const std::function<void(double)> &onProgress,
    const std::function<void(QNetworkReply *)> &onComplete,
    const std::function<void(QString)> &onError) {

  qDebug() << "Start download process " + url;

  Network::Request(
      url, appId(), RequestType::Get, QJsonDocument(),
      [=](QNetworkReply *reply) {
        if (reply->error()) {
          qDebug() << "Error " << reply->error();
          onError(reply->readAll());
          return;
        }
        qDebug() << "DownloadComplene " << reply->error();

        onComplete(reply);
      },
      [=](qint64 p1, qint64 p2) { onProgress((double)p1 / (double)p2); });
}
