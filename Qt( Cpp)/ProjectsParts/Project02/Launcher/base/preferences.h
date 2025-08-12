#ifndef PREFERENCES_H
#define PREFERENCES_H

#include <QNetworkCookie>
#include <QObject>


enum SchedulerDays
{
    EVERY_DAY,
    WEEK_DAYS,
    WEEK_ENDS,
    MON,
    TUE,
    WED,
    THU,
    FRI,
    SAT,
    SUN
};

class Preferences : public QObject
{
    Q_OBJECT
    Preferences();

public:
    static void initInstance();
    static void freeInstance();
    static Preferences *instance();

    int getTrackerPort() const;

    bool recheckTorrentsOnCompletion() const;
    void recheckTorrentsOnCompletion(bool recheck);

signals:

public slots:


private:
    static Preferences *m_instance;

    // Network
    QList<QNetworkCookie> getNetworkCookies() const;
    void setNetworkCookies(const QList<QNetworkCookie> &cookies);
};

#endif // PREFERENCES_H
