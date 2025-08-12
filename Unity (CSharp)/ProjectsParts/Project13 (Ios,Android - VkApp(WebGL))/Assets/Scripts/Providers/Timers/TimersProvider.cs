using System.Collections.Generic;
using Game.Providers.Timers.Base;
using Game.Providers.Timers.Common;
using Zenject;

namespace Game.Providers.Timers
{
	public class TimersProvider : ITickable
	{
		private List<ITimerTick> _timers = new();
		private List<ITimerTick> _timersAdd = new();
		private List<ITimerTick> _timersForRemove = new();
		public static TimersProvider Instance { get; set; }
		public List<ITimerTick> TimersAdd => _timersAdd;

		public TimersProvider()
		{
			Instance = this;
		}

		public void Tick()
		{
			if (_timers.Count == 0 && _timersAdd.Count == 0)
				return;

			if (_timersAdd.Count > 0)
			{
				_timers.AddRange(_timersAdd);
				_timersAdd.Clear();
			}

			foreach (var timer in _timers)
			{
				if (timer.IsActive && !timer.IsPaused)
					_ = timer.Tick();

				if (!timer.IsActive && timer.IsAutoRemove)
					_timersForRemove.Add(timer);
			}

			if (_timersForRemove.Count > 0)
			{
				foreach (var timer in _timersForRemove)
				{
					_ = timer.Remove();
					_ = _timers.Remove(timer);
				}
				_timersForRemove.Clear();
			}
		}

		public static ITimer Create(string type)
		{
			ITimerTick timer = type switch
			{
				TimerType.RealtimeDesc => new RealDeskTimer(),
				TimerType.Realtime => new RealTimer(),
				TimerType.GameDesc => new GameDesTimer(),
				_ => new GameTimer()
			};
			Instance.TimersAdd.Add(timer);
			return timer;
		}
	}
}
