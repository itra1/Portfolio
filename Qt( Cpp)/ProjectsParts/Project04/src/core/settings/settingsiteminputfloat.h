#ifndef SETTINGSITEMINPUTFLOAT_H
#define SETTINGSITEMINPUTFLOAT_H
#include "settingsitem.h"
namespace Core
{

	class SettingsItemInputFloat : public SettingsItem {
		public:
		explicit SettingsItemInputFloat(
			QJsonObject jObject, QObject *parent = nullptr);

		QString runProperty() override;
	};
} // namespace Core
#endif // SETTINGSITEMINPUTFLOAT_H
