using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorHelper : MonoBehaviour {
	
	public Action OnAnim1;
	public Action OnBearDown;
  public System.Action<string> OnEvent;

	public void OnAnim1Event() {
		if (OnAnim1 != null) OnAnim1();
	}

	public void BearDown() {
		if (OnBearDown != null) OnBearDown();
	}

  public void Event(string name) {
    if (OnEvent != null)
      OnEvent(name);
  }

}
