using System.Collections;
using System.Collections.Generic;
using ExEvent;
using UnityEngine;
using UnityEngine.UI;

public class CoinsUi : ExEvent.EventBehaviour {

	public Text text;
	public float incWidth;

	private RectTransform _rectTransform;

	private RectTransform rectTransform {
		get {
			if (_rectTransform == null)
				_rectTransform = GetComponent<RectTransform>();
			return _rectTransform;
		}
	}

	private void Start() {
		CoinsChange();
	}
	
	private void ScalingRect() {
		rectTransform.sizeDelta = new Vector2(text.preferredWidth + incWidth, rectTransform.sizeDelta.y);
	}

	[ExEventHandler(typeof(RunEvents.CoinsChange))]
	private void CoinsChange(RunEvents.CoinsChange eventData) {
		CoinsChange();
	}

	private void CoinsChange() {

		text.text = UserManager.coins.ToString();
		ScalingRect();
	}

}
