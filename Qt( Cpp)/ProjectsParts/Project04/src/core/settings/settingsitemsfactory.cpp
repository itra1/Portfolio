#include "settingsitemsfactory.h"
#include "settingsitemfilepath.h"
#include "settingsiteminputfloat.h"
#include "settingsiteminputint.h"
#include "settingsiteminputpassword.h"
#include "settingsiteminputstring.h"
#include "settingsitemtoggle.h"
#include "settingsitemtoggleregedit.h"

namespace Core
{
	SettingsItemsFactory::SettingsItemsFactory() {}
	
	SettingsItem *SettingsItemsFactory::getItem(QJsonObject object)
	{
		auto type = object.value("type");

		if (type == "toggle")
			return new SettingsItemToggle(object);
		if (type == "toggleRegedit")
			return new SettingsItemToggleRegedit(object);
		if (type == "inputInt")
			return new SettingsItemInputInt(object);
		if (type == "inputFloat")
			return new SettingsItemInputFloat(object);
		if (type == "inputString" || type == "hiddenString")
			return new SettingsItemInputString(object);
		if (type == "inputPassword")
			return new SettingsItemInputPassword(object);
		if (type == "filePath")
			return new SettingsItemFilePath(object);
		return nullptr;
	}

} // namespace Core
