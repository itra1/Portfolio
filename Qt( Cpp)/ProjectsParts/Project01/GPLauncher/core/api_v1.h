#ifndef API_V1_H
#define API_V1_H

#include <QString>
#include <iostream>
#include <functional>
#include <QJsonDocument>
#include <QNetworkReply>


class Api_v1
{
public:

  static QString getUrl(QString url);
  static QString appId();


  static void launcherReleasesGet(const std::function<void(QJsonDocument)>& onComplete, const std::function<void(QString)>& onError);
  static void gameReleasesGet(const std::function<void(QJsonDocument)>& onComplete, const std::function<void(QString)>& onError);
  static void downloadRelease(QString url,const std::function<void(double)>& onProgress, const std::function<void(QNetworkReply*)>& onComplete, const std::function<void(QString)>& onError);


};

#endif // API_V1_H
