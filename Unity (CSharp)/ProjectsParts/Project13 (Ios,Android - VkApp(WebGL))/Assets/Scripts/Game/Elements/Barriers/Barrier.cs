using Cysharp.Threading.Tasks;
using Game.Game.Elements.Boards;
using Game.Game.Elements.Interfaces;
using Game.Game.Elements.Weapons;
using Game.Game.Elements.Weapons.Common;
using Game.Providers.Audio.Base;
using Game.Providers.Audio.Handlers;
using UnityEngine;
using Zenject;

namespace Game.Game.Elements.Barriers
{
	public abstract class Barrier : MonoBehaviour, IWeaponCollision, IFormationItem
	{
		[SerializeField] private Rigidbody2D _rigibody;
		[SerializeField] private Collider2D _collider;

		private SignalBus _signalBus;
		private AudioHandler _audioHandler;

		public string GameCollisionType => GameCollisionName.Barrier;
		public string State { get; set; }
		public string FormationType => GameCollisionName.Barrier;

		public abstract string FormationSubType { get; }

		[Inject]
		public void Constructor(SignalBus signalBus, AudioHandler audioHandler)
		{
			_signalBus = signalBus;
			_audioHandler = audioHandler;
		}

		private void OnEnable()
		{
			//_signalBus.Subscribe<BarrierColliderSignal>(OnBarrierColliderSignal);
		}

		private void OnDisable()
		{
			//_signalBus.Unsubscribe<BarrierColliderSignal>(OnBarrierColliderSignal);
			_collider.enabled = true;
		}

		public void KnockOut()
		{
		}

		//private void OnBarrierColliderSignal(BarrierColliderSignal signal)
		//{

		//	if (State != WeaponStates.Lock)
		//		return;

		//	_collider.enabled = signal.ColliderEnable;
		//}

		public void FixedOnBoard(Board board)
		{
			State = WeaponStates.Lock;
			ClearVelocity();
			_rigibody.bodyType = RigidbodyType2D.Kinematic;
			_rigibody.simulated = true;
			_collider.isTrigger = false;
			_rigibody.velocity = Vector2.zero;
			var currentParent = transform.parent;
			transform.SetParent(board.ItemsParent);
			transform.localPosition = Vector3.up * -0.93f;
			board.OnDestroy.AddListener(() =>
			{
				transform.SetParent(currentParent);
				FlyOut(board);
			});
		}

		public void ClearVelocity()
		{
			_rigibody.velocity = Vector2.zero;
			_rigibody.angularVelocity = 0;
		}

		private void FlyOut(Board board)
		{
			State = WeaponStates.Out;
			_rigibody.bodyType = RigidbodyType2D.Dynamic;
			_rigibody.velocity = Vector2.zero;
			_rigibody.gravityScale = 1;
			_rigibody.simulated = true;
			_collider.isTrigger = true;
			_rigibody.angularVelocity = Random.Range(-360, 360);
			_rigibody.AddForce(((Vector2) transform.position - (Vector2) board.transform.position).normalized * Random.Range(7f, 10f), ForceMode2D.Impulse);
			_ = UniTask.Create(async () =>
			{
				await UniTask.Delay(2000);
				gameObject.SetActive(false);
			});
		}

		public void WeaponHit(Weapon knife)
		{
			_ = _audioHandler.PlayRandomClip(SoundNames.BorderHit);
		}
	}
}
