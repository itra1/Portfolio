#ifndef BROWSERMANAGER_H
#define BROWSERMANAGER_H

#include "releaseloader.h"
#include "releasemanager.h"

namespace Core {
class BrowserManager : public QObject
{
    Q_OBJECT
public:
    BrowserManager(Core::ReleaseLoader *releaseLoader, Core::ReleaseManager *releaseManager);

private:
    //! Загружаем
    void Download(Release *release);
    //! Экспортируем релиз
    void Extract(Release *release);
    //! Проверка необходимости обновления
    void CheckNeedUpdate();
    //! Проверить установленный экземпляр
    void CheckInstallVersion();

    //! Пусть к исполняемому файлу
    QString ExePath();

    void NeedUpdateSignalEmit();

public slots:
    //! События загрузки релизов
    void ReleasesLoaded();

signals:
    void NeedUpdateSignal();

private:
    Core::ReleaseLoader *_releaseLoader;
    Core::ReleaseManager *_releaseManager;
    Core::Release *_updateVersion{nullptr};
    QString _installVersion{"0.0.0.0"};
    bool _updateReady{false};
    bool _isUpdateProcess{false};
};
} // namespace Core
#endif // BROWSERMANAGER_H
