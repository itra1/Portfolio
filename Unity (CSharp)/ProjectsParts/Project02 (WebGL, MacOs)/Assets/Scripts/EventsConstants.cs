public class EventsConstants
{
  public const string SocketOpen = "Socket.Open";
  public const string WelcomeBonusUpdate = "WelcomeBonus.Update";
  public const string SocketClose = "Socket.Close";
  public const string SocketError = "Socket.Error";
  public const string UsetLoginError = "User.LoginError";
  public const string PageSelect = "Page.select";
  public const string GameSheetSelect = "Game.Sheet.Select";
  public const string UserLogin = "User.Login";
	public const string UserUnauthorized = "User.Unauthorized";
	public const string TableLoad = "Table.Load";
  public const string CurrencyChange = "Currency.Change";
  public const string LocalizationChange = "Localization.Change";
  public const string UserRequestUpdate = "User.RequestUpdate";
	public const string UserProfileUpdate = "User.ProfileUpdate";
  public const string UserBalanceUpdate = "User.BalanceUpdate";
  public const string UserReferenceUpdate = "User.ReferenceUpdate";
  public const string UserCreateTableOptionsUpdate = "User.CreateTableOptionsUpdate";
  public const string UserRankUpdate = "User.Rank.Update";
	public const string UserTimebankUpdate = "User.Timebank.Update";
	public const string CashierVisibleChange = "Cashier.VisibleChange";
	public const string ReplenishmentTransactionUpdated = "ReplenishmentTransactionUpdated";
  public const string WSDisconnect = "WSDisconnect";
  public const string WSConnect = "WSConnect";
  public const string FrontDeckChange = "FrontDeck.Change";
  public const string BackDeckChange = "BackDeck.Change";
  public const string MainPageOpen = "MainPage.Open";
  public const string SettingsUpdate = "Settings.Update";
  public const string WindowsTableListChange = "Windows.TableListChange";
  //public const string CardsMoveUpGameCombitation = "Cards.MoveUpGameCombitation";
	public const string MyAfkChange = "MyAfk.Change";
	public const string MyAfkActive = "MyAfk.Active";
	public const string AppFullLoad = "App.FullLoad";
	public const string SessionSingleError = "Session.SingleError";
	public const string SessionSingleAnotherConfirm = "Session.AnotherConfirm";
	public const string ActiveTableListChange = "ActiveTable.ListChange";
	public const string SinglePageChnage = "SinglePage.Chnage";
	public const string ServersLoaded = "Servers.Loaded";
  public const string AppExistsUpdate = "App.ExistsUpdate";
  public const string NetworkTokenUpdate = "Network.TokenUpdate";

	public static string GameTableShowCards(string chanel) { return $"Game.Table_{chanel}.ShowCards"; }
	public static string GameTableEvent(string chanel){ return $"Game.Table_{chanel}.Event"; }
	public static string JackpotWin(string chanel) { return $"Game.Table_{chanel}.JackpotWin"; }
	public static string GameTableCountdown(string chanel) { return $"Game.Table_{chanel}.Countdown"; }
  public static string GameTableReplenishmentTransaction(string chanel) { return $"Game.Table_{chanel}.ReplenishmentTransaction"; }
  public static string GameTableUpdate(string chanel) { return $"Game.Table_{chanel}.Update"; }
	public static string DealerChoise(string chanel) { return $"Game.Table_{chanel}.DealerChoise"; }
	public static string GameTableReserve(string chanel) { return $"Game.Table_{chanel}.Reserve"; }
  public static string ChatMessage(string chanel) { return $"Game.Chat_{chanel}.Message"; }
	public static string DropSmile(string chanel) { return $"Game.Smile_{chanel}.Drop"; }
	public static string SupportChatMessageCreates(string chanel) { return $"Game.Chat_{chanel}.SupportMessage"; }
  public static string ObsorveListUpdate(string chanel) { return $"Obsorve.Lis_{chanel}.Update"; }
}
