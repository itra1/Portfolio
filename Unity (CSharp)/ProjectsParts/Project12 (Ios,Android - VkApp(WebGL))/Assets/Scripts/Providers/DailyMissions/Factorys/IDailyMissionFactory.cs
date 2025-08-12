using Game.Scripts.Providers.DailyMissions.Common;
using Game.Scripts.Providers.DailyMissions.Settings;

namespace Game.Scripts.Providers.DailyMissions.Factorys
{
	public interface IDailyMissionFactory
	{
		IMission GetInstance(MissionItem mission, string save);
		string RandomKey();
	}
}