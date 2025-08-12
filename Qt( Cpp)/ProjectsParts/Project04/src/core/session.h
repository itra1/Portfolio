#ifndef SESSION_H
#define SESSION_H

#include <QObject>
#include "./users/user.h"

namespace Core {

class Session : public QObject
{
	Q_OBJECT
	public:
	Session(const Session &) = delete;
	Session &operator=(const Session &) = delete;

	explicit Session(QObject *parent = nullptr);
	static void initInstance(QObject *parent = nullptr);
	static void freeInstance();
	static Session *instance();

	bool isPresentationMode() const;
	void setIsPresentationMode(bool newIsPresentationMode);
	const bool isRunWallAvailable() const;

	//! Текущий пользователь
	Q_INVOKABLE QVariant userVar();
	Q_INVOKABLE void logout();
	//! Имя
	Q_INVOKABLE const QString &firstName() const;
	//! Фамилия
	Q_INVOKABLE const QString &lastName() const;
	//! Мыло
	Q_INVOKABLE const QString &email() const;
	//! Мыло
	Q_INVOKABLE QString fullNameQml();

	Q_INVOKABLE Core::User *user();

	Q_PROPERTY(bool isAuth READ isAuth WRITE setIsAuth NOTIFY authChange FINAL)
	Q_PROPERTY(bool isRunWallAvailable READ isRunWallAvailable NOTIFY userChange FINAL)

	void setUser(const Core::User &newUser);

	QString authToken() const;
	void setAuthToken(const QString &newAuthToken);

	bool isAuth() const;
	void setIsAuth(bool newIslogin);

	signals:
	void authChange(bool isLogin);
	void userChange();

	private:
	static Session *_instance;
	bool _isPresentationMode;
	bool _isLog;
	bool _isAuth{false};
	QString _authToken;
	Core::User _user{};

	void emitIsAuth();
};
} // namespace Core
#endif // SESSION_H
