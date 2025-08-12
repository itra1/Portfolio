#include "settingsiteminputint.h"

namespace Core
{

	SettingsItemInputInt::SettingsItemInputInt(
		QJsonObject jObject, QObject *parent)
			: SettingsItem{jObject, parent}
	{
	}

	QString SettingsItemInputInt::runProperty()
	{
		auto rKey = runKey();

		return rKey.length() > 0 ? rKey + "=" + QString::number(valueAsInt()) : "";
	}

} // namespace Core
