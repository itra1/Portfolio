using System.Collections;
using System.Collections.Generic;
using ExEvent;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LocationSwipeTranslateElement : EventBehaviour {

	private Text _text;
	private Image _image;
	private TextMeshProUGUI _textMesh;

	private void OnEnable() {
		_text = GetComponent<Text>();
		_image = GetComponent<Image>();
		_textMesh = GetComponent<TextMeshProUGUI>();
	}

	[ExEventHandler(typeof(GameEvents.SwipeLocation))]
	public void SwipeLocation(GameEvents.SwipeLocation cll) {
		
		if(_text != null)
			_text.color = new Color(_text.color.r, _text.color.g, _text.color.b, cll.alpha);

		if (_image != null)
			_image.color = new Color(_image.color.r, _image.color.g, _image.color.b, cll.alpha);

		if (_textMesh != null)
			_textMesh.color = new Color(_textMesh.color.r, _textMesh.color.g, _textMesh.color.b, cll.alpha);

	}

}
