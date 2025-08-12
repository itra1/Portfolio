using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Игровой таймер
/// </summary>
public class GameTimer : MonoBehaviour {

	public Text text;

	public Color defaultColor;
	public Color alarmColor;
	private bool changeToAlarm;
	private float _alarmSecond;

	private Action OnComplete;

	private float _targetTime;
	private float _lastChangeTime = 0;

	public void StartTimer(float secondCount, float alarmSecond, Action OnComplete) {
		this.OnComplete = OnComplete;
		changeToAlarm = false;
		text.color = defaultColor;

		this._alarmSecond = alarmSecond;
		_targetTime = Time.realtimeSinceStartup + secondCount;
		_lastChangeTime = 0;
		isTimer = true;

		//StartCoroutine(TimerCor());
	}

	public void StopTimer() {
		isTimer = false;
	}

	private bool isTimer;

	private void Update() {

		if (!isTimer) return;

		if (_targetTime > Time.realtimeSinceStartup) {
			if (_lastChangeTime < Time.realtimeSinceStartup - 1) {
				SetTime(_targetTime - Time.realtimeSinceStartup);
				_lastChangeTime = Time.realtimeSinceStartup;
			}

		}
		else {
			SetTime(0);
			isTimer = false;
			if (OnComplete != null) OnComplete();
		}


	}



	IEnumerator TimerCor() {

		while (_targetTime > Time.time) {
			SetTime(_targetTime - Time.time);
			yield return new WaitForSeconds(1);
		}

		if (OnComplete != null) OnComplete();

	}

	public void SetTime(float secondCount) {
		text.text = String.Format("{0}:{1:00}", (int)(secondCount / 60), (int)(secondCount % 60));

		if (!changeToAlarm) {
			if (secondCount < _alarmSecond) {
				changeToAlarm = true;
				text.color = alarmColor;
			}
		}

	}

}
