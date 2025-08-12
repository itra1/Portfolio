#include "releaseloader.h"

namespace Core {
ReleaseLoader::ReleaseLoader() {}

void ReleaseLoader::DownloadItem(Release *item)
{
    _downloadQueue.enqueue(item);
    NextDownload();
}

void ReleaseLoader::NextDownload()
{
    if (_isDownloadProcess)
        return;
    if (_downloadQueue.count() <= 0)
        return;

    _dowloadItem = _downloadQueue.dequeue();
    _isDownloadProcess = true;

    _dowloadItem->download(
        [&](Release *res) {
            SetIsDownloadProcess(false);
            NextDownload();
        },
        nullptr,
        nullptr);
}

bool ReleaseLoader::IsDownloadProcess() const
{
    return _isDownloadProcess;
}

void ReleaseLoader::SetIsDownloadProcess(bool newIsDownloadProcess)
{
    _isDownloadProcess = newIsDownloadProcess;
}

} // namespace Core
