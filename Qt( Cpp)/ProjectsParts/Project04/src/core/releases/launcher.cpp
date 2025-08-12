#include "launcher.h"

namespace Core {
Launcher::Launcher()
		: Release()
{}

Launcher::Launcher(QJsonObject jObject)
		: Release(jObject)
{}
} // namespace Core
