using UnityEngine;

namespace Platforms.RateUs.Settings
{
	[System.Serializable]
	public class TDIosSettings
	{
		[SerializeField] private int _daysUntilPrompt = 1;
		[SerializeField] private int _usesUntilPrompt = 10;
		[SerializeField] private int _significantEventsUntilPrompt = -1;
		[SerializeField] private int _timeBeforeReminding = 2;
		[SerializeField] private bool _isDebug = true;

		public int DaysUntilPrompt => _daysUntilPrompt;
		public int UsesUntilPrompt => _usesUntilPrompt;
		public int SignificantEventsUntilPrompt => _significantEventsUntilPrompt;
		public int TimeBeforeReminding => _timeBeforeReminding;
		public bool IsDebug => _isDebug;
	}
}
