using Cysharp.Threading.Tasks;
using Engine.Scripts.Managers;
using Game.Scripts.Controllers.Sessions.Common;
using Game.Scripts.Providers.Songs.Helpers;
using Game.Scripts.Scoring;
using Game.Scripts.UI.Components;
using Game.Scripts.UI.Presenters.Base;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zenject;

namespace Game.Scripts.UI.Presenters
{
	public class GameResultPresenter : WindowPresenter
	{
		[HideInInspector] public UnityEvent OnRetryEvent = new();
		[HideInInspector] public UnityEvent OnContinueEvent = new();
		[HideInInspector] public UnityEvent OnClose = new();

		[SerializeField] private SongBigUiElement _songPanel;
		[SerializeField] private RectTransform _progressLine;
		[SerializeField] private TMP_Text _perfectValueLabel;
		[SerializeField] private TMP_Text _greatValueLabel;
		[SerializeField] private TMP_Text _goodValueLabel;
		[SerializeField] private TMP_Text _pointValueLabel;
		[SerializeField] private Button _retryButton;
		[SerializeField] private Button _continueButton;

		private ScoreManager _scoreManager;
		private IBattleSession _battleSession;
		private ISongsHelper _songsHelper;
		private IUiNavigator _uiHelper;
		private IGameHandler _gameHelper;
		private float _progressWidth;

		[Inject]
		private void Constructor(
		DiContainer diContainer,
		IUiNavigator uiHelper,
		IGameHandler gameHelper,
		ScoreManager scoreManager,
		IBattleSession battleSession,
		ISongsHelper songsHelper
		)
		{
			_uiHelper = uiHelper;
			_gameHelper = gameHelper;
			_scoreManager = scoreManager;
			_battleSession = battleSession;
			_songsHelper = songsHelper;

			_retryButton.onClick.RemoveAllListeners();
			_retryButton.onClick.AddListener(RetryButtonTouch);

			_continueButton.onClick.RemoveAllListeners();
			_continueButton.onClick.AddListener(ContinueButtonTouch);
			diContainer.Inject(_songPanel);
		}

		public override async UniTask<bool> Initialize()
		{
			if (!await base.Initialize())
				return false;

			_progressWidth = _progressLine.sizeDelta.x;
			return true;
		}

		public override async UniTask Show()
		{
			//var score = _songsHelper.GetScore(_scoreManager.CurrentSong.Uuid);

			_perfectValueLabel.text = _scoreManager.GameScore.NoteAccuracyIDCounts[0].ToString();
			_greatValueLabel.text = _scoreManager.GameScore.NoteAccuracyIDCounts[1].ToString();
			_goodValueLabel.text = _scoreManager.GameScore.NoteAccuracyIDCounts[2].ToString();
			PointValueSet(_scoreManager.GameScore.Score);

			_songPanel.SetData(_battleSession.SelectedSong, _songsHelper.GetCover(_battleSession.SelectedSong.Uuid), _scoreManager.GameScore.Stars);

			await base.Show();
		}

		public void PointValueSet(float points)
		{
			_progressLine.sizeDelta = new(_progressWidth * (points / 500), _progressLine.sizeDelta.y);
			_pointValueLabel.text = $"<sprite=6> {points}/<color=#ffffff40>500</color>";
		}

		public void RetryButtonTouch() =>
			OnRetryEvent?.Invoke();

		public void ContinueButtonTouch() =>
			OnContinueEvent?.Invoke();
	}
}
