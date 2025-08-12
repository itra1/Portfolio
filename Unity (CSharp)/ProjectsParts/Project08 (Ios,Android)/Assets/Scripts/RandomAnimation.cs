using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animation))]
public class RandomAnimation : MonoBehaviour {
	private Animation anim;
	public List<string> animList;

	public float minTime;
	public float maxTime;

	private void OnEnable() {
		anim = GetComponent<Animation>();

		if (animList.Count == 0) return;
		_waitCor = StartCoroutine(WaitTime());
	}

	private void OnDisable() {
		StopCoroutine(_waitCor);
	}
	
	private Coroutine _waitCor;
	IEnumerator WaitTime() {
		while (true) {
			yield return new WaitForSeconds(Random.Range(minTime, maxTime));
			anim.Play(animList[Random.Range(0, animList.Count)]);
		}
	}
}
