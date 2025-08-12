using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Scripts.Providers.DailyMissions.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI.Components
{
	public class DailyMissionsNotification : MonoBehaviour
	{
		[SerializeField] private TMP_Text _titleLabel;
		[SerializeField] private TMP_Text _countLabel;
		[SerializeField] private Image _iconeImage;
		[SerializeField] private RectTransform _procressImage;
		[SerializeField] private RectTransform _body;
		[SerializeField] private GameObject _progressBlock;
		[SerializeField] private GameObject _completeLabel;
		[SerializeField] private float _progresMinWidth = 95;
		[SerializeField] private float _progresMaxWidth = 315;

		private IMission _mission;
		private VisibleStep _visibleStep = VisibleStep.Closed;
		private RectTransform _rectTransform;

		public bool IsVisible => (_visibleStep & VisibleStep.Visible) != 0;

		public IMission Mission => _mission;
		private DateTime _closeDateTime;

		public void SetMission(IMission mission)
		{
			_mission = mission;
			_rectTransform = GetComponent<RectTransform>();

			_titleLabel.text = _mission.Title;

			_iconeImage.sprite = _mission.IconeData.Icone;
			_iconeImage.color = _mission.ColorData.Color;

			UpdateVisible();
		}

		public void UpdateVisible()
		{
			SetProgress();

			if (IsVisible)
			{
				_closeDateTime = DateTime.Now.AddSeconds(1);
			}
			else
			{
				_ = VisibleAnimation();
			}
		}

		public void SetProgress()
		{
			if (_mission.IsRewardReady)
			{
				_progressBlock.SetActive(false);
				_completeLabel.SetActive(true);
				return;
			}

			_progressBlock.SetActive(true);
			_completeLabel.SetActive(false);

			_countLabel.text = $"{_mission.CurrentCount}/{_mission.TargetCount}";

			float progressAllLenght = _progresMaxWidth - _progresMinWidth;
			float progressTargetLenght = _progresMinWidth + (progressAllLenght * ((float) _mission.CurrentCount / _mission.TargetCount));
			_procressImage.sizeDelta = new Vector2(progressTargetLenght, _procressImage.sizeDelta.y);
			_procressImage.gameObject.SetActive(_mission.CurrentCount > 0);
		}

		private async UniTask VisibleAnimation()
		{
			_body.anchoredPosition = new Vector2(_body.rect.width, _body.anchoredPosition.y);

			gameObject.SetActive(true);

			_visibleStep = VisibleStep.Showing;

			await DOTween.To(() => _body.anchoredPosition, (x) => _body.anchoredPosition = x, new Vector2(0, _body.anchoredPosition.y), 0.2f).ToUniTask();

			_visibleStep = VisibleStep.Shown;
			_closeDateTime = DateTime.Now.AddSeconds(1);

			await UniTask.WaitUntil(() => DateTime.Now > _closeDateTime);

			_visibleStep = VisibleStep.Closing;

			await DOTween.To(() => _body.anchoredPosition, (x) => _body.anchoredPosition = x, new Vector2(_body.rect.width, _body.anchoredPosition.y), 0.2f).ToUniTask();

			_visibleStep = VisibleStep.Closed;

			gameObject.SetActive(false);
		}

		[System.Flags]
		private enum VisibleStep
		{
			Closed = 0,
			Showing = 1,
			Shown = 2,
			Closing = 4,
			Visible = Showing | Shown
		}
	}
}
