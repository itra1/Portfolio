#include "server.h"
namespace Pipe {
Server::Server(QObject *parent)
    : QObject{parent}
{}

void Server::Run(QString serverName)
{
    _server = new QLocalServer(this);
    if (!_server->listen(serverName)) {
    } else {
    }
    connect(_server, SIGNAL(newConnection()), this, SLOT(NewConnectionSlot()), Qt::UniqueConnection);
}

void Server::NewConnectionSlot()
{
    QLocalSocket *localSocket = _server->nextPendingConnection();
    connect(localSocket, SIGNAL(readyRead()), this, SLOT(slotRead()), Qt::UniqueConnection);
}

void Server::ReadSlot()
{
    QLocalSocket *senderSocket = (QLocalSocket *) sender();
    QString response = senderSocket->readAll().remove(0, 2);
}

void Server::WriteSlot(QString message)
{
    QByteArray size;
    size.append((char) message.length() / 256);
    _client->write(size);
    QByteArray size2;
    size2.append((char) message.length() & 255);
    _client->write(size2);

    _client->write(message.toUtf8());
    _client->flush();
}
} // namespace Pipe
