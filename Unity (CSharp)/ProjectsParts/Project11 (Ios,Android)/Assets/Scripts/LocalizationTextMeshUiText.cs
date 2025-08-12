using System;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class LocalizationTextMeshUiText : ExEvent.EventBehaviour {

  public string code;
  private TextMeshProUGUI _textUi;
  public bool toUppcase;
  
  protected override void Awake() {
    base.Awake();
    _textUi = GetComponent<TextMeshProUGUI>();
  }

  private void OnEnable() {
    ChangeLanuage();
  }

  public void SetCode(string code) {
    this.code = code;
    ChangeLanuage();
  }

  [ExEvent.ExEventHandler(typeof(ExEvent.LanuageEvents.OnChangeLanguage))]
  public void OnChangeLanuage(ExEvent.LanuageEvents.OnChangeLanguage language) {
    ChangeLanuage();
  }

  private void ChangeLanuage() {

    string titleText = LanguageManager.GetTranslate(code);

    if (String.IsNullOrEmpty(titleText)) return;

    if (toUppcase)
      titleText = titleText.ToUpper();

    _textUi.text = titleText;

  }
}
