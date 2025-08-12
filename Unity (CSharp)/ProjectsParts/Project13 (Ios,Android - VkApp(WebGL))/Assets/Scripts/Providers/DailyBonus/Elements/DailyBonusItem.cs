using static Game.Providers.DailyBonus.Settings.DailyBonusSettings;

namespace Game.Providers.DailyBonus.Elements {
	[System.Serializable]
	public class DailyBonusItem :IDailyBonusItem {
		private DailyBonusItemSettings _setting;
		private DailyBonusProvider _provider;
		private int _index;
		public string Uuid => _setting.Uuid;
		public bool IsGetReady => _provider.NextReady && _provider.NextIndex == _index;
		public bool IsGet => _provider.NextIndex > _index;
		public DailyBonusItemSettings Setting { get => _setting; set => _setting = value; }
		public int Index { get => _index; set => _index = value; }

		public DailyBonusItem(int index, DailyBonusProvider provider, DailyBonusItemSettings setting) {
			_provider = provider;
			_setting = setting;
			_index = index;
		}
	}
}
