#include "commonsettings.h"
#include "../../config/config.h"
#include "../configcontroller.h"
#include "../settings/settingsitem.h"
#include <QDir>
#include <QJsonArray>
#include <QJsonDocument>
#include <QJsonObject>

namespace Core
{
	CommonSettings::CommonSettings(QObject *parent)
			: SettingsBase(parent),
				_autorunSettings{new QSettings(
					"HKEY_CURRENT_"
					"USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Run",
					QSettings::NativeFormat)}
	{
		readConfigFile("commonSettings");

		// load();
	}

	void CommonSettings::load()
	{
		QJsonObject jObj = Core::ConfigController::getObject(_saveKey);

		if (jObj.count() <= 0) {
			subscribeChange();
			return;
		}

		// Чтение сохранения
		loadJsonObject(jObj);
	}

	void CommonSettings::save()
	{
		QJsonObject jObj = makeJsonObject();

		Core::ConfigController::setData(_saveKey, jObj);
	}

	int CommonSettings::getScreenBorderUp()
	{
		return value("screenBorderUp").toInt();
	}

	int CommonSettings::getScreenBorderDown()
	{
		return value("screenBorderDown").toInt();
	}

	int CommonSettings::getScreenBorderLeft()
	{
		return value("screenBorderLeft").toInt();
	}

	int CommonSettings::getScreenBorderRight()
	{
		return value("screenBorderRight").toInt();
	}

	int CommonSettings::getTargetResolutionX()
	{
		return value("targetResolutionX").toInt();
	}

	int CommonSettings::getTargetResolutionY()
	{
		return value("targetResolutionY").toInt();
	}

	void CommonSettings::itemChange() { save(); }

	void CommonSettings::itemChange(SettingsItem *item)
	{
		if (item->type() == "toggleRegedit") {
			auto itemKey = item->key();
			bool isSumLight = itemKey == "systemAutoStartSumAdaptive";
			auto itemValue = item->valueAsBool();
			auto disableKey = itemKey == "systemAutoStart"
													? "systemAutoStartSumAdaptive"
													: "systemAutoStart";
			auto disableValue = value(disableKey).toBool();

			auto path =
				Config::currentPath().replace("/", "\\") + "\\Launcher.exe";
			QString regRunKey = "Launcher";
			if (!itemValue && !disableValue) {
				_autorunSettings->remove(regRunKey);
			}
			if (itemValue) {
				if (isSumLight)
					path += " " + Config::getStringValue(ConfigKeys::sumAdaptive_runKey);
				_autorunSettings->setValue(regRunKey, path);
				setValue(disableKey, false);
			}
			// if (_systemAutoStart) {
			// 	_autorunSettings->setValue(regRunKey, path);
			// }
			// if (_systemAutoStartSumLight) {
			// 	_autorunSettings->setValue(
			// 		regRunKey,
			// 		path + " " +
			// Config::getStringValue(ConfigKeys::sumAdaptive_runKey));
			// }
		}
		save();
	}
} // namespace Core
