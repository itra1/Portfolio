using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlphaRegionUi : MonoBehaviour {

	public Action DownEvent;
	public Action EnterEvent;

	public void OnDown() {
		if (DownEvent != null) DownEvent();
	} 

	public void OnEnter() {
		if (EnterEvent != null) EnterEvent();
	}


}
