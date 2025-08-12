#ifndef RELEOASELOADER_H
#define RELEOASELOADER_H
#include <QQueue>
#include "releases/release.h"

namespace Core {

class ReleaseLoader
{
public:
    ReleaseLoader();
    void DownloadItem(Release *item);

    bool IsDownloadProcess() const;
    void NextDownload();
    void SetIsDownloadProcess(bool newIsDownloadProcess);

private:
private:
    Release *_dowloadItem;
    bool _isDownloadProcess{false};
    QQueue<Core::Release *> _downloadQueue;
};
} // namespace Core
#endif // RELEOASELOADER_H
