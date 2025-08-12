#ifndef API_MODELS_USER_H
#define API_MODELS_USER_H

#include "basemodel.h"
#include "userrolemodel.h"

namespace Api {
namespace Models {

struct UserModel: BaseModel
{
  UserModel();
  UserModel(QJsonObject jObject);
  UserModel(const UserModel& other);
  UserModel& operator=(const UserModel& other);
  UserModel(const UserModel&& other);
  UserModel& operator=(const UserModel&& other);

  //! Присутствует роль стены
  const bool isWall() const;
  //! Присутствует роль админа
  const bool isAdmin() const;
  //! Присутствует роль супер админа
  const bool isSuperAdmin() const;
  //! Пользователю разрешен запуск стены
  const bool isRunWallAvailable() const;

  const QString &firstName() const;
  const QString &lastName() const;
  const QString &email() const;
  QList<UserRoleModel *> roles() const;

private:

  QString _firstName;
  QString _lastName;
  QString _email;
  QList<UserRoleModel*> _roles;

  const bool existsRole(QString roleName) const;
};

} // namespace Models
} // namespace Api

#endif // API_MODELS_USER_H
