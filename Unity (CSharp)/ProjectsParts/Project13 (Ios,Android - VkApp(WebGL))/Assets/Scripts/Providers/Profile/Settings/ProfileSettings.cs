using System.Collections.Generic;
using Game.Base;

namespace Game.Providers.Profile.Settings {
	[System.Serializable]
	public class ProfileSettings {
		public List<LevelReward> LevelRewards;
	}

	[System.Serializable]
	public struct LevelReward {
		public int Experience;
		public CalculablePlayerResources Reward;
	}
}
