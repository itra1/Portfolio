using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Threading;
//using System.Threading.Tasks;
using TMPro;
using it.Network.Rest;
using it.UI;
using it.Popups;
using Sett = it.Settings;
using System.Linq;
using it.UI.Elements;
using DG.Tweening;

namespace Garilla.Games.UI
{

	public class HandHistoryPopup : PopupBase
	{

		[SerializeField] private TextMeshProUGUI _titleLabel;
		[SerializeField] private RectTransform _buttonsParent;
		[SerializeField] private ButtonsStruct[] _buttons;
		[SerializeField] private RectTransform TableParent;
		[SerializeField] private it.UI.Elements.Bets _totalBank;
		[SerializeField] private it.Game.Panels.DealerChoisePanel _dealerChoisePanel;
		//private HandHistoryTable _historyTable;
		//[SerializeField] private Transform[] StagesContent;
		//[SerializeField] private string[] StagesName;
		private string stageFinal;
		//[SerializeField] private HistoryItemPlayer ItemPlayerLeft;
		//[SerializeField] private HistoryItemPlayer ItemPlayerRight;
		//private List<HistoryItemPlayer> ItemPlayers = new List<HistoryItemPlayer>();

		[Space, Header("Distibutions")]
		[SerializeField] private ScrollRect _distrScroll;
		[SerializeField] private HistoryListItem _historyItemPref;
		private PoolList<HistoryListItem> _poolHistoryItems;
		private List<HistoryListItem> _historyListItems = new List<HistoryListItem>();
		[SerializeField] private List<string> NotUseEvent = new List<string>() { "user_action_request" };

		//[Space, Header("GameWindow")]
		//[SerializeField] private Transform TableParent;
		//[SerializeField] private float TableSizeDelta = 2f;
		//[SerializeField] private List<TablePlaceManager> _tableListPrefab;

		[Space]
		private List<Transform> _playerPlaces = new List<Transform>();
		private List<Transform> _sitPlaces = new List<Transform>();
		private List<PlayerGameIcone> _playersUI = new List<PlayerGameIcone>();

		[SerializeField] private List<Transform> CardPlaces;
		[SerializeField] private GameCardUI GameCardUIPrefab;
		private List<GameCardUI> cards = new List<GameCardUI>();

		[Space]
		[SerializeField] private Rect RectScreenShot;
		[SerializeField] private string path;
		private string ScreenshotName = "screen";

		//#if UNITY_ANDROID || UNITY_WEBGL || UNITY_IOS
		//		[Space, Header("Mobile")]
		//    [SerializeField] private GameObject DistributionList;
		//    [SerializeField] private GameObject DistributionNow;
		//    [SerializeField] private TextMeshProUGUI TitleTxt;
		//#endif
		//#if UNITY_ANDROID || UNITY_WEBGL || UNITY_IOS
		[Space, Header("Mobile")]
		[SerializeField] private RectTransform _distributionListView;
		[SerializeField] private RectTransform _distributionTableView;
		//[SerializeField] private ScrollRect _buttonsScroll;
//#endif

		private TablePlaceManager _placeManager;
		private GameUIManager _gameUIManager;
		//private DistributionHistorySharedData _sharedData;
		private DistributionSharedData _sharedData;
		[HideInInspector] public SocketEventDistributionUserData userData;
		private Table _table;
		private bool _isFinish;

		private Dictionary<string, Dictionary<ulong, StageData>> _balances = new Dictionary<string, Dictionary<ulong, StageData>>();
		private Dictionary<string, decimal> _bank = new Dictionary<string, decimal>();
		private Dictionary<ulong, decimal> _winSumm = new Dictionary<ulong, decimal>();
		private Coroutine _showCombinationsTask;
		public class StageData
		{
			public decimal UserBalance;
			public decimal Balance;
			public string Action;
			public bool IsShowDown;
		}


		[System.Serializable]
		public struct ButtonsStruct
		{
			public string Slug;
			public bool IsAllOrNothing;
			public List<GameType> AllOrNofingGameTypes;
			[HideInInspector] public bool IsDisable;
			public it.UI.Elements.GraphicButtonUI GoldButton;
			public it.UI.Elements.GraphicButtonUI GrayButton;
		}

		protected override void Awake()
		{
			base.Awake();
#if UNITY_ANDROID || UNITY_WEBGL || UNITY_IOS
			_distributionTableView.gameObject.SetActive(false);
			_distributionTableView.anchoredPosition = new Vector2(-_distributionTableView.rect.width, _distributionTableView.anchoredPosition.y);
			//var btnClose = _distributionTableView.GetComponentInChildren<BackButton>().GetComponent<GraphicButtonUI>();
			//btnClose.OnClick.RemoveAllListeners();
			//btnClose.OnClick.AddListener(() =>
			//{
			//	_distributionTableView.DOAnchorPos(new Vector2(_distributionTableView.rect.width, _distributionTableView.anchoredPosition.y), 0.3f);
			//});
#endif
		}

		public void GetCustomHistory(ulong tableId, string startDeapasone, string endDiapasone){

			var table = TableManager.Instance.GetTableById(tableId);

			if (table == null) return;

			TableApi.GetTableDistributionsDataByCustom(table, startDeapasone, endDiapasone, (result) =>
			{
				if (result.IsSuccess)
					DataCreateDistributionsList(result.Result);
			});
		}

		public void Init(Table table, GameUIManager gameUIManager)
		{
			this._table = table;
			//this._gameUIManager = gameUIManager;
			ConfirmData();

			TableApi.GetTableDistributionsData(table, (result) =>
			{
				if (result.IsSuccess)
					DataCreateDistributionsList(result.Result);
			});

		}

		private void ConfirmData()
		{
			if (_dealerChoisePanel != null)
				_dealerChoisePanel.gameObject.SetActive(false);
			_historyItemPref.gameObject.SetActive(false);


			for (int i = 0; i < _buttons.Length; i++)
			{
				int index = i;
				_buttons[index].GoldButton.OnClickPointer.RemoveAllListeners();
				_buttons[index].GrayButton.OnClickPointer.RemoveAllListeners();
				_buttons[index].GrayButton.OnClickPointer.AddListener(() =>
				{
					StageSelect(_buttons[index].Slug);
				});
				_buttons[index].GoldButton.gameObject.SetActive(false);
				_buttons[index].GrayButton.gameObject.SetActive(false);
				_buttons[index].IsDisable = true;
			}

			_titleLabel.text = $"{_table.name} - {it.Helpers.Currency.String(_table.SmallBlindSize)} / {it.Helpers.Currency.String(_table.big_blind_size)}";

			if (_poolHistoryItems == null)
				_poolHistoryItems = new PoolList<HistoryListItem>(_historyItemPref.gameObject, _distrScroll.content);

			_poolHistoryItems.HideAll();

			//ItemPlayers.ForEach(x => { x.gameObject.SetActive(false); });
			//ItemPlayers.ForEach(x => { Destroy(x.gameObject); });
			//ItemPlayers.Clear();
			_totalBank.SetValue($"Total Pot : <color=#C68C43>{it.Helpers.Currency.String(0m)}</color>", _table, 0);
			cards.ForEach(x => { Destroy(x.gameObject); });
			cards.Clear();
			if (_placeManager) Destroy(_placeManager.gameObject);

			//var tb = Sett.GameSettings.GetHandHistoryTable(_table);
			//_historyTable = Instantiate(tb, _tableParent);
			//_historyTable.gameObject.SetActive(false);
			//tb.Select = StageSelect;

#if UNITY_ANDROID || UNITY_WEBGL || UNITY_IOS
			var placePref = Sett.AndroidSettings.GetTablePlaceManagerStatic(_table);
#else
			var placePref = Sett.StandaloneSettings.GetTablePlaceManager(_table);
#endif


			_placeManager = Instantiate(placePref, TableParent);

			//RectTransform rt = placeManager.GetComponent<RectTransform>();
			//rt.sizeDelta = new Vector2(0, -50);

			_placeManager.transform.SetAsFirstSibling();
			_placeManager.SetTablePositions(_table);
			//placeManager.transform.localScale /= TableSizeDelta;
			_playerPlaces = _placeManager.Places.PlayerPlaces;
			_sitPlaces = _placeManager.Places.PlayerPlacesTakeSit;
			_placeManager.Places.ClearBetAndDealer();

			//#if UNITY_ANDROID || UNITY_WEBGL || UNITY_IOS
			//			DistributionNowSet(false);
			//#endif
			gameObject.SetActive(true);
		}

		void DataCreateDistributionsList(DistributionTableHistoryResponse historyResponse)
		{
			_totalBank.SetValue($"Total Pot : <color=#C68C43>{it.Helpers.Currency.String(0m)}</color>", _table, 0);
			DistributionHistoryDataResponse[] dataResponse = historyResponse.data;
			_poolHistoryItems.HideAll();
			_historyListItems.Clear();

			for (int i = 0; i < dataResponse.Length; i++)
			{
				HistoryListItem item = _poolHistoryItems.GetItem();
				_historyListItems.Add(item);
				item.Init(dataResponse[i], this);
			}

			_distrScroll.content.sizeDelta = new Vector2(_distrScroll.content.sizeDelta.x, dataResponse.Length * (_historyItemPref.GetComponent<RectTransform>().rect.height + 10));

#if !UNITY_ANDROID && !UNITY_WEBGL && !UNITY_IOS
			if (_historyListItems.Count > 0) _historyListItems[0].ClickMe();
#endif
		}

		/// <summary>
		/// Только для мобилки
		/// </summary>
		public void HideTableView()
		{
		#if !UNITY_STANDALONE
			_distributionTableView.DOAnchorPos(new Vector2(-_distributionTableView.rect.width, 0), 0.3f).OnComplete(() =>
			{
				_distributionTableView.gameObject.SetActive(false);
			});
			#endif
		}

		public void SelectHistoryItems(HistoryListItem listItem, DistributionHistoryDataResponse dataResponse)
		{
			for (int i = 0; i < _historyListItems.Count; i++)
			{
				if (_historyListItems[i] != listItem)
				{
					_historyListItems[i].Close();
				}
			}

#if UNITY_ANDROID || UNITY_WEBGL || UNITY_IOS
			_distributionTableView.gameObject.SetActive(true);
			_distributionTableView.anchoredPosition = new Vector2(-_distributionTableView.rect.width, 0);
			_distributionTableView.DOAnchorPos(Vector2.zero, 0.3f);
#endif
			DataCreateGame(dataResponse);
		}

		void DataCreateGame(DistributionHistoryDataResponse dataResponse)
		{

			if (_dealerChoisePanel != null && _table.is_dealer_choice)
			{
				_dealerChoisePanel.gameObject.SetActive(true);
				_dealerChoisePanel.Set((int)dataResponse.game_rule.id);
			}

			_balances.Clear();
			_sharedData = dataResponse.shared_data;
			userData = dataResponse.user_data;
			//ItemPlayers.ForEach(x => { x.gameObject.SetActive(false); });
			//ItemPlayers.ForEach(x => { Destroy(x.gameObject); });
			//ItemPlayers.Clear();
			_winSumm.Clear();
			bool isShowDown = false;
			ulong showdownStage = ulong.MaxValue;

			_placeManager.Places.Dealers.ForEach(x => x.gameObject.SetActive(false));

			Dictionary<ulong, int> keyValuesStages = new Dictionary<ulong, int>();
			List<int> stages = new List<int>();
			int? stageFinalN = null;


			for (int i = 0; i < _buttons.Length; i++)
			{
				_buttons[i].GoldButton.gameObject.SetActive(false);
				_buttons[i].GrayButton.gameObject.SetActive(false);
				_buttons[i].IsDisable = true;
			}
			int buttonCount = 0;

			for (int i = 0; i < _sharedData.stages.Count; i++)
			{
				for (int n = 0; n < _buttons.Length; n++)
				{
					if (_sharedData.stages[i].distribution_stage_type.slug == "showdown")
						showdownStage = _sharedData.stages[i].id;

					if (_sharedData.stages[i].distribution_stage_type.title == _buttons[n].Slug)
					{
						keyValuesStages.Add(_sharedData.stages[i].id, n);
						stages.Add(n);
						stageFinal = _sharedData.stages[i].distribution_stage_type.title;
						stageFinalN = n;
						break;
					}
				}

				//for (int n = 0; n < _historyTable.StagesName.Length; n++)
				//{
				//	if (_sharedData.stages[i].distribution_stage_type.slug == "showdown")
				//		showdownStage = _sharedData.stages[i].id;

				//	if (_sharedData.stages[i].distribution_stage_type.title == _historyTable.StagesName[n])
				//	{
				//		keyValuesStages.Add(_sharedData.stages[i].id, n);
				//		stages.Add(n);
				//		stageFinal = _sharedData.stages[i].distribution_stage_type.title;
				//		stageFinalN = n;
				//		break;
				//	}
				//}

				for (int n = 0; n < _buttons.Length; n++)
					if (_buttons[n].Slug == _sharedData.stages[i].distribution_stage_type.title)
					{
						_buttons[n].IsDisable = false;
						_buttons[n].GrayButton.gameObject.SetActive(!(_table.is_all_or_nothing && (!_buttons[n].IsAllOrNothing || !_buttons[n].AllOrNofingGameTypes.Contains((GameType)_table.game_rule_id))));
						buttonCount++;
					}

			}

			_buttonsParent.sizeDelta = new Vector2(128 * buttonCount + (32 * (buttonCount - 1)), _buttonsParent.sizeDelta.y);

			Dictionary<ulong, DistributionSharedDataPlayer> keyValuesUsers = new Dictionary<ulong, DistributionSharedDataPlayer>();
			for (int i = 0; i < _sharedData.players.Count; i++)
			{
				keyValuesUsers.Add(_sharedData.players[i].user.id, _sharedData.players[i]);

			}

			Dictionary<int, List<DistributionSharedDataPlayer>> playersInEvent = new Dictionary<int, List<DistributionSharedDataPlayer>>();
			decimal bankSumm = 0;
			string stageName = "";
			string beforeStageName = "";
			for (int i = 0; i < _sharedData.events.Count; i++)
			{
				if (showdownStage == _sharedData.events[i].distribution_stage_id)
					isShowDown = true;


				if (_sharedData.events[i].bank_amount_delta != null && _sharedData.events[i].bank_amount_delta > 0)
					bankSumm += (decimal)_sharedData.events[i].bank_amount_delta;

				keyValuesStages.TryGetValue(_sharedData.events[i].distribution_stage_id, out int bankStage);
				string stageNameBank = _buttons[bankStage].Slug;
				if (!_bank.ContainsKey(stageNameBank))
					_bank.Add(stageNameBank, 0);
				_bank[stageNameBank] = bankSumm;


				if (_sharedData.events[i].user_id != null && NotUseEvent.Contains(_sharedData.events[i].distribution_event_type.slug) == false &&
						keyValuesStages.TryGetValue(_sharedData.events[i].distribution_stage_id, out int stage) && (int)stage < _buttons.Length &&
						keyValuesUsers.TryGetValue((ulong)_sharedData.events[i].user_id, out DistributionSharedDataPlayer sharedDataPlayer))
				{

					//float height = 0;
					//for (int ii = 0; ii < _historyTable.StagesContent[stage].childCount; ii++)
					//	if (_historyTable.StagesContent[stage].GetChild(ii).gameObject.activeInHierarchy)
					//		height += _historyTable.StagesContent[stage].GetChild(ii).GetComponent<RectTransform>().rect.height;

					//HistoryItemPlayer itemPlayer = Instantiate(sharedDataPlayer.user.id == GameHelper.UserInfo.id ? ItemPlayerRight : ItemPlayerLeft, _historyTable.StagesContent[stage]);
					bool stageFinalBl = stage >= stageFinalN;
					if (sharedDataPlayer.user.id == GameHelper.UserInfo.id) sharedDataPlayer.cards = dataResponse.user_data.cards;
					//itemPlayer.Init(sharedDataPlayer, _sharedData.events[i], (stageFinalBl && sharedDataPlayer.IsWin == false),
					//		 (stageFinalBl && sharedDataPlayer.IsWin) ? _sharedData.shared_cards : null, stageFinalBl && sharedDataPlayer.IsWin ? "WIN" : null,
					//		 stageFinalBl && sharedDataPlayer.IsWin ? _sharedData.banks[0].amount : -1);

					string nStageName = _buttons[stage].Slug;
					if (nStageName == "Blinds" || nStageName == "Distribution")
						nStageName = "Preflop";

					if (stageName != nStageName)
					{
						beforeStageName = stageName;
					}
					stageName = nStageName;
					if (!_balances.ContainsKey(stageName))
						_balances.Add(stageName, new Dictionary<ulong, StageData>());

					if (!_balances[stageName].ContainsKey(sharedDataPlayer.user.id))
					{
						_balances[stageName].Add(sharedDataPlayer.user.id, new StageData());

						if (!string.IsNullOrEmpty(beforeStageName))
							_balances[stageName][sharedDataPlayer.user.id].Action = _balances[beforeStageName][sharedDataPlayer.user.id].Action;
					}

					_balances[stageName][sharedDataPlayer.user.id].Balance += _sharedData.events[i].BankAmountDelta;
					_balances[stageName][sharedDataPlayer.user.id].Action = _sharedData.events[i].distribution_event_type.slug;
					_balances[stageName][sharedDataPlayer.user.id].IsShowDown = isShowDown;

					var stf = _sharedData.stages.Find(x => x.id == _sharedData.events[i].distribution_stage_id);
					var fsa = stf.after_stage_amounts.Find(x => x.distribution_session_id == (_sharedData.players.Find(y => y.user.id == sharedDataPlayer.user.id).distribution_session_id));
					_balances[stageName][sharedDataPlayer.user.id].UserBalance = fsa.after_stage_amount;

					//if (!_bank.ContainsKey(stageName))
					//	_bank.Add(stageName, 0);
					//_bank[stageName] += sharedData.events[i].BankAmountDelta;

					//ItemPlayers.Add(itemPlayer);

					if (!playersInEvent.ContainsKey(stage)) playersInEvent.Add(stage, new List<DistributionSharedDataPlayer>());
					playersInEvent[stage].Add(sharedDataPlayer);
					//itemPlayer.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -height);

				}
			}
			//for (int i = 0; i < stages.Count; i++) // Добавляем игроков, которые ничег оне сделали
			//{
			//	for (int a = 0; a < _sharedData.players.Count; a++)
			//	{
			//		if (!playersInEvent.ContainsKey(stages[i]) || !playersInEvent[stages[i]].Contains(_sharedData.players[a]))
			//		{

			//			DistributionSharedDataPlayer sharedDataPlayer = _sharedData.players[a]; int stage = stages[i];

			//			float height = 0;
			//			for (int ii = 0; ii < _historyTable.StagesContent[stage].childCount; ii++)
			//				if (_historyTable.StagesContent[stage].GetChild(ii).gameObject.activeInHierarchy)
			//					height += _historyTable.StagesContent[stage].GetChild(ii).GetComponent<RectTransform>().rect.height;

			//			HistoryItemPlayer itemPlayer = Instantiate(sharedDataPlayer.user.id == GameHelper.UserInfo.id ? ItemPlayerRight : ItemPlayerLeft, _historyTable.StagesContent[stage]);
			//			bool stageFinalBl = stage >= stageFinalN;
			//			if (sharedDataPlayer.user.id == GameHelper.UserInfo.id) sharedDataPlayer.cards = dataResponse.user_data.cards;
			//			DistributionEvent distributionEvent = new DistributionEvent()
			//			{
			//				user_id = _sharedData.players[a].user.id,
			//				distribution_event_type = new DistributionEvent.DistributionEventType
			//				{
			//					slug = "check",
			//					title = "Check"
			//				}
			//			};

			//			itemPlayer.Init(sharedDataPlayer, distributionEvent, (stageFinalBl && sharedDataPlayer.IsWin == false),
			//					 (stageFinalBl && sharedDataPlayer.IsWin) ? _sharedData.shared_cards : null, stageFinalBl && sharedDataPlayer.IsWin ? "WIN" : null,
			//					 stageFinalBl && sharedDataPlayer.IsWin ? _sharedData.banks[0].amount : -1);
			//			ItemPlayers.Add(itemPlayer);

			//			itemPlayer.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -height);
			//		}
			//	}
			//}

			//StageSelect(_historyTable.StagesName[0]);
			if (_table.is_all_or_nothing && (GameType)_table.game_rule_id != GameType.Holdem)
				StageSelect(stage: "Flop");
			else
				StageSelect("Preflop");

		}

		public void StageSelect(string stage)
		{
			TableSet(_sharedData.shared_cards, _sharedData.players, stage, stage == stageFinal);

			//for (int i = 0; i < _historyTable.StagesContent.Length; i++)
			//{
			//	Color c = Color.white;
			//	c.a = (stage == _historyTable.StagesName[i] ? 0.1f : 0);
			//	_historyTable.StagesContent[i].GetComponent<Image>().color = c;

			//}

			for (int i = 0; i < _buttons.Length; i++)
			{
				if (_buttons[i].IsDisable) return;
				if (_table.is_all_or_nothing && (!_buttons[i].IsAllOrNothing || !_buttons[i].AllOrNofingGameTypes.Contains((GameType)_table.game_rule_id)))
				{
					_buttons[i].GoldButton.gameObject.SetActive(false);
					_buttons[i].GrayButton.gameObject.SetActive(false);
					continue;
				}
				_buttons[i].GoldButton.gameObject.SetActive(_buttons[i].Slug == stage);
				_buttons[i].GrayButton.gameObject.SetActive(_buttons[i].Slug != stage);
			}

			ScreenshotName = $"{_table.id}-{_sharedData.id}-{stage}-{System.DateTime.Now.ToShortTimeString().Replace(":", "-")}-{System.DateTime.Now.ToShortDateString().Replace(".", "-")}";
		}
		private bool isAllAllIn = true;
		void TableSet(List<DistributionCard> shared_cards, List<DistributionSharedDataPlayer> players, string stage, bool final)
		{
			_isFinish = final;
			_playersUI.ForEach(x => {
				if (x != null) //Destroy(x.gameObject);
					PoolerManager.Return("PlayerGame", x.gameObject);
			});
			_playersUI.Clear();
			_winSumm.Clear();

			int cardsOpen = 0;
			bool isPreFlop = false;
			if (final)
			{
				isPreFlop = true;
				cardsOpen = 5;
			}
			else
			{
				switch (stage)
				{
					case "Blinds":
						cardsOpen = 0;
						break;
					case "Preflop":
						cardsOpen = 0;
						isPreFlop = true;
						break;
					case "Flop":
						cardsOpen = 3;
						isPreFlop = true;
						break;
					case "Turn":
						cardsOpen = 4;
						isPreFlop = true;
						break;
					case "River":
						cardsOpen = 5;
						isPreFlop = true;
						break;
					case "Finish":
						cardsOpen = 5;
						isPreFlop = true;
						break;
					default:
						cardsOpen = 5;
						break;
				}
			}

			_placeManager.Places.ClearBetAndDealer();
			int existswin = -1;

			foreach (var by in _placeManager.Places.Bets)
			{
				by.gameObject.SetActive(false);
				by.SetValue(_table, 0);
			}

			for (int i = 0; i < players.Count; i++)
			{
				if (final && players[i].IsWin)
					existswin = players[i].place - 1;
			}
			isAllAllIn = true;
			for (int i = 0; i < players.Count; i++)
			{
				int place = players[i].place - 1;
				string role = players[i].role;
				if (role == DistributionSharedDataPlayer.RoleDealer
					|| role == DistributionSharedDataPlayer.RoleDealerSb
					|| role == DistributionSharedDataPlayer.RoleDealerBb
					|| role == DistributionSharedDataPlayer.RoleDealerBbp)
					_placeManager.Places.Dealers[place].gameObject.SetActive(true);

				//PlayerGameIcone playerNew = Instantiate(_gameUIManager.GamePlayerUIPrefabs, PlayerPlaces[players[i].Place - 1]);
				GameObject inst = PoolerManager.Spawn("PlayerGame");
				inst.transform.SetParent(_sitPlaces[players[i].place - 1]);
				inst.transform.localScale = Vector3.one;
				PlayerGameIcone playerNew = inst.GetComponent<PlayerGameIcone>();
				inst.transform.localPosition = Vector3.zero;
				var pRect =  inst.GetComponent<RectTransform>();
				pRect.anchoredPosition = Vector3.zero;
				pRect.localScale = Vector3.zero;

				List<DistributionCard> playerCards = players[i].user.id == GameHelper.UserInfo.id ? userData.cards : (players[i].cards == null ? null : players[i].cards);
				//playerNew.transform.localScale /= TableSizeDelta;
				bool isShowDown = (final
				|| (_balances.ContainsKey(stage) && _balances[stage].ContainsKey(players[i].user.id) && _balances[stage][players[i].user.id].IsShowDown)
				|| players[i].user.id == GameHelper.UserInfo.id
				);
				playerNew.SetDistributive(players[i], _sharedData, _sharedData.events[0], playerCards, isPreFlop,
						false, false);
				playerNew.ProcessDistributive(true);

				// проверить на отсутствие значения
				if (_balances.ContainsKey(stage) && _balances[stage].ContainsKey(players[i].user.id))
					playerNew.SetAmount(_balances[stage][players[i].user.id].UserBalance);
				else
				if (_balances.Last().Value.ContainsKey(players[i].user.id))
					playerNew.SetAmount(_balances.Last().Value[players[i].user.id].UserBalance);


				//if (!isShowDown && !final)
				playerNew.Cards.ForEach(x => x.SetCloseState());
				if (isShowDown || final)
					playerNew.Cards.ForEach(x => x.SetOpenState());

				//if (final)
				//{
				//	bool winSee = playerNew.WinSee;
				//	playerNew.ShowCombinations(false);
				//	if (winSee && playerNew.ExistsLowCombinations())
				//	{
				//		playerNew.WinSee = winSee;
				//		playerNew.ShowCloneLowCombinations();
				//	}
				//}

				_playersUI.Add(playerNew);
				playerNew.transform.localScale = Vector3.one * 0.8f;

#if !UNITY_STANDALONE
				playerNew.transform.localScale = Vector3.one * 1.5f;

				if (players[i].user.id == UserController.User.id)
					playerNew.transform.localScale = Vector3.one * 1.8f;
#endif


				if (existswin == -1)
					if (_balances.ContainsKey(stage))
					{
						if (_balances[stage].ContainsKey(players[i].user.id))
						{
							decimal balance = _balances[stage][players[i].user.id].Balance;
							if (balance > 0)
							{
								_placeManager.Places.Bets[place].gameObject.SetActive(true);
								_placeManager.Places.Bets[place].transform.localScale = Vector3.one * 0.8f;
#if !UNITY_STANDALONE
							//	playerNew.transform.localScale = Vector3.one * 1f;
#endif
								_placeManager.Places.Bets[place].SetValue(_table, balance);
							}
						}
						//playerNew.InitState(_balances[stage][players[i].User.Id].Action, false);
					}

				string action = "";
				string beforeAction = "";

				if (_balances.ContainsKey(stage))
				{
					if (_balances[stage].ContainsKey(players[i].user.id))
					{
						action = _balances[stage][players[i].user.id].Action;
					}
					else
						foreach (var b in _balances.Keys)
							if (_balances[b].ContainsKey(players[i].user.id))
								beforeAction = _balances[b][players[i].user.id].Action;
				}
				else
					if (_balances.Last().Value.ContainsKey(players[i].user.id))
					action = _balances.Last().Value[players[i].user.id].Action;

				if (action != "all-in" && action != "fold")
					isAllAllIn = false;

				playerNew.InitState(action, false);
				if (action == "fold" || (string.IsNullOrEmpty(action) && beforeAction != "all-in"))
				{
					playerNew.Cards.ForEach(x => x.gameObject.SetActive(false));
				}

			}

			// Проверка что все ушли в алл ин или фолд
			if (isAllAllIn)
				foreach (var pl in _playersUI)
					foreach (var plc in pl.Cards)
						plc.SetOpenState();

			if (existswin != -1)
			{
				for (int i = 0; i < _placeManager.Places.Bets.Count; i++)
				{
					if (i == existswin)
					{
						if (_sharedData.IsFinish)
						{
							for (int j = _sharedData.events.Count - 1; j >= 0; j--)
							{
								if (_sharedData.events[j].distribution_event_type.slug == "transfer-of-winnings"
								|| _sharedData.events[j].distribution_event_type.slug == "rake-payout"
								|| _sharedData.events[j].distribution_event_type.slug == "refund-of-funds")
								{
									if (!_winSumm.ContainsKey((ulong)_sharedData.events[j].user_id))
										_winSumm.Add((ulong)_sharedData.events[j].user_id, 0);

									_winSumm[(ulong)_sharedData.events[j].user_id] += System.Math.Abs((decimal)_sharedData.events[j].bank_amount_delta);
								}
							}
						}
						else
						{
							//Winer.text = "Not Finished";
							//Pot.text = "Not Finished";
						}
					}
				}
				// Показываем победные стопки с фишками
				foreach (var key in _winSumm.Keys)
				{
					for (int i = 0; i < players.Count; i++)
					{
						if (players[i].user.id != key) continue;

						int place = players[i].place - 1;
						_placeManager.Places.Bets[place].gameObject.SetActive(true);
						_placeManager.Places.Bets[place].SetValue(_table, _winSumm[key]);
					}
				}

			}

			if (_bank.ContainsKey(stage))
			{
				_totalBank.SetValue($"Total Pot : <color=#C68C43>{it.Helpers.Currency.String(_bank[stage])}</color>", _table, _bank[stage]);
			}
			_totalBank.GetComponent<it.UI.Elements.TotalPot>().ResetPosition();
			cards.ForEach(x => { Destroy(x.gameObject); });
			cards.Clear();
			for (int i = 0; (i < shared_cards.Count && i < cardsOpen); i++)
			{
				if (i >= CardPlaces.Count)
				{
					it.Logger.Log("i >= CardPlaces.Count");
					break;
				}

				var card = shared_cards[i];
				var cardPanel = Instantiate(GameCardUIPrefab, CardPlaces[i]);
				cardPanel.InitOnlyVisual(card);
				RectTransform cardRT = cardPanel.GetComponent<RectTransform>();
				cardRT.anchorMin = Vector2.zero;
				cardRT.anchorMax = Vector2.one;
				cardRT.anchoredPosition = Vector2.zero;
				cardRT.sizeDelta = Vector2.zero;
				//cardPanel.IsEmitMoveUpEvent = false;

				cardPanel.OnIsMoveUp = (val) =>
				{
					if (!final) return;
					if (val)
						_totalBank.GetComponent<it.UI.Elements.TotalPot>().MoveUp();
				};

				cardPanel.Init(card);
				if (final)
				{
					//cardPanel.Init(_sharedData.WinCombinations, _sharedData.WinCombinations);
				}
				if (!final)
					cardPanel.ForceNoWin();
				//cardPanel.transform.localScale /= TableSizeDelta;
				cards.Add(cardPanel);
			}

			if (final)
			{
				//Task.Run(async () =>
				//{
				//	await ProcessCombinations(shared_cards);
				//});
				_showCombinationsTask = StartCoroutine(ProcessCombinationsCoroutine(shared_cards));
			}

		}

		private void ShowCardsCombinations(List<DistributionPlayerCombination> combinations, List<DistributionCard> shared_cards)
		{
			foreach (var cardPanel in cards)
			{
				cardPanel.ApplyCombinationOnly(combinations, combinations, true, false);
			}
		}

		private IEnumerator ProcessCombinationsCoroutine(List<DistributionCard> shared_cards)
		{
			List<DistributionPlayerCombinationGroup> _combGroup = DistributionPlayerCombinationGroup.CombinationsGroup(_sharedData.WinCombinations);

			yield return new WaitForSeconds(0.5f);

			foreach (var cmbGroup in _combGroup)
			{
				if (cmbGroup.Hight.Count > 0)
				{
					if (!_isFinish) yield break;
					yield return ShowOneGroupCombinations(cmbGroup.Hight, shared_cards);
				}
				if (cmbGroup.Low.Count > 0)
				{
					if (!_isFinish) yield break;
					yield return ShowOneGroupCombinations(cmbGroup.Low, shared_cards);
				}
			}
		}

		private IEnumerator ShowOneGroupCombinations(List<DistributionPlayerCombination> combinations
		, List<DistributionCard> shared_cards)
		{
			ShowCardsCombinations(combinations, shared_cards);
			//await Task.Delay(500);

			if (!_isFinish) yield break;
			foreach (var play in _playersUI)
				play.ShowCardsCombinations(combinations);

			if (!_isFinish) yield break;
			yield return new WaitForSeconds(0.3f);

			//foreach (var pl in _sharedData.Players)
			//{
			if (!_isFinish) yield break;
			foreach (PlayerGameIcone play in _playersUI)
				play.ShowCombinations(combinations);
			//}
			if (!_isFinish) yield break;
			yield return new WaitForSeconds(1f);

			if (!_isFinish) yield break;
			foreach (var play in _playersUI)
				play.NoWin();

		}

		public void ScreenShotTable()
		{
			StartCoroutine(UploadPNG());
		}

		IEnumerator UploadPNG()
		{
			yield return new WaitForEndOfFrame();
			Texture2D tex = new Texture2D((int)RectScreenShot.width, (int)RectScreenShot.height, TextureFormat.RGB24, false);

			tex.ReadPixels(RectScreenShot, 0, 0);
			tex.Apply();

			string pathNow = Application.dataPath + "/" + path + $"/{ScreenshotName}.png";
			File.WriteAllBytes(pathNow, tex.EncodeToPNG());
			it.Logger.Log(pathNow);
		}

		public void ClickBB()
		{
			it.Logger.Log("BB (?)");
		}


		//#if UNITY_ANDROID || UNITY_WEBGL || UNITY_IOS
		//		public void DistributionNowSet(bool bl)
		//    {
		//        DistributionList.SetActive(bl == false);
		//        DistributionNow.SetActive(bl);
		//    }

		//    public void ButtonBack()
		//    {
		//        if (DistributionList.activeSelf)
		//        {
		//            Hide();
		//        }
		//        else
		//        {
		//            DistributionNowSet(false);
		//            //TitleTxt.text = "HAND_HISTORY".Localized();
		//        }
		//    }
		//#endif

	}
}