using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShopItemCotnroller : ExEvent.EventBehaviour {

	public ShopController shop;
	private Shop.Products.Product product;                                    // Описание элемента

	public Text title;                                          // Заголовок
	public Text descriptionPanel;                               // Описание

	public Text coinsCountText;                                 // Текст стоимости элемента в монетах

	bool itemStatus;                                            // Текущее состояние элемента

	bool readyBye;

	bool addEvent = false;
	Animator thisAnim;
	float timeWaitReset;

	public GameObject coundCount;
	public Text countText;
	public Text levelText;
	public Image icone;

	public void SetProduct(Shop.Products.Product product) {

		thisAnim = GetComponent<Animator>();
		this.product = product;

		title.text = LanguageManager.GetTranslate(product.displayTitle);
		descriptionPanel.text = LanguageManager.GetTranslate(product.displayDescription);

		if (icone != null && product.sprite != null) {
			icone.sprite = product.sprite;
			icone.GetComponent<AspectRatioFitter>().aspectRatio = product.sprite.rect.width / product.sprite.rect.height;
		}
		
		CheckSumm(true);
	}

	[ExEvent.ExEventHandler(typeof(ExEvent.RunEvents.CoinsChange))]
	public void CoinsUpdate(ExEvent.RunEvents.CoinsChange eventData) {
		CheckSumm(true);
	}

	void OnEnable() {

		if (!addEvent) {
			//ShopController.CheckAllSummEvent += this.CheckSumm;     // Цыпляем событие проверки балансов
			ShopController.allReset += this.resetByed;              // Событие сброса
			addEvent = true;
		}
		
	}
	
	void OnDisable() {

		if (addEvent) {
			//ShopController.CheckAllSummEvent -= this.CheckSumm;     // Отцепляемся от события
			ShopController.allReset -= this.resetByed;
			addEvent = false;
		}
	}

	void CheckSumm(bool force = false) {
		if(product == null) return;
		int level = product.GetLevel();

		if (levelText != null)
			levelText.text = LanguageManager.GetTranslate("shop_Level") + " " + level.ToString();

		if (product.IsBye()) {
			thisAnim.SetBool("full", true);
			return;
		}

		Shop.Products.Price price = product.GetPrice();

		//if (product.unLimLevels) {
		//	if (coundCount != null) {
		//		coundCount.SetActive(true);
		//		countText.text = level.ToString();
		//	}
		//	level = 0;

		//} else {


		//	// Если все купленно, то и не паримся
		//	if (product.IsBye()) {
		//		thisAnim.SetBool("full", true);
		//		return;
		//	} else {
		//		thisAnim.SetBool("full", false);
		//	}
		//}

		coinsCountText.text = price.coins.ToString();

		if (product.ByePossible()) {
			thisAnim.SetBool("active", true);
		} else {
			thisAnim.SetBool("active", false);
		}

	}

	/// <summary>
	/// Покупка элемента
	/// </summary>
	public void ByeElement() {
		
		int level = product.GetLevel();
		
		if (!product.ByePossible()) return;
		
		if (!readyBye) {
			GetComponent<Animator>().SetBool("confirm", true);
			timeWaitReset = Time.time + 0.3f;
			shop.BottomAudio();
			readyBye = true;
		} else {
			timeWaitReset = Time.time + 0.5f;
			if (product.GetPrice().coins <= UserManager.coins) {
				GetComponent<Animator>().SetBool("bye", true);
				product.Bye();

				if (coundCount != null && coundCount.activeInHierarchy)
					coundCount.GetComponent<Animator>().SetTrigger("anim");
			}
		}
	}

	public void EventReset() {

		thisAnim.SetBool("confirm", false);
		thisAnim.SetBool("bye", false);
		readyBye = false;
	}

	public void resetByed(bool force) {
		if (timeWaitReset > Time.time) return;

		if (readyBye) {
			readyBye = false;
			EventReset();
		}

	}

	

}
