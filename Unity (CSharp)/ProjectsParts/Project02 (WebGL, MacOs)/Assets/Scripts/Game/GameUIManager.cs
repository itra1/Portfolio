using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using it.Network.Rest;
using it.Game.Panels;
using it.UI;
using it.Popups;
using it.Network.Socket;
using Sett = it.Settings;
using it.UI.Elements;
using Garilla.Games.UI;
using DG.Tweening;
using it.Main.SinglePages;
using Sentry;
using Cysharp.Threading.Tasks;
using System.Runtime.ConstrainedExecution;


namespace Garilla.Games
{
	public class GameUIManager : GameControllerBase
	{
		//public PlayerGameIcone GamePlayerUIPrefabs;
		//[SerializeField] private GamePlaceUI GamePlaceUIPrefab;
		//[SerializeField] public GameCardUI GameCardUIPrefab;
		[SerializeField] private BankSplit _bankSplit;
		[SerializeField] private TextMeshProUGUI ToggleCheckOrFoldText;
		[SerializeField] private GameInfoLine _waitingForPlayers;
		[SerializeField] private ActionsButtonsPanel _actionsPanel;
		[SerializeField] private DynamitePanel _dynamitePanel;
		[SerializeField] private AllOrNothingPanel _allInWindow;
		[SerializeField] private PrizePanel _prisePanel;
		[SerializeField] private JackpotPanel _jackpotPanel;
		[SerializeField] private AddTimePanel _addTimePanel;
		[SerializeField] private DealerChoisePanel _dealerChoisePanel;
		[SerializeField] private RectTransform _jackpotWidget;
		[SerializeField] private RectTransform _badBeadWidget;
		[SerializeField] private RectTransform _leaderboardSwitcherPanel;
		[SerializeField] private LeaderboardPanel _leaderboardPanel;
		[SerializeField] private ClickToShowPanel _clickToShowPanel;
		[SerializeField] private StraddlePanel _straddlePanel;
		[SerializeField] private BadBeatPanel _badBeatPanel;
		[SerializeField] private UpdateInfoGamePanel _updatePanel;
		[SerializeField] private Garilla.Promotions.PromotionsIconePanel _promotionsPanel;

		[SerializeField] private Toggle CheckOrFoldToggle;
		[SerializeField] private Toggle WaitBlindsToggle;

		//[SerializeField] private GameObject RightContainer;
		[SerializeField] private GameObject WaitBlindsContainer;
		[SerializeField] private GameObject CheckOrFoldContainer;
		//[SerializeField] private GameObject ContainerCombination;

		[Space][SerializeField] private List<Transform> PlayerPlacesTakeSit;
		[SerializeField] private List<Transform> PlayerPlaces;
		[SerializeField] private List<RectTransform> _cardPlaces;

		[Space][SerializeField] private Transform TableParent;

		[HideInInspector] public DistributionSharedData _currentSharedData = null;
		private SocketEventDistributionUserData _currentUserData = null;

		[Space] private it.UI.Elements.GraphicButtonUI ChatButton;
		private List<GameCardUI> _cardsOnTable = new List<GameCardUI>();
		private bool _isShowDown = false;
		private SocketEventDistributionSharedData _sharedData;
		private List<DistributionPlayerCombination> _winCombinations = new List<DistributionPlayerCombination>();
		private TablePlaceManager _placeManager;
		private System.Threading.CancellationTokenSource _eventCancelledTokenSource;

		public List<RectTransform> CardPlaces => _cardPlaces;
		public RectTransform CardTableParent => _cardPlaces[0].parent.GetComponent<RectTransform>();
		public GamePhase GamePhase { get; set; } = GamePhase.None;
		public SocketEventDistributionSharedData SharedData { get => _sharedData; set => _sharedData = value; }
		public ActionsButtonsPanel ActionsPanel { get => _actionsPanel; set => _actionsPanel = value; }
		public PrizePanel PrisePanel { get => _prisePanel; set => _prisePanel = value; }
		public TablePlaceManager PlaceManager { get => _placeManager; set => _placeManager = value; }
		public List<GameCardUI> CardsOnTable { get => _cardsOnTable; set => _cardsOnTable = value; }
		public BankSplit BankSplit { get => _bankSplit; set => _bankSplit = value; }

		public int AnimationToTableWait = 0;

		private bool _alwaysChipMax;

		private bool _existsNewEvents;
		private ulong _lastEvent;
		private bool _visibleClickToShow;
		private TotalPot _totalPot;
		private bool _waitAddChipPopup;

		void Start()
		{
			_lastSocketEventTime = DateTime.Now;
			_totalPot = _totalBank.GetComponent<TotalPot>();
			//StartGame();
			_actionsPanel.GameManager = this;
			_dynamitePanel.GameManager = this;
			_allInWindow.GameManager = this;
			_bankSplit.GameManager = this;
			_badBeatPanel.GameManager = this;
			if (_updatePanel != null) _updatePanel.GameManager = this;
			if (_straddlePanel != null)
				_straddlePanel.GameManager = this;
			_promotionsPanel.GameManager = this;
			_clickToShowPanel.gameObject.SetActive(false);
			//_actionsPanel.UpdateSlider = UpdateRaiseCount;
			_actionsPanel.gameObject.SetActive(false);

			WaitBlindsToggle.onValueChanged.RemoveAllListeners();
			WaitBlindsToggle.onValueChanged.AddListener((val) =>
			{
				OnValueChangedWaitBlinds();
			});
			CheckOrFoldToggle.onValueChanged.RemoveAllListeners();
			CheckOrFoldToggle.onValueChanged.AddListener((val) =>
			{
				OnValueChangedAutoFold();
			});

			//_dealerChoisePanel.gameObject.SetActive(false);
		}

		public override void StopSubscribe()
		{
			base.StopSubscribe();
			com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.JackpotWin(_chanel), OnJackpot);
			com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.DealerChoise(_chanel), DealerChoise);
			com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.GameTableEvent(_chanel), OnTableEvent);
			com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.GameTableReserve(_chanel), OnTableReserve);
			com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.GameTableCountdown(_chanel), OnTableCountdown);
			com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.GameTableUpdate(_chanel), GameUpdate);

			foreach (var elem in _players)
				PoolerManager.Return("PlayerGame", elem.Value.gameObject);
			_players.Clear();
		}

		public override void InitGame(Table table, it.UI.GamePanel gameInitManager)
		{
			this.SelectTable = table;
			_promotionsPanel.SelectTable(SelectTable);
			_currentSharedData = null;
			base.InitGame(table, gameInitManager);

			this._gamePanel.SetInteractableButtons(false);
			var placePref
#if UNITY_STANDALONE
			= Sett.StandaloneSettings.GetTablePlaceManager(table, false);
#elif UNITY_ANDROID || UNITY_WEBGL || UNITY_IOS
			= Sett.AndroidSettings.GetTablePlaceManagerStatic(table, false);
#endif
			Clear();

			_placeManager = Instantiate(placePref, TableParent);

			_placeManager.transform.SetAsFirstSibling();

			RectTransform rt = _placeManager.GetComponent<RectTransform>();

			rt.position = Vector3.zero;
			rt.anchoredPosition = Vector3.zero;
			rt.localScale = Vector3.one;
			rt.anchorMin = Vector3.zero;
			rt.anchorMax = Vector3.one;
			rt.sizeDelta = Vector3.zero;

			_placeManager.SetTablePositions(table);

			PlayerPlaces = _placeManager.Places.PlayerPlaces;
			PlayerPlacesTakeSit = _placeManager.Places.PlayerPlacesTakeSit;
			_bets = _placeManager.Places.Bets;
			_dealers = _placeManager.Places.Dealers;
			_dealers.ForEach(x => x.enabled = false);
			CardDrop = _placeManager.Places.CardDrop;
			ChatButton = gameInitManager.ChatButton;

			_allInWindow.GameManager = this;
			_allInWindow.gameObject.SetActive(table.is_all_or_nothing);
			if (table.is_all_or_nothing)
			{
				_allInWindow.Init();
				_allInWindow.SetBaseData(SelectTable.bingo_info);
			}

			_actionsPanel.gameObject.SetActive(false);

			if (_dealerChoisePanel != null)
				_dealerChoisePanel.gameObject.SetActive(table.is_dealer_choice);
			if (_jackpotWidget != null)
				_jackpotWidget.gameObject.SetActive(table.is_all_or_nothing);

#if !UNITY_STANDALONE
			if (table.is_all_or_nothing)
			{
				var promoRect = _promotionsPanel.GetComponent<RectTransform>();
				promoRect.anchoredPosition = new Vector2(promoRect.anchoredPosition.x, -288);
			}
#endif

			if (_leaderboardSwitcherPanel != null)
				_leaderboardSwitcherPanel.gameObject.SetActive(false);

			if (_badBeadWidget != null)
				_badBeadWidget.gameObject.SetActive(false);

			if (!table.is_all_or_nothing)
			{
				var leaderElement = it.Settings.GameSettings.GameNames.Find(x => x.GameType == (GameType)table.game_rule_id);
				if (leaderElement != null
				&& !table.IsFaceToFace
				//&& !table.is_dealer_choice
				&& GameType.OmahaLow5 != (GameType)table.game_rule_id
				&& GameType.OmahaLow4 != (GameType)table.game_rule_id
				&& GameType.Holdem6 != (GameType)table.game_rule_id
				&& GameType.Holdem3 != (GameType)table.game_rule_id
				)
				{
					if (!table.is_dealer_choice && _leaderboardSwitcherPanel != null)
						_leaderboardSwitcherPanel.gameObject.SetActive(true);

					if (table.is_dealer_choice && _badBeadWidget != null)
						_badBeadWidget.gameObject.SetActive(true);

					if (_leaderboardPanel != null)
					{
						_leaderboardPanel.gameObject.SetActive(true);
						_leaderboardPanel.SetTable(table);
					}
				}
				else
				{
					if (_leaderboardSwitcherPanel != null)
						_leaderboardSwitcherPanel.gameObject.SetActive(false);
					if (_badBeadWidget != null)
						_badBeadWidget.gameObject.SetActive(false);
				}
			}

			StartGame(table);
			ReservedPlaces(table.table_player_reservations);

		}
		private void Clear(bool isFull = false)
		{
			it.Logger.Log("Clear table distribution");

			if (isFull)
			{
				_eventCancelledTokenSource?.Cancel();
				_clickToShowPanel.Clear();
				_clickToShowPanel.gameObject.SetActive(false);
			}


			ClearBets();
			foreach (var cards in CardsOnTable)
				PoolerManager.Return("GameCard", cards.gameObject);

			foreach (var pl in _players.Values)
				if (pl != null)
					pl.ClearCombinations();

			//Destroy(cards.gameObject);
			CardsOnTable.Clear();
			SetBank(0);
			_bankSplit.Clear();
			_actionsPanel.gameObject.SetActive(false);
			_addTimePanel.Clear();

		}

		private void ClearBets()
		{
			_bets.ForEach((Action<Bets>)(_ =>
			{
				_.SetValue((Table)base.SelectTable, 0);
				_.gameObject.SetActive(false);
			}));

		}

		private void StartGame(Table table)
		{
			Init(table);
			CheckOrFoldToggle.isOn = false;
			WaitBlindsToggle.isOn = true;
			ToggleCheckOrFoldText.text = "Fold";
			StartSubscribe();
		}

		protected override void StartSubscribe()
		{

			base.StartSubscribe();

			com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.JackpotWin(_chanel), OnJackpot);
			com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.DealerChoise(_chanel), DealerChoise);
			com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.GameTableEvent(_chanel), OnTableEvent);
			com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.GameTableReserve(_chanel), OnTableReserve);
			com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.GameTableCountdown(_chanel), OnTableCountdown);
			com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.GameTableUpdate(_chanel), GameUpdate);
			com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.GameTableShowCards(_chanel), ShowDropCards);

			com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.JackpotWin(_chanel), OnJackpot);
			com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.DealerChoise(_chanel), DealerChoise);
			com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.GameTableEvent(_chanel), OnTableEvent);
			com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.GameTableReserve(_chanel), OnTableReserve);
			com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.GameTableCountdown(_chanel), OnTableCountdown);
			com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.GameTableUpdate(_chanel), GameUpdate);
			com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.GameTableShowCards(_chanel), ShowDropCards);
		}


		private void FullReloadTable()
		{
			_isWeatFullLoad = true;
			ClearPackages();
			TableManager.Instance.LoadTableRecord(SelectTable.id, (table) =>
			{
				SelectTable = table;
				CheckActiveDistribution();
			});
		}

		private void DealerChoise(com.ootii.Messages.IMessage handle)
		{
			_lastSocketEventTime = System.DateTime.Now;
			var tabPacket = (DealerChoiceMade)handle.Data;
			SelectTable = tabPacket.Table;
			if (SelectTable.is_dealer_choice)
				_dealerChoisePanel.SetTable(SelectTable);
		}
		public override void WSConnect()
		{
			_waitingForPlayers.DisableLabel();
		}
		public override void WSDisconnect()
		{
			_waitingForPlayers.SetValue("Internet Connection Lost");
		}

		public override void WSSocketOpen()
		{
			SocketClient.Instance.EnterTableChanel(SelectTable.id);
			_lastSocketEventTime = System.DateTime.Now;
			FullReloadTable();
		}

		private void ShowDropCards(com.ootii.Messages.IMessage handle)
		{
			if (_currentSharedData == null) return;
			var distribEvents = (Assets.Scripts.Network.Socket.In.PokerCardShow)handle.Data;
			if (_currentSharedData.id != distribEvents.DistributionId) return;

			foreach (var pl in Players.Values)
			{
				if (pl.UserId == distribEvents.UserId)
				{
					try
					{
						pl.ShowDropCards(distribEvents.CardList);
					}
					catch (Exception ex)
					{
						it.Logger.LogError("Drop cards error " + ex.Message + " : " + ex.StackTrace);
					}
				}
			}

		}

		private void GameUpdate(com.ootii.Messages.IMessage handle)
		{
			_lastSocketEventTime = System.DateTime.Now;
			AddPackageToProcessedQueue(new PacketProcessItem()
			{
				Package = (SocketIn)handle.Sender,
				Data = handle.Data,
				Action = (package) =>
				{
					var tableEvent = ((it.Network.Socket.GameUpdate)package).TableEvent;
					if (tableEvent.table.id != SelectTable.id) return;

					SelectTable = tableEvent.table;

					UpdateStateTable();
					CompletePackageProcessed();

					if (HandsBeforeUpdate != null && !InGame())
					{
						GamePanel.CloseRequest();
					}

					//if (tableEvent.table.sortedPlayerSessions.Length <= 1)
					//{
					//	StartCoroutine(OnePlayerCor(tableEvent.table));
					//}
				}
			}); ;

		}

		private void UpdateStateTable()
		{
			try
			{
				_gamePanel.RequestObserveCount();

				if (SelectTable.is_dealer_choice)
					_dealerChoisePanel.SetTable(SelectTable);
				UpdateTable(SelectTable);
				SetStateOnTable();
				ProcessDiller(SelectTable.table_player_sessions);


				for (int i = 0; i < SelectTable.table_player_sessions.Length; i++)
				{
					if (!SelectTable.is_all_or_nothing
						&& SelectTable.isAtTheTable
						&& SelectTable.table_player_sessions[i].user_id == UserController.User.id
						&& SelectTable.table_player_sessions[i].amount <= 0)
					{
						_waitAddChipPopup = true;
						if (!_isGame)
							ShowAddChipIfNeed();
					}
				}

				if (SelectTable.table_player_sessions.Length <= 1)
				{
					Clear();
					foreach (var elem in Players)
						elem.Value.Clear();
				}

#if UNITY_WEBGL || UNITY_ANDROID || UNITY_IOS
				if (!SelectTable.is_all_or_nothing)
				{
					RectTransform rt = _bets[0].GetComponent<RectTransform>();
					rt.anchoredPosition = SelectTable.isAtTheTable ? new Vector2(-230, -30) : Vector2.zero;
				}
#endif
				//CheckAutoMaxIncrement();
			}
			catch (Exception ex)
			{
				it.Logger.LogError("Error action GameUpdate " + ex.Message + " : " + ex.StackTrace);
			}
		}

		private void ShowAddChipIfNeed()
		{
			_waitAddChipPopup = false;
			for (int i = 0; i < SelectTable.table_player_sessions.Length; i++)
			{
				if (!SelectTable.is_all_or_nothing
					&& SelectTable.isAtTheTable
					&& SelectTable.table_player_sessions[i].user_id == UserController.User.id
					&& SelectTable.table_player_sessions[i].amount <= 0)
				{
					TouchChips(true, SelectTable.table_player_sessions[i].amount_check_at);
				}
			}
		}

		private void OnTableReserve(com.ootii.Messages.IMessage handle)
		{
			_lastSocketEventTime = System.DateTime.Now;
			AddPackageToProcessedQueue(new PacketProcessItem()
			{
				Package = (SocketIn)handle.Sender,
				Data = handle.Data,
				Action = (package) =>
				{
					try
					{
						var tableEvent = ((it.Network.Socket.GameUpdate)package).TableEvent;
						ReservedPlaces(tableEvent.table.table_player_reservations);
					}
					catch (Exception ex)
					{
						it.Logger.LogError("Error action OnTableReserve " + ex.Message);
					}
					CompletePackageProcessed();
				}
			});

		}

		private void OnTableCountdown(com.ootii.Messages.IMessage handle)
		{
			_lastSocketEventTime = System.DateTime.Now;
			AddPackageToProcessedQueue(new PacketProcessItem()
			{
				Package = (SocketIn)handle.Sender,
				Data = handle.Data,
				Action = (package) =>
				{
					try
					{
						_waitingForPlayers.gameObject.SetActive(true);
						_waitingForPlayers.StartTimer(((DistributionStartCountdown)handle.Data).Cooldown.delay);
					}
					catch (Exception ex)
					{
						it.Logger.LogError("Error action OnTableCountdown " + ex.Message);
					}
					CompletePackageProcessed();
				}
			});
		}

		private void OnTableEvent(com.ootii.Messages.IMessage handle)
		{
			_lastSocketEventTime = System.DateTime.Now;

			if (_eventCancelledTokenSource != null)
			{
				_eventCancelledTokenSource.Dispose();
			}
			_eventCancelledTokenSource = null;
			_eventCancelledTokenSource = new System.Threading.CancellationTokenSource();

			AddPackageToProcessedQueue(new PacketProcessItem()
			{
				Package = (SocketIn)handle.Sender,
				Data = handle.Data,
				Action = (package) =>
				{
					try
					{
						it.Network.Socket.DistributonEvents data = (it.Network.Socket.DistributonEvents)package;

						GameHelper.ServerTime = data.server_time;
						_sharedData = null;
						_sharedData = data.SocketEventSharedData;
						_currentUserData = null;
						_currentUserData = data.SocketEventUserData;
						_currentSharedData = null;
						_currentSharedData = _sharedData.distribution;
						_currentUserData = null;
						_currentUserData = data.SocketEventUserData;
						_isShowDown = false;

						if (_currentSharedData.WinCombinations.Count > 0)
							_winCombinations = _currentSharedData.WinCombinations;

						if (SelectTable.is_dealer_choice)
							_dealerChoisePanel.SetTable(SelectTable);
						if (SelectTable.is_all_or_nothing)
							_allInWindow.Init();

						DistributionInit(_sharedData.distribution, data.SocketEventUserData);
						ParseEvents(_sharedData.events, () =>
						{
							if (!_existsNewEvents)
							{
								_eventCancelledTokenSource?.Dispose();
								_eventCancelledTokenSource = null;
								CompletePackageProcessed();
								return;
							}

							try
							{
								DistributionProcess();

								if (_sharedData != null && _sharedData.distribution != null && GamePanel.DistribVersion != null)
									GamePanel.DistribVersion.text = $"D = {_sharedData.distribution.id}";

							}
							catch (Exception ex)
							{
								it.Logger.LogError("Error action DistributionProcess " + ex.Message + " : " + ex.StackTrace);
							}
							it.Logger.Log("Before complete");
							//_eventCancelledTokenSource?.Dispose();
							//_eventCancelledTokenSource = null;
							CompletePackageProcessed();
						}, _eventCancelledTokenSource.Token);
					}
					catch (Exception ex)
					{
						it.Logger.LogError("Error action OnTableEvent " + ex.Message + " : " + ex.StackTrace);
						CompletePackageProcessed();
					}
				}
			});
		}

		private void ChangeTable(SocketEventTable socketEventTable, string chanel)
		{
			if (socketEventTable.table.id != SelectTable.id) return;

			UpdateTable(socketEventTable.table);
			if (socketEventTable.table.sortedPlayerSessions.Length <= 1)
			{
				OnePlayer(socketEventTable.table);
			}
		}

		private void OnJackpot(com.ootii.Messages.IMessage handler)
		{
			_lastSocketEventTime = System.DateTime.Now;
			var data = (it.Network.Socket.JackpotWin)handler.Data;
			_jackpotPanel.gameObject.SetActive(true);
			var panel = _jackpotPanel.GetComponent<it.Game.Panels.JackpotPanel>();
			panel.SetValue(data.Amount);
		}

		private void ReservedPlaces(List<PlaceReserve> reserves)
		{
			foreach (var key in placesFree.Keys)
			{
				var place = reserves.Find(x => x.place - 1 == placesFree[key].Place && x.FinishDate > GameHelper.NowTime);

				placesFree[key].SetReserved(place != null);
			}
		}

		async void OnePlayer(Table table)
		{
			await UniTask.Delay(1000);
			ClearTable();
			Init(table);
		}
		private void PlayersPositions(TablePlayerSession[] taplePlayers)
		{
			var LeavePlayers = _players.Values.ToList<PlayerGameIcone>();

			bool isAtTheTable = SelectTable.isAtTheTable;
			var mePlace = SelectTable.MePlaceInSorted;
			var t = isAtTheTable ? SelectTable.sortedPlayerSessions : SelectTable.table_player_sessions;
			TablePlayerSession[] playersAtTable = new TablePlayerSession[t.Length];
			Array.Copy(t, playersAtTable, t.Length);
			var playerList = t.ToList();
			this._gamePanel.SetInteractableButtons(isAtTheTable);

			var newPlayersList = new Dictionary<int, PlayerGameIcone>(_players);

			foreach (var pl in newPlayersList)
			{
				var sessionPlayer = playerList.Find((x) => x.user_id == pl.Value.UserId);

				if (sessionPlayer == null) continue;

				PlayerPositions(sessionPlayer, ref LeavePlayers);
				playerList.Remove(sessionPlayer);
			}


			foreach (var player in playerList)
			{
				PlayerPositions(player, ref LeavePlayers);
			}

			if (LeavePlayers.Count > 0)
			{
				LeavePlayers.ForEach(x =>
				{
					foreach (var key in _players.Keys)
						if (_players[key] == x)
						{
							if (_players[key] != null)
								PoolerManager.Return("PlayerGame", _players[key].gameObject);
							_players.Remove(key);
							break;
						}
					x.Hide();
				});
			}
			ProcessDiller();

		}
		private void PlayerPositions(TablePlayerSession player, ref List<PlayerGameIcone> LeavePlayers)
		{
			bool isAtTheTable = SelectTable.isAtTheTable;
			var mePlace = SelectTable.MePlaceInSorted;

			List<Transform> positionsSpawn = isAtTheTable ? PlayerPlaces : PlayerPlacesTakeSit;

			var place = 0;

			if (isAtTheTable)
			{
				place = mePlace != -1 ? player.sortedPlace - mePlace : player.sortedPlace;
				if (place < 0) place = positionsSpawn.Count + place;
			}
			else
			{
				place = player.placeAtTable;
			}

			var isMe = player.user_id == UserController.User.id;
			if (place >= positionsSpawn.Count)
			{
				it.Logger.Log("place >= PlayerPlaces.Count");
				return;
			}

			PlayerGameIcone playerIcone = null;

			int? oldPlace = null;

			foreach (var elem in _players)
			{
				if (elem.Value.UserId == player.user_id)
				{
					playerIcone = elem.Value;
					LeavePlayers.RemoveAll(x => x.UserId == playerIcone.UserId);

					if (elem.Key != place)
					{
						oldPlace = elem.Key;
						//if (_bets[oldPlace.Value].Value > 0)
						//{
						//	_bets[place].SetValue(_bets[oldPlace.Value].Value);
						//	_bets[oldPlace.Value].gameObject.SetActive(false);
						//	_bets[place].SetValue(true);
						//}
						if (_bets[oldPlace.Value].Value > 0)
						{
							_bets[place].SetValue(SelectTable, _bets[oldPlace.Value].Value);
							_bets[oldPlace.Value].gameObject.SetActive(false);
							_bets[place].gameObject.SetActive(true);
						}
						playerIcone.MoveParent(positionsSpawn[place]);
					}

					break;
				}

			}
			// нет уже созданного игрока
			// Создаем
			if (playerIcone == null)
			{

				//#if !UNITY_ANDROID || UNITY_WEBGL || UNITY_IOS

				it.Logger.Log("CREATE PLAYER " + player.id + " : " + place);
				//GameObject inst = Instantiate(GamePlayerUIPrefabs.gameObject, positionsSpawn[place].parent);
				GameObject inst = PoolerManager.Spawn("PlayerGame");
				inst.transform.SetParent(positionsSpawn[place].parent);
				inst.transform.localScale = Vector3.one;
				playerIcone = inst.GetComponent<PlayerGameIcone>();
				playerIcone.SpawnClear();
				playerIcone.gameObject.SetActive(true);
#if UNITY_EDITOR
				playerIcone.gameObject.name = "Player in game " + player.user_id;
#endif
				playerIcone.transform.localPosition = Vector3.one * 1000;
				playerIcone.transform.localScale = Vector3.zero;
				playerIcone.Set(player);
				playerIcone.Show(positionsSpawn[place]);
				//#endif

			}
			else
				playerIcone.Set(player);

			if (oldPlace != null)
				_players.Remove((int)oldPlace);

			_players.Remove(place);
			_players.Add(place, playerIcone);

			//#if UNITY_ANDROID || UNITY_WEBGL || UNITY_IOS
			//			if (player.UserId == UserController.User.Id)
			//				playerIcone.transform.localScale = Vector3.one * 1.85f;
			//			else
			//				playerIcone.transform.localScale = Vector3.one * 1.5f;
			//#endif
			//PlayerPlaces[place].localScale = isMe ? new Vector3(1.2f, 1.2f, 1.2f) : Vector3.one;
			//PlayerPlaces[place].localScale = Vector3.one;
			if (isMe)
			{
				if (ChatButton != null) ChatButton.interactable = true;
			}
		}

		private void Init(Table selectTable)
		{
			this.SelectTable = selectTable;

			_waitingForPlayers.gameObject.SetActive(selectTable.table_player_sessions.Length < selectTable.players_autostart_count && selectTable.table_player_sessions.Length > 0);
			_waitingForPlayers.SetWait();

			ClearTable();
			if (ChatButton != null) ChatButton.interactable = false;

			var existsUsers = _players.Values.ToList<PlayerGameIcone>();

			//посадка игроков за стол
			if (selectTable.table_player_sessions != null && selectTable.table_player_sessions.Length > 0)
			{
				PlayersPositions(selectTable.table_player_sessions);
			}

			SetBank(0);

			SetStateOnTable();
			SetStateButtons(selectTable.has_active_distribution);

			if (selectTable.isAtTheTable)
			{
				GameHelper.GetTablePlayerSessionOptions(selectTable, (options) =>
				{
					_alwaysChipMax = (bool)options.auto_add_to_max_buy_in;
					_gamePanel.GameSession.SettingsStraddal = !(bool)options.skip_straddle;
				});
			}
			else if (selectTable.isExistFreePlace)
			{
				SpawnFreePlaces();
			}

			// Автоматическая посадка
			if (UserController.AutoJoin)
			{
				UserController.AutoJoin = false;
				if (placesFree.Keys.Count <= 0) return;
				int position = UnityEngine.Random.Range(0, placesFree.Count);

				placesFree.ElementAt(position).Value.TouchSitdown();

			}

			CheckActiveDistribution();
		}

		private void UpdateTable(Table selectTable)
		{
			this.SelectTable = selectTable;
			if (ChatButton != null) ChatButton.interactable = false;

			_waitingForPlayers.gameObject.SetActive(selectTable.table_player_sessions.Length < selectTable.players_autostart_count && selectTable.table_player_sessions.Length > 0);
			_waitingForPlayers.SetWait();

			var existsUsers = _players.Values.ToList<PlayerGameIcone>();

			if (selectTable.table_player_sessions != null && selectTable.table_player_sessions.Length > 0)
			{
				PlayersPositions(selectTable.table_player_sessions);

			}
			else
			{
				foreach (var item in _players)
					item.Value.Hide();
				_players.Clear();
				//ClearTable();
			}

			SetStateOnTable();
			SetStateButtons(selectTable.has_active_distribution);

			if (!selectTable.isAtTheTable && selectTable.isExistFreePlace)
			{
				SpawnFreePlaces();
			}
		}

		private void SpawnFreePlaces()
		{
			foreach (var elem in placesFree)
			{
				//elem.Value.gameObject.SetActive(false);
				PoolerManager.Return("GamePlace", elem.Value.gameObject);
			}
			placesFree.Clear();

			for (int i = 0; i < SelectTable.MaxPlayers; i++)
			{
				if (i >= PlayerPlacesTakeSit.Count)
				{
					it.Logger.Log("free place >= PlayerPlacesTakeSit.Count");
					break;
				}

				if (SelectTable.IsFreePlace(i))
				{
					if (!placesFree.ContainsKey(i))
					{
						//var placeFreePanel = Instantiate(GamePlaceUIPrefab, PlayerPlacesTakeSit[i]);
						GameObject inst = PoolerManager.Spawn("GamePlace");
						var placeFreePanel = inst.GetComponent<GamePlaceUI>();
						inst.transform.SetParent(PlayerPlacesTakeSit[i]);
						inst.transform.localScale = Vector3.one;
						RectTransform rt = placeFreePanel.GetComponent<RectTransform>();
						rt.position = Vector3.zero;
						rt.localPosition = Vector3.zero;
						rt.localScale = Vector3.one;
						rt.anchoredPosition = Vector3.zero;
						placesFree.Add(i, placeFreePanel);
#if UNITY_STANDALONE
						PlayerPlacesTakeSit[i].localScale = Vector3.one;
#endif
#if UNITY_ANDROID || UNITY_WEBGL || UNITY_IOS
						placeFreePanel.IsInverce = true;
						//placeFreePanel.transform.localScale = Vector3.one*1.5f;
#endif
						placeFreePanel.Init(i, (place) => { SitDownTable(place + 1); });
					}
					placesFree[i].gameObject.SetActive(true);
				}
			}
		}

		//private void OnApplicationPause(bool pause)
		//{
		//	if (!pause)
		//		FullReloadTable();
		//}

		public void CheckActiveDistribution()
		{
			if (SelectTable.has_active_distribution)
			{
				GameHelper.GetActiveDistributionsData(SelectTable, (data) =>
				{
					Clear(true);
					_isWeatFullLoad = false;
					if (data != null)
					{
						DistributionInit(data.shared_data, data.user_data);
						DistributionProcess();
						foreach (var elem in CardsOnTable)
							elem.InitOnlyVisual(elem.Card);
					}
				});
			}
			else
			{
				Clear(true);
				UpdateStateTable();
				_isWeatFullLoad = false;
			}

			SetBidCount();
		}

		private void CheckAfk(DistributionSharedDataPlayer player)
		{
			if (player.is_resting)
			{
				AfkPopup popup = it.Main.PopupController.Instance.ShowPopup<AfkPopup>(PopupType.Afk);
				popup.Init(SelectTable.MePlayer, (isSuccess) =>
				{
					if (isSuccess)
					{
						ClearIsResting(player.user.id);
						SetStateButtons(true);
						GameHelper.FinishAfk(SelectTable, session => { SetStateButtons(true); },
											s => { SetStateButtons(true); });
					}
					else
					{
						_gamePanel.CloseRequest();
						//GetUpTable();
					}
				});
			}
		}


		private void ClearOldPlacesPlayer(ulong id)
		{
			var oldPlacesPlayers = (from item in _players where item.Value.TablePlayerSession.user_id == id select item.Key).ToList();
			RemoveLeavePlayers(oldPlacesPlayers);
		}

		private int GetRandomPlace()
		{
			if (placesFree.Count == 0) return 0;
			var placeIndex = UnityEngine.Random.Range(0, placesFree.Count - 1);
			return placesFree.ElementAt(placeIndex).Value.Place + 1;
		}

		private void SitDownTable(int place)
		{
			if (!TableManager.CheckAlowedOpenTable())
				return;

			string password = "";

			if (!SelectTable.is_vip)
			{
				TryReserve(place, password);
				return;
			}

			if (SelectTable.is_vip)
			{
#if UNITY_EDITOR
				password = PlayerPrefs.GetString(StringConstants.TablePassword(SelectTable.id), "");
#endif

#if UNITY_STANDALONE && !UNITY_EDITOR
				password = CommandLineController.GetPassword();
#endif

				if (string.IsNullOrEmpty(password) && !SelectTable.isAtTheTable)
				{
					TablePinPopup panel = it.Main.PopupController.Instance.ShowPopup<TablePinPopup>(PopupType.TablePin);
					panel.Set(SelectTable);
					panel.OnConfirm = (pass) =>
					{
						TryReserve(place, pass);
					};
					return;
				}
				TryReserve(place, password);
			}

		}

		private void TryReserve(int place, string password)
		{
			if (SelectTable.can_join_only_with_exit_amount && SelectTable.exit_amount.Value > UserController.User.user_wallet.amount.Value)
			{
				it.Main.PopupController.Instance.ShowPopup<InfoPopup>(PopupType.Info).SetDescriptionString("popup.buyIn.insufficientFunds".Localized());
				return;
			}

			TableApi.ReservatioTable(SelectTable.id, place, password, (result) =>
			{
				if (result.IsSuccess)
				{
					ShowBuyInPopup((buyIn) => SitDown(buyIn, place, password));
				}
				else
				{
					//var data = (ErrorRest)it.Helpers.ParserHelper.Parse(typeof(ErrorRest),		Leguar.TotalJSON.JSON.ParseString(result.ErrorMessage));
					var data = Newtonsoft.Json.JsonConvert.DeserializeObject<ErrorResponse>(result.ErrorMessage);

					if (data.errors[0].id == "active_tables_limit_reached")
					{
						it.Main.PopupController.Instance.ShowPopup(PopupType.TableLimit);
						return;
					}
					if (data.errors[0].id == "reservations_rate_limit")
					{
						var panel = it.Main.PopupController.Instance.ShowPopup<InfoPopup>(PopupType.Info);
						panel.SetDescriptionString("errors.forms.reserveRateLimit".Localized());
						return;
					}


				}

			});
		}

		private void ShowBuyInPopup(Action<decimal> callback)
		{
			_bankContainer.SetActive(false);
			//gameInitManager.BuyInPopup.Show(min, max, callback);

			BuyInPopup popup = it.Main.PopupController.Instance.ShowPopup<BuyInPopup>(PopupType.BuyIn);
			popup.Init(SelectTable, callback);
		}

		private void SitDown(decimal buyIn, int place, string password = "")
		{

			//			if (_selectTable.IsVip)
			//			{
			//#if UNITY_EDITOR
			//				password = PlayerPrefs.GetString("password", "");
			//#elif UNITY_STANDALONE
			//				password = CommandLineController.GetPassword();
			//#endif
			//			}


			if (buyIn == -1)
			{
				TableApi.ReservatioCancelTable(SelectTable.id, place, password, (result) =>
				{

				});
				return;
			}

			GameHelper.SitDownTable(SelectTable, new SitdownInfo(buyIn, place, password), (table) =>
			{
				_bankContainer.SetActive(true);
				StartGame(table);
			}, (error) =>
			{
				try
				{
					//var data = (ErrorRest)it.Helpers.ParserHelper.Parse(typeof(ErrorRest),		Leguar.TotalJSON.JSON.ParseString(error));
					var data = Newtonsoft.Json.JsonConvert.DeserializeObject<ErrorResponse>(error);

					for (int i = 0; i < data.errors.Length; i++)
					{
						if (data.errors[0].id == "active_tables_limit_reached")
						{
							it.Main.PopupController.Instance.ShowPopup(PopupType.TableLimit);
							return;
						}
					}
					for (int i = 0; i < data.errors.Length; i++)
					{
						if (data.errors[0].id == "active_tables_limit_reached")
						{
							it.Main.PopupController.Instance.ShowPopup(PopupType.TableLimit);
							return;
						}
					}

				}
				catch
				{
				};
				_bankContainer.SetActive(true);

			});
		}

		private void SetBank(decimal bank)
		{
			_totalBank.SetValue(string.Format("game.bank.totalPot".Localized(), it.Helpers.Currency.String(bank)), SelectTable, bank);
		}

		private void DistributionInit(DistributionSharedData sharedData, SocketEventDistributionUserData userData)
		{
			_currentSharedData = sharedData;
			_currentUserData = userData;
			_isShowDown = false;

			InitPlayersAtDistribution();
		}

		private void DistributionProcess()
		{

			CheckActiveActionButtons();

			//todo Небольшой костыль из-за пропадания кнопок действия на вебгл
			//DOVirtual.DelayedCall(0.3f, () =>
			//{
			//	CheckActiveActionButtons();
			//});

			ProcessPlayersAtDistribution();
			InitCards(_currentSharedData.shared_cards, _currentSharedData.WinCombinations);
			ProcessDiller();

			if (!_currentSharedData.IsFinish) SetStateOnTable();
			else
			{
				CheckOrFoldContainer.SetActive(false);
				//RightContainer.SetActive(false);
			}

			var isMeActive = _currentSharedData.IsMeActive;

			if (!_gamePanel.GameSession.ClearBank)
			{
				SetBank(_currentSharedData.GetTotalBank());
				_bankSplit.TableUpdate();
			}
			try
			{
				ParseSessionEvents(_currentSharedData.stages);
			}
			catch (System.Exception ex)
			{
				it.Logger.LogError($"{ex.Message} - {ex.StackTrace}");
			}
			if (isMeActive)
			{
				var betInfo = _currentSharedData.BetInfo;
				var isMeAllIn = betInfo.IsAllIn;
				SetBidCount();

				if (CheckOrFoldToggle.isOn)
				{
					CheckOrFoldToggle.isOn = false;
					if (betInfo.CountCall <= 0) _actionsPanel.CheckButton();
					else _actionsPanel.FoldButton();
					_actionsPanel.gameObject.SetActive(false);

				}

			}
			else
			{
				ToggleCheckOrFoldText.text =
						_currentSharedData.IsEmptyBetAndNotFisrtWord() || !_currentSharedData.IsAvailableCheck()
								? "game.checkBox.fold".Localized()
								: "game.checkBox.checkFold".Localized();
			}

		}

		private void CheckActiveActionButtons()
		{
			if (_currentSharedData.IsMeActive && !_currentSharedData.IsStraddle())
			{
				if (!SelectTable.is_all_or_nothing)
				{
					_actionsPanel.gameObject.SetActive(true);
					_actionsPanel.Distribution(SelectTable, _currentSharedData, _currentUserData);
					_actionsPanel.gameObject.SetActive(!CheckOrFoldToggle.isOn && !_currentSharedData.IsDealerChoiseStage);
#if UNITY_STANDALONE
					if (!CheckOrFoldToggle.isOn)
						StandaloneController.Instance.FocusWindow();
#endif
				}
				else
				{
					_dynamitePanel.gameObject.SetActive(true);
					_dynamitePanel.UpdateDistribute();
				}
			}
			else
			{
				_actionsPanel.gameObject.SetActive(false);
				_dynamitePanel.gameObject.SetActive(false);
			}
		}

		Tween _chipMove;

		private async void ProcessDiller(TablePlayerSession[] players = null)
		{
			await ProcessDillerAsync(players);
		}

		private async UniTask ProcessDillerAsync(TablePlayerSession[] players = null)
		{
			bool wait = true;
			try
			{
				if (players != null && players.Length <= 1)
				{
					_dealerChip.gameObject.SetActive(false);
					return;
				}
				if (_currentSharedData == null || _currentSharedData.players.Count <= 1)
				{
					_dealerChip.gameObject.SetActive(false);
					return;
				}

				var dillerItem = _currentSharedData.players.Find(x => x.role == DistributionSharedDataPlayer.RoleDealer
				|| x.role == DistributionSharedDataPlayer.RoleDealerSb
				|| x.role == DistributionSharedDataPlayer.RoleDealerBb
				|| x.role == DistributionSharedDataPlayer.RoleDealerBbp);

				if (dillerItem == null)
				{
					_dealerChip.gameObject.SetActive(false);
					return;
				}
				int key = -1;
				foreach (var elem in _players.Keys)
					if (_players[elem].UserId == dillerItem.user.id)
						key = elem;

				if (key == -1)
				{
					_dealerChip.gameObject.SetActive(false);
					return;
				}
				Image img = _dealers[key];
				img.enabled = false;
				img.gameObject.SetActive(true);
				_dealerChip.gameObject.SetActive(true);

				_dealerChip.rectTransform.SetParent(img.rectTransform);
				_dealerChip.rectTransform.anchorMin = Vector2.zero;
				_dealerChip.rectTransform.anchorMax = Vector2.one;
				_dealerChip.rectTransform.sizeDelta = Vector2.zero;

				if (_chipMove != null && _chipMove.active)
					_chipMove.Kill();


				_chipMove = _dealerChip.rectTransform.DOAnchorPos(Vector2.zero, 0.2f).OnComplete(() =>
				{
					wait = false;
				});
			}
			catch (Exception ex)
			{
				it.Logger.LogError($"Distribution {_currentSharedData.id} Ошибка в обработчике диллера " + ex.Message);
			}
			finally
			{
				wait = false;
			}

			await UniTask.WaitWhile(() => wait);

			//while (wait) yield return null;

		}

		private void SetBidCount()
		{
			if (_currentSharedData == null || !_currentSharedData.IsMeActive) return;

			float maxValue = 1;
			float minValue = 1;
			if (_currentSharedData != null && _currentSharedData.MePlayer != null)
			{
				if (_currentSharedData.MaxRaise != -1 && _currentSharedData.MinRaise != -1)
				{
					maxValue = (float)_currentSharedData.MaxRaise;
					minValue = (float)_currentSharedData.MinRaise;
				}
				else
				{
					it.Logger.Log($"raise maxValue or maxValue not found");
				}
			}

		}

		private void InitPlayersAtDistribution()
		{

			if (_currentSharedData == null) return;


			foreach (var playerPanel in _players)
			{
				bool isLeaveAtTable = true;
				foreach (var player in _currentSharedData.players)
				{
					if (playerPanel.Value.TablePlayerSession.user_id == player.user.id)
					{
						isLeaveAtTable = false;
						bool isMe = playerPanel.Value.TablePlayerSession.user_id == GameHelper.UserInfo.id;
						playerPanel.Value.SetDistributive(player, _currentSharedData, _currentSharedData.active_event,
								isMe && _currentUserData != null ? _currentUserData.cards : player.cards,
								_currentSharedData.IsPreFlop, _isShowDown);
#if UNITY_WEBGL || UNITY_ANDROID || UNITY_IOS
						if (!SelectTable.is_all_or_nothing)
						{
							RectTransform rt = _bets[0].GetComponent<RectTransform>();
							rt.anchoredPosition = SelectTable.isAtTheTable ? new Vector2(-230, -30) : Vector2.zero;
						}
#endif
						//if (isMe)
						//if (isMe)
						//	_addTimePanel.SetData(_currentSharedData.IsMeActive && _currentSharedData.ActiveEvent.TimeBankUsageReady && _currentSharedData.ActiveEvent.IsMeActive, _selectTable, _currentSharedData, _currentSharedData.ActiveEvent);

						//InitBetAndRole(player, _currentSharedData.IsPreFlop, playerPanel.Key);
					}
				}

				if (isLeaveAtTable)//if player leave at table need clear card and state
				{
					//if(playerPanel != null)
					playerPanel.Value.Clear();
				}
			}
		}

		private void ProcessPlayersAtDistribution()
		{
			if (_currentSharedData == null) return;

			foreach (var playerPanel in _players)
			{
				try
				{
					if (playerPanel.Value == null)
					{
						it.Logger.LogError("Error process ProcessPlayersAtDistribution player is null in position " + playerPanel.Key);
						continue;
					}
				}
				catch { }

				bool isLeaveAtTable = true;
				try
				{
					foreach (var player in _currentSharedData.players)
					{
						if (playerPanel.Value.TablePlayerSession.user_id == player.user.id)
						{
							isLeaveAtTable = false;
							bool isMe = playerPanel.Value.TablePlayerSession.user_id == GameHelper.UserInfo.id;
							//playerPanel.Value.SetDistributive(player, _currentSharedData, _currentSharedData.ActiveEvent,
							//		isMe && _currentUserData != null ? _currentUserData.Cards : player.Cards,
							//		_currentSharedData.IsPreFlop, _isShowDown);

							try
							{
								playerPanel.Value.ProcessDistributive();
							}
							catch (Exception ex)
							{
								it.Logger.LogError("Error process player distributions " + ex.Message + " : " + ex.StackTrace);
							}

							try
							{
								//if (isMe)
								if (isMe && !SelectTable.is_all_or_nothing && !_currentSharedData.IsStraddle())
									_addTimePanel.SetData(_currentSharedData.IsMeActive && _currentSharedData.active_event.TimeBankUsageReady && _currentUserData.total_time_bank > 0 && _currentSharedData.active_event.IsMeActive, SelectTable, _currentSharedData, _currentSharedData.active_event);
							}
							catch
							{
								Debug.Log("Error");
							}
							if (isMe && player.state.slug == "fold" && _visibleClickToShow && _currentSharedData != null && _currentUserData != null)
							{
								_clickToShowPanel.gameObject.SetActive(true);
								_clickToShowPanel.SetCardPanel(SelectTable, (ulong)_currentSharedData.id, _currentUserData.cards);
							}

							if (isMe)
								CheckAfk(player);

							InitBetAndRole(player, _currentSharedData.IsPreFlop, playerPanel.Key);
						}
					}
				}
				catch (Exception ex)
				{
					it.Logger.LogError("Error process ProcessPlayersAtDistribution " + ex.Message + " : " + ex.StackTrace);
				}


				if (isLeaveAtTable)
				{
					//if(playerPanel != null)
					playerPanel.Value.Clear();
				}
			}

		}

		private void RemoveLeavePlayers(List<int> leavePlayers)
		{
			foreach (var key in leavePlayers)
			{
				_players[key].transform.parent.localScale = Vector3.one;
				PoolerManager.Return("PlayerGame", _players[key].gameObject);
				//DestroyImmediate(_players[key].gameObject);
				_players.Remove(key);
			}
		}

		private void InitCombinations(DistributionPlayerCombination[] combinations)
		{
			if (combinations.Length > 0)
			{
				//ContainerCombination.SetActive(true);
				bool hiCombination = false;
				bool lowCombination = false;

				foreach (var combination in combinations)
				{

					if (!lowCombination
					&& (GameType.OmahaLow5 == (GameType)GameManager.Instance.Table.game_rule_id
							|| GameType.OmahaLow4 == (GameType)GameManager.Instance.Table.game_rule_id)
					&& combination.category == "low")
					{
						lowCombination = true;
						var t = combination.game_card_combination.title;
						DG.Tweening.DOVirtual.DelayedCall(2, () =>
						{
							//NameCombination.text = t;
						});
					}


					if (!hiCombination && (combination.category == null || combination.category != "low"))
					{
						hiCombination = true;
						//NameCombination.text = combination.game_card_combination.title;
						//break;
					}
				}
			}
			else
			{
				//ContainerCombination.SetActive(false);
			}
		}

		private void SetStateOnTable()
		{
			bool isAtTable = SelectTable.isAtTheTable;
			bool isNeedAcceptBb = !SelectTable.InGame;

			bool isShowButtons = isAtTable && !isNeedAcceptBb;
			if (!isAtTable && !SelectTable.isExistFreePlace)
			{
				SetMessage(true, "No free places");
			}
			else if (isAtTable && SelectTable.current_players_count < SelectTable.players_autostart_count)
			{
				SetMessage(true,
						$"{SelectTable.players_autostart_count - SelectTable.current_players_count} more players are required to start");
			}
			else
			{
				//RightContainer.SetActive(isShowButtons);
			}

			//WaitBlindsContainer.SetActive(isAtTable && currentSharedData.isWaitDistribution);
			WaitBlindsContainer.SetActive(isAtTable && _currentSharedData is { IsWaitDistribution: true } && isNeedAcceptBb);
			if (isAtTable && _currentSharedData is { IsWaitDistribution: false })
			{
				WaitBlindsToggle.isOn = true;
			}

			bool isShowSitDown = !isAtTable && SelectTable.isExistFreePlace;
			bool meIsFold = _currentSharedData != null && _currentSharedData.MePlayer != null && _currentSharedData.MePlayer.state.slug == "fold";
			bool isCardPhase = _currentSharedData != null && _currentSharedData.IsCardsState();

			CheckOrFoldContainer.SetActive(!isShowSitDown && !SelectTable.is_all_or_nothing && !meIsFold && isCardPhase);

			if (SelectTable.isAtTheTable)
			{
				if (_currentSharedData is { IsWaitDistribution: true })
				{
					SetMessage(true, "Wait next distribution");
				}
				else
				{
					var isMeActive = _currentSharedData is { IsMeActive: true };
					SetStateIsMe(isMeActive);
				}
			}
		}

		private void SetStateIsMe(bool isMeActive)
		{
			bool meIsFold = _currentSharedData != null && _currentSharedData.MePlayer != null && _currentSharedData.MePlayer.state.slug == "fold";
			bool isCardPhase = _currentSharedData != null && _currentSharedData.IsCardsState();

			CheckOrFoldContainer.SetActive(!isMeActive && !SelectTable.is_all_or_nothing && !meIsFold && isCardPhase);

			//RightContainer.SetActive(isMeActive && _selectTable.CurrentPlayersCount >= _selectTable.PlayersAutostartCount);
		}

		private void SetMessage(bool isShow, string message = "")
		{
			//RightContainer.SetActive(!isShow);
		}

		private void SetStateButtons(bool isActiveState)
		{
			_actionsPanel.SetStateButtons(isActiveState && !CheckOrFoldToggle.isOn && (_currentSharedData == null || !_currentSharedData.IsDealerChoiseStage));
			_dynamitePanel.SetStateButtons(isActiveState);
		}

		private async void ParseEvents(List<DistributionEvent> events, UnityEngine.Events.UnityAction onComplete, System.Threading.CancellationToken cancelledToken)
		{
			//float animBetsToTable = 0;
			_existsNewEvents = (events.Count <= 0);
			foreach (var dEvent in events)
			{
				if (_lastEvent >= dEvent.id)
				{
					it.Logger.LogWarning($"Dublicate event {dEvent.id} {dEvent.distribution_event_type.title}");
					continue;
				}

				_lastEvent = dEvent.id;
				_existsNewEvents = true;

				//try
				//{
				//if (dEvent.BankAmountDelta > 0)
				//	animBetsToTable = 0.8f;

				var stage = _currentSharedData.stages.Find(x => x.id == dEvent.distribution_stage_id);

				it.Logger.Log("Process event " + dEvent.distribution_event_type.slug);

				//com.ootii.Messages.MessageDispatcher.SendMessage(this, EventsConstants.GameEvent, dEvent.distribution_event_type.slug, 0);

				switch (dEvent.distribution_event_type.slug)
				{
					case "timebank":
						{
							foreach (var elem in _players.Values)
								if (elem.UserId == dEvent.user_id)
									elem.SetUseTimeBank();

							ActionController.Instance.Emit("timeBankUpdate");
							break;
						}
					case "all-in":
						{
							foreach (var elem in _players.Values)
								if (elem.UserId == dEvent.user_id)
									elem.ShowAction();
							await PlayAnimationBetOnTable(dEvent, cancelledToken);
							if (cancelledToken.IsCancellationRequested) return;

							break;
						}
					case "big-blind":
						{
							await PlayAnimationBetOnTable(dEvent, cancelledToken);
							if (cancelledToken.IsCancellationRequested) return;
							break;
						}
					case "big-blind_with_all-in":
						{
							await PlayAnimationBetOnTable(dEvent, cancelledToken);
							if (cancelledToken.IsCancellationRequested) return;
							break;
						}
					case "call":
						{
							foreach (var elem in _players.Values)
								if (elem.UserId == dEvent.user_id)
									elem.ShowAction();
							await PlayAnimationBetOnTable(dEvent, cancelledToken);
							if (cancelledToken.IsCancellationRequested) return;
							break;
						}
					case "bet":
						{
							foreach (var elem in _players.Values)
								if (elem.UserId == dEvent.user_id)
									elem.ShowAction();
							await PlayAnimationBetOnTable(dEvent, cancelledToken);
							if (cancelledToken.IsCancellationRequested) return;
							break;
						}
					case "check":
						{
							foreach (var elem in _players.Values)
								if (elem.UserId == dEvent.user_id)
									elem.ShowAction();

							await PlayAnimationBetOnTable(dEvent, cancelledToken);
							if (cancelledToken.IsCancellationRequested) return;
							break;
						}
					case "distribution_finished":
						{
							it.Logger.Log("Process event distribution_finished step 1");
							//time = animBetsToTable;
							_visibleClickToShow = false;

							//_gamePanel.GameSession.CardTableOut = false;

							await DistributionnFinish(dEvent, cancelledToken);
							it.Logger.Log("Process event distribution_finished step 2");
							it.Logger.Log("Process event distribution_finished step 2 " + cancelledToken.IsCancellationRequested);
							if (cancelledToken.IsCancellationRequested) return;
							it.Logger.Log("Process event distribution_finished step 3");

							if (HandsBeforeUpdate != null && HandsBeforeUpdate <= 0)
							{
								GamePanel.CloseRequest();
								//Garilla.Update.UpdateController.Instance.ExistsCriticleUpdate();
							}
							else
								TableApi.ReadyToPlay(SelectTable.id, null);

							it.Logger.Log("Process event distribution_finished step 4");
							_isGame = false;
							if (_waitAddChipPopup)
								ShowAddChipIfNeed();
							//CheckAutoMaxIncrement();
							break;
						}
					case "distribution_started":
						{
							if (_totalPot != null)
								_totalPot.ResetPosition();
							_isGame = true;
							if (HandsBeforeUpdate != null && _updatePanel != null)
							{

								_updatePanel.Show((int)HandsBeforeUpdate);
								HandsBeforeUpdate--;
							}

							_clickToShowPanel.Clear();
							_clickToShowPanel.gameObject.SetActive(false);
							if (SelectTable.is_all_or_nothing)
								_allInWindow.DistributionStart();
							_gamePanel.GameSession.PreflopBetsOut = _sharedData.distribution.IsPreFlop;

							foreach (var pl in _players.Values)
							{
								pl.Clear();
								pl.VisibleDropCards = false;
								pl.WaitMovePlayerAnimation = false;
							}

							SetBank(0);
							_bankSplit.Clear();
							_gamePanel.GameSession.ClearBank = false;
							_gamePanel.GameSession.CardTableOut = false;
							_gamePanel.GameSession.CardOutPlayers.Clear();

							await ProcessDillerAsync();
							if (cancelledToken.IsCancellationRequested) return;

							ClearCards();

							break;
						}
					case "fold":
						{
							PlayAnimationFold(dEvent);
							break;
						}
					case "playing_card_given_to_player":
						{
							break;
						}
					case "raise":
						{
							foreach (var elem in _players.Values)
								if (elem.UserId == dEvent.user_id)
									elem.ShowAction();
							await PlayAnimationBetOnTable(dEvent, cancelledToken);
							if (cancelledToken.IsCancellationRequested) return;
							break;
						}
					case "re-raise":
						{
							foreach (var elem in _players.Values)
								if (elem.UserId == dEvent.user_id)
									elem.ShowAction();
							await PlayAnimationBetOnTable(dEvent, cancelledToken);
							if (cancelledToken.IsCancellationRequested) return;
							break;
						}
					//case "rake-payout":
					//	{
					//		//VisibleWinChips(distribEvent);

					//		foreach (var pl in _players.Values)
					//		{

					//			if (pl.UserId == dEvent.user_id)
					//			{
					//				pl.SetRake((double)dEvent.bank_amount_delta);
					//			}

					//		}

					//		break;
					//	}
					case "refund-of-funds":
						{
							break;
						}
					case "shared_playing_card_issued":
						{
							if (!_sharedData.events.Exists(x => x.distribution_event_type.slug == "stage_finished"))
							{
								Hashtable hash = new Hashtable();
								hash.Add("event", dEvent);
								hash.Add("cards", _sharedData.distribution.shared_cards);

								_gamePanel.AnimationsManager.AddAnimation(dEvent.id, TableAnimationsType.CardToTable, hash);
							}

							break;
						}
					case "small-blind":
						{
							await PlayAnimationBetOnTable(dEvent, cancelledToken);
							if (cancelledToken.IsCancellationRequested) return;
							break;
						}
					case "small-blind_with_all-in":
						{
							await PlayAnimationBetOnTable(dEvent, cancelledToken);
							if (cancelledToken.IsCancellationRequested) return;
							break;
						}
					case "stage_finished":
						{
							bool existsBingo = false;
							if (stage.distribution_stage_type.slug == "blinds" || stage.distribution_stage_type.slug == "distribution") continue;
							//time = animBetsToTable;

							foreach (var pl in _players.Values)
								if (pl.ProcessBingo())
									existsBingo = true;

							foreach (var pl in _players.Values)
								pl.OpenIfNeed();

							if (existsBingo)
								await UniTask.Delay(5000);

							if (cancelledToken.IsCancellationRequested) return;

							//yield return new WaitForSeconds(5f);

							if (SelectTable.is_all_or_nothing)
							{
								if (!_sharedData.distribution.IsPreFlop)
									_allInWindow.ClearCompare();
							}

							//animBetsToTable = 0;

							_gamePanel.GameSession.PreflopBetsOut = _sharedData.distribution.IsPreFlop;

							if (_sharedData != null && _sharedData.distribution.ExistsPreflop())
								await PlayAnimationChipToBank(dEvent);

							if (cancelledToken.IsCancellationRequested) return;
							//time += 0.7f;

							if (_sharedData.events.Exists(x => x.distribution_event_type.slug == "shared_playing_card_issued"))
							{
								//double deltaTime = MathF.Max(time, (float)(_gamePanel.GameSession.CardOnTableTime - Time.timeAsDouble));
								//double waitTime = 0;

								//if (deltaTime > 0)
								//{
								//	waitTime = deltaTime + 1f;
								//	_gamePanel.GameSession.CardOnTableTime += 1f;
								//}
								//else
								//{
								//	waitTime = 1;
								//	_gamePanel.GameSession.CardOnTableTime = Time.timeAsDouble + 1;
								//}

								List<DistributionCard> cards = new List<DistributionCard>(_sharedData.distribution.shared_cards);

								//time += 0.4f + _sharedData.Events.Count;


								foreach (var elem in _sharedData.events)
								{
									var evnt = elem;

									if (evnt.distribution_event_type.slug == "shared_playing_card_issued")
									{
										//	DOVirtual.DelayedCall((float)waitTime, () =>
										//	{
										Hashtable hash = new Hashtable();
										hash.Add("event", evnt);
										hash.Add("cards", cards);

										_gamePanel.AnimationsManager.AddAnimation(elem.id, TableAnimationsType.CardToTable, hash);
										//});
									}
								}
								await UniTask.Delay(500);
								if (cancelledToken.IsCancellationRequested) return;
								//yield return new WaitForSeconds(0.5f);
							}

							break;
						}
					case "stage_started":
						{
							if (_sharedData.distribution.stages.Exists(x => x.distribution_stage_type.slug == "flop"))
							{
								foreach (var elem in _players.Values)
									elem.StraddlePanelEnable(false);
							}
							else
								foreach (var elem in _players.Values)
								{
									for (int i = 0; i < _sharedData.distribution.players.Count; i++)
										if (_sharedData.distribution.players[i].user.id == elem.UserId)
											elem.StraddlePanelEnable(_sharedData.distribution.players[i].roles.Contains("last-straddle"));
								}

							if (stage.distribution_stage_type.slug == "distribution")
							{
								ClearCards();
								if (_currentUserData != null && _currentUserData.cards != null)
								{
									Hashtable hash = new Hashtable();
									hash.Add("players", _sharedData.distribution.players);
									hash.Add("cards", _currentUserData.cards);

									bool waitPlayers = true;

									_gamePanel.AnimationsManager.AddAnimation(dEvent.id, TableAnimationsType.CardsToPlayer, hash, (UnityEngine.Events.UnityAction)(() =>
									{
										if (base.SelectTable.is_all_or_nothing)
											_allInWindow.CheckCompare();
										waitPlayers = false;

									}));

									await UniTask.WaitWhile(() => waitPlayers);
									if (cancelledToken.IsCancellationRequested) return;

									//while (waitPlayers)
									//	yield return null;

								}
							}


							foreach (var pl in Players.Values)
							{
								pl.GetCombinationsActiveStage();
							}

							if (stage.distribution_stage_type.slug == "showdown")
								foreach (var pl in Players.Values)
									pl.WaitToClickToShowShow = true;

							GamePanel.GameSession.IsFullBank = false;
							if (_currentSharedData.IsShowDown() && !_currentSharedData.ExistsRiver())
								Showdown();
							break;
						}
					case "transfer-of-winnings":
						{
							//VisibleWinChips(distribEvent);
							break;
						}
					case "user_action_request":
						{
							if (stage.distribution_stage_type.slug == "straddle")
							{
								//foreach (var elem in _players.Values)
								//	if (elem.UserId == dEvent.userId)
								//		elem.StraddlePanelEnable(true);
								if (dEvent.user_id == UserController.User.id)
									if (_straddlePanel != null)
										_straddlePanel.SetVisibleButton(true);

								break;
							}
							if (_straddlePanel != null)
								_straddlePanel.SetVisibleButton(false);

							if (_currentSharedData.GetActiveStage().distribution_stage_type.slug == "dealer_choice")
								DealerChoiceMenu();
							break;
						}
				}
				//}
				//catch (System.Exception ex)
				//{
				//	it.Logger.Log($"Error event {dEvent.distribution_event_type.slug} process " + ex.Message + " : " + ex.StackTrace);
				//}

			}

			onComplete?.Invoke();

		}

		private void InitCards(List<DistributionCard> sharedCards, List<DistributionPlayerCombination> combinations)
		{
			//ClearCards();

			if (AnimationToTableWait > 0) return;
			if (_gamePanel.GameSession.CardOnTableTime - Time.timeAsDouble > 0) return;
			if (_gamePanel.GameSession.CardTableOut) return;
			if (sharedCards == null) return;

			for (int i = 0; i < sharedCards.Count; i++)
			{
				if (i >= CardPlaces.Count)
				{
					it.Logger.Log("i >= CardPlaces.Count");
					break;
				}
				var card = sharedCards[i];
				var cardUi = CardsOnTable.Find(x => x.Card.id == card.id);
				int targetPlace = Mathf.Min(CardsOnTable.Count, CardPlaces.Count - 1);

				if (cardUi == null)
				{
					//var inst = Instantiate(GameCardUIPrefab.gameObject, CardPlaces[CardsOnTable.Count]);
					GameObject newInstanceCard = PoolerManager.Spawn("GameCard");
					newInstanceCard.transform.SetParent(CardPlaces[targetPlace]);
					newInstanceCard.transform.localScale = Vector3.one;
					cardUi = newInstanceCard.GetComponent<GameCardUI>();
					RectTransform cardRect = newInstanceCard.GetComponent<RectTransform>();
					cardRect.anchorMin = Vector2.zero;
					cardRect.anchorMax = Vector2.one;
					cardRect.sizeDelta = Vector2.zero;
					cardRect.localPosition = Vector2.zero;
					cardRect.anchoredPosition = Vector2.zero;
					cardRect.localScale = Vector3.one;
					cardRect.localRotation = Quaternion.identity;
					cardUi.Init(card);
					cardUi.SetOpenState();
					cardUi.IsOnTable = true;
					//cardItem.MoveShadow();
					cardUi.Shadow.color = Color.white;
					CardsOnTable.Add(cardUi);
				}
				cardUi.OnIsMoveUp = (isUp) =>
				{
					if (isUp)
						_gamePanel.GameSession.ShowWinCardTableTime = Time.timeAsDouble;

					if (isUp && _totalPot != null)
						_totalPot.MoveUp();
				};
				//cardItem.IsEmitMoveUpEvent = true;
				//cardItem.Init(combinations);

			}
			//if (_currentSharedData != null && !_gamePanel.GameSession.WainTableShowCombintaion)
			//	ShowCardsCombinations();
		}

		private void ShowCardsCombinations(List<DistributionPlayerCombination> combinations, List<DistributionCard> shared_cards)
		{
			foreach (var cardPanel in CardsOnTable)
			{
				cardPanel.ApplyCombinationOnly(combinations, combinations, true, false);
			}
		}
		private void ShowCardsCombinations(List<DistributionPlayerCombination> combination)
		{
			for (int i = 0; i < _currentSharedData.shared_cards.Count; i++)
			{
				var card = _currentSharedData.shared_cards[i];
				var cardItem = CardsOnTable.Find(x => x.Card.id == card.id);
				//cardItem.IsEmitMoveUpEvent = true;
				cardItem.Init(combination, combination);
			}
			//_winCombinations.Clear();
		}
		private void ShowCardsCombinations()
		{
			for (int i = 0; i < _currentSharedData.shared_cards.Count; i++)
			{
				var card = _currentSharedData.shared_cards[i];
				var cardItem = CardsOnTable.Find(x => x.Card.id == card.id);
				//cardItem.IsEmitMoveUpEvent = true;
				cardItem.Init(_winCombinations, _winCombinations);
			}
			_winCombinations.Clear();
			//_gamePanel.GameSession.WainTableShowCombintaion = false;
		}

		private async UniTask PlayAnimationChipToBank(DistributionEvent ent)
		{
			bool wait = false;
			for (int i = 0; i < _bets.Count; i++)
			{
				int index = i;
				if (_bets[i].Value > 0)
				{
					wait = true;
					Hashtable hash = new Hashtable();
					hash.Add("bets", _bets[i]);
					_gamePanel.AnimationsManager.AddAnimation(ulong.MaxValue, TableAnimationsType.PlayerChipToBank, hash, () =>
					{
						wait = false;

					});
				}
			}

			await UniTask.WaitWhile(() => wait);

			//while (wait)
			//	yield return null;

			GamePanel.GameSession.IsFullBank = true;
			if (!_gamePanel.GameSession.ClearBank)
			{
				SetBank(_currentSharedData.GetTotalBank());
				_bankSplit.TableUpdate();
			}
			//GamePanel.GameSession.IsFullBank = false;
		}

		private void PlayAnimationFold(DistributionEvent ent)
		{
			if (!_gamePanel.GameSession.CardOutPlayers.Contains(ent.user_id.Value))
				_gamePanel.GameSession.CardOutPlayers.Add(ent.user_id.Value);
			if (ent.user_id == UserController.User.id)
			{
				_visibleClickToShow = true;
				_gamePanel.GameSession.TimeMyFold = Time.timeAsDouble;
				_gamePanel.AnimationsManager.AddAnimation(ent.id, TableAnimationsType.CardToFold, new Hashtable());
			}
			else
			{
				foreach (var pl in Players.Values)
				{
					if (pl.UserId == ent.user_id)
					{
						Hashtable hash = new Hashtable();
						hash.Add("player", pl);
						_gamePanel.AnimationsManager.AddAnimation(ent.id, TableAnimationsType.CardPlayerToOut, hash);
					}
				}
			}
		}

		/// <summary>
		/// Фишки от играков летят на стол
		/// </summary>
		/// <param name="ent"></param>
		private async UniTask PlayAnimationBetOnTable(DistributionEvent ent, System.Threading.CancellationToken cancelledToken)
		{
			if (ent.bank_amount_delta == 0) return;

			Hashtable hash = new Hashtable();
			hash.Add("event", ent);
			hash.Add("table", SelectTable);

			bool wait = true;

			_gamePanel.AnimationsManager.AddAnimation(ulong.MaxValue, TableAnimationsType.PlayerBetsOnTable, hash, (() =>
			{
				wait = false;
			}));

			await UniTask.WaitWhile(() => wait, PlayerLoopTiming.Update, cancelledToken);

		}

		private async UniTask ShowOneGroupCombinations(List<DistributionPlayerCombination> combinations, System.Threading.CancellationToken cancelledToke)
		{
			ShowCardsCombinations(combinations, _sharedData.distribution.shared_cards);
			//await Task.Delay(500);

			foreach (var pl in _currentSharedData.players)
			{
				foreach (var keyP in Players.Keys)
					if (pl.user.id == Players[keyP].UserId)
						Players[keyP].ShowCardsCombinations(combinations);
			}
			await UniTask.Delay(300);

			if (cancelledToke.IsCancellationRequested) return;

			foreach (var pl in _currentSharedData.players)
			{
				foreach (var keyP in Players.Keys)
					if (pl.user.id == Players[keyP].UserId)
					{
						try
						{
							Players[keyP].ShowCombinations(combinations);
						}
						catch (System.Exception ex)
						{
							it.Logger.LogError("[Exceprion] Ошибка показа выйграшной комбинации " + ex.Message + " " + ex.StackTrace);
						}
					}
			}
			await UniTask.Delay(1000);
			if (cancelledToke.IsCancellationRequested) return;

			foreach (var keyP in Players.Keys)
				Players[keyP].NoWin();

		}

		private async UniTask DistributionnFinish(DistributionEvent ent, System.Threading.CancellationToken cancelledToken)
		{
			_gamePanel.GameSession.CardTableOut = true;
			_gamePanel.GameSession.ClearBank = false;
			ClearCarsDrop();
			UserController.Instance.GetUserProfile();

			_gamePanel.GameSession.WainTableShowCombintaion = true;
			_gamePanel.GameSession.WainPlayerCombitaions = true;
			_gamePanel.GameSession.WainWinFlag = true;

			if (_currentSharedData.ExistsShowDown())
			{
				var allWinCombinations = _currentSharedData.WinCombinations;

				List<DistributionPlayerCombinationGroup> _combGroup = DistributionPlayerCombinationGroup.CombinationsGroup(allWinCombinations);

				_gamePanel.GameSession.WainPlayerCombitaions = false;
				_gamePanel.GameSession.WainWinFlag = false;
				foreach (var cmbGroup in _combGroup)
				{

					if (cmbGroup.Hight.Count > 0)
					{
						await ShowOneGroupCombinations(cmbGroup.Hight, cancelledToken);
					}
					if (cmbGroup.Low.Count > 0)
					{
						await ShowOneGroupCombinations(cmbGroup.Low, cancelledToken);
					}

				}

			}
			//await Task.Delay(4000);
			_gamePanel.GameSession.WainTableShowCombintaion = false;
			// поднимаем карты на столе
			//Поднимает карты на в руке
			//if (_currentSharedData.ExistsShowDown())
			//{
			//	it.Logger.Log("DistributionnFinish 6");
			//	ShowCardsCombinations();

			//	it.Logger.Log("DistributionnFinish 7");
			//	await Task.Delay(500);
			//	_gamePanel.GameSession.WainPlayerCombitaions = false;
			//	foreach (var pl in _currentSharedData.Players)
			//	{
			//		foreach (var keyP in Players.Keys)
			//			if (pl.User.Id == Players[keyP].UserId)
			//				Players[keyP].ShowCardsCombinations();
			//	}
			//}
			//it.Logger.Log("DistributionnFinish 8");
			////CheckAutoMaxIncrement();

			//it.Logger.Log("DistributionnFinish 9");
			//await Task.Delay(1000);
			//it.Logger.Log("DistributionnFinish 10");
			//// Показываем плашку WIN
			//_gamePanel.GameSession.WainWinFlag = false;
			//foreach (var pl in _currentSharedData.Players)
			//{
			//	foreach (var keyP in Players.Keys)
			//		if (pl.User.Id == Players[keyP].UserId)
			//			Players[keyP].ShowCombitaions();
			//}

			Hashtable hash = new Hashtable();
			hash.Add("events", _sharedData.events);

			SetBank(0);
			_bankSplit.Clear();

			_gamePanel.GameSession.ClearBank = true;
			_gamePanel.AnimationsManager.AddAnimation(ulong.MaxValue, TableAnimationsType.BankToPlayers, hash);

			//bool existsLowCombination = false;
			//foreach (var pl in _currentSharedData.Players)
			//	foreach (var key in Players.Keys)
			//		if (pl.User.Id == Players[key].UserId && Players[key].ExistsLowCombinations())
			//			existsLowCombination = true;

			//it.Logger.Log("DistributionnFinish 15");
			//if (existsLowCombination)
			//	await Task.Delay(3000);

			if (!SelectTable.is_all_or_nothing)
				if (SelectTable.isAtTheTable && _currentSharedData.IsFinish) //&& sharedData.MePlayer != null)
				{
					if (_currentSharedData.IsLostMoney)
					{
						//StartCoroutine(OpenInsufficientBalanceCor());
					}
				}


			if (cancelledToken.IsCancellationRequested) return;

			// Все карты удетают со стола
			await UniTask.Delay(1000, false, PlayerLoopTiming.Update, cancelledToken);

			if (cancelledToken.IsCancellationRequested) return;

			if (CardsOnTable.Count > 0)
			{
				_totalBank.GetComponent<TotalPot>().MoveDown();
				_gamePanel.AnimationsManager.AddAnimation(ulong.MaxValue, TableAnimationsType.CardFromTableToOut, new Hashtable());

				foreach (var key in Players.Keys)
					Players[key].NoWin();
			}

			_clickToShowPanel.Clear();
			_clickToShowPanel.gameObject.SetActive(false);

			if (cancelledToken.IsCancellationRequested) return;

			// Удаляем карты из руки
			foreach (var pl in _players.Values)
			{
				pl.ClearCombinations();
				if (pl.Cards.Count <= 0) continue;
				{
					DOVirtual.DelayedCall((pl.VisibleDropCards ? 0.5f : 0), () =>
					{
						Hashtable hashCardOut = new Hashtable();
						hashCardOut.Add("player", pl);
						//foreach (var key in Players.Values)
						if (!_gamePanel.GameSession.CardOutPlayers.Contains(pl.UserId))
							_gamePanel.GameSession.CardOutPlayers.Add(pl.UserId);
						_gamePanel.AnimationsManager.AddAnimation(ulong.MaxValue, TableAnimationsType.CardPlayerToOut, hashCardOut);

						foreach (var key in Players.Keys)
							Players[key].NoWin();
					});
					pl.VisibleDropCards = false;
				}
			}

			// показываем пресет смайлов
			_gamePanel.ChatManager.ShowMySmilePresets();

			//yield return new WaitForSeconds(1f);

			foreach (var key in _players.Keys)
				_players[key].Clear();

			//yield return new WaitForSeconds(1f);
			Clear();

		}

		private void ClearBetAndRole()
		{
			for (int i = 0; i < _bets.Count; i++)
			{
				var bet = _bets[i];
				var dealer = _dealers[i];
				bet.gameObject.SetActive(false);
				dealer.gameObject.SetActive(false);
			}
		}

		private void InitBetAndRole(DistributionSharedDataPlayer dataPlayer, bool isPreFlop, int playerPanelKey)
		{
			if (dataPlayer.IsWin) return;
			//if (_sharedData == null) return;
			var betCount = _currentSharedData.StageDistribToPreflop() ? dataPlayer.bet : dataPlayer.BetInRound;

			var betItem = _bets[playerPanelKey];

			if (_gamePanel.GameSession.PreflopBetsOut) return;

			if (!betItem.WaitChange)
			{
				betItem.SetValue(SelectTable, betCount);
				betItem.gameObject.SetActive(_bets[playerPanelKey].Value > 0);
			}
		}

		public void ParseSessionEvents(List<DistributionStage> stages)
		{
			foreach (var item in stages)
			{
				ParseSessionEvent(item);
			}
		}

		public void ParseSessionEvent(DistributionStage stage)
		{
			switch (stage.distribution_stage_type.slug)
			{
				case "preflop":
					{
						break;
					}
				case "flop":
					{
						break;
					}
				case "turn":
					{
						break;
					}
				case "river":
					{
						break;
					}
				case "showdown":
					{
						break;
					}
				case "dealer_choice":
					{
						break;
					}
				case "finish":
					{
						break;
					}
				case "distribution":
					{
						break;
					}
				case "blinds":
					{
						break;
					}
				default:
					break;
			}
		}

		public void DealerChoiceMenu()
		{

			if (!_currentSharedData.IsMeActive)
			{

				foreach (var player in _players.Values)
				{

					if (player.UserId == _currentSharedData.active_event.user_id)
					{
						var endTime = DateTime.Parse(_currentSharedData.active_event.calltime_at);
						var diff = (endTime - GameHelper.NowTime).TotalSeconds;
						player.StartTimer(endTime);
					}
				}

				return;
			}

			var popup = it.Main.PopupController.Instance.ShowPopup<it.Popups.DealerChoicePopup>(PopupType.DealerChoice);
			popup.SetTable(SelectTable);
		}

		public void Showdown()
		{
			_isShowDown = true;

			CheckOrFoldContainer.SetActive(false);
			foreach (var item in _players)
			{
				if (item.Value != null)
				{
					item.Value.OpenCards();
					item.Value.ClearStateWithoutWin();
				}
			}
		}


		public void Raise(decimal count)
		{
			if (count > _currentSharedData.MePlayer.amount)
			{
				//SSTools.ShowMessage("Insufficient funds", SSTools.Position.bottom, SSTools.Time.oneSecond);
				return;
			}

			SetStateButtons(false);
			RaiseInfo raiseInfo = new RaiseInfo(count);
			GameHelper.RaiseActiveDistributionBet(SelectTable, raiseInfo, (isSuccess) =>
			{
				if (!isSuccess) SetStateButtons(true);
			});
		}

		public void TouchOpenCard()
		{
			SetStateButtons(false);
			GameHelper.OpenDistributionCard((isSuccess) =>
			{
				if (!isSuccess) SetStateButtons(true);
			});
		}

		public void TouchChips(bool visibleTimer = false, string stringEndTime = null)
		{
			TablePlayerSession user = null;

			foreach (var elem in _players)
			{
				if (elem.Value.TablePlayerSession.user_id == UserController.User.id)
					user = elem.Value.TablePlayerSession;
			}

			if (user == null) return;

			AddChipsPopup popup = it.Main.PopupController.Instance.ShowPopup<AddChipsPopup>(PopupType.AddChip);
			popup.Init(SelectTable, user, _currentSharedData == null ? null : _currentSharedData.MePlayer, (count, autoIncrement) =>
			{
				if (count > 0)
				{
					AddChips(count);
				}
				if (count != -1)
				{
					if (autoIncrement != _alwaysChipMax)
					{
						_alwaysChipMax = autoIncrement;
						UpdateAutoIncrementBalance(_alwaysChipMax);
					}
				}
			}, stringEndTime, _alwaysChipMax);

			if (visibleTimer)
				popup.OnTimerOut = () =>
				{
					GetUpTable();
				};
		}

		/// <summary>
		/// Обработка чекбокса автоинкремента
		/// </summary>
		/// <param name="value">Значение</param>
		private void UpdateAutoIncrementBalance(bool value)
		{
			TableApi.UpdateAutoAddChipsOptions(SelectTable.id, value, (result) => { });
		}
		private void GetAutoIncrementBalance()
		{
			TableApi.GetAutoAddChipsOptions(SelectTable.id, (result) =>
			{
				if (result.IsSuccess)
					_alwaysChipMax = (bool)result.Result.auto_add_to_max_buy_in;
			});
		}

		private void AddChips(decimal value)
		{
			if (value <= 0) return;


			SetStateButtons(false);
			GameHelper.AddMoney(SelectTable, new MoneyBody(value), playerSession =>
			{

				var panel = it.Main.PopupController.Instance.ShowPopup<ChipInNextHandPopup>(PopupType.ChipInNextHand);
				panel.SetValue(value);

				SetStateButtons(true);
			}, s =>
			{
				SetStateButtons(true);
				//SSTools.ShowMessage("Failed top up balance", SSTools.Position.bottom, SSTools.Time.twoSecond);
			});

		}

		private void GetUpTable()
		{
			SetStateButtons(false);
			GameHelper.GetUpTable(SelectTable, (table) =>
			{
				_currentSharedData = null;
				_currentUserData = null;

				SetStateButtons(true);
				StartGame(table);
			}, (error) =>
			{
				SetStateButtons(true);

			}, false);
			if (ChatButton != null) ChatButton.interactable = false;
			_gamePanel.CloseChat();
		}

		public void OnValueChangedWaitBlinds()
		{
			UpdateOptions(isSuccess =>
			{
				if (!isSuccess)
				{
					WaitBlindsToggle.onValueChanged.RemoveAllListeners();
					WaitBlindsToggle.isOn = WaitBlindsToggle.isOn;
					WaitBlindsToggle.onValueChanged.AddListener((val) =>
					{
						OnValueChangedWaitBlinds();
					});
				}
			});
		}

		public void OnValueChangedAutoFold()
		{
			UpdateOptions(isSuccess =>
			{
			});
		}

		public void OnValueChangedSkip()
		{
			UpdateOptions(isSuccess =>
			{
			});
		}

		public void UpdateOptions(Action<bool> callback = null, bool isBbAccepted = false)
		{
			PlayerSessionOptions options = new PlayerSessionOptions(

					false,
					false,
					!WaitBlindsToggle.isOn);
			GameHelper.UpdateTablePlayerSessionOptions(SelectTable, options,
					(isSuccess) => { callback?.Invoke(isSuccess); });
		}

		public void CloseWithoutStendup()
		{
			GameHelper.CloseTable();
		}

		public void StopObserve()
		{
			//RightContainer.SetActive(false);
		}

		public void TouchStartNewGame()
		{
			GameHelper.GetTable(SelectTable, resultTable => { StartGame(resultTable); },
					s =>
					{
						//SSTools.ShowMessage("Connect failed, try later", SSTools.Position.bottom, SSTools.Time.oneSecond);
					});
		}

		private void ClearTable(bool playersClear = true)
		{
			//foreach (var item in players) DestroyImmediate(item.Value.gameObject);
			foreach (var item in placesFree)
			{
				PoolerManager.Return("GamePlace", item.Value.gameObject);
				//DestroyImmediate(item.Value.gameObject); 
			}
#if UNITY_STANDALONE
			foreach (var item in PlayerPlaces) item.localScale = Vector3.one;
			foreach (var item in PlayerPlacesTakeSit) item.localScale = Vector3.one;
#endif
			ClearBetAndDealer();
			//players = new Dictionary<int, PlayerGameIcone>();
			placesFree.Clear();
			ClearCards();
		}

		private void ClearBetAndDealer()
		{
			foreach (var item in _bets) item.gameObject.SetActive(false);
			//foreach (var item in _dealers) item.gameObject.SetActive(false);
		}

		private void ClearCards()
		{
			foreach (var item in CardsOnTable)
			{
				PoolerManager.Return("GameCard", item.gameObject);
				//Destroy(item.gameObject);
			}
			CardsOnTable.Clear();
		}
	}
}