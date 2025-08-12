#include "configcontroller.h"
#include <QFile>
#include <QJsonArray>
#include <QJsonDocument>
#include <QJsonObject>
#include "../config/config.h"
#include "src/iohelper.h"

namespace Core
{

	QJsonObject ConfigController::_document = QJsonObject();

	ConfigController::ConfigController() {}

	void ConfigController::load()
	{
		auto configFile = Config::configFile();

        auto data = QtSystemLib::IOHelper::TextFileRead(configFile);

        if (data == nullptr) {
			qDebug() << "ConfigController error read";
		}

		_document = QJsonDocument::fromJson(data).object();
	}

	void ConfigController::save()
	{
		QJsonDocument jDoc(_document);

		auto configFile = Config::configFile();
		QFile file(configFile);
		file.open(QFile::WriteOnly);
		file.write(jDoc.toJson());
		file.close();
	}

	void ConfigController::setData(QString key, QJsonObject data)
	{
		_document[key] = data;
		save();
	}

	void ConfigController::setData(QString key, QJsonArray data)
	{
		_document[key] = data;
		save();
	}

	QJsonValue ConfigController::getValue(QString key)
	{
		return _document.contains(key) ? _document.value(key) : QJsonValue{};
	}

	QJsonObject ConfigController::getObject(QString key)
	{
		return _document.contains(key) ? _document.value(key).toObject()
																	 : QJsonObject{};
	}

	QJsonArray ConfigController::getArray(QString key)
	{
		return _document.contains(key) ? _document.value(key).toArray()
																	 : QJsonArray{};
	}
} // namespace Core
