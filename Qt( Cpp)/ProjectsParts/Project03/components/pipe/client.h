#ifndef CLIENT_H
#define CLIENT_H

#include <QObject>
#include <QtNetwork/QLocalSocket>
#include <QJsonDocument>
#include <QJsonObject>
#include <QJsonArray>

namespace Pipe
{

class Client : public QObject
{
    Q_OBJECT
public:
    explicit Client();

    void Connect(QString serverName);
    void Write(QByteArray message);

public slots:
    void ReadCommandsSlot();

signals:
    void OnMessageSignal(QString message);
    void OnConnectClose();

private:
    QLocalSocket *_localClient;
};
}

#endif
