using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Engine.Scripts.Common.Interfaces;
using Engine.Scripts.Managers;
using Engine.Scripts.Settings;
using Engine.Scripts.Settings.Common;
using Engine.Scripts.Timelines;
using Engine.Scripts.Timelines.Notes.Base;
using Engine.Scripts.Timelines.Notes.Common;
using Game.Scripts.Controllers.AccuracyLabels;
using Game.Scripts.Providers.Profiles.Handlers;
using Game.Scripts.Providers.Songs.Helpers;
using Game.Scripts.Providers.Songs.Saves;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Game.Scripts.Scoring
{
	public class ScoreManager : IScoreManager, IInitializable
	{
		public UnityEvent<RhythmTimelineAsset, SongScoreData> OnNewHighScore { get; set; } = new();
		public UnityEvent<Note, INoteAccuracy> OnNoteScore { get; set; } = new();
		public UnityEvent OnBreakChain { get; set; } = new();
		public UnityEvent<int> OnContinueChain { get; set; } = new();
		public UnityEvent<float> OnScoreChange { get; set; } = new();
		public UnityEvent<float> OnScoreVisualChange { get; set; } = new();
		public UnityEvent<int, float> OnStarChange { get; set; } = new();
		public UnityEvent OnNotesComplete { get; set; } = new();

		private readonly IRhythmDirector _rhythmDirector;
		private readonly IDspTime _dspTime;
		private readonly ISongsHelper _songsHelper;
		private readonly IProfileStarsHandler _profileScoreHandler;
		private readonly IScorePerSong _scorePerSong;
		private readonly IAccuracyController _accuracyController;
		private readonly ISceneAccuracy _sceneAccuracyParent;

		private float _currentScore;
		private List<int> _currentAccuracyIDHistogram;
		private int _currentMaxChain;
		private int _currentChain;
		private float _currentMaxPossibleScore;
		private int _currentStars;

		private float _scoreMultiplier = 1;
		private CancellationTokenSource _multimlayerCTS;

		private int _allNotes;
		private int _maxPoint;
		private int _oneStarPoint;
		private double _lastPopup = 0;

		public int UseScore => Mathf.RoundToInt(_currentScore / _maxPoint * _scorePerSong.ScorePerSong);
		public static ScoreManager Instance { get; private set; }
		public RhythmTimelineAsset CurrentSong { get; private set; }
		public SongScoreData GameScore { get; private set; }
		private Transform AccuracySpawnPoint => _sceneAccuracyParent.AccuracySpawnPoint;

		public ScoreManager(
			IRhythmDirector rhythmDirector,
			ISceneAccuracy sceneAccuracyParent,
			IDspTime dspTime,
			IProfileStarsHandler profileScoreHandler,
			ISongsHelper songsHelper,
			IScorePerSong scorePerSong,
			IAccuracyController accuracyController
		)
		{
			Instance = this;
			_sceneAccuracyParent = sceneAccuracyParent;
			_rhythmDirector = rhythmDirector;
			_dspTime = dspTime;
			_songsHelper = songsHelper;
			_profileScoreHandler = profileScoreHandler;
			_scorePerSong = scorePerSong;
			_accuracyController = accuracyController;

			_rhythmDirector.OnSongPlay.AddListener(HandleOnSongPlay);
			_rhythmDirector.OnSongEnd.AddListener(HandleOnSongEnd);
			_rhythmDirector.RhythmProcessor.OnNoteTriggerEvent.AddListener(HandleOnNoteTriggerEvent);
		}

		public void Initialize()
		{
			UpdateScoreVisual(false);

			if (_rhythmDirector.IsPlaying)
				HandleOnSongPlay();
		}

		private void HandleOnNoteTriggerEvent(NoteTriggerEventData noteTriggerEventData)
		{
			if (noteTriggerEventData.EventType == NoteEventType.Early)
				return;

			var noteAccuracy = AddNoteAccuracyScore(
					noteTriggerEventData.Note,
					noteTriggerEventData.DspTimeDifferencePercentage,
					noteTriggerEventData.EventType == NoteEventType.Miss);

			var spawnTransform = AccuracySpawnPoint != null
					? AccuracySpawnPoint
					: noteTriggerEventData.Note.RhythmClipData.TrackObject.EndPoint;

			ScorePopup(spawnTransform, noteAccuracy);
		}

		private void HandleOnSongEnd()
		{
			OnSongEnd(_rhythmDirector.SongTimelineAsset);
		}

		private void HandleOnSongPlay()
		{
			SetSong(_rhythmDirector.SongTimelineAsset);
		}

		public void SetSong(RhythmTimelineAsset song)
		{
			CurrentSong = song;
			_currentScore = 0;

			_currentMaxPossibleScore = _scorePerSong.ScorePerSong;
			_allNotes = song.RhythmClipCount;

			if (_currentMaxPossibleScore < 0)
			{
				_currentMaxPossibleScore = _allNotes * _accuracyController.MaxNoteScore;
			}

			_maxPoint = song.RhythmClipCount * 5;
			_oneStarPoint = (int) Mathf.Ceil(_maxPoint * 0.85f) / 5;

			_currentAccuracyIDHistogram = new List<int>(CurrentSong.RhythmClipCount);
			_currentMaxChain = 0;
			_currentChain = 0;

			_scoreMultiplier = 1;

			UpdateScoreVisual(false);
		}

		public void OnSongEnd(RhythmTimelineAsset song)
		{
			if (_currentAccuracyIDHistogram == null)
				SetSong(song);

			var songScore = _songsHelper.GetScore(song.Uuid);
			GameScore ??= _songsHelper.GetEmptyScore();

			GameScore.Stars = _currentStars;
			GameScore.Score = UseScore;
			GameScore.MaxChain = _currentMaxChain;
			GameScore.NoteAccuracyIDHistogram = _currentAccuracyIDHistogram;
			GameScore.Recalc();

			var newStars = _currentStars - songScore.Stars;
			var newScore = songScore.Stars < _currentStars || songScore.Score < _currentScore;

			if (newScore)
			{
				songScore.Stars = _currentStars;
				songScore.Score = _currentScore;
				songScore.MaxChain = _currentMaxChain;
				songScore.NoteAccuracyIDHistogram = _currentAccuracyIDHistogram;

				if (newStars > 0)
					_profileScoreHandler.AddStars(newStars);
				OnNewHighScore?.Invoke(song, songScore);
				songScore.Recalc();
			}
		}

		public INoteAccuracy GetAccuracy(float offsetPercentage, bool miss) => miss ? GetMissAccuracy() : GetAccuracy(offsetPercentage);

		public INoteAccuracy GetAccuracy(float offsetPercentage)
		{
			var noteAccuracy = _accuracyController.GetNoteAccuracy(offsetPercentage);

			if (noteAccuracy == null)
			{
				Debug.LogWarningFormat("Note Accuracy could not be found for offset ({0}), make sure the score settings are correctly set up", offsetPercentage);
				return null;
			}

			return noteAccuracy;
		}

		public INoteAccuracy GetMissAccuracy() => _accuracyController.GetMissAccuracy();

		public virtual INoteAccuracy AddNoteAccuracyScore(Note note, float offsetPercentage, bool miss)
		{
			var noteAccuracy = GetAccuracy(offsetPercentage, miss);
			AddNoteAccuracyScore(note, noteAccuracy);
			return noteAccuracy;
		}

		public virtual void AddNoteAccuracyScore(Note note, INoteAccuracy noteAccuracy)
		{
			if (noteAccuracy == null)
			{
				Debug.LogWarningFormat("Note Accuracy is null");
				return;
			}

			if (noteAccuracy.BreakChain)
			{
				_currentChain = 0;
				OnBreakChain?.Invoke();
			}
			else
			{
				_currentChain++;
				_currentMaxChain = Mathf.Max(_currentChain, _currentMaxChain);
				OnContinueChain?.Invoke(_currentChain);
			}

			AddScore(noteAccuracy.Score);
			OnNoteScore?.Invoke(note, noteAccuracy);

			if (CurrentSong == null)
				return;

			_currentAccuracyIDHistogram.Add(_accuracyController.GetID(noteAccuracy));
		}

		public virtual void AddScore(float score)
		{
			if (score < 0)
			{
				_currentScore += score;
			}
			else
			{
				_currentScore += _scoreMultiplier * score;
			}

			if (CurrentSong.PreventMaxScoreOvershoot)
			{
				_currentScore = Mathf.Min(_currentScore, _currentMaxPossibleScore);
			}

			if (CurrentSong.PreventMinScoreOvershoot)
			{
				_currentScore = Mathf.Max(_currentScore, 0);
			}

			OnScoreChange?.Invoke(_currentScore);
			UpdateScoreVisual(true);
		}

		public void SetMultiplier(float multiplier)
		{
			_scoreMultiplier = multiplier;
			UpdateScoreVisual(false);
		}

		public void SetMultiplier(float multiplier, float time)
		{
			SetMultiplier(multiplier);

			if (_multimlayerCTS != null)
			{
				if (!_multimlayerCTS.IsCancellationRequested)
					_multimlayerCTS.Cancel();
			}
			_ = ResetMultiplierDelayed(time);
		}

		public async UniTask ResetMultiplierDelayed(float delay)
		{

			_multimlayerCTS = new();

			try
			{
				var start = _dspTime.AdaptiveTime;
				await UniTask.WaitWhile(() => start + delay > _dspTime.AdaptiveTime, cancellationToken: _multimlayerCTS.Token);
				SetMultiplier(1);
			}
			catch (OperationCanceledException)
			{
				_multimlayerCTS.Dispose();
				_multimlayerCTS = null;
			}
		}

		public float GetChainPercentage()
		{
			var maxScore = CurrentSong.RhythmClipCount;
			var percentage = 100 * _currentChain / maxScore;
			return percentage;
		}

		public float GetMaxChainPercentage()
		{
			var maxScore = CurrentSong.RhythmClipCount;
			var percentage = 100 * _currentMaxChain / maxScore;
			return percentage;
		}

		public int GetChain() => _currentChain;
		public float GetMaxChain() => _currentMaxChain;
		public float GetScore() => _currentScore;
		public float GetScorePercentage() => _currentScore * 100 / _currentMaxPossibleScore;

		public void UpdateScoreVisual(bool fromDestroy)
		{
			OnScoreVisualChange?.Invoke(UseScore);

			int starCount = (int) (_currentScore / _oneStarPoint);
			starCount = Mathf.Min(starCount, 5);
			_currentStars = starCount;
			float oneStart = (_currentScore % _oneStarPoint) / _oneStarPoint;
			OnStarChange?.Invoke(starCount, oneStart);

			if (fromDestroy)
			{
				_allNotes--;

				if (_allNotes == 0)
				{
					_ = DOVirtual.DelayedCall(2, () => OnNotesComplete?.Invoke());
				}
			}
		}

		public virtual void ScorePopup(Transform spawnPoint, INoteAccuracy noteAccuracy)
		{
			if (noteAccuracy.Active && Time.realtimeSinceStartupAsDouble - _lastPopup > 0.2)
			{
				//Pop(noteAccuracy.popPrefab, spawnPoint);
				//_lastPopup = Time.realtimeSinceStartupAsDouble;
				_accuracyController.SpawnLabel(noteAccuracy.Type);
			}
		}

		//public virtual void Pop(GameObject prefab, Transform spawnPoint)
		//{
		//	_ = PoolManager.Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
		//}
	}
}
