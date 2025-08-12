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

	public static event System.Action SoundChange;
	public GameObject audioPrefs;


  private string m_backgroundSound = "";

	[SerializeField]
	private AudioMixerGroup backMusicGroup;

	private bool _isMusic;
	public bool IsMusic {
		get { return _isMusic; }
		set {
			_isMusic = value;
			Save();
			ConfirmParametrs();
			if (SoundChange != null) SoundChange();
		}
	}
	private bool _isEffects;

	public bool IsEffects {
		get { return _isEffects; }
		set {
			_isEffects = value;
			Save();
			ConfirmParametrs();
			if (SoundChange != null) SoundChange();
		}
	}
	
	public AudioBlock _backMusic;
	private static AudioBlock backMusic {
		get { return Instance._backMusic; }
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
	}

	private void Start() {
		Load();
	}

  /// <summary>
  /// Зфпуск бакгройнд музыки
  /// </summary>
  /// <param name="name"></param>
  public void PlayBackGroundSound(string name) {
    if (m_backgroundSound == name)
      return;
    m_backgroundSound = name;
    DarkTonic.MasterAudio.MasterAudio.TriggerPlaylistClip("PlaylistController", m_backgroundSound);
  }
	
	void InitBackSound() {
		_backMusic.Init(this);
	}

	public void SetDefaultParametrs() {
		IsMusic = true;
		IsEffects = true;
	}
	
	private void Load() {

		if (!PlayerPrefs.HasKey("sound")) {
			IsMusic = true;
			IsEffects = true;
		}
		else {
			SaveData sd = JsonUtility.FromJson<SaveData>(PlayerPrefs.GetString("sound"));
			IsMusic = sd.music;
			IsEffects = sd.effects;
		}

		ConfirmParametrs();
	}

	public struct SaveData {
		public bool music;
		public bool effects;
	}

	private void Save() {
		PlayerPrefs.SetString("sound",JsonUtility.ToJson(new SaveData() {
			music = this.IsMusic,
			effects = this.IsEffects
		}));
	}

	public void ToggleMusic() {
		this.IsMusic = !this.IsMusic;
	}

	public void ToggleEffect() {
		this.IsEffects = !this.IsEffects;
	}

	public void ConfirmParametrs() {

		if (IsMusic && IsEffects) {
			SetSoundMixer(AudioSnapshotTypes.allOn,0);
		}else if (!IsMusic && IsEffects) {
			SetSoundMixer(AudioSnapshotTypes.musicOff, 0);
		} else if (IsMusic && !IsEffects) {
			SetSoundMixer(AudioSnapshotTypes.effectOff, 0);
		}
		else {
			SetSoundMixer(AudioSnapshotTypes.allOff, 0);
		}
	}
	
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

	public void PlaySound() {}

	[HideInInspector]
	public float translateTime = 0;
	public float startTimeClip;

	[HideInInspector]
	public bool isLocked;   // Блокировать для воспроизведения
}