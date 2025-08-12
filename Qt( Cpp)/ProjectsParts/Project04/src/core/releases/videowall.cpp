#include "videowall.h"
#include "../../config/config.h"

namespace Core {
VideoWall::VideoWall()
		: RunRelease()
{}

VideoWall::VideoWall(QJsonObject jObject)
		: RunRelease(jObject)
{}

Release *VideoWall::getDependence()
{
	if (_dependence == nullptr)
		_dependence = getStreamingAssets();
	return _dependence;
}

void VideoWall::findDependence()
{
	_dependence = getStreamingAssets();
}

void VideoWall::downloadStreamingProgress(double progress)
{
	getDependence()->setDownloadProgress(progress);
	if (!isStreamingAsset())
		emitDownloadProgress();
}

float VideoWall::getFullDownloadProgress()
{
	return (_downloadProgress
					+ (!this->IsLocalOnly
								 ? (getDependence() == nullptr ? 0 : getDependence()->getDownloadProgress())
								 : 0))
				 / 2;
}

const bool VideoWall::isDownload()
{
	return existsZip() && getDependence()->existsZip();
}

void VideoWall::afterCreate()
{
	findDependence();
}

void VideoWall::afterUnzip()
{
	getDependence()->unpack(this);
}

bool VideoWall::checkState()
{
	if (getState() == ReleaseState::Loading && getDependence()->getState() == ReleaseState::Loading) {
		setState(ReleaseState::Loading);
		return true;
	}
	return RunRelease::checkState();
}

void VideoWall::unpack(Release *parentRelease)
{
	RunRelease::unpack();
	getDependence()->unpack(this);
}

const QString VideoWall::exePath() const
{
	return installPath() + "/" + Config::getStringValue(ConfigKeys::videoWallExe);
}

void VideoWall::download(const std::function<void(Release *)> &onComplete,
                         const std::function<void(qint64, qint64)> &progress,
                         const std::function<void(QString)> &onError)
{
    Release::download(
        [&, onComplete](Release *res) {
            if (getDependence()->existsZip() && existsZip())
                onComplete(this);
        },
        progress,
        onError);

    getDependence()->download(
        [&, onComplete](Release *res) {
            emitDownloadProgress();
            checkState();
            //downloadAnyCallback();

            if (getDependence()->existsZip() && existsZip())
                onComplete(this);
        },
        [&](double p1, double p2) { emitDownloadProgress(); });
}
} // namespace Core
