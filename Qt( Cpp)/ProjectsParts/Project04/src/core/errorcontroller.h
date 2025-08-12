#ifndef ERRORCONTROLLER_H
#define ERRORCONTROLLER_H

#include <QObject>
#include <QVariantList>

namespace Core {

class ErrorController : public QObject
{
	Q_OBJECT
	public:
	ErrorController(const ErrorController &) = delete;
	ErrorController &operator=(const ErrorController &) = delete;

	explicit ErrorController(QObject *parent = nullptr);
	static void initInstance(QObject *parent = nullptr);
	static void freeInstance();
	static ErrorController *instance();

	//! Список ошибок
	Q_INVOKABLE QVariantList getErrorList();

	signals:

	private:
	static ErrorController *_instance;
};
} // namespace Core
#endif // ERRORCONTROLLER_H
