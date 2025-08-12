#ifndef STREAMINGASSETS_H
#define STREAMINGASSETS_H

#include "release.h"

namespace Core {

class StreamingAssets : public Release
{
	public:
	StreamingAssets();
	StreamingAssets(QJsonObject jObject);

	void unpack(Release *parentRelease = nullptr) override;

	const QString installPath() const override;

	private:
	Release *parentRelease{nullptr};
};
} // namespace Core
#endif // STREAMINGASSETS_H
