using UnityEngine;
using Zenject;

namespace Engine.Scripts.Managers
{
	public class DspTime : IDspTime, ITickable
	{
		private double _time;
		private double _adaptiveTime;

		public double Time => _time;

		public double AdaptiveTime => _adaptiveTime;

		protected DspTime()
		{
			_time = AudioSettings.dspTime;
			_adaptiveTime = _time;
		}

		public void Tick()
		{
			if (_time == AudioSettings.dspTime)
			{
				_adaptiveTime += UnityEngine.Time.unscaledDeltaTime;
			}
			else
			{
				_time = AudioSettings.dspTime;
				_adaptiveTime = _time;
			}
		}
	}
}