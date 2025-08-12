using System.Collections.Generic;
using Engine.Scripts.Timelines;
using Game.Scripts.Providers.Songs.Helpers;
using Game.Scripts.UI.Components;
using Game.Scripts.UI.Presenters.Base;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zenject;

namespace Game.Scripts.UI.Presenters
{
	/// <summary>
	/// Окно тутора
	/// </summary>
	public class TutorialSelectPresenter : WindowPresenter
	{
		[HideInInspector] public UnityAction<RhythmTimelineAsset> OnPlayEvent;

		[SerializeField] private Button _playButton;
		[SerializeField] private List<SongBigUiElement> _tracks;

		private SongBigUiElement _selectItem;
		private ISongsHelper _songsHelper;

		[Inject]
		private void Constructor(ISongsHelper songsHelper)
		{
			_songsHelper = songsHelper;
			_playButton.onClick.RemoveAllListeners();
			_playButton.onClick.AddListener(PlayButtonTouch);
		}

		public void MakeGrid(List<RhythmTimelineAsset> assetList)
		{
			for (var i = 0; i < _tracks.Count; i++)
			{
				var item = _tracks[i];
				item.SetData(assetList[i], _songsHelper.GetCover(assetList[i].Uuid), 0);
				item.OnClick.AddListener(() =>
				{
					_selectItem = item;
					_playButton.interactable = true;
					SelectItem(_selectItem);
				});
				item.IsSelected = false;
			}
			_playButton.interactable = false;
		}

		private void SelectItem(SongBigUiElement selectedTrack)
		{
			foreach (var elementrack in _tracks)
			{
				elementrack.IsSelected = elementrack == selectedTrack;
			}
		}

		private void PlayButtonTouch()
		{
			OnPlayEvent?.Invoke(_selectItem.Song);
		}
	}
}
