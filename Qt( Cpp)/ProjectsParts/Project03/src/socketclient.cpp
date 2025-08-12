#include "socketclient.h"
#include <QJsonDocument>
#include <QJsonObject>
#include "config/config.h"
#include "socket/packs/actionpack.h"
#include "socket/packs/windowMaterialAction.h"

#ifdef WIN32
#define BIND_EVENT(IO, EV, FN) \
    do { \
        socket::event_listener_aux l = FN; \
        IO->on(EV, l); \
    } while (0)

#else
#define BIND_EVENT(IO, EV, FN) IO->on(EV, FN)
#endif

SocketClient::SocketClient()
    : _io{new client()}
{}

void SocketClient::Connect()
{
    using std::placeholders::_1;
    using std::placeholders::_2;
    using std::placeholders::_3;
    using std::placeholders::_4;

    auto packAuth = sio::object_message::createPack();
    packAuth->insert("token", _token.toStdString());
    _io->setAuth(sio::object_message::makePtr(packAuth));
    socket::ptr sock = _io->socket();

    BIND_EVENT(sock, "error", std::bind(&SocketClient::OnError, this, _1, _2, _3, _4));
    BIND_EVENT(sock, "action", std::bind(&SocketClient::OnAction, this, _1, _2, _3, _4));

    _io->set_open_listener(std::bind(&SocketClient::OnOpen, this));
    _io->set_socket_open_listener(
        std::bind(&SocketClient::OnConnected, this, std::placeholders::_1));
    _io->set_close_listener(std::bind(&SocketClient::OnClosed, this, _1));
    _io->set_fail_listener(std::bind(&SocketClient::OnFailed, this));
    auto server = Config::getStringValue(ConfigKeys::server)
                  + Config::getStringValue(ConfigKeys::socket);
    _io->connect(server.toStdString(), sio::object_message::makePtr(packAuth));
}

QString SocketClient::token() const
{
    return _token;
}

void SocketClient::setToken(const QString &newToken)
{
    _token = newToken;
}

void SocketClient::OnConnected(std::string const &nsp)
{
    qDebug() << "event OnConnected";
}

void SocketClient::OnError(std::string const &name,
                           message::ptr const &data,
                           bool hasAck,
                           message::list &ack_resp)
{
    qDebug() << "event Error";
    if (data->get_flag() == message::flag_string) {
        qDebug() << data->get_string();
    }
    if (data->get_flag() == message::flag_object) {
        for (auto &item : data->get_map()) {
            qDebug() << item.first << " "
                     << (item.second->get_flag() == message::flag_string
                             ? QString::fromStdString(item.second->get_string())
                             : QString::number(item.second->get_int()));
        }
    }
}

void SocketClient::OnAction(const std::string &name,
                            const message::ptr &data,
                            bool hasAck,
                            message::list &ack_resp)
{
    qDebug() << "event OnAction";

    auto packName = Sockets::Packs::ActionPack::GetActionName(data);

    bool targetActive = Sockets::Packs::WindowsMaterialAction::ThisAction(packName);

    qDebug() << "Incoming action: " << packName;
}

void SocketClient::OnClosed(client::close_reason const &reason)
{
    qDebug() << "event OnClosed";
}

void SocketClient::OnFailed()
{
    qDebug() << "event OnFailed";
}

void SocketClient::OnOpen()
{
    qDebug() << "event OnOpen";
}
