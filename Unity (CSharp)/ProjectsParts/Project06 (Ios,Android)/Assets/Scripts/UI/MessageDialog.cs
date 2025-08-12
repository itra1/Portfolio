using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class MessageDialog : UiDialog {
  
  public Action OnClickOk;

  public Text messageText;

  public void OkButton() {
    if(OnClickOk != null) OnClickOk();
    Destroy(gameObject);
  }
}
