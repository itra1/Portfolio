using UnityEngine;
using it.UI;
using it.Network.Rest;
using System.Linq;
using System.Collections.Generic;
using DG.Tweening;
using JetBrains.Annotations;

namespace it.Mobile
{
	public class OpenTableManager : Singleton<OpenTableManager>
	{
		[SerializeField] private MobileAppNavigation _baseNavigations;
		[SerializeField] private MobileTablesUIManager _uiManager;
		[SerializeField] private Garilla.Games.MobileGameTopPanel _mobileTopPanel;
		[SerializeField] private GamePanel _gamePanelPrefab;
		[SerializeField] private CanvasGroup _gamePanelCg;
		[SerializeField] private RectTransform _gamePanelsParent;
		[SerializeField] private CanvasGroup _gamePanelsParentCg;

		public Dictionary<ulong, GamePanel> OpenPanels { get => _openPanels; set => _openPanels = value; }
		public RectTransform GameRt { get => _gameRt; set => _gameRt = value; }

		private Dictionary<ulong, GamePanel> _openPanels = new Dictionary<ulong, GamePanel>();
		private GamePanel _activeGamePanel;
		private RectTransform _gameRt;

		private void Awake()
		{
			_gamePanelCg.alpha = 0;
			_gamePanelCg.blocksRaycasts = false;
			_gameRt = _gamePanelCg.GetComponent<RectTransform>();
			_mobileTopPanel.OnCloseButton = CloseButtonTouch;
			_mobileTopPanel.OnHomeButton = HomeButtonTouch;
			_mobileTopPanel.OnObserveButton = ObserversButtonTouch;
			_mobileTopPanel.OnSettingsButton = SettingsButtonTouch;
			_mobileTopPanel.OnAddButton = AddTableButtonTouch;
			_mobileTopPanel.OnTableClick = SetActiveGamePanel;
			_mobileTopPanel.OnThemeButton = ThemeButtonTouch;
			_mobileTopPanel.OnWCLButton = WclButtonTouch;
		}
		private void Start()
		{
			com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.UserLogin, (h) =>
			{
				foreach (var key in OpenPanels.Keys)
					OpenPanels[key].Close();
			});
		}

		private void OnEnable()
		{
			_gamePanelPrefab.gameObject.SetActive(true);
		}

		public void VisibleGamePanels()
		{
			_mobileTopPanel.VisibleTables = true;

			_gamePanelsParentCg.blocksRaycasts = true;
			DOTween.To(() => _gamePanelsParentCg.alpha, (x) => _gamePanelsParentCg.alpha = x, 1, 0.3f).OnComplete(() =>
			{
				MainMenuActivate(false);
			});
		}

		public void HideGamePanels()
		{
			MainMenuActivate(true);
			_mobileTopPanel.VisibleTables = false;
			_gamePanelsParentCg.blocksRaycasts = false;
			DOTween.To(() => _gamePanelsParentCg.alpha, (x) => _gamePanelsParentCg.alpha = x, 0, 0.3f);
		}

		public void OpenGame(Table table)
		{

			if (UserController.User == null)
			{
				it.Main.PopupController.Instance.ShowPopup(PopupType.Authorization);
				return;
			}

			if (_openPanels.ContainsKey((ulong)table.id))
			{
				SetActiveGamePanel(_openPanels[(ulong)table.id]);
				return;
			}
			if (_openPanels.Count == 0)
			{
				_gamePanelCg.blocksRaycasts = true;
				DOTween.To(() => _gamePanelCg.alpha, (x) => _gamePanelCg.alpha = x, 1, 0.3f);
			}
			SocketClient.Instance.EnterTableChanel(table.id);

			GameObject instTable = Instantiate(_gamePanelPrefab.gameObject, _gamePanelsParent);
			GamePanel newGamePanel = instTable.GetComponent<GamePanel>();
			_openPanels.Add((ulong)table.id, newGamePanel);
			SetActiveGamePanel(newGamePanel);

			instTable.transform.localPosition = Vector3.zero;
			instTable.transform.localRotation = Quaternion.identity;
			instTable.transform.localScale = Vector3.one;

			instTable.gameObject.SetActive(true);
			newGamePanel.Init(table);
			_mobileTopPanel.AddTable(newGamePanel);
			newGamePanel.OnClose.AddListener(() =>
			{
				_openPanels.Remove(newGamePanel.Table.id);
				_mobileTopPanel.RemoveTable(newGamePanel);
				if (_openPanels.Count == 0)
				{
					_gamePanelCg.blocksRaycasts = false;
					MainMenuActivate(true);

					var go = MobileTablesUIManager.Instance.SelectPage("Main");
					var goc = go.GetComponent<it.Mobile.Main.LobbyPage>();

					if (table.is_vip)
					{
						goc.SelectPage(LobbyType.VipGame);
					}
					else if (table.is_dealer_choice)
					{
						goc.SelectPage(LobbyType.DealerChoice);
					}
					else
						goc.OpenLobbyByTable(table);
					DOTween.To(() => _gamePanelCg.alpha, (x) => _gamePanelCg.alpha = x, 0, 0.3f);
				}
				else
				{
					SetActiveGamePanel(_openPanels.Last().Value);
				}
			});
		}

		public void MainMenuActivate(bool isActive)
		{

			_uiManager.ContentBlock.gameObject.SetActive(isActive);
			_uiManager.Navigations.gameObject.SetActive(isActive);
		}

		public void SetActiveGamePanel(ulong tagetTableId)
		{
			SetActiveGamePanel(_openPanels[tagetTableId]);
		}

		public void SetActiveGamePanel(GamePanel targetPanel)
		{
			foreach (var elem in _openPanels.Values)
				elem.Cg.alpha = 0;

			_activeGamePanel = targetPanel;
			_activeGamePanel.transform.SetAsLastSibling();
			_activeGamePanel.Cg.alpha = 1;
			_activeGamePanel.SetFocus();
			MobileTablesUIManager.Instance.SelectPage("Main");
			VisibleGamePanels();
		}

		public void CloseButtonTouch()
		{
			if (_activeGamePanel == null) return;
			_activeGamePanel.ClickClose();
		}

		public void HomeButtonTouch()
		{
			HideGamePanels();
			_baseNavigations.MainTouch();
		}

		public void SettingsButtonTouch()
		{
			HideGamePanels();
			//_baseNavigations.SettingsTouch();
			var go = MobileTablesUIManager.Instance.SelectPage("Settings");
			var mt = go.GetComponentInChildren<TopMobilePanel>(true);
			mt.SetVisiblePanels(TopMobilePanel.VisibleElements.BackButton);
			mt.OnBackAction.RemoveAllListeners();
			mt.OnBackAction.AddListener(() =>
			{
				MobileTablesUIManager.Instance.SelectPage("Main");
				VisibleGamePanels();
			});
		}
		public void ThemeButtonTouch()
		{
			//HideGamePanels();
			//it.Main.PopupController.Instance.ShowPopup(PopupType.Develop);
			HideGamePanels();
			var go = MobileTablesUIManager.Instance.SelectPage("Settings");
			go.GetComponent<it.UI.SettingsPage>().SelectPageByType("theme");
			var mt = go.GetComponentInChildren<TopMobilePanel>(true);
			mt.SetVisiblePanels(TopMobilePanel.VisibleElements.BackButton);
			mt.OnBackAction.RemoveAllListeners();
			mt.OnBackAction.AddListener(() =>
			{
				MobileTablesUIManager.Instance.SelectPage("Main");
				VisibleGamePanels();
			});
			//_baseNavigations.ProfileTouch();
		}
		public void WclButtonTouch()
		{
			HideGamePanels();
			_baseNavigations.WclTouch();
		}

		public void AddTableButtonTouch()
		{
			HideGamePanels();
			_baseNavigations.MainTouch();

			if (_activeGamePanel != null && _activeGamePanel.Table != null && _activeGamePanel.Table.is_vip)
			{
				var go = MobileTablesUIManager.Instance.SelectPage("Main");
				go.GetComponent<it.Mobile.Main.LobbyPage>().SelectPage(LobbyType.VipGame);
			}

		}

		public void ObserversButtonTouch()
		{
			if (_activeGamePanel == null) return;
			_activeGamePanel.OpenObservers();
		}

	}
}