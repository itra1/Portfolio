using UnityEngine;
using System.Collections;
using Game.Weapon;

public class WeaponBuyPanel : UiDialog {

  WeaponManager weaponManager;
  bool isSpecial;

  public void SetWeaponManager(WeaponManager newWeaponManager, bool special = false) {
    weaponManager = newWeaponManager;
    isSpecial = special;
  }

  public void ButtonOk() {
    WeaponGenerator.Instance.BuyWeaponConfirm(weaponManager , isSpecial);
    ButtonCancel();
  }

  public void ButtonCancel() {
    WeaponGenerator.Instance.BuyWeaponCancel();
    gameObject.SetActive(false);
  }


}
