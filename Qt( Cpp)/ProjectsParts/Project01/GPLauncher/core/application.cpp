#include "application.h"
#include "../ui/form.h"
#include "network.h"
#include <QException>
#include <QProcess>

#ifdef API_V1
#include "api_v1.h"
#endif

#include "appPhase.h"
#include "config/config.h"
#include "helpers/iohelpers.h"
#include "quazip/JlCompress.h"
#include "quazip/quazip.h"
#include "quazip/quazipfile.h"
#include "rsa/rsastaticlib.h"
#include "servers.h"
#include "storage.h"
#include "updatePhase.h"

Application *Application::_instance = nullptr;
bool Application::IsDevelop = false;
bool Application::IsGameDevelop = false;

Application::Application(int argc, char *argv[], QQmlApplicationEngine *engine,
                         QObject *parent)
    : QObject(parent), m_phase(AppPhase::Connect), m_isConnectLost(false) {
  _argvInputLenght = 0;
  for (int i = 0; i < argc; i++) {

    _argvInputLenght++;
    QString key = argv[i];

    if (key == "-develop")
      IsDevelop = true;
    if (key == "-devGameDevelop")
      IsGameDevelop = true;
  }
  _argvInput = new QString[_argvInputLenght];
  for (int i = 0; i < argc; i++)
    _argvInput[i] = argv[i];

  connect(Servers::instance(), SIGNAL(OnSetServer()), SLOT(serversSet()));
QDir dir;
  tempString = dir.absolutePath();

}

void Application::initInstance(int argc, char *argv[],
                               QQmlApplicationEngine *engine, QObject *parent) {
  if (!_instance)
    _instance = new Application(argc, argv, engine, parent);
}

void Application::freeInstance() {
  if (_instance) {
    delete _instance;
    _instance = nullptr;
  }
}

Application *Application::instance() { return _instance; }

void Application::appStart()
{
  readPromo();

  if (Servers::instance()->serversLoaded())
    serversSet();

  qDebug() << tempString;
}

void Application::initinitForm(QQmlApplicationEngine *engine) {
  Form::initInstance(engine, nullptr);
  this->engine = engine;
}

QString Application::getUrl(QString url) {
  return QString(Servers::instance()->serverApi() + url);
}

void Application::parseReleasesJson(QJsonDocument document) {
  QJsonArray jArr = document.object().value("items").toArray();

  if (m_updatePhase == UpdatePhase::Launcher)
    checkLauchers(jArr);
  else if (m_updatePhase == UpdatePhase::Game)
    checkGames(jArr);
}

void Application::checkLauchers(QJsonArray jArr) {

  m_allLaunchers = new QList<Record *>();

  for (int i = 0; i < jArr.count(); i++) {
    Record *itm = Record::Parce(jArr[i].toObject());
    m_allLaunchers->append(itm);
  }

  qDebug() << "Records count Launcher" << m_allLaunchers->count();

  if (m_allLaunchers->count() > 0) {

    QString version = APP_VERSION;
    Record *rec = nullptr;

    for (int i = 0; i < m_allLaunchers->count(); i++) {

      // if(IsDevelop && m_allLaunchers->value(i)->appTypeTitle != "DEV")
      // continue;
      if (!IsDevelop && m_allLaunchers->value(i)->appTypeTitle == "DEV")
        continue;

      if (maxCheck(m_allLaunchers->value(i)->appVersion, version)) {
        version = m_allLaunchers->value(i)->appVersion;
        rec = m_allLaunchers->value(i);
      }
    }

    if (version != APP_VERSION && rec != nullptr) {
      download(rec);
      setPhase(AppPhase::Download);
    } else {
      getGameReleases();
    }
  } else
    getGameReleases();
}

void Application::checkGames(QJsonArray jArr) {
  m_allGames = new QList<Record *>();

  for (int i = 0; i < jArr.count(); i++) {
    Record *itm = Record::Parce(jArr[i].toObject());
    m_allGames->append(itm);
    qDebug() << "Games" << itm->appVersion;
  }

  qDebug() << "Records count Games" << m_allGames->count();

  // Ищем установленную игру
  QFile *installRelease = new QFile(Config::gameInfoPath());

  QString existsVersion = "0.0.0.0";
  bool existsGame = false;
  if (installRelease->exists()) {
    QByteArray data;
    if (!installRelease->open(QFile::ReadOnly))
      return;
    data =
        installRelease
            ->readAll(); // ... и записываем всю информацию со страницы в файл
    installRelease->close();

    QJsonDocument document = QJsonDocument::fromJson(data);
    QJsonObject jObj = document.object();
    Record *exists = Record::Parce(jObj);
    existsVersion = exists->appVersion;
    existsGame = true;
  }

  if (m_allGames->count() > 0) {

    QString version = existsVersion;
    Record *rec = nullptr;

    for (int i = 0; i < m_allGames->count(); i++) {

      // if(IsDevelop && m_allGames->value(i)->appTypeTitle != "DEV") continue;
      if (!IsDevelop && m_allGames->value(i)->appTypeTitle == "DEV")
        continue;

      if (maxCheck(m_allGames->value(i)->appVersion, version)) {
        version = m_allGames->value(i)->appVersion;
        rec = m_allGames->value(i);
      }
    }

    if (version != APP_VERSION && rec != nullptr) {
      download(rec);
      setPhase(AppPhase::Download);
    } else {
      if (existsGame)
        playGame();
    }
  } else {
    if (existsGame)
      playGame();
  }
}

AppPhase::State Application::getPhase() const { return m_phase; }

void Application::getLaunchersReleases() {
  m_updatePhase = UpdatePhase::Launcher;
  setPhase(AppPhase::Connect);

  Api_v1::launcherReleasesGet(
      [&](QJsonDocument result) {
        parseReleasesJson(result);
      },
      [&](QString error) {
        setIsConnectLost(true);
        qDebug() << "Network error " << error;
      });
}

void Application::getGameReleases() {
  m_updatePhase = UpdatePhase::Game;
  setPhase(AppPhase::Connect);

  Api_v1::gameReleasesGet(
      [&](QJsonDocument result) {
        parseReleasesJson(result);
      },
      [&](QString error) {
        setIsConnectLost(true);
        qDebug() << "Network error " << error;
      });
}

void Application::repeatButton() {
  if (m_updatePhase == UpdatePhase::Launcher)
    getLaunchersReleases();
  else if (m_updatePhase == UpdatePhase::Game)
    getGameReleases();
}

void Application::setError(QString error) {
  m_error = error;
  qDebug() << "setError: " << m_error;
  setPhase(AppPhase::Error);
}

QString Application::appId() { return "5535fg34fd"; }

void Application::playGame() {
  QProcess *pr = new QProcess();
  QStringList ott = {};
  ott << "-fromLauncher";
  ott << "-servData" << Servers::instance()->getServersDataBase64();
  ott << "-sourceLauncher"
      << (IOHelpers::currentPath() + "/Launcher.exe").toUtf8().toBase64();

  if (IsDevelop)
    ott << "-devApp";

  if (IsGameDevelop)
    ott << "-gameDevelop";

  if (Storage::instance()->hashValue("stag")) {
    ott << "-stag" << Storage::instance()->loadValue("stag", "").toString();
    Storage::instance()->remove("stag");
    Storage::instance()->save();
  }

  if (Storage::instance()->hashValue("promo")) {
    ott << "-promo"
        << Storage::instance()
               ->loadValue("promo", "")
               .toString()
               .toUtf8()
               .toBase64();
    Storage::instance()->remove("promo");
    Storage::instance()->save();
  }

  try {
    for (int i = 1; i < _argvInputLenght; i++) {
      ott << _argvInput[i];
    }
  } catch (std::exception ex) {
    qDebug() << "p ";
  }
  qDebug() << ott;

#if TARGET_OS_MAC
  pr->startDetached(Config::gamePathMacExe(), ott);
#else
  pr->startDetached(Config::gamePathExe, ott);
#endif
  engine->quit();
}

void Application::download(Record *rec) {
  setPhase(AppPhase::Download);
  QString targetUrl = rec->fileUrl;
  qDebug() << "Start download " << targetUrl;
  setPhase(AppPhase::Download);
  m_downloadRecord = rec;

  Api_v1::downloadRelease(
      targetUrl, [&](double progress) { setDownloadProgress(progress); },
      [&](QNetworkReply *result) {
        downloadFinish(result);
      },
      [&](QString error) {
        setIsConnectLost(true);
        qDebug() << "Network error " << error;
      });

  //  Network::Request(targetUrl,appId(),RequestType::Get,QJsonDocument(),this,[](QNetworkReply
  //  *res){
  //    Application::instance()->downloadFinish(res);
  //  },[](qint64 p1,qint64 p2){
  //    Application::instance()->setDownloadProgress((double)p1 / (double)p2);
  //  });
}

void Application::downloadFinish(QNetworkReply *reply) {
  // qDebug() << "Download build complete " <<  file.fieldname;

  if (reply->error()) {
    // Сообщаем об этом и показываем информацию об ошибках
    // emit emitDownloadError("Ошибка загрузки файла");
    qDebug() << "Ошибка загрузки файла";
    return;
  }

  // Запись архива
  QByteArray byte = reply->readAll();

  if (m_updatePhase == UpdatePhase::Launcher)
    processUpdateLauncher(byte);
  else if (m_updatePhase == UpdatePhase::Game)
    processUpdateGame(byte);
}

void Application::processUpdateLauncher(QByteArray byte) {

  QString pathWrite(Config::downloadPath());

  if (!QDir(pathWrite).exists())
    QDir(pathWrite).mkdir(pathWrite);

  QString zipPath =
      Config::downloadPath() + "/" + m_downloadRecord->fileZipName();
  QString pathExtract =
      QString(Config::unpackPath()) + "/" + m_downloadRecord->extractPath();

  if (!QDir(pathExtract).exists())
    QDir(pathExtract).mkdir(pathExtract);

  QFile *file = new QFile(zipPath);
  // Создаём файл или открываем его на перезапись ...
  if (file->open(QFile::WriteOnly)) {
    file->write(byte); // ... и записываем всю информацию со страницы в файл
    file->close();     // закрываем файл
  }

  qDebug() << "Zip path " << zipPath;
  qDebug() << "Update path " << pathExtract;
  qDebug() << "Zip path " << zipPath;
  // Распаковка
  JlCompress::extractDir(zipPath, pathExtract);

  // Копирование
#if TARGET_OS_MAC
  QString dt = IOHelpers::currentPath() + "/Contents/MacOS/GPLauncherTemp";
  if (QFile::exists(IOHelpers::currentPath() + "/Contents/MacOS/GPLauncherTemp"))
    QFile::remove(IOHelpers::currentPath() + "/Contents/MacOS/GPLauncherTemp");
  if (QFile::exists(IOHelpers::currentPath() + "/Contents/MacOS/GPLauncher"))
    qDebug() << "Exists file";
  QFile::copy(IOHelpers::currentPath() + "/Contents/MacOS/GPLauncher", IOHelpers::currentPath() + "/Contents/MacOS/GPLauncherTemp");

#else
  if (QFile::exists(IOHelpers::currentPath() + "/GPLauncherTemp.exe"))
    QFile::remove(IOHelpers::currentPath() + "/GPLauncherTemp.exe");
  if (QFile::exists(IOHelpers::currentPath() + "/LauncherTemp.exe"))
    QFile::remove(IOHelpers::currentPath() + "/LauncherTemp.exe");

  QString exeFile = IOHelpers::currentPath() + "/GPLauncher.exe";

  if (QFile::exists(IOHelpers::currentPath() + "/GPLauncher.exe"))
    QFile::copy(IOHelpers::currentPath() + "/GPLauncher.exe", IOHelpers::currentPath() + "/GPLauncherTemp.exe");
  if (QFile::exists(IOHelpers::currentPath() + "/Launcher.exe"))
    QFile::copy(IOHelpers::currentPath() + "/Launcher.exe", IOHelpers::currentPath() + "/GPLauncherTemp.exe");
#endif

  // Запуск
  QProcess *pr = new QProcess();
  QStringList ott = {};
  ott << "-update";
  ott << pathExtract.toUtf8().toBase64();
#if TARGET_OS_MAC
  pr->startDetached(IOHelpers::currentPath() +
                        "/Contents/MacOS/GPLauncherTemp",
                    ott);
#else
  pr->startDetached(IOHelpers::currentPath() + "/GPLauncherTemp.exe", ott);
#endif
  engine->quit();
}

void Application::processUpdateGame(QByteArray byte) {

  QString pathWrite(Config::downloadPath());

  if (!QDir(pathWrite).exists())
    QDir(pathWrite).mkdir(pathWrite);

  QString zipPath =
      Config::downloadPath() + "/" + m_downloadRecord->fileZipName();

  QFile *file = new QFile(zipPath);
  // Создаём файл или открываем его на перезапись ...
  if (file->open(QFile::WriteOnly)) {
    file->write(byte); // ... и записываем всю информацию со страницы в файл
    file->close();     // закрываем файл
  }
  // Распаковка
  JlCompress::extractDir(zipPath, Config::gamePath());

  // Запись info файла
  QFile *fileInfo = new QFile(Config::gameInfoPath());

  if (fileInfo->exists())
    fileInfo->remove();

  QJsonDocument doc(m_downloadRecord->jObject);
  // Создаём файл или открываем его на перезапись ...
  if (fileInfo->open(QFile::WriteOnly)) {
    fileInfo->write(
        doc.toJson(QJsonDocument::Compact)); // ... и записываем всю информацию
                                             // со страницы в файл
    fileInfo->close();
  }

  // Запуск
  playGame();
}

void Application::setDownloadProgress(double progress) {
  emit onDownloadProgress(progress);
}

bool Application::maxCheck(QString v1, QString v2) {
  if (v2 == "")
    return true;
  if (v1 == v2)
    return false;
  try {
    QStringList v1l = v1.split(QLatin1Char('.'));
    QStringList v2l = v2.split(QLatin1Char('.'));

    for (int i = 0; i < v1l.length(); i++) {
      if (i >= v2l.count())
        return true;
      if (v1l[i].toInt() == v2l[i].toInt())
        continue;
      return v1l[i].toInt() > v2l[i].toInt();
    }
    return false;

  } catch (std::exception ex) {
    return false;
  }
  return false;
}

void Application::connectLost() { setIsConnectLost(true); }

void Application::quit() { engine->quit(); }

void Application::tryConnectAgaine() {
  setIsConnectLost(false);

  if (m_updatePhase == UpdatePhase::Launcher)
    getLaunchersReleases();

  if (m_updatePhase == UpdatePhase::Game)
    getGameReleases();
}

QString Application::getError() const { return m_error; }

void Application::readPromo() {

  QString promoPath = IOHelpers::currentPath() + "/promo.txt";

  if (!QFile::exists(promoPath))
    return;

  QFile *fileInfo = new QFile(promoPath);

  if (fileInfo->open(QFile::ReadOnly)) {
    _promo = fileInfo->readAll();
    fileInfo->close();
  }
  delete fileInfo;

  QFile::remove(promoPath);

  Storage::instance()->storeValue("promo", _promo);
  Storage::instance()->save();
}

bool Application::getIsConnectLost() const { return m_isConnectLost; }

void Application::setIsConnectLost(bool newIsConnectLost) {
  m_isConnectLost = newIsConnectLost;
  emit connectLostSignal();
}

QString Application::getLogText() const
{
  return _logText;
}

void Application::setLogText(QString addText)
{
  _logText = addText + '\n' + _logText;
  emit onNewLog();
}

QString Application::getAppVersion()
{
  return APP_VERSION;
}

void Application::serversSet() { getLaunchersReleases(); }

void Application::setPhase(AppPhase::State newPhase) {
  m_phase = newPhase;
  emit onChangePhase();
}
