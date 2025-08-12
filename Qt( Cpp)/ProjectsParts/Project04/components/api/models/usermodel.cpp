#include "usermodel.h"
#include "userrolemodel.h"
#include <QJsonArray>

namespace Api {
  namespace Models {

    UserModel::UserModel() : BaseModel{} {}

    UserModel::UserModel(QJsonObject jObject) : BaseModel{jObject} {
      _firstName = _jObject.value("firstname").toString();
      _lastName = _jObject.value("lastname").toString();
      _email = _jObject.value("email").toString();

      auto roleList = _jObject.value("roles").toArray();
      _roles.clear();
      for (auto role : roleList) {
        _roles.push_back(new UserRoleModel{role.toObject()});
      }
    }

    UserModel::UserModel(const UserModel &other)
        : BaseModel(other), _firstName(other.firstName()),
          _lastName(other.lastName()), _email(other.email()) {}

    UserModel &UserModel::operator=(const UserModel &other) {
      BaseModel::operator=(other);
      _firstName = other.firstName();
      _lastName = other.lastName();
      _email = other.email();
      return *this;
    }

    UserModel::UserModel(const UserModel &&other)
        : BaseModel(other), _firstName(other.firstName()),
          _lastName(other.lastName()), _email(other.email()) {}

    UserModel &UserModel::operator=(const UserModel &&other) {
      BaseModel::operator=(other);
      _firstName = other.firstName();
      _lastName = other.lastName();
      _email = other.email();
      return *this;
    }

    const bool UserModel::isWall() const {
      return existsRole(UserRoleModel::VideoWallRole);
    }

    const bool UserModel::isAdmin() const {
      return existsRole(UserRoleModel::AdminRole);
    }

    const bool UserModel::isSuperAdmin() const {
      return existsRole(UserRoleModel::SuperAdminRole);
    }

    const bool UserModel::isRunWallAvailable() const {
      return isWall() && (isSuperAdmin() || isAdmin());
    }

    const QString &UserModel::firstName() const { return _firstName; }

    const QString &UserModel::lastName() const { return _lastName; }

    const QString &UserModel::email() const { return _email; }

    QList<UserRoleModel *> UserModel::roles() const { return _roles; }

    const bool UserModel::existsRole(QString roleName) const {
      if (_roles.size() == 0)
        return false;

      for (auto &element : _roles)
        if (element->name() == roleName)
          return true;
      return false;
    }

  } // namespace Models
} // namespace Api
