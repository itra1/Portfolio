using System;

namespace Core.Engine.Components.Timers
{
	public class RealTimer : Timer
	{
		private DateTime _startTime;
		private DateTime _currentTime;

		public override ITimer Remove()
		{
			return this;
		}

		public override ITimer Start(double startValue = 0)
		{
			_startTime = DateTime.Now;
			_currentTime = _startTime;
			return this;
		}

		public override ITimer Tick()
		{
			_currentTime = DateTime.Now;
			EmitOnTick((_currentTime - _startTime).TotalSeconds);
			return this;
		}
	}
}
