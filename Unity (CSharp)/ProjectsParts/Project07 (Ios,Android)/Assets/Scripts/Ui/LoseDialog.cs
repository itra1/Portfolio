using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoseDialog : UiDialog {

  public System.Action OnBase;
  public System.Action OnRepeat;
  public System.Action OnVideoAds;
  public Image iconSprite;

  public GameObject adsButton;

  protected override void OnEnable() {
    base.OnEnable();

    iconSprite.sprite = UserManager.Instance.ActiveLocation.IconLogo;

    adsButton.SetActive(BattleController.Instance.adsReady);
    BattleController.Instance.adsReady = false;

  }

  public void BaseButton() {
    if (OnBase != null) OnBase();
  }

  public void RepeatButton() {
    if (OnRepeat != null) OnRepeat();
  }

  public void VideoAdsButton() {
    if (OnVideoAds != null) OnVideoAds();
  }


}
