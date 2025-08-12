#ifndef CORE_SETTINGSITEMINPUTSTRING_H
#define CORE_SETTINGSITEMINPUTSTRING_H

#include "settingsitem.h"
#include <QObject>

namespace Core
{

	class SettingsItemInputString : public SettingsItem {
		Q_OBJECT
		public:
		explicit SettingsItemInputString(
			QJsonObject jObject, QObject *parent = nullptr);

		QString runProperty() override;
	};

} // namespace Core

#endif // CORE_SETTINGSITEMINPUTSTRING_H
