using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArsenalGroup : ExEvent.EventBehaviour {

  public I2.Loc.Localize title;
  public Image bullet;
  public Text bulletCount;
  public Text countBulletAdd;
  public Text priceBullet;
  public GameObject body;

  private BulletsProduct product;

  private WeaponGroup group;

  public void SetData(WeaponGroup group, BulletsProduct product) {
    this.group = group;
    this.product = product;
    title.Term = group.title;

    body.SetActive(group.wishBullets);
    if (!group.wishBullets)
      return;
    
    bulletCount.text = Utils.RoundValueString(group.bulletCount);
    bullet.sprite = group.bulletIcon;

    countBulletAdd.text = "+" + Utils.RoundValueString(product.countPatron);
    priceBullet.text = Utils.RoundValueString(product.softCashPrise);
  }

  public void ByeButton() {
    UiController.Instance.ClickSound();
    Shop.Instance.BuyProduct(this.product);
  }

  [ExEvent.ExEventHandler(typeof(ExEvent.BattleEvents.WeaponGroupChange))]
  private void GroupChange(ExEvent.BattleEvents.WeaponGroupChange eventData) {

    bulletCount.text = Utils.RoundValueString(group.bulletCount);
  }

}
