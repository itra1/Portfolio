#ifndef SETTINGSSTORAGE_H
#define SETTINGSSTORAGE_H

#include <QDebug>
#include <QObject>
#include <QSettings>

namespace General
{

	class SettingsStorage : public QObject {
		Q_OBJECT
		explicit SettingsStorage(QObject *parent = nullptr);

		public:
		static void initInstance();
		static void freeInstance();
		static SettingsStorage *instance();

		QVariant loadValue(
			const QString &key, const QVariant &defaultValue = QVariant()) const;
		void storeValue(const QString &key, const QVariant &value);
		bool hashValue(const QString &key);
		void remove(const QString &key);

		void saveSync();

		bool isDirty();

		signals:

		public slots:
		bool save();

		private:
		static SettingsStorage *_instance;
		bool _dirty;
		QSettings *_settings;
	};
} // namespace General

#endif // SETTINGSSTORAGE_H
