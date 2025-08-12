using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimTest : MonoBehaviour {


	public Animation anim;

	private void OnEnable() {
		anim.Play("Player");
	}


}
