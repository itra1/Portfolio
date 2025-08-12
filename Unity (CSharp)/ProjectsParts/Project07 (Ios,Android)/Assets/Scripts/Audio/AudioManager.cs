using System;
using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;

public enum AudioSnapshotTypes {
	none, musicOn, musicOff, effectOn, effectOff
}
public enum AudioMixerTypes {
	none, music, musicBattle, effect, effectBattle, effectUi
}

/// <summary>
/// Контроллер работы со звуком
/// </summary>
public class AudioManager : Singleton<AudioManager> {

  public static event Action SoundChange;
  public GameObject audioPrefs;

	[SerializeField]
	AudioMixerGroup backMusicGroup;

	public static AudioBlock backMusic {
		get { return Instance._backMusic; }
	}
	
	public AudioBlock _backMusic;
	private AudioPoint _backMusicPoint;

  private string _musicPlayerPrefsKey = "music";
  private string _soundPlayerPrefsKey = "sound";

  public void PlayBackMusic(int trackNum) {

		AudioClipData audioClip = new AudioClipData();


		switch (trackNum) {
			case 1:
				audioClip = library.backMusicMenu;
				break;
			default:
				audioClip = library.backMusicMenu;
				break;
		}

		if (_backMusicPoint == null) {
			_backMusicPoint = PlayEffects(audioClip, AudioMixerTypes.music);
		}
		else {

      if (_backMusicPoint.source.clip == audioClip.audioClip) return;

			_backMusicPoint.ChangeMusic(audioClip);
		}

	}

  public void PlayBackMusic(AudioClipData playData) {
    if (_backMusicPoint == null) {
      _backMusicPoint = PlayEffects(playData);
    } else {

      if (_backMusicPoint.source.clip == playData.audioClip) return;
      _backMusicPoint.ChangeMusic(playData);
    }
  }

  public List<AudioMixerSnapshot> normalPassSnapShotList;

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

  private void Start()
  {
    Load();
    InitBackSound();
  }

  void InitBackSound() {
		_backMusic.Init(this);
	}

	#region Настройки
	private bool _musicValue;
	private bool _soundValue;

  public bool Music
  {
    get { return _musicValue; }
    set
    {
      _musicValue = value;
      SetSoundMixer(_musicValue ? AudioSnapshotTypes.musicOn : AudioSnapshotTypes.musicOff);
      PlayerPrefs.SetString(_musicPlayerPrefsKey, _musicValue.ToString());
      if (SoundChange != null)
        SoundChange();
    }
  }

  public bool Effects
  {
    get { return _soundValue; }
    set
    {
      _soundValue = value;
      SetSoundMixer(_soundValue ? AudioSnapshotTypes.effectOn : AudioSnapshotTypes.effectOff);
      PlayerPrefs.SetString(_soundPlayerPrefsKey, _soundValue.ToString());
      if (SoundChange != null)
        SoundChange();
    }
  }
  
  private void Load()
  {
    Music = PlayerPrefs.HasKey(_musicPlayerPrefsKey)
      ? Boolean.Parse(PlayerPrefs.GetString(_musicPlayerPrefsKey))
      : true;
    Effects = PlayerPrefs.HasKey(_soundPlayerPrefsKey)
      ? Boolean.Parse(PlayerPrefs.GetString(_soundPlayerPrefsKey))
      : true;
  }

	//[ExEvent.ExEventHandler(typeof(ExEvent.GameEvents.OnLoad))]
	//private void OnLoad(ExEvent.GameEvents.OnLoad param) {
	//	music = UserManager.Instance.AudioMisic;
	//	effects = UserManager.Instance.audioEffects;
	//	ConfirmParametrs();
	//}

	[ExEvent.ExEventHandler(typeof(ExEvent.GameEvents.OnChangeEffects))]
	private void OnChangeEffects(ExEvent.GameEvents.OnChangeEffects eventData) {
	  Effects = !Effects;
	}

	[ExEvent.ExEventHandler(typeof(ExEvent.GameEvents.OnChangeMusic))]
	private void OnChangeMusic(ExEvent.GameEvents.OnChangeMusic eventData) {
	  Music = !Music;
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

  #endregion

  #region Звуковые эффекты

  public static AudioPoint PlayEffects(AudioClipData playData, AudioPoint source = null) {
    return Instance.PlayEffects(playData, playData.group, source);
  }

  public static AudioPoint PlayEffects(AudioClipData playData, AudioMixerTypes mixer, AudioPoint source = null) {
    AudioMixerGroup mixerGroup = Instance.audioGroupParametrs.Find(x => x.type == mixer).mixer;
    return Instance.PlayEffects(playData, mixerGroup, source);
    
  }

	public AudioPoint PlayEffects(AudioClipData playData, AudioMixerGroup mixer, AudioPoint source = null) {

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

		sourceAudio.source.outputAudioMixerGroup = mixer;

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
  public AudioMixerGroup group;
  public SpanFloat pitch = new SpanFloat() { min = 1, max = 1 };
	public float pitchValue {
		get { return UnityEngine.Random.Range(pitch.min, pitch.max); }
	}

  public void Play() {
    AudioManager.PlayEffects(this);
  }
  public void PlayBackGround() {
    AudioManager.Instance.PlayBackMusic(this);
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

  public AudioClipData backMusicMenu;
  public AudioClipData backMusicBattle;

}