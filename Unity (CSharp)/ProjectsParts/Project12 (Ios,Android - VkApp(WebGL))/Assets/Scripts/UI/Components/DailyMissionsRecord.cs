using Engine.Scripts.Base;
using Game.Scripts.Controllers;
using Game.Scripts.Providers.DailyMissions;
using Game.Scripts.Providers.DailyMissions.Common;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zenject;

namespace Game.Scripts.UI.Components
{
	public class DailyMissionsRecord : MonoBehaviour, IInjection
	{
		[SerializeField] private TMP_Text _titleLabel;
		[SerializeField] private TMP_Text _descriptionLabel;
		[SerializeField] private TMP_Text _countLabel;
		[SerializeField] private TMP_Text _collectButtonLabel;
		[SerializeField] private TMP_Text _collectedLabel;
		[SerializeField] private Image _iconeBackImage;
		[SerializeField] private Image _iconeImage;
		[SerializeField] private Button _collectButton;
		[SerializeField] private RectTransform _procressImage;
		[SerializeField] private float _progresMinWidth = 95;
		[SerializeField] private float _progresMaxWidth = 315;
		[SerializeField] private Button _debAddButton;

		private IMission _mission;
		private IApplicationSettingsController _applicationSettingsController;
		private IDailyMissionsProvider _dailyMissionProvider;

		[Inject]
		private void Constructor(
			IApplicationSettingsController applicationSettingsController,
			IDailyMissionsProvider dailyMissionProvider
		)
		{
			_applicationSettingsController = applicationSettingsController;
			_dailyMissionProvider = dailyMissionProvider;

			_debAddButton.onClick.RemoveAllListeners();
			_debAddButton.gameObject.SetActive(false);
			if (_applicationSettingsController.ApplicationSettings.DevMode)
			{
				_debAddButton.gameObject.SetActive(true);
				_debAddButton.onClick.AddListener(() => _dailyMissionProvider.AddItem(_mission));
			}
		}

		public void SetMission(IMission mission, UnityAction OnCollectEvent)
		{
			_mission = mission;
			_titleLabel.text = _mission.Title;
			_descriptionLabel.text = _mission.Description;
			_countLabel.text = $"{(Mathf.Min(_mission.CurrentCount, _mission.TargetCount))}/{_mission.TargetCount}";
			_collectButton.interactable = _mission.IsRewardReady && !_mission.IsRewarded;
			_iconeBackImage.sprite = _mission.ColorData.IconeBack;
			_iconeImage.sprite = _mission.IconeData.Icone;
			_iconeImage.color = _mission.ColorData.Color;
			SetProgress();

			_collectButton.gameObject.SetActive(!_mission.IsRewarded);
			_collectedLabel.gameObject.SetActive(_mission.IsRewarded);

			_collectButton.onClick.RemoveAllListeners();
			_collectButton.onClick.AddListener(() => OnCollectEvent?.Invoke());
		}

		private void SetProgress()
		{
			float progressAllLenght = _progresMaxWidth - _progresMinWidth;
			float progressTargetLenght = _progresMinWidth + (progressAllLenght * ((float) (Mathf.Min(_mission.CurrentCount, _mission.TargetCount)) / _mission.TargetCount));
			_procressImage.sizeDelta = new Vector2(progressTargetLenght, _procressImage.sizeDelta.y);
			_procressImage.gameObject.SetActive(_mission.CurrentCount > 0);
		}
	}
}
