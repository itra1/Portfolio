using System;
using System.Collections.Generic;
using Game.Scripts.Providers.DailyMissions.Common;
using Game.Scripts.Providers.Saves.Data;

namespace Game.Scripts.Providers.DailyMissions.Save
{
	public class DailyMissionsSaveData : SaveProperty<DailyMissionsSave>
	{
		public override DailyMissionsSave DefaultValue => new();

		protected override void PreSave()
		{
			base.PreSave();
			Value.ReadSaves();
		}
	}
	[System.Serializable]
	public class DailyMissionsSave
	{
		public DateTime DayComplete;
		public int DayIndex = -1;
		public Dictionary<string, string> Mission = new();

		private List<IMission> _missions = new();

		public void SetMissions(List<IMission> missions)
		{
			_missions = missions;
		}

		public void ReadSaves()
		{
			Mission.Clear();

			foreach (var item in _missions)
			{
				Mission.Add(item.Type, item.GetSaveData());
			}
		}
	}
}
