using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShopPage: MonoBehaviour {

  public Action<ShopPage> OnHeader;
  public Action<ShopProductBehaviour> OnBye;

  public Game.Weapon.WeaponCategory category;
  public RectTransform rectContent;
  public ShopElement shopElement;
  public ShopElement bigView;

  public GameObject activeBack;
  public GameObject deactiveBack;

  private ShopLibrary _shopLibrary;
  private List<ShopElement> shopProducts = new List<ShopElement>();

  public void Init(ShopLibrary shopLibrary, Action<ShopPage> OnSelect, Action<ShopProductBehaviour> OnBye) {
    OnHeader = OnSelect;
    _shopLibrary = shopLibrary;
    this.OnBye = OnBye;
    ShopProduct();
  }

  private void ShopProduct() {
    shopElement.gameObject.SetActive(false);
    if (shopProducts.Count != 0) return;

    List<ShopProductBehaviour> shopProductList = _shopLibrary.shopLibrary.FindAll(x => x is WeaponShopProduct
                                                                                    && Game.User.UserWeapon.Instance.weaponsManagers.Exists(w => w.category == category
                                                                                    && (x as WeaponShopProduct).weaponType == w.weaponType)
                                                                                  ).OrderBy(x => (x as WeaponShopProduct).weaponType).ThenBy(x => x.count).ToList();

    if (shopProductList.Count == 0) {
      bigView.gameObject.SetActive(false);
      return;
    }

    for (int i = 0; i < shopProductList.Count; i++) {
      GameObject instelem = Instantiate(shopElement.gameObject);
      instelem.transform.SetParent(shopElement.transform.parent);
      instelem.transform.localScale = shopElement.transform.localScale;
      instelem.GetComponent<RectTransform>().anchoredPosition = new Vector2(50 + 100 * (i % 3), -(60 + 120 * (i / 3)));
      instelem.SetActive(true);
      instelem.GetComponent<ShopElement>().SetProduct(shopProductList[i]);
      instelem.GetComponent<ShopElement>().OnBye = ByeProduct;
      instelem.GetComponent<ShopElement>().OnClick = SetProduct;
      shopProducts.Add(instelem.GetComponent<ShopElement>());
    }

    rectContent.sizeDelta = new Vector2(rectContent.sizeDelta.x, (120 * (shopProductList.Count / 3)));

    SetProduct(shopProductList[0]);
  }

  private void SetProduct(ShopProductBehaviour elem) {
    bigView.SetProduct(elem);
  }

  private void ByeProduct(ShopProductBehaviour elem) {
    if (OnBye != null) OnBye(elem);
  }


  public void SetFocus(bool isFocus) {
    activeBack.SetActive(isFocus);
    deactiveBack.SetActive(!isFocus);
  }

  public void ClickHeader() {
    UIController.ClickPlay();
    if (OnHeader != null) OnHeader(this);
  }

}
