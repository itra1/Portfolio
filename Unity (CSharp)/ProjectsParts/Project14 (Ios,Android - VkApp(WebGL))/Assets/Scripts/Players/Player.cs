using System;
using System.Threading;
using Core.Engine.Elements.Player;
using Cysharp.Threading.Tasks;
using Game.Players.Skins;
using Scripts.Common;
using Scripts.GameItems.Platforms;
using Scripts.Signals;
using SoundPoint;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Scripts.Players
{
	/// <summary>
	/// Базовый класс игрока
	/// </summary>
	public class Player :MonoBehaviour
	{
		[SerializeField] private PlayerMeshRotator _meshGO;
		[SerializeField] private ParticleSystem _particleSystem;
		[SerializeField] private PlayerSkinController _skinController;
		[SerializeField] private AudioClip[] _fireSounds;

		public UnityAction OnPlayerDamage;
		public UnityAction OnComplete;

		private PlayerMove _playerMove;
		private SignalBus _signalBus;
		private BlobSpawner _blobSpawner;
		private IAudioPointFactory _audioPointFactory;
		private IAudioPoint _audioPointFire;
		private Vector3 _startPosition;
		private LevelData _levelData;
		private DiContainer _diController;
		private Platform _lastPlatform;
		private int _fireClipIndex = -1;
		private int _gateOpenSetia = 0;
		private bool _isEmition;
		private CancellationTokenSource _fireAudioCancellationTokenSource;

		public SkinSound SkinSound { get; set; }
		public Color BlobColor { get; set; }

		public bool IsMove
		{
			get
			{
				if (_playerMove == null)
					_playerMove = GetComponent<PlayerMove>();

				return _playerMove.enabled;
			}
			set
			{
				if (_playerMove == null)
					_playerMove = GetComponent<PlayerMove>();

				_playerMove.enabled = value;
			}
		}

		public PlayerMeshRotator MeshGO { get => _meshGO; set => _meshGO = value; }

		[Inject]
		public void Initiate(SignalBus signalBus, DiContainer diController, BlobSpawner blobSpawner, IAudioPointFactory audioPointFactory)
		{
			_signalBus = signalBus;
			_blobSpawner = blobSpawner;
			_audioPointFactory = audioPointFactory;
			_diController = diController;
			_diController.Inject(_skinController);
			_audioPointFire = _audioPointFactory.Create();
		}

		private void Awake()
		{
			IsMove = false;
			_startPosition = transform.position;
			EmissionDisable();
		}

		public void SetLevelData(LevelData ld)
		{
			_levelData = ld;
		}

		public void ResetCountGame()
		{
			_gateOpenSetia = 0;
			EmissionDisable();
		}
		public void AddCountGame()
		{
			_gateOpenSetia++;

			if (DestroyplatformInSeria())
			{
				EmissionEnable();
			}
		}

		private bool DestroyplatformInSeria()
		{
			return _gateOpenSetia >= 3;
		}

		public void ResetPosition()
		{
			_playerMove.Clear();
			transform.position = _startPosition;
			_meshGO.enabled = true;
			_lastPlatform = null;
		}
		private void EmissionEnable()
		{
			if (_isEmition)
				return;
			_isEmition = true;
			var emis = _particleSystem.emission;
			emis.rateOverTime = 300;
			_ = AudioFire();
		}
		private void EmissionDisable()
		{
			_isEmition = false;
			var emis = _particleSystem.emission;
			emis.rateOverTime = 0;

			if (_fireAudioCancellationTokenSource != null && !_fireAudioCancellationTokenSource.IsCancellationRequested)
				_fireAudioCancellationTokenSource.Cancel();
		}

		private async UniTask AudioFire()
		{
			_fireAudioCancellationTokenSource = new();

			try
			{
				while (!_fireAudioCancellationTokenSource.IsCancellationRequested)
				{
					_ = _audioPointFire.SetVolume(1);
					var oldIndex = _fireClipIndex;
					while (oldIndex == _fireClipIndex)
						_fireClipIndex = UnityEngine.Random.Range(0, _fireSounds.Length);

					_ = _audioPointFire.Play(_fireSounds[_fireClipIndex]);
					await _audioPointFire.WaitComplete(_fireAudioCancellationTokenSource.Token);
				}
			}
			catch (OperationCanceledException)
			{
				_fireAudioCancellationTokenSource.Dispose();
				_ = _audioPointFire.Stop();
			}
		}

		private void OnCollisionEnter(Collision collision)
		{
			if (collision.collider.TryGetComponent<IPlayerFinish>(out _))
			{
				PlayerFinish();
				if (SkinSound != null)
					this.SkinSound.Win();
				return;
			}

			if (collision.collider.TryGetComponent<IPlayerDamage>(out _))
			{
				PlayerDamage();
				if (SkinSound != null)
					this.SkinSound.Damage();

				return;
			}

			_meshGO.NewRotate();

			if (collision.collider.TryGetComponent<PlatformElement>(out var collisionPlatform))
			{
				var isDestroy = DestroyplatformInSeria();
				ResetCountGame();

				if (isDestroy)
				{
					var playerCrossing = collision.collider.GetComponentInParent<IPlayerCrossing>();
					playerCrossing?.PlayerCrossing();
				}
				else
				{
					var platform = collisionPlatform.GetComponentInParent<Platform>();
					_blobSpawner.Spawn(BlobColor, collision.GetContact(0).point, platform);

					if (SkinSound != null)
					{
						if (_lastPlatform != platform)
							SkinSound.JumpNext();
						else
							SkinSound.Jump();
					}
					_lastPlatform = platform;
				}
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.TryGetComponent<IPlayerCrossing>(out var playerCrossing))
			{
				playerCrossing.PlayerCrossing();
				_signalBus.Fire(new PlatformCrossingSignal() { PlatformsAll = _levelData.PlatformCount, PlatformIndex = playerCrossing.Index });
				AddCountGame();
			}
		}

		/// <summary>
		/// Получение урона игроком
		/// </summary>
		private void PlayerDamage()
		{
			IsMove = false;
			_meshGO.enabled = false;
			EmissionDisable();
			OnPlayerDamage?.Invoke();
		}

		/// <summary>
		/// Окончание уровня
		/// </summary>
		private void PlayerFinish()
		{
			IsMove = false;
			_meshGO.enabled = false;
			EmissionDisable();
			OnComplete?.Invoke();
		}
	}
}