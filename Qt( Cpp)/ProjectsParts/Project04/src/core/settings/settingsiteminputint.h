#ifndef CORE_SETTINGSITEMINPUTINT_H
#define CORE_SETTINGSITEMINPUTINT_H

#include "settingsitem.h"
#include <QObject>

namespace Core
{

	class SettingsItemInputInt : public SettingsItem {
		Q_OBJECT
		public:
		explicit SettingsItemInputInt(
			QJsonObject jObject, QObject *parent = nullptr);

		QString runProperty() override;
	};

} // namespace Core

#endif // CORE_SETTINGSITEMINPUTINT_H
