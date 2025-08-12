#include "user.h"
#include <QObject>
#include <QString>
#include <QJsonObject>
#include <QJsonArray>
#include "userrole.h"

namespace Core{

  User::User(QObject *parent)
    : QObject(parent)
  {

  }

  User::User(QJsonObject jObject, QObject *parent)
    : QObject(parent)
  {
    _id = jObject.value("id").toInt();
    _firstName = jObject.value("firstname").toString();
    _lastName = jObject.value("lastname").toString();
    _email = jObject.value("email").toString();
    auto roleList = jObject.value("roles").toArray();

    _roles.clear();
    for(auto role : roleList){
        _roles.push_back(new UserRole{role.toObject(),nullptr});
      }
  }

  User::User(const User &newUser)
  {
    _id = newUser.id();
    _firstName = newUser.firstName();
    _lastName = newUser.lastName();
    _email = newUser.email();
    _roles.clear();
    _roles = newUser.roles();

  }

  User &User::operator=(const User &newUser)
  {
    if(this == &newUser) return *this;
    _id = newUser.id();
    _firstName = newUser.firstName();
    _lastName = newUser.lastName();
    _email = newUser.email();
    _roles.clear();
    _roles = newUser.roles();
    return *this;
  }

  const bool User::isWall() const
  {
    return existsRole(VideoWallRole);
  }

  const bool User::isMobile() const
  {
    return existsRole(MobileOperatorRole);
  }

  const bool User::isAdmin() const
  {
    return existsRole(AdminRole);
  }

  const bool User::isSuperAdmin() const
  {
    return existsRole(SuperAdminRole);
  }

  const bool User::isPreviewGenerator() const
  {
    return existsRole("PreviewGenerator");
  }

  const bool User::isRunWallAvailable() const
  {
    return isWall() && (isSuperAdmin() || isAdmin());
  }

  QList<QString> User::getErrorList()
  {
    QList<QString> errors{};
    if(!isWall())
      errors.append(QString{"Отсутствует роль " + VideoWallRole});
    if(!isAdmin() && !isSuperAdmin())
      errors.append(QString{"Отсутствует роль "+SuperAdminRole+" или " + AdminRole});
    return errors;
  }

  const int User::id() const
  {
    return _id;
  }

  const QString &User::firstName() const
  {
    return _firstName;
  }

  const QString &User::lastName() const
  {
    return _lastName;
  }

  const QString &User::email() const
  {
    return _email;
  }

  const QList<UserRole *> &User::roles() const
  {
    return _roles;
  }

  const bool User::existsRole(QString roleName) const
  {
    if(_roles.size() == 0)
      return false;

    for(auto &element : _roles)
      if(element->name() == roleName)
        return true;
    return false;
  }
}
