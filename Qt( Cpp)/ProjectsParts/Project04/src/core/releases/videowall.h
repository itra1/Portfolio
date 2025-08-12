#ifndef VIDEOWALL_H
#define VIDEOWALL_H

#include "runrelease.h"

namespace Core {
class VideoWall : public RunRelease
{
	public:
	VideoWall();
	VideoWall(QJsonObject jObject);

	Release *getDependence();
	void findDependence();
	void downloadStreamingProgress(double progress);
	float getFullDownloadProgress() override;
	const bool isDownload() override;
	void afterCreate() override;
	void afterUnzip() override;
	bool checkState() override;
	void unpack(Release *parentRelease = nullptr) override;
	const QString exePath() const override;

    void download(const std::function<void(Release *)> &onComplete = nullptr,
                  const std::function<void(qint64, qint64)> &progress = nullptr,
                  const std::function<void(QString)> &onError = nullptr) override;

private:
    Release *_dependence;
};
} // namespace Core
#endif // VIDEOWALL_H
