using UnityEngine;

namespace Game.Scripts.Providers.DailyMissions.Settings
{
	[System.Serializable]
	public class MissionDay
	{
		[SerializeField] private MissionItem[] _missions;

		public MissionItem[] Missions => _missions;
	}
}
