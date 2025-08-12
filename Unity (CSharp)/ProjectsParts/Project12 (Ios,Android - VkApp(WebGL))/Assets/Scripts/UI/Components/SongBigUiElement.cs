using System.Collections.Generic;
using Engine.Scripts.Base;
using Engine.Scripts.Timelines;
using Game.Scripts.Custom;
using Game.Scripts.Providers.Songs.Helpers;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zenject;

namespace Game.Scripts.UI.Components
{
	public class SongBigUiElement : MonoBehaviour, IInjection
	{
		[HideInInspector] public UnityEvent OnClick = new();

		[SerializeField] private TMP_Text _author;
		[SerializeField] private TMP_Text _title;
		[SerializeField] private RawImage _coverImage;
		[SerializeField] private CanvasGroup _canvasGroup;
		[SerializeField] private GameObject _standartBackground;
		[SerializeField] private GameObject _selectBackground;
		[SerializeField] private List<SpecialPointUi> _starsList;

		private RhythmTimelineAsset _song;
		private bool _isSelected;
		private ISongsHelper _songHelper;
		private Texture2D _cover;

		public RectTransform RectTransform => transform as RectTransform;
		public CanvasGroup CanvasGroup => _canvasGroup;
		public RhythmTimelineAsset Song => _song;

		[Inject]
		private void Constructor(ISongsHelper songHelper)
		{
			_songHelper = songHelper;
		}

		private void OnEnable()
		{
			FillStars();

			if (gameObject.TryGetComponent<Button>(out var button))
			{
				button.onClick.RemoveAllListeners();
				button.onClick.AddListener(() => OnClick?.Invoke());
			}
		}

		public bool IsSelected
		{
			get => _isSelected; set
			{
				_isSelected = value;
				_standartBackground.SetActive(!_isSelected);
				_selectBackground.SetActive(_isSelected);
			}
		}

		public void SetData(RhythmTimelineAsset song, Texture2D cover, int stars)
		{
			_song = song;
			_author.text = _song.Authour;
			_title.text = _song.FullName;
			_cover = cover;
			_coverImage.texture = _cover;
			IsSelected = false;

			FillStars(stars);

			if (_coverImage.TryGetComponent<AspectRatioFitter>(out var arf))
			{
				arf.aspectRatio = (float) _coverImage.texture.width / (float) _coverImage.texture.height;
			}
			FillStars(stars);
		}

		public void SetCoverRaycastListener(bool value)
		{
			_coverImage.raycastTarget = value;
		}

		private void FillStars(int? stars = null)
		{
			if (_song == null)
				return;

			stars ??= _songHelper.GetScore(_song.Uuid).Stars;
			for (int i = 0; i < _starsList.Count; i++)
				_starsList[i].SetFillValue(i < stars ? 1 : 0);
		}
	}
}
