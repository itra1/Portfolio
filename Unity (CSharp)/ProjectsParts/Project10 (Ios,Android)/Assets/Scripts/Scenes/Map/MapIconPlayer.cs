using UnityEngine;
using System.Collections;

public class MapIconPlayer : MonoBehaviour {

	public AudioClip jumpClip;

	void Start() {

		if (UserManager.Instance.survivleMaxRunDistance >= 5800) GetComponent<Animator>().enabled = false;

#if UNITY_EDITOR
		//GetComponent<Animator>().Stop();
#endif
	}

	public void EventJump() {
		AudioManager.PlayEffect(jumpClip, AudioMixerTypes.mapEffect, 0.7f);
	}
}
