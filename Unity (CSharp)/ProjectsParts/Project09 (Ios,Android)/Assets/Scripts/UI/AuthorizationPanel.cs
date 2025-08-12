using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

/// <summary>
/// Панель авторизации
/// </summary>
public class AuthorizationPanel : MonoBehaviour {

  public Action<string> OnAuthorization;                  // Событие нажатие кнопки авторизации
  public Text nameInput;                                  // Поле ввода имени

  /// <summary>
  /// Успешная авторизаци
  /// </summary>
  public void AuthorizationButton() {
    if(OnAuthorization != null) OnAuthorization(nameInput.text);
    Close();
  }

  public void Close() {
    OnClose();
  }

  public void OnClose() {
    gameObject.SetActive(false);
  }

}
