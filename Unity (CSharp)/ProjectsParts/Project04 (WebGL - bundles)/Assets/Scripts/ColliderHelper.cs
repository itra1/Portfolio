using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderHelper : MonoBehaviour {

	public Action<Collider> TriggerEnter;
	public Action<Collider> TriggerExit;

	private void OnTriggerEnter(Collider other) {
		if (TriggerEnter != null) TriggerEnter(other);
	}

	private void OnTriggerExit(Collider other) {
		if (TriggerExit != null) TriggerExit(other);
	}




}
