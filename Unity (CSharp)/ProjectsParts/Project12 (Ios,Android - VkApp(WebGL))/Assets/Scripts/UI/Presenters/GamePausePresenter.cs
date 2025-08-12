using Game.Scripts.Scoring;
using Game.Scripts.UI.Presenters.Base;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zenject;

namespace Game.Scripts.UI.Presenters
{
	public class GamePausePresenter : WindowPresenter
	{
		[HideInInspector] public UnityEvent OnCloseEvent = new();

		[SerializeField] private TMP_Text _scoreLabel;
		[SerializeField] private TMP_Text _partTrackLabel;
		[SerializeField] private Button _closeButton;

		private ScoreManager _scoreManager;

		[Inject]
		public void Build(ScoreManager scoreManager)
		{
			_scoreManager = scoreManager;

			_closeButton.onClick.RemoveAllListeners();
			_closeButton.onClick.AddListener(CloseButtonTouch);

			_scoreManager.OnScoreVisualChange.AddListener(ScoreChange);
		}

		private void CloseButtonTouch() => OnCloseEvent?.Invoke();

		private void TrackNameChange(string title) =>
			_partTrackLabel.text = $"<color=#FFFFFF80>Part of track:</color> <font=\"Rubik-Bold SDF\" material=\"Rubik-Bold High\"><color=#FFFFFF>{title}</color></font>";

		private void ScoreChange(float value) =>
			_scoreLabel.text = $"<color=#FFFFFF80>Score:</color> <font=\"Rubik-Bold SDF\" material=\"Rubik-Bold High\"><color=#FFFFFF>{value}</color></font>";
	}
}
