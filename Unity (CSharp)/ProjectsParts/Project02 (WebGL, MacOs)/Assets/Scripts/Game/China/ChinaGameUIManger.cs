using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;
using it.Network.Rest;
using it.Popups;
using it.Network.Socket;

public enum ChinaPlayerUIType
{
	Bottom,
	Left,
	Right,
}
namespace Garilla.Games
{
	public class ChinaGameUIManger : GameControllerBase
	{
		[Header("Prefabs")]
		[SerializeField] private List<ChinaGameUIPlayer> GamePlayerUIPrefabs;
		[SerializeField] private GamePlaceUI GamePlaceUIPrefab;
		[SerializeField] private TablePlaceManager _place;

		[SerializeField] private Button OkBtn;

		[Space]
		[SerializeField] private List<Transform> PlayerPlaces;

		[HideInInspector] public ChinaDistributionSharedData currentSharedData = null;
		private SocketEventDistributionUserData currentUserData = null;

		private Dictionary<int, ChinaGameUIPlayer> players = new Dictionary<int, ChinaGameUIPlayer>();
		private List<GameCardUI> cards = new List<GameCardUI>();
		private bool isShowDown = false;
		private bool isGameInProgress = false;

		public override void InitGame(Table table, it.UI.GamePanel gameInitManager)
		{
			base.InitGame(table, gameInitManager);

			StartGame(table);
		}

		private void StartGame(Table table)
		{
			Init(table);
			InitObserve();
		}

		private void InitObserve()
		{
			com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.GameTableEvent(_chanel), DistributionCalback);


			//WebsocketClient.Instance.ChinaDistributionTableCallback = (sharedData, userData, serverTime, chanel) =>
			//{
			//	var lastIndexShift = chanel.LastIndexOf("_", StringComparison.Ordinal);
			//	if (!int.TryParse(chanel.Substring(lastIndexShift + 1), out var idTableDistribution)) return;
			//	if (idTableDistribution != selectTable.Id) return;

			//	GameController.ServerTime = serverTime;
			//	Distribution(sharedData.distribution, userData);
			//	ParseEvents(sharedData.events);
			//};

			//WebsocketClient.Instance.ChangeTableCallback = ChangeTable;

			com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.GameTableUpdate(_chanel), GameUpdate);
		}

		public override void WSConnect(){}

		public override void WSDisconnect(){}

		public override void WSSocketOpen(){}

		private void DistributionCalback(com.ootii.Messages.IMessage handle)
		{
			var data = (DistributonEvents)handle.Data;

			GameHelper.ServerTime = data.server_time;
			Distribution(data.SocketEventChinaSharedData.distribution, data.SocketEventUserData);
			ParseEvents(data.SocketEventChinaSharedData.events);
		}

		private void GameUpdate(com.ootii.Messages.IMessage handle)
		{
			var tableEvent = ((it.Network.Socket.GameUpdate)handle.Data).TableEvent;
			if (tableEvent.table.id != SelectTable.id) return;
			UpdateTable(tableEvent.table);
		}

		private void ChangeTable(SocketEventTable socketEventTable, string chanel)
		{
			if (socketEventTable.table.id != SelectTable.id) return;
			UpdateTable(socketEventTable.table);
		}

		private void Init(Table selectTable)
		{
			this.SelectTable = selectTable;

			ClearTable();

			if (selectTable.table_player_sessions != null && selectTable.table_player_sessions.Length > 0)
			{
				bool isAtTheTable = selectTable.isAtTheTable;
				var mePlace = selectTable.MePlaceInSorted;
				var playersAtTable = isAtTheTable ? selectTable.sortedPlayerSessions : selectTable.table_player_sessions;
				foreach (var player in playersAtTable)
				{
					var place = 0;
					if (isAtTheTable)
					{
						place = mePlace != -1 ? player.sortedPlace - mePlace : player.sortedPlace;
						if (place < 0) place = PlayerPlaces.Count + place;
					}
					else
					{
						place = player.placeAtTable;
					}

					var isMe = player.user_id == UserController.User.id;
					if (place >= PlayerPlaces.Count)
					{
						it.Logger.Log("place >= PlayerPlaces.Count");
						continue;
					}

					CreateAndInitPlayer(place, player);
				}
			}

			SetBank(0);
			SetStateOnTable();
			SetStateButtons(selectTable.has_active_distribution);

			if (!selectTable.isAtTheTable && selectTable.isExistFreePlace)
			{
				for (int i = 0; i < selectTable.MaxPlayers; i++)
				{
					if (i >= PlayerPlaces.Count)
					{
						it.Logger.Log("free place >= PlayerPlaces.Count");
						break;
					}

					if (selectTable.IsFreePlace(i))
					{
						var placeFreePanel = Instantiate(GamePlaceUIPrefab, PlayerPlaces[i]);
						placeFreePanel.Init(i, (place) => { SitDownTable(place + 1); });
						placesFree.Add(i, placeFreePanel);
					}
				}
			}

			CheckActiveDistribution();
		}

		private void UpdateTable(Table selectTable)
		{
			this.SelectTable = selectTable;

			if (selectTable.table_player_sessions != null && selectTable.table_player_sessions.Length > 0)
			{
				var leavePlayers = new List<int>();
				foreach (var item in players)
				{
					if (!selectTable.IsExistPlayer(item.Value.TablePlayerSession.user_id))
					{
						leavePlayers.Add(item.Key);
					}
				}

				foreach (var key in leavePlayers)
				{
					DestroyImmediate(players[key].gameObject);
					players.Remove(key);
				}

				bool isAtTheTable = selectTable.isAtTheTable;
				var mePlace = selectTable.MePlaceInSorted;
				var playersAtTable = isAtTheTable ? selectTable.sortedPlayerSessions : selectTable.table_player_sessions;
				foreach (var player in playersAtTable)
				{
					var place = 0;
					if (isAtTheTable)
					{
						place = mePlace != -1 ? player.sortedPlace - mePlace : player.sortedPlace;
						if (place < 0) place = PlayerPlaces.Count + place;
					}
					else
					{
						place = player.placeAtTable;
					}

					var isMe = player.user_id == UserController.User.id;
					if (place >= PlayerPlaces.Count)
					{
						it.Logger.Log("place >= PlayerPlaces.Count");
						continue;
					}

					ClearOldPlacesPlayer(player.user_id);

					if (players.ContainsKey(place))
					{
						if (isAtTheTable && place == 0 && players[place].TablePlayerSession.user_id != UserController.User.id)
						{
							DestroyImmediate(players[place].gameObject);
							players.Remove(place);

							CreateAndInitPlayer(place, player);
						}
						else
						{
							players[place].Init(player, isShowDown);
						}
					}
					else
					{
						if (placesFree.ContainsKey(place))
						{
							DestroyImmediate(placesFree[place].gameObject);
							placesFree.Remove(place);
						}

						CreateAndInitPlayer(place, player);
					}
				}
			}
			else
			{
				ClearTable();
			}

			SetStateOnTable();
			SetStateButtons(selectTable.has_active_distribution);

			if (!selectTable.isAtTheTable && selectTable.isExistFreePlace)
			{
				for (int i = 0; i < selectTable.MaxPlayers; i++)
				{
					if (i >= PlayerPlaces.Count)
					{
						it.Logger.Log("free place >= PlayerPlaces.Count");
						break;
					}

					if (selectTable.IsFreePlace(i) && !placesFree.ContainsKey(i))
					{
						var placeFreePanel = Instantiate(GamePlaceUIPrefab, PlayerPlaces[i]);
						placeFreePanel.Init(i, (place) => { SitDownTable(place + 1); });
						placesFree.Add(i, placeFreePanel);
					}
				}
			}

			CheckActiveDistribution();
			InitPlayersAtDistribution();

			//CheckAfk();
		}

		private void CheckActiveDistribution()
		{
			if (SelectTable.has_active_distribution)
			{
				GameHelper.GetChinaActiveDistributionsData(SelectTable, (data) =>
				{
					if (data != null)
					{
						Distribution(data.shared_data, data.user_data);
					}
				});
			}

			//SetBidCount();
			CheckAfk();
		}

		private void CreateAndInitPlayer(int place, TablePlayerSession player)
		{
			var gamePlayerUIPrefab = GetPlayerUIPrefab(place);
			var playerPanel = Instantiate(gamePlayerUIPrefab, PlayerPlaces[place]);
			playerPanel.Init(player, isShowDown);
			players.Add(place, playerPanel);
		}

		private void CheckAfk()
		{


			if (SelectTable.IsAfk)
			{
				AfkPopup popup = it.Main.PopupController.Instance.ShowPopup<AfkPopup>(PopupType.Afk);
				popup.Init(SelectTable.MePlayer, (isSuccess) =>
				{
					if (isSuccess)
					{
						SetStateButtons(false);
						GameHelper.FinishAfk(SelectTable, session => { SetStateButtons(true); },
											s => { SetStateButtons(true); });
					}
					else
					{
						GetUpTable();
					}
				});
			}
		}

		private void ClearOldPlacesPlayer(ulong id)
		{
			var oldPlacesPlayers = new List<int>();
			foreach (var item in players)
			{
				if (item.Value.TablePlayerSession.user_id == id)
				{
					oldPlacesPlayers.Add(item.Key);
				}
			}

			foreach (var key in oldPlacesPlayers)
			{
				DestroyImmediate(players[key].gameObject);
				players.Remove(key);
			}
		}

		public void SitDownTable()
		{
			ShowBuyInPopup((buyIn) => SitDown(buyIn, GetRandomPlace()));
		}

		private int GetRandomPlace()
		{
			if (placesFree.Count == 0) return 0;
			var placeIndex = Random.Range(0, placesFree.Count - 1);
			return placesFree.ElementAt(placeIndex).Value.Place + 1;
		}

		private void SitDownTable(int place)
		{
			ShowBuyInPopup((buyIn) => SitDown(buyIn, place));
		}

		private void ShowBuyInPopup(Action<decimal> callback)
		{

			BuyInPopup popup = it.Main.PopupController.Instance.ShowPopup<BuyInPopup>(PopupType.BuyIn);
			popup.Init(SelectTable, callback);


		}

		private void SitDown(decimal buyIn, int place)
		{
			if (buyIn <= 0) return;

			GameHelper.SitDownTable(SelectTable, new SitdownInfo(buyIn, place), (table) =>
			{
				StartGame(table);
			}, (error) => { });
		}

		private ChinaGameUIPlayer GetPlayerUIPrefab(int place)
		{
			switch (place)
			{
				case 0:
					{
						if (SelectTable.isAtTheTable) return GamePlayerUIPrefabs[(int)ChinaPlayerUIType.Bottom];
						else return GamePlayerUIPrefabs[(int)ChinaPlayerUIType.Right];
					}
				case 1: return GamePlayerUIPrefabs[(int)ChinaPlayerUIType.Left];
				case 2: return GamePlayerUIPrefabs[(int)ChinaPlayerUIType.Right];

				default: return GamePlayerUIPrefabs[(int)ChinaPlayerUIType.Bottom];
			}
		}

		private void SetBank(decimal bank)
		{
			_totalBank.SetValue($"Total bank: <b>{bank}</b>",SelectTable,bank);
		}

		private void Distribution(ChinaDistributionSharedData sharedData, SocketEventDistributionUserData userData)
		{
			isGameInProgress = SelectTable.isAtTheTable && sharedData.is_active;
			isShowDown = false;
			currentSharedData = sharedData;
			currentUserData = userData;

			InitPlayersAtDistribution();
			SetStateOnTable();

			var isMeActive = sharedData.IsMeActive;
			SetStateButtons(isMeActive);
			SetBank(sharedData.GetTotalBank());
			ParseSessionEvents(sharedData.stages);
		}

		private void InitPlayersAtDistribution()
		{
			if (currentSharedData == null) return;

			foreach (var player in currentSharedData.players)
			{
				foreach (var playerPanel in players)
				{
					if (playerPanel.Value.TablePlayerSession.user_id == player.user.id)
					{
						bool isMe = playerPanel.Value.TablePlayerSession.user_id == UserController.User.id;
						playerPanel.Value.Init(player, currentSharedData.GetActiveEventByPlayer(player),
								isMe && currentUserData != null ? currentUserData.cards : player.cards,
								currentSharedData.IsPreFlop, isShowDown);
						InitBetAndRole(player, currentSharedData.IsPreFlop);
					}
				}
			}
		}

		private void SetStateOnTable()
		{
			if (currentSharedData != null && SelectTable.isAtTheTable)
			{
				OkBtn.gameObject.SetActive((currentSharedData.IsAvailableEndTurn && currentSharedData.IsMeActive) || GetMePlayerPrefab().IsFullGrid());
			}
			else
			{
				OkBtn.gameObject.SetActive(false);
			}
		}

		private ChinaGameUIPlayer GetMePlayerPrefab()
		{
			foreach (var player in players.Values)
			{
				if (player.IsMe) return player;
			}

			return null;
		}

		private void ParseEvents(List<DistributionEvent> events)
		{
			foreach (var dEvent in events)
			{
				if (dEvent.is_active != null && (bool)dEvent.is_active) ParseDistributionEvent(dEvent.distribution_event_type.slug);
			}
		}

		private void ParseDistributionEvent(string eventType)
		{
			switch (eventType)
			{
				case "all-in":
					{
						break;
					}
				case "big-blind":
					{
						break;
					}
				case "big-blind_with_all-in":
					{
						break;
					}
				case "call":
					{
						break;
					}
				case "check":
					{
						break;
					}
				case "distribution_finished":
					{
						break;
					}
				case "distribution_started":
					{
						break;
					}
				case "fold":
					{
						break;
					}
				case "playing_card_given_to_player":
					{
						break;
					}
				case "raise":
					{
						break;
					}
				case "rake-payout":
					{
						break;
					}
				case "refund-of-funds":
					{
						break;
					}
				case "shared_playing_card_issued":
					{
						break;
					}
				case "small-blind":
					{
						break;
					}
				case "small-blind_with_all-in":
					{
						break;
					}
				case "stage_finished":
					{
						break;
					}
				case "stage_started":
					{
						break;
					}
				case "transfer-of-winnings":
					{
						break;
					}
				case "user_action_request":
					{
						break;
					}
			}
		}

		public void InitBetAndRole(ChinaDistributionSharedDataPlayer dataPlayer, bool isPreFlop)
		{
			for (int i = 0; i < _dealers.Count; i++)
			{
				var dealer = _dealers[i];
				dealer.gameObject.SetActive(false);

				foreach (var item in players)
				{
					if (item.Key == i && !dataPlayer.IsWin)
					{
						InitRole(dataPlayer.role, dealer);
					}
				}
			}
		}

		private void InitRole(string role, Image dealer)
		{
			switch (role)
			{
				case DistributionSharedDataPlayer.RoleDealer:
				case DistributionSharedDataPlayer.RoleDealerSb:
				case DistributionSharedDataPlayer.RoleDealerBb:
				case DistributionSharedDataPlayer.RoleDealerBbp:
					dealer.gameObject.SetActive(true);
					break;
				default:
					dealer.gameObject.SetActive(false);
					break;
			}
		}

		public void ParseSessionEvents(DistributionStage[] stages)
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
				case "distribution_started":
					{
						if (SelectTable.isAtTheTable) isGameInProgress = true;
						break;
					}
				case "distribution_finished":
					{
						Showdown();
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

		public void Showdown()
		{
			isGameInProgress = false;
			isShowDown = true;
			foreach (var item in players)
			{
				item.Value.OpenCards();
				item.Value.ClearStateWithoutWin();
			}

			ClearBetAndDealer();
		}

		public void TouchChips()
		{

			TablePlayerSession user = null;
			for (int i = 0; i < players.Count; i++)
			{
				if (players[i].TablePlayerSession.user_id == UserController.User.id)
					user = players[i].TablePlayerSession;
			}


			AddChipsPopup popup = it.Main.PopupController.Instance.ShowPopup<AddChipsPopup>(PopupType.AddChip);

			popup.Init(SelectTable, user, null, (count,autoIncrement) =>
			{
				if (count > 0)
				{
					SetStateButtons(false);
					GameHelper.AddMoney(SelectTable, new MoneyBody(count), session =>
							{
								SetStateButtons(true);
							//UpdateTable(session.table);
							}, s =>
					{
						SetStateButtons(true);
					});

					//if(autoIncrement != )

				}
			});
		}

		private void GetUpTable()
		{
			SetStateButtons(false);
			GameHelper.GetUpTable(SelectTable, (table) =>
			{
				currentSharedData = null;
				currentUserData = null;

				SetStateButtons(true);
				StartGame(table);
			}, (error) => { SetStateButtons(true); }, false);
		}

		public void Close()
		{
			ClearEvents();
			if (SelectTable.isAtTheTable)
			{
				GameHelper.GetUpTable(SelectTable, (table) =>
				{
				}, (error) => { });
			}
		}

		public void CloseWithoutStandUp()
		{
			ClearEvents();
			GameHelper.CloseTable();
		}

		public void ClearEvents()
		{
			//WebsocketClient.Instance.ChinaDistributionTableCallback = null;
			//WebsocketClient.Instance.ChangeTableCallback = null;
		}

		public void TouchApplyAllCards()
		{
			SetStateButtons(false);
			GameHelper.ChinaApplyAllCard(SelectTable.id, resultTable =>
					{
						SetStateButtons(true);
					},
					s =>
					{
						SetStateButtons(true);
					});
		}

		public void TouchStartNewGame()
		{
			GameHelper.GetTable(SelectTable, resultTable => { StartGame(resultTable); },
					s =>
					{
					});
		}

		private void SetStateButtons(bool isActiveState)
		{
			OkBtn.interactable = isActiveState;
			foreach (var item in players.Values)
			{
				item.SetStateButtons(isActiveState);
			}
		}

		private void FixedUpdate()
		{
			if (currentSharedData != null && currentSharedData.IsMeActive &&
					currentSharedData.GetActiveEventByPlayerId(UserController.User.id).calltime_at != null && DateTime.TryParse(currentSharedData.GetActiveEventByPlayerId(UserController.User.id).calltime_at, out var endTime))
			{
				var diff = (endTime - GameHelper.NowTime).TotalMilliseconds;
				if (diff <= 300)
				{
					SetStateButtons(false);
				}
			}
		}

		private void ClearTable()
		{
			foreach (var item in players) DestroyImmediate(item.Value.gameObject);
			foreach (var item in placesFree) DestroyImmediate(item.Value.gameObject);
			ClearBetAndDealer();
			players = new Dictionary<int, ChinaGameUIPlayer>();
			placesFree = new Dictionary<int, GamePlaceUI>();
			ClearCards();
		}

		private void ClearBetAndDealer()
		{
			foreach (var item in _dealers) item.gameObject.SetActive(false);
		}

		private void ClearCards()
		{
			foreach (var item in cards) Destroy(item.gameObject);
			cards = new List<GameCardUI>();
		}
	}
}