using Cysharp.Threading.Tasks;
using DG.Tweening;
using Engine.Engine.Scripts.Managers.Interfaces;
using Engine.Scripts.Common.Interfaces;
using Engine.Scripts.Managers;
using Engine.Scripts.Timelines;
using Engine.Scripts.Timelines.Notes.Common;
using Game.Scripts.Controllers.Sessions.Common;
using Game.Scripts.Helpers;
using Game.Scripts.Managers.Base;
using Game.Scripts.Providers.Songs.Helpers;
using Game.Scripts.Scoring;
using Game.Scripts.Settings.Common;
using Game.Scripts.UI;
using Game.Scripts.UI.Controllers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using Zenject;

namespace Game.Scripts.Managers
{
	public class GameHandler : IGameHandler
	{
		public UnityEvent<RhythmTimelineAsset, bool> OnGameChangeEvent { get; set; } = new();

		private readonly DiContainer _diContainer;
		private readonly IRhythmProcessor _rhythmProcessor;
		private readonly IRhythmDirector _rhythmDirector;
		private readonly ISongsHelper _songHelper;
		private readonly ScoreManager _scoreManager;
		private readonly SceneModeHelper _sceneModeHelper;
		private readonly IPauseHandler _pauseHelper;
		private readonly IUiNavigator _uiNavigator;
		private readonly ISceneAudioSources _sceneAudioSources;
		private readonly ISession _session;
		private readonly IGamePlaying _gamePlaying;
		private readonly IBattleSession _battleSession;
		private bool _restarting;
		private AudioSource _audioSource;
		private Tween _audioSourceMute;

		private SplashPresenterController _splashController;
		private GamePlayPresenterController _gamePlayController;
		private GameResultPresenterController _gameResultController;
		private HomePresenterController _homeController;


		public GameHandler(
			DiContainer diContainer,
			IRhythmProcessor rhythmProcessor,
			IRhythmDirector rhythmDirector,
			ISongsHelper songHelper,
			ScoreManager scoreManager,
			SceneModeHelper sceneModeHelper,
			IPauseHandler pauseHelper,
			IUiNavigator uiHelper,
			ISceneAudioSources sceneAudioSources,
			ISession session,
			IBattleSession battleSession,
			IGamePlaying gamePlaying
		)
		{
			_diContainer = diContainer;
			_rhythmProcessor = rhythmProcessor;
			_rhythmDirector = rhythmDirector;
			_songHelper = songHelper;
			_scoreManager = scoreManager;
			_sceneModeHelper = sceneModeHelper;
			_pauseHelper = pauseHelper;
			_uiNavigator = uiHelper;
			_sceneAudioSources = sceneAudioSources;
			_session = session;
			_gamePlaying = gamePlaying;
			_battleSession = battleSession;

			_audioSource = _sceneAudioSources.AudioSources[0];

			_rhythmDirector.OnSongEnd.AddListener(OnSongEnd);
			_scoreManager.OnNotesComplete.AddListener(NotesComplete);

			_rhythmProcessor.OnNoteTriggerEvent.AddListener(NoteTriggerEven);
		}

		~GameHandler()
		{
			_rhythmDirector.OnSongEnd.RemoveListener(OnSongEnd);
			_scoreManager.OnNotesComplete.RemoveListener(NotesComplete);
		}

		public void PlaySong(string songUuid)
		{
			_rhythmDirector.ActiveAudioSource.pitch = 1;
			_ = PlaySong(_songHelper.GetReadySong(songUuid));
		}

		private void InitPresenters()
		{
			_splashController ??= _uiNavigator.GetController<SplashPresenterController>();
			_gamePlayController ??= _uiNavigator.GetController<GamePlayPresenterController>();
			_gameResultController ??= _uiNavigator.GetController<GameResultPresenterController>();
			_homeController ??= _uiNavigator.GetController<HomePresenterController>();
		}

		public async UniTask PlaySong(RhythmTimelineAsset song)
		{
			InitPresenters();

			_diContainer.Inject(song);

			_uiNavigator.ClearStack();
			_ = await _splashController.Open();
			_uiNavigator.ClearStack();
			_ = _gamePlayController.Open();

			ResetState();
			_battleSession.SelectedSong = song;

			await UniTask.Delay(300);
			_ = await _splashController.Close();

			_ = Play();
		}

		private void NoteTriggerEven(NoteTriggerEventData noteTriggerEventData)
		{
			switch (noteTriggerEventData.EventType)
			{
				case NoteEventType.Miss:
				case NoteEventType.Early:
					EndSong();
					break;
			}
		}

		private void ResetState()
		{
			_ = _rhythmDirector.ActiveAudioSource.outputAudioMixerGroup.audioMixer.SetFloat("PitchParam", 1);
			_audioSourceMute?.Kill();
			_audioSource.volume = 1;
			_rhythmDirector.PlayableDirector.RebuildGraph();
			_sceneModeHelper.SetMode(_session.SceneVisibleMode == SceneVisibleModeType.Orthographic);
		}

		private async UniTask Play()
		{
			_battleSession.SelectedSong.ResetSpeedKoefficient();
			_gamePlaying.IsPlaying = true;

			await _rhythmDirector.PlaySong(_battleSession.SelectedSong);
			OnGameChangeEvent?.Invoke(_battleSession.SelectedSong, true);
		}

		public void OnSongEnd()
		{
			if (!_gamePlaying.IsPlaying)
				return;
			if (!Application.isPlaying)
				return;

			_gamePlaying.IsPlaying = false;

			if (_restarting)
				return;

			_ = OnSongEndAsync();
		}

		private async UniTask OnSongEndAsync()
		{
			try
			{
				if (_rhythmDirector.PlayableDirector.playableGraph.IsValid())
				{
					var playable = _rhythmDirector.PlayableDirector.playableGraph.GetRootPlayable(0);
					float timeStop = 2;

					float getPinch()
					{
						return _rhythmDirector.ActiveAudioSource.outputAudioMixerGroup.audioMixer.GetFloat("PitchParam", out var result) ? result : 1;
					}
					void setPinch(float value)
					{
						_ = _rhythmDirector.ActiveAudioSource.outputAudioMixerGroup.audioMixer.SetFloat("PitchParam", value);
					}

					_ = DOTween.To(() => playable.GetSpeed(), (x) => playable.SetSpeed(x), 0, timeStop);
					await DOTween.To(() => getPinch(), (x) => setPinch(x), 0, timeStop).ToUniTask();
				}
			}
			catch (System.Exception ex)
			{
				AppLog.LogError($"Stop sound track error {ex.Message} {ex.StackTrace}");
			}

			_scoreManager.OnSongEnd(_battleSession.SelectedSong);
			_ = ShowEndGame();
			OnGameChangeEvent?.Invoke(_battleSession.SelectedSong, false);
		}

		/// <summary>
		/// Конец
		/// </summary>
		public void EndSong()
		{
			if (_session.GameMissMode == GameMissModeType.Ignore)
				return;


			if (_session.GameMissMode == GameMissModeType.Rollback)
			{
				Rollback();
				return;
			}

			_rhythmDirector.EndSong();
			//_ = _uiHelper.OpenWindow(WindowPresenterNames.GameResult);
			//_pauseHelper.UnPause();
		}

		public async UniTask RestartSong()
		{
			_restarting = true;

			//var currentSong = SelectedSong;
			//EndSong();
			//_ = _gamePlayController.Open(true, true);
			_ = await _gameResultController.Close();
			_ = Play();
			ResetState();
			//_ = PlaySong(currentSong);

			_restarting = false;
		}

		private void NotesComplete()
		{
			_gamePlaying.IsPlaying = false;
			_scoreManager.OnSongEnd(_battleSession.SelectedSong);
			_ = ShowEndGame();

			_audioSourceMute = DOTween.To(() => _audioSource.volume, (x) => _audioSource.volume = x, 0, 3)
			.OnComplete(EndSong);
		}

		private void Rollback()
		{
			_rhythmDirector.Rollback();
		}

		public async UniTask ShowEndGame()
		{
			InitPresenters();
			if (!_gamePlayController.WindowPresenter.IsVisible)
				return;

			_ = await _gameResultController.Open();

			_gameResultController.OnRepeatEvent = () =>
			{
				_ = RestartSong();
			};
			_gameResultController.OnContinueEvent = () =>
			{
				_ = ToMenu();
				//_ = await _uiHelper.GetController<RewardFakePresenterController>().Open(false, false);
			};
		}

		private async UniTask ToMenu()
		{
			_rhythmDirector.ClearPlayableAsset();

			_uiNavigator.ClearStack();
			_ = await _splashController.Open();
			_ = _gameResultController.Close();
			_ = _gamePlayController.Close();
			_uiNavigator.ClearStack();
			_ = await _homeController.Open();
			await UniTask.Delay(300);
			_ = await _splashController.Close();
		}
	}
}
