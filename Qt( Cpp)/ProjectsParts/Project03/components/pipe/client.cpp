#include "client.h"

#include <QDataStream>
#include <QDebug>
#include <QObject>

namespace Pipe
{

    Client::Client()
        : QObject(nullptr)
    {
    }

    void Client::Connect(QString serverName)
    {
        _localClient = new QLocalSocket(this);
        _localClient->connectToServer(serverName);
        qDebug() << "Connected " << serverName;
        connect(_localClient,
                &QLocalSocket::readyRead,
                this,
                &Client::ReadCommandsSlot,
                Qt::UniqueConnection);
        connect(
            _localClient,
            &QLocalSocket::aboutToClose,
            this,
            [=]() {
                qDebug() << "Disconnect";
                emit OnConnectClose();
            },
            Qt::UniqueConnection);
    }

    void Client::Write(QByteArray message)
    {
        long long multi = message.length() / 256;
        long long add = message.length() & 255;

        long long len = multi * 256 + add;

        qDebug() << "Pipe out: "<< len << " send: " << message;

        QByteArray sendMessage;
        sendMessage.append((char)multi);
        sendMessage.append((char)add);
        sendMessage.append(message);

        _localClient->write(sendMessage);
    }

    void Client::ReadCommandsSlot()
    {
        QByteArray response = _localClient->readAll().remove(0, 2);

        if (response == "")
        {
            qDebug() << "Pipe in empty";
            return;
        }

        QString responseStr = response;

        if (responseStr.mid(0, 1) != "[")
        {
            responseStr = "[\"" + responseStr;
            response = responseStr.toUtf8();
        }

        qDebug() << "Pipe in: " << response;

        emit OnMessageSignal(response);
    }

} // namespace Pipe
