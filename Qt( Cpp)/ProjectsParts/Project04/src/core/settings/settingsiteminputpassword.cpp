#include "settingsiteminputpassword.h"

namespace Core
{

	SettingsItemInputPassword::SettingsItemInputPassword(
		QJsonObject jObject, QObject *parent)
			: SettingsItem{jObject, parent}
	{
	}

	QString SettingsItemInputPassword::runProperty()
	{
		auto rKey = runKey();
		return rKey.length() > 0 ? rKey + "=" + valueAsString() : "";
	}

} // namespace Core
