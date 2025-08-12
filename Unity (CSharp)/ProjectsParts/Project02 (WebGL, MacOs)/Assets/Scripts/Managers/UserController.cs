using it.Managers;
using it.Network.Rest;
using Leguar.TotalJSON;
using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using it.Api;
using DG.Tweening;
using it.Main;

using it.Popups;

public class UserController : Singleton<UserController>
{

	public static it.Network.Rest.User User { get => Instance._user; private set => Instance._user = value; }
	public static ReferenceData ReferenceData { get => Instance._referenceData; set => Instance._referenceData = value; }
	public static List<Language> Languages { get => _languages; set => _languages = value; }
	public TokenRest Token { get => _token; set => _token = value; }
	public SessionSaver Session { get => _session; set => _session = value; }
	public CashierController Cashier { get => _cashier; set => _cashier = value; }
	//public string Stag { get => _stag; set => _stag = value; }
	public static bool IsLogin => Instance != null && Instance._user != null; // Авторизирован
	public RankUser Rank { get => _rank; }
	public WelcomeBonusData WelcomeBonus { get => _welcomeBonus; set => _welcomeBonus = value; }
	private string CurrentTokenPP => "token_current" + (AppConfig.SessinId == null ? "" : $"_session{AppConfig.SessinId}");

	public Garilla.Managers.ChatManager ChatManager = new Garilla.Managers.ChatManager();
	public Garilla.Managers.ActiveTableManager ActiveTableManager;

	public static bool AutoJoin;

	private TokenRest _token;
	private static List<Language> _languages;
	private it.Network.Rest.User _user;
	private SessionSaver _session = new SessionSaver();
	private ReferenceData _referenceData;
	private RankUser _rank;
	private WelcomeBonusData _welcomeBonus;
	private CashierController _cashier = new CashierController();
	//private string _stag;
	private bool _saveSession;

	public static decimal Balance => Instance == null || User == null
	? 0
	: (decimal)User.user_wallet.amount;

	private bool _isLoginning;

	private void Awake()
	{
		ActiveTableManager = new Garilla.Managers.ActiveTableManager(this);
		PlayerPrefs.DeleteKey(StringConstants.SETTINGS_UPDATE);
		PlayerPrefs.DeleteKey(CurrentTokenPP);
		PlayerPrefs.DeleteKey(StringConstants.SESSION_SINGLE_ERROR);
#if UNITY_STANDALONE
		AutoJoin = CommandLineController.ExistsArgumentExe("autoJoin");
#endif

		//if (PlayerPrefs.HasKey("user_stag"))
		//{
		//	_stag = PlayerPrefs.GetString("user_stag");
		//}
		//string str = CommandLineController.GetStagFromExe();
		//if (!string.IsNullOrEmpty(str))
		//{
		//	_stag = str;
		//	PlayerPrefs.SetString("user_stag", _stag);
		//	//AppManager.Instance.VersionLabel.text += " " + _stag;
		//}
	}

	//public void ClearStag()
	//{
	//	_stag = "";
	//	PlayerPrefs.DeleteKey("user_stag");
	//}

	private void Start()
	{
		_session = new SessionSaver();
		_session.LoadUser();
		GetLanguages();

#if !UNITY_EDITOR && UNITY_STANDALONE
		string tokenFromExec = SessionSaver.GetTokenForMultiWindow();
		if (!string.IsNullOrEmpty(tokenFromExec) && CommandLineController.GetIdTableFromExe() != null)
		{
			PlayerPrefs.DeleteKey(CurrentTokenPP);
			NetworkManager.Token = null;
			_isLoginning = true;
			it.Managers.NetworkManager.Token = tokenFromExec;
			_token = new TokenRest();
			_token.access_token = tokenFromExec;
			AfterLogin();
			GetUserData();

			return;
		}
#endif

		com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.ReplenishmentTransactionUpdated, ReplenishmentTransactionUpdated);

		if (!string.IsNullOrWhiteSpace(_session.AuthKey) &&
				DateTime.Now < _session.RefreshDateTime &&
				(DateTime.Now - _session.FirstLoginDateTime).Days <= 30)
		{
			NetworkManager.Token = null;
			_isLoginning = true;
			_saveSession = true;
			RefreshData(_session.AuthKey);
		}
		else
		{
#if UNITY_ANDROID || UNITY_WEBGL || UNITY_IOS
			if (it.Main.PopupController.Instance != null)
				it.Main.PopupController.Instance.ShowPopup(PopupType.Authorization);
#endif
		}

	}

	private void ReplenishmentTransactionUpdated(com.ootii.Messages.IMessage handler)
	{
		it.Network.Socket.ReplenishmentTransactionUpdated pack = (it.Network.Socket.ReplenishmentTransactionUpdated)handler.Data;

		UpdateAmount((decimal)pack.ReplenishmentTransaction.user.user_wallet.amount);
	}
	/// <summary>
	/// Установка нового значения баланса
	/// </summary>
	/// <param name="value">Сумма</param>
	public void UpdateAmount(decimal value)
	{
		User.user_wallet.amount = value;
		com.ootii.Messages.MessageDispatcher.SendMessage(EventsConstants.UserBalanceUpdate,0.01f);
	}

	public void SetAvatar(AvatarObject ava)
	{
		User.user_profile.avatar = ava;
		User.avatar_url = ava.url;
	}

	/// <summary>
	/// Вызов диалога выхода из пользователя
	/// </summary>
	public void LogoutDialog()
	{

#if UNITY_STANDALONE && !UNITY_EDITOR
	if(StandaloneController.Instance.TableWindows.Count > 0){
			var panel = it.Main.PopupController.Instance.ShowPopup<it.Popups.InfoPopup>(PopupType.Info);
			panel.SetDescriptionString("Необходимо закрыть все открытые окна столов");
			return;
	}
#endif

		var popup = it.Main.PopupController.Instance.ShowPopup<it.Popups.LogOutPopup>(PopupType.LogOut);

		popup.OnOk = () =>
		{
			Logout();
		};
	}

	/// <summary>
	/// Подключился другой клиент с тем же аккаунтом
	/// </summary>
	public void AnotherPlayerAuthorization(UnityEngine.Events.UnityAction infoOk = null)
	{
		if (Token == null) return;
		com.ootii.Messages.MessageDispatcher.SendMessage(EventsConstants.SessionSingleError);
		Logout();
		PlayerPrefs.SetString(StringConstants.SESSION_SINGLE_ERROR, "0");

		var infoPanel = it.Main.PopupController.Instance.ShowPopup<InfoPopup>(PopupType.Info);
		infoPanel.SetDescriptionString("errors.forms.sessionSingleErrors".Localized());
		infoPanel.OnConfirm = () =>
		{
			infoOk?.Invoke();
			com.ootii.Messages.MessageDispatcher.SendMessage(EventsConstants.SessionSingleAnotherConfirm);
		};
	}

	/// <summary>
	/// Выход из учетной записи
	/// </summary>
	public void Logout()
	{

		it.Api.UserApi.Logout((result) =>
		{

			if (!result) return;

#if UNITY_EDITOR
			//it.Logger.Log("LOGOUT RESPONSE " + result);
#endif
			//NetworkManager.Token = null;
			//_user = null;
			//_token = null;
			//_saveSession = false;

			_cashier.KillRequestsProcess();
			SocketClient.Instance.Disconnect();

		});
		ClearTokenData();

	}

	public void ClearTokenData()
	{

		_session.ClearUser();
		PlayerPrefs.DeleteKey(CurrentTokenPP);
		NetworkManager.Token = null;
		_user = null;
		CashierVisibleconfirm(false);
		_token = null;
		_saveSession = false;

		com.ootii.Messages.MessageDispatcher.SendMessage(EventsConstants.UserLogin);

#if UNITY_ANDROID || UNITY_WEBGL || UNITY_IOS
		it.Main.PopupController.Instance.ShowPopup(PopupType.Authorization);
#endif
	}

	private void CashierVisibleconfirm(bool value = false)
	{
		AppConfig.ActiveCashier = value;
		com.ootii.Messages.MessageDispatcher.SendMessage(EventsConstants.CashierVisibleChange);
	}

	/// <summary>
	/// Авторизация
	/// </summary>
	/// <param name="email">Электронная почта</param>
	/// <param name="password">Пароль</param>
	/// <param name="isRemember">Запоснить учетную запись</param>
	/// <param name="onComplete">Событие успешного выполнения</param>
	/// <param name="onError">ошибка выполнения</param>
	public void Login(string email, string password, bool isRemember, UnityEngine.Events.UnityAction onComplete, UnityEngine.Events.UnityAction<string> onError)
	{
		PlayerPrefs.DeleteKey(CurrentTokenPP);
		NetworkManager.Token = null;
		_isLoginning = true;

		UserApi.Login(email, password, (result) =>
		{
			//ClearStag();
			if (!result.IsSuccess)
			{
				onError?.Invoke(result.ErrorMessage);
				return;
			}
			_user = result.Result.User;
			CashierVisibleconfirm(_user.cashier_available);
			_token = result.Result.Token;

			NetworkManager.Token = _token.access_token;
			PlayerPrefs.SetString(CurrentTokenPP, _token.access_token);

			if (isRemember)
			{
				_saveSession = true;
				_session.SaveAuth(_token, true);
			}
			else
				_session.ClearUser();
			NetworkManager.ClearQueue();

			//_stag = "";

			_session.SetActiveToken(_token.access_token);

			//it.Managers.WSIOClient.Instance.Init();
			//WSManager.Instance.Connect();
			SocketClient.Instance.Init();

			onComplete?.Invoke();

			AfterLogin();
			GetUserData();
			GetInitData();
			_cashier.GetPaymentRequest();

		});


		//List<KeyValuePair<string, object>> paramsList = new List<KeyValuePair<string, object>>();

		//paramsList.Add(new KeyValuePair<string, object>("email", email));
		//paramsList.Add(new KeyValuePair<string, object>("password", password));

		//string url = "/auth/login";

		//it.Managers.NetworkManager.Request(url, paramsList, (result) =>
		//{
		//	ClearStag();
		//	_user = (it.Network.Rest.User)it.Helpers.ParserHelper.Parse(typeof(it.Network.Rest.User), JSON.ParseString(result).GetJSON("user"));
		//	_token = (TokenRest)it.Helpers.ParserHelper.Parse(typeof(TokenRest), JSON.ParseString(result));

		//	NetworkManager.Token = _token.AccessToken;
		//	PlayerPrefs.SetString(CurrentTokenPP, _token.AccessToken);

		//	if (isRemember)
		//	{
		//		_saveSession = true;
		//		_session.SaveAuth(_token, true);
		//	}
		//	else
		//		_session.ClearUser();
		//	NetworkManager.ClearQueue();

		//	_stag = "";

		//	_session.SetActiveToken(_token.AccessToken);

		//	//it.Managers.WSIOClient.Instance.Init();
		//	//WSManager.Instance.Connect();
		//	WebsocketClient.Instance.Init();

		//	onComplete?.Invoke();

		//	AfterLogin();
		//	GetUserData();
		//	GetInitData();
		//	GetPaymentRequest();

		//},
		//(error) =>
		//{
		// it.Logger.LogError("Login error " + error + " | Request: " + "/auth/login");
		// onError?.Invoke(error);
		// //OutputError();
		// return;
		//});
	}

	/// <summary>
	/// Подтвержение учетной записи кодом
	/// </summary>
	/// <param name="email">Электронная почта</param>
	/// <param name="code">Код</param>
	/// <param name="onComplete">Успешное выполнения</param>
	/// <param name="onError">Ошибка выполнения</param>
	public void ConfirmCode(string email, string code, UnityEngine.Events.UnityAction onComplete, UnityEngine.Events.UnityAction<string> onError)
	{

		_isLoginning = true;

		UserApi.ConfirmCode(email, code, (result) =>
		{
			if (!result.IsSuccess)
			{
				onError?.Invoke(result.ErrorMessage);
				return;
			}
			_token = result.Result.Token;
			NetworkManager.Token = _token.access_token;

			_session.SetActiveToken(_token.access_token);
			PlayerPrefs.SetString(CurrentTokenPP, _token.access_token);

			UpdateUser(result.Result.User);

			onComplete?.Invoke();
			_cashier.GetPaymentRequest();

		});


		//List<KeyValuePair<string, object>> paramsList = new List<KeyValuePair<string, object>>();

		//paramsList.Add(new KeyValuePair<string, object>("email", email));
		//paramsList.Add(new KeyValuePair<string, object>("code", code));

		//it.Managers.NetworkManager.Request(url, paramsList, (result) =>
		//{
		//	it.Logger.Log("REGISTR RESPONSE " + result);

		//	var user = (it.Network.Rest.User)it.Helpers.ParserHelper.Parse(typeof(it.Network.Rest.User), JSON.ParseString(result).GetJSON("user"));
		//	_token = (TokenRest)it.Helpers.ParserHelper.Parse(typeof(TokenRest), JSON.ParseString(result));
		//	NetworkManager.Token = _token.AccessToken;

		//	_session.SetActiveToken(_token.AccessToken);
		//	PlayerPrefs.SetString(CurrentTokenPP, _token.AccessToken);

		//	UpdateUser(user);

		//	onComplete?.Invoke();
		//	GetPaymentRequest();

		//},
		//(error) =>
		//{
		// it.Logger.LogError("Login error " + error + " | Request: " + "/auth/login");
		// onError?.Invoke(error);
		// //OutputError();
		// return;
		//});

	}

	/// <summary>
	/// Сброс пароля
	/// </summary>
	/// <param name="email">Пароль</param>
	/// <param name="code">Код</param>
	/// <param name="onComplete">Событие успешного выполнения</param>
	/// <param name="onError">ошибка выполнения</param>
	public void ResetPassword(string email, string code, UnityEngine.Events.UnityAction onComplete, UnityEngine.Events.UnityAction<string> onError)
	{

		UserApi.ResetPassword(email, code, (result) =>
		{
			if (!result.IsSuccess)
			{
				onError?.Invoke(result.ErrorMessage);
				return;
			}

			onComplete?.Invoke();

		});

		//string url = "/auth/check_password_reset_code";

		//List<KeyValuePair<string, object>> paramsList = new List<KeyValuePair<string, object>>();

		//paramsList.Add(new KeyValuePair<string, object>("email", email));
		//paramsList.Add(new KeyValuePair<string, object>("token", code));


		//it.Managers.NetworkManager.Request(url, paramsList, (result) =>
		//{


		//	/*	_user = (it.Network.Rest.User)it.Helpers.ParserHelper.Parse(typeof(it.Network.Rest.User), JSON.ParseString(result).GetJSON("user"));
		//		_token = (TokenRest)it.Helpers.ParserHelper.Parse(typeof(TokenRest), JSON.ParseString(result));

		//		NetworkManager.Token = _token.AccessToken;*/
		//	//GetUserData();

		//	//it.Managers.WSIOClient.Instance.Init();
		//	//WSManager.Instance.Connect();
		//	//WebsocketClient.Instance.Init();

		//	/*GetUserData();*/

		//	onComplete.Invoke();

		//},
		//(error) =>
		//{
		// it.Logger.LogError("Login error " + error + " | Request: " + "/auth/login");
		// onError?.Invoke(error);
		// //OutputError();
		// return;
		//});

	}

	/// <summary>
	/// Запрос повторного отправки кода
	/// </summary>
	/// <param name="email">Электронная почта</param>
	/// <param name="onComplete">Событие успешного выполнения</param>
	/// <param name="onError">событие ошибки выполнения</param>
	public void ResendConfirmationCode(string email, UnityEngine.Events.UnityAction onComplete, UnityEngine.Events.UnityAction<string> onError)
	{
		it.Api.UserApi.RequestConfirmationCode(email, (result) =>
		{
			if (!result.IsSuccess)
			{
				onError?.Invoke(result.ErrorMessage);
				return;
			}
			onComplete.Invoke();
		});
	}
	public static void GetLanguages()
	{
		it.Api.UserApi.GetLanguages((result) =>
		{
			if (result.IsSuccess)
			{
				_languages = result.Result;
				return;
			}
		});
	}

	/// <summary>
	/// Установка нового номера телефона
	/// </summary>
	/// <param name="phone">Номер телефона</param>
	public void SetPhone(string phone)
	{
		_user.phone = phone;
		com.ootii.Messages.MessageDispatcher.SendMessage(EventsConstants.UserProfileUpdate);
	}

	/// <summary>
	/// Обновление данных пользователя
	/// </summary>
	/// <param name="user">Обьект пользователя</param>
	public void UpdateUser(User user)
	{
		_user = user;
		CashierVisibleconfirm(_user.cashier_available);

		//StandaloneController.SetNewWindowName($"Garilla Poker - " + _user.nickname);

		GetUserProfile();
		if (SocketClient.Instance)
			SocketClient.Instance.Init();
		GetInitData();
	}

	public void GetUserData()
	{
		UserApi.GetUserData((result) =>
		{
			UpdateUser(result.user);
		},
		(error) =>
		{
			it.Logger.Log("GetUserData error " + error);
		});
	}

	private void GetInitData()
	{
		GetReferences();
		_cashier.GetCashierMethods();

		it.Api.UserApi.GetReplenishmentMethod();
		//GetUserProfile();
		GetMyRank();
		GetWelcomeBonus();
	}

	public void UpdateTaimbank()
	{
		UserApi.GetUserBank((newbank) =>
		{
			if (newbank.IsSuccess)
			{
				User.user_profile.time_bank = newbank.Result.time_bank;
				User.user_profile.time_bank_paid = newbank.Result.time_bank_paid;
				com.ootii.Messages.MessageDispatcher.SendMessage(EventsConstants.UserTimebankUpdate);
			}
		});
	}

	public void SetWelcomeBonusList(WelcomeBonusData wbd)
	{
		_welcomeBonus = wbd;
		com.ootii.Messages.MessageDispatcher.SendMessage(EventsConstants.WelcomeBonusUpdate);
	}


	private void GetWelcomeBonus()
	{

		UserApi.WelcomeBonus((result) =>
		{
			SetWelcomeBonusList(result);

			DOVirtual.DelayedCall(120, GetWelcomeBonus);


		}, (error) =>
		{

		});
	}

	public void SetUserRank(RankUser rk)
	{
		_rank = rk;
		com.ootii.Messages.MessageDispatcher.SendMessage(EventsConstants.UserRankUpdate, 0.3f);
	}


	private void GetMyRank()
	{

		UserApi.GetMyRank((result) =>
		{
			SetUserRank(result);
		}, (error) =>
		{

		});
	}
#if UNITY_STANDALONE
	private void Update()
	{
		if (NetworkManager.Token != PlayerPrefs.GetString(CurrentTokenPP, "") && !string.IsNullOrEmpty(PlayerPrefs.GetString(CurrentTokenPP, "")))
		{
			NetworkManager.Token = PlayerPrefs.GetString(CurrentTokenPP, "");
			if (NetworkManager.ReadyQueue)
				NetworkManager.UpdateToken();
			it.Logger.Log("[WS] tokenUpdate from prefs");
			com.ootii.Messages.MessageDispatcher.SendMessage(EventsConstants.NetworkTokenUpdate, 0.01f);
		}
	}
#endif

	public void RefreshData(string token)
	{
		it.Managers.NetworkManager.Token = token;

		UserApi.Refresh((result) =>
		{
			if (result.IsSuccess)
			{
				//ClearStag();
				_token = result.Result;

				NetworkManager.Token = _token.access_token;
				PlayerPrefs.SetString(CurrentTokenPP, _token.access_token);

				_session.SetActiveToken(_token.access_token);
				if (NetworkManager.ReadyQueue)
					NetworkManager.UpdateToken();

				if (_saveSession)
					_session.SaveAuth(_token, false);

				AfterLogin();

				GetUserData();
				_cashier.GetPaymentRequest();
			}
			else
			{
				_session.ClearUser();
			}

		});
	}

	private void AfterLogin()
	{
		ChatManager.LoadBlockList();
		ActiveTableManager.LoadActiveTable();
	}

	public void SessionLoss()
	{
		string activeToken = SessionSaver.GetActiveToken();

		if (activeToken != "-1" && activeToken != _token.access_token)
		{
			_token.access_token = activeToken;
			NetworkManager.Token = activeToken;
			NetworkManager.UpdateToken();
		}
		else
		{
			DOVirtual.DelayedCall(0.1f, () =>
			{
				RefreshData(_token.access_token);
			});
		}

	}

	public void GetReferences()
	{

		UserApi.GetReferenceData((result) =>
		{
			if (result.IsSuccess)
			{
				//ClearStag();
				_referenceData = result.Result.data;
				com.ootii.Messages.MessageDispatcher.SendMessage(EventsConstants.UserReferenceUpdate);
			}
		});
	}

	public void UpdateProfile(UserProfilePost profilePost, Action<UserProfile> callbackSuccess = null,
			Action<string> callbackFailed = null)
	{
		it.Api.UserApi.PostUserProfile(profilePost, (result) =>
		{
			if (result.IsSuccess)
			{
#if UNITY_STANDALONE
				PlayerPrefs.SetString(StringConstants.SETTINGS_UPDATE, System.DateTime.Now.ToString());
#endif

				ConformProfile(result.Result.data);
				callbackSuccess?.Invoke(result.Result.data);
			}
			else
				callbackFailed?.Invoke(result.ErrorMessage);
		});
	}

	private void ConformProfile(UserProfile profile)
	{

		User.user_profile = profile;

		CardLibrary.SetPackName((CardLibrary.CardsFrontType)(int.Parse(User.user_profile.table_theme.front_deck) - 1),
		(CardLibrary.CardsBackType)(int.Parse(User.user_profile.table_theme.back_deck) - 1));

		it.Settings.GameSettings.GameTheme.SetStyle(User.user_profile.table_theme.felt, User.user_profile.table_theme.background);

		com.ootii.Messages.MessageDispatcher.SendMessage(EventsConstants.UserProfileUpdate);
	}


	public void GetUserProfile(Action<UserProfile> callbackSuccess = null,
			Action<string> callbackFailed = null)
	{
		it.Api.UserApi.GetUsetProfile((result) =>
		{
			if (result.IsSuccess)
			{
				ConformProfile(result.Result.data);
				callbackSuccess?.Invoke(result.Result.data);
				if (_isLoginning)
				{
					com.ootii.Messages.MessageDispatcher.SendMessage(EventsConstants.UserLogin);
					_isLoginning = false;
				}
			}
			else
				callbackFailed?.Invoke(result.ErrorMessage);

		});
	}

	public static void NicknameChange(string nickname, UnityEngine.Events.UnityAction<bool, string> OnComplete)
	{
		UserApi.NicknameChange(nickname, (result) =>
		{
			if (result.IsSuccess)
				Instance.UpdateUser(result.Result);
			OnComplete?.Invoke(result.IsSuccess, result.ErrorMessage);
		});
	}

}


public class SessionSaver
{

	private static void PostPrefsKey(ref string key)
	{
		var session = AppConfig.SessinId;
#if UNITY_EDITOR
		key = "editor_" + key;
#endif
		if (_session != null)
			key += $"_session{session}";
	}

	private static string ActiveToken
	{
		get
		{
			string Key = "ACTIVE_TOKEN";
			PostPrefsKey(ref Key);
			return Key;
		}
	}
	private static string FirstLoginKeyPrefs
	{
		get
		{
			string Key = "FIRST_LOGIN";
			PostPrefsKey(ref Key);
			return Key;
		}
	}
	private static string AuthKeyPrefs
	{
		get
		{
			string Key = "AUTH_KEY";
			PostPrefsKey(ref Key);
			return Key;
		}
	}
	private static string ExpressKeyPrefs
	{
		get
		{
			string Key = "EXPIRES_KEY";
			PostPrefsKey(ref Key);
			return Key;
		}
	}
	private static string RefreshKeyPrefs
	{
		get
		{
			string Key = "REFRESH_KEY";
			PostPrefsKey(ref Key);
			return Key;
		}
	}
	private static string MultiWindowKeyPrefs
	{
		get
		{
			string Key = "AUTH_KEY_FOR_MULTI_WINDOW";
			PostPrefsKey(ref Key);
			return Key;
		}
	}

	public DateTime FirstLoginDateTime;
	public string AuthKey;
	public DateTime ExpiresDateTime;
	public DateTime RefreshDateTime;

	static int? _session;

	public SessionSaver()
	{
		_session = AppConfig.SessinId;
	}

	public void SaveAuth(TokenRest info, bool firstLogin)
	{
		if (firstLogin)
			PlayerPrefs.SetString(FirstLoginKeyPrefs, DateTime.Now.ToString(CultureInfo.CurrentCulture));
		PlayerPrefs.SetString(AuthKeyPrefs, info.access_token);
		PlayerPrefs.SetString(ExpressKeyPrefs, DateTime.Now.AddSeconds(info.expires_in).ToString(CultureInfo.CurrentCulture));
		PlayerPrefs.SetString(RefreshKeyPrefs, DateTime.Now.AddSeconds(info.refresh_in).ToString(CultureInfo.CurrentCulture));
	}

	public void LoadUser()
	{
		AuthKey = PlayerPrefs.GetString(AuthKeyPrefs, string.Empty);

		var date = PlayerPrefs.GetString(FirstLoginKeyPrefs, string.Empty);
		if (!string.IsNullOrWhiteSpace(date))
			FirstLoginDateTime = DateTime.Parse(date);
		date = PlayerPrefs.GetString(ExpressKeyPrefs, string.Empty);
		if (!string.IsNullOrWhiteSpace(date))
			ExpiresDateTime = DateTime.Parse(date);
		date = PlayerPrefs.GetString(RefreshKeyPrefs, string.Empty);
		if (!string.IsNullOrWhiteSpace(date))
			RefreshDateTime = DateTime.Parse(date);
	}

	public void ClearUser()
	{
		PlayerPrefs.SetString(FirstLoginKeyPrefs, null);
		PlayerPrefs.SetString(AuthKeyPrefs, null);
		PlayerPrefs.SetString(ExpressKeyPrefs, null);
		PlayerPrefs.SetString(RefreshKeyPrefs, null);
	}

	public void Logout()
	{
		ClearUser();
		GameHelper.LogOut();
	}

	public static string GetTokenForMultiWindow()
	{
		string tokenValue = PlayerPrefs.GetString(MultiWindowKeyPrefs, "");
		it.Logger.Log("Token table = " + tokenValue);
		//PlayerPrefs.SetString("AUTH_KEY_FOR_MULTI_WINDOW", "");
		return tokenValue;
	}

	public static void SaveTokenForMultiWindow()
	{
		PlayerPrefs.SetString(MultiWindowKeyPrefs, NetworkManager.Token);
	}

	public void SetActiveToken(string activeToken)
	{
		PlayerPrefs.SetString(ActiveToken, activeToken);
	}

	public static string GetActiveToken()
	{
		return PlayerPrefs.GetString(ActiveToken, "-1");
	}

}
