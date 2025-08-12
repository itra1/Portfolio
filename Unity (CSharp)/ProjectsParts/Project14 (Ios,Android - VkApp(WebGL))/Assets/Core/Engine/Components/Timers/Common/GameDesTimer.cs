
using UnityEngine;

namespace Core.Engine.Components.Timers
{
	public class GameDesTimer : Timer
	{
		private double _startTimer;
		private double _currentTime;

		public override ITimer Remove()
		{
			EmitOnRemove();

			return this;
		}

		public override ITimer Start(double startValue = 0)
		{
			_startTimer = startValue;
			_currentTime = _startTimer;
			return base.Start(startValue);
		}

		public override ITimer Tick()
		{
			_currentTime -= Time.deltaTime;
			EmitOnTick(_currentTime);

			return this;
		}
	}
}
