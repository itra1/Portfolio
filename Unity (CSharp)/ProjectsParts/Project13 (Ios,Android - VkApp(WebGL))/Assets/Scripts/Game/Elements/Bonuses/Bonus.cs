using System;
using Cysharp.Threading.Tasks;
using Game.Game.Common;
using Game.Game.Elements.Boards;
using Game.Game.Elements.Interfaces;
using Game.Game.Elements.Weapons.Common;
using UnityEngine;
using Zenject;

namespace Game.Game.Elements.Bonuses
{
	public abstract class Bonus : MonoBehaviour, IFormationItem, IWeaponTrigger
	{

		public Action OnDestroy;

		[SerializeField] private int _points = 200;
		[SerializeField] private Rigidbody2D _rigibody;
		[SerializeField] private Collider2D _collider;
		[SerializeField] private ParticleSystem _particleSystem;
		[SerializeField] private SpriteRenderer _graphic;

		private SignalBus _signalBus;
		private GameSession _gameSession;
		//private PointsModificatorHandler _pointsModificatorHandler;
		private bool _isReady;
		public string State { get; set; }

		public string FormationType => GameCollisionName.Bonus;

		public abstract string FormationSubType { get; }

		[Inject]
		public void Constructor(SignalBus signalBus,
		//PointsModificatorHandler pointsModificatorHandler
		GameSession gameSession
		)
		{
			_signalBus = signalBus;
			_gameSession = gameSession;
			//_pointsModificatorHandler = pointsModificatorHandler;
		}

		public void OnEnable()
		{
			_isReady = true;
			_graphic.gameObject.SetActive(true);
		}

		public void OnWeaponTrigger()
		{
			if (!_isReady)
				return;
			_isReady = false;

			OnDestroy?.Invoke();

			var addPoints = Mathf.RoundToInt((float) (Math.Round(_gameSession.Modificator, 1) * _points));

			_gameSession.Points += (int) addPoints;
			//_signalBus.Fire(new LevelPointsChangeSignal(_gameSession.Points));

			//_pointsModificatorHandler.Increment();

			_graphic.gameObject.SetActive(false);
			_particleSystem.Play();
			_ = UniTask.Create(async () =>
			{
				await UniTask.Delay(300);
				gameObject.SetActive(false);
			});
		}
		public void FixedOnBoard(Board board)
		{
			State = WeaponStates.Lock;
			_rigibody.bodyType = RigidbodyType2D.Kinematic;
			_rigibody.simulated = true;
			var currentParent = transform.parent;
			transform.SetParent(board.ItemsParent);
			transform.localPosition = transform.up * 0.93f;
			board.OnDestroy.AddListener(() =>
			{
				transform.SetParent(currentParent);
				BoardDestroy(board);
			});
		}

		private void BoardDestroy(Board board)
		{
			State = WeaponStates.Out;
			_rigibody.bodyType = RigidbodyType2D.Dynamic;
			_rigibody.velocity = Vector2.zero;
			_rigibody.gravityScale = 1;
			_rigibody.simulated = true;
			transform.SetParent(null);
			_rigibody.angularVelocity = UnityEngine.Random.Range(-360, 360);
			_rigibody.AddForce(((Vector2) transform.position - (Vector2) board.transform.position).normalized * UnityEngine.Random.Range(7f, 10f), ForceMode2D.Impulse);
			_ = UniTask.Create(async () =>
			{
				await UniTask.Delay(900);
				gameObject.SetActive(false);
			});
		}
	}
}
