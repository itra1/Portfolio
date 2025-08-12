using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoldLabel : MonoBehaviour {

	public LocalizationUiText text;
	public RectTransform back;

	private void OnEnable() {
		ChangeCalue();
		text.OnChange += ChangeCalue;
	}

	private void OnDisable() {
		text.OnChange -= ChangeCalue;
	}

	public void ChangeCalue() {

		float width = text.GetComponent<Text>().preferredWidth;
		back.sizeDelta = new Vector2(width + 75, back.sizeDelta.y);

	}

}
