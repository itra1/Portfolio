using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinDialog : UiDialog {

  public System.Action OnBase;
  public System.Action OnNext;

  public Text goldCoinsText;

  public void BaseButton() {
    if (OnBase != null) OnBase();
  }

  public void NextButton() {
    if (OnNext != null) OnNext();
  }

  protected override void OnEnable() {
    base.OnEnable();
    goldCoinsText.text = UserManager.Instance.ActiveLocation.goldWin.ToString() + " монет";
  }

}
