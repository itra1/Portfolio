using UnityEngine;

namespace Game.Providers.Battles.Settings
{
	[System.Serializable]
	public class BattleSettings
	{
		[SerializeField] private string _settingsPath;

		public string SettingsPath => _settingsPath;
	}
}
