#include "releasemanager.h"
#include <QDateTime>
#include <QJsonArray>
#include <QFileInfo>
#include <QTimer>
#include "../config/config.h"
#include "api.h"
#include "helpers/releasehelper.h"
#include "releases/browser.h"
#include "releases/cross.h"
#include "releases/launcher.h"
#include "releases/streamingassets.h"
#include "releases/videowall.h"
#include "session.h"

namespace Core
{

    ReleaseManager *ReleaseManager::_instance = nullptr;

    ReleaseManager::ReleaseManager(Core::ReleaseLoader *releaseLoader)
        : QObject(nullptr), _releaseLoader{releaseLoader}, _releasesList(new QList<Core::Release *>()), _releaseTags(new QList<Core::ReleaseTag *>())
    {
        connect(Core::Session::instance(), SIGNAL(authChange(bool)), SLOT(authChange(bool)));
    }

    void ReleaseManager::initInstance(Core::ReleaseLoader *releaseLoader)
    {
        if (!_instance)
            _instance = new ReleaseManager(releaseLoader);
    }

    void ReleaseManager::freeInstance()
    {
        if (_instance)
        {
            delete _instance;
            _instance = nullptr;
        }
    }

    ReleaseManager *ReleaseManager::instance()
    {
        return _instance;
    }

    QVariantList ReleaseManager::getVideowallList()
    {
        QVariantList list;

        for (auto elem : *_releasesList)
        {
            if (elem->isRunned())
                list.append(QVariant::fromValue((RunRelease *)elem));
        }
        return list;
    }

    QVariantList ReleaseManager::getVideowallList(bool isFavorite, QString tag)
    {
        QVariantList list;

        for (auto elem : *_releasesList)
        {
            if (elem->isRunned() && (tag == "" || elem->existsTag(tag)) && (!isFavorite || elem->getIsFavorite() == isFavorite))
                list.append(QVariant::fromValue((RunRelease *)elem));
        }
        return list;
    }

    QVariantList ReleaseManager::getTagsList()
    {
        QVariantList list;

        for (auto elem : *_releaseTags)
            list.append(QVariant::fromValue(elem->name()));

        return list;
    }

    void ReleaseManager::loadReleases()
    {
        if (!Core::Session::instance()->isAuth())
            return;

        Api::loadReleases(
            [&](QJsonArray jListReleases) {
                if (noLocalOnlyReleases() == jListReleases.count())
                    return;

                _releasesList->clear();
                for (int i = 0; i < jListReleases.count(); i++)
                {
                    auto oneRelease = getReleaseByType(jListReleases[i].toObject());

                    for (int ii = 0; ii < oneRelease->tags.count(); ii++)
                        addTagIfNeed(oneRelease->tags[ii]);

                    _releasesList->append(oneRelease);
                }
                afterLoadReleases();
            },
            [&](QString error) {

            });

        // Повторяем запрос каждую минуту
        QTimer::singleShot(60000, this, SLOT(loadReleases()));
    }

    void ReleaseManager::afterLoadReleases()
    {
        ReadInstallReleases();
        afterCreate();
        checkAllStates();

        QMap<QDateTime, Release *> releasesMap;

        for (int i = 0; i < _releasesList->length(); i++)
        {
            releasesMap.insert(_releasesList->value(i)->createAt, _releasesList->value(i));
        }

        _releasesList->clear();

        foreach (QDateTime key, releasesMap.keys())
        {
            _releasesList->insert(0, releasesMap.value(key));
        }

        qDebug() << "Releases count = " << _releasesList->length();

        autoPlayReleaseWithSumlite();

        emit releaseLoadSignal();
    }

    void ReleaseManager::ReadInstallReleases()
    {
        QDir installpath(Config::getPath(ConfigKeys::installPath));
        
        QFileInfoList list = installpath.entryInfoList();
        foreach (QFileInfo finfo, list)
        {
            QString name = finfo.fileName();
            if (name == "." || name == "..")
                continue;
            ReadInstallDirRelease(name);
        }
    }

    void ReleaseManager::ReadInstallDirRelease(QString folderName)
    {
        QStringList version = folderName.split("_");

        if (version[0] == Config::getStringValue(ConfigKeys::browserType))
            return;

        QString fullPath = Config::getPath(ConfigKeys::installPath) + "/" + folderName;
        QFileInfo readdir(fullPath);

        Core::Release *newRes = getReleaseByType(version[0]);
        newRes->version = version[1];
        newRes->type = version[0];
        newRes->createAt = newRes->birthTime();

        bool existsRelease = false;

        for (int i = 0; i < _releasesList->count(); i++)
        {
            if (_releasesList->value(i)->type == newRes->type && _releasesList->value(i)->version == newRes->version)
            {
                existsRelease = true;
            }
        }

        if (!existsRelease && newRes->existsDisk())
        {
            newRes->IsLocalOnly = true;
            _releasesList->append(newRes);
        }
    }
    int ReleaseManager::noLocalOnlyReleases()
    {
        int count = 0;

        for (int i = 0; i < _releasesList->count(); i++) {
            if (!_releasesList->value(i)->IsLocalOnly) {
                count++;
            }
        }
        return count;
    }

    void ReleaseManager::afterCreate()
    {
        for (int i = 0; i < _releasesList->count(); i++)
        {
            _releasesList->value(i)->afterCreate();
        }
    }

    void ReleaseManager::checkAllStates()
    {
        for (int i = 0; i < _releasesList->count(); i++)
        {
            if (_releasesList->value(i)->type == "videoWall" || _releasesList->value(i)->type == "cross")
                _releasesList->value(i)->checkState();
        }
    }

    void ReleaseManager::autoPlayReleaseWithSumlite()
    {
        if (Core::Session::instance()->authToken().length() > 0 && Core::Session::instance()->isPresentationMode())
        {
            auto runRelease = getPresentationRelease();
            if (runRelease && runRelease->existsDisk() && !runRelease->isRunning())
                ((RunRelease *)runRelease)->run();
        }
    }

    void ReleaseManager::clear()
    {
        if (_releasesList != nullptr)
            _releasesList->clear();
        if (_releaseTags != nullptr)
            _releaseTags->clear();
    }

    void ReleaseManager::addTagIfNeed(ReleaseTag *tag)
    {
        for (int i = 0; i < _releaseTags->count(); i++)
        {
            if (_releaseTags->value(i)->id() == tag->id())
                return;
        }

        _releaseTags->append(tag);
    }

    void ReleaseManager::downloadItem(Release *item)
    {
        _releaseLoader->DownloadItem(item);
    }

    Release *ReleaseManager::getReleaseByType(QJsonObject jObject)
    {
        QString type = jObject.value("type").toString();

        if (type == Config::getStringValue(ConfigKeys::streamingAssetsType))
            return new Core::StreamingAssets(jObject);
        if (type == Config::getStringValue(ConfigKeys::launcherType))
            return new Core::Launcher(jObject);
        if (type == Config::getStringValue(ConfigKeys::crossType))
            return new Core::Cross(jObject);
        if (type == Config::getStringValue(ConfigKeys::browserType))
            return new Core::Browser(jObject);
        return new Core::VideoWall(jObject);
    }

    Release *ReleaseManager::getReleaseByType(QString type)
    {
        if (type == Config::getStringValue(ConfigKeys::streamingAssetsType))
            return new Core::StreamingAssets();
        if (type == Config::getStringValue(ConfigKeys::launcherType))
            return new Core::Launcher();
        if (type == Config::getStringValue(ConfigKeys::crossType))
            return new Core::Cross();
        if (type == Config::getStringValue(ConfigKeys::browserType))
            return new Core::Browser();
        return new Core::VideoWall();
    }

    QList<Core::Release *> *ReleaseManager::getReleasesList() const
    {
        return _releasesList;
    }

    QList<Core::Release *> *ReleaseManager::getReleasesList(QString type) const
    {
        QList<Release *> *list = new QList<Release *>{};

        for (int i = 0; i < _releasesList->count(); i++)
        {
            if (_releasesList->value(i)->type == type)
                list->append(_releasesList->value(i));
        }

        return list;
    }

    Release *ReleaseManager::getReleaseById(QString version)
    {
        for (int i = 0; i < _releasesList->count(); i++)
        {
            Release *release = _releasesList->value(i);
            if (release->version == version)
                return release;
        }
        return nullptr;
    }

    const QString &ReleaseManager::getErrorMessage() const
    {
        return _errorMessage;
    }

    void ReleaseManager::setErrorMessage(const QString &newErrorMessage)
    {
        _errorMessage = newErrorMessage;
        emitErrorMessage();
    }

    void ReleaseManager::authChange(bool isLogin)
    {
        if (!isLogin)
        {
            _releasesList->clear();
            return;
        }
        loadReleases();
    }

    void ReleaseManager::emitErrorMessage()
    {
        emit onErrorMessageSignal();
    }

    Release *ReleaseManager::getPresentationRelease()
    {
        Release *result = nullptr;

        for (int i = 0; i < _releasesList->count(); i++)
        {
            Release *release = _releasesList->value(i);
            if (!release->isVideowall() || !release->isForSumLite())
                continue;

            if (result == nullptr || Core::ReleaseHelper::MaxVersion(release->version.split(QLatin1Char('_'))[0],
                                                                     result->version.split(QLatin1Char('_'))[0]))
            {
                result = release;
            }
        }

        return result;
    }

} // namespace Core
