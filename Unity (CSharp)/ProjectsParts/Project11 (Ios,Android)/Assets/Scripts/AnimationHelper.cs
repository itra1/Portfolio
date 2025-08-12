using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHelper : MonoBehaviour {

	public Action OnEvent1;

	public void Event1() {
		if (OnEvent1 != null) OnEvent1();
	}

}
