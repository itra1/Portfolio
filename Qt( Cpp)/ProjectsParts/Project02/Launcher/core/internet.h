#ifndef INTERNET_H
#define INTERNET_H

#include <QObject>
#include "config.h"

namespace Core {

    class Internet : public QObject
    {
        Q_OBJECT
    public:
        explicit Internet(QObject *parent = nullptr);

        static void initInstance();
        static void freeInstance();
        static Internet *instance();

        bool exists() const;

    signals:
        void onInternetChange(bool exists);

    public slots:
        void checkInternet();

    private:

        static Internet *m_instance;
        bool m_exists;
        bool m_wait;

        void setExists(bool exists);
        void waitCheckInternet();

    };
}

#endif // INTERNET_H
