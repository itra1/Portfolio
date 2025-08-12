using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergyHealthUiGamePlay : MonoBehaviour {

	private float lenght = 300;
	public RectTransform liveLine;

	private void OnEnable() {
		HealthManagerBase.OnChangeEvent += OnChangeEvent;
	}

	private void OnDisable() {
		HealthManagerBase.OnChangeEvent -= OnChangeEvent;
	}

	private void OnChangeEvent(float actualValue , float maxValue) {
		liveLine.sizeDelta = new Vector2(300 * (actualValue/ maxValue), liveLine.sizeDelta.y);
	}

}
