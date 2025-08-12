using System;
using UnityEngine;

public class Timer : UiDialog {

	public Action OnStart;
	public Action<int> OnNum;

	public void StartEvent() {
		if (OnStart != null)
			OnStart();
	}

	public void EndEvent() {
		gameObject.SetActive(false);
	}

	public void TimerItem(int num) {
		if (OnNum != null) OnNum(num);
	}

}
