using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatUiGamePlay : MonoBehaviour {

	public Text distanceText;
	public Text coinsText;
	public RectTransform coinsImage;
	public Animation coinsImageAnim;

	private void OnEnable() {
		RunnerController.changeDistance += ChangeDistantionPoint;
		RunnerController.OnCoinsInRace += ChangeCoinsCount;
		ChangeDistantionPoint(RunnerController.playerDistantion);
		ChangeCoinsCount(0);
	}

	private void OnDisable() {
		RunnerController.changeDistance -= ChangeDistantionPoint;
		RunnerController.OnCoinsInRace -= ChangeCoinsCount;
	}
	
	public void ChangeDistantionPoint(float newDistance) {
		distanceText.text = ((int)newDistance).ToString() + " М";
	}
	
	// Изменение значения счетчика монет
	public void ChangeCoinsCount(int count) {

		coinsImageAnim.Play("coins");

		// Смещение значка при изменении длинны числа
		coinsText.text = count.ToString();
		coinsImage.anchoredPosition = new Vector3(-15 - coinsText.preferredWidth, coinsImage.anchoredPosition.y, 0);

	}

}
