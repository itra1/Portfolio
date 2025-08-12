using TMPro;
using UnityEngine;
using it.Network.Rest;
using it.Popups;
using it.Network.Socket;
using System.Collections.Generic;
using DG.Tweening;
using it.UI.Elements;
using Garilla.Games;
using Garilla.Games.Animations;
using Garilla;
using System.Linq;
using System.Net.Http;
using it.Settings;

using it.Mobile;
using com.ootii.Geometry;
using System;

namespace it.UI
{
	public class GamePanel : it.UI.UIPanel
	{
		[HideInInspector] public UnityEngine.Events.UnityEvent OnClose = new UnityEngine.Events.UnityEvent();
		[HideInInspector] public UnityEngine.Events.UnityEvent OnFocus = new UnityEngine.Events.UnityEvent();

		[HideInInspector] public UnityEngine.Events.UnityEvent OnSettings = new UnityEngine.Events.UnityEvent();
		[HideInInspector] public UnityEngine.Events.UnityEvent OnTheme = new UnityEngine.Events.UnityEvent();
		[HideInInspector] public UnityEngine.Events.UnityEvent OnWcl = new UnityEngine.Events.UnityEvent();
		[HideInInspector] public UnityEngine.Events.UnityEvent<DateTime> OnPlayerAction = new UnityEngine.Events.UnityEvent<DateTime>();

		public List<InteractableActivationObject> Buttons { get => _buttons; set => _buttons = value; }
		public GameUIManager CurrentGameUIManager { get => _currentGameUIManager; set => _currentGameUIManager = value; }
		public ChatController ChatManager { get => _chatManager; set => _chatManager = value; }
		public TableAnimationsManager AnimationsManager { get => _animationsManager; set => _animationsManager = value; }
		public Table Table { get => _table; set => _table = value; }
		public CanvasGroup Cg => _cg;

		public TextMeshProUGUI DistribVersion;

		[SerializeField] public RectTransform _burgerButton;
		[SerializeField] private List<InteractableActivationObject> _buttons;
		[SerializeField] private InteractableActivationObject _hudButtons;
		[SerializeField] private InteractableActivationObject _afkButton;
		[SerializeField] private InteractableActivationObject _addChipsButton;
		[SerializeField] private InteractableActivationObject _settingsButton;
		[SerializeField] private RectTransform _contentMove;
		[SerializeField] private RectTransform _gamePanelsParent;

		[Header("GamePrefabs")]
		[SerializeField] private GameUIManager _gameUIManager;
		[SerializeField] private ChinaGameUIManger _chinaGameUIManger;
		[SerializeField] private ChatController _chatManager;

		[Space]
		[SerializeField] private RectTransform _gameContainer;
		[SerializeField] private TextMeshProUGUI _title;

		[Space, Header("Chat")]
		[SerializeField] private ChatController _chat;
		[SerializeField] public UI.Elements.GraphicButtonUI ChatButton;

		[Space, Header("Observers")]
		[SerializeField] private ObserversListPopup _observersPopup;
		[SerializeField] private TextMeshProUGUI _observersCount;

		[Space, Header("Panel For Mobile China")]
		[SerializeField] private GameObject mobileChinaButtonsPanel;

		[SerializeField] private TableBalances _tableBalancePanel;
		[SerializeField] private TableSettings _tableSettingsPanel;

		public GameSession GameSession;
		private ChinaGameUIManger _currentChinaGameUIManger;
		private GameUIManager _currentGameUIManager;
		private Table _table;
		private string _chanel;
		private Garilla.Games.Animations.TableAnimationsManager _animationsManager;
		//private TimerManager.RealTimer _playerAfkTimer;
		//private TimerManager.RealTimer _playerNoAfkTimer;
		private bool _interactableButtons = true;
		private bool _interactableAfkButton;

		[HideInInspector] public bool exitNotWindow = false;
		private CanvasGroup _cg;

		private void Awake()
		{
			GameSession = new GameSession(this);
			_tableBalancePanel.GamePanel = this;
			_tableSettingsPanel.GamePanel = this;

#if !UNITY_STANDALONE

			_cg = gameObject.GetOrAddComponent<CanvasGroup>();
			_cg.alpha = 1;
#endif

		}

		private void Start()
		{
			if (_gamePanelsParent != null)
			{
#if !UNITY_STANDALONE
				var otm = GetComponentInParent<OpenTableManager>();
				if (otm != null)
				{
					_gamePanelsParent.transform.SetParent(otm.GameRt.transform);
					_gamePanelsParent.transform.SetAsLastSibling();
				}
#endif
			}
		}

		private void OnEnable()
		{
			DeSubscribeSocketEvents();
			SubscribeSocketEvents();
		}

		private void OnDisable()
		{
			DeSubscribeSocketEvents();
		}

		private void SubscribeSocketEvents()
		{
			com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.MyAfkChange, MyAfkChange);
			com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.MyAfkActive, MyAfkChange);
			com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.SessionSingleAnotherConfirm, SessionSingleError);
			com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.WSConnect, WSConnect);
			com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.WSDisconnect, WSDisconnect);
			com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.SocketOpen, SocketOpen);
		}

		private void DeSubscribeSocketEvents()
		{
			com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.MyAfkChange, MyAfkChange);
			com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.MyAfkActive, MyAfkChange);
			com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.SessionSingleAnotherConfirm, SessionSingleError);
			com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.WSConnect, WSConnect);
			com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.WSDisconnect, WSDisconnect);
			com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.SocketOpen, SocketOpen);
			if (!string.IsNullOrEmpty(_chanel))
				com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.DropSmile(_chanel), DropSmile);
		}
		private void WSConnect(com.ootii.Messages.IMessage handle)
		{
			_currentGameUIManager.WSConnect();
		}
		private void WSDisconnect(com.ootii.Messages.IMessage handle)
		{
			_currentGameUIManager.WSDisconnect();
		}
		private void SocketOpen(com.ootii.Messages.IMessage handle)
		{
			_currentGameUIManager.WSSocketOpen();
		}

		private void OnDestroy()
		{
		}

		public bool HashActiveDistribution()
		{
			return CurrentGameUIManager != null && CurrentGameUIManager.IsHasGame;
		}

		public bool HasInGame()
		{
			if (CurrentGameUIManager == null)
				return false;

			if (!CurrentGameUIManager.InGame())
				return false;

			return true;

		}


		public void SetWaitToUpdate(int handsCount)
		{
			if (CurrentGameUIManager.HandsBeforeUpdate != null)
				return;

			if (!CurrentGameUIManager.IsHasGame)
				return;

			CurrentGameUIManager.HandsBeforeUpdate = handsCount;
		}

		public void EmitCloseTable()
		{
			gameObject.SetActive(false);

			OnClose?.Invoke();

			if (_currentGameUIManager != null)
			{
				_currentGameUIManager.StopSubscribe();
				_currentGameUIManager.ClearPackages();
			}
			Destroy(gameObject, 10);

			if (_table != null)
				SocketClient.Instance.LeaveTableChanel(_table.id);

			if (_gamePanelsParent != null)
				Destroy(_gamePanelsParent.gameObject);
		}

		private void MyAfkChange(com.ootii.Messages.IMessage handel)
		{
			RequestMyAfk();
		}

		private void SessionSingleError(com.ootii.Messages.IMessage handel)
		{
			CheckLossSession();
		}

		public void SetFocus()
		{
			OnFocus?.Invoke();
		}

		public void EminUserAction(DateTime time)
		{
			OnPlayerAction?.Invoke(time);
		}
		public void Init(Table table)
		{
			_animationsManager = new Garilla.Games.Animations.TableAnimationsManager();
			_animationsManager.BasePanel = this;

			_table = table;

			if (_burgerButton != null)
				_burgerButton.gameObject.SetActive(!_table.is_all_or_nothing);

			_chanel = SocketClient.GetChanelTable(_table.id);
			SocketClient.Instance.EnterTableChanel(_table.id);

			if (_title != null)
				_title.text = (_table.ante != null)
				? $"{table.name} - {it.Helpers.Currency.String((float)_table.ante)} (A{it.Helpers.Currency.String((float)_table.ante)})"
				: $"{table.name} - {it.Helpers.Currency.String(table.SmallBlindSize)} / {it.Helpers.Currency.String(table.big_blind_size)}";

			if (table.game_rule_id == (int)GameType.China)
			{
				_currentChinaGameUIManger = Instantiate(_chinaGameUIManger, _gameContainer);
				_currentChinaGameUIManger.InitGame(table, this);
				_currentChinaGameUIManger.transform.SetAsFirstSibling();
			}
			else
			{
				_currentGameUIManager = Instantiate(_gameUIManager, _gameContainer);
				_currentGameUIManager.InitGame(table, this);
				_currentGameUIManager.transform.SetAsFirstSibling();
			}
#if UNITY_STANDALONE
			TablesUIManager.tablesOpen.Add(StandaloneController.GetWindowTableName(table));
#endif

			//WebsocketClient.Instance.ObserversUpdateCallback += UpdateObserversCount;

			com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.ObsorveListUpdate(_chanel), ObsorveUpdate);
			com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.ObsorveListUpdate(_chanel), ObsorveUpdate);
			com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.DropSmile(_chanel), DropSmile);
			com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.DropSmile(_chanel), DropSmile);

			RequestObserveCount();

#if UNITY_STANDALONE && !UNITY_EDITOR
			StandaloneController.SetNewWindowName(table);
#endif
#if UNITY_STANDALONE && !UNITY_EDITOR
        Application.wantsToQuit += WantsToQuit;
#endif
			if (_chat != null)
				_chat.Init(_table, true, false);
			RequestMyAfk();
		}

		public void RequestObserveCount()
		{
			if (_observersCount == null) return;

			GameHelper.GetObservers(_table, x =>
			{
				_observersCount.text = x.data.Length.ToString();
			});
		}

		//#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
		bool WantsToQuit()
		{
			bool close = exitNotWindow || _table.isAtTheTable == false || PlayerPrefs.GetInt(StringConstants.CloseNow) == 1;

			if (close)
			{
				CloseTable();
			}
			else
			{
				CloseWindowOpen();
			}
			return close;
		}
		//#endif

		public void SetInteractableButtons(bool isInteractable)
		{
			if (_interactableButtons == isInteractable) return;

			_interactableButtons = isInteractable;
			foreach (var elem in _buttons)
				elem.Interactable = isInteractable;

			if (_hudButtons != null && Table != null && Table.is_all_or_nothing)
				_hudButtons.Interactable = false;

			if (isInteractable)
				RequestMyAfk();

			if (_settingsButton != null)
				_settingsButton.Interactable = _interactableButtons && !_currentGameUIManager.SelectTable.is_all_or_nothing;

			if (_addChipsButton != null)
				_addChipsButton.Interactable = _interactableButtons && (_currentGameUIManager.SelectTable == null || !_currentGameUIManager.SelectTable.is_all_or_nothing);

			if (_afkButton != null)
				_afkButton.Interactable = _interactableButtons && _interactableAfkButton;
		}

		public void CloseRequest()
		{
			TableApi.ObserveCancelTable(_table.id, null);

			////if (_table.isAtTheTable)
			//{
			GameHelper.GetUpTable(_table, (table) =>
			{
				Close();
			}, (error) =>
			{
				Close();
			});
			//}
		}

		public void Close()
		{
#if UNITY_EDITOR || UNITY_ANDROID || UNITY_WEBGL || UNITY_IOS
			CloseTable();
#endif

#if UNITY_STANDALONE
			exitNotWindow = true;
			Application.Quit();
#endif
		}

		private void CloseTable()
		{
			if (_table.game_rule_id == (int)GameType.China)
			{
				_currentChinaGameUIManger.Close();
			}
			else
			{
				//Close();
			}
			EmitCloseTable();
			//Destroy(gameObject);
#if UNITY_STANDALONE

			StandaloneController.Instance.RemoveGameFromParent(_table);
#endif
		}

		public void ClickClose()
		{
#if !UNITY_STANDALONE || UNITY_EDITOR
			CloseWindowOpen();
#endif
		}

		public void CloseWindowOpen()
		{
			//if (!_table.isAtTheTable && (_table == null || !_table.isAtTheTable))
			bool inTable = false;
			foreach (var elem in _currentGameUIManager.Players)
				if (elem.Value.UserId == UserController.User.id)
					inTable = true;
			if (!inTable)
			{
				CloseRequest();
			}
			else
			{
				LeaveWindowPopup popup = it.Main.PopupController.Instance.ShowPopup<LeaveWindowPopup>(PopupType.ExitGame);

				popup.OnOk = CloseRequest;

				if (_table.game_rule_id == (int)GameType.China)
				{
					popup.Init(_table, this);
				}
				else
				{
					if (_currentGameUIManager._currentSharedData != null && _currentGameUIManager._currentSharedData.MePlayer != null)
						popup.Init(_table, this, _currentGameUIManager._currentSharedData.MePlayer.amount);
					else
						popup.Init(_table, this);
				}
			}
		}
		private bool _isWaitToCloseUpdate;
		private void Update()
		{
			if (PlayerPrefs.GetInt(StringConstants.CloseNow) == 1)
			{
				Application.Quit();
			}

			if (PlayerPrefs.HasKey(StringConstants.SESSION_SINGLE_ERROR))
			{
				CheckLossSession();
			}

			if (PlayerPrefs.HasKey(StringConstants.APP_UPDATE))
			{
				if (!HasInGame() && CurrentGameUIManager.HandsBeforeUpdate == null)
				{
					if (!_isWaitToCloseUpdate)
					{
						_isWaitToCloseUpdate = true;
						CloseRequest();
					}
				}
				else
				{
					if (CurrentGameUIManager.HandsBeforeUpdate == null)
					{
						SetWaitToUpdate(Garilla.Update.UpdateController.STEP_TO_EXIT);
					}
				}


			}

		}

		private void CheckLossSession()
		{
#if UNITY_STANDALONE
			//UserController.Instance.AnotherPlayerAuthorization(() =>
			//{
			//	exitNotWindow = true;
			//	Application.Quit();
			//});
			exitNotWindow = true;
			Application.Quit();

			//var infoPanel = it.Main.PopupController.Instance.ShowPopup<InfoPopup>(PopupType.Info);
			//infoPanel.SetDescriptionString("errors.forms.sessionSingleErrors".Localized());
			//infoPanel.OnConfirm = () =>
			//{
			//	exitNotWindow = true;
			//	Application.Quit();
			//};
#endif
		}

		//float lastAfkRequest;
		public void RequestMyAfk(bool state = false)
		{
			//if (Time.realtimeSinceStartup - lastAfkRequest < 0.3f) return;
			//lastAfkRequest = Time.realtimeSinceStartup;

			TableApi.GetMyAfk(_table.id, (result) =>
			{

				if (!result.IsSuccess)
				{
					//it.Logger.LogError("Error GetMyAfk " + result.ErrorMessage);
					return;
				}
				TimerManager.RealTimer timer = null;
				var afk = result.Result;

				_interactableAfkButton = true;

				if (!afk.SkipDistributions && !afk.CanSkipDistributions && !afk.SkipDistributionsWellBeSet)
				{
					_interactableAfkButton = false;

					timer = (TimerManager.RealTimer)TimerManager.Instance.GetTimer(StringConstants.AfkNoTimerName(UserController.User.id));
					if (timer == null)
						timer = (TimerManager.RealTimer)TimerManager.Instance.AddTimer(StringConstants.AfkNoTimerName(UserController.User.id), afk.CanSkipDistributionsTime);
					timer.Set(afk.CanSkipDistributionsTime);
				}
				else
				{
					_interactableAfkButton = true;

					if (afk.SkipDistributions)
					{
						timer = (TimerManager.RealTimer)TimerManager.Instance.GetTimer(StringConstants.AfkTimerName(UserController.User.id));

						if (timer == null)
							timer = (TimerManager.RealTimer)TimerManager.Instance.AddTimer(StringConstants.AfkTimerName(UserController.User.id), afk.SkipDistributionsTime);
						timer.Set(afk.SkipDistributionsTime);
					}
				}

				if (_afkButton != null)
					_afkButton.Interactable = _interactableButtons && _interactableAfkButton;

				if (timer != null)
				{
					timer.OnComplete.RemoveListener(RequestMyAfk);
					timer.OnComplete.AddListener(RequestMyAfk);
				}

			});

		}

		#region Buttons

		public void OpenChatButtonTouch()
		{
			//_chat.gameObject.SetActive(true);
			//_chat.Init(_table, true, false);
			_chat.ChatOpen();
		}

		public void OpenChatSmileButtonTouch()
		{
			//_chat.gameObject.SetActive(true);
			//_chat.Init(_table, false, true);
			_chat.SmileWindowOpen();
		}

		public void OpenBalancesPanel()
		{
			if (_tableBalancePanel != null)
			{
				_tableBalancePanel.SetVisible();
			}
		}

		public void CloseChat()
		{
			_chat.gameObject.SetActive(false);
			_chat.ChatClose();
		}

		public void AfkButtonTouch()
		{
			if (_table == null) return;
			PlayerGameIcone player = null;
			foreach (var pl in _currentGameUIManager.Players.Values)
				if (pl.UserId == UserController.User.id)
					player = pl;

			if (player == null) return;

			AfkActivatePopup panel = it.Main.PopupController.Instance.ShowPopup<AfkActivatePopup>(PopupType.AfkActivate);
			panel.Init(_table.id);
		}
		public void TouchChips()
		{
			if (_currentGameUIManager)
			{
				_currentGameUIManager.TouchChips();
			}
			else
			{
				_currentChinaGameUIManger.TouchChips();
			}
		}
		public void OpenInfo()
		{
			//_tableInfo.Show(_table);
			UITablenInfo popup = it.Main.PopupController.Instance.ShowPopup<UITablenInfo>(PopupType.TableInfo);
			popup.Init(_table);
		}

		public void OpenHandHistory()
		{
			Garilla.Games.UI.HandHistoryPopup popup = it.Main.PopupController.Instance.ShowPopup<Garilla.Games.UI.HandHistoryPopup>(PopupType.HandHistory);
			popup.Init(_table, _currentGameUIManager);
			//_handHistory.Show(_table, _currentGameUIManager);
		}

		public void OpenInsufficientBalance()
		{
			it.Main.PopupController.Instance.ShowPopup(PopupType.InsufficientBalance);
			//_insuffWindow.Show();
		}

		public void OpenCashier()
		{
			it.Main.PopupController.Instance.ShowPopup(PopupType.Cashier);
		}

		public void SettingsButtonTouch()
		{
#if !UNITY_STANDALONE
			OnSettings?.Invoke();
			return;
#endif

#if UNITY_STANDALONE
			PlayerPrefs.SetString(StringConstants.BUTTON_SETTINGS, "");
			StandaloneController.Instance.FocusMain();
#endif
		}

		public void TableThemeButtonTouch()
		{
#if !UNITY_STANDALONE
			OnTheme?.Invoke();
			return;
#endif

#if UNITY_STANDALONE
			PlayerPrefs.SetString(StringConstants.BUTTON_TABLE_THEME, "");
			StandaloneController.Instance.FocusMain();
#endif
		}

		public void CasinoButtonTouch()
		{
			LinkManager.OpenUrl("casino");
		}

		public void WCLButtonTouch()
		{

#if !UNITY_STANDALONE
			OnWcl?.Invoke();
			return;
#endif
		}

		#endregion



		public void HideGames()
		{
#if UNITY_STANDALONE
			StandaloneController.Instance.HideGames();
#endif
		}

		public void OpenObservers()
		{
			var popup = it.Main.PopupController.Instance.ShowPopup<ObserversListPopup>(PopupType.ObservingList);
			popup.SetData(_table);
			//_observersPopup.ObserversUpdate(ul);
		}

		public void OpenUserInfo(UserLimited user, UserStat stat, UserNote note)
		{
			if (user.id == UserController.User.id)
			{
				var panel = it.Main.PopupController.Instance.ShowPopup<SmartHudPopup>(PopupType.SmartHudPlayer);
				panel.SetData(user, stat);
			}
			else
			{
				var panel = it.Main.PopupController.Instance.ShowPopup<SmartHudOpponentPopup>(PopupType.SmartHudOpponenty);

				if (stat == null)
				{
					panel.SetData(this, user, _table, note);
				}
				else
					panel.SetData(this, _table, user, stat, note);

			}

		}

		public void EnableMobileChinaPanel(bool isVisible)
		{
			mobileChinaButtonsPanel.SetActive(isVisible);
		}
		UsersLimitedRespone ul;


		public void UpdateObserversCount(ObserversUsersRespone observersRespone)
		{
			if (_observersCount != null) _observersCount.text = $"{observersRespone.observers.Length}";
			ul = observersRespone.GetUsersLimitedRespone;
		}

		private void ObsorveUpdate(com.ootii.Messages.IMessage handle)
		{
			UpdateObserversCount(((ObserversListUpdated)handle.Data).Obsorves);
		}

		private void DropSmile(com.ootii.Messages.IMessage handle)
		{
			var packet = (SmileDrop)handle.Data;
			if (packet.FromUserId == UserController.User.id) return;
			ChatManager.DropSmileActive(packet.FromUserId, packet.ToUserId, packet.SmileId);
		}

		public void HeaderGameRulseTouch()
		{

			if (_table == null) return;

			var bl = GameSettings.Blocks.Find(x =>
			x.AllOrNothing == _table.is_all_or_nothing
			&& x.IsDealerChoice == _table.is_dealer_choice
			&& x.TypeGame.Contains((GameType)_table.game_rule_id)
			);

			LinkManager.OpenUrl(bl.keyRulesLink);
		}

	}
}