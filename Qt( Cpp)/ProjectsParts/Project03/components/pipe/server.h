#ifndef SERVER_H
#define SERVER_H

#include <QObject>
#include <QtNetwork/QLocalServer>
#include <QtNetwork/QLocalSocket>

namespace Pipe {
class Server : public QObject
{
    Q_OBJECT
public:
    explicit Server(QObject *parent = nullptr);

    void Run(QString serverName);

signals:

private slots:
    void NewConnectionSlot();
    void ReadSlot();
    void WriteSlot(QString message);

private:
    QLocalServer *_server;
    QLocalSocket *_client;
};
} // namespace Pipe
#endif // SERVER_H
