#include "session.h"

namespace Core {

Session *Session::_instance = nullptr;

Session::Session(QObject *parent)
		: QObject(parent)
		, _isPresentationMode(false)
		, _isLog(true)
{}

void Session::initInstance(QObject *parent)
{
	if (!_instance)
		_instance = new Session(parent);
}

void Session::freeInstance()
{
	if (_instance) {
		delete _instance;
		_instance = nullptr;
	}
}

Session *Session::instance()
{
	return _instance;
}

bool Session::isPresentationMode() const
{
	return _isPresentationMode;
}

void Session::setIsPresentationMode(bool newIsPresentationMode)
{
	_isPresentationMode = newIsPresentationMode;
}

QVariant Session::userVar()
{
	return QVariant::fromValue(&_user);
}

const bool Session::isRunWallAvailable() const
{
	if (_user.id() == 0)
		return false;

	return _user.isRunWallAvailable();
}

void Session::logout()
{
	qDebug() << "logout";
	setAuthToken("");
	setIsAuth(false);
}

const QString &Session::firstName() const
{
	return _user.firstName();
}

const QString &Session::lastName() const
{
	return _user.lastName();
}

const QString &Session::email() const
{
	return _user.email();
}

QString Session::fullNameQml()
{
	return _user.firstName() + " " + _user.lastName() + " (" + _user.email() + ")";
}

Core::User *Session::user()
{
	return &_user;
}

void Session::setUser(const Core::User &newUser)
{
	_user = newUser;
	emit userChange();
}

QString Session::authToken() const
{
	return _authToken;
}

void Session::setAuthToken(const QString &newAuthToken)
{
	_authToken = newAuthToken;
}

bool Session::isAuth() const
{
	return _isAuth;
}

void Session::setIsAuth(bool newIslogin)
{
	_isAuth = newIslogin;
	emitIsAuth();
}

void Session::emitIsAuth()
{
	emit authChange(_isAuth);
}
} // namespace Core
