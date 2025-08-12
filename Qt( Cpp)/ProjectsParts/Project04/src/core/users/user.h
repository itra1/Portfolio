#ifndef USER_H
#define USER_H

#include <QObject>
#include <QJsonObject>
#include "userrole.h"

namespace Core{

  class User : public QObject
  {
    Q_OBJECT
  public:
    explicit User(QObject *parent = nullptr);
    explicit User(QJsonObject jObject, QObject *parent = nullptr);
    explicit User(const User& newUser);
    User &operator=(const User& newUser);

    //! Присутствует роль стены
    Q_INVOKABLE const bool isWall() const;
    //! Присутствует роль мобильного оператора
    Q_INVOKABLE const bool isMobile() const;
    //! Присутствует роль админа
    Q_INVOKABLE const bool isAdmin() const;
    //! Присутствует роль супер админа
    Q_INVOKABLE const bool isSuperAdmin() const;
    //! Присутствует роль генератора превью
    Q_INVOKABLE const bool isPreviewGenerator() const;
    //! Пользователю разрешен запуск стены
    Q_INVOKABLE const bool isRunWallAvailable() const;

    //! Получить список ошибок
    QList<QString> getErrorList();

    //! Идентификатор
    const int id() const;
    //! Имя
		Q_INVOKABLE const QString &firstName() const;
		//! Фамилия
		Q_INVOKABLE const QString &lastName() const;
		//! Мыло
		Q_INVOKABLE const QString &email() const;

		const QList<UserRole *> &roles() const;

  signals:

  private:
    int _id;
    QString _firstName;
    QString _lastName;
    QString _email;
    QList<UserRole*> _roles{};

  private:
    const QString VideoWallRole{"VideoWall"};
    const QString AdminRole{"Admin"};
    const QString SuperAdminRole{"SuperAdmin"};
    const QString MobileOperatorRole{"MobileOperator"};

    //! Проверка наличия роли
    const bool existsRole(QString roleName) const;

  };
}
//Q_DECLARE_METATYPE(Core::User)
#endif // USER_H
