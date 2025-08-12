using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Иконка специального продукта
/// </summary>
public class SpecialProductIcon : PanelUiBase {

	public Text timer;

	public void OnEnable() {
		SpecialSales.OnPhaseChange += ChangePhase;
		StartCoroutine(Timer());
	}

	public void OnDisable() {
		SpecialSales.OnPhaseChange -= ChangePhase;
	}

	private void Update() {
		IconLightUpdate();
	}

	private void ChangePhase(SpecialSales.Phase phase) {

		if (phase != SpecialSales.Phase.ready)
			gameObject.SetActive(false);

	}

	private TimeSpan timeDelta;
	private IEnumerator Timer() {

		while (true) {

			if (SpecialSales.Instance.nextShow < DateTime.Now) {
				timer.text = "00:00:00";
			}
			else {
				timeDelta = SpecialSales.Instance.nextShow - DateTime.Now;
				timer.text = String.Format("{0:00}:{1:00}:{2:00}", timeDelta.Hours, timeDelta.Minutes, timeDelta.Seconds);
			}

			yield return new WaitForSeconds(1);
		}
	}
	
	/// <summary>
	/// Событие клика по значку
	/// </summary>
	public void Click() {
		UiController.ClickButtonAudio();
		SpecialSales.Instance.IconButtonClick();
	}

	public GameObject goldLight;            // Объект света
	private float nextLightTime;            // Время следующей анимации

	/// <summary>
	/// Анимация блеска
	/// </summary>
	private void IconLightUpdate() {
		if (nextLightTime < Time.time) {
			nextLightTime = Time.time + UnityEngine.Random.Range(1f, 4f);
			goldLight.GetComponent<Animator>().SetTrigger("play");
		}
	}

}
