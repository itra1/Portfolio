using System;
using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

using UnityEditor;

using UnityEngine;

namespace it.Network.Rest
{

	public class BuildResponce
	{
		public bool error;
		public List<BuildItem> items;
	}

	public class BuildItem
	{
		public ulong id;
		public string file;
		public string app_version;
		public int app_version_type;
		public int app_type;
		public int crucially;
		public string descr;
		public ulong user_id;
		public string date_time_upload;
		public string date_time_changed;
		public string app_version_type_title;
		public string app_type_title;
		public string download_url;

		public bool IsCitical => crucially == 1;
		public bool IsAndroid => app_version_type == 3;
		public bool IsProduction => app_type == 2;

		public static bool Max(string sourceVersion, string newVersion)
		{
			try
			{
				string[] s1 = sourceVersion.Split(new char[] { '.' });
				string[] s2 = newVersion.Split(new char[] { '.' });

				for (int i = 0; i < s2.Length; i++)
				{
					if (i >= s1.Length)
						return true;

					int v1 = int.Parse(s1[i]);
					int v2 = int.Parse(s2[i]);

					if (v2 > v1)
						return true;
					else if (v2 < v1)
						return false;
					else
						continue;

				}
				return false;
			}
			catch
			{
				return false;
			}
		}

	}

	public class SitdownInfo
	{
		
		public decimal buy_in;
		
		public string password;
		
		public int place;

		public SitdownInfo(decimal buy_in, int place)
		{
			this.buy_in = buy_in;
			this.place = place;
		}

		public SitdownInfo(decimal buy_in, int place, string password)
		{
			this.buy_in = buy_in;
			this.place = place;
			this.password = password;
		}
	}

	public class MoneyBody
	{
		
		public decimal amount;

		public MoneyBody(decimal amount)
		{
			this.amount = amount;
		}

		public MoneyBody()
		{

		}
	}

	public class RaiseInfo
	{
		public RaiseInfo(decimal raise)
		{
			this.raise = raise;
		}

		
		public decimal raise;
	}

	public class UsersLimitedRespone
	{
		
		public UserLimited[] data;
	}

	public class ObserversUsersRespone
	{
		
		public UserLimited[] observers;

		public UsersLimitedRespone GetUsersLimitedRespone
		{
			get { return new UsersLimitedRespone { data = observers }; }
		}
	}

	public class UserLimitedRespone
	{
		
		public UserLimited data;
	}

	public class DistributionDataResponse
	{
		
		public DistributionSharedData shared_data;
		
		public SocketEventDistributionUserData user_data;
	}

	public class ChinaDistributionDataResponse
	{
		
		public ChinaDistributionSharedData shared_data;
		
		public SocketEventDistributionUserData user_data;
	}

	public class ActiveEventMetaData
	{
		
		public bool? can_check;
		
		public bool? can_call;
		
		public bool? can_raise;
		
		public bool? can_allin;
		
		public decimal? min_raise;
		
		public decimal? max_raise;
		
		public decimal? to_call;
		
		public string raise_type;
		
		public decimal? straddle_size;
	}

	public class DistributionSharedData
	{
		
		public ulong? id;
		
		public bool? is_active;
		
		public string created_at;
		
		public string updated_at;
		
		public ActiveEventMetaData active_event_meta_data;
		
		public List<DistributionCard> shared_cards;
		
		public List<DistributionSharedDataPlayer> players;
		
		public List<DistributionStage> stages;
		
		public DistributionEvent active_event;
		
		public List<DistributionEvent> events;
		
		public List<DistributionBank> banks;


		public decimal MaxRaise => active_event_meta_data != null && active_event_meta_data.max_raise != null ? (decimal)active_event_meta_data.max_raise : -1;

		public decimal MinRaise => active_event_meta_data != null && active_event_meta_data.min_raise != null ? (decimal)active_event_meta_data.min_raise : -1;

		public decimal CallCount => active_event_meta_data != null && active_event_meta_data.to_call != null ? (decimal)active_event_meta_data.to_call : 0;

		public bool IsFixedRaise => MinRaise == MaxRaise;

		public bool CanRaise => active_event_meta_data != null && active_event_meta_data.can_raise != null && (bool)active_event_meta_data.can_raise && active_event_meta_data.raise_type != "bet";
		public bool CanAllin => active_event_meta_data != null && active_event_meta_data.can_allin != null && (bool)active_event_meta_data.can_allin;
		public bool CanBet => active_event_meta_data != null && active_event_meta_data.can_raise != null && (bool)active_event_meta_data.can_raise && active_event_meta_data.raise_type == "bet";

		public bool CanCall
		{
			get
			{
				if (active_event_meta_data != null && active_event_meta_data.can_raise != null) return (bool)active_event_meta_data.can_call;
				else return false;
			}
		}

		public bool CanCheck
		{
			get
			{
				if (active_event_meta_data != null && active_event_meta_data.can_raise != null) return (bool)active_event_meta_data.can_check;
				else return false;
			}
		}

		public string CallTime
		{
			get
			{
				return active_event != null && active_event.calltime_at != null ? active_event.calltime_at : "";
			}
		}
		public bool TryGetPlayer(ulong id, out DistributionSharedDataPlayer player)
		{
			player = GetPlayer(id);
			return player != null;
		}
		public DistributionSharedDataPlayer GetPlayer(ulong id)
		{
			int index = players.FindIndex(x => x.user.id == id);

			//int index = Array.FindIndex(
			//		Players,
			//		delegate (DistributionSharedDataPlayer it)
			//		{
			//			return it.user.Id == id;
			//		}
			//);

			return index != -1 ? players[index] : null;
		}

		public bool IsFinish =>
				stages != null && stages.Count > 0 &&
				stages[stages.Count - 1].distribution_stage_type.slug == "finish";

		public bool IsLostMoney => MePlayer != null && MePlayer.amount <= 0;

		public bool IsWaitDistribution
		{
			get
			{
				int index = players.FindIndex(x => x.user.id == GameHelper.UserInfo.id);
				//int index = Array.FindIndex(
				//		Players,
				//		delegate (DistributionSharedDataPlayer it)
				//		{
				//			return it.user.Id == GameHelper.UserInfo.Id;
				//		}
				//);

				return index != -1 ? false : true;
			}
		}

		public bool IsStraddle()
		{
			return stages != null && stages.Count > 0 && stages[stages.Count - 1].distribution_stage_type.slug == "straddle";
		}
		public bool IsShowDown()
		{
			return stages != null && stages.Count > 0 && stages[stages.Count - 1].distribution_stage_type.slug == "showdown";
		}
		public bool IsPreflop()
		{
			return stages != null && stages.Count > 0 && stages[stages.Count - 1].distribution_stage_type.slug == "preflop";
		}
		public bool IsFlop()
		{
			return stages != null && stages.Count > 0 && stages[stages.Count - 1].distribution_stage_type.slug == "flop";
		}
		public bool IsRiver()
		{
			return stages != null && stages.Count > 0 && stages[stages.Count - 1].distribution_stage_type.slug == "river";
		}
		public bool IsTurn()
		{
			return stages != null && stages.Count > 0 && stages[stages.Count - 1].distribution_stage_type.slug == "turn";
		}
		public bool IsCardsState()
		{
			return IsPreflop() || IsFlop() || IsRiver() || IsTurn();
		}
		public bool IsDealerChoiseStage =>
			stages != null && stages.Count > 0 && stages[stages.Count - 1].distribution_stage_type.slug == "dealer_choice";
		public bool ExistsShowDown()
		{
			return stages != null && stages.Count > 0 && stages.FindIndex(x => x.distribution_stage_type.slug == "showdown") >= 0;
		}
		public bool ExistsRiver()
		{
			return stages != null && stages.Count > 0 && stages.FindIndex(x => x.distribution_stage_type.slug == "river") >= 0;
		}
		public bool ExistsFlop()
		{
			return stages != null && stages.Count > 0 && stages.FindIndex(x => x.distribution_stage_type.slug == "flop") >= 0;
		}
		public bool ExistsPreflop()
		{
			return stages != null && stages.Count > 0 && stages.FindIndex(x => x.distribution_stage_type.slug == "preflop") >= 0;
		}
		public bool ExistsFinish()
		{
			return stages != null && stages.Count > 0 && stages.FindIndex(x => x.distribution_stage_type.slug == "finish") >= 0;
		}
		public bool StageDistribToPreflop()
		{
			return stages != null
				&& stages.Count > 0
				&& (stages.FindIndex(x => x.distribution_stage_type.slug == "preflop") == -1
						|| stages.FindIndex(x => x.distribution_stage_type.slug == "preflop") == stages.Count - 1);
		}

		public bool IsFirstBetInRound
		{
			get
			{
				return players.All(player => player.active_stage_bet == null || !(player.active_stage_bet > 0));
			}
		}

		public List<DistributionPlayerCombination> MeCombinations
		{
			get
			{
				var me = mePlayer;
				return me == null ? new List<DistributionPlayerCombination>() : me.combinations;
			}
		}

		public DistributionPlayerCombination[] AllCombinations
		{
			get
			{
				var allCombinations = new List<DistributionPlayerCombination>();
				foreach (var player in players)
				{
					if (player.combinations != null) allCombinations.AddRange(player.combinations);
				}

				return allCombinations.ToArray();
			}
		}

		public List<DistributionPlayerCombination> WinCombinations
		{
			get
			{
				var allCombinations = new List<DistributionPlayerCombination>();
				foreach (var player in players)
					if (player.combinations != null && player.IsWin)
						allCombinations.AddRange(player.combinations.FindAll(x => x.is_best));

				return allCombinations.OrderByDescending(x => x.order).ToList();
			}
		}

		public List<DistributionPlayerCombination> MaxHightCombintion()
		{
			//List<DistributionPlayerCombination> combinations = new List<DistributionPlayerCombination>();
			//foreach (var comb in WinCombinations)
			//	if (comb.category != "low")
			//	{
			//		if (combinations.Count <= 0)
			//			combinations.Add(comb);
			//		else
			//		{
			//			if (combinations.Exists(x => x.worth == comb.worth))
			//				combinations.Add(comb);
			//		}
			//	}
			return DistributionPlayerCombination.MaxHightCombintion(WinCombinations);
		}

		public List<DistributionPlayerCombination> MaxLowCombintion()
		{
			//List<DistributionPlayerCombination> combinations = new List<DistributionPlayerCombination>();
			//foreach (var comb in WinCombinations)
			//	if (comb.category == "low")
			//	{
			//		if (combinations.Count <= 0)
			//			combinations.Add(comb);
			//		else
			//		{
			//			if (combinations.Exists(x => x.worth == comb.worth))
			//				combinations.Add(comb);
			//		}
			//	}
			return DistributionPlayerCombination.MaxLowCombintion(WinCombinations);
		}

		public decimal GetTotalBank()
		{
			decimal totalBetInRound = players.Sum(item => item.BetInRound);
			decimal totalBank = banks.Sum(item => item.amount);

			return totalBank;
		}

		public decimal MaxBetInPlayers()
		{

			decimal max = 0;

			for (int i = 0; i < players.Count; i++)
			{
				var plBer = IsPreFlop ? players[i].bet : players[i].BetInRound;
				if (max < plBer)
					max = plBer;
			}


			return max;
		}

		public bool IsMeAllin()
		{
			var me = mePlayer;
			return me != null && GetMaxBet() >= me.amount;
		}

		public decimal GetMaxBet()
		{
			return players.Select(player => player.bet).Prepend(0).Max();
		}

		public Dictionary<int, decimal> GetWins()
		{
			Dictionary<int, decimal> idWins = new Dictionary<int, decimal>();
			foreach (var item in banks)
			{
				if (item.candidates != null)
				{
					foreach (var candidate in item.candidates) idWins.Add(candidate.distribution_player_session_id, item.amount);
				}
			}

			return idWins;
		}

		public bool IsPostflop => GetActiveStage().distribution_stage_type.slug != "preflop";

		public bool IsPreFlop => !IsPostflop;

		public bool IsNeedCall => MePlayer != null && MePlayer.ActiveStageStateName == "need-call";

		public bool IsMeActive => active_event?.IsMeActive ?? false;

		public CallInfo BetInfo
		{
			get
			{
				var maxBet = GetMaxBet();
				var me = MePlayer;
				if (me == null) return new CallInfo(false);
				var callCount = maxBet - me.bet;
				if (callCount > me.amount) callCount = me.amount;

				return new CallInfo(callCount, callCount >= me.amount, maxBet, true);
			}
		}

		public bool IsEmptyBetAndNotFisrtWord()
		{
			if (IsPreFlop)
			{
				return MePlayer != null && MePlayer.bet == 0 && !IsFirstWord();
			}

			return false;
		}

		public bool IsAvailableCheck()
		{
			return MePlayer != null && MePlayer.bet >= GetMaxBet();
		}

		public bool IsFirstWord()
		{
			if (MePlayer != null)
			{
				if (IsPreFlop)
				{
					var bbPlayer = GetBigBlindPlayer();
					if (bbPlayer == null)
					{
						it.Logger.Log("Нет игрока с большим блайндом");
						return false;
					}
					players = players = players.OrderBy(x => x.place).ToList();
					//Array.Sort(Players, (session, playerSession) => session.place > playerSession.place ? 1 : 0);

					for (int i = 0; i < players.Count; i++)
					{
						if (players[i].role == DistributionSharedDataPlayer.RoleBigBlind)
						{
							if (i == players.Count - 1) return MePlayer.user.id == players[0].user.id;
							else return MePlayer.user.id == players[i + 1].user.id;
						}
					}

					return MePlayer.place == bbPlayer.place + 1;
				}
				else
				{
					return false;// MePlayer.role == DistributionSharedDataPlayer.RoleSmallBlind;
				}
			}

			return false;
		}

		public DistributionSharedDataPlayer GetNextPlayer(int currentPlace)
		{
			var nextPlayerPlace = currentPlace;//места с сервера приходят начиная с 1, поэтому без приведения равно следующему месту из массива
			if (nextPlayerPlace >= players.Count) return players[0];

			return players[nextPlayerPlace];
		}

		public DistributionSharedDataPlayer GetRaisePlayer()
		{
			return players.FirstOrDefault(item => item.ActiveStageStateName == "raise");
		}

		public DistributionSharedDataPlayer GetBigBlindPlayer()
		{
			return players.FirstOrDefault(item => item.role == DistributionSharedDataPlayer.RoleBigBlind || item.role == DistributionSharedDataPlayer.RoleDealerBb);
		}

		public DistributionSharedDataPlayer GetSmallBlindPlayer()
		{
			return players.FirstOrDefault(item => item.role == DistributionSharedDataPlayer.RoleSmallBlind);
		}

		public DistributionSharedDataPlayer MePlayer
		{
			get
			{
				if (mePlayer == null)
				{
					foreach (var item in players)
					{
						if (item.user.id == GameHelper.UserInfo.id)
						{
							mePlayer = item;
							break;
						}
					}
				}

				return mePlayer;
			}
		}

		private DistributionSharedDataPlayer mePlayer = null;

		public DistributionSharedDataPlayer GetPlayerByPlace(int place)
		{
			return players.FirstOrDefault(item => item.place == place);
		}

		public DistributionStage GetActiveStage()
		{
			foreach (var item in stages)
			{
				if (item.is_active) return item;
			}

			return stages[stages.Count - 1];
		}

		public decimal GetPotBet()
		{
			var placeMe = 1;
			foreach (var item in players)
			{
				if (item.user.id == GameHelper.UserInfo.id)
				{
					placeMe = item.place;
					break;
				}
			}

			decimal bet = 0;
			foreach (var item in players)
			{
				if (item.place == placeMe - 1)
				{
					bet += item.bet * 3;
				}
				else if (item.place < placeMe)
				{
					bet += item.bet;
				}
			}

			bet += GetTotalBank();

			return bet;
		}
	}

	public class CallInfo
	{
		public bool IsAllIn = false;
		public bool IsMe = false;
		public decimal? Count = -1;
		public decimal MaxBet = -1;

		public CallInfo(bool b)
		{
			IsMe = b;
		}

		public CallInfo(decimal callCount, bool isAllIn, decimal maxBet, bool isMe)
		{
			Count = callCount;
			MaxBet = maxBet;
			IsAllIn = isAllIn;
			IsMe = isMe;
		}

		public bool IsCheck => CountCall == 0;

		public bool IsCall => CountCall > 0;

		public decimal CountCall => Count ?? -1;
	}

	public class DistributionSharedDataPlayer : Garilla.Games.IMyAfk
	{
		public const string RoleBigBlind = "big-blind";
		public const string RoleSmallBlind = "small-blind";
		public const string RoleDealer = "dealer";
		public const string RoleDealerSb = "dealer+sb";
		public const string RoleDealerBb = "dealer+bb";
		public const string RolePlayerBb = "bb-payer";
		public const string RolePlayerAnte = "ante-payer";
		public const string RoleDealerBbp = "dealer+bbp";

		
		public ulong distribution_session_id;
		
		public int place;
		
		public UserLimited user;
		
		public decimal amount;
		
		public decimal bet;
		
		public decimal? active_stage_bet;
		
		public string role;
		
		public List<string> roles;
		/// <summary>
		/// Сумма выйгрыша
		/// </summary>
		
		public decimal? winning_amount;
		/// <summary>
		/// Комиссия взымаемаая казино
		/// </summary>
		
		public decimal? rake_amount;
		
		public UserStat user_stat;
		
		public UserSessionStat session_stat;
		
		public DistributionSessionState state;
		
		public DistributionSessionState active_stage_state;
		
		public List<DistributionCard> cards;
		
		public List<DistributionPlayerCombination> combinations;
		
		public BingoGame bingo_game;
		/// <summary>
		/// тгрок не совершает действия, отошел
		/// </summary>
		
		public bool is_resting;
		/// <summary>
		/// Будет установлен АФК
		/// </summary>
		
		public bool skip_distributions_will_be_set;
		/// <summary>
		/// Активен ли AFK
		/// </summary>
		
		public bool skip_distributions;
		/// <summary>
		/// Оставшееся время действия AFK
		/// </summary>
		
		public float skip_distributions_seconds_left;
		/// <summary>
		/// Можем ли выполнить AFK
		/// </summary>
		
		public bool can_set_skip_distributions;
		/// <summary>
		/// Оставшееся время до применения AFK
		/// </summary>
		
		public float can_set_skip_distributions_after;

		public bool SkipDistributionsWellBeSet => skip_distributions_will_be_set;
		public bool SkipDistributions => skip_distributions;
		public float SkipDistributionsTime => skip_distributions_seconds_left;
		public bool CanSkipDistributions => can_set_skip_distributions;
		public float CanSkipDistributionsTime => can_set_skip_distributions_after;

		public string ActiveStageStateName => active_stage_state != null ? active_stage_state.slug : "";

		public bool IsWin => winning_amount != null && winning_amount > 0;

		public decimal BetInRound => active_stage_bet ?? 0;
	}

	/// <summary>
	/// Состояние игрока
	/// </summary>

	public class DistributionSessionState
	{
		
		public string slug;
		
		public string title;
	}

	public interface IBingoInfo
	{
		public int BingoMaxHand { get; }
		public decimal SpinMax { get; }
	}


	public class BingoInfo : IBingoInfo
	{
		
		public int hands_max;
		
		public List<decimal> spin_variants;
		int IBingoInfo.BingoMaxHand => hands_max;

		decimal IBingoInfo.SpinMax => spin_variants[0];
	}

	public class BingoGameResponse
	{
		public BingoGame data;
	}


	public class BingoGame : IBingoInfo
	{
		
		public int id;
		
		public int table_player_session_id;
		
		public int deck_id;
		
		public int hands_left;
		
		public int hands_max;
		
		public bool? is_win;
		
		public bool? is_paid;
		
		public int[] matched_cards_ids;
		
		public string finished_at;
		
		public decimal? winning_amount;
		
		public List<decimal> winning_amounts;
		
		public int state_id;
		
		public int? spin_id;
		
		public string created_at;
		
		public string updated_at;
		
		public BingoGameDesk deck;
		
		public BingoMatrix[][] matrix;
		
		public List<decimal> spin_variants;

		public List<decimal> WinningAmounts => winning_amounts ?? new List<decimal>();

		int IBingoInfo.BingoMaxHand => hands_max;

		decimal IBingoInfo.SpinMax => spin_variants[0];

		//public int IBingoInfo.BingoMaxHand => HandsMax;
		//public decimal IBingoInfo.SpinMax => SpinVariants.;

	}


	public class BingoMatrix
	{
		
		public Card card;
		
		public bool is_matched;
	}


	public class BingoGameDesk
	{
		
		public int id;
		
		public int deck_type_id;
		
		public string created_at;
		
		public string updated_at;
		
		public Card[] cards;
	}


	public class DistributionStage
	{
		
		public ulong id;
		
		public bool is_active;
		
		public int? raises_count;
		
		public string created_at;
		
		public string updated_at;
		
		public DistributionStageType distribution_stage_type;
		
		public List<AfterStageAmount> after_stage_amounts;
	}


	public class AfterStageAmount
	{
		
		public ulong distribution_session_id;
		
		public decimal after_stage_amount;
	}



	public class DistributionStageType
	{
		
		public string slug;
		
		public string title;
	}


	public class DistributionBank
	{
		
		public decimal amount;
		
		public decimal stage_amount;
		
		public DistributionBankSource[] sources;
		
		public DistributionBankCandidate[] candidates;


		public class DistributionBankSource
		{
			
			public float amount;
			
			public int? distribution_player_session_id;
			
			public decimal StageAmount;
		}


		public class DistributionBankCandidate
		{
			
			public int distribution_player_session_id;
		}
	}


	public class DistributionEvent
	{
		public ulong id;
		public ulong? distribution_id;
		public ulong distribution_stage_id;
		public ulong? user_id;
		public decimal? bank_amount_delta;
		public bool? is_active;
		public bool? is_auto_action;
		public string calltime_at;
		public ulong? eventable_id;
		public string eventable_type;
		public int? timebank_used_times;
		public int? timebank_max_usages;
		public DistributionEventType distribution_event_type;
		public string created_at;
		public string updated_at;

		public bool TimeBankUsageReady => (int)timebank_max_usages - (int)timebank_used_times > 0;

		public decimal BankAmountDelta => bank_amount_delta ?? 0;

		public class DistributionEventType
		{
			
			public string slug;
			
			public string title;
		}

		public bool IsMeActive
		{
			get { return user_id == UserController.User.id; }
		}
	}


	public class DistributionCard
	{
		
		public ulong id;
		
		public bool? is_open;
		
		public bool? is_folded;
		
		public bool under_user_control;
		
		public int? cp_row;
		
		public int? cp_position;
		
		public Card card;

		public ulong CardId => (card?.id ?? ulong.MaxValue);
		public int CpRow => (cp_row ?? -1);
		public int CpPosition => (cp_position ?? -1);
		public bool IsFolded => (is_folded ?? false);
		public bool IsFreeDistribution => CpRow == -1 && CpPosition == -1 && under_user_control;
	}


	public class Card
	{
		
		public ulong id;
		
		public ulong? card_type_id;
		
		public ulong? deck_id;
		
		public CardType card_type;
		
		public Deck deck;
	}


	public class Deck
	{
		
		public ulong? id;
		
		public ulong? deck_type_id;
		
		public DeckType deck_type;
		
		public string cards;
		
		public string created_at;
		
		public string updated_at;
	}

	public class DistributionPlayerCombinationGroup
	{
		public int Order;
		public List<DistributionPlayerCombination> Hight = new List<DistributionPlayerCombination>();
		public List<DistributionPlayerCombination> Low = new List<DistributionPlayerCombination>();

		public static List<DistributionPlayerCombinationGroup> CombinationsGroup(List<DistributionPlayerCombination> combinations)
		{

			List<DistributionPlayerCombinationGroup> _combGroup = new List<DistributionPlayerCombinationGroup>();

			foreach (DistributionPlayerCombination comb in combinations)
			{
				if (!_combGroup.Exists(x => x.Order == comb.order))
					_combGroup.Add(new DistributionPlayerCombinationGroup() { Order = (int)comb.order });

				var tComb = _combGroup.Find(x => x.Order == (int)comb.order);
				if (comb.category == "low")
					tComb.Low.Add(comb);
				else
					tComb.Hight.Add(comb);
			}
			return _combGroup;
		}
	}


	public class DistributionPlayerCombination
	{
		
		public ulong? id;
		
		public int? twist_number;
		
		public long? worth;
		
		public int? order;
		
		public bool is_best;
		
		public List<ulong> card_ids;
		
		[CanBeNull] public string category;
		
		public GameCardCombination game_card_combination;

		public bool IsContainsCard(ulong id)
		{
			return card_ids.Contains(id);
		}

		public bool Equalence(DistributionPlayerCombination h)
		{

			foreach (var el in h.card_ids)
				if (!card_ids.Contains(el))
					return false;
			return true;

		}


		public static List<DistributionPlayerCombination> MaxHightCombintion(List<DistributionPlayerCombination> WinCombinations)
		{
			List<DistributionPlayerCombination> combinations = new List<DistributionPlayerCombination>();
			foreach (var comb in WinCombinations)
				if (comb.category != "low")
				{
					if (combinations.Count <= 0)
						combinations.Add(comb);
					else
					{
						if (combinations.Exists(x => x.worth == comb.worth))
							combinations.Add(comb);
					}
				}
			return combinations;
		}

		public static List<DistributionPlayerCombination> MaxLowCombintion(List<DistributionPlayerCombination> WinCombinations)
		{
			List<DistributionPlayerCombination> combinations = new List<DistributionPlayerCombination>();
			foreach (var comb in WinCombinations)
				if (comb.category == "low")
				{
					if (combinations.Count <= 0)
						combinations.Add(comb);
					else
					{
						if (combinations.Exists(x => x.worth == comb.worth))
							combinations.Add(comb);
					}
				}
			return combinations;
		}

	}


	public class DistributionHistoryDataResponse
	{
		
		//public DistributionHistorySharedData SharedData;
		public DistributionSharedData shared_data;
		
		public SocketEventDistributionUserData user_data;
		
		public GameRule game_rule;
	}



	public class DistributionHistorySharedData
	{
		
		public ulong? id;
		
		public bool? is_active;
		
		public string created_at;
		
		public string updated_at;
		
		public DistributionCard[] shared_cards;
		
		public DistributionSharedDataPlayer[] players;
		
		public DistributionStage[] stages;
		
		public DistributionEvent[] events;
		
		public DistributionBank[] banks;

		public bool IsFinish =>
				stages != null && stages.Length > 0 &&
				stages[stages.Length - 1].distribution_stage_type.slug == "finish";

		public DistributionSharedDataPlayer GetPlayer(ulong id)
		{
			int index = Array.FindIndex(
					players,
					delegate (DistributionSharedDataPlayer it)
					{
						return it.user.id == id;
					}
			);

			return index != -1 ? players[index] : null;
		}

		public bool TryGetPlayer(ulong id, out DistributionSharedDataPlayer player)
		{
			player = GetPlayer(id);
			return player != null;
		}

		public DistributionPlayerCombination[] WinCombinations
		{
			get
			{
				var allCombinations = new List<DistributionPlayerCombination>();
				foreach (var player in players)
				{
					if (player.combinations != null && player.IsWin) allCombinations.AddRange(player.combinations);
				}

				return allCombinations.ToArray();
			}
		}

	}


	public class DistributionTableHistoryResponse
	{
		
		public DistributionHistoryDataResponse[] data;
	}
	public class GameCardCombinationInGameResponse
	{
		public GameCardCombinationInGame data;
	}


	public class GameCardCombinationInGame
	{
		
		public GameCardCombination high;
		
		public GameCardCombination low;
	}


	public class GameCardCombination
	{
		
		public string slug;
		
		public string title;

	}


	public class TableUpDownResponse
	{
		
		public TableSessionResponse data;
	}


	public class TableActionResponse
	{
		
		public TablePlayerSession data;
	}


	public class TableAfkResponse
	{
		
		public TablePlayerSession data;
	}


	public class TableSessionResponse
	{
		
		public int? id;
		
		public int? table_id;
		
		public int? user_id;
		
		public int? place;
		
		public int? ca_moves_count;
		
		public bool? cp_is_fantasy;
		
		public bool? is_resting;
		
		public bool? is_already_participated;
		
		public bool? participates_in_next_distribution;
		
		public bool? is_bb_accepted;
		
		public bool? need_bb_accept;
		
		public float? amount;
		
		public float? amount_buffer;
		
		public UserLimited user;
		
		public UserSessionStat statistics;
		
		[CanBeNull] public string rest_timeout_at;
		
		[CanBeNull] public string finished_at;
		
		[CanBeNull] public string created_at;
		
		[CanBeNull] public string updated_at;
		
		[CanBeNull] public Table table;
		
		public int? sortedPlace;
	}


	public class TableObserverSessionResponse
	{
		
		public TableObserverSession data;
	}


	public class TableObserverSession
	{
		
		public int? id;
		
		public int? table_id;
		
		public int? user_id;
		
		public Table table;
		
		public UserLimited user;
		
		public string finished_at;
		
		public string created_at;
		
		public string updated_at;
	}


	public class CardType
	{
		
		public int? id;
		
		public int? card_type_id;
		
		public int? card_value_id;
		
		public int? card_suit_id;
		
		public CardValue card_value;
		
		public CardSuit card_suit;
	}


	public class CardValue
	{
		
		public int? id;
		
		public int? number;
		
		public string name;
		
		public string honneur;
		
		public string created_at;
		
		public string updated_at;
	}


	public class CardSuit
	{
		
		public int? id;
		
		public string name;
		
		public string card_name;
		
		public string system_id;
		
		public string created_at;
		
		public string updated_at;
	}


	public class JackpotResponece
	{
		
		public JackpotInfoResponse data;
	}

	public class CashierDepositeResponse
	{
		public CashierDeposite data;
	}

	public class CashierDeposite
	{
		public string qr;
		public string external_id;
		public string addr;
	}


	public class JackpotInfoResponse
	{
		
		public decimal amount;
		
		public JackpotWinnerResponse[] winners;
		
		public JackpoBlinds[] blinds;
	}



	public class JackpoBlinds
	{
		
		public double small_blind;
		
		public double big_blind;
		
		public double payout;
		
		public double result;
		
		public string game;

	}



	public class JackpotWinnerResponse
	{
		
		public int id;
		
		public int user_id;
		
		public string nickname;
		
		public string avatar_url;
		
		public string date;
		
		public float amount;
		
		public float big_blind_size;
		
		public float small_blind_size;
		
		public string game_rule;
		
		public CardType[] cards;

		public DateTime Date => DateTime.Parse(date);
	}



	public class PlayerSessionOptionsResponse
	{
		
		public PlayerSessionOptions data;
	}


	public class PlayerSessionOptions
	{
		
		public bool? skip_distributions;
		
		public bool? fold_to_any_bet;
		
		public bool? is_bb_accepted;
		
		public bool auto_add_to_max_buy_in;
		public bool skip_straddle;

		public PlayerSessionOptions()
		{
			this.fold_to_any_bet = false;
			this.skip_distributions = false;
			this.is_bb_accepted = false;
		}

		public PlayerSessionOptions(bool fold_to_any_bet, bool skip_distributions)
		{
			this.fold_to_any_bet = fold_to_any_bet;
			this.skip_distributions = skip_distributions;
			this.is_bb_accepted = false;
		}

		public PlayerSessionOptions(bool fold_to_any_bet, bool skip_distributions, bool is_bb_accepted)
		{
			this.fold_to_any_bet = fold_to_any_bet;
			this.skip_distributions = skip_distributions;
			this.is_bb_accepted = is_bb_accepted;
		}
	}


	public class TablesResponse
	{
		
		public List<Table> data;
	}


	public class TableResponse
	{
		
		public Table data;
	}



	public class TableChatMessage
	{
		
		public ulong? id;
		
		public ulong? table_id;
		
		public Table table;
		
		public User user;
		
		public ulong? user_id;
		
		public string message;
		
		public string created_at;
		
		public string updated_at;

		public DateTime CreateDate => DateTime.Parse(created_at);

	}


	public class TableChatGetResponse
	{
		
		public List<TableChatMessage> data;
	}

	public class TableChatSendResponse
	{
		
		public TableChatMessage data;
	}


	public class TableChatPostMessage
	{
		
		public string message;
	}



	public class ReferenceDataResponse
	{
		
		public ReferenceData data;
	}


	public class ReferenceData
	{
		public List<Language> languages;
		public SmileSet[] smile_sets;
		public SmilePresets[] smile_presets;
		public Smile[] smiles_to_send;
		public List<string> kauri_currency;
		public List<string> kauri_currency_out;
		public AvatarCategory[] avatar_categories;
		public List<RankItemResponse> ranks;
		public List<WelcomeBonnusListItem> welcome_bonus;
		public Dictionary<string, double> timebank_price;
		public Dictionary<string, TableOptions> create_vip_table_options;
		public List<Classifier> time_bank_types;

		public AvatarObject GetAvatarObject(ulong id)
		{
			AvatarCategory[] avatarCategories = avatar_categories;
			if (avatarCategories != null)
			{
				for (int i = 0; i < avatarCategories.Length; i++)
				{
					AvatarObject[] avatarObjects = avatarCategories[i].avatars;
					if (avatarObjects != null)
					{
						for (int a = 0; a < avatarCategories[i].avatars.Length; a++)
						{
							if (id == avatarObjects[a].id)
							{
								return avatarObjects[a];
							}
						}
					}
				}
			}
			return null;
		}
	}


	public class SmileSet
	{
		
		public int? id;
		
		public string name;
		
		public List<Smile> smiles;
	}


	public class SmilePresets
	{
		
		public int? id;
		
		public string name;
		
		public Smile[][] smiles;
	}


	public class Smile
	{
		
		public ulong? id;
		
		public string url;
		
		public string name;
	}

	public class LeaderBoardResponseDataResponse
	{
		public LeaderBoardResponseData data;
	}


	public class LeaderBoardResponseData
	{
		
		public ulong id;
		
		public string nickname;
		
		public string avatar;
		
		public double amount;
		
		public double prize;
		
		public int place;
	}


	public class LeaderBoardResponse
	{
		
		public LeaderBoardData data;
	}
	public class LeaderBoardData
	{
		
		public LeaderBoardResponseData[] leaderboard;
		public LeaderBoardResponseData current_user_place;
	}
	/*public class LeaderBoardsResponse
	{
		
		public LeaderBoardResponse[] data;
	}*/

	//public class 


	public class TableOptionsResponse
	{
		
		public Dictionary<string, TableOptions> options;
	}


	public class TableOptions
	{
		
		public string id;
		
		public Dictionary<string, Level> levels;
		
		public int[] action_time;
		
		public int players_count_min;
		
		public int players_count_max;
		
		public int auto_start_players_count_min;
		
		public int auto_start_players_count_max;
		
		public float time_bank_min;
		
		public float time_bank_max;
	}


	public class Level
	{
		
		public string id;
		
		public string name;
		
		public decimal small_blind;
		
		public decimal big_blind;
		
		public decimal min_buy_in;
		
		public decimal max_buy_in;
	}



	public class CreateTableInfo
	{
		
		public string id;
		
		public string level_id;
		
		public int action_time;
		
		public int players_count;
		
		public int auto_start_players_count;
		
		public int time_bank;
		
		public string password;

		public CreateTableInfo(string id, string level_id, int action_time, int players_count, int auto_start_players_count, int time_bank, string password)
		{
			this.id = id;
			this.level_id = level_id;
			this.action_time = action_time;
			this.players_count = players_count;
			this.auto_start_players_count = auto_start_players_count;
			this.time_bank = time_bank;
			this.password = password;
		}
		public CreateTableInfo()
		{

		}
	}
}