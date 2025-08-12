using Game.Providers.Profile.Common;
using Game.Providers.Profile.Signals;
using UnityEngine;
using Zenject;

namespace Game.Providers.Profile.Handlers
{
	public class LevelRewardsHandler
	{
		private SignalBus _signalBus;
		private IProfileProvider _profileProvider;

		public LevelRewardsHandler(SignalBus signalBus, IProfileProvider profileProvider)
		{
			_signalBus = signalBus;
			_profileProvider = profileProvider;
		}

		public void GetRewards(PlayerLevel playerLevel, RectTransform point)
		{
			_signalBus.Fire(new ResourceAddSignal(playerLevel.LevelRewards.Reward.Type, playerLevel.LevelRewards.Reward.Value, point));

			if (!_profileProvider.LevelsRewardsGet.Contains(playerLevel.Index))
			{
				_profileProvider.LevelsRewardsGet.Add(playerLevel.Index);
				_signalBus.Fire<ExperienceChangeSignal>();
			}
		}
	}
}
