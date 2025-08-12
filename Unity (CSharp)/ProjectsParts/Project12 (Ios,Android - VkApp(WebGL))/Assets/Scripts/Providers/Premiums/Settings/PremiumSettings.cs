using UnityEngine;

namespace Game.Scripts.Providers.Premiums.Settings
{
	[System.Serializable]
	public class PremiumSettings
	{
		[SerializeField] private int _monthRewardPeriod;
		[SerializeField] private Reward _monthReward;

		public int MonthRewardPeriod => _monthRewardPeriod;

		public Reward MonthReward => _monthReward;
	}
}
