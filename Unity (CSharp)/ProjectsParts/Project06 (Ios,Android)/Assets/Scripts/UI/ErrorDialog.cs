using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Диалог ошибки
/// </summary>
public class ErrorDialog : UiDialog {

  public Text errorText;
	
  public void OkButton() {
    Destroy(gameObject);
  }
}
