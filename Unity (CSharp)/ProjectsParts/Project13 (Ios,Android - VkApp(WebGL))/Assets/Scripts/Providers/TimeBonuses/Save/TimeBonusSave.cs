using System;
using System.Collections.Generic;
using Game.Providers.Saves.Data;

namespace Game.Providers.TimeBonuses.Save
{
	public class TimeBonusSave : SaveProperty<TimeBonusData>
	{
		public override TimeBonusData DefaultValue => new();
	}
	public class TimeBonusData
	{
		public List<TimeBonusSaveItem> Items = new();
	}

	public class TimeBonusSaveItem
	{
		public string Uuid;
		public DateTime LastGet;
	}

}
