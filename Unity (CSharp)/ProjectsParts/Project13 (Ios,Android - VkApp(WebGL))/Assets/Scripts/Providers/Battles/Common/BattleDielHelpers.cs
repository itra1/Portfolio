using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Providers.Timers;
using Game.Providers.Timers.Base;
using Game.Providers.Timers.Common;
using UnityEngine;

namespace Game.Providers.Battles.Common
{
	public partial class BattleDuel
	{
		public List<string> BoardsInBattle { get; private set; } = new();
		protected override void RemoveSelectWeapons()
		{
			base.RemoveSelectWeapons();
			_bot.RemoveWeapon();
		}

		private void CalcBoardsInRounds()
		{
			BoardsInBattle.Clear();

			while (BoardsInBattle.Count < Stages)
			{
				var newKey = _boardSpawner.GetRandomKey();
				if (!BoardsInBattle.Contains(newKey))
				{
					BoardsInBattle.Add(newKey);
				}
			}
		}

		private async UniTask StartTimer()
		{
			var startTimerLabel = _gamePlayController.Presenter.StartTimerLabel;
			startTimerLabel.gameObject.SetActive(true);

			bool isWait = true;

			Action<string> setTimerValue = (timerString) =>
				startTimerLabel.text = timerString;
			ITimer startTimer = TimersProvider.Create(TimerType.RealtimeDesc)
			.End(3)
			.OnTick((val) =>
			{
				setTimerValue(Mathf.CeilToInt((float) val).ToString());
			})
			.OnComplete(() =>
			{
				isWait = false;
				setTimerValue("GO!");

				_ = UniTask.Create(async () =>
				{
					await UniTask.Delay(1000);
					startTimerLabel.gameObject.SetActive(false);
				});

			})
			.Start();

			await UniTask.WaitWhile(() => isWait);
		}

		public string CurrentBoard() => BoardsInBattle[State.CurrentStage];

		private void ChangeRoundLabel() =>
			_gamePlayController.SetRoundLabel(State.CurrentStage + 1, _settings.Stages.Count);

		private void ResetNewGame()
		{
			State.CurrentStage = -1;
			State.PlayerState.Points = 0;
			State.BotState.Points = 0;
		}

		private void SpawnNewBoard()
		{
			try
			{
				if (State.Board != null && State.Board is Component boardComponent)
					_boardSpawner.Despawn(State.Board);
				State.Board = _boardSpawner.SpawnNew(CurrentBoard(), State.Stage);
			}
			catch (Exception ex)
			{
				AppLog.LogError(ex);
			}
		}

		private void ReadStage()
		{
			State.Stage = _settings.Stages[State.CurrentStage];
		}

		private void ClearHits()
		{
			_player.ClearWeaponInBoards();
			_bot.ClearWeaponInBoards();
			_gamePlayController.SetPlayerHitLabel(0, State.Stage.BoardHits);
			_gamePlayController.SetBotHitLabel(0, State.Stage.BoardHits);
		}

		private void BotHitChange(int hitCount)
		{
			_gamePlayController.SetBotHitLabel(hitCount, State.Stage.BoardHits);
			State.BotState.BoardsHit = hitCount;
			if (hitCount >= State.Stage.BoardHits)
			{
				_ = StageComplete();
			}
		}

		private void PlayerHitChange(int hitCount)
		{
			_gamePlayController.SetPlayerHitLabel(hitCount, State.Stage.BoardHits);
			State.PlayerState.BoardsHit = hitCount;
			if (hitCount >= State.Stage.BoardHits)
			{
				_ = StageComplete();
			}
		}
	}
}
