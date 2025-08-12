#ifndef RENDERSTREAMINGCONNECTTYPE_H
#define RENDERSTREAMINGCONNECTTYPE_H

#include <QObject>
#include <QString>
#include <QVariantList>

namespace Core{

  class RenderStreamintConnectType : QObject{
    Q_OBJECT
  public:
    enum RSC_Type{
      http = 0x0,
      websocket = 0x1
    };
    Q_ENUM(RSC_Type);

    static inline QString RSC_HTTP = "http";
    static inline QString RSC_WEBSOCKET = "websocket";

    static QString getRenderStremingTypeByType(RSC_Type type){

      switch (type) {
        case RSC_Type::http:
          return RSC_HTTP;
        case RSC_Type::websocket:
          return RSC_WEBSOCKET;
        }
      return RSC_HTTP;
    }

  };
}
#endif // RENDERSTREAMINGCONNECTTYPE_H
