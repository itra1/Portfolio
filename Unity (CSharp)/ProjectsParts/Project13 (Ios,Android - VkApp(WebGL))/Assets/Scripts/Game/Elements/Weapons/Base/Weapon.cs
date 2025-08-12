using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Game.Common;
using Game.Game.Elements.Boards;
using Game.Game.Elements.Interfaces;
using Game.Game.Elements.Scenes;
using Game.Game.Elements.Weapons.Common;
using Game.Game.Handlers;
using Game.Providers.Audio.Base;
using Game.Providers.Audio.Handlers;
using Game.Providers.Audio.Settings;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Game.Game.Elements.Weapons
{
	public abstract partial class Weapon : MonoBehaviour, IWeapon, IWeaponCollision
	{
		//public UnityAction OnShootAction;
		public UnityAction<IWeapon> OnBoardEvent { get; set; }
		public UnityAction<IWeapon> OnWeaponRecapturedEvent { get; set; }
		public UnityAction<IWeapon> OnKnockOut { get; set; }
		public UnityAction<IWeapon> OnLossHit { get; set; }

		[SerializeField] protected LayerMask _boardMask;
		[SerializeField] protected SpriteRenderer _renderer;
		[SerializeField] private Sprite icone;
		[SerializeField] private bool _stickInBoard;
		[SerializeField] protected float _shootSpeed = 40;
		[SerializeField] private bool _shootIsDynamic = true;

		protected SignalBus _signalBus;
		protected WeaponHelper _weaponHelper;
		protected AudioHandler _audioHandler;
		protected IGameScene _gameScene;
		protected AudioProviderSettings _audioSettings;
		protected GameSession _gameSession;
		protected Rigidbody2D _rigibody;
		protected Collider2D _collider;

		public abstract string Type { get; }
		public Transform Transform => transform;
		public string State { get; set; }
		public bool IsShootReady => State == WeaponStates.Ready;
		public string GameCollisionType => GameCollisionName.Weapon;
		public Sprite Icone => icone;
		//public int DestroyKnifeCount => _destroyKnifeCount;
		public bool StickInBoard => _stickInBoard;
		public Vector3 ShootVector { get; set; } = Vector3.up;

		[Inject]
		public void Constructor(
			SignalBus signalBus,
			WeaponHelper knifeHelper,
			AudioProviderSettings audioSettings,
			AudioHandler audioHandler,
			GameSession gameSession,
			IGameScene gameScene
		)
		{
			_signalBus = signalBus;
			_weaponHelper = knifeHelper;
			_audioSettings = audioSettings;
			_audioHandler = audioHandler;
			_gameSession = gameSession;
			_gameScene = gameScene;

			_rigibody = GetComponent<Rigidbody2D>();
			_collider = GetComponent<Collider2D>();
		}

		private void OnEnable()
		{
			//_signalBus.Subscribe<BarrierColliderSignal>(OnBarrierColliderSignal);
			SetOnBoard(true);
		}

		private void OnDisable()
		{
			//_signalBus.Unsubscribe<BarrierColliderSignal>(OnBarrierColliderSignal);
			_collider.enabled = true;
		}

		private void Update()
		{
			if (State == WeaponStates.Shoot)
			{
				if (!_shootIsDynamic)
					MoveForwardVelocity();
			}
		}

		private void FixedUpdate()
		{
			if (State == WeaponStates.Shoot)
			{
				if (_shootIsDynamic)
					SetForwardVelocity();
			}
		}

		public virtual void OnSpawned()
		{
			_renderer.color = Color.white;
			SetOnBoard(false);
			_collider.enabled = true;
		}

		public virtual void SetMode(int mode) { }

		public void WeaponHit(Weapon knife)
		{
			_ = _audioHandler.PlayRandomClip(SoundNames.KnifeHit);
		}

		public async UniTask ShootReady()
		{
			State = WeaponStates.Ready;
			ReadyState();
			Color sourceColor = _renderer.color;
			Color sourceColorTransparent = sourceColor;
			sourceColorTransparent.a = 0;

			_renderer.color = sourceColorTransparent;


			await _renderer.DOColor(sourceColor, 0.25f).ToUniTask();
		}

		public virtual void Shoot()
		{
			if (!IsShootReady)
				return;

			var color = _renderer.color;
			color.a = 1;
			_renderer.color = color;
			State = WeaponStates.Shoot;
			ShootState();

			if (!_shootIsDynamic)
			{
				_collider.isTrigger = true;
			}

			SetForwardVelocity();
			MoveForwardVelocity();
		}

		protected virtual void SetForwardVelocity()
		{
			if (_shootIsDynamic)
				_rigibody.velocity = ShootVector * _shootSpeed;
		}

		private void MoveForwardVelocity()
		{
			if (!_shootIsDynamic)
				transform.Translate(ShootVector * _shootSpeed * Time.deltaTime);
		}

		protected virtual void WeaponCollision()
		{
			//_signalBus.Fire(new WeaponHitSignal(false));
			_weaponHelper.OnBarrier();
			FlyOut();
		}

		protected virtual void FlyOut()
		{
			FlyCollider();
			_rigibody.AddTorque(Random.Range(-5, 5), ForceMode2D.Impulse);
		}

		public void Remove()
		{
			State = WeaponStates.Out;
			gameObject.SetActive(false);
			_rigibody.bodyType = RigidbodyType2D.Dynamic;
			_rigibody.gravityScale = 1;
			_rigibody.simulated = true;
			_collider.isTrigger = true;
			transform.SetParent(null);
			_collider.enabled = true;
		}

		protected virtual void FlyCollider()
		{
			State = WeaponStates.Out;
			FreeState();
			transform.SetParent(null);

			Vector2 moveDirection = (transform.position - _gameScene.BoardPoint.position).normalized;

			var rotateAngle = Random.Range(-600f, 600f);

			moveDirection = MathHelper.RotateVector(moveDirection, Random.Range(-90f, 90f));
			float speed = Random.Range(3f, 7f);

			_ = _renderer.DOColor(new Color(1, 1, 1, 0), 1f)
			.OnUpdate(() =>
			{
				transform.Rotate(0, 0, rotateAngle * Time.deltaTime);
				transform.position += (Vector3) moveDirection * speed * Time.deltaTime;
			})
			.OnComplete(() =>
			{
				gameObject.SetActive(false);
			});
		}

		private void HitOnBoard(Board board)
		{
			//_signalBus.Fire(new WeaponHitSignal(true));
			OnBoardEvent?.Invoke(this);
			if (_stickInBoard)
				FixedOnBoard(board);
			_weaponHelper.OnBoardAsync(this);
		}

		public void WeaponRecaptured()
		{
			OnWeaponRecapturedEvent?.Invoke(this);
		}

		protected void FixedOnBoard(Board board)
		{
			State = WeaponStates.Lock;
			FixedState();
			SetOnBoard(true);
			var currentParent = transform.parent;
			transform.SetParent(board.ItemsParent);
			transform.localPosition = transform.localPosition.normalized * 1.1f;
			_ = UniTask.Create(async () =>
			{
				await UniTask.Yield();
				transform.localPosition = transform.localPosition.normalized * 1.1f;
			});
			ClearVelocity();
			board.OnDestroy.AddListener(() =>
			{
				transform.SetParent(currentParent);
				BoardDestroy(board);
			});
			transform.position = transform.parent.position + (transform.position - transform.parent.position).normalized * 1.1f;
		}

		protected void ClearVelocity()
		{
			_rigibody.velocity = Vector2.zero;
			_rigibody.angularVelocity = 0;
		}

		public void CriticalHit()
		{
			OnLossHit?.Invoke(this);
		}

		protected void BoardDestroy(Board board)
		{
			FlyCollider();
			_rigibody.velocity = Vector2.zero;
			_rigibody.AddForce(((Vector2) transform.position - (Vector2) board.transform.position).normalized * Random.Range(7f, 10f), ForceMode2D.Impulse);
			_rigibody.angularVelocity = Random.Range(-60, 60);
		}

		protected virtual void SetOnBoard(bool isBoard) { }

		public void KnockOut()
		{
			OnKnockOut?.Invoke(this);
			FlyOut();
		}

		protected virtual void ReadyState()
		{
			_rigibody.bodyType = RigidbodyType2D.Kinematic;
			_rigibody.gravityScale = 0;
			_rigibody.simulated = false;
			_collider.isTrigger = false;
		}

		protected virtual void ShootState()
		{
			_rigibody.bodyType = _shootIsDynamic ? RigidbodyType2D.Dynamic : RigidbodyType2D.Kinematic;
			_rigibody.simulated = true;
		}

		protected virtual void FreeState()
		{
			_rigibody.bodyType = RigidbodyType2D.Kinematic;
			_rigibody.gravityScale = 0;
			_rigibody.simulated = false;
			_collider.isTrigger = false;
			_collider.enabled = false;
		}

		protected virtual void FixedState()
		{
			_rigibody.bodyType = RigidbodyType2D.Kinematic;
			_rigibody.simulated = true;
			_collider.isTrigger = false;
			_collider.enabled = false;
			ClearVelocity();
		}
	}
}
