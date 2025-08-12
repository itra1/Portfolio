#include "settingsiteminputstring.h"

namespace Core
{

	SettingsItemInputString::SettingsItemInputString(
		QJsonObject jObject, QObject *parent)
			: SettingsItem{jObject, parent}
	{
	}

	QString SettingsItemInputString::runProperty()
	{
		auto rKey = runKey();
		return rKey.length() > 0 ? rKey + "=" + valueAsString() : "";
	}

} // namespace Core
