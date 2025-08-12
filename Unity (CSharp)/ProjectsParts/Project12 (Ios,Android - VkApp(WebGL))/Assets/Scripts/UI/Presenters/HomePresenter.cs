using Cysharp.Threading.Tasks;
using Engine.Scripts.Managers;
using Engine.Scripts.Timelines;
using Game.Scripts.Providers.Songs.Helpers;
using Game.Scripts.UI.Components;
using Game.Scripts.UI.Controllers.Base;
using Game.Scripts.UI.Presenters.Base;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zenject;

namespace Game.Scripts.UI.Presenters
{
	public class HomePresenter : WindowPresenter, IDialogAllowed
	{

		[HideInInspector] public UnityEvent<RhythmTimelineAsset> PlayTouchEvent = new();
		[HideInInspector] public UnityEvent ShopTouchEveny = new();
		[HideInInspector] public UnityEvent RatingTouchEvent = new();
		[HideInInspector] public UnityEvent CollectionTouchEvent = new();
		[HideInInspector] public UnityEvent ProfileTouchEvent = new();

		[SerializeField] private Carousel _carousel;
		[SerializeField] private Button _collectionButton;
		[SerializeField] private Button _playButton;
		[SerializeField] private Button _shopButton;
		[SerializeField] private Button _ratingButton;
		[SerializeField] private Button _profileButton;

		private RhythmTimelineAsset _selectedItem;
		private IGameHandler _gameHelper;
		private ISongsHelper _songHelper;

		[Inject]
		public void Build(IGameHandler gameHelper, ISongsHelper songHelper)
		{
			_gameHelper = gameHelper;
			_songHelper = songHelper;
		}

		public override async UniTask<bool> Initialize()
		{
			if (!await base.Initialize())
				return false;
			_collectionButton.onClick.AddListener(CollectionButtonTouch);
			_playButton.onClick.AddListener(PlayButtonTouch);
			_shopButton.onClick.AddListener(ShopButtonTouch);
			_ratingButton.onClick.AddListener(RatingButtonTouch);
			_profileButton.onClick.AddListener(ProfileButtonTouch);

			_playButton.interactable = true;
			_carousel.OnDragEvent.AddListener((isDrag) => _playButton.interactable = !isDrag);
			_carousel.OnSelectItem.AddListener((item) => _selectedItem = item);

			_carousel.SpawnItems();

			return true;
		}

		private void RatingButtonTouch() => RatingTouchEvent?.Invoke();
		private void ShopButtonTouch() => ShopTouchEveny?.Invoke();
		private void PlayButtonTouch() => PlayTouchEvent?.Invoke(_selectedItem);
		private void CollectionButtonTouch() => CollectionTouchEvent?.Invoke();
		private void ProfileButtonTouch() => ProfileTouchEvent?.Invoke();

		public override async UniTask Show()
		{
			await base.Show();
			_carousel.Open();
			_carousel.SelectAndMoveElement(_selectedItem ?? _songHelper.GetReadySongs()[0]);
		}

		protected override void PositionContent()
		{
			_positionTransform.anchoredPosition = Vector2.zero;
			_positionTransform.sizeDelta = Vector2.zero;
		}
	}
}
