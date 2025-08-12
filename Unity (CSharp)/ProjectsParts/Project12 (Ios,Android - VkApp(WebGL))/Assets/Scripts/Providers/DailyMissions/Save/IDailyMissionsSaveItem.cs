namespace Game.Scripts.Providers.DailyMissions.Save
{
	public interface IDailyMissionsSaveItem
	{
		string Type { get; set; }
		int Count { get; set; }
		bool Rewarded { get; set; }
	}
}