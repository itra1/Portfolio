using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;
using Zenject;

namespace Game.Providers.Audio.Common
{
	public class AudioSourcePoint : MonoBehaviour, IPoolable<IMemoryPool>
	{
		[SerializeField] private AudioSource _soundSource;

		private IMemoryPool _pool;
		private AudioClip _clip;
		private bool _autoDespawn;

		public string ClipName => _clip.name;

		public void OnSpawned(IMemoryPool pool)
		{
			_pool = pool;
		}

		private void Despawn()
		{
			_pool.Despawn(this);
		}

		public void OnDespawned()
		{

		}

		public AudioSourcePoint SetMixerGroup(AudioMixerGroup mixerGroup)
		{
			_soundSource.outputAudioMixerGroup = mixerGroup;
			return this;
		}

		public AudioSourcePoint SetVolume(float volume)
		{
			_soundSource.volume = volume;
			return this;
		}

		public AudioSourcePoint Play(AudioClip clip)
		{
			_clip = clip;
			_soundSource.clip = _clip;
			_soundSource.Play();
			return this;
		}

		public AudioSourcePoint PlayAndDespawn(AudioClip clip)
		{
			_clip = clip;
			_soundSource.clip = clip;
			_soundSource.Play();
			WaitCompleteAndDespawn().Forget();
			return this;
		}

		private async UniTask WaitCompleteAndDespawn()
		{
			await UniTask.WaitUntil(() => _soundSource.isPlaying);
			await UniTask.WaitUntil(() => !_soundSource.isPlaying);
			Despawn();
		}

		public class Factory : PlaceholderFactory<AudioSourcePoint> { }
	}
}
