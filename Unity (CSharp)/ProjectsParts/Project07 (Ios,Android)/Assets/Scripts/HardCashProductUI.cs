using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HardCashProductUI : ShopProductUi {

  public HardCacheBillingProduct productBilling;

  protected override void OnEnable() {

    titleText.Term = productBilling.title;
    try {
      priceText.text = productBilling.product.metadata.localizedPriceString;
    } catch { }
  }

  public override void BuyButton() {
    UiController.Instance.ClickSound();
    BillingManager.Instance.ByeProduct(productBilling);

  }
  
}
