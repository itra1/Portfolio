#ifndef SETTINGSITEMSFACTORY_H
#define SETTINGSITEMSFACTORY_H

#include "QJsonObject"
#include "settingsitem.h"
#include <QObject>

namespace Core
{

	struct SettingsItemsFactory
	{
		SettingsItemsFactory();
		
		static SettingsItem *getItem(QJsonObject object);
	};
} // namespace Core
#endif // SETTINGSITEMSFACTORY_H
