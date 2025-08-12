#ifndef USERROLE_H
#define USERROLE_H

#include <QObject>
#include <QJsonObject>

namespace Core{

  class UserRole : public QObject
  {
    Q_OBJECT
  public:
    explicit UserRole(QJsonObject jObject, QObject *parent = nullptr);

    const int id() const;
    const QString &name() const;
    const bool isAdmin() const;

  signals:

  private:
    int _id;
    QString _name;
    bool _isAdmin;

  };
}
#endif // USERROLE_H
