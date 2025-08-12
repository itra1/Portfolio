using ExEvent;
using UnityEngine;
using UnityEngine.UI;

public class ShopProductUiItemBase: EventBehaviour {

  public TMPro.TextMeshProUGUI priceText;
  public ShopUi shop;
  public Button buyButton;

  public BillingProductAbstract product;

  public GameObject noAdsLabel;

  protected virtual void OnEnable() {
    ChangeNetStatus();
    PriceInit();
    buyButton.interactable = true;
    InitStatus();
  }

  private void ChangeNetStatus() {
  }

  [ExEvent.ExEventHandler(typeof(GameEvents.NetworkChange))]
  public void NetworkChange(GameEvents.NetworkChange net) {
    ChangeNetStatus();
  }


  [ExEvent.ExEventHandler(typeof(ExEvent.GameEvents.OnBillingInit))]
  public void OnBillingInit(ExEvent.GameEvents.OnBillingInit billing) {
    PriceInit();
  }

  [ExEvent.ExEventHandler(typeof(ExEvent.ShopEvents.OnProductBye))]
  public void OnProductBye(ExEvent.ShopEvents.OnProductBye eventData) {
    InitStatus();
  }

  protected virtual void InitStatus() {
    noAdsLabel.SetActive(!PlayerManager.Instance.noAds);
  }

  private void PriceInit() {

    if (!BillingManager.Instance.isInizialized) {
      priceText.text = "";
    } else {
      priceText.text = product.product.metadata.localizedPriceString;
    }
  }

  // Купить
  public void BuyButton() {
    AudioManager.Instance.library.PlayClickAudio();

    buyButton.interactable = false;
    shop.ByeProduct(product, BuyComplete);
  }

  public void BuyComplete() {
    buyButton.interactable = true;
  }

}
