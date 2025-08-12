using Cysharp.Threading.Tasks;
using DG.Tweening;
using Scripts.GameItems.Platforms;
using SoundPoint;
using System.Threading;
using UnityEngine;
using Zenject;

namespace Scripts.Common.Blobs {
	public class Blob : MonoBehaviour, IPoolable<Color, IPlatform, IMemoryPool> {

		[SerializeField] private Sprite[] _sprites;
		[SerializeField] private SpriteRenderer _spriteRenderer;
		[SerializeField] private AudioClip[] _spawnClips;
		private static int _lastSpawnClipIndex = -1;

		private IPlatform _parentPlatform;
		private IMemoryPool _pool;
		private CancellationTokenSource _platformDestroyCancelationTS;
		private bool _isSpawn;
		private IAudioPointFactory _audioPointFactory;

		[Inject]
		public void Constructor(IAudioPointFactory audioPointFactory) {
			_audioPointFactory = audioPointFactory;
		}

		public void OnDespawned() {
		}

		public void OnSpawned(Color p1, IPlatform p2, IMemoryPool p3) {
			_pool = p3;
			_isSpawn = true;
			SetData(p1, p2);
			SpawnClipSound();
		}

		private void SpawnClipSound() {

			if (_spawnClips.Length == 0)
				return;

			int indexClip = _lastSpawnClipIndex;
			while (_spawnClips.Length > 2 && indexClip == _lastSpawnClipIndex)
				indexClip = Random.Range(0, _spawnClips.Length);

			_ = _audioPointFactory.Create()
			.AutoDespawn()
			.SetPitch(Random.Range(0.88f, 1.12f))
			.Play(_spawnClips[indexClip]);

			_lastSpawnClipIndex = indexClip;
		}

		public void SetData(Color targetColor, IPlatform parentPlatform) {

			if (parentPlatform == null)
				return;

			var color = targetColor;

			_parentPlatform = parentPlatform;
			_parentPlatform.DestroyEvent.AddListener(OnParentPlatformDestroy);
			_spriteRenderer.sprite = _sprites[Random.Range(0, _sprites.Length)];

			if (_platformDestroyCancelationTS != null) {
				_platformDestroyCancelationTS.Dispose();
				_platformDestroyCancelationTS = null;
			}

			_platformDestroyCancelationTS ??= new();

			_spriteRenderer.color = color;
			color.a = 0;
			_ = _spriteRenderer.DOColor(color, 1).OnComplete(() => OnParentPlatformDestroy()).SetDelay(2).ToUniTask(cancellationToken: _platformDestroyCancelationTS.Token);
		}

		private void OnParentPlatformDestroy() {
			if (!_isSpawn)
				return;
			_parentPlatform.DestroyEvent.RemoveListener(OnParentPlatformDestroy);
			if (_platformDestroyCancelationTS != null && !_platformDestroyCancelationTS.IsCancellationRequested) {
				_platformDestroyCancelationTS.Cancel();
				_platformDestroyCancelationTS.Dispose();
				_platformDestroyCancelationTS = null;
			}
			gameObject.transform.SetParent(null);
			_isSpawn = false;
			_pool.Despawn(this);
		}

		public class Factory : PlaceholderFactory<Color, IPlatform, Blob> { }
	}
}
