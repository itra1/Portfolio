using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animation))]
public class RandomAnimation2 : MonoBehaviour {
	private Animation anim;
	public List<string> animList;

	private void OnEnable() {
		anim = GetComponent<Animation>();

		anim.Play(animList[Random.Range(0, animList.Count)]);
	}

	private void OnDisable() {
		anim.Stop();
	}
}
