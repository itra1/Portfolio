using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Shop.Products;
using Pet = Shop.Products.Mount;

namespace Shop.View.Pages {
	public class Mounts : Page {

		public GameObject cameraObj;

		public DisplayDiff displayDiff;

		public MountInformation fullInfoPanel;
		private List<RoundItem> shopProductList = new List<RoundItem>();
		[SerializeField] GameObject itemIconPref;

		public RectTransform parentModel;
		public RectTransform parentParentModel;

		private int page = 0;

		public GameObject buttonBye; // Кнопка Покупки

		[SerializeField]
		private GameObject dinoPet;
		[SerializeField]
		private GameObject spiderPet;
		[SerializeField]
		private GameObject batPet;

		public GameObject mountParent;

		public Text itemTitle;
		public Text itemDescription;
		public Text itemCoinsPrice;
		public Text itemLevel;
		public RectTransform itemCoinsPriceParent;

		public Text infoTime;
		public Text infoTimeFull;
		public Text infoUpgrades;

		public Shop.Products.Product selectedProduct;

		[HideInInspector] ShopElementType selectedElement = ShopElementType.penDino;

		public delegate void resetAll(ShopElementType activeType); // Делегат проверки сумм
		public static event resetAll ResetAllElements; // Событие проверки сумм

		[HideInInspector] public ShopElementType activeType;

		void Start() {
			displayDiff = CalcDisplayDiff();
		}

		void OnEnable() {
			displayDiff = CalcDisplayDiff();


			ShopController.allReset += this.resetByed; // Событие сброса
			ShopController.CheckAllSummEvent += this.resetByed; // Цыпляем событие проверки балансов
			ShopController.movedPanells += UpdatePosition;
			ShopController.SetShopPage += InitPages;

			SetListElements();
			SelectProduct(Shop.ShopManager.Instance.GetProductList(ProductType.mount)[0]);
			//SetItem(selectedElement);

			//SetPet(PlayerPetsTypes.dino);
			//PositingModel();
			page = 0;
			ConfirmPage();
		}

		void OnDisable() {
			ShopController.movedPanells -= UpdatePosition;
			ShopController.allReset -= resetByed; // Событие сброса
			ShopController.CheckAllSummEvent -= resetByed; // Цыпляем событие проверки балансов
			ShopController.SetShopPage -= InitPages;
		}

		/// <summary>
		/// Устанавливаем список элементов для магазина
		/// </summary>
		void SetListElements() {

			shopProductList.ForEach(x => {
				Destroy(x.gameObject);
			});
			shopProductList.Clear();

			List<Shop.Products.Product> allElements = Shop.ShopManager.Instance.GetProductList(ProductType.mount);

			int allLeight = allElements.Count * 165;

			parentItems.sizeDelta = new Vector2(allLeight, parentItems.sizeDelta.y);
			parentItems.anchoredPosition = new Vector2(allLeight / 2, 0);

			itemIconPref.gameObject.SetActive(false);

			for (int i = 0; i < allElements.Count; i++) {
				GameObject inst = Instantiate(itemIconPref);
				inst.SetActive(true);
				RectTransform instRect = inst.GetComponent<RectTransform>();
				instRect.SetParent(parentItems);
				instRect.anchoredPosition = new Vector2((120 + i * 170) - allLeight / 2, 0);
				RoundItem elem = inst.GetComponent<RoundItem>();
				elem.SetProduct(allElements[i]);
				shopProductList.Add(elem);

				elem.OnClick = SelectProduct;

			}
		}

		private void SelectProduct(Shop.Products.Product product) {

			fullInfoPanel.SetProduct(product);

			shopProductList.ForEach(x => x.SetSelect(product));

			selectedProduct = product;

			dinoPet.SetActive(selectedProduct.id == "petDino");
			spiderPet.SetActive(selectedProduct.id == "petSpider");
			batPet.SetActive(selectedProduct.id == "petBat");
			
			int level = selectedProduct.GetLevel();

			if (selectedProduct.IsBye()) {
				buttonBye.GetComponent<Animator>().SetBool("bye", true);
				buttonBye.GetComponent<Animator>().SetBool("active", false);
				return;
			}

			buttonBye.GetComponent<Animator>().SetBool("bye", false);
			buttonBye.GetComponent<Animator>().SetBool("active", true);

			itemCoinsPrice.text = selectedProduct.GetPrice().coins.ToString();
			itemCoinsPriceParent.sizeDelta = new Vector2(itemCoinsPrice.preferredWidth + 65, itemCoinsPriceParent.sizeDelta.y);

			try {
				if (UserManager.coins >= selectedProduct.GetPrice().coins)
					buttonBye.GetComponent<Animator>().SetBool("active", true);
				else
					buttonBye.GetComponent<Animator>().SetBool("active", false);
			} catch {
				Debug.Log("Ошибка " + selectedProduct.id);
			}


		}

		public void LeftButton() {
			page--;
			if (page < 0)
				page = shopProductList.Count-1;
			ConfirmPage();
		}

		public void RightButton() {
			page++;
			if (page > shopProductList.Count-1)
				page = 0;
			ConfirmPage();
		}

		private void ConfirmPage() {
			SelectProduct(shopProductList[page].Product);
		}

		public DisplayDiff CalcDisplayDiff(float positionZ = 0) {

			DisplayDiff display = new DisplayDiff();

			float posX = Display.main.systemWidth;
			float posY = Display.main.systemHeight;
			float posZ = positionZ - transform.position.z;

			Vector3 ld = new Vector3(0, 0, posZ);
			Vector3 rt = new Vector3(posX, posY, posZ);

			Vector3 screen1 = cameraObj.GetComponent<Camera>().ScreenToWorldPoint(ld);
			Vector3 screen2 = cameraObj.GetComponent<Camera>().ScreenToWorldPoint(rt);

			display.left = screen1.x - transform.position.x;
			display.right = screen2.x - transform.position.x;
			display.down = screen1.y - transform.position.y;
			display.top = screen2.y - transform.position.y;
			display.deltaX = display.right - display.left;
			display.transform = transform;

			return display;
		}

		void resetByed(bool force) {
			if (ResetAllElements != null) ResetAllElements(activeType);
			//SetItem(activeType);
		}


		void InitPages(shopTypes newPages) {
			if (newPages == shopTypes.mounts)
				mountParent.transform.localPosition = new Vector2(mountParent.transform.localPosition.x, 0);
			else
				mountParent.transform.localPosition = new Vector2(mountParent.transform.localPosition.x, -1500);

		}

		void UpdatePosition() {
			//Debug.Log(Mathf.Abs(( cameraObj.transform.position.x - transform.position.x ) / ( displayDiff.right * 4 )));
			mountParent.transform.localPosition = new Vector2(mountParent.transform.localPosition.x,
				-Mathf.Lerp(0, 1500, Mathf.Abs((cameraObj.transform.position.x - transform.position.x) / (displayDiff.deltaX * 2))));
		}


		public void SetItem(Shop.Products.ProductType elementType) {

			//switch (elementType) {
			//	case ShopElementType.penBat:
			//		SetPet(PlayerPetsTypes.bat);
			//		break;
			//	case ShopElementType.penDino:
			//		SetPet(PlayerPetsTypes.dino);
			//		break;
			//	case ShopElementType.penSpider:
			//		SetPet(PlayerPetsTypes.spider);
			//		break;
			//}

			//activeType = elementType;

			Product product = Shop.ShopManager.Instance.GetProduct(elementType.ToString());
			int elementLevel = product.GetLevel();
			Shop.Products.Mount price = product as Shop.Products.Mount;

			infoTime.text = (10 + elementLevel * 3).ToString() + " " + LanguageManager.GetTranslate("shop_Sec");
			infoTimeFull.text = (10 + elementLevel * 3).ToString() + " " + LanguageManager.GetTranslate("shop_Sec");
			itemLevel.text = LanguageManager.GetTranslate("shop_Level") + " " + (elementLevel + 1).ToString();

			if (price.priceLevel.Length == elementLevel) {
				infoUpgrades.gameObject.SetActive(false);
				buttonBye.GetComponent<Animator>().SetBool("maximum", true);
				infoTimeFull.gameObject.SetActive(true);
				infoTime.gameObject.SetActive(false);
			} else {
				buttonBye.GetComponent<Animator>().SetBool("maximum", false);
				infoUpgrades.gameObject.SetActive(true);
				infoTime.gameObject.SetActive(true);
				infoTimeFull.gameObject.SetActive(false);
				buttonBye.GetComponent<Animator>().SetBool("bye", false);

				if (UserManager.coins >= price.priceLevel[elementLevel].coins)
					buttonBye.GetComponent<Animator>().SetBool("active", true);
				else
					buttonBye.GetComponent<Animator>().SetBool("active", false);

				itemCoinsPrice.text = price.priceLevel[elementLevel].coins.ToString();
				itemCoinsPriceParent.sizeDelta = new Vector2(itemCoinsPrice.preferredWidth + 65, itemCoinsPriceParent.sizeDelta.y);
			}
			if (buttonBye != null) buttonBye.GetComponent<Animator>().SetTrigger("run");
			//selectedElement = elementType;
			itemTitle.text = LanguageManager.GetTranslate(price.displayTitle);
			itemDescription.text = LanguageManager.GetTranslate(price.displayDescription);
			itemTitle.fontSize = 47;

			ChangeWight(itemTitle, itemCoinsPrice);

			if (ResetAllElements != null) ResetAllElements(activeType);
		}

		/*
			void PositingModel()
			{
					parentModel.anchoredPosition = new Vector2(-parentParentModel.rect.width/4, -parentParentModel.rect.height/100 * 39);
			}
			*/

		
		public void ButtonBye() {
			UiController.ClickButtonAudio();
			buttonBye.GetComponent<Animator>().SetTrigger("press");

			int allCoins = UserManager.coins;
			int elementLevel = PlayerPrefs.GetInt(activeType.ToString());

			Shop.Products.Product product = Shop.ShopManager.Instance.GetProduct(activeType.ToString());
			//Shop.Products.Mount price = product.GetPrice();

			if (!product.ByePossible()) return;

			//if (allCoins < price.priceLevel[elementLevel].coins) {
			//	//Invoke("byeButtonNoMoneyAnim", 1);
			//	return;
			//}

			product.Bye();
		}

		void ChangeWight(Text txt, Text txt2) {
			while (!Checkscale(txt, txt2)) {
				txt.fontSize -= 1;
			}
		}

		bool Checkscale(Text txt, Text txt2) {
			float textWidth = txt.preferredWidth;
			float textWidth2 = txt2.preferredWidth;
			float parentWidth = txt.rectTransform.parent.GetComponent<RectTransform>().rect.width;

			if (textWidth + textWidth2 + 250 > parentWidth)
				return false;
			else
				return true;
		}

	}
}