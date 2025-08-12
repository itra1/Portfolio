using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Окно результатов покупки
/// </summary>
public class SpecialSalesProduct : PanelUi {

	public Action OnUse;

	private bool isClose;

	//[SerializeField]
	//private int addCoins;
	//[SerializeField]
	//private int addClothes;
	//[SerializeField]
	//private int addGadget;
	//private int needAddCoins;
	
	public Sprite coinsIcon;

	[SerializeField]
	private Text coinsCountProductText;
	[SerializeField]
	private Image[] clothesPosition;
	[SerializeField]
	private Image[] gadgetPosition;
	[SerializeField]
	private Text[] gadgetPositionCount;

	//private List<ShopElementType> getClothes;
	//private Dictionary<ShopElementType, int> getGadget;

	public AudioClip openProductClip;

	protected override void OnEnable() {
		base.OnEnable();
		isClose = false;
	}

	//public void ByeSpecial() {
	//	getClothes = new List<ShopElementType>();
	//	// Инкремент бабла
	//	int allCoins = UserManager.coins;
	//	needAddCoins = addCoins;

	//	// Получаем лист доступных шпотом
	//	ShopElementType[] productClothes = Config.GetProdyctArrayByType(shopTypes.clothes);

	//	List<ShopElementType> clothesList = new List<ShopElementType>();

	//	foreach (ShopElementType one in productClothes) {
	//		if (PlayerPrefs.GetInt(one.ToString(), 0) == 0)
	//			clothesList.Add(one);
	//	}

	//	// Если доступных шпотом не хватает, остаток выражаем в золоте
	//	if (clothesList.Count < addClothes) {
	//		needAddCoins += (addClothes - clothesList.Count) * 10000;
	//		addClothes = clothesList.Count;
	//	}

	//	if (addClothes > 0) {
	//		for (int i = 0; i < addClothes; i++) {
	//			ShopElementType need = clothesList[UnityEngine.Random.Range(0, clothesList.Count)];
	//			getClothes.Add(need);
	//			clothesList.Remove(need);
	//		}
	//	}

	//	// Сохраняем полученный список
	//	foreach (ShopElementType one in getClothes) {
	//		PlayerPrefs.SetInt(one.ToString(), 1);
	//	}

	//	// 5 случайных гаджетов

	//	getGadget = new Dictionary<ShopElementType, int>();
	//	ShopElementType[] productPowers = Config.GetProdyctArrayByType(shopTypes.powers);

	//	for (int i = 0; i < addGadget; i++) {
	//		ShopElementType need = productPowers[UnityEngine.Random.Range(0, productPowers.Length)];
	//		if (getGadget.Count < 3) {
	//			while (getGadget.ContainsKey(need))
	//				need = productPowers[UnityEngine.Random.Range(0, productPowers.Length)];
	//		} else {
	//			while (!getGadget.ContainsKey(need))
	//				need = productPowers[UnityEngine.Random.Range(0, productPowers.Length)];
	//		}

	//		if (getGadget.ContainsKey(need))
	//			getGadget[need] = getGadget[need] + 1;
	//		else
	//			getGadget.Add(need, 1);

	//	}

	//	// Сохраняем полученный список
	//	foreach (ShopElementType one in getGadget.Keys) {
	//		int ctn = PlayerPrefs.GetInt(one.ToString(), 0);
	//		PlayerPrefs.SetInt(one.ToString(), ctn + 1);
	//	}

	//	UserManager.coins += needAddCoins;

	//	int clothesNum = 0;
	//	foreach (ShopElementType oneElem in getClothes) {
	//		ShopPrices shopPrice = Config.GetElementPrices(oneElem);

	//		clothesPosition[clothesNum].sprite = shopPrice.sprite;
	//		if (shopPrice.sprite.rect.width > shopPrice.sprite.rect.height) {
	//			clothesPosition[clothesNum].rectTransform.sizeDelta = new Vector2(110, 110 / shopPrice.sprite.rect.width * shopPrice.sprite.rect.height);
	//		} else {
	//			clothesPosition[clothesNum].rectTransform.sizeDelta = new Vector2(110 / shopPrice.sprite.rect.height * shopPrice.sprite.rect.width, 110);
	//		}

	//		clothesNum++;

	//	}

	//	if (clothesNum < addClothes) {
	//		for (int i = clothesNum; i < addClothes; i++) {
	//			clothesPosition[i].sprite = coinsIcon;
	//			if (coinsIcon.rect.width > coinsIcon.rect.height) {
	//				clothesPosition[i].rectTransform.sizeDelta = new Vector2(110, 110 / coinsIcon.rect.width * coinsIcon.rect.height);
	//			} else {
	//				clothesPosition[i].rectTransform.sizeDelta = new Vector2(110 / coinsIcon.rect.height * coinsIcon.rect.width, 110);
	//			}
	//		}
	//	}

	//	int gadgetNum = 0;

	//	foreach (ShopElementType oneElem in getGadget.Keys) {
	//		ShopPrices shopPrice = Config.GetElementPrices(oneElem);

	//		gadgetPosition[gadgetNum].sprite = shopPrice.sprite;
	//		if (shopPrice.sprite.rect.width > shopPrice.sprite.rect.height) {
	//			gadgetPosition[gadgetNum].rectTransform.sizeDelta = new Vector2(110, 110 / shopPrice.sprite.rect.width * shopPrice.sprite.rect.height);
	//		} else {
	//			gadgetPosition[gadgetNum].rectTransform.sizeDelta = new Vector2(110 / shopPrice.sprite.rect.height * shopPrice.sprite.rect.width, 110);
	//		}

	//		if (getGadget[oneElem] > 1) {
	//			gadgetPositionCount[gadgetNum].text = getGadget[oneElem].ToString();
	//			gadgetPositionCount[gadgetNum].transform.parent.gameObject.SetActive(true);
	//		} else {
	//			gadgetPositionCount[gadgetNum].transform.parent.gameObject.SetActive(false);
	//		}

	//		gadgetNum++;

	//	}

	//	AudioManager.PlayEffects(openProductClip, AudioMixerTypes.shopEffect);
	//	coinsCountProductText.text = needAddCoins.ToString();

	//}

	public void ShowByeSpecial(Assets.Scripts.Billing.SpecialBillingProduct product) {

		UserManager.coins += product.goldAdded;

		int clothesNum = 0;
		foreach (ShopElementType oneElem in product.clothestAdded) {
			Shop.Products.Product shopPrice = Shop.ShopManager.Instance.GetProduct(oneElem.ToString());

			clothesPosition[clothesNum].sprite = shopPrice.sprite;
			if (shopPrice.sprite.rect.width > shopPrice.sprite.rect.height) {
				clothesPosition[clothesNum].rectTransform.sizeDelta = new Vector2(110, 110 / shopPrice.sprite.rect.width * shopPrice.sprite.rect.height);
			} else {
				clothesPosition[clothesNum].rectTransform.sizeDelta = new Vector2(110 / shopPrice.sprite.rect.height * shopPrice.sprite.rect.width, 110);
			}

			clothesNum++;

		}

		if (clothesNum < product.clothCount) {
			for (int i = clothesNum; i < product.clothCount; i++) {
				clothesPosition[i].sprite = coinsIcon;
				if (coinsIcon.rect.width > coinsIcon.rect.height) {
					clothesPosition[i].rectTransform.sizeDelta = new Vector2(110, 110 / coinsIcon.rect.width * coinsIcon.rect.height);
				} else {
					clothesPosition[i].rectTransform.sizeDelta = new Vector2(110 / coinsIcon.rect.height * coinsIcon.rect.width, 110);
				}
			}
		}

		int gadgetNum = 0;

		foreach (ShopElementType oneElem in product.gadgetAdded.Keys) {
			Shop.Products.Product shopPrice = Shop.ShopManager.Instance.GetProduct(oneElem.ToString());

			gadgetPosition[gadgetNum].sprite = shopPrice.sprite;
			if (shopPrice.sprite.rect.width > shopPrice.sprite.rect.height) {
				gadgetPosition[gadgetNum].rectTransform.sizeDelta = new Vector2(110, 110 / shopPrice.sprite.rect.width * shopPrice.sprite.rect.height);
			} else {
				gadgetPosition[gadgetNum].rectTransform.sizeDelta = new Vector2(110 / shopPrice.sprite.rect.height * shopPrice.sprite.rect.width, 110);
			}

			if (product.gadgetAdded[oneElem] > 1) {
				gadgetPositionCount[gadgetNum].text = product.gadgetAdded[oneElem].ToString();
				gadgetPositionCount[gadgetNum].transform.parent.gameObject.SetActive(true);
			} else {
				gadgetPositionCount[gadgetNum].transform.parent.gameObject.SetActive(false);
			}

			gadgetNum++;

		}

		AudioManager.PlayEffect(openProductClip, AudioMixerTypes.shopEffect);
		coinsCountProductText.text = product.goldAdded.ToString();

	}

	protected override void OnDisable() {
		base.OnDisable();
		isClose = false;
		if (OnClose != null) ClosePanel();

	}

	public void GetPgoduct() {
		if (OnUse != null) OnUse();
		Close();
	}

	public void Close() {
		isClose = true;
		UiController.ClickButtonAudio();
		UiController.ClickButtonAudio();
		GetComponent<Animator>().SetTrigger("close");
	}

	public void ProductButtonClosedNod() {
		gameObject.SetActive(false);
	}

	public void ClosePanel() {
		if (!isClose) return;
		isClose = false;
		gameObject.SetActive(false);
	}

	public override void BackButton() {
		Close();
	}
}
