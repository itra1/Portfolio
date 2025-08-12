using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.Providers.DailyMissions.Common;
using Game.Scripts.Providers.DailyMissions.Factorys;
using Game.Scripts.Providers.DailyMissions.Save;
using Game.Scripts.Providers.DailyMissions.Settings;
using Game.Scripts.Providers.Saves;
using UnityEngine.Events;

namespace Game.Scripts.Providers.DailyMissions
{
	public class DailyMissionProvider : IDailyMissionsProvider
	{
		public UnityEvent<MissionEventData> OnMissionChangeEvent { get; set; } = new();

		private readonly DailyMissionSettings _missionsSettings;
		private readonly DailyMissionColorSettings _missionsColorsSettings;
		private readonly DailyMissionIconeSettings _missionsIconsSettings;
		private readonly IDailyMissionFactory _missionsFactory;
		private readonly List<IMission> _activeMissions = new();
		private readonly ISaveProvider _saveProvider;
		private DailyMissionsSave _saveData;

		public List<IMission> ActiveMissions => _activeMissions;
		public DateTime DateComplete => _saveData.DayComplete;

		public DailyMissionProvider(ISaveProvider saveProvider, IDailyMissionFactory missionsFactory, DailyMissionSettings missionsSettings, DailyMissionColorSettings missionsColorsSettings, DailyMissionIconeSettings missionsIconsSettings)
		{
			_saveProvider = saveProvider;
			_missionsSettings = missionsSettings;
			_missionsColorsSettings = missionsColorsSettings;
			_missionsIconsSettings = missionsIconsSettings;
			_missionsFactory = missionsFactory;
		}

		public async UniTask StartAppLoad(IProgress<float> OnProgress, CancellationToken cancellationToken)
		{
			_saveData = _saveProvider.GetProperty<DailyMissionsSaveData>().Value;
			InitData();
			await UniTask.Yield();
		}

		private void InitData()
		{
			var nextDate = DateTime.Now.Date.AddDays(1);

			if (nextDate != _saveData.DayComplete)
			{
				CalcNewMissions();
			}
			else
			{
				LoadMissions();
			}
		}

		private void LoadMissions()
		{
			for (int i = 0; i < _missionsSettings.Days[_saveData.DayIndex].Missions.Length; i++)
			{
				var mission = _missionsSettings.Days[_saveData.DayIndex].Missions[i];
				AddMission(mission, _saveData.Mission[mission.Type]);
			}
			_saveData.SetMissions(_activeMissions);
		}

		private void CalcNewMissions()
		{
			_saveData.Mission = new();
			_saveData.DayIndex++;

			if (_saveData.DayIndex >= _missionsSettings.Days.Count)
				_saveData.DayIndex = 0;

			for (int i = 0; i < _missionsSettings.Days[_saveData.DayIndex].Missions.Length; i++)
				AddMission(_missionsSettings.Days[_saveData.DayIndex].Missions[i], null);

			_saveData.SetMissions(_activeMissions);
			_saveData.DayComplete = DateTime.Now.Date.AddDays(1);

			Save();
		}

		private void AddMission(MissionItem mission, string saveMission)
		{
			var missionInstance = _missionsFactory.GetInstance(mission, saveMission);
			_activeMissions.Add(missionInstance);
			missionInstance.SetColor(_missionsColorsSettings.ColorItems.Find(x => x.Type == mission.Color));
			missionInstance.SetIcone(_missionsIconsSettings.Icons.Find(x => x.Type == mission.Icone));
			missionInstance.OnStateChange.AddListener(MissionStateChange);
		}

		private void MissionStateChange(MissionEventData evantData)
		{
			OnMissionChangeEvent?.Invoke(evantData);
			if (evantData.NeedSave)
				Save();
		}

		public void Reward(IMission mission)
		{
			if (!mission.Reward())
				return;
			Save();
		}

		private void Save() => _ = _saveProvider.Save();

		public void AddItem(IMission mission)
		{
			mission.AddItem(1);
			Save();
		}
	}
}
