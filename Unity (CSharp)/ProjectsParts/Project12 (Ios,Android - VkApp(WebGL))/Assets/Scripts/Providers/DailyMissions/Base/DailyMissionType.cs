using StringDrop;

namespace Game.Scripts.Providers.DailyMissions.Base
{
	public struct DailyMissionType
	{
		[StringDropItem] public const string DestroyAnyNote = "DestroyAnyNote";
		[StringDropItem] public const string NewStars = "NewStars";
		[StringDropItem] public const string DestroyTapNote = "DestroyTapNote";
		[StringDropItem] public const string DestroyHoldNote = "DestroyHoldNote";
		[StringDropItem] public const string CompleteTrack = "CompleteTrack";
		[StringDropItem] public const string SongExplorer = "SongExplorer";
		[StringDropItem] public const string SongUnlock = "SongUnlock";
		[StringDropItem] public const string OpenSongCase = "OpenSongCase";
	}
}
