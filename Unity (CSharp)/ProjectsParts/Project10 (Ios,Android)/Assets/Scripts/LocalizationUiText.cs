using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class LocalizationUiText : ExEvent.EventBehaviour {

	public event Action OnChange; 

	public string code;
	private Text _textUi;

	private Text textUi {
		get {
			if (_textUi == null) {
				textUi = GetComponent<Text>();
				rectTrans = textUi.GetComponent<RectTransform>().sizeDelta;
				_fontSize = textUi.fontSize;
			}
			return _textUi;
		}
		set { _textUi = value; }
	}

	private Vector2 rectTrans;
	public bool toUppcase;

	private int _fontSize;

	public bool scaleText = true;
	
	private void OnEnable() {
		ChangeLanuage();
	}

	public void SetCode(string code) {
		this.code = code;
		ChangeLanuage();
	}

	[ExEvent.ExEventHandler(typeof(ExEvent.LanguageEvents.OnChangeLanguage))]
	public void OnChangeLanuage(ExEvent.LanguageEvents.OnChangeLanguage language) {
		ChangeLanuage();
	}

	private void ChangeLanuage() {

		string titleText = LanguageManager.GetTranslate(code);

		if (String.IsNullOrEmpty(titleText)) return;

		if (toUppcase)
			titleText = titleText.ToUpper();
		
		textUi.text = titleText;
		
		if (scaleText) {
			textUi.fontSize = _fontSize;
			while (rectTrans.x < textUi.preferredWidth)
				textUi.fontSize--;
		}

		if (OnChange != null) OnChange();

	}

}
