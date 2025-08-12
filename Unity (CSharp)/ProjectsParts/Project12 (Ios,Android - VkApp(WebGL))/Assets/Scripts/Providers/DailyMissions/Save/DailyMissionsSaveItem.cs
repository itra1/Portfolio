namespace Game.Scripts.Providers.DailyMissions.Save
{
	[System.Serializable]
	public class DailyMissionsSaveItem : IDailyMissionsSaveItem
	{
		public string Type { get; set; }
		public int Count { get; set; }
		public bool Rewarded { get; set; }
	}
}