using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusTimer : MonoBehaviour {

	public GameObject timerObject;
	public Animation animComp;

	public float timeWait;
	public float videoIncrement;

	public GameTimer timer;

	public void StartTimer() {

		timer.StartTimer(timeWait, 30, () => {
			PlayerManager.Instance.company.BonusLevelTimeEnd();
		});
	}

	public void ResumeTimer() {
		timer.StartTimer(60, 30, () => {
			PlayerManager.Instance.company.BonusLevelTimeEnd();
		});
	}

	public void StopTimer() {
		timer.StopTimer();
	}

	public void SetShow() {
		timerObject.gameObject.SetActive(true);
		timer.SetTime(timeWait);
	}

	public void SetHide() {
		timerObject.gameObject.SetActive(false);
	}

}
