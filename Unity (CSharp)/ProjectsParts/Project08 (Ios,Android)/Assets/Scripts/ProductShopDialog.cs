using ExEvent;
using UnityEngine;
using UnityEngine.UI;

public class ProductShopDialog : EventBehaviour {

	public LocalizationUiText descriptionText;
	public GameObject countStick;
	public Text countText;
	public TextUiScale priceText;
	public Image icon;
	public ShopUi shop;
	public Button byeButton;

	private BillingProductAbstract _product;

	public Animation productAnim;

	public GameObject deactiveBack;
	public GameObject deactiveLine;

	//private bool internetAvalable;

	public bool priceInit = false;

	private void OnEnable() {
		//internetAvalable = NetManager.Instance.internetAwalable;
		ChangeNetStatus();
		PriceInit();
		byeButton.interactable = true;
	}

	private void ChangeNetStatus() {
		//deactiveBack.SetActive(!internetAvalable);
		//deactiveLine.SetActive(!internetAvalable);
	}

	[ExEvent.ExEventHandler(typeof(GameEvents.NetworkChange))]
	public void NetworkChange(GameEvents.NetworkChange net) {
		//internetAvalable = net.isActive;
		ChangeNetStatus();
	}

	public void OnSelectPage() {
		productAnim.Play("focus");
	}

	[ExEvent.ExEventHandler(typeof(ExEvent.GameEvents.OnBillingInit))]
	public void OnBillingInit(ExEvent.GameEvents.OnBillingInit billing) {
		PriceInit();
	}

	private void PriceInit() {
		if (_product != null && _product.product != null && !priceInit) {

			if (!BillingManager.Instance.isInizialized) {
				priceInit = false;
				priceText.SetText("");
			} else {
				priceText.SetText(_product.product.metadata.localizedPriceString, LanguageManager.Instance.activeLanuage.code);
				priceInit = true;
			}
		}
	}

	public void SetData(int num, BillingProductAbstract product, Sprite spr) {

		_product = product;

		countText.text = product.count.ToString();
		PriceInit();

		//
		descriptionText.SetCode(product.description);
		if (product.count == 0)
			countStick.gameObject.SetActive(false);
		icon.sprite = spr;
		icon.rectTransform.sizeDelta = new Vector2(spr.rect.width, spr.rect.height);

		if (product.type == IapType.pack || product.type == IapType.locationAll) return;

		switch (num) {
			case 0:
				icon.transform.localScale = Vector3.one * 0.75f;
				break;
			case 2:
				icon.transform.localScale = Vector3.one * 1.25f;
				break;
			default:
				break;
		}
	}
	
	// Купить
	public void ByeButton() {
		//BillingManager.Instance.ByeProduct(_product);
		AudioManager.Instance.library.PlayClickAudio();

		//if (!internetAvalable) return;
		byeButton.interactable = false;
		shop.ByeProduct(_product, ByeComplete);
	}

	public void ByeComplete() {
		byeButton.interactable = true;
	}

}
