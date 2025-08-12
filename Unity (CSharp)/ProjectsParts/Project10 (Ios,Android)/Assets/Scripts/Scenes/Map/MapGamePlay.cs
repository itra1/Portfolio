using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;
using ExEvent;

public class MapGamePlay : PanelUi {

	public static event Action OnTapDown;
	public static event Action OnTapUp;
	public static event Action<Vector2> OnTapDrag;

  public GameObject energyPanel;


	public Action OnHome;
	public Action OnPlay;
	public Action OnChangeKey;

	public TouchHelper touchHelper;

	private string toScene = "";
	private bool isActive = true;

	protected override void OnEnable() {
		base.OnEnable();
    Company.Live.LiveCompany.OnChange += EnergyChange;
		touchHelper.OnTapDrag = OnDragMouse;
		touchHelper.OnTapDown = OnTapDownMouse;
		touchHelper.OnTapUp = OnTapUpMouse;
		isActive = true;

		UpdateParametrs();
		// Установка значений счетчиков
		SetCountText();

    energyPanel.SetActive(!Company.Live.LiveCompany.Instance.isUnlimited);
    
    EnergyInit();
	}
	protected override void OnDisable() {
		base.OnDisable();
    Company.Live.LiveCompany.OnChange -= EnergyChange;
		if (OnClose != null) OnClose();
	}

	void UpdateParametrs() {
		SetCountText();
	}

	// Переходим на другую сцену
	public void LoadMainScene() {
		toScene = "Runner";
	}

	public void ShowBlackBg() {
		if (toScene != "")
			SceneManager.LoadScene(toScene);
	}

	public void ClosePanel() {

		GetComponent<Animator>().SetTrigger("hide");
	}

	public void Close() {
		if (!isActive) gameObject.SetActive(false);
	}

	void Update() {
		if (Company.Live.LiveCompany.Instance.isUnlim)
			EnergyUnlimUpdate();
	}

	public void PlayButton() {
		if (!isActive) return;
		isActive = false;
		UiController.ClickButtonAudio();
		if (OnPlay != null) OnPlay();
	}

	public void HomeButton() {
		if (!isActive) return;
		isActive = false;
		UiController.ClickButtonAudio();

		if (OnHome != null) OnHome();
	}

	

	public void OnDragMouse(Vector2 deltaDrag) {
		if (OnTapDrag != null) OnTapDrag(deltaDrag);
	}

	public void OnTapDownMouse() {
		if (OnTapDown != null) OnTapDown();
	}

	public void OnTapUpMouse() {
		if (OnTapUp != null) OnTapUp();
	}

	#region Квесты

	GameObject questionPanel;                        // Панель квестов

	public void questioPanelCange(bool flag) {
		UiController.ClickButtonAudio();

		if (!Questions.QuestionManager.Instance.ExistsActiveQuests())
			return;
	}

	#endregion

	#region Значения счетчиков

	public Text coinsCountText;                                     // Количество монет
	public Text blackMarkText;                                      // Количество черных меток
	public Text distantionText;                                     // Дистанция

	public void SetCountText() {
		SetBestDistPosition();
		SetCoinsCountText(new RunEvents.CoinsChange(UserManager.coins));
		SetBlackMarkCountText();
		SetKeysCountText();

		Vector2 widthCoins = new Vector2(coinsCountText.preferredWidth, coinsCountText.rectTransform.parent.GetComponent<RectTransform>().sizeDelta.y);

		Vector2 widthBlackMark = new Vector2(blackMarkText.preferredWidth + 90, blackMarkText.rectTransform.parent.GetComponent<RectTransform>().sizeDelta.y);
		blackMarkText.rectTransform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(widthBlackMark.x, widthBlackMark.y);
		Vector3 allWidth = new Vector3(widthCoins.x + /*widthKey.x + */widthBlackMark.x, widthCoins.y, 0);
		blackMarkText.rectTransform.parent.parent.GetComponent<RectTransform>().sizeDelta = allWidth;

		blackMarkText.rectTransform.parent.GetComponent<RectTransform>().localPosition = new Vector3(-(allWidth.x / 2) + widthCoins.x/* + widthKey.x*/, 0, 0);
	}

	public Transform GetCoinsCountPositionInWorld() {
		return coinsCountText.transform;
	}

	void SetBestDistPosition() {
		distantionText.text = ((int)UserManager.Instance.survivleMaxRunDistance).ToString();
	}

	[ExEventHandler(typeof(RunEvents.CoinsChange))]
	public void SetCoinsCountText(RunEvents.CoinsChange coins) {
		coinsCountText.text = coins.coins.ToString();

		Vector2 widthCoins = new Vector2(coinsCountText.preferredWidth, coinsCountText.rectTransform.parent.GetComponent<RectTransform>().sizeDelta.y);
		RectTransform coinsIcon = coinsCountText.transform.parent.Find("Image").GetComponent<RectTransform>();
		RectTransform coinsTextParent = coinsCountText.rectTransform.parent.GetComponent<RectTransform>();
		RectTransform coinsCountTextTrf = coinsCountText.GetComponent<RectTransform>();
		coinsTextParent.sizeDelta = new Vector2(coinsTextParent.sizeDelta.x, coinsTextParent.sizeDelta.y);
		coinsCountTextTrf.sizeDelta = new Vector2(widthCoins.x, coinsCountTextTrf.sizeDelta.y);

		float w1 = widthCoins.x + 10 + coinsIcon.sizeDelta.x;

		coinsCountTextTrf.anchoredPosition = new Vector2((w1 / 2 - coinsCountTextTrf.sizeDelta.x / 2), coinsCountTextTrf.anchoredPosition.y);
		coinsIcon.anchoredPosition = new Vector2(-(w1 / 2 - coinsIcon.sizeDelta.x / 2), coinsIcon.anchoredPosition.y);

	}

	void SetBlackMarkCountText() {
		blackMarkText.text = UserManager.Instance.blackMark.ToString();
	}

	void SetKeysCountText() {

		//keysText.text = PlayerManager.instance.keys.ToString();
	}

	public void ChangeKey(int count) {
		int key = UserManager.Instance.keys;
		UserManager.Instance.keys = key + count;
		SetKeysCountText();
		if (OnChangeKey != null) OnChangeKey();
	}

	public void ChangeCoins(int count) {
		int coins = UserManager.coins;
		UserManager.coins = coins + count;
		SetCoinsCountText(new RunEvents.CoinsChange(UserManager.coins));
	}

	#endregion
	
	#region Кнопка энергии

	public Text energyText;               // Текст количества энергии

	/// <summary>
	/// Кнопка энергии
	/// </summary>
	public void EnergyButton(Action OnClose) {
		EnergyDialog salePanel = UiController.ShowUi<EnergyDialog>();
		salePanel.gameObject.SetActive(true);
		salePanel.OnClose = OnClose;
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

	TimeSpan deltaTime;

	public string GetUnlimTime() {
		deltaTime = Company.Live.LiveCompany.Instance.unlimRemain;
		return (deltaTime.TotalDays > 0 ? String.Format("{0:00}", deltaTime.TotalDays) : "") + String.Format("{0:00}:{1:00}:{2:00}", deltaTime.TotalHours, deltaTime.TotalMinutes, deltaTime.TotalMilliseconds);
	}

	public override void BackButton() {
		HomeButton();
	}

	#endregion

}
