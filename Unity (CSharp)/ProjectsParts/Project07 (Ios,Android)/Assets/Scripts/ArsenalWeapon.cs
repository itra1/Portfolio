using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArsenalWeapon : MonoBehaviour {

  public ArsenalDialog dialog;
  public Image image;
  private WeaponBehaviour _weapon;

  public Sprite sprite;

  public void SetWeapon(WeaponBehaviour weapon) {
    this._weapon = weapon;

    if(this._weapon == null) {
      image.sprite = sprite;
    } else {
      image.sprite = weapon.WeaponIcon;
    }

    image.GetComponent<AspectRatioFitter>().aspectRatio = image.sprite.rect.width / image.sprite.rect.height;
  }

	public void Click() {
    dialog.ClickWeaponList(this._weapon);
  }

}
