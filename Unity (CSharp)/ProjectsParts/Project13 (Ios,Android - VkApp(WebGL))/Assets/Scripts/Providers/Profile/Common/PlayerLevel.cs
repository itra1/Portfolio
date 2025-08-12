using Game.Providers.Profile.Settings;

namespace Game.Providers.Profile.Common
{
	public class PlayerLevel
	{
		private IProfileProvider _profileProvider;
		private LevelReward _levelRewards;
		private int _index;

		public int Index => _index;
		public int Experience => _levelRewards.Experience;
		public LevelReward LevelRewards => _levelRewards;
		public bool RewardReady => _profileProvider.CurrentLevel >= _index && !_profileProvider.IsReceivedReward(_index);
		public bool IsReceivedLevel => _profileProvider.CurrentLevel >= _index;

		public PlayerLevel(IProfileProvider profileProvider, int index, LevelReward levelRewards)
		{
			_profileProvider = profileProvider;
			_index = index;
			_levelRewards = levelRewards;
		}
	}
}
