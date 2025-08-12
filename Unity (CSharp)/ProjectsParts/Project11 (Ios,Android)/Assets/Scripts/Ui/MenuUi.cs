using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuUi: UiPanel {

  public Animation animComponent;
  
  public GameObject logo;

  private string _hideAnim = "hide";
  private string _showAnim = "show";

  public RectTransform bannerPosition;
  public RectTransform shopButton;
  public RectTransform bonusButton;
  public RectTransform moreButton;
  public RectTransform settingButton;

  protected override void OnEnable() {
    base.OnEnable();
    
    KingBird.Ads.AdsKingBird.Instance.OpenWindow();

#if UNITY_IOS
    if (UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhoneX) {
    bannerPosition.anchoredPosition = new Vector2(0, -65);
    shopButton.anchoredPosition = new Vector2(0, 50);
    bonusButton.anchoredPosition = new Vector2(0, 50);
    moreButton.anchoredPosition = new Vector2(0, -50);
    settingButton.anchoredPosition = new Vector2(0, -50);
    }
#endif

  }

  public void PlayLogo() {
    logo.SetActive(true);
    logo.GetComponent<Animator>().SetTrigger("play");
  }

  public void MoreGamesButton() {
    AudioManager.Instance.library.PlayClickAudio();
    GameManager.Instance.MoreGames();
  }

  public void SettingButton() {
    //AudioManager.Instance.library.PlayClickAudio();
    GameManager.Instance.Setting();
  }

  public void ShopButton() {
    //AudioManager.Instance.library.PlayClickAudio();
    GameManager.Instance.Shop();
  }

  public void GetBonusButton() {
    AudioManager.Instance.library.PlayClickAudio();
    GameManager.Instance.GetBonus();
  }

  public void PlayButton() {
    AudioManager.Instance.library.PlayClickAudio();
    GameManager.Instance.MainPlay();
  }

  public override void Show(Action OnShow = null) {
    base.Show(OnShow);
    animComponent.Play(_showAnim);
  }
  
  public override void Hide(Action OnHide = null) {
    base.Hide(OnHide);
    animComponent.Play(_hideAnim);
  }

  public override void ManagerClose() {

    ExitGameQuestion panel = UIManager.Instance.GetPanel<ExitGameQuestion>();

    if (panel.isActiveAndEnabled) {
      panel.Hide();
    } else {
      panel.gameObject.SetActive(true);
      panel.OnCancel = () => {
        panel.Hide();
      };
      panel.OnExit = () => {
        Application.Quit();
      };
    }

  }


}
