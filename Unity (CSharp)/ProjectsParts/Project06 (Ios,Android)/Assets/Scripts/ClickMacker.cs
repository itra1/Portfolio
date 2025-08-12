using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickMacker : MonoBehaviour {

	public Action OnClick;

	private void OnEnable() {
		ScreenTap.Instance.DeactiveImege();
	}

	private void OnDisable() {
		ScreenTap.Instance.ActiveImage();
	}



	public void Click() {
		if (OnClick != null) OnClick();
	}
}
