using System;
using System.Collections.Generic;
using Game.Scripts.Providers.Saves.Data;

namespace Game.Scripts.Providers.DailyBonuses.Save
{
	public class DailyBonusSaveData : SaveProperty<DailyBonusSave>
	{
		public override DailyBonusSave DefaultValue => new();
	}

	public class DailyBonusSave
	{
		public List<DailyBonusItemSave> SaveItems = new();
	}

	public class DailyBonusItemSave
	{
		public string Type;
		public DateTime TimeComplete;
	}
}
