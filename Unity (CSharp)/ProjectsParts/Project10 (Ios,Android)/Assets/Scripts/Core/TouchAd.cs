using UnityEngine;
using System.Collections;

public class TouchAd : MonoBehaviour {

    public RunnerController runner;
    public AdController adCon;

    void OnMouseDown() {
        if(Input.GetMouseButtonDown(0)) {
            if(runner.runnerPhase != RunnerPhase.start) return;
            adCon.Click();
        }
    }
}
