#include "userrolemodel.h"

namespace Api {
  namespace Models {

    UserRoleModel::UserRoleModel() : BaseModel{} {}

    UserRoleModel::UserRoleModel(QJsonObject jObject) : BaseModel{jObject} {
      _name = jObject.value("name").toString();
      _isAdmin = jObject.value("isAdmin").toBool();
    }

    UserRoleModel::UserRoleModel(const UserRoleModel &other)
        : BaseModel{other}, _name{other.name()}, _isAdmin{other.isAdmin()} {}

    UserRoleModel &UserRoleModel::operator=(const UserRoleModel &other) {
      BaseModel::operator=(other);
      _name = other.name();
      _isAdmin = other.isAdmin();
      return *this;
    }

    UserRoleModel::UserRoleModel(const UserRoleModel &&other) noexcept
        : BaseModel{other}, _name{other.name()}, _isAdmin{other.isAdmin()} {}

    UserRoleModel &
    UserRoleModel::operator=(const UserRoleModel &&other) noexcept {
      BaseModel::operator=(other);
      _name = other.name();
      _isAdmin = other.isAdmin();
      return *this;
    }

    const QString &UserRoleModel::name() const { return _name; }

    const bool UserRoleModel::isAdmin() const { return _isAdmin; }

  } // namespace Models
} // namespace Api
