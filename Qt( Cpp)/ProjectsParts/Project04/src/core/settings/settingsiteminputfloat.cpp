#include "settingsiteminputfloat.h"

namespace Core
{
	SettingsItemInputFloat::SettingsItemInputFloat(
		QJsonObject jObject, QObject *parent)
			: SettingsItem{jObject, parent}
	{
	}

	QString SettingsItemInputFloat::runProperty()
	{
		auto rKey = runKey();

		return rKey.length() > 0 ? rKey + "=" + QString::number(valueAsFloat())
														 : "";
	}
} // namespace Core
