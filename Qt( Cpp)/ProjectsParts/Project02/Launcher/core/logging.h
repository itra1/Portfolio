#ifndef LOGGING_H
#define LOGGING_H

#include <QObject>

namespace Core{

    class Logging : public QObject
    {
        Q_OBJECT
    public:
        explicit Logging(QObject *parent = nullptr);

        static void logHandler(QtMsgType type, const QMessageLogContext &context, const QString &msg);

    signals:

    public slots:
    };
}

#endif // LOGGING_H
