using System;
using Game.Providers.Saves.Data;

namespace Game.Providers.DailyBonus.Save
{
	public class DailyBonusSave : SaveProperty<DailyBonusSaveData>
	{
		public override DailyBonusSaveData DefaultValue => new();
	}

	public class DailyBonusSaveData
	{
		public DateTime LastDateGet = DateTime.MinValue;
		public int LastDayGet = -1;
	}
}
