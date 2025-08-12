using UnityEngine;
using UnityEngine.UI;
using System;
using ExEvent;

/// <summary>
/// Визуализация основного меню
/// </summary>
public class LevelMenu : PanelUi {

	public CompanyRoad companyRoad;

	public Action OnForward;
	public Action OnEnergy;
	public Action OnShop;
	public Action OnHome;

	public RectTransform coinsRect;
	public Animation anim;

  public GameObject heartPanel;

	private bool isActive;

	protected override void OnEnable() {
		base.OnEnable();
		isActive = true;
		ShowPanel();
		EnegryChange(Company.Live.LiveCompany.Instance.value);
    Company.Live.LiveCompany.OnChange += EnegryChange;
		ShopCountChange();
    heartPanel.SetActive(!Company.Live.LiveCompany.Instance.isUnlimited);
    //CoinsChange(new RunEvents.CoinsChange(UserManager.coins));
  }

	protected override void OnDisable() {
		base.OnDisable();
    Company.Live.LiveCompany.OnChange -= EnegryChange;
		if (OnClose != null)
			OnClose();
	}
  
  public void ShowAnimateComplete() {
    companyRoad.DrawRoad();
  }
	
	public void OnEnergyButton() {
		if(OnEnergy != null) OnEnergy();
	}

	public void OnShopButton() {
		if (!isActive) return;
		UiController.ClickButtonAudio();
		if (OnShop != null)		OnShop();
	}

	public void OnForwardButton() {
		if (!isActive) return;
		UiController.ClickButtonAudio();
		if (OnForward != null) OnForward();
	}

	public void OnHomeButton() {
		if (!isActive) return;
		UiController.ClickButtonAudio();
		if (OnHome != null) OnHome();
	}

	public Text energyValue;
	void EnegryChange(float energyValue) {
		if(energyValue == -1)
			this.energyValue.text = "-";
		else
			this.energyValue.text = energyValue.ToString();
	}

	//public Text coinsCount;

	//[ExEventHandler(typeof(RunEvents.CoinsChange))]
	//void CoinsChange(RunEvents.CoinsChange coin) {
	//	this.coinsCount.text = coin.coins.ToString();
	//	coinsRect.sizeDelta = new Vector2(this.coinsCount.preferredWidth + 80, coinsRect.sizeDelta.y);
	//	ShopCountChange();
	//}

	public Text shopCount;
	void ShopCountChange() {

		int allProducts =
			Config.GetFolowsCount((Shop.Products.ProductType.all));

		if (allProducts <= 0) {
			shopCount.transform.parent.gameObject.SetActive(false);
		} else {
			shopCount.transform.parent.gameObject.SetActive(true);
			shopCount.text = allProducts.ToString();
		}
		
	}
	
	void ShowPanel() {
		anim.Play("show");
	}

	public void HidePanel() {
		isActive = false;
		anim.Play("hide");
	}


	public void OnHidePanel() {
		gameObject.SetActive(false);
	}

	public override void BackButton() {
		OnHomeButton();
	}

  public void EnergyButton() {

    if (Company.Live.LiveCompany.Instance.value >= Company.Live.LiveCompany.Instance.maxValue) return;

    HeartSaleDialog dialog = UiController.ShowUi<HeartSaleDialog>();
    dialog.gameObject.SetActive(true);

  }

}
