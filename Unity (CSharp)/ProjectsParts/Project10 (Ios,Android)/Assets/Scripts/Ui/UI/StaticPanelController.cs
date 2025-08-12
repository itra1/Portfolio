using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using Random = UnityEngine.Random;

/// <summary>
/// панельстатистики
/// </summary>
public class StaticPanelController : ScrollingMenu {

	public Action OnHome;
	public Action OnForward;

	private RunnerController runner;
	private GameManager manager;

	public GameObject bottonPanel;

	public GameObject enemyIcons;                                       // Ссылка на заголовки
	public GameObject enemyIconsObj;

	public GameObject[] leftDecor;
	public GameObject[] rightDecor;

	public Text distanceText;                           // Дистанция
	public Text coinsText;                              // Монет
	public Text cristallText;                           // Кристалов
	public Text pointText;                              // Очков

	public Text allCoinsText;                           // Суммарно монет
	public Text allCristallText;                        // Сумарно кристаллов
	public Text allBlackMarkText;                       // Суммарно черных меток

	private bool panelHide;

	public GameObject FbAuth;
	public GameObject FbRating;

	public GameObject continueButton;
	public GameObject restartButton;
	
	public int incrementCoins;

	public AudioClip resultScrolClip;

	private void Start() {
		bottonPanel.SetActive(false);
	}

	protected override void OnEnable() {
		base.OnEnable();
    Company.Live.LiveCompany.OnChange += EnergyChange;

		//int key = RunnerController.instance.keysInRaceCalc;
		continueButton.SetActive(RunnerController.Instance.levelComplited);
		restartButton.SetActive(!RunnerController.Instance.levelComplited);

		SetDecor();
		EnergyInit();
		InitFb();

		runner = RunnerController.Instance;
		AudioManager.HidePercent50Sound();

		VisibleEnemyIcons();
		InitPoints();
		
	}
	protected override void OnDisable() {
		base.OnDisable();
    Company.Live.LiveCompany.OnChange -= EnergyChange;
		if (OnClose != null) OnClose();
	}

	protected override void OnDestroy() {
		base.OnDestroy();
		AudioManager.HidePercent50Sound(false, true);
	}

	void SetDecor() {
		int leftNum = Random.Range(0, leftDecor.Length);
		for (int i = 0; i < leftDecor.Length; i++)
			leftDecor[i].SetActive(i == leftNum);

		int rightNum = Random.Range(0, rightDecor.Length);
		for (int i = 0; i < rightDecor.Length; i++)
			rightDecor[i].SetActive(i == rightNum);
	}

	void InitPoints() {
		int dist = (int)runner.thisDistantionPosition;
		int points = dist
												+ (runner.moneyInRace * 30)
												+ (runner.cristallInRace * 300)
												+ (runner.blackPointsInRace * 1000);

		for (int i = 0; i < runner.enemiesDeadCount.Length; i++) {

			switch (i) {
				case 0:
					points += runner.enemiesDeadCount[i] * 100;
					break;
				case 1:
					points += runner.enemiesDeadCount[i] * 150;
					break;
				case 2:
					points += runner.enemiesDeadCount[i] * 200;
					break;
				case 3:
					points += runner.enemiesDeadCount[i] * 600;
					break;
				case 4:
					points += runner.enemiesDeadCount[i] * 800;
					break;
				case 5:
					points += runner.enemiesDeadCount[i] * 1200;
					break;
			}

		}
		SetCounts();

		if (PlayerPrefs.GetInt("points") < points)
			PlayerPrefs.SetInt("points", points);

		PlayerPrefs.Save();
	}

	void SetCounts() {
		int allCoins = UserManager.coins/* + runner.moneyInRace*/;
		int allCristall = UserManager.Instance.cristall;
		int allBlackMark = UserManager.Instance.blackMark/* + runner.blackPointsInRace*/;

		allCoinsText.text = allCoins.ToString();
		PositiongCoinsIcon();
		allCristallText.text = allCristall.ToString();
		allBlackMarkText.text = allBlackMark.ToString();
	}

	void PositiongCoinsIcon() {
		Vector2 widthCoins = new Vector2(allCoinsText.preferredWidth, allCoinsText.rectTransform.parent.GetComponent<RectTransform>().sizeDelta.y);
		RectTransform coinsIcon = allCoinsText.transform.parent.Find("Image").GetComponent<RectTransform>();
		RectTransform coinsTextParent = allCoinsText.rectTransform.parent.GetComponent<RectTransform>();
		RectTransform coinsCountTextTrf = allCoinsText.GetComponent<RectTransform>();
		coinsTextParent.sizeDelta = new Vector2(coinsTextParent.sizeDelta.x, coinsTextParent.sizeDelta.y);
		coinsCountTextTrf.sizeDelta = new Vector2(widthCoins.x, coinsCountTextTrf.sizeDelta.y);

		float w1 = widthCoins.x + 10 + coinsIcon.sizeDelta.x;

		coinsCountTextTrf.anchoredPosition = new Vector2((w1 / 2 - coinsCountTextTrf.sizeDelta.x / 2), coinsCountTextTrf.anchoredPosition.y);
		coinsIcon.anchoredPosition = new Vector2(-(w1 / 2 - coinsIcon.sizeDelta.x / 2), coinsIcon.anchoredPosition.y);
	}

	public void ButtomHome() {

		if (OnHome != null) OnHome();

		UiController.ClickButtonAudio();
		panelHide = true;

	}

	public void ButtomRestart() {

		UiController.ClickButtonAudio();
		if (OnForward != null) OnForward();

	}

	public void calcDistance() {
		if (panelHide) return;
		int dist = (int)runner.thisDistantionPosition;
		StartCoroutine(Increment(UpdateDistance, 0, dist));
	}

	void UpdateDistance(int value) {
		distanceText.text = value.ToString() + " M";
	}

	public void calcCoins() {
		if (panelHide) return;
		StartCoroutine(Increment(UpdateCoins, 0, runner.moneyInRace));
	}

	void UpdateCoins(int value) {
		coinsText.text = value.ToString();
	}
	public void calcCristals() {
		if (panelHide) return;
		StartCoroutine(Increment(UpdateCristals, 0, runner.cristallInRace));
	}

	void UpdateCristals(int value) {
		cristallText.text = value.ToString() + " X 300";
	}

	public void calcPoints() {
		if (panelHide) return;
		int points = (int)runner.thisDistantionPosition
						+ (runner.moneyInRace * 30)
						+ (runner.cristallInRace * 300)
						+ (runner.blackPointsInRace * 1000);
		//+ (runner.questionInRaceList.Count * 5000);

		for (int i = 0; i < runner.enemiesDeadCount.Length; i++) {

			switch (i) {
				case 0:
					points += runner.enemiesDeadCount[i] * 100;
					break;
				case 1:
					points += runner.enemiesDeadCount[i] * 150;
					break;
				case 2:
					points += runner.enemiesDeadCount[i] * 200;
					break;
				case 3:
					points += runner.enemiesDeadCount[i] * 600;
					break;
				case 4:
					points += runner.enemiesDeadCount[i] * 800;
					break;
				case 5:
					points += runner.enemiesDeadCount[i] * 1200;
					break;
			}

		}
#if UNITY_IOS && !UNITY_EDITOR
        GameCenterController.instance.ReportScore(points);
#endif
		StartCoroutine(Increment(UpdatePoints, 0, points));
	}

	void UpdatePoints(int value) {
		pointText.text = value.ToString();
	}

	IEnumerator Increment(Action<int> funct, int from, int to) {
		int tmpVal = Mathf.Abs(to - from);
		int iter = 1;

		if (tmpVal > 50)
			iter = tmpVal / 50;

		ChangeCalcCounts(1);

		while (from != to) {
			yield return new WaitForSeconds(0.01f);
			if (from < to) {
				if (to < from + iter)
					from = to;
				else
					from += iter;
			} else if (from > to) {
				if (to > from - iter)
					from = to;
				else
					from -= iter;
			}
			funct(from);
		}

		ChangeCalcCounts(-1);
	}


	public void AcceptCoins() {
		StartCoroutine(Increment(UpdateCoins, runner.moneyInRace, runner.moneyInRace + incrementCoins));
		UserManager.coins += incrementCoins;
		incrementCoins = 0;
	}

	void VisibleEnemyIcons() {
		int[] icons = runner.enemiesDeadCount;

		int allCount = 0;

		foreach (int i in icons)
			if (i > 0) allCount++;

		RectTransform enemyIconsRect = enemyIcons.GetComponent<RectTransform>();
		enemyIconsRect.sizeDelta = new Vector2(195 * allCount, enemyIconsRect.sizeDelta.y);

		int num = 0;
		for (int i = 0; i < icons.Length; i++) {
			if (icons[i] > 0) {
				GameObject clone = Instantiate(enemyIconsObj, Vector3.zero, Quaternion.identity) as GameObject;
				//clone.transform.parent = enemyIcons.transform;
				clone.GetComponent<RectTransform>().SetParent(enemyIcons.transform);
				clone.transform.localScale = Vector2.one;
				clone.transform.localPosition = Vector3.zero;
				clone.transform.localPosition = new Vector2((num * 175), 0);

				clone.GetComponent<StatisticEnemyIcon>().SetCalues(icons[i].ToString(), i);
				clone.SetActive(true);
				num++;
			}
		}
	}

	void ChangeCalcCounts(int num) {

		incrementCoins += num;

		if (incrementCoins > 0 && !GetComponent<AudioSource>().isPlaying) {
			GetComponent<AudioSource>().clip = resultScrolClip;
			GetComponent<AudioSource>().loop = true;
			GetComponent<AudioSource>().Play();
		}

		if (incrementCoins <= 0 && GetComponent<AudioSource>().isPlaying)
			GetComponent<AudioSource>().Stop();

	}

	#region Facebook

	void InitFb() {
		FacebookAuth();
	}

	public void FBLogin() {
#if PLUGIN_FACEBOOK
    UiController.ClickButtonAudio();
    FBController.instance.FBlogin(FacebookAuth);
#endif
	}

	// калбак авторизации

	void FacebookAuth() {
		if (FBController.CheckFbLogin)
			GetComponent<Animator>().SetBool("fbAuth", true);
		else
			GetComponent<Animator>().SetBool("fbAuth", false);
	}


	public void FbInvate() {

		UiController.ClickButtonAudio();
#if PLUGIN_FACEBOOK
		FBController.instance.InvateFriendFb();
#endif
	}

	public void ShareResult() {
#if PLUGIN_FACEBOOK
    FBController.ShareResult();
#endif
	}

	#endregion

	#region Кнопка энергии

	public Text energyText;               // Текст количества энергии

	/// <summary>
	/// Кнопка энергии
	/// </summary>
	public void EnergyButton() {
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

	TimeSpan deltaTime;

	public string GetUnlimTime() {
		deltaTime = Company.Live.LiveCompany.Instance.unlimRemain;
		return (deltaTime.TotalDays > 0 ? String.Format("{0:00}", deltaTime.TotalDays) : "") + String.Format("{0:00}:{1:00}:{2:00}", deltaTime.TotalHours, deltaTime.TotalMinutes, deltaTime.TotalMilliseconds);
	}

	#endregion
	
	public override void BackButton() {
		ButtomHome();
	}

}
