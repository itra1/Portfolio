#ifndef USERSESSION_H
#define USERSESSION_H

#include <QObject>

namespace Core {

    class UserSession : public QObject
    {
        Q_OBJECT
        explicit UserSession(QObject *parent = nullptr);

    public:

        struct Version{
            QString stream;
            QString url;
            QString guid;
        };

        static void initInstance();
        static void freeInstance();
        static UserSession *instance();

        void init();

        bool isAuth();
        void setIsAuth(bool isAuth);

        QString userName() const;
        QString token() const;
        void setToken(const QString &token);
        QList<Version>* versions() const;
        QString authUrl();
        QString redirectUrl();
        QString userDataUrl();
        void login();
        void logOut();
        void downloadUserInfo();


    signals:
        bool onAuthChange(bool isAuth);

    public slots:

    private:
        static UserSession *m_instance;
        bool m_isAuth;
        QString m_token;
        QString m_userName;
        QList<Version> *m_versions;

        void parceUserData(QByteArray data);

        void save();
    };
}
#endif // USERSESSION_H
