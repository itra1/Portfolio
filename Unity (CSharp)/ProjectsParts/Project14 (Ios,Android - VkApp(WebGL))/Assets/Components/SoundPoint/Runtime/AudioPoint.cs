using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using Zenject;

namespace SoundPoint
{

	[RequireComponent(typeof(AudioSource))]
	public class AudioPoint : MonoBehaviour, IPoolable<IMemoryPool>, IAudioPoint
	{

		public UnityEvent OnPlay { get; set; } = new();
		public UnityEvent OnComplete { get; set; } = new();

		[SerializeField] private AudioSource _soundSource;

		private IMemoryPool _pool;

		public bool IsPlaying => _soundSource.isPlaying;
		public float Pitch => _soundSource.pitch;
		public float Volume => _soundSource.volume;

		public void OnDespawned()
		{
			_soundSource.pitch = 1;
			_soundSource.volume = 1;
			if (IsPlaying)
				_soundSource.Stop();
		}

		public void OnSpawned(IMemoryPool pool)
		{
			_pool = pool;
		}

		private void Despawn()
		{
			_pool.Despawn(this);
		}

		public IAudioPoint Play(AudioClip clip)
		{
			_soundSource.clip = clip;
			_soundSource.Play();
			return this;
		}

		public IAudioPoint PlayRandom(AudioClip[] clip)
		{
			_ = Play(clip[Random.Range(0, clip.Length)]);
			return this;
		}

		public IAudioPoint AutoDespawn()
		{
			_ = WaitCompleteAndDespawn();
			return this;
		}

		public IAudioPoint SetPitch(float value = 1)
		{
			_soundSource.pitch = value;
			return this;
		}

		public IAudioPoint SetVolume(float value = 1)
		{
			_soundSource.pitch = value;
			return this;
		}

		public IAudioPoint SetMute(bool value)
		{
			_soundSource.mute = value;
			return this;
		}

		public IAudioPoint SetOutputAudioMixerGroup(AudioMixerGroup value)
		{
			_soundSource.outputAudioMixerGroup = value;
			return this;
		}

		public async UniTask WaitComplete(CancellationToken cancellationToken = default)
		{
			await UniTask.WaitUntil(() => IsPlaying, cancellationToken: cancellationToken);
			await UniTask.WaitUntil(() => !IsPlaying, cancellationToken: cancellationToken);
		}

		private async UniTask WaitCompleteAndDespawn()
		{
			await WaitComplete();
			Despawn();
		}

		public IAudioPoint Stop()
		{
			if (_soundSource.isPlaying)
				_soundSource.Stop();
			return this;
		}

		public class Factory : PlaceholderFactory<AudioPoint>, IAudioPointFactory { }
	}

	public interface IAudioPointFactory : IFactory<AudioPoint> { }
}
