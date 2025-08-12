using System.Collections;

using UnityEngine;

namespace it.Popups
{
	public class UpdateTimerPopup : PopupBase
	{
		public UnityEngine.Events.UnityAction OnConfirm;
		[SerializeField] private TMPro.TextMeshProUGUI _timerLabel;

		private System.DateTime _endTimer;
		private int _secondsTimer;

		protected override void EnableInit()
		{
			base.EnableInit();
			_secondsTimer = 10;
			_endTimer = System.DateTime.Now.AddSeconds(_secondsTimer);
			ConfirmTimer(_secondsTimer.ToString());
		}

		private void Update()
		{
			if ((_endTimer - System.DateTime.Now).TotalSeconds < _secondsTimer - 1)
			{
				_secondsTimer--;
				ConfirmTimer(_secondsTimer.ToString());
			}
			if (_secondsTimer <= 0)
				CompleteTouch();
		}

		private void ConfirmTimer(string value)
		{
			_timerLabel.text = value;
		}

		public void CompleteTouch()
		{
			Hide();
			OnConfirm?.Invoke();
		}

	}
}