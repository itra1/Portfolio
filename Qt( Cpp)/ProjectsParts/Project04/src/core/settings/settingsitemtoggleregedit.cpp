#include "settingsitemtoggleregedit.h"

namespace Core
{

	SettingsItemToggleRegedit::SettingsItemToggleRegedit(
		QJsonObject jObject, QObject *parent)
			: SettingsItem{jObject, parent}
	{
	}

	QString SettingsItemToggleRegedit::runProperty()
	{
		auto rKey = runKey();

        return rKey.length() > 0 && valueAsBool() ? rKey : "";
    }

	void SettingsItemToggleRegedit::confirmChange() {}

} // namespace Core
