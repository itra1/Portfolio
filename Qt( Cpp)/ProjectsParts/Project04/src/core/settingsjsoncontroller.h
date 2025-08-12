#ifndef SETTINGSJSONCONTROLLER_H
#define SETTINGSJSONCONTROLLER_H

#include <QJsonArray>
#include <QJsonDocument>
#include <QJsonObject>
#include <QObject>

namespace Core
{
	struct SettingsJsonController
	{
		static void readConfig();
		static QJsonArray getArray(QString key);

		inline static QJsonObject _jsonObject;
	};
} // namespace Core

#endif // SETTINGSJSONCONTROLLER_H
