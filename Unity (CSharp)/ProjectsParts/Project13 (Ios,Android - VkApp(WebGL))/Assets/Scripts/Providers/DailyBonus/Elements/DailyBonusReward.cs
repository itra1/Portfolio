using Game.Base;
using StringDrop;

namespace Game.Providers.DailyBonus.Elements {
	[System.Serializable]
	public class DailyBonusReward {
		[StringDropList(typeof(RewardTypes))]
		public string Reward;
		public float Value;

		public string ValueToString => Value.ToString().Replace(",", ".");
	}
}
