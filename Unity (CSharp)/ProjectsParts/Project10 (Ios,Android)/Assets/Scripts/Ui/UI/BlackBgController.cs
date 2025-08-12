/*
  Контроллер плашки черного фона
*/

using UnityEngine;
using System.Collections;

public class BlackBgController : MonoBehaviour {

    public MapController map;
    public RunnerController runner;

    void OnEnable() {
        //GameManager.actionProcess = true;
    }

	public void EndPlashka() {
        //GameManager.actionProcess = false;
        gameObject.SetActive(false);
        if (runner) {
            runner.EventEndPlashka();
        }

    }
    public void StartPlashka()
    {
        if (runner)
        {
            runner.ShowBlackBg();
        }
        if (map) {
            map.ShowBlackBg();
        }
    }
}
