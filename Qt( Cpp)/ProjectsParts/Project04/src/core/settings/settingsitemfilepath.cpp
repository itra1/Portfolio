#include "settingsitemfilepath.h"

namespace Core
{
	SettingsItemFilePath::SettingsItemFilePath(
		QJsonObject jObject, QObject *parent)
			: SettingsItem{jObject, parent}
	{
	}

	QString SettingsItemFilePath::runProperty()
	{
		auto rKey = runKey();
		auto val = valueAsString();
		return rKey.length() > 0 && val.length() > 0
						 ? rKey + "=" + val.toUtf8().toBase64()
						 : "";
	}
} // namespace Core
