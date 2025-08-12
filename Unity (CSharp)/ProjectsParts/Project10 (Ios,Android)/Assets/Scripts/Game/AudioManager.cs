using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;

public enum AudioSnapshotTypes {
	none, stopAll, stopMusic, stopEffects, runnerEffectDef, runnerEffect50, runnerMusicDef, runnerMusic50, runnerMusic0, runnerEffect0,
	mapMusicDef, mapMusic0, mapEffectDef, mapEffect0
}
public enum AudioMixerTypes {
	none, music, runnerMusic, comicsMusic, firstSplashMusic, gateMisic, mapMusic, shopMusic,
	soundEffect, runnerEffect, comicsEffect, firstSplashEffect, gateEffect, mapEffect, shopEffect,
	runnerUi
}

/// <summary>
/// Контроллер работы со звуком
/// </summary>
public class AudioManager : Singleton<AudioManager> {

	public GameObject audioPrefs;

	public static event System.Action<bool> OnMusic;
	public static event System.Action<bool> OnEffect;
  	
	private bool? _isMusic = true;
	public bool isMusic {
		get {
      if(_isMusic == null)
        _isMusic = bool.Parse(PlayerPrefs.GetString("audioMusic", "True"));
      return _isMusic.Value;
		}
		set {
			if (_isMusic == value) return;
			_isMusic = value;
			SetSoundParametrs();
			if(OnMusic != null) OnMusic(value);
			PlayerPrefs.SetString("audioMusic", _isMusic.Value.ToString());
		}
	}

	private bool? _isEffects = true;
	public bool isEffect {
		get {
      if (_isEffects == null)
        _isEffects = bool.Parse(PlayerPrefs.GetString("audioEffects", "True"));
      return _isEffects.Value; }
		set {
			if (_isEffects == value) return;
			_isEffects = value;
			SetSoundParametrs();
			if(OnEffect != null) OnEffect(value);
			PlayerPrefs.SetString("audioEffects", _isEffects.Value.ToString());
		}
	}

	[SerializeField]
	private AudioMixerGroup backMusicGroup;

	private AudioSource backGroundMusic;

	[System.Serializable]
	public struct AudioSnapshotParametrs {
		public AudioSnapshotTypes type;
		public AudioMixerSnapshot snapshot;
	}

	[System.Serializable]
	public struct AudioGroupParametrs {
		public AudioMixerTypes type;
		public AudioMixerGroup mixer;
	}

	[SerializeField]
	private AudioSnapshotParametrs[] audioSnapshotParametrs;
	[SerializeField]
	private AudioGroupParametrs[] audioGroupParametrs;

	private List<AudioSnapshotParametrs> audioSnapshotParametrsList = new List<AudioSnapshotParametrs>();
	private List<AudioGroupParametrs> audioGroupParametrsList = new List<AudioGroupParametrs>();

	private List<AudioSource> pooledObjectsEffects  = new List<AudioSource>();
	
	protected override void Awake() {
		base.Awake();
		
		foreach(AudioSnapshotParametrs one in audioSnapshotParametrs) audioSnapshotParametrsList.Add(one);
		foreach(AudioGroupParametrs one in audioGroupParametrs) audioGroupParametrsList.Add(one);
		InitBackMusic();
	}
  
	void Start() {
		SetSoundParametrs();
	}

	#region Настройки

	public void SetSoundParametrs() {
		if(isMusic && isEffect)
			SetSoundMixer(AudioSnapshotTypes.none);
		else if(isMusic && !isEffect)
			SetSoundMixer(AudioSnapshotTypes.stopEffects);
		else if(!isMusic && isEffect)
			SetSoundMixer(AudioSnapshotTypes.stopMusic);
		else
			SetSoundMixer(AudioSnapshotTypes.stopAll);
	}


	/// <summary>
	/// Общая настройка
	/// </summary>
	/// <param name="snapshot">Тип снапшота</param>
	/// <param name="translate">Скорость перехода</param>
	public static void SetSoundMixer(AudioSnapshotTypes snapshot, float translate = 0) {
		Instance.audioSnapshotParametrsList.Find(x => x.type == snapshot).snapshot.TransitionTo(translate);
	}

	public static void HidePercent50Sound(bool flag = true, bool force = false) {
		SetSoundMixer((flag ? AudioSnapshotTypes.runnerEffect50 : AudioSnapshotTypes.runnerEffectDef), (force ? 0 : 1));
		SetSoundMixer((flag ? AudioSnapshotTypes.runnerMusic50 : AudioSnapshotTypes.runnerMusicDef), (force ? 0 : 1));
	}
	public static void HideAllSound() {
		//allOff.TransitionTo(1f);
		SetSoundMixer(AudioSnapshotTypes.stopAll, 1);
	}

	#endregion

	#region Фоновая музыка

	void InitBackMusic() {
		GameObject obj = Instantiate(audioPrefs);
		obj.transform.parent = transform;
		obj.name = "BackGroundMusic";
		backGroundMusic = obj.GetComponent<AudioSource>();
	}
	public static void BackMusic(AudioClip clipPlay, AudioMixerTypes mixer, float value = 1) {
		Instance.backMusic(clipPlay, mixer, value);
	}
	public void backMusic(AudioClip clipPlay, AudioMixerTypes mixer, float value = 1) {
		
		backMusicGroup = audioGroupParametrsList.Find(x => x.type == mixer).mixer;

		if(backGroundMusic.clip != null && clipPlay.name == backGroundMusic.clip.name) {
			if(backMusicGroup != backGroundMusic.outputAudioMixerGroup) {
				backGroundMusic.outputAudioMixerGroup = backMusicGroup;
			}
			return;
		}

		if(backGroundMusic.isPlaying) {
			StopAllCoroutines();
			StartCoroutine(SwitchBackMusic(clipPlay, backMusicGroup, value));
		} else {
			backGroundMusic.clip = clipPlay;
			backGroundMusic.volume = value;
			backGroundMusic.outputAudioMixerGroup = backMusicGroup;
			backGroundMusic.loop = true;
			backGroundMusic.Play();
		}
	}


	public static void StopBackMusic() {
		Instance.stopBackMusic();
	}

	public void stopBackMusic() {
		StartCoroutine(StopBackMusicCor());
	}


	public static void PlayBackMusic() {
		Instance.playBackMusic();
	}

	public void playBackMusic() {
		StartCoroutine(PlayBackMusicCor());
	}

	IEnumerator StopBackMusicCor() {
		while(backGroundMusic.volume > 0.1f) {
			backGroundMusic.volume -= 0.1f;
			yield return new WaitForSeconds(0.1f);
		}
	}

	IEnumerator PlayBackMusicCor() {
		while(backGroundMusic.volume < 1) {
			backGroundMusic.volume += 0.1f;
			yield return new WaitForSeconds(0.1f);
		}
	}

	IEnumerator SwitchBackMusic(AudioClip clip, AudioMixerGroup mixer, float value = 1) {

		while(backGroundMusic.volume > 0.1f) {
			backGroundMusic.volume -= 0.1f;
			yield return new WaitForSeconds(0.1f);
		}

		backGroundMusic.clip = clip;
		backGroundMusic.volume = value;
		backGroundMusic.outputAudioMixerGroup = mixer;
		backGroundMusic.loop = true;
		backGroundMusic.Play();

		while(backGroundMusic.volume < 1) {
			backGroundMusic.volume += 0.1f;
			yield return new WaitForSeconds(0.1f);
		}
	}

	#endregion

	#region Звуковые эффекты

	public static void PlayEffect(AudioClip clipPlay, AudioMixerTypes mixer, float value = 1) {
		Instance._PlayEffect(clipPlay, mixer, value);
	}

	private void _PlayEffect(AudioClip sourceClip, AudioMixerTypes mixer, float value = 1) {

		foreach(AudioSource ready in pooledObjectsEffects) {
			if(!ready.isPlaying) {
				ready.outputAudioMixerGroup = audioGroupParametrsList.Find(x => x.type == mixer).mixer;

				ready.PlayOneShot(sourceClip, value);
				return;
			}
		}

		GameObject obj = Instantiate(audioPrefs);
		obj.transform.parent = transform;
		AudioSource newAudioSource = obj.GetComponent<AudioSource>();
		pooledObjectsEffects.Add(newAudioSource);
		newAudioSource.outputAudioMixerGroup = audioGroupParametrsList.Find(x => x.type == mixer).mixer;
		newAudioSource.PlayOneShot(sourceClip, value);
		return;
	}
	#endregion
}
