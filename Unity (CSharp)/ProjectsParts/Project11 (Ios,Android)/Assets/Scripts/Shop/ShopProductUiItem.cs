using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopProductUiItem : ShopProductUiItemBase {

  public GameObject bonusTextObject;
  public GameObject coinsOld;
  public Text coinsCount;
  public Text oldCoinsCount;

  protected override void InitStatus() {
    base.InitStatus();

    bonusTextObject.SetActive(Shop.Instance.is30Percent);
    coinsOld.SetActive(Shop.Instance.is30Percent);
    oldCoinsCount.text = product.count.ToString();
    if (Shop.Instance.is30Percent) {
      coinsCount.text = ((int)(product.count * Shop.Instance.countCoeff)).ToString();
    } else {
      coinsCount.text = product.count.ToString();
    }
  }

}
