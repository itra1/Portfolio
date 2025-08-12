//using System;
//using Cysharp.Threading.Tasks;
//using Game.Game.Common;
//using Game.Game.Elements.Boards;
//using Game.Game.Elements.Scenes;
//using Game.Game.Settings;
//using Game.Game.Signals;
//using Game.Providers.Avatars;
//using Game.Providers.Battles.Helpers;
//using Game.Providers.Nicknames;
//using Game.Providers.Profile;
//using Game.Providers.Ui;
//using Game.Providers.Ui.Controllers;
//using Game.Providers.Ui.Popups;
//using Game.Providers.Ui.Popups.Common;
//using Game.Providers.Ui.Windows;
//using UnityEngine;
//using UnityEngine.Events;
//using Zenject;

//namespace Game.Game.Handlers
//{
//	public class GameHandler
//	{
//		public UnityEvent OnStartStage = new();

//		private readonly SignalBus _signalBus;
//		private readonly IWindowProvider _windowProvider;
//		private readonly GameSession _gameSession;
//		private readonly IGameScene _gameScene;
//		private readonly GameSettings _gameSettings;
//		private readonly BoardSpawner _boardSpawner;
//		private readonly IBattleHelper _battleHelper;
//		private readonly PopupProvider _popupProvider;
//		private readonly INicknamesProvider _nicknamesProvider;
//		private readonly DiContainer _container;
//		private readonly BoardsSettings _boardSettings;
//		private readonly PointsModificatorHandler _pointsModificatorHandler;
//		private readonly IAvatarsProvider _avatarsProvider;
//		private readonly IProfileProvider _profileProvider;
//		private readonly IUiProvider _uiProvider;
//		private readonly PlayerGameHelper _playerGameHelper;

//		public GameHandler(SignalBus signalBus
//		, IWindowProvider windowProvider
//		, GameSession gameSession
//		, IBattleHelper battleHelper
//		, IGameScene gameScene
//		, GameSettings gameSettings
//		, BoardSpawner boardSpawner
//		, PopupProvider popupProvider
//		, INicknamesProvider nicknamesProvider
//		, DiContainer container
//		, BoardsSettings boardSettings
//		, PointsModificatorHandler pointsModificatorHandler
//		, IAvatarsProvider avatarsProvider
//		, IProfileProvider profileProvider
//		, IUiProvider uiProvider
//		, PlayerGameHelper playerGameHelper
//		)
//		{
//			_signalBus = signalBus;
//			_windowProvider = windowProvider;
//			_gameSession = gameSession;
//			_gameScene = gameScene;
//			_gameSettings = gameSettings;
//			_boardSpawner = boardSpawner;
//			_battleHelper = battleHelper;
//			_popupProvider = popupProvider;
//			_nicknamesProvider = nicknamesProvider;
//			_container = container;
//			_boardSettings = boardSettings;
//			_pointsModificatorHandler = pointsModificatorHandler;
//			_avatarsProvider = avatarsProvider;
//			_profileProvider = profileProvider;
//			_uiProvider = uiProvider;
//			_playerGameHelper = playerGameHelper;

//			_signalBus.Subscribe<BoardDestroySignal>(OnBoardDestroySignal);
//		}

//		public void PlayGame()
//		{
//			PlayGameAsync().Forget();
//		}

//		private void OnBoardDestroySignal(BoardDestroySignal eventData)
//		{
//			var playerBoardDestroy = eventData.IsPlayer
//			? _gameSession.Player
//			: _gameSession.Bot;
//			playerBoardDestroy.SetWinData(playerBoardDestroy.RoundWin + 1);
//			_signalBus.Fire(new RoundWinSignal(eventData.IsPlayer));

//			NewStageAsync().Forget();
//		}

//		private async UniTask PlayGameAsync()
//		{
//			_gameSession.IsGame = true;
//			//_gameSession.ActiveBattle = eventData.Tournament;
//			_gameSession.CalcBoards();
//			//_gameSession.Timer = TimersProvider.Create(TimerType.RealtimeDesc)
//			//	.End(_gameSession.ActiveTournament.Timer > 0 ? _gameSession.ActiveTournament.Timer : 60 * 3)
//			//	.AutoRemove(true)
//			//	.OnComplete(() => {
//			//		if (!_gameSession.IsGame)
//			//			return;
//			//		GameOwer();
//			//	})
//			//	.Start();
//			_gameSession.Player.SetWinData(0);
//			//_gameSession.Bot.SetWinData(0);
//			_playerGameHelper.RemoveWeapon();
//			_botGameHandler.RemoveWeapon();

//			_gameSession.BattleResult = new(_gameSession.ActiveBattle);
//			_container.Inject(_gameSession.BattleResult);
//			_pointsModificatorHandler.Reset();
//			//_gameSession.ResetOnNewGame();
//			_botGameHandler.StartGame(_gameSession.ActiveBattle);
//			(_gameScene as Component).gameObject.SetActive(true);
//			var splashController = _uiProvider.GetController<SplashWindowPresenterController>();
//			//var splashScreen = _windowProvider.GetWindow(WindowsNames.Splash, true);
//			_ = await splashController.Show(null);
//			_windowProvider.CloseAllWindows();

//			NewStage(true).Forget();

//			var gameController = _uiProvider.GetController<GamePlayWindowPresenterController>();

//			//var gameWindow = _windowProvider.GetWindow(WindowsNames.Game);
//			_ = await gameController.Show(null);
//			_signalBus.Fire<StartGameSignal>();
//			_ = await splashController.Hide();
//		}

//		private async UniTask NewStageAsync()
//		{
//			await UniTask.Delay(1000);
//			await NewStage(false);
//		}

//		private async UniTask NewStage(bool isNewGame)
//		{
//			if (!_gameSession.IsGame)
//				return;
//			if (!isNewGame)
//			{
//				var currentStage = _gameSession.ActiveBattle.Stages[_gameSession.StageIndex];
//				if (currentStage.Points > 0)
//				{
//					_gameSession.Points += currentStage.Points;
//					_signalBus.Fire(new LevelPointsChangeSignal(_gameSession.Points));
//				}
//			}
//			_gameSession.ShootReady = false;
//			_gameSession.StageIndex = isNewGame ? 0 : ++_gameSession.StageIndex;

//			//if (!isNewGame && _gameSession.Stage.IsBoss)
//			//{
//			//	var bossDefeatPopup = _popupProvider.GetPopup(PopupsNames.BossDefeat);
//			//	await bossDefeatPopup.Show();
//			//	await UniTask.Delay(1000);
//			//	await bossDefeatPopup.Hide();
//			//	_signalBus.Fire(new ResourceAddSignal(RewardTypes.Experience, 5, null));
//			//}
//			//if (_gameSession.StageIndex >= _gameSession.ActiveBattle.Stages.Count)
//			//{
//			//	if (!_gameSession.IsTutorial)
//			//	{
//			//		if (_gameSession.Player.RoundWin >= 5)
//			//			_profileProvider.AddWin();
//			//		if (_gameSession.Bot.RoundWin >= 5)
//			//			_profileProvider.AddDefeat();
//			//		_ = _profileProvider.Save.Save();
//			//		GameOwer();
//			//	}
//			//	return;
//			//}
//			//if (_gameSession.ActiveTournament.Stages.Count <= _gameSession.StageIndex) {
//			//	if (!_gameSession.IsTutorial)
//			//		GameOwer();
//			//	return;
//			//}

//			_gameSession.ResetOnNewStage();
//			_gameSession.Stage = _gameSession.ActiveBattle.Stages[_gameSession.StageIndex];
//			var hits = _gameSession.Stage.Hits > 0 ? _gameSession.Stage.Hits : 6;
//			_gameSession.Player.SetHit(hits);

//			if (_gameSession.Stage.IsBoss)
//			{
//				var bossBattlePopup = _popupProvider.GetPopup(PopupsNames.BossBattle);
//				await bossBattlePopup.Show();
//				await UniTask.Delay(1000);
//				await bossBattlePopup.Hide();
//				_pointsModificatorHandler.StartDelayReset();
//			}
//			else
//			{
//				_pointsModificatorHandler.StartDelayReset();
//			}

//			if (_gameSession.Board != null)
//				(_gameSession.Board as Component).gameObject.SetActive(false);

//			//var boardName = string.IsNullOrEmpty(_gameSession.Stage.BoardName) ? BoardNames.Pizza : _gameSession.Stage.BoardName;
//			var boardName = _gameSession.CurrentBoard();
//			var targetBoard = _boardSettings.Boards.Find(x => x.Name == boardName);
//			_gameSession.Board = _boardSpawner.SpawnNew(targetBoard, _gameSession.Stage);
//			_gameSession.Board.SetData(targetBoard, _gameSession.Stage);
//			OnStartStage?.Invoke();
//			_signalBus.Fire(new NewStageSignal());
//			await UniTask.Delay(200);

//			_botGameHandler.CreateWeapon();
//			_playerGameHelper.CreateWeapon();

//			_gameSession.ShootReady = true;
//		}

//		public void GameOwer()
//		{
//			GameOwerAsync().Forget();
//		}

//		public void PlayerDefeat()
//		{
//			_gameSession.ShootReady = false;
//			_gameSession.Board.Destroy(false);
//		}

//		private async UniTask GameOwerAsync()
//		{
//			if (!_gameSession.IsGame)
//				return;
//			_gameSession.ShootReady = false;
//			_gameSession.IsGame = false;
//			_botGameHandler.StopGame();

//			_playerGameHelper.RemoveWeapon();
//			_botGameHandler.RemoveWeapon();
//			//var seconds = (int)_gameSession.Timer.CurrentValueSeconds;

//			//var timePoints = Mathf.Max(seconds * 10, 0);
//			var timePoints = 0;
//			var gamePoinst = _gameSession.Points;

//			_gameSession.Board?.Destroy();

//			//_ = _gameSession.Timer.Stop();
//			_gameSession.BattleResult.Points = gamePoinst + timePoints;
//			float secondsToComplete = _gameSession.IsTutorial ? 0 : UnityEngine.Random.Range(15, 300);
//			_gameSession.BattleResult.WaitComplete = true;
//			_gameSession.BattleResult.TimeComplete = DateTime.Now.AddSeconds(secondsToComplete);

//			_ = _gameSession.ActiveBattle.FirstGameWin && !_profileProvider.Save.Value.TournamentsGames.Contains(_gameSession.ActiveBattle.Uuid);

//			//if (isFirstGameWin)
//			//{
//			//	_gameSession.TournamentResult.IsPlayerWin = true;
//			//	_profileProvider.Save.Value.TournamentsGames.Add(_gameSession.ActiveBattle.Uuid);
//			//	_profileProvider.Save.Save();
//			//}
//			//else
//			//{
//			//	_gameSession.TournamentResult.IsPlayerWin = UnityEngine.Random.value <= _gameSession.ActiveBattle.WinRate;
//			//}
//			_gameSession.BattleResult.IsPlayerWin = _gameSession.Player.RoundWin > _gameSession.Bot.RoundWin;

//			//if (!_gameSession.TournamentResult.IsPlayerWin) {
//			//	_gameSession.TournamentResult.WinPlayer = _nicknamesProvider.GetRandom();
//			//	_gameSession.TournamentResult.OpponentAvatar = _avatarsProvider.GetRandomKeyExclude(_profileProvider.Save.Value.Avatar);
//			//_gameSession.TournamentResult.OpponentPoints = totalPoints + (int)(Mathf.CeilToInt(UnityEngine.Random.Range(totalPoints * 0.01f, totalPoints * 0.05f) / 10f) * 10);
//			//} else {
//			//_gameSession.TournamentResult.OpponentPoints = totalPoints - (int)(Mathf.CeilToInt(UnityEngine.Random.Range(totalPoints * 0.01f, totalPoints * 0.15f) / 10f) * 10);
//			//}

//			if (!_gameSession.IsTutorial)
//			{
//				_gameSession.BattleResult.StartTimerIfNeed();
//				_battleHelper.AddBattleResult(_gameSession.BattleResult);
//				_battleHelper.UpdateTournamentResult();
//			}
//			else
//			{
//				_gameSession.BattleResult.EmitWaitTimerComplete();
//			}

//			_signalBus.Fire<GameOverSignal>();

//			var splashController = _uiProvider.GetController<SplashWindowPresenterController>();
//			//var splashScreen = _windowProvider.GetWindow(WindowsNames.Splash, true);
//			_ = await splashController.Show(null);
//			var gameController = _uiProvider.GetController<GamePlayWindowPresenterController>();
//			_ = await gameController.Hide();
//			//var gameWindow = _windowProvider.GetWindow(WindowsNames.Game);
//			//await gameWindow.Hide();

//			_signalBus.Fire<DestroyAllKnifesSignal>();
//			//var homeWindowPresenterControllerr = _uiProvider.GetController<HomeWindowPresenterController>();
//			////var homeWindow = _windowProvider.GetWindow(WindowsNames.Home);
//			//_ = await homeWindowPresenterControllerr.Show(null);
//			await UniTask.Delay(400);
//			_ = await splashController.Hide();

//			var gameResultController = _uiProvider.GetController<GameResultWindowPresenterController>();

//			_ = await gameResultController.Show(null);
//			//gameResultController.Presenter.SetPoints(gamePoinst + timePoints, gamePoinst, timePoints);
//		}
//	}
//}
