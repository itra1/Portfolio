#ifndef APPLICATION_H
#define APPLICATION_H
#include <QQmlApplicationEngine>
#include <QObject>
#include <QNetworkReply>
#include "record.h"
#include "appPhase.h"
#include "updatePhase.h"
#include "quazip/quazip.h"

class Application : public QObject
{
  Q_OBJECT
public:

  explicit Application(int argc, char *argv[], QQmlApplicationEngine *engine, QObject *parent = nullptr);
  static void initInstance(int argc, char *argv[], QQmlApplicationEngine *engine, QObject *parent);
  static void freeInstance();
  static Application *instance();
  void appStart();
  void initinitForm(QQmlApplicationEngine *engine);

  //static void loadReleases(QNetworkReply *reply);

  static QString getUrl(QString url);
  void parseReleasesJson(QJsonDocument document);
  void checkLauchers(QJsonArray jArr);
  void checkGames(QJsonArray jArr);

  AppPhase::State getPhase() const;
  void getLaunchersReleases();
  void getGameReleases();
  void repeatButton();
  void setError(QString error);
  QString appId();
  void playGame();
  void download(Record *rec);
  void downloadFinish(QNetworkReply *reply);
  void processUpdateLauncher(QByteArray byte);
  void processUpdateGame(QByteArray byte);
  void setDownloadProgress(double progress);
  static bool maxCheck(QString v1, QString v2);  

  void connectLost();
  Q_PROPERTY(bool isConnectLost READ getIsConnectLost NOTIFY connectLostSignal)
  Q_PROPERTY(QString logText READ getLogText NOTIFY onNewLog)
  Q_PROPERTY(QString appVersion READ getAppVersion NOTIFY onNewLog)
  Q_INVOKABLE void quit();
  Q_INVOKABLE void tryConnectAgaine();

  QString getError() const;
  void readPromo();
  bool getIsConnectLost() const;
  void setIsConnectLost(bool newIsConnectLost);
  QString getLogText() const;
  void setLogText(QString addText);
  QString getAppVersion();
  static bool IsDevelop;
  static bool IsGameDevelop;

signals:
  void onChangePhase();
  void onDownloadProgress(double progress);
  void connectLostSignal();
  void onNewLog();

public slots:
  void serversSet();

private:
  static Application *_instance;
  QQmlApplicationEngine* engine;
  QList<Record*> *m_allLaunchers;
  QList<Record*> *m_allGames;
  AppPhase::State m_phase;
  UpdatePhase::State m_updatePhase;
  QString m_error;
  Record *m_downloadRecord;
  QString _promo;
  QString *_argvInput;
  QString _logText;
  int _argvInputLenght;
  // Флаг потери сигнала
  bool m_isConnectLost;
  QString tempString;

  void setPhase(AppPhase::State newPhase);

};

#endif // APPLICATION_H
