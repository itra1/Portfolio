using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Providers.DailyMissions.Settings
{
	[System.Serializable]
	public class DailyMissionSettings
	{
		[SerializeField] private List<MissionDay> _days;

		public List<MissionDay> Days => _days;
	}
}
