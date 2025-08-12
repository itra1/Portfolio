using Game.Providers.Audio.Common;
using Game.Providers.Audio.Settings;
using UnityEngine;
using UnityEngine.Audio;

namespace Game.Providers.Audio.Handlers {
	public class AudioHandler {
		private AudioProviderSettings _audioSettings;
		private AudioSourcePoint.Factory _factory;

		public AudioHandler(AudioProviderSettings audioSettings, AudioSourcePoint.Factory factory) {
			_audioSettings = audioSettings;
			_factory = factory;
		}

		public AudioSourcePoint PlayIndexClip(string groupName, int index) {
			var soundGroup = _audioSettings.GetSoundByName(groupName);
			return PlayClip(soundGroup.GetByIndex(index), soundGroup.Mixer, soundGroup.Volume);
		}

		public AudioSourcePoint PlayRandomClipExclude(string groupName, string exclude) {
			var soundGroup = _audioSettings.GetSoundByName(groupName);

			var playClip = soundGroup.GetRandom();
			var repeatMax = 10;
			var currentIndex = 0;

			while (playClip.name == exclude && currentIndex < repeatMax) {
				currentIndex++;
				playClip = soundGroup.GetRandom();
			}

			return PlayClip(playClip, soundGroup.Mixer, soundGroup.Volume);
		}

		public AudioSourcePoint PlayRandomClip(string groupName) {
			var soundGroup = _audioSettings.GetSoundByName(groupName);
			return PlayClip(soundGroup.GetRandom(), soundGroup.Mixer, soundGroup.Volume);
		}

		public AudioSourcePoint PlayClip(AudioClip clip, AudioMixerGroup amg = null, float volume = 1, bool autoDespawn = true) {
			var point = _factory.Create();
			point.SetMixerGroup(amg);
			point.SetVolume(volume);
			if (autoDespawn)
				return point.PlayAndDespawn(clip);
			else
				return point.Play(clip);
		}
	}
}
