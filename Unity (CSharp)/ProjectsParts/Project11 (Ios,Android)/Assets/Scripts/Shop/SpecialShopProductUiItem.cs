public class SpecialShopProductUiItem: ShopProductUiItemBase {

  public Timer timerText;

  protected override void OnEnable() {

    if (!Shop.Instance.isSpecialProduct) {
      gameObject.SetActive(false);
      return;
    }

    base.OnEnable();

    timerText.StartTimer(Shop.Instance.startTime.AddDays(1) - System.DateTime.Now);

  }

  protected override void InitStatus() {
    base.InitStatus();
  }

}
