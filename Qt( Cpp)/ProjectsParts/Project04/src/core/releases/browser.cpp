#include "browser.h"
#include "../../config/config.h"

namespace Core {
Browser::Browser()
    : Release()
{}

Browser::Browser(QJsonObject jObject)
    : Release(jObject)
{}

const QString Browser::installPath() const
{
    return Config::getPath(ConfigKeys::installPath) + QString("/browser");
}

const QString Browser::exePath() const
{
    return installPath() + "/" + Config::getStringValue(ConfigKeys::browserExe);
}

} // namespace Core
