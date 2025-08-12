#ifndef SETTINGSITEM_H
#define SETTINGSITEM_H

#include <QJsonObject>
#include <QObject>

namespace Core
{
	struct SettingsItem : public QObject
	{
		Q_OBJECT

		public:
		explicit SettingsItem(QJsonObject jObject, QObject *parent = nullptr);

		//! Ключ
		Q_INVOKABLE QString key();
		//! Имя для ui
		Q_INVOKABLE QString name();
		//! Подсказка для ui
		Q_INVOKABLE QString toolType();
		//! Тип ui
		Q_INVOKABLE QString type();

        Q_INVOKABLE bool isEnable();
        //! Ключ к запуску сборке
		Q_INVOKABLE QString runKey();
		Q_INVOKABLE QString paramAsString(QString key);
		Q_INVOKABLE bool paramAsBool(QString key);

		Q_INVOKABLE bool valueAsBool();
		Q_INVOKABLE void setValueAsBool(bool newValue);

		Q_INVOKABLE QString valueAsString();
		Q_INVOKABLE void setValueAsString(QString newValue);

		Q_INVOKABLE float valueAsFloat();
		Q_INVOKABLE void setValueAsFloat(float newValue);

		Q_INVOKABLE int valueAsInt();
		Q_INVOKABLE void setValueAsInt(int newValue);

		Q_INVOKABLE QVariant valueAsVariant();
		Q_INVOKABLE void setValueAsVariant(QVariant newValue);

		Q_INVOKABLE QJsonValue valueAsJsonValue();

		virtual void confirmChange();
		virtual QString runProperty() = 0;

		QString makeKey();
		bool existsProperty(QString property);

		signals:
		void valueChange(SettingsItem *item);

		private:
		void parceJson(QJsonObject jObject);

		protected:
		//! Значение
		QVariant _value;
		QMap<QString, QJsonValue> _params{};
	};

} // namespace Core
#endif // SETTINGSITEM_H
