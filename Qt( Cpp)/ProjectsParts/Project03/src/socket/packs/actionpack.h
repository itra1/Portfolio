#ifndef ACTIONPACK_H
#define ACTIONPACK_H
#include "../../customtypes.h"
#include "packbase.h"
#include "sio_message.h"

namespace Sockets {
namespace Packs {

class ActionPack : private PackBase
{
public:
    ActionPack(sio::message::ptr pack);
    const std::string action() const { return _action; };
    static std::string GetActionName(sio::message::ptr pack);

protected:
    sio::object_message *_content;
    std::string _action;
};

Sockets::Packs::ActionPack::ActionPack(sio::message::ptr pack)
    : Sockets::Packs::PackBase(pack)
{
    const sio::object_message *data = static_cast<sio::object_message *>(pack.get());
    const std::map<std::string, sio::message::ptr> *values = &(data->get_map());

    _action = GetActionName(pack);

    auto content = values->find("content");
    if (content != values->end()) {
        _content = std::static_pointer_cast<sio::object_message>(content->second).get();
    }
}

std::string Sockets::Packs::ActionPack::GetActionName(sio::message::ptr pack)
{
    const sio::object_message *data = static_cast<sio::object_message *>(pack.get());
    const std::map<std::string, sio::message::ptr> *values = &(data->get_map());

    auto action = values->find("action");
    if (action != values->end()) {
        return std::static_pointer_cast<sio::string_message>(action->second)->get_string();
    }
    return "";
}

} // namespace Packs
} // namespace Sockets

#endif // ACTIONPACK_H
