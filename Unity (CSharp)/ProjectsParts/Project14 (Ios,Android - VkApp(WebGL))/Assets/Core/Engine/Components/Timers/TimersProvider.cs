using System.Collections.Generic;

using Zenject;

namespace Core.Engine.Components.Timers
{
[System.Flags]
	public enum TimerType
	{
		Game = 0
	, GameDesc = 1
	, RealTime = 2
	, RealTimeDesc = 4
	}


	/// <summary>
	/// Таймеры
	/// </summary>
	public class TimersProvider
	: ITickable
	, ITimersProvider
	{
		private List<ITimerTick> _timers = new();
		private List<ITimerTick> _timersAdd = new();
		private List<ITimerTick> _timersForRemove = new();

		public void Tick()
		{
			if (_timers.Count == 0 && _timersAdd.Count == 0) return;

			// Добавляем новые
			if (_timersAdd.Count > 0)
			{
				_timers.AddRange(_timersAdd);
				_timersAdd.Clear();
			}

			// Апдейтим текущие
			foreach (var timer in _timers)
			{
				if (timer.IsStarted && !timer.IsPaused && !timer.IsStoped)
					timer.Tick();

				if (timer.IsStoped && timer.IsAutoRemove)
					_timersForRemove.Add(timer);
			}

			// Удаляем остановленные
			if (_timersForRemove.Count > 0)
			{
				foreach (var timer in _timersForRemove)
				{
					timer.Remove();
					_timers.Remove(timer);
				}
				_timersForRemove.Clear();
			}
		}

		/// <summary>
		/// Добавить таймер сцены
		/// </summary>
		/// <returns></returns>
		public ITimer Create(TimerType type)
		{
			ITimerTick timer = type switch
			{
				TimerType.RealTimeDesc => new RealTimer(),
				TimerType.RealTime => new RealTimer(),
				TimerType.GameDesc => new GameDesTimer(),
				_ => new GameTimer()
			};
			_timersAdd.Add(timer);
			return timer;
		}

	}
}