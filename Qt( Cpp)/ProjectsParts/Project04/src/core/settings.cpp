#include "settings.h"
#include <QDir>
#include <QVariant>

namespace Core{

  Settings *Settings::_instance = nullptr;
	Settings::Settings(QObject *parent)
			: QObject(parent)
			, _commonSettings(new CommonSettings())
			, _autorunSettings(new QSettings("HKEY_CURRENT_"
																			 "USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Run",
																			 QSettings::NativeFormat))
	{
	}
	void Settings::initInstance(QObject *parent)
  {
    if(!_instance)
      _instance = new Settings(parent);
  }

  void Settings::freeInstance()
  {
    if (_instance) {
        delete _instance;
        _instance = nullptr;
      }
  }

  Settings *Settings::instance()
  {
    return _instance;
  }

  void Settings::load()
  {
		_commonSettings->load();
	}

	QObject *Settings::getBaseSettingsQObject() { return _commonSettings; }

	CommonSettings *Settings::getBaseSettings() { return _commonSettings; }
}
