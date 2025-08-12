#ifndef STATISTIC_H
#define STATISTIC_H

#include <QObject>

namespace Core{

    class Statistic : public QObject
    {
        Q_OBJECT
    public:
        explicit Statistic(QString deviceInfo, QObject *parent = nullptr);

        static void initInstance(QString deviceInfo);
        static void freeInstance();
        static Statistic *instance();

        void send(QString action);

    signals:

    public slots:

    private:

        QString server = "https://s.pzonline.com/l";

        static Statistic *m_instance;
        QString m_token;
        QString m_hardwareId;

        QString getProperty(QString hardwareClass, QString propertyName);
    };

}

#endif // STATISTIC_H
