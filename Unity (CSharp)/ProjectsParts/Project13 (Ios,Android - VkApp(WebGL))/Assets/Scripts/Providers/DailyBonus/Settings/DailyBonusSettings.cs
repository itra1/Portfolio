using Game.Providers.DailyBonus.Elements;
using System.Collections.Generic;
using Uuid;

namespace Game.Providers.DailyBonus.Settings {
	[System.Serializable]
	public class DailyBonusSettings {
		public List<DailyBonusItemSettings> BonusList;

		[System.Serializable]
		public struct DailyBonusItemSettings {
			[UUID] public string Uuid;
			public DailyBonusReward[] Rewards;
		}
	}
}
