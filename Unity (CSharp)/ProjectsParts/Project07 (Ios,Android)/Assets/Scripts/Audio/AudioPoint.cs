using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPoint : MonoBehaviour {

	public bool isLocked;           // Блокирован для переключения
	public bool isBack;
	public AudioClipData clipData;
  public AudioSource source;

	Coroutine pauseCoroutine;
	Coroutine unpauseCoroutine;

	public void Start() {	}
	
	public void SetData(AudioClipData clipData) {
		this.clipData = clipData;
	}

	public void Pause(float time = 0) {

		if(time == 0)
			source.Pause();
		else {
			pauseCoroutine = StartCoroutine(PauseCor(time));
		}

	}

	public bool isPlaing { get { return source.isPlaying; } }

	public void Play() {
		StopAllCoroutines();
		Play(clipData);
	}

	public void Play(AudioSource audioSource) {
		if (clipData.audioClip == null) return;
		audioSource = source;
		audioSource.Play();
	}

	public void Play(AudioClipData clipData, bool force = false) {

		if(!force && source.clip == clipData.audioClip && source.isPlaying) {
			return;
		}

		source.loop = clipData.loop;
		source.clip = clipData.audioClip;
		source.volume = clipData.volume;
		source.pitch = clipData.pitchValue;
		isLocked = clipData.isLocked;
		source.time = clipData.startTimeClip;
		source.Play();
		SetData(clipData);
	}

	public void UnPause(float time = 0) {

		if(time == 0) {
			source.volume = clipData.volume;
			source.UnPause();
		} else {
			unpauseCoroutine = StartCoroutine(UpPauseCor(time));
		}
	}
	public void Stop() {
		Stop(0, false, null);
	}
	public void Stop(float time = 0) {
		Stop(time, false, null);
	}
	public void Stop(float time = 0, Action OnComplited = null) {
		Stop(time, false, OnComplited);
	}

	public void Stop(float time = 0, bool isLocked = false, Action OnComplited = null) {
		StopAllCoroutines();

		if(time == 0) {
			source.Stop();
			if (OnComplited != null) OnComplited();
			this.isLocked = isLocked;
		} else {
			StartCoroutine(StopCor(time, isLocked, OnComplited));
		}

	}

	IEnumerator PauseCor(float time) {
		if(unpauseCoroutine != null)	StopCoroutine(unpauseCoroutine);
		while(source.volume > 0f) {
			source.volume -= 0.1f / time;
			yield return new WaitForSecondsRealtime(0.1f);
		}
		source.Pause();
	}

	IEnumerator UpPauseCor(float time) {
		if(pauseCoroutine != null) StopCoroutine(pauseCoroutine);
		source.UnPause();
		while(source.volume < clipData.volume) {
			source.volume += 0.1f / time;
			if(source.volume > clipData.volume) source.volume = clipData.volume;
			yield return new WaitForSecondsRealtime(0.1f);
		}
	}

	IEnumerator StopCor(float time, bool isLocked = false, Action OnComplited = null) {
		while(source.volume > 0f) {
			source.volume -= 0.1f / time;
			yield return new WaitForSecondsRealtime(0.1f);
		}
		source.Stop();
		this.isLocked = isLocked;
		if (OnComplited != null) OnComplited();
	}

	public void ChangeMusic(AudioClipData clipData) {
		StartCoroutine(ChangeMusicCor(()=>{
			source.loop = clipData.loop;
			source.clip = clipData.audioClip;
			source.pitch = clipData.pitchValue;
			isLocked = clipData.isLocked;
			source.time = clipData.startTimeClip;
			source.Play();
			SetData(clipData);
		}));
	}

	IEnumerator ChangeMusicCor(Action OnComplete) {
		while (source.volume > 0f) {
			source.volume -= 2f *Time.deltaTime;
			yield return null;
		}
		source.volume = 0;
		OnComplete();
		while (source.volume < clipData.volume) {
			source.volume += 2f * Time.deltaTime;
			yield return null;
		}
		source.volume += clipData.volume;
	}

}
