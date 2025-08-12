#include "streamingassets.h"
#include "../../config/config.h"

namespace Core {
StreamingAssets::StreamingAssets()
		: Release()
{}

StreamingAssets::StreamingAssets(QJsonObject jObject)
		: Release(jObject)
{}

void StreamingAssets::unpack(Release *parentRelease)
{
	this->parentRelease = parentRelease;
	Release::unpack();
}

const QString StreamingAssets::installPath() const
{
	return (parentRelease == nullptr ? Release::installPath() : parentRelease->installPath())
				 + Config::getStringValue(ConfigKeys::streamingAssetsPath);
}

} // namespace Core
