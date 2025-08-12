#ifndef CORE_SETTINGSITEMTOGGLEREGEDIT_H
#define CORE_SETTINGSITEMTOGGLEREGEDIT_H

#include "settingsitem.h"
#include <QObject>

namespace Core
{

	class SettingsItemToggleRegedit : public SettingsItem {
		public:
		SettingsItemToggleRegedit(QJsonObject jObject, QObject *parent = nullptr);

		QString runProperty() override;

		void confirmChange() override;
	};

} // namespace Core

#endif // CORE_SETTINGSITEMTOGGLEREGEDIT_H
