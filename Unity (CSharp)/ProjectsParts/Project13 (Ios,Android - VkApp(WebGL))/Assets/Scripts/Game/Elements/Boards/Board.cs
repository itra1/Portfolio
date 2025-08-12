using DG.Tweening;
using Game.Game.Common;
using Game.Game.Elements.Barriers;
using Game.Game.Elements.Interfaces;
using Game.Game.Elements.Weapons;
using Game.Game.Settings;
using Game.Providers.Audio.Handlers;
using Game.Providers.Battles.Interfaces;
using Game.Providers.Ui;
using Game.Providers.Ui.Controllers;
using Game.Providers.Ui.Popups;
using StringDrop;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Game.Game.Elements.Boards
{
	public class Board : MonoBehaviour, IBoard, IPoolable<BoardsSettings.BoardItem, IMemoryPool>, IWeaponCollision
	{

		[HideInInspector] public UnityEvent OnDestroy = new();

		[StringDropList(typeof(BoardNames))] public string Type;
		[SerializeField] private BoardBarriers _boardBarriers;
		[SerializeField] private Transform _body;
		[SerializeField] private Transform _itemsParent;
		[SerializeField] private Transform _model;
		[SerializeField] private Collider2D _collider;
		[SerializeField] private BoardRotation _rotator;
		[SerializeField] private Material _defaultMaterial;
		[SerializeField] private Material _transparentMaterial;

		private GameSettings _gameSettings;
		private BoardsSettings.BoardItem _boardItem;
		private IBoardStageSettings _stageData;
		private AudioHandler _audioHandler;
		private SignalBus _signalBus;
		private PopupProvider _popupProvider;
		private GameSession _gameSession;
		private IUiProvider _uiProvider;
		private bool _waitDestroy;

		public Transform ItemsParent => _itemsParent;
		public string GameCollisionType => GameCollisionName.Board;
		public float Speed { get; set; }
		public BoardRotation Rotator => _rotator;
		public Transform Body => _body;

		[Inject]
		public void Initiate(
			SignalBus signalBus,
			GameSettings gameSettings,
			AudioHandler audioHandler,
			PopupProvider popupProvider,
			GameSession gameSession,
			IUiProvider uiProvider
		)
		{
			_gameSettings = gameSettings;
			_audioHandler = audioHandler;
			_signalBus = signalBus;
			_popupProvider = popupProvider;
			_gameSession = gameSession;
			_uiProvider = uiProvider;
			Speed = _gameSettings.Speed;
		}

		public void OnEnable()
		{
			_collider.enabled = false;
			transform.localScale = Vector3.zero;
			_ = transform.DOScale(Vector3.one, 0.2f).OnComplete(() =>
			{
				_collider.enabled = true;
			});
			_waitDestroy = false;
			_rotator.enabled = true;
			_rotator.RandomRotate();
		}

		public void OnDisable()
		{
			OnDestroy.RemoveAllListeners();
		}

		public void KnockOut()
		{
		}

		public void WeaponHit(Weapon knife)
		{
			_ = _audioHandler.PlayRandomClip(_boardItem.HitSound);
		}

		[ContextMenu("Destroy")]
		public void Destroy(bool isPlayer = true)
		{
			if (_waitDestroy)
			{
				return;
			}
			//_signalBus.Fire(new BoardDestroySignal(isPlayer));
			_ = _audioHandler.PlayRandomClip(_boardItem.CrashSound);
			_collider.enabled = false;
			_waitDestroy = true;
			_transparentMaterial.SetFloat("_Opacity", 1);
			_rotator.enabled = false;

			var gamePlayController = _uiProvider.GetController<GamePlayDuelWindowPresenterController>();

			if (isPlayer)
			{
				//_ = _popupProvider.GetPopup(PopupsNames.YouWin).Show();
				//_ = gamePlayController.ShowWin();
			}
			else
			{
				//_ = _popupProvider.GetPopup(PopupsNames.YouLoss).Show();
				//_ = gamePlayController.ShowLoss();
			}

			OnDestroy?.Invoke();
			_ = DOTween.To(() => _transparentMaterial.GetFloat("_Opacity"), (x) => _transparentMaterial.SetFloat("_Opacity", x), 0, 0.5f).SetDelay(0.5f).OnComplete(() =>
			{

				gameObject.SetActive(false);
			});
		}

		public void OnDespawned()
		{
		}

		public void SetData(BoardsSettings.BoardItem boardItem)
		{
			_boardItem = boardItem;
		}

		public void SetData(IBoardStageSettings stageData)
		{
			_stageData = stageData;
			ConfirmData();
		}

		public void OnSpawned(BoardsSettings.BoardItem borderItem, IMemoryPool pool)
		{
			_boardItem = borderItem;
			ConfirmData();
		}

		public void ConfirmData()
		{
			_boardBarriers.PositingBarriers(_stageData);
		}

		public class Factory : PlaceholderFactory<BoardsSettings.BoardItem, Board> { }

		[System.Serializable]
		public struct StartRectTransform
		{
			public Rigidbody Rigidbody;
			public Transform Transform;
			public Vector3 LocalPosition;
			public Vector3 LocalScale;
			public Vector3 LocalRotation;
		}
	}
}
