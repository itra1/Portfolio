using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Follower : MonoBehaviour {

	public Transform source;

	public bool position;

	private void LateUpdate() {
		if (position)
			transform.position = source.position;
	}



}
