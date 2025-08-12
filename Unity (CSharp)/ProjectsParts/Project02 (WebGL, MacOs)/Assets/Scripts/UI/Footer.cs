using it.Main;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace it.UI
{
	public class Footer : MonoBehaviour
	{
		[SerializeField] private RectTransform _logoutRect;
		[SerializeField] private RectTransform _buttonsRect;

		private Vector2 _startPosition;
		private void OnEnable()
		{
			_startPosition = _buttonsRect.anchoredPosition;
			com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.UserLogin, OnAuthorization);
			ConfirmPanel();
		}

		private void OnAuthorization(com.ootii.Messages.IMessage handler)
		{
			ConfirmPanel();
		}

		private void ConfirmPanel()
		{
			_logoutRect.gameObject.SetActive(UserController.User != null);
			if (UserController.User != null)
				_buttonsRect.anchoredPosition = _startPosition + Vector2.left * _logoutRect.rect.width;
			else
				_buttonsRect.anchoredPosition = _startPosition;
		}
		public void LogoutButton()
		{
			UserController.Instance.LogoutDialog();
		}
		public void StatisticButton()
		{
			SinglePageController.Instance.Show(SinglePagesType.PokerStatistic);
		}

		public void SettingsButton()
		{
			UiManager.GetPanel<MainPanel>().SettingsButton();
		}

		public void ITechLabsTouch()
		{
			Garilla.LinkManager.OpenUrl("iTechLabs");
		}
		public void BeGambleAwareTouch()
		{
			Garilla.LinkManager.OpenUrl("beGambleAware");
		}
		public void GameCareTouch()
		{
			Garilla.LinkManager.OpenUrl("gamCare");
		}
		public void GamingCurocaoTouch()
		{
			Garilla.LinkManager.OpenUrl("gamingCurocao");
		}

	}
}