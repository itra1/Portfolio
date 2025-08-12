using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanguageUiElement : MonoBehaviour {

	public Action<LanuageLibrary> OnDown;
	public Action<LanuageLibrary> OnUp;

	private LanuageLibrary _language;

	public Text title;
	public Image icon;
	public GameObject gal;

	public void SetData(LanuageLibrary selectData) {
		_language = selectData;
		title.text = _language.nativeTitle;
		icon.sprite = _language.sprite;
	}

	public void SetSelected(LanuageLibrary selected) {
		
		gal.gameObject.SetActive(selected.code == _language.code);
	}

	public void OnClickDown() {
		if (OnDown != null) OnDown(_language);
	}

	public void OnClickUp() {
		if (OnUp != null) OnUp(_language);
	}

}
