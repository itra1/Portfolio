#ifndef API_MODELS_USERROLE_H
#define API_MODELS_USERROLE_H
#include "basemodel.h"

namespace Api {
  namespace Models {

    struct UserRoleModel : BaseModel {
      inline static const QString VideoWallRole{"VideoWall"};
      inline static const QString AdminRole{"Admin"};
      inline static const QString SuperAdminRole{"SuperAdmin"};
      inline static const QString MobileOperatorRole{"MobileOperator"};

      UserRoleModel();
      UserRoleModel(QJsonObject jObject);
      UserRoleModel(const UserRoleModel& other);
      UserRoleModel& operator=(const UserRoleModel& other);
      UserRoleModel(const UserRoleModel&& other) noexcept;
      UserRoleModel& operator=(const UserRoleModel&& other) noexcept;

      const QString& name() const;
      const bool isAdmin() const;

    private:
      QString _name;
      bool _isAdmin;
    };

  } // namespace Models
} // namespace Api

#endif // API_MODELS_USERROLE_H
