#ifndef CONFIGKEYS_H
#define CONFIGKEYS_H
#include <QString>

struct ConfigKeys
{
	inline static QString apiUrl{"api"};
	inline static QString videoWallType{"videowallType"};
	inline static QString launcherType{"launcherType"};
	inline static QString streamingAssetsType{"streamingAssetsType"};
	inline static QString crossType{"crossType"};
	inline static QString browserType{"browserType"};
	inline static QString installPath{"installPath"};
	inline static QString browserPath{"browserPath"};
	inline static QString downloadPath{"downloadPath"};
	inline static QString updatePath{"updatePath"};
	inline static QString cachePath{"cachePath"};
	inline static QString cacheBrowserPath{"cacheBrowserPath"};
	inline static QString streamingAssetsPath{"streamingAssetsPath"};
	inline static QString distribFolder{"distribFolder"};
	inline static QString videoWallExe{"videowallExe"};
	inline static QString crossExe{"crossExe"};
	inline static QString launcherExe{"launcherExe"};
	inline static QString browserExe{"browserExe"};
	inline static QString favoriteBuildKey{"favoriteBuildKey"};
	inline static QString clientConfig{"clientConfig"};
	inline static QString sumAdaptive_minTimeLiveClient{
			"sumAdaptive_minTimeLiveClient"};
	inline static QString sumAdaptive_hourInDayToReload{
			"sumAdaptive_hourInDayToReload"};
	inline static QString sumAdaptive_runKey{"sumAdaptive_runKey"};
	inline static QString sumAdaptive_buildKey{"sumAdaptive_buildKey"};
};

#endif // CONFIGKEYS_H
