#include <QGuiApplication>
#include <QQmlApplicationEngine>
#include <string>
#include <QDebug>
#include <QDir>

#include "core/application.h"

int main(int argc, char *argv[])
{
    if (argc == 1) {
        return 1;
    }

    QString from = argv[1];
    QString to = argv[2];
    QString afterRun = argv[3];

//    QFile torrentFile("e:/source.log");
//    torrentFile.open(QIODevice::WriteOnly | QIODevice::Append);
//    QTextStream stream(&torrentFile);
//    stream<<(from + "\n");
//    stream<<(to + "\n");
//    stream<<(afterRun + "\n");
//    torrentFile.close();

//    QString from = "";
//    QString to = "";
//    QString afterRun = "";

    qDebug() << "Start";
    qDebug() << "Arg count " + argc;

    argv += 1;
    argc -= 1;

//    QString from = "";
//    QString to = "";
//    QString afterRun = "";

//    for (; argc > 0; --argc, ++argv) {

//        if (argv[0][0] != '-') {
//            return 1;
//        }

//        QString key = QString(argv[0]).remove(0,1);
//        QString value = QString(argv[1]);

//        if(key == "from"){
//            from = QDir::toNativeSeparators(value);
//        }
//        if(key == "to"){
//            to = QDir::toNativeSeparators(value);
//        }
//        if(key == "afterRun"){
//            afterRun = QDir::toNativeSeparators(value);
//        }

//        ++argv;
//        --argc;
//    }


    QCoreApplication::setAttribute(Qt::AA_EnableHighDpiScaling);

    QGuiApplication app(argc, argv);

    QQmlApplicationEngine engine;

    Updater::Application application(app,engine,from,to, afterRun);

    if (engine.rootObjects().isEmpty())
        return -1;

    return app.exec();
}
