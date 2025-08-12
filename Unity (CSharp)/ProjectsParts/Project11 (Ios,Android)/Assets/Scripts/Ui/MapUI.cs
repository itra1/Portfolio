using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapUI : UiPanel {

  public RectTransform cloudRect;
  public System.Action OnMenu;

  public Animator cloudsAnimator;

  public RectTransform menuButton;
  public RectTransform bonusButton;


  protected override void OnEnable() {
    base.OnEnable();

#if UNITY_IOS
    if (UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhoneX) {
    menuButton.anchoredPosition = new Vector2(0, -50);
    bonusButton.anchoredPosition = new Vector2(0, -50);
    }
#endif
  }

  private bool isCloud;
  public void SetCloudDiff(float perc) {
    
    cloudRect.anchoredPosition = new Vector2(0, Mathf.Lerp(750, 0, perc));
    if(!isCloud && perc >= 0.5f) {
      cloudsAnimator.SetBool("show", true);
    }
    if (!isCloud && perc <= 0.1f) {
      cloudsAnimator.SetBool("show", false);
    }
  }

  public override void ManagerClose() {
    MenuButton();
  }

  public void ShareButton() {

    string sendText = LanguageManager.GetTranslate("share.link") + " ";

#if UNITY_ANDROID
    sendText += "https://play.google.com/store/apps/details?id=com.kingbirdgames.candywords";
#endif

#if UNITY_IOS
		sendText += "https://itunes.apple.com/app/id1294114269";
#endif

    GetComponent<Sharing>().Share(sendText);
  }

  public void RateUsButton() {

    GameManager.Instance.RateUsButton();
  }

  public void MenuButton() {
    if (OnMenu != null) OnMenu();
  }

}
