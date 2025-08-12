#ifndef SETTINGSITEMTOGGLE_H
#define SETTINGSITEMTOGGLE_H

#include "settingsitem.h"
namespace Core
{
	struct SettingsItemToggle : public SettingsItem
	{
		Q_OBJECT
		public:
		explicit SettingsItemToggle(QJsonObject jObject, QObject *parent = nullptr);

		QString runProperty() override;
	};
} // namespace Core

#endif // SETTINGSITEMTOGGLE_H
