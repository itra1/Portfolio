#ifndef CONFIGITEM_H
#define CONFIGITEM_H

#include <QJsonValue>
#include <QString>
#include <QVariant>

namespace Configs
{

	struct ConfigItem
	{
		QString key;
		QJsonValue value;
		QString description;
	};

} // namespace Configs

#endif // CONFIGITEM_H
