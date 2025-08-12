#ifndef CROSS_H
#define CROSS_H

#include <QJsonObject>
#include "runrelease.h"

namespace Core {
class Cross : public RunRelease
{
	public:
	Cross();
	Cross(QJsonObject jObject);

	void afterUnpack() override;
	const QString exePath() const override;
	//const bool existsDisk() override;
};
} // namespace Core
#endif // CROSS_H
