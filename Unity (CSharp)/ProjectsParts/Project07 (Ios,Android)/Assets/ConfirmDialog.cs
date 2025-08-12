using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmDialog: UiDialog {

  public I2.Loc.Localize text;

  private System.Action OkCallback = null;
  private System.Action CancelCallback = null;

  protected override void OnEnable() {
    base.OnEnable();
    OkCallback = null;
  }

  public void SetData(string data, System.Action OkCallback = null, System.Action CancelCallback = null) {
    text.Term = data;
    this.OkCallback = OkCallback;
    this.CancelCallback = CancelCallback;
  }

  public void CloseButton() {
    Hide(() => {

    });

  }

  public void OkButton() {
    if (OkCallback != null) OkCallback();
    CloseButton();
  }

  public void CancelButton() {
    if (CancelCallback != null) CancelCallback();
    CloseButton();
  }
}
