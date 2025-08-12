#include "releasetag.h"
#include <QJsonObject>

namespace Core {
ReleaseTag::ReleaseTag(QObject *parent)
		: QObject{parent}
{}

int ReleaseTag::id() const
{
	return _id;
}

QString ReleaseTag::name() const
{
	return _name;
}

QString ReleaseTag::description() const
{
	return _description;
}

void ReleaseTag::parse(QJsonObject jObj)
{
	_id = jObj.value("id").toInt();
	_name = jObj.value("name").toString();
	_description = jObj.value("description").toString();
}
} // namespace Core
