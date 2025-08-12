using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour {

  public System.Action onComplete;
	public Text text;

	private DateTime _targetTime;

	public bool isUtcTime;
    
	private void Start() { }

	public void StartTimer(TimeSpan timeSpan) {

		if (!gameObject.activeInHierarchy) return;
        
    _targetTime = useDateTime + timeSpan;
		StartCoroutine(TimerCor());
	}

	IEnumerator TimerCor() {

		while (_targetTime > useDateTime) {
			SetTime(_targetTime - useDateTime);
			yield return new WaitForSeconds(1);
		}
    if (onComplete != null)
      onComplete();

  }

	private DateTime useDateTime {
		get { return isUtcTime ? DateTimeOffset.UtcNow.DateTime : DateTime.Now; }
	}

	void SetTime(TimeSpan delta) {
		text.text = String.Format("{0}:{1:00}:{2:00}", delta.Hours + (delta.Days*24), delta.Minutes, delta.Seconds);
	}

}
