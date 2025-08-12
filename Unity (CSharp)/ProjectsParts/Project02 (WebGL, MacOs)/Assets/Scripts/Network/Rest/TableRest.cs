using Garilla.Games;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

using UnityEngine;

namespace it.Network.Rest
{
	public class Table : UpdatedMaterial
	{
		
		public ulong id;
		[it.Update]		
		public string name;
		[it.Update]		
		public int game_rule_id;
		[it.Update]		
		public float amount_control_time;
		[it.Update]		
		public float distribution_interval;
		[it.Update]		
		public GameRule game_rule;
		[it.Update]		
		public int? deck_type_id;
		[it.Update]		
		public int? avg_pot;
		[it.Update]		
		public DeckType deck_type;
		[it.Update]		
		public bool is_private;
		[it.Update]		
		public bool has_active_distribution;
		[it.Update]		
		public int players_count;
		[it.Update]		
		public int? players_autostart_count;
		[it.Update]		
		public int? calltime;
		[it.Update]		
		public int? action_time;
		[it.Update]		
		public decimal big_blind_size;
		[it.Update]		
		public decimal buy_in_min;
		[it.Update]		
		public decimal buy_in_max;
		[it.Update]		
		public float? player_vpip_min;
		[it.Update]		
		public float? min_raise_size;
		[it.Update]		
		public bool? is_disable_chat;
		[it.Update]		
		public bool? is_auto_prolong;
		[it.Update]		
		public bool? is_auto_restart;
		[it.Update]		
		public bool? is_auto_create;
		[it.Update]		
		public int game_type_id;
		[it.Update]		
		public bool is_all_or_nothing;
		[it.Update]		
		public bool? imminent_closing_notification;
		[it.Update]		
		public bool? is_gps_limit;
		[it.Update]		
		public bool? is_ip_limit;
		[it.Update]		
		public bool? is_device_limit;
		[it.Update]		
		public float? distributions_per_hour;
		[it.Update]		
		public bool? is_captcha_enabled;
		[it.Update]		
		public bool? is_captcha_limit;
		[it.Update]		
		public bool? is_twist_tt_times;
		[it.Update]		
		public float? rake;
		[it.Update]		
		public int? cap;
		[it.Update]		
		public int? game_duration;
		[it.Update]		
		public int? autochecks_count_to_rest;
		[it.Update]		
		public int? rest_timeout;
		[it.Update]		
		public bool? is_active;
		[it.Update]		
		public bool is_at_the_table;
		[it.Update]		
		public int current_players_count;
		[it.Update]		
		public List<PlaceReserve> table_player_reservations;
		[it.Update]		
		public TablePlayerSession[] table_player_sessions;
		[it.Update]		
		public string created_at;
		[it.Update]		
		public string updated_at;
		[it.Update]		
		public GameType game_type;
		[it.Update]		
		public bool is_vip;
		/// <summary>
		/// Ante на столе
		/// </summary>
		[it.Update]		
		public float? ante;
		/// <summary>
		/// Средний Vpip
		/// </summary>
		[it.Update]		
		public float? average_vpip;
		/// <summary>
		/// Средний Pot
		/// </summary>
		[it.Update]		
		public decimal? average_pot;
		/// <summary>
		/// Тип игры: Диллер чойз
		/// </summary>
		[it.Update]		
		public bool is_dealer_choice;
		/// <summary>
		/// Данные Бинго
		/// </summary>
		[it.Update]		
		public BingoInfo bingo_info;
		/// <summary>
		/// Необходимо сесть за стол с суммой выхода exit_amount
		/// </summary>
		[it.Update]		
		public bool can_join_only_with_exit_amount;
		[it.Update]		
		public string can_join_only_with_exit_amount_until;
		[it.Update]
		public List<string> available_dealer_choices;
		/// <summary>
		/// Сумма с которой игрок вышел со стола
		/// 
		/// Записывается в случае если игрок увеличил свой баланс и сохраняется в течении полтора часа
		/// </summary>
		[it.Update]		
		public decimal? exit_amount;

		public PokerGameType PokerGameType;

		//public Color RecordColor => Settings.Games._gameColors.Find(x => x.GameType == PokerGameType).Color;

		public bool IsFaceToFace => players_count <= 2;


		public static int maxPlayersStat = 6; //Для тестов. См. maxPlayers и скрипт TablesUIManager
		public bool IsAfk
		{
			get
			{
				var me = MePlayer;
				return me != null && me.is_resting;
			}
		}
		public bool isAtTheTable => MePlayer != null;
		public bool needBbAccept
		{
			get
			{
				return MePlayer != null ? MePlayer.needBbAccept : false;
			}
		}
		public bool InGame
		{
			get
			{
				return MePlayer != null ? MePlayer.is_already_participated ?? false : false;
			}
		}
		public bool isExistFreePlace
		{
			get
			{
				return current_players_count < players_count;
			}
		}
		public TablePlayerSession MePlayer
		{
			get
			{
				int index = Array.FindIndex(
						table_player_sessions,
						delegate (TablePlayerSession it)
						{
							return it.user_id == UserController.User.id;
						}
				);

				return index != -1 ? table_player_sessions[index] : null;
			}
		}
		public bool IsExistPlayer(ulong id)
		{
			int index = Array.FindIndex(
					table_player_sessions,
					delegate (TablePlayerSession it)
					{
						return it.user_id == id;
					}
			);

			return index != -1 ? true : false;
		}
		public int mePlace
		{
			get
			{
				return MePlayer != null ? MePlayer.placeAtTable : -1;
			}
		}
		public int MaxPlayers
		{
			get
			{
				return players_count;
				//return maxPlayersStat;
			}
		}
		public decimal BuyInMinEURO
		{
			get
			{
				return (decimal)(buy_in_min * big_blind_size);
			}
		}
		public decimal BuyInMaxEURO
		{
			get
			{
				return (decimal)(buy_in_max * big_blind_size);
			}
		}
		public decimal BuyInUSD(decimal buy)
		{
			return (decimal)(buy * big_blind_size);
		}
		public int MePlaceInSorted
		{
			get
			{
				int index = Array.FindIndex(
						sortedPlayerSessions,
						delegate (TablePlayerSession it)
						{
							return it.user_id == UserController.User.id;
						}
				);

				return index != -1 ? sortedPlayerSessions[index].sortedPlace : -1;
			}
		}
		public TablePlayerSession[] sortedPlayerSessions
		{
			get
			{
				if (table_player_sessions.Length == 0) return table_player_sessions;
				Array.Sort(table_player_sessions, (session, playerSession) => session.place > playerSession.place ? 1 : 0);
				var sortedPlayers = table_player_sessions;

				int minPlace = sortedPlayers[0].placeAtTable;
				for (int i = 0; i < sortedPlayers.Length; i++)
				{
					if (minPlace > 5) minPlace = 0;
					sortedPlayers[i].sortedPlace = minPlace;
					minPlace++;
				}

				return sortedPlayers;
			}
		}
		public bool IsFreePlace(int place)
		{
			foreach (var player in table_player_sessions)
			{
				if (player.placeAtTable == place) return false;
			}

			return true;
		}
		public TablePlayerSession GetPlayerByPlace(int place)
		{
			foreach (var player in table_player_sessions)
			{
				if (player.placeAtTable == place) return player;
			}

			return null;
		}
		public bool IsReservePlace(int place)
		{
			foreach (var player in table_player_reservations)
			{
				if (player.place - 1 == place) return true;
			}

			return false;
		}
		public decimal SmallBlindSize
		{
			get
			{
				return big_blind_size / 2;
			}
		}

		public string GetCategory()
		{
			string sprKey = "micro";
			var elem = it.Settings.GameSettings.GameNames.Find(x => x.GameType == (global::GameType)game_rule_id);

			if (!UserController.ReferenceData.create_vip_table_options.ContainsKey(elem.Slug))
				elem = it.Settings.GameSettings.GameNames.Find(x => x.GameType == global::GameType.Holdem);

			if (elem != null)
			{
				var opt = UserController.ReferenceData.create_vip_table_options[elem.Slug];
				foreach (var key in opt.levels.Keys)
				{
					//if (opt.levels[key].min_buy_in == table.BuyInMin && opt.levels[key].max_buy_in == table.BuyInMax)
					//	sprKey = opt.levels[key].name;
					if (opt.levels[key].small_blind <= SmallBlindSize /*&& opt.levels[key].max_buy_in == table.BuyInMax*/)
						sprKey = opt.levels[key].name;
				}
			}
			return sprKey.ToLower();
		}

	}

	
	public class PlaceReserve
	{
		
		public int id;
		
		public int table_id;
		
		public int user_id;
		
		public int place;
		
		public string started_at;
		
		public string finish_at;

		public DateTime StartedDate => DateTime.Parse(started_at);
		public DateTime FinishDate => DateTime.Parse(finish_at);
	}


	
	public class GameRule
	{
		
		public int? id;
		
		public string name;
		
		public string system_id;
		
		public string created_at;
		
		public string updated_at;
		
		public TablePlayerSession[] table_player_sessions;

		public DateTime CreateDate => DateTime.Parse(created_at);
		public DateTime UpdateDate => DateTime.Parse(updated_at);
	}

	
	public class DeckType
	{
		
		public int? id;
		
		public string name;
		
		public string description;
		
		public CardType[] card_types;
		
		public string created_at;
		
		public string updated_at;
	}

	public class GameType
	{
		
		public int? id;
		
		public string name;
		
		public string slug;
	}

	public class TablePlayerOptionsResponse{
		public TablePlayerOptions data;
	}
	public class TablePlayerOptions : Garilla.Games.IMyAfk
	{
		
		public bool? skip_distributions;
		
		public bool? fold_to_any_bet;
		
		public bool? is_bb_accepted;
		
		public bool? auto_add_to_max_buy_in;
		
		public bool? skip_distributions_will_be_set;
		
		public bool? can_set_skip_distributions;
		
		public float? can_set_skip_distributions_after;
		
		public float? skip_distributions_seconds_left;
		
		public float? skip_distributions_duration;

		public bool SkipDistributionsWellBeSet => (bool)skip_distributions_will_be_set;
		public bool SkipDistributions => (bool)skip_distributions;
		public float SkipDistributionsTime => (float)skip_distributions_seconds_left;
		public bool CanSkipDistributions => (bool)can_set_skip_distributions;
		public float CanSkipDistributionsTime => (float)can_set_skip_distributions_after;
	}

	public class TablePlayerSessionResponse
	{
		public List<TablePlayerSession> active_table_player_sessions;
	}
	//active_table_player_sessions

	public class TablePlayerSession : Garilla.Games.IMyAfk
	{
		
		public ulong? id;
		
		public ulong table_id;
		//public Table table;
		
		public ulong user_id;
		
		public int place;
		
		public int? ca_moves_count;
		
		public bool? cp_is_fantasy;
		
		public bool is_resting;
		
		public bool? is_already_participated;
		
		public bool? participates_in_next_distribution;
		
		public bool? is_bb_accepted;
		
		public bool need_bb_accept;
		
		public string amount_check_at;
		
		public decimal amount;
		
		public decimal? amount_buffer;
		
		public UserLimited user;
		
		public string rest_timeout_at;
		
		public string finished_at;
		
		public string created_at;
		
		public string updated_at;
		
		public UserStat user_stat;
		
		public UserSessionStat session_stat;
		
		public int sortedPlace { get; set; }
		/// <summary>
		/// Активен ли AFK
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

		public decimal amountBuffer => (decimal)(amount_buffer == null ? 0m : amount_buffer);

		public int placeAtTable
		{
			get
			{
				return place - 1;
			}
		}
		public bool needBbAccept
		{
			get
			{
				return need_bb_accept != null ? need_bb_accept : false;
			}
		}

		public bool IsMe => user.id == GameHelper.UserInfo.id;

		public bool SkipDistributionsWellBeSet => skip_distributions_will_be_set;
		public bool SkipDistributions => skip_distributions;
		public float SkipDistributionsTime => skip_distributions_seconds_left;
		public bool CanSkipDistributions => can_set_skip_distributions;
		public float CanSkipDistributionsTime => can_set_skip_distributions_after;
	}



}