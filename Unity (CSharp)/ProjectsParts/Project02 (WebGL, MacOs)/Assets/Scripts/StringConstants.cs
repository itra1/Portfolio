using System;
using System.Collections;

using UnityEngine;


public class StringConstants
{
	//public const string BUTTON_GRID_TABLEORDER = "BUTTON.GRID.TABLEORDER";
	//public const string BUTTON_QUEUE_TABLEORDER = "BUTTON.QUEUE.TABLEORDER";
	//public const string BUTTON_SETTINGS = "BUTTON.SETTINGS";
	//public const string BUTTON_TABLE_THEME = "BUTTON.TABLE.THEME";
	//public const string BUTTON_LEADERBOARD_MICRO = "BUTTON.LEADERBOARD.MICRO";
	//public const string BUTTON_LEADERBOARD_AVERAGE = "BUTTON.LEADERBOARD.AVERAGE";
	//public const string BUTTON_LEADERBOARD_HIGH = "BUTTON.LEADERBOARD.HIGH";
	//public const string BUTTON_JACKPOT = "BUTTON.JACKPOT";
	//public const string WINDOW_NEW = "WINDOW.NEW";

	public const string RESOURCES_TEXTURES = "Textures";

	public const string SOUND_BUTTON_CLICK = "Click";
	public const string SOUND_BUTTON_CLICKChange = "CLICKChange";
	public const string SOUND_CHAT = "Chat";
	public const string SOUND_CHAT_RECIEVE = "recieve";
	public const string SOUND_CHAT_SEND = "send";
	public const string SOUND_GAME_TIMER = "GameTimer";

	public const string SOUND_GAME_ALERT = "GameAlert";
	public const string SOUND_GAME_CARDFOLD = "GameCardFold";
	public const string SOUND_GAME_CARDOPEN = "GameCardOpen";
	public const string SOUND_GAME_BET = "GameBet";
	public const string SOUND_GAME_WIN = "GameWin";
	public const string SOUND_GAME_DEAL = "GameDeal";
	public const string SOUND_GAME_TRANSACTIONS = "SoundTransactions";

	public const string SETTINGS_UPDATE = "Settings.Update";
	public const string CHAT_BLOCKLOAD = "Chat.BlockLoad";
	public const string SESSION_SINGLE_ERROR = "Session.SingleError";
	public const string APP_UPDATE = "App.Update";

	public static string NoteUpdate(ulong userId)
	{
		if (UserController.Instance == null)
			return "";
		if (UserController.User == null)
			return "";

		return $"{UserController.User.id}_{userId}";
	}
	public static string AfkTimerName(ulong userId)
	{
		return $"playerAfk_{userId}";
	}
	public static string AfkNoTimerName(ulong userId)
	{
		return $"playerNoAfk_{userId}";
	}
	public static string TablePassword(ulong tableId)
	{
		return $"tablePass_{tableId}";
	}

	public static string CloseNow => "CloseNow" + AppConfig.SessinId ?? "";
	public static string BUTTON_QUEUE_TABLEORDER => "BUTTON.QUEUE.TABLEORDER" + (AppConfig.SessinId >= 0 ? (".SESSION" + AppConfig.SessinId) : "");
	public static string BUTTON_GRID_TABLEORDER => "BUTTON.GRID.TABLEORDER" + (AppConfig.SessinId >= 0 ? (".SESSION" + AppConfig.SessinId) : "");
	public static string BUTTON_SETTINGS => "BUTTON.SETTINGS" + (AppConfig.SessinId >= 0 ? (".SESSION" + AppConfig.SessinId) : "");
	public static string BUTTON_TABLE_THEME => "BUTTON.TABLE.THEME" + (AppConfig.SessinId >= 0 ? (".SESSION" + AppConfig.SessinId) : "");
	public static string BUTTON_LEADERBOARD_MICRO => "BUTTON.LEADERBOARD.MICRO" + (AppConfig.SessinId >= 0 ? (".SESSION" + AppConfig.SessinId) : "");
	public static string BUTTON_LEADERBOARD_AVERAGE => "BUTTON.LEADERBOARD.AVERAGE" + (AppConfig.SessinId >= 0 ? (".SESSION" + AppConfig.SessinId) : "");
	public static string BUTTON_LEADERBOARD_HIGH => "BUTTON.LEADERBOARD.HIGH" + (AppConfig.SessinId >= 0 ? (".SESSION" + AppConfig.SessinId) : "");
	public static string BUTTON_BADBEAT => "BUTTON.BADBEAT" + (AppConfig.SessinId >= 0 ? (".SESSION" + AppConfig.SessinId) : "");
	public static string BUTTON_JACKPOT => "BUTTON.JACKPOT" + (AppConfig.SessinId >= 0 ? (".SESSION" + AppConfig.SessinId) : "");
	public static string WINDOW_NEW => "WINDOW.NEW" + (AppConfig.SessinId >= 0 ? (".SESSION" + AppConfig.SessinId) : "");
	public static string TIMEBANK_UPDATE => "TIMEBANK.UPDATE" + (AppConfig.SessinId >= 0 ? (".SESSION" + AppConfig.SessinId) : "");

	#region Валюты

	public static string CURRENCY_EURO => "€";
	public static string CURRENCY_EURO_STR => "euro";
	public static string CURRENCY_EURO_STR_SHORT => "eur";
	public static string CURRENCY_IOS => "🟊";// 🟊◆
	public static string CURRENCY_IOS_STR => "star";
	public static string CURRENCY_IOS_STR_SHORT => CURRENCY_IOS_STR;
	public static string CURRENCY_SYMBOL
	{
		get
		{
#if UNITY_IOS
			return CURRENCY_IOS;
#else
			return CURRENCY_EURO;
#endif
		}
	}
	public static string CURRENCY_SYMBOL_GOLD => $"<color=#E9B069>{CURRENCY_SYMBOL}</color>";

	#endregion

}