using System;
using System.Collections.Generic;

using it.Network.Rest;

[Serializable]
public class LoginInfo
{
	
	public string email;
	
	public string password;
}


[Serializable]
public class AuthData
{
	public TokenRest AuthInfo;
	public User UserData;

	public AuthData(TokenRest authInfo, User userData)
	{
		this.AuthInfo = authInfo;
		this.UserData = userData;
	}
}

[Serializable]
public class UserResponse
{
	
	public User user;
}

[Serializable]
public class UserProfileResponse
{
	
	public UserProfile data;
}

[Serializable]
public class UserProfilePost
{
	
	public ulong? language_id;
	
	public int? time_bank_type_id;
	
	public ulong? avatar_id;
	
	public Betting betting;
	
	public SoundGet sound;
	
	public SwitchChipDisplay switch_chip_display;
	
	public TableTheme table_theme;

	public UserProfilePost() { }
	public UserProfilePost(ulong language_id, int time_bank_type_id, Betting betting, SoundGet sound, SwitchChipDisplay switch_chip_display,
			TableTheme table_theme, ulong? avatar_id)
	{
		this.language_id = language_id;
		this.sound = sound;
		this.betting = betting;
		this.switch_chip_display = switch_chip_display;
		this.table_theme = table_theme;
		this.time_bank_type_id = time_bank_type_id;
		this.avatar_id = avatar_id;
	}
}

[Serializable]
public class UserProfileRespone
{
	
	public UserProfile data;
}


[Serializable]
public class UserProfileLimited
{
	
	public int? id;
	
	public int? user_id;
	
	public int? language_id;
	
	public Language language;
	
	public int? sp;
	
	public string avatar_url;
	
	public RankRecord[] rank_records;
	
	public string created_at;
	
	public string updated_at;

	public string avatarUrl => avatar_url != null ? avatar_url : "";
}

[Serializable]
public class Betting
{
	
	public BettingButton button1 = new BettingButton();
	
	public BettingButton button2 = new BettingButton();
	
	public BettingButton button3 = new BettingButton();
	
	public BettingButton button4 = new BettingButton();
	
	public bool dont_round_to_nearest_blind;

	//public Betting(BettingButton button1, BettingButton button2, BettingButton button3, BettingButton button4, bool dont_round)
	//{
	//	this.button1 = new BettingButton(button1.opening_bet_size, button1.normal_bet_size);
	//	this.button2 = new BettingButton(button2.opening_bet_size, button2.normal_bet_size);
	//	this.button3 = new BettingButton(button3.opening_bet_size, button3.normal_bet_size);
	//	this.button4 = new BettingButton(button4.opening_bet_size, button4.normal_bet_size);
	//	dont_round_to_nearest_blind = dont_round;
	//}
}

[Serializable]
public class BettingButton
{
	
	public string opening_bet_size;
	
	public string normal_bet_size;

	//public BettingButton(string opening_bet_size, string normal_bet_size)
	//{
	//	this.opening_bet_size = opening_bet_size;
	//	this.normal_bet_size = normal_bet_size;
	//}
}

[Serializable]
public class SwitchChipDisplay
{
	
	public bool on;
	
	public string chip_display;
	
	public bool alternative_chip;

	//public SwitchChipDisplay(bool on, string chip_display, bool alternative_chip)
	//{
	//	this.on = on;
	//	this.chip_display = chip_display;
	//	this.alternative_chip = alternative_chip;
	//}
}

[Serializable]
public class TableTheme
{
	
	public string front_deck;
	
	public string back_deck;
	
	public string felt;
	
	public string background;

	//public TableTheme(string front_deck, string back_deck, string felt, string background)
	//{
	//	this.front_deck = front_deck;
	//	this.back_deck = back_deck;
	//	this.felt = felt;
	//	this.background = background;
	//}
}

[Serializable]
public class SoundGet
{
	
	public bool on;
	
	public int background;
	
	public int dealer_voice;
	
	public int sound_effect;
	
	public bool mute_critical_alert;

	public SoundGet Clone()
	{
		return new SoundGet()
		{
			on = this.on,
			background = this.background,
			dealer_voice = this.dealer_voice,
			mute_critical_alert = this.mute_critical_alert,
			sound_effect = this.sound_effect
		};
	}
}

[Serializable]
public class RankRecord
{
	
	public int? id;
	
	public ulong? rank_id;
	
	public int? user_profile_id;
	
	public string created_at;
	
	public string updated_at;
	
	public Rank rank;
	
	public string finished_at;

	public DateTime CreateDate => DateTime.Parse(created_at);
}

[Serializable]
public class Rank
{
	
	public int? id;
	
	public string name;
	
	public string slug;
	
	public int? climb_sp;
	
	public int? maintain_sp;
	
	public int? cashback;
	
	public int? period;
	
	public int? timebank;
	
	public int? timebank_cost_usd;

	public int ClimbSp => climb_sp ?? 0;
	public int MaintainSp => maintain_sp ?? 0;
}

[Serializable]
public class AuthResponse
{
	
	public string access_token;
	
	public string token_type;
	
	public int expires_in;
	
	public int refresh_in;
}


[Serializable]
public class UserWallet
{
	
	public int? id;
	
	public int? user_id;
	
	public float? amount;
	
	public string created_at;
	
	public string updated_at;
}

[Serializable]
public class UserWalletTransactionRespone
{
	
	public List<UserWalletTransaction> data;
	
	public IndexRequestMetaData meta;
}
[Serializable]
public class UserWalletRequestRespone
{
	
	public List<UserRequestTransaction> data;
	
	public IndexRequestMetaData meta;
}

[Serializable]
public class IndexRequestMetaData
{
	
	public int? page = 1;
	
	public int? per_page = 20;
	
	public int? total_items_count;
	
	public int? pages_count;
	
	public string order_by = "id";
	
	public string order_direction = "asc";
	
	public string search = "string_to_find";
	
	public string[] filters;
	
	public bool without_pagination = true;
}

public interface ITransactionType
{
	string TransactionType { get; }
	System.DateTime CreateAt { get; }
}

[Serializable]
public class UserWalletTransaction : ITransactionType
{
	public string TransactionType => "transaction";
	
	public string id;
	
	public int? user_wallet_id;
	
	public decimal? amount;
	
	//public int? WalletTransactionableId;
	
	//public string WalletTransactionableType;
	
	public int? user_wallet_transaction_type_id;
	
	public string system_comment;
	
	public string created_at;
	
	public string updated_at;
	
	public string request_id;
	
	public WalletTransactionable wallet_transactionable;
	
	public UserWalletTransactionTipe user_wallet_transaction_type;

	public System.DateTime CreateAt => System.DateTime.Parse(created_at);
	public System.DateTime UpdatedAt => System.DateTime.Parse(updated_at);

}

[Serializable]
public class UserRequestTransaction: ITransactionType
{
	public string TransactionType => "request";
	
	public string id;
	
	public ulong system_id;
	
	public ulong user_id;
	
	public string card_number;
	
	public decimal? amount;
	
	public CurrencyPay currency;
	
	public decimal? euro_amount;
	
	public PayoutMethod payout_method;
	
	public string status;
	
	public bool has_unseen_change;
	
	public string created_at;

	public System.DateTime CreateAt => System.DateTime.Parse(created_at);

}

[Serializable]
public class CurrencyPay
{
	
	public ulong id;
	
	public string name;
	
	public string abbreviation;
	
	public string sign;
}

public class PayoutMethod
{
	
	public ulong id;
	
	public string name;
	
	public string slug;
}

[Serializable]
public class WalletTransactionable
{

	
	public string id;
	
	public string card_number;
}

[Serializable]
public class UserWalletTransactionTipe
{
	
	public string slug;
	
	public string title;
}

[Serializable]
public class PasswordBody
{
	
	public string password;

	public PasswordBody(string password)
	{
		this.password = password;
	}

	public PasswordBody()
	{

	}
}



[Serializable]
public class PaymentBody
{
	
	public float amount;
	
	public Requisites requisites;

	public PaymentBody(float amount, Requisites requisites)
	{
		this.amount = amount;
		this.requisites = requisites;
	}

	[Serializable]
	public class Requisites
	{
		
		public string cardHolder;
		
		public string cardNumber;
		
		public int expireMonth;
		
		public int expireYear;
		
		public int cvv;

		public Requisites(string cardHolder, string cardNumber, int expireMonth, int expireYear, int cvv)
		{
			this.cardHolder = cardHolder;
			this.cardNumber = cardNumber;
			this.expireMonth = expireMonth;
			this.expireYear = expireYear;
			this.cvv = cvv;
		}
	}
}

[Serializable]
public class PaymentResponse
{
	
	public string data;
}

[Serializable]
public class ReplenishmentTransactionResponse
{
	
	public ReplenishmentTransaction replenishment_transaction;
}
[Serializable]
public class DistributinStartCooldown
{
	
	public Table table;
	
	public float delay;
}

[Serializable]
public class ReplenishmentTransaction
{
	
	public ulong? id;
	
	public User user;
	
	public ulong? user_id;
	
	public int? replenishment_method_id;
	
	public ReplenishmentMethod replenishment_method;
	
	public int? state_id;
	
	public Classifier state;
	
	public int? payment_system_id;
	
	public int? payment_system_state_id;
	
	public Classifier payment_system_state;
	
	public int? currency_id;
	
	public Currency currency;
	
	public string client_initiate_data;
	
	public decimal? amount;
	
	public string created_at;
	
	public string updated_at;
}

[Serializable]
public class ReplenishmentMethod
{
	
	public int? id;
	
	public string name;
	
	public string slug;
	
	public bool? is_active;
	
	public bool? is_default;
}

[Serializable]
public class Classifier
{
	
	public int id;
	
	public string name;
	
	public string slug;
	
	public string short_name;
}


[Serializable]
public class Currency
{
	
	public int? id;
	
	public string name;
	
	public string abbrevation;
	
	public string sign;
	
	public bool? is_active;
	
	public bool? is_default;
}

public class LanguageResponse{
	public List<Language> languages;
}

[Serializable]
public class Language
{
	
	public ulong? id;
	
	public string slug;
	
	public string name;
	
	public string flag;
}

[Serializable]
public class AvatarCategory
{
	
	public ulong? id;
	
	public int? order_no;
	
	public string name;
	
	public AvatarObject[] avatars;
}

[Serializable]
public class AvatarObject
{
	
	public ulong id;
	
	public int? order_no;
	
	public string url;
	
	public string name;
}

#region Rengs

public class RanksResponce
{
	
	public List<RankItemResponse> data;
}
public class UserRankResponce
{
	
	public RankUser data;
}
public class RankHistoryResponce
{
	
	public RankHistoryStruct data;
}
public class AuthToken
{
	
	public string data;
}

public class RankUser
{
	
	public double current_points;
	
	public RankItemResponse current_rank;
	
	public RankItemResponse next_rank;
}

public class RankHistoryStruct
{
	
	public ulong id;
	
	public RankItemResponse rank;
	
	public string created_at;
}

public class RankItemResponse
{
	
	public ulong id;
	
	public string name;
	
	public string slug;
	
	public double price;
	
	public float rakeback;
	
	public string image;
}

#endregion


[Serializable]
public class WelcomeBonnusListResponce
{
	
	public WelcomeBonnusListItem[] data;
}

[Serializable]
public class WelcomeBonnusListItem
{
	
	public int level;
	
	public string name;
	
	public int multiplier;
}

[Serializable]
public class WelcomeBonnusResponce
{
	
	public WelcomeBonusData data;
}

[Serializable]
public class WelcomeBonusData
{
	
	public string bonus_time_limit;
	
	public WelcomeBonus last_bonus;
	
	public WelcomeBonus next_level;
}

[Serializable]
public class WelcomeBonus
{
	
	public int level;
	
	public string name;
	
	public int? current_stage;
	
	public double multiplier;
	
	public int? total_stages;
	
	public string bonus_expire_at;
	
	public double bonus_amount;
	
	public bool? finished;

	public DateTime ExpiredDate => DateTime.Parse(bonus_expire_at);
}

[Serializable]
public class TimebankPriceResponce
{
	
	public Dictionary<string, double> data;
}

[Serializable]
public class FindUserResponse
{
	
	public List<FindUser> data;
}

[Serializable]
public class FindUser
{
	
	public ulong id;
	
	public string nickname;
	
	public string avatar_url;
}

public class CasinoUser
{
	
	public string login;
}

/*
 * {
  "data": {
    "bonus_time_limit": "string",
    "last_bonus": {
      "name": "string",
      "current_stage": 0,
      "total_stages": 0,
      "bonus_expire_at": "string",
      "bonus_amount": "Unknown Type: float",
      "finished": true
    }
  }
}
 * 
 */

#region SupportMessage
[Serializable]
public class Operator
{
	
	public string display_name;
}
[Serializable]
public class Datum
{
	
	public ulong id;
	
	public int operator_id;
	
	public Operator @operator;
	
	public int ticket_id;
	
	public string ticket;
	
	public ulong user_id;
	
	public string message;
	
	public bool is_read;
	
	public bool is_open;
	
	public bool is_user_message;
	
	public DateTime created_at;
	
	public DateTime updated_at;
}
[Serializable]
public class Meta
{
	
	public int page;
	
	public int per_page;
	
	public int total_items_count;
	
	public int pages_count;
	
	public string order_by;
	
	public string order_direction;
	
	public string search;
	
	public List<string> filters;
}
[Serializable]
public class MessageRequest
{
	
	public List<Datum> data;
	
	public Meta meta;
}
public class SupportMessage //for posting 
{
	
	public string message;
}

#endregion