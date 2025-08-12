using System;

namespace Game.Scripts.Providers.Timers.Common
{
	public class RealTimer : Timer
	{
		private DateTime _startTime;
		private DateTime _currentTime;
		private double _lastTick;

		public override double CurrentValueSeconds => (DateTime.Now - _startTime).TotalSeconds;

		public override ITimer Remove()
		{
			return this;
		}

		public override ITimer Start(double startValue = 0)
		{
			_startTime = DateTime.Now;
			_currentTime = _startTime;
			base.Start(startValue);
			return this;
		}

		public override ITimer Tick()
		{
			_currentTime = DateTime.Now;
			var tick = (_currentTime - _startTime).TotalSeconds;

			if (_lastTick != tick)
			{
				_lastTick = tick;
				EmitOnTick(_lastTick);
				return this;
			}
			return this;
		}
	}
}
