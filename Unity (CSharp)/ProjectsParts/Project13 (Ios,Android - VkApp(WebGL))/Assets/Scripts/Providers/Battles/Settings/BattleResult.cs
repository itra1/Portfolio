using System;
using Game.Base;
using Game.Providers.Profile.Signals;
using Game.Providers.Timers;
using Game.Providers.Timers.Base;
using Game.Providers.Timers.Common;
using Zenject;

namespace Game.Providers.Battles.Settings
{
	public class BattleResult
	{
		public string Uuid;
		public DuelItemSettings Tournament;
		public string WinPlayer;
		public DateTime DateStart = DateTime.Now;
		public DateTime TimeComplete = DateTime.MinValue;
		public bool InGame;
		public int Points;
		public double TimeInGame;

		public bool WaitComplete;
		[NonSerialized] public ITimer TimerComplete;
		public bool IsPlayerWin;
		public int OpponentPoints;
		public string OpponentAvatar;
		public bool IsGetReward;

		public bool ExistsReward => !WaitComplete && IsPlayerWin && !IsGetReward;
		public bool IsComplete => !WaitComplete;
		public bool IsWin => IsPlayerWin;

		[Inject] protected SignalBus _signalBus;
		[Inject] protected IBattleProvider _tournamentProvider;

		public BattleResult()
		{
			StartTimerIfNeed();
		}

		public BattleResult(DuelItemSettings tournament)
		{
			Uuid = System.Guid.NewGuid().ToString();
			Tournament = tournament;
			DateStart = DateTime.Now;
			InGame = true;
			StartTimerIfNeed();
		}

		public void StartTimerIfNeed()
		{
			if (!WaitComplete)
				return;

			if (TimeComplete < DateTime.Now)
			{
				EmitWaitTimerComplete();
			}
			else
			{
				TimerComplete = TimersProvider.Create(TimerType.RealtimeDesc)
					.End((double) (TimeComplete - DateTime.Now).TotalSeconds)
					.AutoRemove()
					.OnComplete(() =>
					{
						EmitWaitTimerComplete();
					})
					.Start();
			}
		}

		public void EmitWaitTimerComplete()
		{
			WaitComplete = false;
			if (IsWin && Tournament.WinExp > 0)
				_signalBus.Fire(new ResourceAddSignal(RewardTypes.Experience, Tournament.WinExp, null));
			_tournamentProvider.ResultsChangeEmit();
		}
	}
}
