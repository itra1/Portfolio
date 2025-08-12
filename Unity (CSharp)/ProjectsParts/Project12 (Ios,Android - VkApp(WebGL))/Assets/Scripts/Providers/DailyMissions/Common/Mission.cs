using Game.Scripts.Providers.DailyMissions.Base;
using Game.Scripts.Providers.DailyMissions.Save;
using Game.Scripts.Providers.DailyMissions.Settings;
using Game.Scripts.Providers.Saves;
using UnityEngine.Events;
using Zenject;

namespace Game.Scripts.Providers.DailyMissions.Common
{
	public abstract class Mission : IMission
	{
		public UnityEvent<MissionEventData> OnStateChange { get; set; } = new();

		protected IDailyMissionsSaveItem _saveData;
		protected IDailyMissionsProvider _dailyMissionProvider;
		protected ISaveHandler _saveHandler;

		protected MissionEventData _eventData;
		private MissionItem _mission;
		private DailyMissionColorItem _colorData;
		private DailyMissionIcone _iconeData;

		public string Title => _mission.Title;
		public string Description => _mission.Description;
		public string Uuid => _mission.Uuid;
		public int TargetCount => _mission.Count;
		public virtual int CurrentCount => _saveData.Count;
		public bool IsRewardReady => _mission.Count <= _saveData.Count;
		public bool IsRewarded => _saveData.Rewarded;
		public abstract string Type { get; }
		public DailyMissionColorItem ColorData => _colorData;
		public DailyMissionIcone IconeData => _iconeData;
		public MissionNotificationType NotificationType => _mission.NotificationType;

		protected Mission()
		{
			_eventData ??= new() { Mission = this };
		}

		~Mission()
		{

		}

		[Inject]
		private void Constructor(ISaveHandler saveHandler)
		{
			_saveHandler = saveHandler;
		}

		public virtual void Initialize()
		{
			if (_saveData == null)
			{
				MakeSaveData();
			}
		}

		public void SetMission(MissionItem mission) => _mission = mission;
		public void SetColor(DailyMissionColorItem colorData) => _colorData = colorData;
		public void SetIcone(DailyMissionIcone iconeData) => _iconeData = iconeData;
		public virtual void SetSaveData(string data) =>
			_saveData = Newtonsoft.Json.JsonConvert.DeserializeObject<DailyMissionsSaveItem>(data);
		protected void StateChangeEmit() => OnStateChange?.Invoke(_eventData);

		public string GetSaveData()
		{
			if (_saveData == null)
				MakeSaveData();
			return Newtonsoft.Json.JsonConvert.SerializeObject(_saveData);
		}

		protected virtual void MakeSaveData()
		{
			_saveData = new DailyMissionsSaveItem()
			{
				Count = 0,
				Type = Type,
				Rewarded = false
			};
		}

		public void AddItem(int count)
		{
			if (IsRewardReady)
				return;

			_saveData.Count += count;
			_eventData.EventType = MissionEventType.CountChange;

			if (IsRewardReady)
				_eventData.EventType = MissionEventType.RewardReady;

			StateChangeEmit();
		}

		public bool Reward()
		{
			_saveData.Rewarded = true;
			_eventData.EventType = MissionEventType.Rewarded;
			StateChangeEmit();
			return true;
		}
	}
}
