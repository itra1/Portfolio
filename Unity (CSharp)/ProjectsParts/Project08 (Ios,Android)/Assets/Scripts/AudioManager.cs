using System;
using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;

public enum AudioSnapshotTypes {
	none, allOn, allOff, musicOn, musicOff, effectOn, effectOff
}
public enum AudioMixerTypes {
	none, music, musicPlay, effect, effectPlay, effectUi
}

/// <summary>
/// Контроллер работы со звуком
/// </summary>
public class AudioManager : Singleton<AudioManager> {

	public GameObject audioPrefs;

	[SerializeField]
	AudioMixerGroup backMusicGroup;

	public static AudioBlock backMusic {
		get { return Instance._backMusic; }
	}
	
	public AudioBlock _backMusic;
	private AudioPoint _backMusicPoint;

	public void PlayBackMusic(int trackNum) {

		AudioClipData audioClip = new AudioClipData();

		switch (trackNum) {
			case 1:
				audioClip = library.backMusic1;
				break;
			default:
				audioClip = library.backMusic0;
				break;
		}

		if (_backMusicPoint == null) {
			_backMusicPoint = PlayEffects(audioClip, AudioMixerTypes.music);
		}
		else {
			_backMusicPoint.ChangeMusic(audioClip);
		}

	}

	public List<AudioMixerSnapshot> lowPassSnapShotList;
	public List<AudioMixerSnapshot> normalPassSnapShotList;
	private int setLowPassCount;
	public void SetLowPass(bool isLowPass) {
		setLowPassCount += isLowPass ? 1 : -1;

		if (setLowPassCount > 0)
			lowPassSnapShotList.ForEach(x=>x.TransitionTo(0.5f));
		else
			normalPassSnapShotList.ForEach(x => x.TransitionTo(0.5f));

	}

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

	public List<AudioSnapshotParametrs> audioSnapshotParametrs;
	public List<AudioGroupParametrs> audioGroupParametrs;

	List<AudioPoint> pooledObjectsEffects = new List<AudioPoint>();

	protected override void Awake() {
		base.Awake();
		InitBackSound();
		//library.PlayBackMusic1Audio();
	}

	void InitBackSound() {
		_backMusic.Init(this);
	}

	private void Start() {
		SetSoundParametrs();
	}
	
	#region Настройки
	public static bool music;
	public static bool effects;

	public static void SetSoundParametrs() {
		music = true;
		effects = true;
		ConfirmParametrs();
	}

	[ExEvent.ExEventHandler(typeof(ExEvent.PlayerEvents.OnLoad))]
	private void OnLoad(ExEvent.PlayerEvents.OnLoad param) {
		music = PlayerManager.Instance.audioMisic;
		effects = PlayerManager.Instance.audioEffects;
		ConfirmParametrs();
	}

	[ExEvent.ExEventHandler(typeof(ExEvent.GameEvents.OnChangeEffects))]
	private void OnChangeEffects(ExEvent.GameEvents.OnChangeEffects param) {
		effects = param.isActive;
		ConfirmParametrs();
	}

	[ExEvent.ExEventHandler(typeof(ExEvent.GameEvents.OnChangeMusic))]
	private void OnChangeMusic(ExEvent.GameEvents.OnChangeMusic param) {
		music = param.isActive;
		ConfirmParametrs();
	}

	public static void ToggleMusic() {
		music = !music;
		ConfirmParametrs();
	}
	public static void ToggleEffects() {
		effects = !effects;
		ConfirmParametrs();
	}

	public static void ConfirmParametrs() {
		SetSoundMixer(music ? AudioSnapshotTypes.musicOn : AudioSnapshotTypes.musicOff);
		SetSoundMixer(effects ? AudioSnapshotTypes.effectOn : AudioSnapshotTypes.effectOff);

		if (SoundChange != null) SoundChange();
	}

	public static event Action SoundChange;

	/// <summary>
	/// Общая настройка
	/// </summary>
	/// <param name="snapshot">Тип снапшота</param>
	/// <param name="translate">Скорость перехода</param>
	public static void SetSoundMixer(AudioSnapshotTypes snapshot, float translate = 0) {
		foreach (AudioSnapshotParametrs one in Instance.audioSnapshotParametrs) {
			if (snapshot == one.type) { one.snapshot.TransitionTo(translate); }
		}
	}

	public static void HideAllSound() {
		//allOff.TransitionTo(1f);
		SetSoundMixer(AudioSnapshotTypes.allOff, 1);
	}

	#endregion
	
	#region Звуковые эффекты

	public static AudioPoint PlayEffects(AudioClipData playData, AudioMixerTypes mixer, AudioPoint source = null) {
		return Instance.playEffects(playData, mixer, source);
	}

	public AudioPoint playEffects(AudioClipData playData, AudioMixerTypes mixer, AudioPoint source = null) {

		AudioPoint sourceAudio = null;

		if (source != null && pooledObjectsEffects.Exists(x => x == source)) {
			sourceAudio = source;
		}

		if (sourceAudio == null) {
			foreach (AudioPoint ready in pooledObjectsEffects) {
				if (!ready.isLocked && !ready.isBack && !ready.source.isPlaying) {
					sourceAudio = ready;
					break;
				}
			}
		}

		if (sourceAudio == null) {
			GameObject obj = (GameObject)Instantiate(audioPrefs);
			obj.transform.parent = transform;
			AudioPoint newAudioSource = obj.GetComponent<AudioPoint>();
			pooledObjectsEffects.Add(newAudioSource);
			sourceAudio = newAudioSource;
		}

		sourceAudio.source.outputAudioMixerGroup = audioGroupParametrs.Find(x => x.type == mixer).mixer;

		sourceAudio.source.loop = playData.loop;
		sourceAudio.source.clip = playData.audioClip;
		sourceAudio.source.volume = playData.volume;
		sourceAudio.source.pitch = playData.pitchValue;
		sourceAudio.isLocked = playData.isLocked;
		sourceAudio.source.time = playData.startTimeClip;
		sourceAudio.source.Play();
		sourceAudio.SetData(playData);

		return sourceAudio;
	}
	#endregion

	#region Плей

	public PlayLibrary library;

	#endregion

}

[System.Serializable]
public class AudioClipData {
	public AudioClip audioClip;
	[Range(0, 1)]
	public float volume = 1;
	public bool loop;
	public SpanFloat pitch = new SpanFloat() { min = 1, max = 1 };
	public float pitchValue {
		get { return UnityEngine.Random.Range(pitch.min, pitch.max); }
	}

	public AudioClipData() {
		this.volume = 1;
		pitch.max = 1;
		pitch.min = 1;
	}

	public void PlaySound() { }

	[HideInInspector]
	public float translateTime = 0;
	public float startTimeClip;

	[HideInInspector]
	public bool isLocked;   // Блокировать для воспроизведения
}

[System.Serializable]
public class SpanFloat {
	public float min = 1;
	public float max = 1;
}

[System.Serializable]
public class PlayLibrary {

	public AudioClipData clickAudio;

	public void PlayClickAudio() {
		AudioManager.PlayEffects(clickAudio, AudioMixerTypes.effectUi);
	}

	public AudioClipData clickInactiveAudio;

	public void PlayClickInactiveAudio() {
		AudioManager.PlayEffects(clickInactiveAudio, AudioMixerTypes.effectUi);
	}

	public AudioClipData windowOpenAudio;

	public void PlayWindowOpenAudio() {
		AudioManager.PlayEffects(windowOpenAudio, AudioMixerTypes.effectUi);
	}

	public AudioClipData windowCloseAudio;

	public void PlayWindowCloseAudio() {
		AudioManager.PlayEffects(windowCloseAudio, AudioMixerTypes.effectUi);
	}


	public AudioClipData backMusic0;
	public AudioClipData backMusic1;


	public List<AudioClipData> coinsAudio;

	public void PlayCoinsAudio() {
		AudioManager.PlayEffects(coinsAudio[UnityEngine.Random.Range(0, coinsAudio.Count)], AudioMixerTypes.effectUi);
	}

	public AudioClipData byeAudio;

	public void PlayByeAudio() {
		AudioManager.PlayEffects(byeAudio, AudioMixerTypes.effectUi);
	}

	public AudioClipData getBonusAudio;

	public void PlayGetBonusAudio() {
		AudioManager.PlayEffects(getBonusAudio, AudioMixerTypes.effectUi);
	}

	public AudioClipData giftShowAudio;

	public void PlayGiftShowAudio() {
		AudioManager.PlayEffects(giftShowAudio, AudioMixerTypes.effectUi);
	}

}