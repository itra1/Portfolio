using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Элементы магазина
/// </summary>
public class ShopItem : MonoBehaviour {

  public delegate void ButtonBye(ShopProduct product);
  public ButtonBye OnButtonBye;

  ShopProduct shopProduct;

  public Image icon;
  public Text titleText;
  public Text priceText;
  public Text bulletProductText;
  public Text bulletExistsText;

  void OnEnable() {
    Game.User.UserWeapon.OnChangeBulletCount += SetBulletCount;
  }

  void OnDisable() {
    Game.User.UserWeapon.OnChangeBulletCount -= SetBulletCount;
  }

  public void Init(ShopProduct newShopProduct) {
    shopProduct = newShopProduct;
    icon.sprite = shopProduct.sprite;
    titleText.text = shopProduct.name;
    priceText.text = shopProduct.price.ToString();
    bulletProductText.text = shopProduct.bullet.ToString();
    bulletExistsText.text = Game.User.UserWeapon.Instance.GetWeapon(newShopProduct.type).BulletCount.ToString();
  }

  void SetBulletCount(Game.Weapon.WeaponType weaponType, int bulletCount) {
    if(shopProduct.type == weaponType)
      bulletExistsText.text = bulletCount.ToString();
  }

  public void ByeButton() {
		if (OnButtonBye != null) OnButtonBye(shopProduct);
  }

}
