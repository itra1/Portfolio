#ifndef APIBASE_H
#define APIBASE_H

#include <QJsonObject>

namespace Core{

  class ApiBase : public QObject{

  public:
    QJsonObject sourceJson;
  };

}

#endif // APIBASE_H
