#include "settingsitemtoggle.h"

namespace Core
{

	SettingsItemToggle::SettingsItemToggle(QJsonObject jObject, QObject *parent)
			: SettingsItem{jObject, parent}
	{
	}

	QString SettingsItemToggle::runProperty()
	{
		auto rKey = runKey();

        return rKey.length() > 0 && valueAsBool() ? rKey : "";
    }

} // namespace Core
