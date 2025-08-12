using Game.Scripts.Providers.DailyMissions.Base;
using Game.Scripts.Providers.DailyMissions.Settings;
using UnityEngine.Events;

namespace Game.Scripts.Providers.DailyMissions.Common
{
	public interface IMission
	{
		UnityEvent<MissionEventData> OnStateChange { get; set; }
		string Title { get; }
		string Description { get; }
		string Uuid { get; }
		int CurrentCount { get; }
		int TargetCount { get; }
		bool IsRewardReady { get; }
		bool IsRewarded { get; }
		DailyMissionColorItem ColorData { get; }
		DailyMissionIcone IconeData { get; }
		MissionNotificationType NotificationType { get; }
		string Type { get; }

		void SetMission(MissionItem mission);
		void SetColor(DailyMissionColorItem colorData);
		void SetIcone(DailyMissionIcone iconeData);

		void SetSaveData(string data);
		string GetSaveData();

		void Initialize();
		bool Reward();
		void AddItem(int count);
	}
}