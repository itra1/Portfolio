#ifndef SETTINGS_H
#define SETTINGS_H

#include <QObject>
#include <QSettings>
#include <QVariant>
#include "settings/commonsettings.h"

namespace Core {

class Settings : public QObject
{
	Q_OBJECT
	public:
	Settings(const Settings &) = delete;
	Settings &operator=(const Settings &) = delete;

	explicit Settings(QObject *parent = nullptr);
	static void initInstance(QObject *parent = nullptr);
	static void freeInstance();
	static Settings *instance();
	void load();

	Q_INVOKABLE QObject *getBaseSettingsQObject();

	CommonSettings *getBaseSettings();

	signals:

	private:
	static Settings *_instance;

	CommonSettings *_commonSettings;
	QSettings *_autorunSettings;
};
} // namespace Core
#endif // SETTINGS_H
