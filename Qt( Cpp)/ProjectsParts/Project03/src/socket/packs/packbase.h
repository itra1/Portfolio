#ifndef PACKBASE_H
#define PACKBASE_H
#include "sio_message.h"

namespace Sockets{
  namespace Packs{

    class PackBase{
    public:
        PackBase(sio::message::ptr pack);

    protected:
        sio::object_message *_content;
        std::string _action;
    };

    Sockets::Packs::PackBase::PackBase(sio::message::ptr pack) {}

    } // namespace Packs
}

#endif // PACKBASE_H
