using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArsenalItem: ExEvent.EventBehaviour {

  public ArsenalDialog dialog;

  public I2.Loc.Localize title;
  public Image icon;
  public WeaponBehaviour weapon;

  public Text damageText;
  public Text reloadText;

  public GameObject bulletCountBlock;

  public Animation animWep;
  public Animation animBull;

  public Text existsBullets;

  public GameObject byeButton;
  public Image byeButtonBack;
  public I2.Loc.Localize byeButtonText;
  public Sprite active;
  public Sprite disactive;

  public void SetData(WeaponBehaviour weapon) {
    this.weapon = weapon;
    //this.product = product;
    title.Term = weapon.title;
    icon.sprite = weapon.WeaponIcon;
    damageText.text = weapon.Damage.ToString();
    reloadText.text = weapon.reloadDefault.ToString() + " c.";
    icon.GetComponent<AspectRatioFitter>().aspectRatio = icon.sprite.rect.width / icon.sprite.rect.height;

    UpdateStatus();

    if (weapon.isExpendable) {
      existsBullets.text = Utils.RoundValueString(weapon.BulletCount);
    }
    if(weapon.unlimBullets)
      existsBullets.text = "arsenal.noLimit";
    
  }

  public void UpdateStatus() {

    byeButtonBack.sprite = this.weapon.weaponData.IsEquipped ? disactive : active;

    if (this.weapon.weaponData.IsEquipped) {
      byeButtonText.Term = "arsenal.item.equipped";
    } else {
      byeButtonText.Term = "arsenal.item.equppedOn";
    }

  }
  
  [ExEvent.ExEventHandler(typeof(ExEvent.BattleEvents.WeaponChange))]
  private void WeaponChange(ExEvent.BattleEvents.WeaponChange eventData) {

    if (eventData.weapon == weapon) {
      //SetData(product, weapon);
      animWep.Play("bye");
      animBull.Play("bye");
    }

    UpdateStatus();

  }

  public void EquippedButton() {
    UiController.Instance.ClickSound();
    dialog.ClickItemWeapon(this);
  }

}
