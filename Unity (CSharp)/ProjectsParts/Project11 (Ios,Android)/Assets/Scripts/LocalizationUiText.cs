using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class LocalizationUiText : ExEvent.EventBehaviour {

	public string code;
	private Text _textUi;

	private Text textUi {
		get {
			if (_textUi == null) {
				textUi = GetComponent<Text>();
				textUi.font = LanguageManager.Instance.activeLanuage.font;
				rectTrans = textUi.GetComponent<RectTransform>().sizeDelta;
			}
			return _textUi;
		}
		set { _textUi = value; }
	}

	private Vector2 rectTrans;
	public bool toUppcase;

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

		textUi.font = LanguageManager.Instance.activeLanuage.font;

		textUi.text = titleText;
		
	}

}
