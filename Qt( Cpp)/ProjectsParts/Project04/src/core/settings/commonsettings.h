#ifndef COMMONSETTINGS_H
#define COMMONSETTINGS_H

#include "settingsbase.h"
#include <QObject>
#include <QSettings>

namespace Core
{

	class CommonSettings : public SettingsBase {
		Q_OBJECT
		public:
		explicit CommonSettings(QObject *parent = nullptr);

		void load();
		void save() override;

		int getScreenBorderUp();
		int getScreenBorderDown();
		int getScreenBorderLeft();
		int getScreenBorderRight();
		int getTargetResolutionX();
		int getTargetResolutionY();

		signals:
		void googleChromePathChange();

		public slots:
		void itemChange();
		void itemChange(SettingsItem *item) override;

		private:
		QString _saveKey{"baseSettinga"};
		QSettings *_autorunSettings;

		private:
	};
} // namespace Core
#endif // COMMONSETTINGS_H
