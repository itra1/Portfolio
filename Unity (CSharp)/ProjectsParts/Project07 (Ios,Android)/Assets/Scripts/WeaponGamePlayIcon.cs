using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponGamePlayIcon : MonoBehaviour, Tutorial.IFocusObject {

  [SerializeField]
  private Image icon;
  public GameObject activeBack;

  private bool isActive;
  [HideInInspector]
  public WeaponBehaviour weapon;

  private void OnEnable() {
    SetActiveWeapon(null);
  }

  public void SetWeapon(WeaponBehaviour weapon) {
    this.weapon = weapon;

    icon.sprite = weapon.WeaponIcon;
    icon.GetComponent<AspectRatioFitter>().aspectRatio = icon.sprite.rect.width / icon.sprite.rect.height;

    if (weapon.IsSelected)
      SetActiveWeapon(this.weapon);
  }

	public void Click() {
    WeaponsSpawner.Instance.SetActiveWeapon(weapon.uuid);
    if (TutorClick != null) TutorClick();
    TutorClick = null;
  }

  public void SetActiveWeapon(WeaponBehaviour newActiveWeapon) {
    isActive = newActiveWeapon != null && this.weapon.uuid == newActiveWeapon.uuid;
    activeBack.SetActive(isActive);
  }

  private System.Action TutorClick;
  public void Focus(bool isFocus, System.Action OnClick) {

    Canvas canv = GetComponent<Canvas>();

    if (isFocus) {
      TutorClick = OnClick;
      canv.overrideSorting = true;
      canv.sortingLayerName = "UI";
      canv.sortingOrder = 1000;
    } else {
      canv.overrideSorting = false;
    }
  }
}
