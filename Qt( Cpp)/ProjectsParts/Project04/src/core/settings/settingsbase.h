#ifndef SETTINGSBASE_H
#define SETTINGSBASE_H

#include "../settings/settingsitem.h"
#include <QObject>

namespace Core
{
	class SettingsBase : public QObject {
		Q_OBJECT
		public:
		explicit SettingsBase(QObject *parent = nullptr);
		explicit SettingsBase(QJsonObject jObject, QObject *parent = nullptr);

		virtual void save() = 0;

		Q_INVOKABLE QVariantList optionsQmlList();

		void subscribeChange();
		void readConfigFile(QString key);
		void loadJsonObject(QJsonObject jObj);
		void setValue(QString keyName, QVariant value);
		QVariant value(QString keyName);
		QJsonObject makeJsonObject();

		//! Получаем список ключей для запуска стены
		QString getRunWallKeys();
        void makeRunWallKey(QStringList &opt, bool cross);

		public slots:
		virtual void itemChange(SettingsItem *item);

		protected:
		QJsonObject _jObject;
		QList<Core::SettingsItem *> *_keyList;
	};
} // namespace Core
#endif // SETTINGSBASE_H
