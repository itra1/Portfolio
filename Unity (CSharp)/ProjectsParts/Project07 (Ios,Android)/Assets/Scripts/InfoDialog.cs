using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoDialog : UiDialog {

  public Text text;

  private System.Action OkCallback = null;

  protected override void OnEnable() {
    base.OnEnable();
    OkCallback = null;
  }

  public void SetData(string data, System.Action OkCallback = null) {
    text.text = data;
    this.OkCallback = OkCallback;
  }

  public void CloseButton() {
    Hide(() => {

      if (OkCallback != null) OkCallback();
    });

  }

  public void OkButton() {
    CloseButton();
  }

}
