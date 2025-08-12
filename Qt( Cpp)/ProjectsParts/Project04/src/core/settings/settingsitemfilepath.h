#ifndef SETTINGSITEMFILEPATH_H
#define SETTINGSITEMFILEPATH_H

#include "settingsitem.h"

namespace Core
{
	class SettingsItemFilePath : public SettingsItem {
		public:
		explicit SettingsItemFilePath(
			QJsonObject jObject, QObject *parent = nullptr);

		QString runProperty() override;
	};
} // namespace Core
#endif // SETTINGSITEMFILEPATH_H
