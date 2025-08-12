using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Главное меню
/// </summary>
public class SurvivalMenuPanel : PanelUi {

	public Action OnStartRun;
	public Action OnShop;
	public Action OnMap;
	public Action OnCredit;
	public Action OnEnergy;
	
	Animator animComp;
	
	public GameObject facebookPanel;
	public GameObject gameCenterPanel;

	public RouletteController rouletteController;
	[HideInInspector]
	public bool isActiveAction;

	protected override void OnEnable() {
		base.OnEnable();
    //UiController.ChangeCount += InitCounts;
    Company.Live.LiveCompany.OnChange += EnergyChange;

		isActiveAction = false;

		animComp = GetComponent<Animator>();
		animComp.SetBool("showPanel", true);

		AudioManager.OnMusic += OnMusicChange;
		AudioManager.OnEffect += OnEffectChange;
		OnMusicChange(AudioManager.Instance.isMusic);
		OnEffectChange(AudioManager.Instance.isEffect);

		OnEnableFacebook();

		RouletteInit();

		InitCounts();
		//InitSettings();
		SetCountsShop();
		EnergyInit();

#if UNITY_IOS || UNITY_EDITOR
		InitGameCenter();
#endif
		InitLanuage();
		ChangeLanuage();
	}

	protected override void OnDisable() {
		base.OnDisable();
    Company.Live.LiveCompany.OnChange -= EnergyChange;
		//UiController.ChangeCount -= InitCounts;
		OnDisableFacebook();
		DeInitLanuage();
		if (OnClose != null)
			OnClose();

	}

	public void Init() {
		OnMusicChange(AudioManager.Instance.isMusic);
		OnEffectChange(AudioManager.Instance.isEffect);
	}

	public void Close() {
		Debug.Log("Close");
		StartCoroutine(hideThisPanel());
	}

	public void StartRun() {
		if (isActiveAction)
			return;
			isActiveAction = true;

		if (OnStartRun != null)
			OnStartRun();

	}

	#region Счетчики

	public Text coinsCountText;
	public Text keysCountText;
	public Text blackMarkText;
	public Text bestDistText;

	public void InitCounts() {
		coinsCountText.text = UserManager.coins.ToString();
		keysCountText.text = UserManager.Instance.keys.ToString();
		blackMarkText.text = UserManager.Instance.blackMark.ToString();
		bestDistText.text = (int)UserManager.Instance.survivleMaxRunDistance + " M";

		Vector2 widthCoins = new Vector2(coinsCountText.preferredWidth, coinsCountText.rectTransform.parent.GetComponent<RectTransform>().sizeDelta.y);
		RectTransform coinsIcon = coinsCountText.transform.parent.Find("Image").GetComponent<RectTransform>();
		RectTransform coinsTextParent = coinsCountText.rectTransform.parent.GetComponent<RectTransform>();
		RectTransform coinsCountTextTrf = coinsCountText.GetComponent<RectTransform>();
		coinsTextParent.sizeDelta = new Vector2(coinsTextParent.sizeDelta.x, coinsTextParent.sizeDelta.y);
		coinsCountTextTrf.sizeDelta = new Vector2(widthCoins.x, coinsCountTextTrf.sizeDelta.y);

		float w1 = widthCoins.x + 10 + coinsIcon.sizeDelta.x;

		coinsCountTextTrf.anchoredPosition = new Vector2((w1 / 2 - coinsCountTextTrf.sizeDelta.x / 2), coinsCountTextTrf.anchoredPosition.y);
		coinsIcon.anchoredPosition = new Vector2(-(w1 / 2 - coinsIcon.sizeDelta.x / 2), coinsIcon.anchoredPosition.y);

		//Vector2 widthKey = new Vector2(keysCountText.preferredWidth + 90,keysCountText.rectTransform.parent.GetComponent<RectTransform>().sizeDelta.y);
		//keysCountText.rectTransform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(widthKey.x, widthKey.y);

		Vector2 widthBlackMark = new Vector2(blackMarkText.preferredWidth + 90, blackMarkText.rectTransform.parent.GetComponent<RectTransform>().sizeDelta.y);
		blackMarkText.rectTransform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(widthBlackMark.x, widthBlackMark.y);
		Vector3 allWidth = new Vector3(widthCoins.x + /*widthKey.x + */widthBlackMark.x, widthCoins.y, 0);
		blackMarkText.rectTransform.parent.parent.GetComponent<RectTransform>().sizeDelta = allWidth;


		keysCountText.rectTransform.parent.GetComponent<RectTransform>().localPosition = new Vector3(-(allWidth.x / 2) + widthCoins.x, 0, 0);
		blackMarkText.rectTransform.parent.GetComponent<RectTransform>().localPosition = new Vector3(-(allWidth.x / 2) + widthCoins.x/* + widthKey.x*/, 0, 0);

	}
	#endregion

	#region Карта

	public string sceneMap;

	public void MapButton() {

		if (isActiveAction)
			return;
		isActiveAction = true;

		UiController.ClickButtonAudio();

		if (OnMap != null)
			OnMap();
	}

	#endregion

	#region Геймцентер

	public void InitGameCenter() {
		animComp.SetBool("gameCenterButton", true);
	}

	public void GameCenterButtom() {
		if (isActiveAction)
			return;
		isActiveAction = true;

		UiController.ClickButtonAudio();
		GameCenterController.GameCenterLeaderBoard();
	}

	#endregion

	#region Facebook

	public Image fbAvatar;                          // Картинка аватара
	public GameObject nearFriend;                   // Объект ближайшего друга
	bool fbAuth;                                    // Флаг авторизации

	void OnEnableFacebook() {
		FBController.authorizationComplited += FacebookAuth;
		FacebookAuth();
	}

	void OnDisableFacebook() {
		FBController.authorizationComplited -= FacebookAuth;
	}

	// Событие Нажатия кнопки
	public void FBLogin() {

		if (isActiveAction)
			return;
		isActiveAction = true;

#if PLUGIN_FACEBOOK
		UiController.ClickButtonAudio();
		FBController.fbLogin(FacebookAuth);
#endif
	}

	/// <summary>
	/// Ответ при авторизации
	/// </summary>
	void FacebookAuth() {
		if (FBController.CheckFbLogin) {
			animComp.SetBool("fbAuth", true);
		}
	}

	void facebookLoginEvent() {

		// Обновляем счетчики, на случай, если подтянута новая дистанция
		InitCounts();

		StartCoroutine(WaitFb());
		StartCoroutine(WaitLeaderBoard());
	}

	IEnumerator WaitFb() {
		int ii = 0;
		while (ii < 240) {
			ii++;
			yield return new WaitForSeconds(0.5f);
			if (FBController.instance != null && FBController.instance.userName != "") {
				StartCoroutine(DownloadUserAvatar(FBController.instance.userPictureUrl));
				ii = 240;
				yield return 0;
			}
		}
	}

	IEnumerator WaitLeaderBoard() {
		int ii = 0;
		while (ii < 240) {
			ii++;
			yield return new WaitForSeconds(0.5f);
			if (Apikbs.Instance.nearFriend.bestDistantion != "") {
				if (Apikbs.Instance.nearFriend.fb != FBController.GetUserId)
					StartCoroutine(DownloadNearFriendAvatar(Apikbs.Instance.nearFriend.picture, Apikbs.Instance.nearFriend.bestDistantion));

				ii = 240;
				yield return 0;
			}
		}
	}

	IEnumerator DownloadUserAvatar(string avaUrl) {
		WWW www = new WWW(avaUrl);
		yield return www;

		if (www.texture.width >= 50 || www.texture.height >= 50)
			fbAvatar.GetComponent<Image>().sprite = Sprite.Create(www.texture, new Rect(0, 0, 50, 50), new Vector2(0, 0));
	}

	IEnumerator DownloadNearFriendAvatar(string avaUrl, string bestDistance) {
		animComp.SetTrigger("nearFriend");
		nearFriend.transform.Find("Distance").GetComponent<Text>().text = bestDistance + " M";
		WWW www = new WWW(avaUrl);
		yield return www;

		if (www.texture.width >= 48 || www.texture.height >= 48)
			nearFriend.transform.Find("Round").Find("RoundPhoto").GetComponent<Image>().sprite = Sprite.Create(www.texture, new Rect(0, 0, 48, 48), new Vector2(0, 0));
	}

	#endregion

	#region Настройки

	public Animator settingAnim;

	public void OnSettingButton() {
		settingAnim.SetBool("show", !settingAnim.GetBool("show"));
	}

	void OnMusicChange(bool flag) {
		settingAnim.SetBool("music", flag);
	}

	void OnEffectChange(bool flag) {
		settingAnim.SetBool("effect", flag);
	}

	public void OnSettingMusic() {
		AudioManager.Instance.isMusic = !AudioManager.Instance.isMusic;
	}

	public void OnSettingSound() {
		AudioManager.Instance.isEffect = !AudioManager.Instance.isEffect;
	}

	public void CreditsButton() {

		if (isActiveAction)
			return;
		isActiveAction = true;

		UiController.ClickButtonAudio();
		animComp.SetBool("showPanel", false);
		animComp.SetBool("settings", false);
		animComp.SetBool("shop", false);
		StartCoroutine(showCredits());
		if (OnCredit != null)
			OnCredit();
	}

	IEnumerator showCredits() {
		yield return new WaitForSeconds(0.5f);
		gameObject.SetActive(false);
		
	}

	#endregion

	#region Магазин

	public Text allShopFollowElements;

	// Инициализация счетчиков в магазине
	void SetCountsShop() {

		int allProducts =
			Config.GetFolowsCount((Shop.Products.ProductType.all));

		if (allProducts <= 0) {
			allShopFollowElements.transform.parent.gameObject.SetActive(false);
		} else {
			allShopFollowElements.transform.parent.gameObject.SetActive(true);
			allShopFollowElements.text = allProducts.ToString();
		}
		
	}

	public void ShopButton() {

		if (isActiveAction)
			return;
		isActiveAction = true;
		
		UiController.ClickButtonAudio();
		Debug.Log("Hide");
		StartCoroutine(hideThisPanel());

		if (OnShop != null)
			OnShop();
		//bool flag = animComp.GetBool("shop");
		//animComp.SetBool("shop", !flag);
	}

	public void LoadShip() {
		SceneManager.LoadScene("ShipRun");
	}

	IEnumerator hideThisPanel() {
		animComp.SetBool("showPanel", false);
		animComp.SetBool("settings", false);
		animComp.SetBool("shop", false);
		yield return new WaitForSeconds(0.5f);
		gameObject.SetActive(false);
		//UiController.instance.ShopDialogShow();

	}

	public void ClosePanel() {
		if (!gameObject.activeInHierarchy)
			return;
		Debug.Log("Close");
		StartCoroutine(closePanel());
	}
	/// <summary>
	/// Закрытие панели
	/// </summary>
	/// <returns></returns>
	IEnumerator closePanel() {

		animComp.SetBool("showPanel", false);
		animComp.SetBool("settings", false);
		animComp.SetBool("shop", false);
		yield return new WaitForSeconds(0.5f);
		gameObject.SetActive(false);
	}

	#endregion

	#region Рулетка

	public GameObject roulettePanel;
	public GameObject roulettePriceButton;
	public Text blackMarkInRouletteText;
	int blackMarkPrice;

	/// <summary>
	/// Инициализация рулетки
	/// </summary>
	void RouletteInit() {

    return;

		if (GameManager.activeLevelData.gameMode == GameMode.survival) {
			if (roulettePanel != null)
				roulettePanel.SetActive(true);

			animComp.SetBool("enableByeRoulette", false);
			animComp.SetBool("rouletteBye", false);
			animComp.SetBool("roulette", true);
		} else {
			if (roulettePanel != null)
				roulettePanel.SetActive(false);
		}
	}

	/// <summary>
	/// Покупка элемента
	/// </summary>
	public void RouletteButtomBye() {

		UiController.ClickButtonAudio();
		int blackMark = UserManager.Instance.blackMark;

		if (blackMark <= 0 && UnityAdsController.adsReady && GameManager.DayAfterStart(1)) {
			UnityAdsController.ShowVideo(AddVideoAnsver);
		}
	}

	/// <summary>
	/// Ответ от воспроизведения рекламного видео
	/// </summary>
	/// <param name="result"></param>
	void AddVideoAnsver(bool result) {
		if (result) {
			animComp.SetBool("enableByeRoulette", false);
			animComp.SetBool("rouletteBye", false);
		}
	}

	public void RouletteButton(bool flag) {
		animComp.SetBool("RouletteButtonPress", flag);
	}

	/// <summary>
	/// Инициадизация кнопки покупки
	/// </summary>
	public void RoulettePriceButtom() {

		if (TutorialManager.Instance.Istutorial && GameManager.DayAfterStart(1) && animComp.GetBool("rouletteBye"))
			animComp.SetBool("enableByeRoulette", true);
	}

	#endregion

	#region Lanuage

	public Text shopButton;
	public Text mapButton;
	public Text leaderMyTitle;
	public Text leaderFriend;
	//public TextMesh startClickText;

	/// <summary>
	/// Инифиализация локализации
	/// </summary>
	void InitLanuage() {
		LanguageManager.changeLanuage += ChangeLanuage;
	}

	/// <summary>
	/// Деинициализация локализации
	/// </summary>
	void DeInitLanuage() {
		LanguageManager.changeLanuage -= ChangeLanuage;
	}

	/// <summary>
	/// Изменение значений языка
	/// </summary>
	void ChangeLanuage() {
		if (shopButton != null)
			shopButton.text = LanguageManager.GetTranslate("main_Shop");
		if (mapButton != null)
			mapButton.text = LanguageManager.GetTranslate("main_Map");
		if (leaderMyTitle != null)
			leaderMyTitle.text = LanguageManager.GetTranslate("main_LbMy");
		if (leaderFriend != null)
			leaderFriend.text = LanguageManager.GetTranslate("main_LbFriends");
		//if (startClickText != null)
		//  startClickText.text = Lanuage.GetTranslate("main_startTap");

	}

	#endregion

	#region Кнопка энергии

	public Text energyText;               // Текст количества энергии

	/// <summary>
	/// Кнопка энергии
	/// </summary>
	public void EnergyButton() {
		if (OnEnergy != null)
			OnEnergy();
		//UiController.instance.EnergyPanelShow(true);
	}

	public void EnergyUnlimUpdate() {
		EnergyChangeText(GetUnlimTime());
	}

	/// <summary>
	/// Инифиализация данных по энергии
	/// </summary>
	public void EnergyInit() {

		EnergyChangeText((Company.Live.LiveCompany.Instance.value == -1 ? GetUnlimTime() : Company.Live.LiveCompany.Instance.value.ToString()));
	}

	/// <summary>
	/// Событие изменения значения энергии
	/// </summary>
	/// <param name="energyValue"></param>
	public void EnergyChange(float energyValue) {
		EnergyChangeText((energyValue == -1 ? GetUnlimTime() : energyValue.ToString()));
	}

	/// <summary>
	/// Изменение отображения энергии на экране
	/// </summary>
	/// <param name="energyString"></param>
	public void EnergyChangeText(string energyString) {
		energyText.text = energyString;
		RectTransform rectBlack1 = energyText.transform.parent.GetComponent<RectTransform>();
		rectBlack1.sizeDelta = new Vector2(energyText.preferredWidth + 54, rectBlack1.sizeDelta.y);
		RectTransform rectBlack2 = rectBlack1.transform.parent.transform.parent.GetComponent<RectTransform>();
		rectBlack2.sizeDelta = new Vector2(rectBlack1.sizeDelta.x + 65, rectBlack2.sizeDelta.y);
	}

	private TimeSpan deltaTime;

	public string GetUnlimTime() {
		deltaTime = Company.Live.LiveCompany.Instance.unlimRemain;
		return (deltaTime.TotalDays > 0 ? String.Format("{0:00}", deltaTime.TotalDays) : "") + String.Format("{0:00}:{1:00}:{2:00}", deltaTime.TotalHours, deltaTime.TotalMinutes, deltaTime.TotalMilliseconds);
	}

	#endregion

	public override void BackButton() {
		Close();
	}

}
