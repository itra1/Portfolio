using Game.Providers.Audio.Base;
using StringDrop;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Game.Providers.Audio.Settings {
	[Serializable]
	public class AudioProviderSettings {
		public AudioMixerGroup MainAudioGroup;
		public AudioMixerGroup MisicMixerGroup;
		public AudioMixerGroup EffectsMixerGroup;
		public GameObject AudioSourcePrefab;
		public List<SouncsGroup> Sounds;

		public SouncsGroup GetSoundByName(string name) {
			return Sounds.Find(x => x.Name == name);
		}

		[System.Serializable]
		public class SouncsGroup {
			[StringDropList(typeof(SoundNames))]
			public string Name;
			[Range(0, 1)]
			public float Volume = 1;
			public AudioMixerGroup Mixer;
			public List<AudioClip> ClipList;

			public AudioClip GetRandom() {
				return ClipList[UnityEngine.Random.Range(0, ClipList.Count)];
			}
			public AudioClip GetByIndex(int index) {
				return ClipList[index % ClipList.Count];
			}
		}
	}
}
