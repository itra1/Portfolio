using Game.Providers.Timers.Common;
using Game.Providers.Ui.Presenters.Base;
using TMPro;
using UnityEngine;

namespace Game.Providers.Ui.Presenters
{
	public class LookingOpponentWindowPresenter : WindowPresenter
	{
		[SerializeField] private TMP_Text _timerLabel;

		private ITimer _timer;
		private int _lastValue;

		public void SetTimer(ITimer timer)
		{
			_timer = timer;
			_ = _timer.OnTick(TimerTick);
		}

		private void TimerTick(double delaySeconds)
		{
			var targetInt = Mathf.FloorToInt((float) delaySeconds);
			if (targetInt == _lastValue)
				return;

			_lastValue = targetInt;

			var minut = _lastValue / 60;
			var seconds = _lastValue % 60;

			string timeString = $"{minut.ToString("00")}:{seconds.ToString("00")}";

			SetTimerValue(timeString);
		}

		private void SetTimerValue(string timeDelay) => _timerLabel.text = $"<sprite=1> {timeDelay}";
	}
}
