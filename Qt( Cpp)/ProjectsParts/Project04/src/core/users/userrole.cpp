#include "userrole.h"
#include <QJsonObject>

namespace Core{

  UserRole::UserRole(QJsonObject jObject, QObject *parent)
  {
    _id = jObject.value("id").toInt();
    _name = jObject.value("name").toString();
    _isAdmin = jObject.value("isAdmin").toBool();
  }

  const int UserRole::id() const
  {
    return _id;
  }

  const QString &UserRole::name() const
  {
    return _name;
  }

  const bool UserRole::isAdmin() const
  {
    return _isAdmin;
  }
}
