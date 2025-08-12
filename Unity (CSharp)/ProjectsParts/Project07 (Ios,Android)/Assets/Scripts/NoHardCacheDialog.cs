using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoHardCacheDialog : UiDialog {

  public HardCacheBillingProduct productBilling;

  public Text priceText;
  public Text titleText;


  public void CloseButton() {
    Hide();

  }
  protected override void OnEnable() {
    base.OnEnable();

    titleText.text = productBilling.title;
    priceText.text = productBilling.product.metadata.localizedPriceString;

  }

  public void ByeButton() {

    BillingManager.Instance.ByeProduct(productBilling, () => {

      Hide();

    });

  }

}
