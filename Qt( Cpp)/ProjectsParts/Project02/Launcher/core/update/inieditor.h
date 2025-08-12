#ifndef INIEDITOR_H
#define INIEDITOR_H

#include <QObject>
#include <QSettings>

namespace Core{

    class IniEditor : public QObject
    {
        Q_OBJECT
    public:
        explicit IniEditor(QString filePath, QObject *parent = nullptr);

        void write(QString key, int value);
        void write(QString key, double value);
        void write(QString key, QString value);
        void write(QHash<QString,QVariant> value);
        bool hash(QString key);

        QHash<QString,QVariant> read();
        QVariant read(QString key);
        QVariant read(QString key, QVariant defauld);

        void sync();

    signals:

    public slots:

    private:
        QSettings *m_settings;

    };
}

#endif // INIEDITOR_H
