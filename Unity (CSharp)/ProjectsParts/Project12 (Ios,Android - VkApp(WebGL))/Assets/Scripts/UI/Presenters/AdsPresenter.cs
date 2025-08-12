using Game.Scripts.UI.Presenters.Base;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Scripts.UI.Presenters
{
	public class AdsPresenter : WindowPresenter
	{
		[SerializeField] private Button _playButton;
		[SerializeField] private Button _skipButton;

		[Inject]
		public void Constructor()
		{
			_playButton.onClick.RemoveAllListeners();
			_playButton.onClick.AddListener(PlaybuttonTouch);

			_skipButton.onClick.RemoveAllListeners();
			_skipButton.onClick.AddListener(SkipButtonTouch);
		}

		private void SkipButtonTouch()
		{
		}

		private void PlaybuttonTouch()
		{
		}
	}
}
