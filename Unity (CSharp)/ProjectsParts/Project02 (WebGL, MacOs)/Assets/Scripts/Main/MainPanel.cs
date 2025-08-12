using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using it.UI;
using it.Main;
 
using it.Popups;

namespace it.UI
{

	public class MainPanel : it.UI.UIPanel
	{
		[SerializeField] private RectTransform _content;
		public event UnityEngine.Events.UnityAction<LobbyType> OnLobbyChange;

		private void OnEnable()
		{
			com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.UserLogin, UserLogin);
			com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.SessionSingleError, SessionSingleError);
			StartCoroutine(UpdateCoroutine());
		}

		private void OnDisable()
		{
			com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.UserLogin, UserLogin);
			com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.SessionSingleError, SessionSingleError);
			StopAllCoroutines();
		}

		private void SessionSingleError(com.ootii.Messages.IMessage handel)
		{
			//UserController.Instance.AnotherPlayerAuthorization();
			//#if UNITY_STANDALONE
			//			var infoPanel = it.Main.PopupController.Instance.ShowPopup<InfoPopup>(PopupType.Info);
			//			infoPanel.SetDescriptionString("errors.forms.sessionSingleErrors".Localized());
			//			infoPanel.OnConfirm = null;
			//#endif
		}

		IEnumerator UpdateCoroutine()
		{
			while (true)
			{
				yield return new WaitForSeconds(1);

				if (PlayerPrefs.HasKey(StringConstants.BUTTON_JACKPOT))
				{
					PlayerPrefs.DeleteKey(StringConstants.BUTTON_JACKPOT);
#if UNITY_STANDALONE
					StandaloneController.Instance.FocusWindow();
#endif
					it.Main.SinglePageController.Instance.Show(SinglePagesType.Jackpot);
					PlayerPrefs.Save();
				}
				if (PlayerPrefs.HasKey(StringConstants.BUTTON_GRID_TABLEORDER))
				{
					PlayerPrefs.DeleteKey(StringConstants.BUTTON_GRID_TABLEORDER);
#if UNITY_STANDALONE
					StandaloneController.Instance.SetGridTableOrder();
#endif
					PlayerPrefs.Save();
				}
				if (PlayerPrefs.HasKey(StringConstants.BUTTON_QUEUE_TABLEORDER))
				{
					PlayerPrefs.DeleteKey(StringConstants.BUTTON_QUEUE_TABLEORDER);
#if UNITY_STANDALONE
					StandaloneController.Instance.SetQueueTableOrder();
#endif
					PlayerPrefs.Save();
				}
				if (PlayerPrefs.HasKey(StringConstants.WINDOW_NEW))
				{
					string newWindow = PlayerPrefs.GetString(StringConstants.WINDOW_NEW);
					if (PlayerPrefs.HasKey(newWindow))
					{
						ulong newWindowId = ulong.Parse(PlayerPrefs.GetString(newWindow));

						PlayerPrefs.DeleteKey(newWindow);
						PlayerPrefs.DeleteKey(StringConstants.WINDOW_NEW);
						it.Logger.Log("WINDOW_NEW " + newWindow);
						it.Logger.Log("WINDOW_NEW_ID " + newWindowId);
#if UNITY_STANDALONE
						StandaloneController.Instance.FindTableWindow(newWindow, newWindowId);
#endif
						PlayerPrefs.Save();
					}
				}
				if (PlayerPrefs.HasKey(StringConstants.BUTTON_TABLE_THEME))
				{
					PlayerPrefs.DeleteKey(StringConstants.BUTTON_TABLE_THEME);
#if UNITY_STANDALONE
					StandaloneController.Instance.FocusWindow();
#endif
					SettingsPage sp = it.Main.SinglePageController.Instance.Show<SettingsPage>(SinglePagesType.Settings);
					sp.SelectPage(4);
					PlayerPrefs.Save();
				}
				if (PlayerPrefs.HasKey(StringConstants.BUTTON_SETTINGS))
				{
					PlayerPrefs.DeleteKey(StringConstants.BUTTON_SETTINGS);
#if UNITY_STANDALONE
					StandaloneController.Instance.FocusWindow();
#endif
					SettingsPage sp = it.Main.SinglePageController.Instance.Show<SettingsPage>(SinglePagesType.Settings);
					sp.SelectPage(0);
					PlayerPrefs.Save();
				}
				if (PlayerPrefs.HasKey(StringConstants.BUTTON_LEADERBOARD_MICRO))
				{
					PlayerPrefs.DeleteKey(StringConstants.BUTTON_LEADERBOARD_MICRO);
#if UNITY_STANDALONE
					StandaloneController.Instance.FocusWindow();
#endif
					it.Main.SinglePages.Leaderboard sp = it.Main.SinglePageController.Instance.Show<it.Main.SinglePages.Leaderboard>(SinglePagesType.Leaderboard);
					sp.VisiblePage(0);
					PlayerPrefs.Save();
				}
				if (PlayerPrefs.HasKey(StringConstants.BUTTON_LEADERBOARD_AVERAGE))
				{
					PlayerPrefs.DeleteKey(StringConstants.BUTTON_LEADERBOARD_AVERAGE);
#if UNITY_STANDALONE
					StandaloneController.Instance.FocusWindow();
#endif
					it.Main.SinglePages.Leaderboard sp = it.Main.SinglePageController.Instance.Show<it.Main.SinglePages.Leaderboard>(SinglePagesType.Leaderboard);
					sp.VisiblePage(1);
					PlayerPrefs.Save();
				}
				if (PlayerPrefs.HasKey(StringConstants.BUTTON_LEADERBOARD_HIGH))
				{
					PlayerPrefs.DeleteKey(StringConstants.BUTTON_LEADERBOARD_HIGH);
					it.Main.SinglePages.Leaderboard sp = it.Main.SinglePageController.Instance.Show<it.Main.SinglePages.Leaderboard>(SinglePagesType.Leaderboard);
					sp.VisiblePage(2);
#if UNITY_STANDALONE
					StandaloneController.Instance.FocusWindow();
#endif
					PlayerPrefs.Save();
				}
				if (PlayerPrefs.HasKey(StringConstants.BUTTON_BADBEAT))
				{
					ActionController.Instance.Emit("badBeat");
					PlayerPrefs.DeleteKey(StringConstants.BUTTON_BADBEAT);
					PlayerPrefs.Save();
				}
				if (PlayerPrefs.HasKey(StringConstants.TIMEBANK_UPDATE))
				{
					ActionController.Instance.Emit("timeBankUpdate");
					PlayerPrefs.DeleteKey(StringConstants.TIMEBANK_UPDATE);
					PlayerPrefs.Save();
				}
			}
		}

		private void UserLogin(com.ootii.Messages.IMessage handle)
		{
			if (!UserController.IsLogin)
				HomeButton();
		}


		public void HomeButton()
		{
			GetComponentInChildren<MainBody>().SetPage(MainPagesType.Home);
			GetComponentInChildren<TableNavigation>().SetPage(MainPagesType.Home, LobbyType.None);
			com.ootii.Messages.MessageDispatcher.SendMessage(EventsConstants.MainPageOpen);
		}

		//public void LobbyButton(NavigationButtonData btn)
		//{
		//	GetComponentInChildren<MainBody>(true).SetPage(btn.PageType);
		//	GetComponentInChildren<TableNavigation>().SetPage(btn);
		//	if(btn.PageType == MainPagesType.Lobby)
		//		GetComponentInChildren<LobbyPage>(true).SelectPage(lobby);
		//}
		public void LobbyButton(LobbyType lobby)
		{
			GetComponentInChildren<MainBody>(true).SetPage(MainPagesType.Lobby);
			GetComponentInChildren<TableNavigation>().SetPage(MainPagesType.Lobby, lobby);
			GetComponentInChildren<LobbyPage>(true).SelectPage(lobby);
			com.ootii.Messages.MessageDispatcher.SendMessage(EventsConstants.MainPageOpen);
		}

		public void SettingsButton()
		{
			//GetComponentInChildren<MainBody>().SetPage(MainPagesType.Settings);
			//GetComponentInChildren<TableNavigation>().SetPage(MainPagesType.Settings, LobbyType.None);
			//com.ootii.Messages.MessageDispatcher.SendMessage(EventsConstants.MainPageOpen);

			if (!UserController.IsLogin)
			{
				it.Main.PopupController.Instance.ShowPopup(PopupType.Authorization);
				return;
			}


			it.Main.SinglePageController.Instance.Show(SinglePagesType.Settings);

		}

		public void UserProfileButton()
		{
			if (!UserController.IsLogin)
			{
				it.Main.PopupController.Instance.ShowPopup(PopupType.Authorization);
				return;
			}
			GetComponentInChildren<MainBody>().SetPage(MainPagesType.UserProfile);
			GetComponentInChildren<TableNavigation>().SetPage(MainPagesType.UserProfile, LobbyType.None);
			com.ootii.Messages.MessageDispatcher.SendMessage(EventsConstants.MainPageOpen);
		}

	}
}
