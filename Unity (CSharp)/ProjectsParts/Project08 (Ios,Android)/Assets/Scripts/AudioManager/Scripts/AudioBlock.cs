using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AudioBlock {

	public AudioGroup audioGroup;
	[HideInInspector]
	public AudioSource _audioSourceInst;
	private List<AudioSource> _audioSourceInstAll = new List<AudioSource>();

	private Coroutine pauseCoroutine;
	private Coroutine unpauseCoroutine;

	private MonoBehaviour parent;

	public bool isBackMusic;
	
	public void PlayRandom(MonoBehaviour parent) {

		if (audioGroup == null) return;
		this.parent = parent;

		CheckSource(parent);
		
		audioGroup.PlayRandom(_audioSourceInst);
	}

	public void Init(MonoBehaviour parent) {
		this.parent = parent;
	}

	public void Play(MonoBehaviour parent, int numPosition) {
		if (audioGroup == null) return;
		this.parent = parent;
		CheckSource(parent);
		audioGroup.Play(_audioSourceInst, numPosition);
	}

	public void Play(AudioBlock audioBlock) {
		if (parent == null) return;
		this.audioGroup = audioBlock.audioGroup;

		CheckSource(parent);

		_audioSourceInst.outputAudioMixerGroup = audioBlock.audioGroup.audioSource.outputAudioMixerGroup;
		
		PlayRandom(parent);
	}

	public void Play(AudioBlock audioBlock, int numPosition) {
		if (parent == null) return;
		this.audioGroup = audioBlock.audioGroup;

		CheckSource(parent);

		_audioSourceInst.outputAudioMixerGroup = audioBlock.audioGroup.audioSource.outputAudioMixerGroup;
		
		Play(parent, numPosition);
	}

	private void CheckSource(Component parent) {
		
		_audioSourceInst = isBackMusic && _audioSourceInstAll.Count > 0 
			? _audioSourceInstAll[0] 
			: _audioSourceInstAll.Find(x =>x != null && !x.isPlaying);

		if (_audioSourceInst == null) {
			GameObject inst = MonoBehaviour.Instantiate(audioGroup.audioSource.gameObject);
			inst.transform.SetParent(parent.transform);
			inst.transform.localPosition = Vector3.zero;
			_audioSourceInst = inst.GetComponent<AudioSource>();
			_audioSourceInst.outputAudioMixerGroup = audioGroup.audioSource.outputAudioMixerGroup;
			_audioSourceInstAll.Add(_audioSourceInst);
		}
	}

	public void Pause(float time = 0) {

		if (_audioSourceInst == null) return;

		if (time == 0)
			_audioSourceInst.Pause();
		else {
			pauseCoroutine = parent.StartCoroutine(PauseCor(time));
		}

	}

	public void UnPause(float time = 0) {

		if (time == 0) {
			_audioSourceInst.volume = 1;
			_audioSourceInst.UnPause();
		} else {
			unpauseCoroutine = parent.StartCoroutine(UpPauseCor(time));
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
		if (parent == null) return;
		parent.StopAllCoroutines();

		if (time == 0) {
			if(_audioSourceInst != null)
				_audioSourceInst.Stop();
			if (OnComplited != null) OnComplited();
		} else {
			parent.StartCoroutine(StopCor(time, isLocked, OnComplited));
		}

	}

	IEnumerator PauseCor(float time) {
		if (unpauseCoroutine != null) parent.StopCoroutine(unpauseCoroutine);
		if (_audioSourceInst != null) {
			while (_audioSourceInst.volume > 0f) {
				_audioSourceInst.volume -= 0.1f / time;
				yield return new WaitForSecondsRealtime(0.1f);
			}
		}
		_audioSourceInst.Pause();
	}

	IEnumerator UpPauseCor(float time) {
		if (pauseCoroutine != null) parent.StopCoroutine(pauseCoroutine);
		_audioSourceInst.UnPause();
		while (_audioSourceInst.volume < 1) {
			_audioSourceInst.volume += 0.1f / time;
			if (_audioSourceInst.volume > 1) _audioSourceInst.volume = 1;
			yield return new WaitForSecondsRealtime(0.1f);
		}
	}

	IEnumerator StopCor(float time, bool isLocked = false, Action OnComplited = null) {
		while (_audioSourceInst.volume > 0f) {
			_audioSourceInst.volume -= 0.1f / time;
			yield return new WaitForSecondsRealtime(0.1f);
		}
		_audioSourceInst.Stop();
		if (OnComplited != null) OnComplited();
	}

}
