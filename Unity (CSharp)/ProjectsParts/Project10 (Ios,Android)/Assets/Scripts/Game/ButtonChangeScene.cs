using UnityEngine;
using System.Collections;

public class ButtonChangeScene : MonoBehaviour {

	public RunnerController runner;
	public int toScene;

	void OnMouseDown() {

		//if (!UiController.checkStart()) return;
		if (runner.runnerPhase != RunnerPhase.start) return;

		if (Input.GetMouseButtonDown(0)) {
			runner.GoToLevel(toScene);
		}
	}
}
