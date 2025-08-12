using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Providers.DailyMissions.Settings
{
	[System.Serializable]
	public class DailyMissionIconeSettings
	{
		[SerializeField] private List<DailyMissionIcone> _icons;

		public List<DailyMissionIcone> Icons => _icons;
	}
}
