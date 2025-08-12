#ifndef SERVER_H
#define SERVER_H

#include "../settings/settingsbase.h"
#include <QObject>

namespace Core
{

	class Server : public SettingsBase {
		Q_OBJECT
		public:
		Server(QObject *parent = nullptr);
		Server(QString name, QObject *parent = nullptr);
		Server(QJsonObject jObj, QObject *parent = nullptr);

		Q_INVOKABLE bool isValid();
		Q_INVOKABLE QString id();
		Q_INVOKABLE QString name();
		QString url();

		QJsonObject getJson();

		private:
		// void load();
		void save() override;

		signals:

		private:
	};
} // namespace Core
// Q_DECLARE_METATYPE(Core::Server)
#endif // SERVER_H
