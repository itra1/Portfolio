using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using System;

namespace Garilla.Games.UI
{
	/// <summary>
	/// Панель динамита
	/// </summary>
	public class DynamitePanel : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI _timerLabel;
		[SerializeField] private it.UI.Elements.ButtonUI _foldButton;
		[SerializeField] private it.UI.Elements.ButtonUI _allInButton;
		[SerializeField] private RectTransform _redBackTimer;

		public GameUIManager GameManager { get => _gameManager; set => _gameManager = value; }

		private GameUIManager _gameManager;
		private bool _isTimer;

		private System.DateTime _targetTime;

		private void OnEnable()
		{
		}

		public void FoldClickbutton()
		{
			SetStateButtons(false);
			GameHelper.FoldActiveCards(_gameManager.SelectTable, (isSuccess) =>
			{
				if (!isSuccess) SetStateButtons(true);
			});
		}

		public void AllInClickButton()
		{
			SetStateButtons(false);
			GameHelper.AllinActiveDistributionBet(_gameManager.SelectTable, (isSuccess) =>
			{
				if (!isSuccess) SetStateButtons(true);
			});
		}

		public void UpdateDistribute()
		{
			var distrib = _gameManager.SharedData.distribution;
			var activeEvent = distrib.active_event;

			it.Logger.Log("[Dynamite] update distrib");
			it.Logger.Log($"[Dynamite] user id {UserController.User.id}");

			if (activeEvent.user_id != UserController.User.id) return;

			for (int p = 0; p < _gameManager.SharedData.distribution.players.Count; p++)
			{
				if (distrib.players[p].user.id == UserController.User.id)
				{
					SetStateButtons(true);
					var player = _gameManager.SharedData.distribution.players[p];

					var endTime = DateTime.Parse(activeEvent.calltime_at);
					var diff = (endTime - GameHelper.NowTime).TotalSeconds;
					if (diff > 0) StartTimer(endTime);
				}
			}
		}

		private void StartTimer(DateTime dateTime)
		{
			_targetTime = dateTime;
			_isTimer = true;
			_redBackTimer.anchoredPosition = new Vector2(-_redBackTimer.rect.width, 0);
			_redBackTimer.DOAnchorPos(new Vector2(_redBackTimer.rect.width, 0), 1f);
		}

		public void SetStateButtons(bool isActiveState)
		{
			_foldButton.interactable = isActiveState;
			_allInButton.interactable = isActiveState;
		}

		private void Update()
		{
			if (!_isTimer) return;
			var dt = (_targetTime - GameHelper.NowTime);
			_timerLabel.text = $"{dt.Seconds.ToString("00")}:{(dt.Milliseconds / 10).ToString("00")}";

			if (dt.Milliseconds < 0)
			{
				_isTimer = false;
				_timerLabel.text = "00:00";
			}

		}

	}
}