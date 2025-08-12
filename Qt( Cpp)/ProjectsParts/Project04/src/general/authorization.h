#ifndef login_H
#define login_H

#include <QNetworkReply>
#include <QVariant>

namespace General {

class Authorization : public QObject
{
	Q_OBJECT
	public:
	explicit Authorization(QObject *parent = nullptr);

	static void initInstance();
	static void freeInstance();
	static Authorization *instance();

	void authorizationStart();
	void auth();
	void authStart();

	//! Текущий пользователь
	Q_INVOKABLE QString userName();
	//! Текущий пароль
	Q_INVOKABLE QString password();
	//! Идентификатор сервера
	Q_INVOKABLE QString serverId();
	//! Имя сервера
	Q_INVOKABLE QString serverName();
	//! Url сервера
	Q_INVOKABLE const QString server() const;
	//! Последняя авторизация заполнена
	Q_INVOKABLE const bool remember() const;

	Q_INVOKABLE void login(QString userName, QString password, bool isRemember, QString serverId);

	void setRemember(bool newRemember);
	void setServer(const QString &newServerNest);
	void setIsAuthProcess(bool isAuthprocess);

	signals:
	void isAuthStart(bool isStart);
	void authError(QString error);

	private:
	static Authorization *_instance;

	QString _server;
	QString _userName;
	QString _password;
	QString _serverId;
	bool _remember{false};
	bool _isAuthProcess{false};

	void loginAuthFinish(QNetworkReply *reply);
};
} // namespace General

#endif // login_H
