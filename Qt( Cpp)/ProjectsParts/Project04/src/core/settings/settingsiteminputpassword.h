#ifndef CORE_SETTINGSITEMINPUTPASSWORD_H
#define CORE_SETTINGSITEMINPUTPASSWORD_H

#include "settingsitem.h"
#include <QObject>

namespace Core
{

	class SettingsItemInputPassword : public SettingsItem {
		Q_OBJECT
		public:
		explicit SettingsItemInputPassword(
			QJsonObject jObject, QObject *parent = nullptr);

		QString runProperty() override;
	};

} // namespace Core

#endif // CORE_SETTINGSITEMINPUTPASSWORD_H
