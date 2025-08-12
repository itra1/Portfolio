#include "settingsitem.h"
#include <QtMath>
namespace Core
{
	
	SettingsItem::SettingsItem(QJsonObject jObject, QObject *parent)
			: QObject{parent}
	{
		parceJson(jObject);
	}
	
	QString SettingsItem::key() { return _params["key"].toString(); }
	
	QString SettingsItem::name() { return _params["name"].toString(); }
	
	QString SettingsItem::toolType() { return _params["toolTipe"].toString(); }
	
	QString SettingsItem::type() { return _params["type"].toString(); }
	
	QString SettingsItem::runKey() { return _params["runKey"].toString(); }

    bool SettingsItem::isEnable()
    {
        if (!_params.contains("isEnable"))
            return true;
        return _params["isEnable"].toBool(true);
    }

    QString SettingsItem::paramAsString(QString key)
	{
		return existsProperty(key) ? _params[key].toString() : "";
	}
	
	bool SettingsItem::paramAsBool(QString key)
	{
		return existsProperty(key) ? _params[key].toBool(false) : false;
	}

	bool SettingsItem::valueAsBool()
	{
		if (_value == NULL)
			return false;
		return _value.toBool();
	}

	void SettingsItem::setValueAsBool(bool newValue)
	{
		_value = newValue;
		confirmChange();
		emit valueChange(this);
	}
	
	QString SettingsItem::valueAsString() { return _value.toString(); }
	
	void SettingsItem::setValueAsString(QString newValue)
	{
		_value = newValue;
		confirmChange();
		emit valueChange(this);
	}

	void SettingsItem::setValueAsFloat(float newValue)
	{
		_value = newValue;
		confirmChange();
		emit valueChange(this);
	}

	float SettingsItem::valueAsFloat() { return _value.toFloat(); }

	int SettingsItem::valueAsInt() { return _value.toInt(); }

	void SettingsItem::setValueAsInt(int newValue)
	{
		_value = newValue;
		confirmChange();
		emit valueChange(this);
	}
	
	QVariant SettingsItem::valueAsVariant() { return _value; }
	
	void SettingsItem::setValueAsVariant(QVariant newValue)
	{
		_value = newValue;
		emit valueChange(this);
	}

	QJsonValue SettingsItem::valueAsJsonValue() { return _value.toJsonValue(); }

	void SettingsItem::confirmChange() {}

	QString SettingsItem::makeKey() { return runKey(); }

	bool SettingsItem::existsProperty(QString property)
	{
		bool exists = _params.contains(property);
		if (!exists) {
			qDebug() << "Отсутствует свойство " << property;
			return "";
		}
		return exists;
	}
	
	void SettingsItem::parceJson(QJsonObject jObject)
	{
		for (QString key : jObject.keys()) {
			if (key == "default") {
				_value = jObject[key].toVariant();
				continue;
			}
			_params.insert(key, jObject[key]);
		}
	}

} // namespace Core
