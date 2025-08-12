#include "network.h"

Network::Network(QObject *parent) : QObject(parent)
{

}
void Network::Request(QString url, QString token, RequestType requestType, QJsonDocument form, const std::function<void(QNetworkReply*)>& callback)
{

    QNetworkAccessManager *manager = new QNetworkAccessManager();
    QNetworkRequest req(QUrl(url, QUrl::ParsingMode::TolerantMode));

    qDebug() << "Request " << url;

    req.setHeader( QNetworkRequest::ContentTypeHeader, "application/json" );

    QNetworkReply* reply;

    if(!token.isEmpty()){
        req.setRawHeader( "Authorization", (new QString("Bearer app_id_" + token))->toUtf8());
      }

    if(requestType == RequestType::Get){
        reply = manager->get(req);
    }
    if(requestType == RequestType::Post){
        QString strJson(form.toJson(QJsonDocument::Compact));
        qDebug() << strJson;
        reply = manager->post(req, strJson.toUtf8());
        //manager->post(req,form.toString(QUrl::FullyEncoded).toUtf8());
    }

    connect(manager, &QNetworkAccessManager::finished, [=](QNetworkReply *result){
        //qDebug() << result->error();
        //qDebug() << result->errorString();
        //qDebug() << result->readAll();
      callback(result);
      });
}

void Network::Request(QString url, QString token, RequestType requestType, QJsonDocument form, QObject *targetCallback, const std::function<void (QNetworkReply *)> &callback, void (download)(qint64,qint64))
{

    QNetworkAccessManager *manager = new QNetworkAccessManager();
    QNetworkRequest req(QUrl(url, QUrl::ParsingMode::TolerantMode));

    qDebug() << "Request: " << url;

    req.setHeader( QNetworkRequest::ContentTypeHeader, "application/json" );

    QNetworkReply* reply;

    if(!token.isEmpty()){
        req.setRawHeader( "Authorization", (new QString("Bearer app_id_" + token))->toUtf8());
      }

    if(requestType == RequestType::Get){
        reply = manager->get(req);
    }
    if(requestType == RequestType::Post){
        QString strJson(form.toJson(QJsonDocument::Compact));
        qDebug() << strJson;
        reply = manager->post(req, strJson.toUtf8());
        //manager->post(req,form.toString(QUrl::FullyEncoded).toUtf8());
    }

    connect(reply, &QNetworkReply::downloadProgress,targetCallback,download);
    //connect(manager, &QNetworkAccessManager::finished,  targetCallback, result);
    connect(manager, &QNetworkAccessManager::finished, [=](QNetworkReply *result){
      callback(result);
      });
}

void Network::Request(QString url, QString token, RequestType requestType, QJsonDocument form, const std::function<void (QNetworkReply *)> &callback, const std::function<void(qint64, qint64)>& onProgress)
{

  QNetworkAccessManager *manager = new QNetworkAccessManager();
  QNetworkRequest req(QUrl(url, QUrl::ParsingMode::TolerantMode));

  qDebug() << "Request: " << url;

  req.setHeader( QNetworkRequest::ContentTypeHeader, "application/json" );

  QNetworkReply* reply;

  if(!token.isEmpty()){
      req.setRawHeader( "Authorization", (new QString("Bearer app_id_" + token))->toUtf8());
    }

  if(requestType == RequestType::Get){
      reply = manager->get(req);
  }
  if(requestType == RequestType::Post){
      QString strJson(form.toJson(QJsonDocument::Compact));
      qDebug() << strJson;
      reply = manager->post(req, strJson.toUtf8());
      //manager->post(req,form.toString(QUrl::FullyEncoded).toUtf8());
  }

  connect(reply, &QNetworkReply::downloadProgress,[=](qint64 p1, qint64 p2){
      onProgress(p1, p2);
      });
  connect(reply, &QNetworkReply::errorOccurred,[=](QNetworkReply::NetworkError result){
      callback(reply);
      });
  //connect(manager, &QNetworkAccessManager::finished,  targetCallback, result);
  connect(manager, &QNetworkAccessManager::finished, [=](QNetworkReply *result){
    callback(result);
    });
}

void Network::Request(QString url, QString token, RequestType requestType, QUrlQuery form, const std::function<void (QNetworkReply *)> &callback)
{

    QNetworkAccessManager *manager = new QNetworkAccessManager();
    QNetworkRequest req(QUrl(url, QUrl::ParsingMode::TolerantMode));

    qDebug() << "Request: " << url;

    //req.setHeader( QNetworkRequest::ContentTypeHeader, "application/json" );

    if(!token.isEmpty()){
        req.setRawHeader( "Authorization", (new QString("Bearer app_id_" + token))->toUtf8());
  }

    if(requestType == RequestType::Get){
        manager->get(req);
    }
    if(requestType == RequestType::Post){
        manager->post(req,form.toString(QUrl::FullyEncoded).toUtf8());
    }

    //connect(manager, &QNetworkAccessManager::finished, result);
    connect(manager, &QNetworkAccessManager::finished, [=](QNetworkReply *result){
      callback(result);
      });
}
