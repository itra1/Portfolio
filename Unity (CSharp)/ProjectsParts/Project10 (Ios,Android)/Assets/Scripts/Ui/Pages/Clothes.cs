using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Shop.Products;

namespace Shop.View.Pages {
	public class Clothes : Page {
		
		public ShopController shop;
		public GameObject cameraObj;

		public ClothInformation fullInfoPanel;

		private List<RoundItem> shopProductList = new List<RoundItem>();

		public DisplayDiff displayDiff;

		public GameObject playerModel;

		public RectTransform parentModel;
		public RectTransform parentParentModel;
		
		[SerializeField] GameObject itemIconPref;
		public GameObject[] navButtons; // Кнопки навигации

		public GameObject buttonBye; // Кнопка Покупки

		public Color starDefColor;
		public Color starActiveColor;
		public Color setsBackDefColor;
		public Color setsBackActiveColor;

		public Text itemTitle;
		public Text itemDescription;
		public Text itemCoinsPrice;
		public RectTransform itemCoinsPriceParent;

		public Text groupTitle;
		public Text groupDescription;
		
		public Shop.Products.Product selectedProduct;
		private Shop.Products.ProductType activeType = ProductType.clothHead;

		int activePage;

		public AudioClip putOnClip;
		public AudioClip putOffClip;

		[System.Serializable]
		public struct ClothesIcons {
			public GameObject icon;
			public ShopElementType type;
		}

		public Image descriptionIcon;

		void Start() {
			SetPagesButton((int)ProductType.clothHead);
			CheckSet();
			displayDiff = CalcDisplayDiff();
		}

		void OnEnable() {
			displayDiff = CalcDisplayDiff();

			ShopController.SetShopPage += InitPages;
			ShopController.movedPanells += UpdatePosition;

			navButtons[0].GetComponent<Animation>().Play("active");
			navButtons[1].GetComponent<Animation>().Play( "deactive");
			navButtons[2].GetComponent<Animation>().Play( "deactive");
		}


		void OnDisable() {
			ShopController.movedPanells -= UpdatePosition;
			
			ShopController.SetShopPage -= InitPages;
		}

		void InitPages(shopTypes newPages) {
			playerModel.transform.localPosition = new Vector2(playerModel.transform.localPosition.x, (newPages == shopTypes.clothes ? 0 : -1500));

		}

		void UpdatePosition() {
			playerModel.transform.localPosition = new Vector2(playerModel.transform.localPosition.x,
				-Mathf.Lerp(0, 1500, Mathf.Abs((cameraObj.transform.position.x - transform.position.x) / (displayDiff.deltaX * 2))));
		}

		void ResetByed(bool force) {
			SelectProduct(selectedProduct);
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

		/// <summary>
		/// Переключение страницы магазина одежды
		/// </summary>
		/// <param name="pageNum">Номер страницы</param>
		public void SetPagesButton(int pageNum) {
			
			activeType = (ProductType)pageNum;

			SetListElements(activeType);
			activePage = pageNum;

			navButtons[0].GetComponent<Animation>().Play(activeType == ProductType.clothHead ? "active" : "deactive");
			navButtons[1].GetComponent<Animation>().Play(activeType == ProductType.clothSpine ? "active" : "deactive");
			navButtons[2].GetComponent<Animation>().Play(activeType == ProductType.clothAccessory ? "active" : "deactive");
			
			SelectProduct(shopProductList[0].Product);
			
		}

		/// <summary>
		/// Устанавливаем список элементов для магазина
		/// </summary>
		void SetListElements(ProductType activePosition) {

			shopProductList.ForEach(x => {
				Destroy(x.gameObject);
			});
			shopProductList.Clear();
			
			List<Shop.Products.Product> allElements = Shop.ShopManager.Instance.GetProductList(activePosition);

			int allLeight = allElements.Count * 165;

			parentItems.sizeDelta = new Vector2(allLeight, parentItems.sizeDelta.y);
			parentItems.anchoredPosition = new Vector2(allLeight / 2, 0);

			itemIconPref.gameObject.SetActive(false);

			for (int i = 0; i < allElements.Count; i++) {
				GameObject inst = Instantiate(itemIconPref);
				inst.SetActive(true);
				RectTransform instRect = inst.GetComponent<RectTransform>();
				instRect.SetParent(parentItems);
				instRect.anchoredPosition = new Vector2((90 + i * 165) - allLeight / 2, 0);
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

			int level = selectedProduct.GetLevel();

			if (level == 2) {
				buttonBye.GetComponent<Animator>().SetBool("bye", true);
				buttonBye.GetComponent<Animator>().SetBool("active", false);
			} else if (level == 1) {
				buttonBye.GetComponent<Animator>().SetBool("bye", true);
				buttonBye.GetComponent<Animator>().SetBool("active", true);
			}
			else {
				buttonBye.GetComponent<Animator>().SetBool("bye", false);
				buttonBye.GetComponent<Animator>().SetBool("active", true);

				try {
					if (selectedProduct.ByePossible())
						buttonBye.GetComponent<Animator>().SetBool("active", true);
					else
						buttonBye.GetComponent<Animator>().SetBool("active", false);
				} catch {
					Debug.Log("Ошибка " + selectedProduct.id);
				}

				itemCoinsPrice.text = selectedProduct.GetPrice().coins.ToString();
				itemCoinsPriceParent.sizeDelta = new Vector2(itemCoinsPrice.preferredWidth + 65, itemCoinsPriceParent.sizeDelta.y);

			}

		}

		public static void SorterObjects(ref ShopPrices[] arr) {
			ShopPrices temp;

			for (int i = 0; i < arr.Length; i++) {
				// Массив просматривается с конца до
				// позиции i и "легкие элементы всплывают"
				for (int j = arr.Length - 1; j > i; j--) {
					// Если соседние элементы расположены
					// в неправильном порядке, то меняем
					// их местами
					if (arr[j].levels[0].coins < arr[j - 1].levels[0].coins) {
						temp = arr[j];
						arr[j] = arr[j - 1];
						arr[j - 1] = temp;
					}
				}
			}
		}

		/*
		void PositingModel()
		{
				parentModel.anchoredPosition = new Vector2(-parentParentModel.rect.width / 4, -parentParentModel.rect.height / 100 * 39);
		}
		*/

		public void SetPagesButtonPress(int pageNum) {
			ShopController.PlayProductTurn();
			navButtons[pageNum].GetComponent<Animator>().SetTrigger("press");
		}

		public void SetPagesButtonUnPress(int pageNum) {
			navButtons[pageNum].GetComponent<Animator>().SetTrigger("normal");
		}

		public static Shop.Products.Product GetPrice(ShopElementType elementType) {
			return Shop.ShopManager.Instance.GetProduct(elementType.ToString());
		}

		public void SetItem(Shop.Products.ProductType product) {

			//if (product.type == Shop.Products.ProductType.none) {
			//	switch (activePage) {
			//		case 0:
			//			elementType = ShopElementType.headFeather;
			//			break;
			//		case 1:
			//			elementType = ShopElementType.spineShroud;
			//			break;
			//		case 2:
			//			elementType = ShopElementType.accessoryHook;
			//			break;
			//	}
			//}

			//activeType = elementType;

			//int elementLevel = PlayerPrefs.GetInt(elementType.ToString());
			//Shop.Products.Product price = Shop.ShopManager.Instance.GetProduct(elementType.ToString());

			//if (elementLevel == 2) {
			//	buttonBye.GetComponent<Animator>().SetBool("bye", true);
			//	buttonBye.GetComponent<Animator>().SetBool("active", false);
			//	elementLevel = 0;
			//} else if (elementLevel == 1) {
			//	buttonBye.GetComponent<Animator>().SetBool("bye", true);
			//	buttonBye.GetComponent<Animator>().SetBool("active", true);
			//	elementLevel = 0;
			//} else {
			//	buttonBye.GetComponent<Animator>().SetBool("bye", false);
			//	buttonBye.GetComponent<Animator>().SetBool("active", true);

			//	try {
			//		if (UserManager.coins >= (price as Shop.Products.Cloth).price.coins)
			//			buttonBye.GetComponent<Animator>().SetBool("active", true);
			//		else
			//			buttonBye.GetComponent<Animator>().SetBool("active", false);
			//	} catch {
			//		Debug.Log("Ошибка " + elementType.ToString());
			//	}

			//	itemCoinsPrice.text = (price as Shop.Products.Cloth).price.coins.ToString();
			//	itemCoinsPriceParent.sizeDelta = new Vector2(itemCoinsPrice.preferredWidth + 65, itemCoinsPriceParent.sizeDelta.y);
			//}
			//buttonBye.GetComponent<Animator>().SetTrigger("run");

			//switch (activePage) {
			//	case 0:
			//		selectedHead = elementType;
			//		break;
			//	case 1:
			//		selectedSpine = elementType;
			//		break;
			//	case 2:
			//		selectedAcsessuary = elementType;
			//		break;
			//	default:
			//		selectedHead = elementType;
			//		break;
			//}

			//itemTitle.text = LanguageManager.GetTranslate(price.title);
			//itemDescription.text = LanguageManager.GetTranslate(price.displayDescription);
			//itemTitle.fontSize = 47;

			//ChangeWight(itemTitle, itemCoinsPrice);

			//// Устанавливаем иконку и назначаем размеры
			//descriptionIcon.sprite = price.sprite;

			//if (descriptionIcon.sprite.rect.width > descriptionIcon.sprite.rect.height) {
			//	descriptionIcon.GetComponent<RectTransform>().sizeDelta = new Vector2(120, 120 / price.sprite.rect.width * price.sprite.rect.height);
			//} else {
			//	descriptionIcon.GetComponent<RectTransform>().sizeDelta = new Vector2(120 / price.sprite.rect.height * price.sprite.rect.width, 120);
			//}

			//if (ResetAllElements != null) ResetAllElements(activeType);
		}


		public void ButtonBye() {
			UiController.ClickButtonAudio();
			buttonBye.GetComponent<Animator>().SetTrigger("press");

			int allCoins = UserManager.coins;
			int elementLevel = selectedProduct.GetLevel();

			if (elementLevel == 1) {
				PickUpCloth(selectedProduct);
				return;
			}


			if (elementLevel == 2) {
				PickOffCloth(selectedProduct);
				return;
			}

			if (UserManager.coins < selectedProduct.GetPrice().coins) {
				//Invoke("byeButtonNoMoneyAnim", 1);
				return;
			}

			selectedProduct.Bye((prod, result) => {

				if(result)
					PickUpCloth(selectedProduct);

			});
			
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

		void PickUpCloth(Shop.Products.Product activeCloth) {
			AudioManager.PlayEffect(putOnClip, AudioMixerTypes.shopEffect);
			Config.SetBoundsCloth(activeCloth);
			//playerModel.GetComponent<PlayerModel>().skeletonRenderer.Reset();
			playerModel.GetComponent<Player.Jack.PlayerModel>().ResetSkin();

			CheckSet();
      Player.Jack.PlayerController.Instance.animation.ResetAnimation();
			//GameObject.Find("Player").GetComponent<PlayerController>().skeletonAnimation.GetComponent<SkeletonRenderer>().Reset();
			//SetItem(activeType);
		}

		void PickOffCloth(Shop.Products.Product activeCloth) {
			AudioManager.PlayEffect(putOffClip, AudioMixerTypes.shopEffect);
			Config.UnSetBoundsCloth(activeCloth);
			//playerModel.GetComponent<PlayerModel>().PickOffCloth(activeType);
			playerModel.GetComponent<Player.Jack.PlayerModel>().ResetSkin();
			CheckSet();
			//playerModel.GetComponent<PlayerModel>().skeletonRenderer.Reset();
			CheckSet();
      Player.Jack.PlayerController.Instance.animation.ResetAnimation();
			//GameObject.Find("Player").GetComponent<PlayerController>().skeletonAnimation.GetComponent<SkeletonRenderer>().Reset();
			//SetItem(activeType);
		}

		[System.Serializable]
		public struct ClothSetsIcons {
			public GameObject star;
			public GameObject rect;
		}

		public ClothSetsIcons[] clothSetIcon;
		public GameObject infoPanel;
		// Проверка на собранный комплект
		void CheckSet() {

			int maxCount;

			ClothesSet set = Config.GetGroup(out maxCount);
			ClothSetIconsChange(maxCount);
			if (set.title != null && set.title != "") {
				groupTitle.text = LanguageManager.GetTranslate(set.title);
				groupDescription.text = LanguageManager.GetTranslate(set.description);
			} else {
				groupTitle.text = LanguageManager.GetTranslate("shop_GroupTitleDefault");
				groupDescription.text = LanguageManager.GetTranslate("shop_GroupDescrDefault");
			}
		}

		void ClothSetIconsChange(int num) {
			infoPanel.GetComponent<Image>().color = (num == clothSetIcon.Length ? setsBackActiveColor : setsBackDefColor);
			infoPanel.transform.Find("Arrow").GetComponent<Image>().color = (num == clothSetIcon.Length
				? setsBackActiveColor
				: setsBackDefColor);


			for (int i = 0; i < clothSetIcon.Length; i++) {
				if (i <= num - 1) {
					clothSetIcon[i].star.SetActive(true);
					clothSetIcon[i].star.GetComponent<Image>().color = (num == clothSetIcon.Length ? starActiveColor : starDefColor);
					clothSetIcon[i].rect.SetActive(false);
				} else {
					clothSetIcon[i].star.SetActive(false);
					clothSetIcon[i].rect.SetActive(true);
				}
			}
		}

	}
}