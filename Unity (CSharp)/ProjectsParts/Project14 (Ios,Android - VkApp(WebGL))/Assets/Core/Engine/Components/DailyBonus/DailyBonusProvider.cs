using Core.Engine.Components.SaveGame;

namespace Core.Engine.Components.DailyBonus
{
	public class DailyBonusProvider : IDailyBonusProvider
	{
		private readonly DailyBonusSave _dailyBonusSave;
		public DailyBonusProvider(SaveGameProvider saveGame)
		{
			_dailyBonusSave = (DailyBonusSave)saveGame.GetProperty<DailyBonusSave>();
		}

		public int DaysGet => _dailyBonusSave.Value.LastDay;

		public bool ReadyNewGet => (System.DateTime.Now.Date - _dailyBonusSave.Value.LastGet).TotalHours >= 24;

		public void AddDay(){
			if (!ReadyNewGet) return;
			_dailyBonusSave.Value.LastDay++;
			_dailyBonusSave.Value.LastGet = System.DateTime.Now.Date;
			_dailyBonusSave.Save();
		}

	}
}