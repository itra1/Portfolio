using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Shop.Products;
using Cloth = Shop.Products.Cloth;

public class ShopRoundItem : MonoBehaviour {
	
	private Shop.Products.Product _product;
	public Text coinsText;
	public RectTransform coinsTextParent;
	public Text boungText;

	[SerializeField]
	Image icon;

	public shopTypes shopType = shopTypes.clothes;

	public GameObject fillRound;
	public GameObject roundSprite;
	public GameObject PricePanel;
	public GameObject BoughtPanel;
	private int elementLevel;

	public Color roundActive;
	public Color roundByed;
	public Color roundBoung;
	public Color textByed;
	public Color textBoung;

	public System.Action<Product> OnClick;

	public void SetProduct(Shop.Products.Product product) {
		this._product = product;
	}


	void OnEnable() {
		BoughtPanel.SetActive(false);
		PricePanel.SetActive(false);
	}
	
	public void SetType(Shop.Products.Product product) {

		//if (isCloth)
		//	ClothesShop.ResetAllElements += UpdateElev;
		//if (isMounts)
		//	MountsShop.ResetAllElements += UpdateElev;
		
		//if (isCloth)
		//	SetCoinsCount(ClothesShop.instance.activeType.ToString() == elementType.id);
		//if (isMounts)
		//	SetCoinsCount(MountsShop.instance.activeType.ToString() == elementType.id);
	}


	void OnDisable() {
		//if (isCloth)
		//	ClothesShop.ResetAllElements -= UpdateElev;
		//if (isMounts)
		//	MountsShop.ResetAllElements -= UpdateElev;
	}

	void UpdateElev(ShopElementType type) {
		//SetCoinsCount(type.ToString() == product.id);
	}

	// Устанавливаем значение Счетчиков
	public void SetCoinsCount(bool active = false) {

		roundSprite.SetActive(active);

		//elementLevel = PlayerPrefs.GetInt(product.ToString());
		/*
		ShopPrices price = new ShopPrices();
		if (isCloth)
				price = ClothesShop.instance.GetPrice(elementType);
		if (isMounts)
				price = MountsShop.instance.GetPrice(elementType);
		*/
		//if (icon != null) {
		//	icon.sprite = product.sprite;

		//	if (product.sprite.rect.width > product.sprite.rect.height) {
		//		icon.GetComponent<RectTransform>().sizeDelta = new Vector2(120, 120 / price.sprite.rect.width * price.sprite.rect.height);
		//	} else {
		//		icon.GetComponent<RectTransform>().sizeDelta = new Vector2(120 / price.sprite.rect.height * price.sprite.rect.width, 120);
		//	}
		//}
		
		if (shopType == shopTypes.clothes) {
			if (elementLevel == 2) {
				BoughtPanel.SetActive(true);
				PricePanel.SetActive(false);
				boungText.color = textBoung;
				boungText.text = LanguageManager.GetTranslate("shop_Wearing"); // "Надето";
				fillRound.GetComponent<Image>().color = roundBoung;
			} else if (elementLevel == 1) {
				BoughtPanel.SetActive(true);
				PricePanel.SetActive(false);
				boungText.color = textByed;
				boungText.text = LanguageManager.GetTranslate("shop_Bought"); //"Куплено";
				fillRound.GetComponent<Image>().color = roundByed;
			} else if (elementLevel == 0) {
				BoughtPanel.SetActive(false);
				PricePanel.SetActive(true);
				fillRound.GetComponent<Image>().color = roundActive;
				//if (isCloth) {
				//	//coinsText.text = price.levels[elementLevel].coins.ToString();
				//	coinsTextParent.sizeDelta = new Vector2(coinsText.preferredWidth + 65, coinsTextParent.sizeDelta.y);
				//}
			}
		} else if (shopType == shopTypes.mounts) {
			BoughtPanel.SetActive(false);
			PricePanel.SetActive(true);
			fillRound.GetComponent<Image>().color = roundActive;

			//if (isMounts) {
				//if ((price as Shop.Products.Mount).priceLevel.Length > elementLevel) {
				//	coinsText.text = (price as Shop.Products.Mount).priceLevel[elementLevel].coins.ToString();
				//	coinsTextParent.sizeDelta = new Vector2(coinsText.preferredWidth + 65, coinsTextParent.sizeDelta.y);
				//} else {
				//	BoughtPanel.SetActive(true);
				//	PricePanel.SetActive(false);
				//	boungText.color = textByed;
				//	boungText.text = LanguageManager.GetTranslate("shop_Bought"); //"Куплено";
				//	fillRound.GetComponent<Image>().color = roundByed;
				//}
			//}
		}
	}

	// Событие нажатия кнопки
	public void ClickEvent() {

		//UiController.ClickButtonAudio();
		ShopController.PlayProductTurn();
		//if (isCloth)
		//	ClothesShop.instance.SetItem(elementType.type);
		//if (isMounts)
		//	MountsShop.instance.SetItem(elementType.type);
	}
}
