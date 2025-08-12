using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextUiScale : MonoBehaviour {
	
	private Text _textUi;
	private Vector2 rectTrans;
	private int _fontSize;
	public bool toUppcase;

	private string _text;

	private string localCode = "";
	public bool horizontalScale;

	public void Awake() {
	}

	private Text _textLabel {
		get {

			if (_textUi == null) {
				_textUi = GetComponent<Text>();
				rectTrans = _textUi.GetComponent<RectTransform>().sizeDelta;
				_fontSize = _textUi.fontSize;
			}
			return _textUi;
		}
	}

	private void OnEnable() {
		if (!String.IsNullOrEmpty(_text))
			SetText(_text);
	}
	
	public void SetText(string dataString, string localeCode = null) {

		if (localeCode != null && this.localCode != localeCode) {
			this.localCode = localeCode;
			
		}

		_text = dataString;

		if(_textLabel == null) return;

		if (toUppcase)
			dataString = dataString.ToUpper();

		_textLabel.text = dataString;

		if (horizontalScale) {
			_textLabel.fontSize = _fontSize;

			while (rectTrans.x < _textLabel.preferredWidth)
				_textLabel.fontSize--;
		}
	}
}
