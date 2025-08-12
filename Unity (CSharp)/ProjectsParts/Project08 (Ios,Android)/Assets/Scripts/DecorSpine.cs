using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class DecorSpine : MonoBehaviour {

	public SkeletonAnimation skeleton;

	private void Awake() {

		skeleton = GetComponent<SkeletonAnimation>();

		Invoke("Playanim",Random.Range(0,2f));

	}

	private void Playanim() {
		skeleton.AnimationState.SetAnimation(0, "idle", true);
	}
	
}
