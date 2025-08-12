using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopProductUi : MonoBehaviour {

  public ProductBase product;
  public I2.Loc.Localize titleText;
  public Image icon;
  public Text priceText;

  protected virtual void OnEnable() {
    titleText.Term = product.title;
    if (priceText != null) {
      icon.sprite = product.icon;
      icon.GetComponent<AspectRatioFitter>().aspectRatio = product.icon.rect.width / product.icon.rect.height;
    }
    if(priceText != null)
      priceText.text = Utils.RoundValueString(product.hardCashPrise);
  }

  protected bool Buy()
  {
    if (!product.CheckCash())
    {

      NoHardCacheDialog dialog = UiController.GetUi<NoHardCacheDialog>();
      dialog.Show();
      return false;
    }

    Shop.Instance.BuyProduct(product);
    return true;
  }

  public virtual void BuyButton()
  {
    Buy();
  }


  public void ChangeVisible() {

  }

}
