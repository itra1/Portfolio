using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Scripts.Providers.DailyMissions.Common;
using Game.Scripts.UI.Components;
using Game.Scripts.UI.Presenters.Base;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.Scripts.UI.Presenters
{
	public class DailyMissionsPresenter : WindowPresenter
	{
		[HideInInspector] public UnityEvent OnBackEvent = new();
		[HideInInspector] public UnityEvent<IMission> OnRewardEvent = new();

		[SerializeField] private RectTransform _content;
		[SerializeField] private DailyMissionsRecord[] _recordsArray;
		[SerializeField] private Button _backButton;
		private List<IMission> _missions;
		[SerializeField] private MissionIndicator[] _indicators;

		public override async UniTask<bool> Initialize()
		{
			if (!await base.Initialize())
				return false;

			_backButton.onClick.AddListener(BackButtonTouch);

			return true;
		}

		private void BackButtonTouch()
		{
			OnBackEvent?.Invoke();
		}

		public void SetMissions(List<IMission> missions)
		{
			_missions = missions;
			FillData();
		}

		public void FillData()
		{
			for (int i = 0; i < _recordsArray.Length; i++)
			{
				var mission = _missions[i];
				_recordsArray[i].SetMission(mission, () =>
				{
					OnRewardEvent?.Invoke(mission);
				});
			}
			FillIndicators(_missions);
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

		[System.Serializable]
		public struct MissionIndicator
		{
			public GameObject DefaultIcone;
			public GameObject CheckIcone;
			public GameObject ReadyIcone;
		}
	}
}
