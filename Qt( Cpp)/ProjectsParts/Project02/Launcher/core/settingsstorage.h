#ifndef SETTINGSSTORAGE_H
#define SETTINGSSTORAGE_H

#include <QObject>
#include <QSettings>
#include <QDebug>

namespace Core{

    class SettingsStorage : public QObject
    {
        Q_OBJECT
        explicit SettingsStorage(QObject *parent = nullptr);

    public:
        static void initInstance();
        static void freeInstance();
        static SettingsStorage *instance();

        QVariant loadValue(const QString &key, const QVariant &defaultValue = QVariant()) const;
        void storeValue(const QString &key, const QVariant &value);
        bool hashValue(const QString &key);
        void remove(const QString &key);

        void saveSync();

        bool isDirty();

    signals:

    public slots:
        bool save();

    private:
        bool m_dirty;
        static SettingsStorage *m_instance;
        QSettings *m_settings;
    };
}

#endif // SETTINGSSTORAGE_H
