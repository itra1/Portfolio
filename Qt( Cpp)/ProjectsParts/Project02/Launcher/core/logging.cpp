#include "logging.h"
#include <QDebug>
#include <QDir>
#include <QDate>
#include <QDateTime>

using namespace Core;

Logging::Logging(QObject *parent) : QObject(parent)
{

}

void Logging::logHandler(QtMsgType type, const QMessageLogContext &context, const QString &msg)
{
    QByteArray localMsg = msg.toLocal8Bit();

    QFile fMessFile(QDir::currentPath() + "/log/" + QDate::currentDate().toString("dd-MM-yyyy") + ".txt");
    if(!fMessFile.open(QIODevice::Append | QIODevice::Text))
        return;

    QString sCurrDateTime = "[" +
              QDateTime::currentDateTime().toString("dd.MM.yyyy hh:mm:ss.zzz") + "]";

    const char *file = context.file ? context.file : "";
    const char *function = context.function ? context.function : "";
    QTextStream tsTextStream(&fMessFile);

    switch(type){
    case QtDebugMsg:
        tsTextStream << QString("%1 Debug - %2 \n %3 %4 %5\n").arg(sCurrDateTime).arg(localMsg.constData()).arg(function).arg(file).arg(context.line);
        break;
    case QtInfoMsg:
        tsTextStream << QString("%1 Info - %2 \n %3 %4 %5\n").arg(sCurrDateTime).arg(localMsg.constData()).arg(function).arg(file).arg(context.line);
        break;
    case QtWarningMsg:
        tsTextStream << QString("%1 Warning - %2 \n %3 %4 %5\n").arg(sCurrDateTime).arg(localMsg.constData()).arg(function).arg(file).arg(context.line);
        break;
    case QtCriticalMsg:
        tsTextStream << QString("%1 Critical - %2 \n %3 %4 %5\n").arg(sCurrDateTime).arg(localMsg.constData()).arg(function).arg(file).arg(context.line);
        break;
    case QtFatalMsg:
        tsTextStream << QString("%1 Fatal - %2 \n %3 %4 %5\n").arg(sCurrDateTime).arg(localMsg.constData()).arg(function).arg(file).arg(context.line);
        abort();
    }

    tsTextStream.flush();
    fMessFile.flush();
    fMessFile.close();
}
