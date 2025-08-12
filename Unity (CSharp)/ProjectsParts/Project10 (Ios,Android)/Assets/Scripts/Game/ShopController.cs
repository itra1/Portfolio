using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum shopTypes { upgrades, powers, clothes, mounts, golds, all }

[System.Serializable]
public struct PagesShop {
	public int pages;
	public GameObject backBg;
	public GameObject panel;
	public string titleText;
	public shopTypes shopType;
}

[System.Serializable]
public struct ShopElementPrice {
	public int coins;
	public int cristall;
}


public class ShopController : Singleton<ShopController> {
	
	public GameObject mainCamera;                                           // Ссылка на объект камеры
	public GameObject topPanel;                                             // Верхняя панель навигации
	public GameObject allPanel;                                             // Ссылка на объект меню
	public GameObject allPanelBg;                                           // Ссылка на объект beka

	public GameObject roundBlack;

	public PagesShop[] pagesShop;
	private int numberPages;
	private int needPages;

	private bool muve;

	public delegate void PanelesMoved();
	public static event PanelesMoved movedPanells;

	public delegate void CheckSumm(bool force);                             // Делегат проверки сумм
	public static event CheckSumm CheckAllSummEvent;                        // Событие проверки сумм

	[HideInInspector]
	public int coins;                                     // Количество монет
	[HideInInspector]
	public int cristall;                                  // Количество кристалловы
	public Text coinsText;                                                  // Текст заголовка с монетами
	public RectTransform coinsTextParent;
	public Text cristallText;                                               // Текст заголовка с кристаллами

	public static event Action<bool> allReset;

	public AudioClip shopClip;
	public AudioClip clickClip;
	public AudioClip bottomAudio;
	public AudioClip byeBottomAudio;
	public AudioClip pageTurn;
	public AudioClip productTurn;

	public int dislayWidth;

	public static event Action<shopTypes> SetShopPage;

	protected override void Awake() {
		base.Awake();
		roundBlack.SetActive(true);
		mainCamera.SetActive(false);

	}

	void Start() {
		InitParametrs();
		SetShop(pagesShop[numberPages].shopType);
		UpdateRes();
		InitLanuage();

		SetShop(pagesShop[numberPages].shopType);
	}

	bool isEnable;
	void OnEnable() {
		InitRount();
		ChangeLanuage();
		isEnable = true;
	}
	
	void OnDisable() {
		DeInitLanuage();
	}

	void Update() {

		if (isEnable) {
			isEnable = false;
			dislayWidth = (int)allPanel.GetComponent<RectTransform>().rect.width;
		}

		if (muve)
			AllPanekMuve();
	}

	/// <summary>
	/// Инициализация параметров
	/// </summary>
	void InitParametrs() {
		mainCamera.SetActive(true);
		dislayWidth = (int)allPanel.GetComponent<RectTransform>().rect.width;
	}

	float koef;
	float delta;

	void AllPanekMuve() {
		if (needPages != numberPages) {
			int toPage = 0;
			for (int i = 0; i < pagesShop.Length; i++) {
				if (pagesShop[i].pages == needPages) toPage = i;
			}

			if (needPages < numberPages) {
				allPanel.transform.localPosition += new Vector3(dislayWidth * 1 * 4, 0, 0) * Time.deltaTime;
				if (allPanel.transform.localPosition.x > -pagesShop[toPage].panel.transform.localPosition.x) {
					allPanel.transform.localPosition = -pagesShop[toPage].panel.transform.localPosition;
					numberPages = needPages;
					muve = false;
					SetActivePages();
				}
			} else {
				allPanel.transform.localPosition -= new Vector3(dislayWidth * 1 * 4, 0, 0) * Time.deltaTime;
				if (allPanel.transform.localPosition.x < -pagesShop[toPage].panel.transform.localPosition.x) {
					allPanel.transform.localPosition = -pagesShop[toPage].panel.transform.localPosition;
					numberPages = needPages;
					muve = false;
					SetActivePages();
				}
			}
			if (movedPanells != null) movedPanells();
		}
	}

	void SetActivePages() {
		for (int i = 0; i < pagesShop.Length; i++) {
			if (numberPages == pagesShop[i].pages) {
				shopTitle.text = LanguageManager.GetTranslate(pagesShop[i].titleText);
				positingPages(i - 1, numberPages - 1);
				positingPages(i + 1, numberPages + 1);
				positingPages(i - 2, numberPages - 2);
				positingPages(i + 2, numberPages + 2);
			}
		}
	}

	void positingPages(int numberInList, int numberPages) {
		if (numberInList < -1) numberInList = pagesShop.Length - 2;
		else if (numberInList < 0) numberInList = pagesShop.Length - 1;
		else if (numberInList >= pagesShop.Length + 1) numberInList = 1;
		else if (numberInList >= pagesShop.Length) numberInList = 0;


		if (dislayWidth == 0) {
			dislayWidth = (int)mainCamera.GetComponent<Camera>().pixelWidth;
		}

		pagesShop[numberInList].panel.transform.localPosition = new Vector3(dislayWidth * 1.5f * numberPages, 0, 0);
		pagesShop[numberInList].pages = numberPages;
	}

	/// <summary>
	/// Покупка
	/// </summary>
	/// <param name="typeElement"></param>
	/// <returns></returns>
	public bool ByeItem(ShopElementType typeElement) {
		int allCoins = UserManager.coins;
		int allCristall = UserManager.Instance.cristall;
		int level = PlayerPrefs.GetInt(typeElement.ToString());
		int thisProduct = level;

		Shop.Products.Product shopPrice = GetElementPrices(typeElement);

		//if (shopPrice.unLimLevels) level = 0;

		bool byed = false;

		//if (System.Array.IndexOf(new ShopElementType[] { ShopElementType.GoldJack1, ShopElementType.GoldJack2, ShopElementType.GoldJack3, ShopElementType.GoldJack4, ShopElementType.GoldJack5, ShopElementType.Special }, typeElement) >= 0) {
			
		//} else if (shopPrice.levels[level].coins <= allCoins && shopPrice.levels[level].cristall <= allCristall) {

		//	YAppMetrica.Instance.ReportEvent("Магазин: куплено " + LanguageManager.GetTranslate(shopPrice.title));
		//	GAnalytics.Instance.LogEvent("Магазин", "Покупка", LanguageManager.GetTranslate(shopPrice.title), 1);

		//	AudioManager.PlayEffects(byeBottomAudio, AudioMixerTypes.shopEffect);
		//	UserManager.coins = allCoins + shopPrice.levels[level].coins;
		//	UserManager.Instance.cristall = allCristall + shopPrice.levels[level].cristall;
		//	PlayerPrefs.SetInt(typeElement.ToString(), thisProduct + 1);
		//	PlayerPrefs.Save();
		//	UpdateRes(true);

		//	byed = true;
		//}
		allReset(false);
		return byed;
	}

	public void BottomAudio() {
		AudioManager.PlayEffect(bottomAudio, AudioMixerTypes.shopEffect);
	}

	public void SetShop(shopTypes shopType) {
		mainCamera.SetActive(true);

		if (SetShopPage != null) SetShopPage(shopType);

		for (int i = 0; i < pagesShop.Length; i++) {
			positingPages(i, i);
		}

		foreach (PagesShop one in pagesShop) {
			if (one.shopType == shopType) {
				allPanel.transform.localPosition = -one.panel.transform.localPosition;
				numberPages = one.pages;
				SetActivePages();
			}
		}
		if (SetShopPage != null) SetShopPage(shopType);
	}

	void EventActiv() {
		ChangeLanuage();
	}

	public void ShowPages(shopTypes shopPages) {
		SetShop(shopPages);
	}
	
	public void UpdateRes(bool plavno = false) {
		coins = UserManager.coins;
		cristall = UserManager.Instance.cristall;

		if (plavno) {
			int fromCoins = int.Parse(coinsText.text);
			StartCoroutine(Increment(coinsText, fromCoins, coins));
			int fromCrista = int.Parse(cristallText.text);
			StartCoroutine(Increment(cristallText, fromCrista, cristall));
		} else {
			coinsText.text = coins.ToString();
			coinsTextParent.sizeDelta = new Vector2(coinsText.preferredWidth + 120, coinsTextParent.sizeDelta.y);
			cristallText.text = cristall.ToString();
		}
		if (CheckAllSummEvent != null) CheckAllSummEvent(false);
	}

	IEnumerator Increment(Text textObject, int from, int to) {
		int iter = Mathf.Abs(to - from) / 50;

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
			textObject.text = from.ToString();
		}
	}

	public void AllResetAnim() {
		if (allReset != null) allReset(false);
	}

	IEnumerator ToMain() {
		yield return new WaitForSeconds(1f);
		RunnerController.SetMenuMusic();
		mainCamera.SetActive(false);
		GameManager.Instance.HideShopPanel();
	}

	public Shop.Products.Product GetElementPrices(ShopElementType elementType) {
		return Shop.ShopManager.Instance.GetProduct(elementType.ToString());
	}

	#region Эффект перехода

	void InitRount() {
		roundBlack.SetActive(true);
		roundBlack.GetComponent<FillBlack>().OpenScene(Vector3.zero, null);
	}

	void CloseRound() {
		roundBlack.SetActive(true);
		roundBlack.GetComponent<FillBlack>().CloseScene(Vector3.zero, 30, 50);
	}

	#endregion

	#region Buttons
	/// <summary>
	/// Выход из магазина
	/// </summary>
	public void ExitShop() {
		AudioManager.PlayEffect(clickClip, AudioMixerTypes.shopEffect);
		allReset(false);
		CloseRound();
		//GetComponent<Animator>().SetTrigger("close");
		//StartCoroutine(ToMain());
    RunnerController.SetMenuMusic();
    mainCamera.SetActive(false);
    GameManager.Instance.HideShopPanel();
  }
	/// <summary>
	/// Реакция на кнопку перемещения в сторону
	/// </summary>
	/// <param name="toPages"></param>
	public void PagesMuved(int toPages) {
		AudioManager.PlayEffect(pageTurn, AudioMixerTypes.shopEffect);
		needPages = numberPages + toPages;
		muve = true;
	}

	/// <summary>
	/// Кнопка увеличения баланса, применяется для отладки
	/// </summary>
	public void Add() {
		AudioManager.PlayEffect(clickClip, AudioMixerTypes.shopEffect);
		UserManager.coins += 5000;
		UserManager.Instance.cristall += 5;
		UpdateRes();
	}

	/// <summary>
	/// Сброс параметров игрока на дефалтные
	/// </summary>
	public void Reset() {
		AudioManager.PlayEffect(clickClip, AudioMixerTypes.shopEffect);
		GameManager.StartFirstGame();
		Questions.QuestionManager.ClearAllQuestionComplited();
		UpdateRes();
	}

	public static void PlayProductTurn() {
		AudioManager.PlayEffect(Instance.productTurn, AudioMixerTypes.shopEffect);
	}

	#endregion

	#region Lanuage

	public Text shopTitle;
	public Text homeBtn;

	void InitLanuage() {
		LanguageManager.changeLanuage += ChangeLanuage;
	}

	void DeInitLanuage() {
		LanguageManager.changeLanuage -= ChangeLanuage;
	}

	void ChangeLanuage() {
		homeBtn.text = LanguageManager.GetTranslate("shop_Back");
	}

	#endregion

}
