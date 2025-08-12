using StringDrop;

namespace Game.Scripts.Providers.DailyMissions.Common
{
	public class MissionEventData
	{
		public IMission Mission { get; set; }
		[StringDropList(typeof(MissionEventType), false)] public string EventType { get; set; }
		public bool NeedSave { get; set; }
	}

	public struct MissionEventType
	{
		public const string CountChange = "CountChange";
		public const string RewardReady = "RewardReady";
		public const string Rewarded = "Rewarded";
		public const string Clear = "Clear";
	}
}
