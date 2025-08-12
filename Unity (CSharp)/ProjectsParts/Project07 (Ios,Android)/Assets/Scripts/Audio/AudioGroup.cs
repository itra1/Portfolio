using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExEvent;

public class AudioGroup : EventBehaviour {

	public AudioSource audioSource;
	public List<AudioClipData> audioDataList;
	
	public void PlayRandom(AudioSource audioSourceInst) {
		Play(audioSourceInst, Random.Range(0, audioDataList.Count));
	}

	public void Play(AudioSource audioSourceInst, int numPosition) {

		if (numPosition > audioDataList.Count) return;

		if (audioDataList[numPosition].audioClip == audioSourceInst.clip) {
			audioSourceInst.volume = audioDataList[numPosition].volume;
			audioSourceInst.UnPause();
			if(!audioSourceInst.isPlaying) audioSourceInst.Play();
			return;
		}

		audioSourceInst.clip = audioDataList[numPosition].audioClip;
		audioSourceInst.volume = audioDataList[numPosition].volume;
		audioSourceInst.loop = audioDataList[numPosition].loop;
		audioSourceInst.time = audioDataList[numPosition].startTimeClip;
		audioSourceInst.pitch = audioDataList[numPosition].pitchValue;
		audioSourceInst.Play();
	}

	public void Pause() {
		
	}

	public void UnPause() {
		
	}

	public void Stop() {
		
	}

}
