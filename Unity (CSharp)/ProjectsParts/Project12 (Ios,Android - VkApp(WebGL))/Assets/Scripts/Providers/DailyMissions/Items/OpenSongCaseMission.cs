using Game.Scripts.Providers.DailyMissions.Base;
using Game.Scripts.Providers.DailyMissions.Common;
using itra.Attributes;

namespace Game.Scripts.Providers.DailyMissions.Items
{
	[PrefabName(DailyMissionType.OpenSongCase)]
	public class OpenSongCaseMission : Mission
	{
		public override string Type => DailyMissionType.OpenSongCase;
	}
}
