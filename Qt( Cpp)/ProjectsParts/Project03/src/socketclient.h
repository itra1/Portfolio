#ifndef SOCKETCLIENT_H
#define SOCKETCLIENT_H
#include <QDebug>
#include "sio_client.h"
#include <cstdlib>

using namespace sio;

class SocketClient
{
public:
    SocketClient();
    void Connect();

    QString token() const;
    void setToken(const QString &newToken);

private:
    QString _token;
    std::unique_ptr<client> _io;
    void OnConnected(std::string const &nsp);
    void OnClosed(client::close_reason const &reason);
    void OnFailed();
    void OnOpen();
    void OnError(std::string const &name,
                 message::ptr const &data,
                 bool hasAck,
                 message::list &ack_resp);
    void OnAction(std::string const &name,
                  message::ptr const &data,
                  bool hasAck,
                  message::list &ack_resp);
};

#endif // SOCKETCLIENT_H
