using System;

namespace Game.Scripts.Providers.Timers.Common
{
	public class RealDeskTimer : Timer
	{
		private DateTime _timeEnd;
		private DateTime _currentTime;
		private double _timeEndDouble = 0;
		private double _lastTick;

		public override double CurrentValueSeconds => (_timeEnd - DateTime.Now).TotalSeconds;

		public override ITimer Remove()
		{
			EmitOnRemove();
			return this;
		}

		public override ITimer End(DateTime timeEnd)
		{
			_timeEnd = timeEnd;
			return this;
		}

		public override ITimer End(double delaySeconds)
		{
			_timeEndDouble = delaySeconds;
			return this;
		}

		public override ITimer Start(double startValue = 0)
		{
			if (_timeEndDouble != 0)
				_timeEnd = DateTime.Now.AddSeconds(_timeEndDouble);
			_ = base.Start(startValue);
			return this;
		}

		public override ITimer Tick()
		{
			_currentTime = DateTime.Now;

			var tick = (_timeEnd - _currentTime).TotalSeconds;

			if (tick <= 0)
			{
				_ = Complete();
				_ = Stop();
				return this;
			}

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
