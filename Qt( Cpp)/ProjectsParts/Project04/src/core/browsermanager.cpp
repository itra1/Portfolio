#include "browsermanager.h"
#include "../config/config.h"
#include "helpers/releasehelper.h"
#include "src/filehelper.h"

namespace Core {
BrowserManager::BrowserManager(Core::ReleaseLoader *releaseLoader, Core::ReleaseManager *releaseManager)
    : QObject(nullptr)
    , _releaseLoader{releaseLoader}
    , _releaseManager{releaseManager}
{
    CheckInstallVersion();
}

void BrowserManager::ReleasesLoaded()
{
    CheckNeedUpdate();
}

void BrowserManager::Download(Release *release)
{
    if (_isUpdateProcess)
        return;

    _isUpdateProcess = true;
    release->download([&](Release *release) { Extract(release); });
}

void BrowserManager::Extract(Release *release)
{
    release->unpack();
    _isUpdateProcess = false;
    CheckInstallVersion();
    CheckNeedUpdate();
}

void BrowserManager::CheckNeedUpdate()
{
    Release *rec = nullptr;

    QString version = _installVersion;

    auto releaseList = _releaseManager->getReleasesList();

    for (int i = 0; i < releaseList->count(); i++) {
        if (releaseList->value(i)->type == Config::getStringValue(ConfigKeys::browserType)
            && Core::ReleaseHelper::MaxVersion(releaseList->value(i)->version, version)) {
            version = releaseList->value(i)->version;
            rec = releaseList->value(i);
        }
    }

    if (version != _installVersion) {
        qDebug() << "Browser update to " << rec->version;
        _updateVersion = rec;
        Download(rec);
    }
}

void BrowserManager::CheckInstallVersion()
{
    QFile file(ExePath());

    if (file.exists()){
        auto curVersion = QtSystemLib::FileHelper::GetFileVersion(file.filesystemFileName().wstring());
        _installVersion = QString::fromStdString(curVersion);
    }

    qDebug() << "Installed browser version " << _installVersion;
}

QString BrowserManager::ExePath()
{
    auto exePath = Config::getPath(ConfigKeys::installPath) + Config::getStringValue(ConfigKeys::browserPath) + "\\"
                   + Config::getStringValue(ConfigKeys::browserExe);

    qDebug() << "Browser exe " << exePath;

    return exePath;
}

void BrowserManager::NeedUpdateSignalEmit()
{
    emit NeedUpdateSignal();
}

} // namespace Core
