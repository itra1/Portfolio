#ifndef LOGGER_H
#define LOGGER_H

#include <QFile>
#include <QString>

class Logger
{
	public:
	Logger(const Logger &) = delete;
	Logger &operator=(const Logger &) = delete;

	Logger();

	void init();
	void subscribe();
	void removeOldFiles();
	QString getLogPath();
	static void messageHandler(QtMsgType type, const QMessageLogContext &context, const QString &msg);

	private:
	static QScopedPointer<QFile> *_logFile;
};

#endif // LOGGER_H
