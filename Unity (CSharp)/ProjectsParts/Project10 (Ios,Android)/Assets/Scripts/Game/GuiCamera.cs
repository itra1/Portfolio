using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuiCamera : MonoBehaviour {

	public static GuiCamera instance;

	public void Awake() {
		instance = this;
	}

}
