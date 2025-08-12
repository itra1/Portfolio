using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickHelper : MonoBehaviour {

	public Action OnClick;

	private void OnMouseDown() {
		if (OnClick != null) OnClick();
	}
}
