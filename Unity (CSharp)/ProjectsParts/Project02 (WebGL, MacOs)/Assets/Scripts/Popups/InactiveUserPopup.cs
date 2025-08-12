using System.Collections;
using UnityEngine;
using TMPro;
 

namespace it.Popups
{
	public class InactiveUserPopup : PopupBase
	{
		public UnityEngine.Events.UnityAction OnOkEvent;
		public UnityEngine.Events.UnityAction OnRestartEvent;

		[SerializeField] private TextMeshProUGUI _timersLabel;
		[SerializeField] private RectTransform _step1Panel;
		[SerializeField] private RectTransform _step2Panel;

		private int _timerValue = 30;

		protected override void EnableInit()
		{
			base.EnableInit();
			_step1Panel.gameObject.SetActive(true);
			_step2Panel.gameObject.SetActive(false);

			StartCoroutine(TimerCoroutine(_timerValue));

		}

		private IEnumerator TimerCoroutine(int value){
			SetTimerData(value);

			while(value > 0)
			{
				SetTimerData(value);
				yield return new WaitForSeconds(1);
				value--;
			}
			SetTimerData(value);
			yield return new WaitForSeconds(1);
			_step1Panel.gameObject.SetActive(false);
			_step2Panel.gameObject.SetActive(true);
		}

		private void SetTimerData(int value)
		{
			_timersLabel.text = $"{value} " + "popup.connectionLost.secondsLeft".Localized();
		}

		public override void Hide()
		{
			StopAllCoroutines();
			base.Hide();
		}

		public void CloseTouch()
		{
			Hide();
		}

		public void OkTouch()
		{
			OnOkEvent?.Invoke();
		}

		public void RestartTouch()
		{
			OnRestartEvent?.Invoke();

		}

	}
}