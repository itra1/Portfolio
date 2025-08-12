using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Providers.DailyMissions.Settings
{
	[System.Serializable]
	public class DailyMissionColorSettings
	{
		[SerializeField] private List<DailyMissionColorItem> _colorItems;

		public List<DailyMissionColorItem> ColorItems => _colorItems;
	}
}
