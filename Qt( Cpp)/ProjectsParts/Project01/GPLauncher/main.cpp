#include <QGuiApplication>
#include <QQmlApplicationEngine>
#include <QLocale>
#include <QProcess>
#include <QTranslator>
#include <QByteArray>
#include <QScopedPointer>
#include "helpers/iohelpers.h"
#include "core/application.h"
#include "config/config.h"
#include "core/storage.h"
#include "core/servers.h"

QScopedPointer<QFile> _logFile;

void CopyDir(QString source, QString target){

  QDir dir(source);

  QDir tDir(target);
  if(!tDir.exists())
    tDir.mkdir(target);


  //dir.setFilter(QDir::Files | QDir::Hidden | QDir::NoSymLinks | QDir::Dirs);
  QFileInfoList list = dir.entryInfoList();
  for (int i = 0; i < list.size(); ++i) {
      QFileInfo fileInfo = list.at(i);

      if(fileInfo.fileName() == "." || fileInfo.fileName() == "..") continue;

      if(fileInfo.isDir()){
          CopyDir(source + "/" + fileInfo.fileName(),target + "/" + fileInfo.fileName());
          continue;
        }

      if(QFile::exists(target + "/" + fileInfo.fileName()))
        QFile::remove(target + "/" + fileInfo.fileName());

      QFile::copy(source + "/" + fileInfo.fileName(), target + "/" + fileInfo.fileName());
      //qDebug() << qPrintable(QString("%1 %2").arg(fileInfo.size(), 10).arg(fileInfo.fileName()));   //выводим в формате "размер имя"
      //qDebug() << "";  //переводим строку
    }
}

void Update(QQmlApplicationEngine *engine, QString sourcePath){

  //QFile::remove(QDir::currentPath() + "/Launcher.exe");

  auto p = QByteArray::fromBase64(sourcePath.toUtf8());
  sourcePath = p;
  qDebug() << QDir::currentPath();

  QTime dieTime= QTime::currentTime().addSecs(5);
  while (QTime::currentTime() < dieTime)
    QCoreApplication::processEvents(QEventLoop::AllEvents, 100);

#if TARGET_OS_MAC
  if(QFile::exists(IOHelpers::currentPath() + "/GPLauncher.app/Contents/MacOS/GPLauncher"))
    QFile::remove(IOHelpers::currentPath() + "/GPLauncher.app/Contents/MacOS/GPLauncher");
  //QFile::copy(sourcePath + "/GPLauncher.app/Contents/MacOS/GPLauncher", IOHelpers::currentPath() + "/GPLauncher.app/Contents/MacOS/GPLauncher");
  CopyDir(sourcePath + "/GPLauncher.app/Contents",IOHelpers::currentPath() + "/Contents");
#else
  if(QFile::exists(IOHelpers::currentPath() + "/GPLauncher.exe"))
    QFile::remove(IOHelpers::currentPath() + "/GPLauncher.exe");
  if(QFile::exists(IOHelpers::currentPath() + "/Launcher.exe"))
    QFile::remove(IOHelpers::currentPath() + "/Launcher.exe");
  CopyDir(sourcePath,IOHelpers::currentPath());
#endif


#if !TARGET_OS_MAC
  if(QFile::exists(IOHelpers::currentPath() + "/Launcher.exe"))
    QFile::rename(IOHelpers::currentPath() + "/Launcher.exe", IOHelpers::currentPath() + "/GPLauncher.exe");

#endif
  //  QFile::copy(QDir::currentPath() + "/LauncherTemp.exe", QDir::currentPath() + "/Launcher.exe");

  QProcess *pr = new QProcess();
  QStringList ott = {};

#if TARGET_OS_MAC
  pr->startDetached(IOHelpers::currentPath() + "/Contents/MacOS/GPLauncher",ott);
#else
  pr->startDetached(IOHelpers::currentPath() + "/GPLauncher.exe",ott);
#endif

  //engine->quit();
}

void ClearTempData(){
#if TARGET_OS_MAC
  if(QFile::exists(IOHelpers::currentPath() + "/Contents/MacOS/GPLauncherTemp"))
    QFile::remove(IOHelpers::currentPath() + "/Contents/MacOS/GPLauncherTemp");
#else
  if(QFile::exists(IOHelpers::currentPath() + "/GPLauncherTemp.exe"))
    QFile::remove(IOHelpers::currentPath() + "/GPLauncherTemp.exe");
#endif

  QDir dir(Config::unpackPath());

  if(dir.exists(Config::unpackPath()))
    dir.removeRecursively();
}

void messageHandle(QtMsgType type, const QMessageLogContext &context, const QString &msg)
{
  // Открываем поток записи в файл
  QTextStream out(_logFile.data());
  // Записываем дату записи
  out << QDateTime::currentDateTime().toString("yyyy-MM-dd hh:mm:ss.zzz ");
  // По типу определяем, к какому уровню относится сообщение
  switch (type)
  {
  case QtInfoMsg:     out << "INF "; break;
  case QtDebugMsg:    out << "DBG "; break;
  case QtWarningMsg:  out << "WRN "; break;
  case QtCriticalMsg: out << "CRT "; break;
  case QtFatalMsg:    out << "FTL "; break;
  }
  // Записываем в вывод категорию сообщения и само сообщение

  QString logText = QString("%1 : %2").arg(context.category).arg(msg);
  Application::instance()->setLogText(logText);

  out << context.category << ": " << msg << Qt::endl;
  out.flush();    // Очищаем буферизированные данные

}

int main(int argc, char *argv[])
{
#if QT_VERSION < QT_VERSION_CHECK(6, 0, 0)
  QCoreApplication::setAttribute(Qt::AA_EnableHighDpiScaling);
#endif

  QGuiApplication app(argc, argv);

  QTranslator translator;
  const QStringList uiLanguages = QLocale::system().uiLanguages();
  for (const QString &locale : uiLanguages) {
      const QString baseName = "Launcher_" + QLocale(locale).name();
      if (translator.load(":/i18n/" + baseName)) {
          app.installTranslator(&translator);
          break;
        }
    }

  QQmlApplicationEngine *engine = new QQmlApplicationEngine();

  Storage::initInstance();
  Servers::initInstance();

  for(int i = 0 ; i < argc ; i++){

      QString key = argv[i];

      if(key == "stag"){
          QString val = argv[i + 1];
          Storage::instance()->storeValue("stag",val);
          Storage::instance()->save();
          qDebug() << "stag " << val;
          engine->quit();
          return -1;
          //return;
        }
    }

  for(int i = 0 ; i < argc ; i++){

      QString key = argv[i];

      if(key == "-update"){
          Update(engine, argv[++i]);
          return 0;
        }

    }
  ClearTempData();

  Application::initInstance(argc, argv, engine,nullptr);

  //if(Application::IsDevelop){
  _logFile.reset(new QFile(IOHelpers::currentPath() + "/logFile.txt"));
  _logFile.data()->open(QFile::Append | QFile::Text);
  qInstallMessageHandler(messageHandle);
//}
  //QFile::setPermissions(IOHelpers::currentPath(), QFile::Permission::WriteOther);
qDebug() << "Current path " << IOHelpers::currentPath();
qDebug() << QFile::permissions(IOHelpers::currentPath());
qDebug() << QFile::permissions(IOHelpers::currentPath() + "/Contents");

  Application::instance()->appStart();
  Application::instance()->initinitForm(engine);

  return app.exec();
}
