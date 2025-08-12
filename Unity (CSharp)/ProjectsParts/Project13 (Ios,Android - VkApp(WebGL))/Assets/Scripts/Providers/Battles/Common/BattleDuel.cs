using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Game.Providers.Battles.Components;
using Game.Providers.Battles.Controllers;
using Game.Providers.Battles.Settings;
using Game.Providers.Profile.Handlers;
using Game.Providers.Timers;
using Game.Providers.Timers.Base;
using Game.Providers.Timers.Common;
using Game.Providers.Ui.Controllers;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Game.Providers.Battles.Common
{
	public partial class BattleDuel : Battle
	{
		private BattleDuelSettings _settings;
		private BotController _bot;
		private PlayerDuelController _player;
		private GamePlayDuelWindowPresenterController _gamePlayController;
		private LookingOpponentWindowPresenterController _lookingOpponentWindowPresenterController;
		private GameSelectWindowPresenterController _gameSelectWindowPresenterController;
		private GameDuelResultWindowPresenterController _gameDuelResultController;
		private CoinsHandler _coinsHandler;

		public BattleDuelState State { get; protected set; } = new();
		public BattleDuelSettings Settings => _settings;
		public override int Stages => Settings.Stages.Count;

		public BattleDuel(IBattleProvider battleProvider) : base(battleProvider)
		{
		}

		[Inject]
		public void Bind(CoinsHandler coinsHandler)
		{
			_coinsHandler = coinsHandler;
			_gameSelectWindowPresenterController ??= _uiProvider.GetController<GameSelectWindowPresenterController>();
			_lookingOpponentWindowPresenterController ??= _uiProvider.GetController<LookingOpponentWindowPresenterController>();
			_gamePlayController ??= _uiProvider.GetController<GamePlayDuelWindowPresenterController>();
			_gameDuelResultController ??= _uiProvider.GetController<GameDuelResultWindowPresenterController>();

			_bot = new(this);
			_diContainer.Inject(_bot);
			_bot.OnHitEvent = BotHitChange;
			_bot.OnLossEvent = () => { _ = StageComplete(); };

			_player = new(this);
			_diContainer.Inject(_player);
			_player.OnHitEvent = PlayerHitChange;
			_player.OnLossEvent = () => { _ = StageComplete(); };

			_gamePlayController.OnPlayerWeaponSelect = (weaponType) => { _player.SetWeapon(weaponType); };
		}

		public override async UniTask StartGame()
		{
			_bot.MakeOpponent();
			await base.StartGame();

			_settings = _settings != null ? _settings : await LoadResourcesConfig("BattleDuelSettings") as BattleDuelSettings;

			if (_bot == null)
			{
			}

			if (_gameSelectWindowPresenterController.IsOpen)
				_ = _gameSelectWindowPresenterController.Hide();

			bool waitTimer = true;
			ITimer startTimer = TimersProvider.Create(TimerType.RealtimeDesc)
				.End(_settings.MaxTimeWait)
				.OnComplete(() => { waitTimer = false; })
				.Start();

			_ = await _lookingOpponentWindowPresenterController.Show(null);
			_lookingOpponentWindowPresenterController.SetTimer(startTimer);

			await UniTask.WaitUntil(() => !waitTimer);
			_ = _lookingOpponentWindowPresenterController.Hide();

			_ = await _gamePlayController.Show(null);
			(_gameScene as Component).gameObject.SetActive(true);
			_gamePlayController.SetVisibleRoundTimer(false);

			_gamePlayController.SetPlayerWinLabel(0);
			_gamePlayController.SetBotWinLabel(0);
			_gamePlayController.SetBotAvatar(State.BotState.Avatar);

			CalcBoardsInRounds();
			ResetNewGame();

			IsActiveBattle = true;
			_player.SpawnNewWeapon();
			ReadyNextStage();
			await StartTimer();
			StartStage();
		}

		private void ReadyNextStage()
		{
			State.CurrentStage++;
			ReadStage();
			ChangeRoundLabel();
			ClearHits();
			SpawnNewBoard();
		}

		private async Task StageComplete()
		{
			if (!State.ShootReady)
				return;

			State.ShootReady = false;
			bool winPlayer = State.PlayerState.BoardsHit > State.BotState.BoardsHit;

			if (State.StageTimer != null)
			{
				_ = State.StageTimer.Stop();
			}

			if (winPlayer)
				State.PlayerState.WinCount++;
			else
				State.BotState.WinCount++;

			bool isGameComplete = State.CurrentStage == _settings.Stages.Count - 1;

			if (isGameComplete)
			{
				_ = GameOver();
				return;
			}

			Func<UnityAction, UniTask> resultDialog = winPlayer
			? _gamePlayController.ShowWin
			: _gamePlayController.ShowLoss;

			await resultDialog(() =>
			{
				_gamePlayController.SetPlayerWinLabel(State.PlayerState.WinCount);
				_gamePlayController.SetBotWinLabel(State.BotState.WinCount);
				ReadyNextStage();
			});

			StartStage();

		}

		private void StartStage()
		{
			if (!IsActiveBattle)
				return;

			State.StageTimer = TimersProvider.Create(TimerType.RealtimeDesc)
			.AutoRemove()
			.End(60 * 2.5f)
			.Start()
			.OnTick((time) =>
			{
				_gamePlayController.SetRoundTimer($"{((int) (time / 60)):00}:{Mathf.CeilToInt((float) time % 60):00}");
			})
			.OnComplete(() =>
			{
				_ = StageComplete();
			});

			_gamePlayController.SetVisibleRoundTimer(true);

			State.ShootReady = true;
			_bot.StartGame();

			OnStartStage?.Invoke();
		}

		private async Task<UniTask> GameOver()
		{
			if (!IsActiveBattle)
				return default;
			IsActiveBattle = false;
			_bot.StopGame();
			_bot.RemoveWeapon();
			_player.RemoveWeapon();

			if (State.IsPlayerWin)
			{
				State.PlayerState.WinCoins = _settings.CoinsWin;
				State.BotState.WinCoins = _settings.CoinsLoss;
				_ = _coinsHandler.AddValue(_settings.CoinsWin, null);
			}
			else
			{
				State.PlayerState.WinCoins = _settings.CoinsLoss;
				State.BotState.WinCoins = _settings.CoinsWin;
				_ = _coinsHandler.AddValue(_settings.CoinsLoss, null);
			}

			_gameDuelResultController.SetState(State);
			_ = await _gameDuelResultController.Show(null);
			_ = _gamePlayController.Hide();

			State.Board?.Destroy();
			return default;
		}
	}
}
