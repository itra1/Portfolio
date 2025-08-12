#ifndef LAUNCHER_H
#define LAUNCHER_H

#include "release.h"

namespace Core {
class Launcher : public Release
{
	public:
	Launcher();
	Launcher(QJsonObject jObject);
};
} // namespace Core
#endif // LAUNCHER_H
