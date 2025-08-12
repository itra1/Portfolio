using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animation))]
public class TapAnimator : MonoBehaviour {

	public List<string> animList;
	private Animation anim;

	private void OnEnable() {
		anim = GetComponent<Animation>();
	}

	public void PlayRandom() {
		anim.Play(animList[Random.Range(0, animList.Count)]);
	}

}
