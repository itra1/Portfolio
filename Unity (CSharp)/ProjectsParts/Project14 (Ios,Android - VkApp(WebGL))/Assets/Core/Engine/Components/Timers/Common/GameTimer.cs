using UnityEngine;

namespace Core.Engine.Components.Timers
{
	public class GameTimer : Timer
	{
		private double _startTimer;
		private double _currentTime;

		public override ITimer Start(double startValue = 0)
		{
			_startTimer = Time.realtimeSinceStartup;
			_currentTime = _startTimer;

			return base.Start(startValue);
		}

		public override ITimer Tick()
		{
			_currentTime += Time.deltaTime;
			EmitOnTick(_currentTime);

			return this;
		}
		public override ITimer Remove()
		{
			EmitOnRemove();

			return this;
		}
	}
}
