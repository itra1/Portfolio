using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LineValue : MonoBehaviour {

	public RectTransform mask;
	private Vector2 _startsizeDelta;

	private void Awake() {
		//_startsizeDelta = mask.sizeDelta;
	}

	public void SetValue(float value, float maxValue = 100) {
		if(_startsizeDelta == Vector2.zero)
			_startsizeDelta = mask.sizeDelta;

		mask.sizeDelta = new Vector2(_startsizeDelta.x/ maxValue* value, _startsizeDelta.y);
	}

}
