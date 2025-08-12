#include "settingsjsoncontroller.h"
#include <QJsonArray>
#include <QJsonObject>
#include "src/iohelper.h"

namespace Core
{
	void SettingsJsonController::readConfig()
	{
        auto data = QtSystemLib::IOHelper::TextFileRead(":/settings/resources/settings.json");

        if (data == nullptr) {
			qDebug() << "[Ошибка] не удается найти файл ресурсов настроек";
		}

		QJsonObject obj = QJsonDocument::fromJson(data).object();
		_jsonObject = obj;
	}

	QJsonArray SettingsJsonController::getArray(QString key)
	{
		if (_jsonObject.contains(key)) {
			auto itm = _jsonObject.value(key).toArray();
			return itm;
		}
		QJsonArray jArray;
		return jArray;
	}

} // namespace Core
