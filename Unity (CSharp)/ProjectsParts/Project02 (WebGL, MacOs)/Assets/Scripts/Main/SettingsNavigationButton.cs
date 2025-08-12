using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class SettingsNavigationButton : MonoBehaviour
{
	//[SerializeField] private SettingsPageType _page;
	//public SettingsPageType Page { get => _page; set => _page = value; }

	[SerializeField] private Color _focusColor;
	[SerializeField] private Image _toggleImage;

	private Image _back;
	private bool _isFocus;
	private TextMeshProUGUI _title;
	public void SetFocus(bool focus){
		if (_isFocus == focus) return;
		_isFocus = focus;

		if(_back == null)
			_back = GetComponent<Image>();
		if (_title == null)
			_title = GetComponentInChildren<TextMeshProUGUI>();

		Color backColor = Color.white;
		backColor.a = _isFocus ? 1 : 0;
		_back.DOColor(backColor, 0.2f);
		_toggleImage.DOColor(backColor, 0.2f);
		_title.DOColor(_isFocus ? _focusColor : Color.white,0.2f);

		GetComponent<it.UI.Elements.TextButtonUI>().StartColor = _isFocus ? _focusColor : Color.white;
	}

}
