using System;
using System.Collections.Generic;
using Engine.Scripts.Base;
using Game.Scripts.Providers.DailyMissions;
using Game.Scripts.Providers.DailyMissions.Common;
using Game.Scripts.Providers.Timers;
using Game.Scripts.Providers.Timers.Base;
using Game.Scripts.Providers.Timers.Common;
using Game.Scripts.Providers.Timers.Helpers;
using Game.Scripts.Providers.Timers.Ui;
using Game.Scripts.UI.Controllers;
using Game.Scripts.UI.Presenters.Interfaces;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Scripts.UI.Components
{
	public class DailyMissionHomePanel : MonoBehaviour, IInjection, IUiVisibleHandler
	{
		[SerializeField] private Button _selfButton;
		[SerializeField] private MissionIndicator[] _indicators;
		[SerializeField] private TimerPresenterBlock _timerBlock;
		private IDailyMissionsProvider _dailyMissionProvider;
		private IUiNavigator _uiHelpers;
		private ITimersProvider _timerProvider;
		private ITimer _timer;

		[Inject]
		private void Constructor(IDailyMissionsProvider dailyMissionProvider, IUiNavigator uiHelpers, ITimersProvider timerProvider)
		{
			_dailyMissionProvider = dailyMissionProvider;
			_uiHelpers = uiHelpers;
			_timerProvider = timerProvider;

			_selfButton.onClick.RemoveAllListeners();
			_selfButton.onClick.AddListener(SelfClick);
		}

		private void SelfClick()
		{
			_ = _uiHelpers.GetController<DailyMissionsPresenterController>().Open();
		}

		public void Show()
		{
			FillIndicators(_dailyMissionProvider.ActiveMissions);

			var dateDelay = _dailyMissionProvider.DateComplete - DateTime.Now;
			_timer = _timerProvider.Create(TimerType.RealtimeDesc)
			.AutoRemove(true)
			.End(dateDelay.TotalSeconds)
			.OnTick(TimerTickEvent)
			.Start(dateDelay.TotalSeconds);
			TimerTickEvent(dateDelay.TotalSeconds);

			_dailyMissionProvider.OnMissionChangeEvent.AddListener(OnMissionChangeEventListener);
		}

		public void Hide()
		{
			_timer = null;

			_dailyMissionProvider.OnMissionChangeEvent.RemoveListener(OnMissionChangeEventListener);
		}

		private void OnMissionChangeEventListener(MissionEventData _)
		{
			FillIndicators(_dailyMissionProvider.ActiveMissions);
		}

		private void FillIndicators(List<IMission> missions)
		{
			for (int i = 0; i < missions.Count; i++)
			{
				var mission = missions[i];
				_indicators[i].DefaultIcone.SetActive(!mission.IsRewarded);
				_indicators[i].CheckIcone.SetActive(mission.IsRewarded);
				_indicators[i].ReadyIcone.SetActive(mission.IsRewardReady && !mission.IsRewarded);
			}
		}

		private void TimerTickEvent(double seconds)
		{
			TimerBlockVisible.VisibleBlock(_timerBlock, seconds);
		}

		[System.Serializable]
		public struct MissionIndicator
		{
			public GameObject DefaultIcone;
			public GameObject CheckIcone;
			public GameObject ReadyIcone;
		}
	}
}
