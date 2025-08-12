using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

namespace SoundPoint
{
	public interface IAudioPoint
	{
		public UnityEvent OnPlay { get; set; }
		public UnityEvent OnComplete { get; set; }

		public bool IsPlaying { get; }
		public float Pitch { get; }
		public float Volume { get; }

		public IAudioPoint Play(AudioClip clip);
		public IAudioPoint PlayRandom(AudioClip[] clip);
		public IAudioPoint AutoDespawn();
		public IAudioPoint Stop();
		public UniTask WaitComplete(CancellationToken cancellationToken = default);
		public IAudioPoint SetPitch(float value = 1);
		public IAudioPoint SetVolume(float value = 1);
		public IAudioPoint SetMute(bool value);
		public IAudioPoint SetOutputAudioMixerGroup(AudioMixerGroup value);
	}
}
