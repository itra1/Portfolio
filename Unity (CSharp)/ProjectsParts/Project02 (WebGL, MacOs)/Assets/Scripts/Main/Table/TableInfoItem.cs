using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;
using it.Network.Rest;
using Sett = it.Settings;
using DG.Tweening;
 

namespace it.UI
{
	public class TableInfoItem : MonoBehaviour
	{
		public UnityEngine.Events.UnityAction<bool> OnTimeBankHover;
		public UnityEngine.Events.UnityAction<bool> OnExtraTimeHover;
		public UnityEngine.Events.UnityAction<bool> OnAnteHover;

		[SerializeField] private string _gameName;
		[SerializeField] private string _tableMax = "8";
		[SerializeField] private string _blinds;
		[SerializeField] private string _timeToAct;
		[SerializeField] private string _timeBank;
		[SerializeField] private string _disconnectExtraTime;
		[SerializeField] private string _minimumPlayers = "2";
		[SerializeField] private string _linkKey;

		[SerializeField] private TextMeshProUGUI _gameNameLabel;
		[SerializeField] private TextMeshProUGUI _tableMaxLabel;
		[SerializeField] private TextMeshProUGUI _blindsLabel;
		[SerializeField] private TextMeshProUGUI _timeToActLabel;
		[SerializeField] private TextMeshProUGUI _timeBankLabel;
		[SerializeField] private TextMeshProUGUI _disconnectExtraTimeLabel;
		[SerializeField] private TextMeshProUGUI _minimumPlayersLabel;
		[SerializeField] private TextMeshProUGUI _moveDetailsLabel;

		public List<GameType> GameTypes;
		public bool IsAllOrNofing;
		public bool IsDealerChoise;

		private void OnEnable()
		{
			com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.LocalizationChange, Localization);
			FillData();
		}

		private void OnDisable()
		{
			com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.LocalizationChange, Localization);
		}

		private void Localization(com.ootii.Messages.IMessage handle)
		{
			FillData();
		}

		private void FillData()
		{
			if (_timeBankLabel != null)
				_timeBankLabel.text = string.IsNullOrEmpty(_timeBank) ? "lobby.info.off".Localized() : string.Format("lobby.info.countSeconds".Localized(), _timeBank);
			if (_disconnectExtraTimeLabel != null)
				_disconnectExtraTimeLabel.text = string.IsNullOrEmpty(_disconnectExtraTime) ? "lobby.info.off".Localized() : string.Format("lobby.info.countSeconds".Localized(), _disconnectExtraTime);
			if (_gameNameLabel != null)
				_gameNameLabel.text = _gameName;
			if (_tableMaxLabel != null)
				_tableMaxLabel.text = string.Format("lobby.info.countPlayers".Localized(), _tableMax);
			if (_blindsLabel != null)
				_blindsLabel.text = _blinds;
			if (_timeToActLabel != null)
				_timeToActLabel.text = string.Format("lobby.info.countSeconds".Localized(), _timeToAct);
			if (_minimumPlayersLabel != null)
				_minimumPlayersLabel.text = string.Format("lobby.info.countPlayers".Localized(), _minimumPlayers);
			if (_moveDetailsLabel != null)
				_moveDetailsLabel.text = string.Format("<link=\"openUrl\">" + "lobby.info.moveAbout".Localized() + "</link>", _gameName);
		}
		public void TimeBankButtonTouch()
		{
			it.Main.PopupController.Instance.ShowPopup(PopupType.TimeBankInfo);
		}

		public void ExtraTimeButtonTouch()
		{
			it.Main.PopupController.Instance.ShowPopup(PopupType.DisconnectExtraTime);
		}
		public void AnteButtonTouch()
		{
			it.Main.PopupController.Instance.ShowPopup(PopupType.ButtonAnte);
		}

		public void TimeBankHover(bool isHover)
		{
#if UNITY_STANDALONE
			GetComponentInParent<it.Main.LobbyPage>().TimeBankPanel.SetActive(isHover);
#endif
			//OnTimeBankHover?.Invoke(isHover);
		}
		public void ExtraTimeHover(bool isHover)
		{
#if UNITY_STANDALONE
			GetComponentInParent<it.Main.LobbyPage>().ExtraTimePanel.SetActive(isHover);
#endif
			//OnExtraTimeHover?.Invoke(isHover);
		}
		public void AnteHover(bool isHover)
		{
#if UNITY_STANDALONE
			GetComponentInParent<it.Main.LobbyPage>().AntePanel.SetActive(isHover);
#endif
			//OnAnteHover?.Invoke(isHover);
		}

		public void OpenLink()
		{
			Garilla.LinkManager.OpenUrl(_linkKey);
		}

	}
}