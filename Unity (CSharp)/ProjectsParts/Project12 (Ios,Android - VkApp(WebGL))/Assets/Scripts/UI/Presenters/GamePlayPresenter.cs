using System.Text;
using Cysharp.Threading.Tasks;
using Game.Scripts.Controllers.Sessions.Common;
using Game.Scripts.Controllers.Sessions.Debugs;
using Game.Scripts.Scoring;
using Game.Scripts.UI.Presenters.Base;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zenject;

namespace Game.Scripts.UI.Presenters
{
	public class GamePlayPresenter : WindowPresenter
	{
		[HideInInspector] public UnityEvent PauseTouchEvent = new();

		[SerializeField] private Button _pauseButton;
		[SerializeField] private TMP_Text _titleLabel;
		[SerializeField] private TMP_Text _scoreLabel;
		[SerializeField] private TMP_Text _gameStatistisLabel;

		private ScoreManager _scoreManager;
		private IDebugSession _debugSession;
		private IBattleSession _battleSession;

		[Inject]
		public void Build(ScoreManager scoreManager, IDebugSession debugSession, IBattleSession battleSession)
		{
			_scoreManager = scoreManager;
			_debugSession = debugSession;
			_battleSession = battleSession;

			_pauseButton.onClick.RemoveAllListeners();
			_pauseButton.onClick.AddListener(PauseButtonTouch);

			_scoreManager.OnScoreVisualChange.AddListener(ScoreChange);
		}

		public override async UniTask Show()
		{
			ScoreChange(0);
			_gameStatistisLabel.gameObject.SetActive(_debugSession.GameStatistic);
			if (_debugSession.GameStatistic)
				_ = UpdateGameStatisticAsync();

			await base.Show();
		}

		private async UniTask UpdateGameStatisticAsync()
		{
			StringBuilder sb = new();

			await UniTask.Yield();

			while (gameObject.activeInHierarchy)
			{
				_ = sb.Clear();
				if (_battleSession.SelectedSong != null)
				{
					_ = sb.Append($"Speed: {_battleSession.SelectedSong.CurrentSpeed}\n");
					_ = sb.Append($"Speed Koefficient: {_battleSession.SelectedSong.SpeedKoefficient}\n");
				}
				_gameStatistisLabel.text = sb.ToString();
				await UniTask.Delay(3000);
			}
		}

		public void PauseButtonTouch() => PauseTouchEvent?.Invoke();

		private void TrackNameChange(string title) =>
			_titleLabel.text = $"<color=#FFFFFF80>Part of track:</color> <font=\"Rubik-Bold SDF\" material=\"Rubik-Bold High\"><color=#FFFFFF>{title}</color></font>";

		private void ScoreChange(float value) =>
			_scoreLabel.text = $"<color=#FFFFFF80>Score:</color> <font=\"Rubik-Bold SDF\" material=\"Rubik-Bold High\"><color=#FFFFFF>{value}</color></font>";

	}
}
