#include "logger.h"
#include <QString>
#include <QDateTime>
#include <QDir>
#include <QFile>
#include <QDebug>
#include "../config/config.h"

QScopedPointer<QFile> *Logger::_logFile = nullptr;
Logger::Logger()
{
}

void Logger::init()
{
  removeOldFiles();
  subscribe();
}

void Logger::subscribe()
{

  QString date = QDateTime::currentDateTime().toString("dd.MM.yyyy_hh.mm.ss.zzz");
  qDebug() << date;
  QString logPath = getLogPath();

  QDir tDir(logPath);
  if(!tDir.exists())
    tDir.mkdir(logPath);

  _logFile = new QScopedPointer<QFile>();
  _logFile->reset(new QFile(logPath +  "/"+ date +".txt"));
  _logFile->data()->open(QFile::WriteOnly | QFile::Text);
  qInstallMessageHandler(messageHandler);
}

void Logger::removeOldFiles()
{
  QDir dir(getLogPath());
  if(!dir.exists())
    return;

  QFileInfoList list = dir.entryInfoList();
  for (int i = 0; i < list.size(); ++i) {

      QFileInfo fileInfo = list.at(i);

      if(fileInfo.fileName() == "." || fileInfo.fileName() == "..") continue;

      auto fileName = fileInfo.fileName().split(QString(".t"))[0];
      QDateTime dt = QDateTime::fromString(fileName, "dd.MM.yyyy_hh.mm.ss.zzz");

      if(dt.date().daysTo(QDateTime::currentDateTime().date()) >= 3)
        QFile::remove(getLogPath() + "/" + fileInfo.fileName());

    }
}

QString Logger::getLogPath()
{
  return Config::currentPath() + "/logs";
}

void Logger::messageHandler(QtMsgType type, const QMessageLogContext &context, const QString &msg)
{
  // Открываем поток записи в файл
  QTextStream out(_logFile->data());
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
  out << context.category << ": " << msg << Qt::endl;
  out.flush();    // Очищаем буферизированные данные
}
