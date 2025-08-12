using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Engine.Scripts.Base;
using Engine.Scripts.Managers;
using Engine.Scripts.Timelines;
using Game.Scripts.Providers.Songs.Helpers;
using Game.Scripts.UI.Components;
using Game.Scripts.UI.Components.Interfaces;
using Game.Scripts.UI.Presenters.Base;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zenject;

namespace Game.Scripts.UI.Presenters
{
	public class CollectionTracksPresenter : WindowPresenter
	{
		[HideInInspector] public UnityEvent<RhythmTimelineAsset> OnPlayTrackEvent = new();
		[HideInInspector] public UnityEvent OnBackTrackEvent = new();

		[SerializeField] private Button _backButton;
		[SerializeField] private SongUiElement _songButtonPrefab;
		[SerializeField] private SongMenuSeparator _songSeparatorPrefab;
		[SerializeField] private ScrollRect _scrollRect;
		[SerializeField] private Image _background;
		[SerializeField] private Sprite _blueBack;
		[SerializeField] private Sprite _redBack;
		[SerializeField] private FilterButton[] _filters;
		[SerializeField] private DifficultyTrack _startFilter;

		protected List<IMainListItem> m_ButtonsList = new();
		private ISongsHelper _songsHelper;
		protected DiContainer _diContainer;
		protected bool _isOpen;
		protected float _viewHeight;
		protected float _buttonHeight;

		private IRhythmDirector _rhythmDirector;
		private DifficultyTrack _currentFilter;

		public bool IsOpen => _isOpen;

		[Inject]
		public void Constructor(DiContainer diContainer, ISongsHelper songsHelper, IRhythmDirector rhythmDirector)
		{
			_diContainer = diContainer;
			_songsHelper = songsHelper;
			_rhythmDirector = rhythmDirector;

			_backButton.onClick.RemoveAllListeners();
			_backButton.onClick.AddListener(BackButtonTouch);

			SubscribeFiler();
		}

		private void BackButtonTouch()
		{
			OnBackTrackEvent?.Invoke();
		}

		public override async UniTask Show()
		{
			_isOpen = true;
			SetFilter(_startFilter);

			if (_rhythmDirector.PlayableDirector.playableAsset != null)
			{
				if (_rhythmDirector.PlayableDirector.playableAsset is RhythmTimelineAsset)
				{
					//_background.sprite = component.GraphicTemplate.GraphicName switch
					//{
					//	GraphicTemplateNames.Red => _redBack,
					//	GraphicTemplateNames.Blue => _blueBack,
					//	_ => _blueBack
					//};
				}
			}
			await base.Show();
		}

		private void SubscribeFiler()
		{
			for (int i = 0; i < _filters.Length; i++)
			{
				_filters[i].OnClick.AddListener((DifficultyTrack filter) => SetFilter(filter));
			}
		}

		private void SetFilter(DifficultyTrack filter)
		{
			_currentFilter = filter;

			foreach (var item in _filters)
			{
				item.IsSelected = item.DificultyTrack == _currentFilter;
			}

			PopulateScrollView();
		}

		public void PopulateScrollView()
		{
			if (m_ButtonsList.Count > 0)
			{
				for (int i = 0; i < m_ButtonsList.Count; i++)
					Destroy((m_ButtonsList[i] as Component).gameObject);
				m_ButtonsList.Clear();
			}

			_viewHeight = _scrollRect.content.rect.height;
			_buttonHeight = (_songButtonPrefab.transform as RectTransform)?.rect.height ?? 160;

			int allStars = _songsHelper.StarsSum;

			int lastSeparator = allStars;

			var visibleCounds = _songsHelper.GetReadySongs(_currentFilter);

			for (int i = 0; i < visibleCounds.Count; i++)
			{
				var song = visibleCounds[i];

				if (lastSeparator < song.ConditionStar)
				{
					lastSeparator = song.ConditionStar;
					MakeSeparator(allStars, song.ConditionStar);
				}
				SongUiElement songButton = GameObject.Instantiate(_songButtonPrefab, _scrollRect.content);
				_diContainer.Inject(songButton);

				songButton.Initialize(song, _songsHelper.GetCover(song.Uuid), i, OnSongButtonClick, song.ConditionStar <= allStars);
				m_ButtonsList.Add(songButton);
			}
		}

		private void MakeSeparator(int currentSpecialpoint, int conditionSpecialPoint)
		{
			SongMenuSeparator songButton = GameObject.Instantiate(_songSeparatorPrefab, _scrollRect.content);
			songButton.SetValue(currentSpecialpoint, conditionSpecialPoint);

			m_ButtonsList.Add(songButton);
		}

		private void OnSongButtonClick(SongUiElement songButton)
		{
			OnPlayTrackEvent?.Invoke(songButton.Song);
		}

		protected override void PositionContent()
		{
			_positionTransform.anchoredPosition = Vector2.zero;
			_positionTransform.sizeDelta = Vector2.zero;
		}
	}
}
