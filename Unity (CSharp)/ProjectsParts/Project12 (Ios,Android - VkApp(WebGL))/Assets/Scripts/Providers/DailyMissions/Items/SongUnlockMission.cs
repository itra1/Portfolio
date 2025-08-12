using Game.Scripts.Providers.DailyMissions.Base;
using Game.Scripts.Providers.DailyMissions.Common;
using itra.Attributes;

namespace Game.Scripts.Providers.DailyMissions.Items
{
	[PrefabName(DailyMissionType.SongUnlock)]
	public class SongUnlockMission : Mission
	{
		public override string Type => DailyMissionType.SongUnlock;
	}
}
