using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextUGUIScale : MonoBehaviour {
	
	private TextMeshProUGUI _textUi;
	private Vector2 rectTrans;
	private float _fontSize;
	public bool toUppcase;

	private string _text;

	private string localCode = "";

	public bool horizontalScale;

	public TextMeshProUGUI textUi {
		get {
			if(_textUi == null)
				_textUi = GetComponent<TextMeshProUGUI>();
			return _textUi;
		}
		set { _textUi = value; }
	}

	public void Awake() {
		_textUi = GetComponent<TextMeshProUGUI>();
		
		rectTrans = _textUi.GetComponent<RectTransform>().sizeDelta;
		_fontSize = _textUi.fontSize;
	}

	private void OnEnable() {
		if (!String.IsNullOrEmpty(_text))
			SetText(_text,this.localCode);
	}

	public void SetText(string dataString, string localeCode = null) {

		if (localeCode != null /*&& this.localCode != localeCode*/) {
			this.localCode = localeCode;

			var lib = LanguageManager.Instance.lanuageTypeParam.Find(x => x.code == this.localCode);
			if (lib == null)
				lib = LanguageManager.Instance.activeLanuage;

		}

		_text = dataString;

		if (_textUi == null) return;

		if (toUppcase)
			dataString = dataString.ToUpper();

		_textUi.text = dataString;

		if (horizontalScale) {
			_textUi.fontSize = _fontSize;
			while (rectTrans.x < _textUi.preferredWidth)
				_textUi.fontSize--;
		}
	}

	
}
