using Core.Engine.Components.SaveGame;

namespace Core.Engine.Components.DailyBonus
{
	/// <summary>
	/// Дневной бонус
	/// </summary>
	public class DailyBonusSave :SaveProperty<DailyBonusSaveItem>
	{
		public override DailyBonusSaveItem DefaultValue =>
		new() { LastDay = 0,LastGet = System.DateTime.Now.Date.AddDays(-1) };
	}

	public class DailyBonusSaveItem
	{
		public System.DateTime LastGet;
		public int LastDay;
	}
}
