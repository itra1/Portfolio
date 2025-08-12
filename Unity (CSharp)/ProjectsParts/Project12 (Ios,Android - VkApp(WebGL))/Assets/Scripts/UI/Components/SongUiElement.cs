using System;
using System.Collections.Generic;
using Engine.Scripts.Timelines;
using Game.Scripts.Custom;
using Game.Scripts.Providers.Songs.Helpers;
using Game.Scripts.UI.Components.Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Scripts.UI.Components
{
	public class SongUiElement : MonoBehaviour, IMainListItem
	{
		[SerializeField] private TMP_Text _author;
		[SerializeField] private TMP_Text _title;
		[SerializeField] private RawImage _cover;
		[SerializeField] private List<RectTransform> _starsPoint;
		[SerializeField] private Button _playButton;
		[SerializeField] private CanvasGroup _canvasGroup;

		private List<SpecialPointUi> _stars = new();
		private DiContainer _diContainer;
		private ISongsHelper _songHelper;

		public int Index { get; private set; }
		public RhythmTimelineAsset Song { get; private set; }
		public bool IsOpen { get; private set; }
		public bool Interactable
		{
			set
			{
				_canvasGroup.interactable = value;
				_canvasGroup.blocksRaycasts = value;
				_canvasGroup.alpha = value ? 1 : 0.5f;
			}
		}

		private Texture2D _coverPicture;

		[Inject]
		private void Constructor(DiContainer diContainer, ISongsHelper songHelper)
		{
			_diContainer = diContainer;
			_songHelper = songHelper;
		}

		public void Initialize(RhythmTimelineAsset song, Texture2D cover, int index, Action<SongUiElement> onClickAction, bool isOpen)
		{
			Song = song;
			Index = index;
			IsOpen = isOpen;
			Interactable = IsOpen;
			_coverPicture = cover;

			_playButton.onClick.RemoveAllListeners();
			if (isOpen)
				_playButton.onClick.AddListener(() => onClickAction.Invoke(this));

			_cover.texture = _coverPicture;
			_author.text = Song.Authour;
			_title.text = Song.FullName;

			MakeStars();
		}

		private void MakeStars()
		{
			//for (int i = 0; i < _starsPoint.Count; i++)
			//{
			//	var inst = Instantiate(_modeHandler.Active.SpecialPoint.gameObject, _starsPoint[i]);
			//	var component = inst.GetComponent<SpecialPointUi>();
			//	_diContainer.Inject(component);
			//	component.SetGrayBack();
			//	var rt = component.transform as RectTransform;
			//	rt.localScale = Vector2.one;
			//	rt.localPosition = Vector3.zero;
			//	rt.anchorMin = Vector2.zero;
			//	rt.anchorMax = Vector2.one;
			//	rt.sizeDelta = Vector2.zero;
			//	_stars.Add(component);
			//	inst.SetActive(true);
			//	var score = _songHelper.GetScore(Song.Uuid);
			//	component.SetFillValue(i < score.Stars ? 1 : 0);
			//}
		}
	}
}
