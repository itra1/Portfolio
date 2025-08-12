using it.Api;

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using static it.Network.Rest.UserProfile;

namespace it.Network.Rest
{
	public class UserResponse
	{
		public User data;
	}

	/// <summary>
	/// Данные По пользователю
	/// </summary>
	public class User
	{
		/// <summary>
		/// Идентификатор пользователя
		/// </summary>
		public ulong id;
		/// <summary>
		/// Идентификатор страны
		/// </summary>
		//public int? gp_id;
		/// <summary>
		/// Мыло
		/// </summary>
		public string email;
		/// <summary>
		/// Телефон
		/// </summary>
		public string phone;
		/// <summary>
		/// Псевдоним
		/// </summary>
		public string nickname;
		/// <summary>
		/// Доступен ли пользователь для авторизации
		/// </summary>
		public bool? is_active;
		/// <summary>
		/// Дата изменения пароля
		/// </summary>
		public string password_change_at;
		/// <summary>
		/// Флаг прохождения авторизации
		/// </summary>
		public bool verified;
		/// <summary>
		/// Url на аватар
		/// </summary>
		public string avatar_url;
		/// <summary>
		/// Идентификатор страны
		/// </summary>
		public ulong? CountryId => country == null ? 179 : country.id;
		/// <summary>
		/// Уровень авторизации
		/// </summary>
		//public int? auth_level;
		//public string gp_created_at;
		/// <summary>
		/// Дата создания пользователя
		/// </summary>
		public string created_at;
		/// <summary>
		/// Дата изменения пользователя
		/// </summary>
		//public string updated_at;
		/// <summary>
		/// Кошелек пользователя
		/// </summary>
		public UserWallet user_wallet;
		/// <summary>
		/// Статистика игрока
		/// </summary>
		public UserStat user_stat;
		/// <summary>
		/// Профиль
		/// </summary>
		public UserProfile user_profile;
		/// <summary>
		/// Страна
		/// </summary>
		public Country country;
		public bool can_change_nickname;
		/// <summary>
		/// Включен ли кешер на iOS
		/// </summary>
		public bool cashier_available;
		public string AvatarUrl => user_profile != null ? user_profile.AvatarUrl : "";
		public UserLimited limited
		{
			get
			{
				return new UserLimited(this);
			}
		}
	}

	public class UserNoteResponse
	{
		public UserNote data;
	}

	/// <summary>
	/// Заметка с пользователю (лейбл)
	/// </summary>
	public class UserNote
	{
		/// <summary>
		/// Событие изменения данных
		/// </summary>
		public UnityEngine.Events.UnityAction OnChange;

		/// <summary>
		/// Идентификатор игрока
		/// </summary>
		public ulong user_id;
		/// <summary>
		/// Информационная подпись к заметке
		/// </summary>
		public string message = "";
		/// <summary>
		/// Цвет лейбла (стикера) на игрока
		/// </summary>
		public int color = 0;

		public void Save()
		{
			UserApi.NoteCreate(this, (result) =>
			{
				if (result.IsSuccess)
				{
					OnChange?.Invoke();
				}
			});
		}
	}

	/// <summary>
	/// Данные авторизацтт
	/// </summary>
	public class AuthUserResponse
	{
		/// <summary>
		/// Пользователь
		/// </summary>
		public User User;
		/// <summary>
		/// Токен
		/// </summary>
		public TokenRest Token;
	}

	public class PokerStatisticResponse
	{
		public PokerStatistic data;
	}

	public class PokerStatistic
	{
		public DateValues day;
		public DateValues week;
		public DateValues month;
		public DateValues year;

		public class DateValues
		{
			public decimal value;
			public int hands;
			public double rake;
		}

	}

	/// <summary>
	/// Данные кошелька
	/// </summary>
	public class UserWallet
	{
		/// <summary>
		/// Идентификатор кошелька
		/// </summary>
		public ulong? id;
		/// <summary>
		/// Идентификатор привязанного пользователя
		/// </summary>
		public ulong? user_id;
		/// <summary>
		/// Баланс
		/// </summary>
		public decimal? amount;
		//public string created_at;
		//public string updated_at;
	}

	public class UserStatResponse
	{
		public UserStat data;
	}
	/// <summary>
	/// Данные статистики
	/// </summary>
	public class UserStat
	{
		public ulong? id;
		public ulong? user_id;
		public string section;
		public float? distributions_count;
		public float? vpip_distributions_count;
		public float? preflop_raise_count;
		public float? ats_raise_count;
		public float? three_bet_count;
		public float? vpip;
		public float? pfr;
		public float? ats;
		public float? three_bet;
		public float? continuation_bet;
		public float? fold_continuation_bet;
		public float? call_continuation_bet;
		public float? raise_continuation_bet;
		public float? showdown_participate;
		public float? showdown_win;
		public float? aggressive_distributions;
	}
	public class UserSessionStat
	{
		
		public float? vpip_distributions_count;
		
		public float? aggressive_distributions;
		
		public decimal balance;
		
		public decimal total_buy_in;
	}

	public class TokenRest
	{
		public string access_token;
		public string token_type;
		public int expires_in;
		public int refresh_in;
	}
	public class UserLimited
	{
		public ulong id;
		public string nickname;
		public string avatar_url;
		public Country country;
		public UserProfileLimited user_profile;

		public string AvatarUrl => !string.IsNullOrEmpty(avatar_url)
		? string.Copy(avatar_url) : ((user_profile != null && user_profile.avatar_url != null) ? user_profile.avatar_url : "");

		public UserLimited() { }

		public UserLimited(User user)
		{
			id = user.id;
			nickname = user.nickname;
			avatar_url = user.AvatarUrl;
			country = user.country;
			//UserStat = user.UserStat;
		}
	}
	/// <summary>
	/// Страна
	/// </summary>
	public class Country
	{
		/// <summary>
		/// Внутренний идентификатор страны
		/// </summary>
		public ulong? id;
		/// <summary>
		/// Идентификатор страны в специальном списке
		/// </summary>
		public int? gp_id;
		/// <summary>
		/// Название страны полное
		/// </summary>
		public string title;
		/// <summary>
		/// Название страны сокращенное
		/// </summary>
		public string short_title;
		/// <summary>
		/// ССылка на флаг
		/// </summary>
		public string flag;
		/// <summary>
		/// Локализованное название стран
		/// </summary>
		public LocalizationsNames localizations;
	}

	/// <summary>
	/// Структура локализованных названий
	/// </summary>
	public class LocalizationsNames
	{
		public string ru;
	}

	public class UserProfile : IUserTimeBank
	{
		public ulong? id;
		public ulong? user_id;
		public ulong language_id;
		public Language language;
		public int time_bank_type_id;
		public Classifier time_bank_type;
		public Betting betting;
		public SoundGet sound;
		public SwitchChipDisplay switch_chip_display;
		public TableTheme table_theme;
		public AvatarObject avatar;
		public string avatar_url;
		public int? sp;
		public int? time_bank;
		public int? time_bank_paid;
		public string rank_check_planned_at;
		public RankRecord[] rank_records;
		public string created_at;
		public string updated_at;

		public string AvatarUrl => avatar != null ? avatar.url : "";
		public UserProfilePost PostProfile
		{
			get
			{
				return new UserProfilePost(language_id, time_bank_type_id, betting, sound, switch_chip_display, table_theme, (avatar == null ? null : avatar.id));
			}
		}

		public void SetAvatar(AvatarObject avatar)
		{
			this.avatar = avatar;
			avatar_url = avatar.url;
		}

		public int TimeBank => time_bank == null ? 0 : (int)time_bank;
		public int TimeBankPaid => time_bank_paid == null ? 0 : (int)time_bank_paid;
	}

	public class UserTimeBanks : IUserTimeBank
	{
		public int? time_bank;
		public int? time_bank_paid;

		public int TimeBank => time_bank == null ? 0 : (int)time_bank;
		public int TimeBankPaid => time_bank_paid == null ? 0 : (int)time_bank_paid;
	}

	public interface IUserTimeBank
	{
		int TimeBank { get; }
		int TimeBankPaid { get; }
	}

	public class PromotionsDataResponse
	{
		public PromotionsData data;
	}

	public class PromotionsData
	{
		public List<PromotionsDataItem> three_bet;
		public List<PromotionsDataItem> wtsd_race;
		public List<PromotionsDataItem> aon_race;
		public List<PromotionsDataItem> game_manager;
	}
	public class PromotionsDataItem
	{
		public string level;
		public float count;
		public float limit;
	}

	public class UserBlockResponse
	{
		public List<UserBlock> data;
	}

	public class UserBlock
	{
		
		public ulong id;
		
		public BlockUserInfo blocked_user;
		public class BlockUserInfo
		{
			
			public ulong id;
			
			public string nickname;
		}

	}

	public class CurrencyConversionResult
	{
		public List<CurrencyConversionBlock> data;
	}

	public class ServersResponse
	{
		public Servers servers;
	}
	public class Servers
	{
		public List<string> game;
		public List<string> delivery;
		public List<string> apps;
	}

	public class CurrencyConversionBlock
	{
		public ulong id;
		public CurrencyConversionItem currencyFrom;
		public CurrencyConversionItem currencyTo;
		public double exchange;
	}

	public class CurrencyConversionItem
	{
		public ulong id;
		public string name;
		public string abbreviation;
		public string sign;
		public bool is_active;
		public bool is_default;
		public string created_at;
		public string updated_at;
	}

}