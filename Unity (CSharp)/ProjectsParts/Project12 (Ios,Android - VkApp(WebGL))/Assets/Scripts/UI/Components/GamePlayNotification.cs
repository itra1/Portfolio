using System.Collections.Generic;
using Engine.Scripts.Base;
using Game.Scripts.Providers.DailyMissions;
using Game.Scripts.Providers.DailyMissions.Common;
using Game.Scripts.UI.Presenters.Interfaces;
using UnityEngine;
using Zenject;

namespace Game.Scripts.UI.Components
{
	public class GamePlayNotification : MonoBehaviour, IInjection, IUiVisibleHandler
	{
		[SerializeField] private DailyMissionsNotification _notifictionPrefab;

		private IDailyMissionsProvider _dailyMissionProvider;
		private List<DailyMissionsNotification> _notificationsList = new();

		[Inject]
		private void Constructor(IDailyMissionsProvider dailyMissionProvider)
		{
			_dailyMissionProvider = dailyMissionProvider;
			_notifictionPrefab.gameObject.SetActive(false);
		}

		public void Show()
		{
			_dailyMissionProvider.OnMissionChangeEvent.AddListener(OnMissionChange);

			_notificationsList.ForEach(x => x.gameObject.SetActive(false));
		}

		public void Hide()
		{
			_dailyMissionProvider.OnMissionChangeEvent.RemoveListener(OnMissionChange);
		}

		private void OnMissionChange(MissionEventData eventData)
		{
			if (eventData.EventType is MissionEventType.Clear)
				return;

			if (eventData.Mission.NotificationType == Providers.DailyMissions.Base.MissionNotificationType.None)
				return;
			if (eventData.Mission.NotificationType == Providers.DailyMissions.Base.MissionNotificationType.Complete
			&& (!eventData.Mission.IsRewardReady || eventData.Mission.IsRewarded))
				return;

			var existsItemMission = _notificationsList.Find(x => x.IsVisible && x.Mission.Uuid == eventData.Mission.Uuid);

			if (existsItemMission != null)
			{
				existsItemMission.UpdateVisible();
				return;
			}

			existsItemMission = _notificationsList.Find(x => !x.isActiveAndEnabled);

			if (existsItemMission == null)
			{
				existsItemMission = Instantiate(_notifictionPrefab, _notifictionPrefab.transform.parent);
				_notificationsList.Add(existsItemMission);
			}

			existsItemMission.SetMission(eventData.Mission);
			existsItemMission.transform.SetAsLastSibling();
		}
	}
}
