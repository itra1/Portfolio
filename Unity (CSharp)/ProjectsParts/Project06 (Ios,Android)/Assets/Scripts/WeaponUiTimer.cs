using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponUiTimer : MonoBehaviour {
	
	public Image timer;
	private float targetTime;
	private float value;
	private float startTime;
	private bool isActive;

	public Action OnComplete;
	
	public void StartTimer(float timeValue, Action OnComplete) {

    timer.fillMethod = Image.FillMethod.Radial360;
    timer.fillOrigin = 0;

    startTime = Time.time;
		targetTime = startTime + timeValue;
		value = 1;
		isActive = true;
		this.OnComplete = OnComplete;
	}
	public void Locked() {
		value = 1;
	}

	public void StopTimer() {
		isActive = false;
		ClearTimer();
	}

	public void ClearTimer() {
		value = 0;
		timer.fillAmount = value;
	}

	private void Update() {
		if(isActive)
			UpdateTimer();
	}
	
	void UpdateTimer() {
		value = (targetTime - Time.time) / (targetTime - startTime);
		timer.fillAmount = value;

		if (value > 0) return;

		StopTimer();
		if (OnComplete != null) OnComplete();
		isActive = false;
	}
}
