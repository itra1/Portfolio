using UnityEngine;

namespace Game.Scripts.Providers.Profiles.Settings
{
	[System.Serializable]
	public class ProfileSettings : IProfileLevelSettings
	{
		[SerializeField] private int _starsPerLevel = 15;

		public int StarsPerLevel => _starsPerLevel;
	}
}
