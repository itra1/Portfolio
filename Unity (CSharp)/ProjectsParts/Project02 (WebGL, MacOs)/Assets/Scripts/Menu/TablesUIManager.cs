using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using it.Popups;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using it.Network.Rest;

public class TablesUIManager : Singleton<TablesUIManager>
{
	//public static TablesUIManager Instance;

	[Header("Windows")][SerializeField] private GameObject SplashScreen;
	[SerializeField] private NetworkStatusPopup NetworkStatusWindow;
	[SerializeField] private ProfileMenuUI profileWindow;
	[SerializeField] private SettingsUI SettingsWindow;
	[SerializeField] private WelcomePopup WelcomeWindow;
	[SerializeField] private PasswordPopup passwordPopup;
	[SerializeField] private PaymentPopup paymentPopup;
	[SerializeField] private SupportPopup supportPopup;
	[SerializeField] private SmartHudPopup infoPlayerPopup;
	//[SerializeField] private UIProfile uiProfile;

	[Space][SerializeField] private GameObject tablePage;
	[SerializeField] private GameObject homePage;

	[SerializeField] private MenuPlayerUI menuPlayerUIPrefab;
	[SerializeField] private List<Transform> playerPlaces;
	[SerializeField] private TextMeshProUGUI buyInRange;
	[SerializeField] private TextMeshProUGUI buyInRangeTitle;
	[SerializeField] private TextMeshProUGUI buyLanguageTxt;

	[Space][SerializeField] private GameObject btnOpenTable;

	[SerializeField] private GameObject btnWait;
	[Space][SerializeField] private Transform TableParent;
	[SerializeField] private TableUIPlaceManager TableUI2MaxPlayers;
	[SerializeField] private TableUIPlaceManager TableUI4MaxPlayers;
	[SerializeField] private TableUIPlaceManager TableUI6MaxPlayers;
	[SerializeField] private TableUIPlaceManager TableUI9MaxPlayers;

	[Space][SerializeField] private bool AlwaysWelcome;

	private GameObject playerSlot;
	private TableUIPlaceManager placeUIManager;
	private List<MenuPlayerUI> tablePlayers = new List<MenuPlayerUI>();
	private Table selectTable = null;
	private SessionSaver sessionSaver => AppManager.Instance.Session;
	private bool allLoad;

	public static List<string> tablesOpen = new List<string>();

	public void Init()
	{
		//Instance = this;
		//ameController.GetUserProfile(SetProfileData);
		//GameController.GetReferenceData(SetReferenceInfo);
#if !UNITY_EDITOR
        AlwaysWelcome = false;
#endif
		//sessionSaver = new SessionSaver();

		//CommandLineController.CheckArgumentsAndSetResolution();

		ulong? idTableFromExe = AppConfig.TableExe;
		//if (SplashScreen != null) SplashScreen.SetActive(idTableFromExe != -1);

		PlayerPrefs.SetInt(StringConstants.CloseNow, 0);
		if (idTableFromExe != null)
		{
			GetTableInfo((ulong)idTableFromExe);
		}
		else
		{
#if UNITY_STANDALONE && !UNITY_EDITOR
            Application.wantsToQuit += StandaloneController.CloseAllTable;
#endif
			//StandaloneController.SetNewWindowName($"Garilla Poker - {UserController.User.nickname}");
		}

		buyInRange.gameObject.SetActive(false);
		buyInRangeTitle.gameObject.SetActive(false);

		//tableListBehaviour.SelectTableCallback = InitTable;
		//WebsocketClient.Instance.ChangeTableCallback = ChangeTable;
		//WebsocketClient.Instance.PaymentCallback = PaymentCallback;
		SelectGame(new List<GameType>() { GameType.None });
	}

	void SetProfileData(UserProfile profile)
	{
#if UNITY_ANDROID || UNITY_WEBGL || UNITY_IOS
		it.Logger.Log("apply this");
#endif
#if !UNITY_ANDROID && !UNITY_WEBGL && !UNITY_IOS
		SoundManager.instance.Init();
		SoundManager.instance.PlaySoundIntro();
		CardLibrary.SetPackName((CardLibrary.CardsFrontType)(int.Parse(profile.table_theme.front_deck) - 1),
				(CardLibrary.CardsBackType)(int.Parse(profile.table_theme.back_deck) - 1));

		if (allLoad) AllIsLoad();

		allLoad = true;
		if (buyLanguageTxt) buyLanguageTxt.text = profile.language.name;
#endif
	}

	public static void SetReferenceInfo(ReferenceData reference)
	{
		if (Instance.allLoad) Instance.AllIsLoad();

		Instance.allLoad = true;
	}

	void AllIsLoad()
	{
		//uiProfile.Init();
		if (PlayerPrefs.GetInt("Welcome" + UserController.User.id, 0) == 0 || AlwaysWelcome) Welcome();
	}

#if UNITY_EDITOR
	private void Update()
	{
		//???????????? ??? ????? (??. Table.maxPlayers)
		if (Input.GetKeyUp(KeyCode.Keypad2))
			_mx = 2;
		if (Input.GetKeyUp(KeyCode.Keypad3))
			_mx = 3;
		if (Input.GetKeyUp(KeyCode.Keypad4))
			_mx = 4;
		if (Input.GetKeyUp(KeyCode.Keypad5))
			_mx = 5;
		if (Input.GetKeyUp(KeyCode.Keypad6))
			_mx = 6;
		if (Input.GetKeyUp(KeyCode.Keypad7))
			_mx = 7;
		if (Input.GetKeyUp(KeyCode.Keypad8))
			_mx = 8;
		if (Input.GetKeyUp(KeyCode.Keypad9))
			_mx = 9;
	}

	int _mx
	{
		set
		{
			Table.maxPlayersStat = value;
			it.Logger.Log($"MaxPlayers = {Table.maxPlayersStat}");
		}
	}
#endif

	private void PaymentCallback(ReplenishmentTransaction obj)
	{
		if (obj.user_id != UserController.User.id) return;

		GameHelper.SaveUserInfo(obj.user);
		//if (uiProfile != null) uiProfile.Init();
	}

	private void ChangeTable(SocketEventTable socketEventTable, string chanel)
	{
		if (socketEventTable.table.id != selectTable.id) return;
		UpdateTable(socketEventTable.table);
	}

	private void InitTable(Table selectTable)
	{
		SocketClient.Instance.EnterTableChanel(selectTable.id);

		this.selectTable = selectTable;

		foreach (var item in tablePlayers)
		{
			Destroy(item.gameObject);
		}

		if (placeUIManager)
		{
			Destroy(placeUIManager.gameObject);
		}

		tablePlayers = new List<MenuPlayerUI>();

		switch (selectTable.MaxPlayers)
		{
			case 2:
				placeUIManager = Instantiate(TableUI2MaxPlayers, TableParent);
				break;
			case 3:
			case 4:
				placeUIManager = Instantiate(TableUI4MaxPlayers, TableParent);
				break;
			case 5:
			case 6:
				placeUIManager = Instantiate(TableUI6MaxPlayers, TableParent);
				break;
			default:
				placeUIManager = Instantiate(TableUI9MaxPlayers, TableParent);
				break;
		}

		placeUIManager.transform.SetSiblingIndex(1);
		playerPlaces = placeUIManager.PlayerPlaces;
		playerSlot = placeUIManager.PlayerSlot;

		if (selectTable.table_player_sessions != null)
		{
			for (var i = 0; i < selectTable?.table_player_sessions.Length; i++)
			{
				if (i >= playerPlaces.Count)
				{
					it.Logger.Log("?????????? ??????? ????????? ?????????? ???? ?? ??????");
					break;
				}

				var playerPanel = Instantiate(menuPlayerUIPrefab, playerPlaces[i]);
				playerPanel.Init(selectTable.table_player_sessions[i]);
				tablePlayers.Add(playerPanel);
			}
		}

		buyInRange.text = $"${selectTable.BuyInMinEURO} - {selectTable.BuyInMaxEURO}";

		playerSlot.SetActive(!selectTable.isAtTheTable);
		btnOpenTable.gameObject.SetActive(true);
		//NameTable.gameObject.SetActive(true);
		buyInRange.gameObject.SetActive(true);
		buyInRangeTitle.gameObject.SetActive(true);
	}
	//moving panel next to choosen item list
#if UNITY_ANDROID || UNITY_WEBGL || UNITY_IOS
	public void TableSetParent(RectTransform listItemTransform)
	{
		if (TableParent.gameObject.activeSelf) TableDisable();
		TableParent.gameObject.SetActive(true);
		TableParent.transform.SetParent(listItemTransform);
		listItemTransform.sizeDelta = new Vector2(listItemTransform.sizeDelta.x,
				listItemTransform.sizeDelta.y + TableParent.GetComponent<RectTransform>().sizeDelta.y);
	}

	public void TableDisable()
	{
		if (TableParent.gameObject.activeSelf == true)
		{
			TableParent.gameObject.SetActive(false);
			RectTransform listItemTransform = TableParent.transform.parent.GetComponent<RectTransform>();
			listItemTransform.sizeDelta = new Vector2(listItemTransform.sizeDelta.x,
					listItemTransform.sizeDelta.y - TableParent.GetComponent<RectTransform>().sizeDelta.y);
			TableParent.transform.SetParent((TableParent.transform.parent).parent);
		}
	}
#endif

	public void GetUpTable()
	{
		GameHelper.GetUpTable(selectTable, (table) => { UpdateTable(table); }, (error) => { });
	}

	public void WaitPlaceAtTable()
	{
	}

	private void UpdateTable(Table table)
	{
		InitTable(table);
		//tableListBehaviour.ChangeTableInfo(table);
	}

	public void OpenTable()
	{

		if (selectTable.isAtTheTable)
		{
			OpenTable(selectTable);
			return;
		}

		if (!selectTable.is_private)
		{
			ObserveAndOpenTable(selectTable, OpenTable);
		}
		else
		{
			passwordPopup.Show(selectTable, (isSuccess =>
			{
				if (isSuccess) OpenTable(selectTable);
			}));
		}
	}

	private void GetTableInfo(ulong id)
	{
		GameHelper.GetTable(id, resultTable =>
		{
			if (resultTable.isAtTheTable)
			{
				SocketClient.Instance.EnterTableChanel(resultTable.id);
				StartNewTable(resultTable);
			}
			else
			{
				ObserveAndOpenTable(resultTable, StartNewTable);
			}
		}, s => { });
	}

	private void ObserveAndOpenTable(Table table, Action<Table> callback)
	{
		GameHelper.ObserveTable(table, "", (tableResponse) => { callback(table); },
				(error) =>
				{
				});
	}

	private void OpenTable(Table table)
	{
#if UNITY_STANDALONE && !UNITY_EDITOR
        //StandaloneController.OpenNewWindow(table);
#else
		StartNewTable(table);
#endif
	}

	private void StartNewTable(Table table)
	{
		//StopRefreshTimer();
#if UNITY_STANDALONE
		StandaloneController.Instance.AddNewGame(table);
#endif
	}

	public Table GetSelectTable()
	{
		return selectTable;
	}

	public void SelectGame(List<GameType> typeGame)
	{
		tablePage.SetActive(typeGame.Contains(GameType.None) == false);
		homePage.SetActive(typeGame.Contains(GameType.None));
		//if (typeGame.Contains(GameType.None) == false) tableListBehaviour.ChangeGameType(typeGame);
#if !UNITY_ANDROID && !UNITY_WEBGL && !UNITY_IOS
		CloseSettings();
#endif
	}

	public void Logout()
	{
		var authManager = AuthManager.Instance;
		authManager.Logout((result) =>
		{
			if (result)
			{
				sessionSaver.Logout();
				SceneManager.LoadScene((int)SceneType.Login);
			}

		});

	}

	public void TouchStore(int window)
	{
		TouchStore();
		paymentPopup.SelectWindowOpen(window);
	}

	public void TouchStore()
	{
		paymentPopup.Show((paymentBody) =>
		{
			GameHelper.InitPayment(paymentBody, session => { OpenUrl(session.data); },
							s => { });
		});
	}

	private async void OpenUrl(string url)
	{
		if (string.IsNullOrEmpty(url)) return;
//#if UNITY_STANDALONE_WIN
//		webViewPrefab.gameObject.SetActive(true);
//		await webViewPrefab.WaitUntilInitialized();
//		webViewPrefab.WebView.LoadUrl(url);
//#else
		Application.OpenURL(url);
//#endif
	}

	public void CloseWebView()
	{
//#if UNITY_STANDALONE_WIN
//		webViewPrefab.gameObject.SetActive(false);
//#endif
	}

	public void OpenProfile()
	{
		profileWindow.gameObject.SetActive(true);
		profileWindow.Init();
	}

	public void CloseProfile()
	{
		profileWindow.gameObject.SetActive(false);
		profileWindow.Close();
	}

	public void ShowGames()
	{
#if UNITY_STANDALONE
		StandaloneController.Instance.ShowGames();
#endif
	}

	public void OpenSettings()
	{
		if (SettingsWindow && UserController.ReferenceData != null) SettingsWindow.Show();
	}

	public void CloseSettings()
	{
		if (SettingsWindow) SettingsWindow.Hide();
	}

	public void Welcome()
	{
		//WelcomeWindow.Show(this);
		PlayerPrefs.SetInt("Welcome" + UserController.User.id, 1);
	}

	public void OpenNetworkStatus()
	{
		NetworkStatusWindow.Show();
	}

	public void OpenSupport()
	{
		supportPopup.Show();
	}

	public void OpenPlayerInfo()
	{

		var popup = it.Main.PopupController.Instance.ShowPopup<SmartHudPopup>(PopupType.SmartHudOpponenty);
		popup.SetData(GameHelper.UserInfo.limited, GameHelper.UserInfo.user_stat);
	}

	public void UpdateCurrentTable()
	{
		//tableListBehaviour.RefreshFromServerCurrentTable();
	}

	public void StopRefreshTimer()
	{
		//tableListBehaviour.SetTimerRefreshTable(false);
	}
}