#ifndef AUTH_H
#define AUTH_H

#include <QJsonObject>
#include <QObject>

namespace CnpApi{
  struct Auth{
      Auth(QJsonObject jObject);

  public:
      qint64 Id() const;
      QString AuthType() const;
      QString UserName() const;
      QString Password() const;
      QString Script() const;
      QString Url() const;
      QString Status() const;
      QString AuthScript();

  private:
      qint64 _id;
      QString _authtype;
      QString _userName;
      QString _password;
      QString _script;
      QString _url;
      QString _status;
  };

  inline Auth::Auth(QJsonObject jObject)
  {
      _id = jObject.value("Id").toInteger();
      _authtype = jObject.value("AuthType").toString();
      _userName = jObject.value("Username").toString();
      _password = jObject.value("Password").toString();
      _script = jObject.value("AuthScript").toString();
      _url = jObject.value("AuthUrl").toString();
      _status = jObject.value("Status").toString();
  }

  inline qint64 Auth::Id() const
  {
      return _id;
  }

  inline QString Auth::AuthType() const
  {
      return _authtype;
  }

  inline QString Auth::UserName() const
  {
      return _userName;
  }

  inline QString Auth::Password() const
  {
      return _password;
  }

  inline QString Auth::Script() const
  {
      return _script;
  }

  inline QString Auth::Url() const
  {
      return _url;
  }

  inline QString Auth::Status() const
  {
      return _status;
  }
  inline QString Auth::AuthScript()
  {
      return _script.replace("{{username}}", _userName).replace("{{password}}", _password);
  }
}
#endif
