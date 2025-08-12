#ifndef COPYTHEARDS_H
#define COPYTHEARDS_H

#include <QObject>

class CopyTheards : public QObject
{
    Q_OBJECT

public:
    CopyTheards(QString fromPath, QString toPath, QObject *parent = nullptr);

public slots:
    void run();

signals:
    void onStart();
    void onStartCopy();
    void onProgress(double progress);
    void onFinished();

private:
    QString m_fromPath;
    QString m_toPath;
    QList<QString> *m_listFiles;
    QList<QString> *m_listDirs;
    double m_sizeFiles;
    double sizeCopy;
};

#endif // COPYTHEARDS_H
