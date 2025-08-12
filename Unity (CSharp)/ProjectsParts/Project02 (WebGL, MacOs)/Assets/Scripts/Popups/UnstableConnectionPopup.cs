using System.Collections;
using UnityEngine;

namespace it.Popups
{
	public class UnstableConnectionPopup : PopupBase
	{
		public UnityEngine.Events.UnityAction OnRetryEvent;
		public UnityEngine.Events.UnityAction OnCheckNetworkStatusEvent;


		public void CloseTouch()
		{
			Hide();
		}

		public void RetryTouch()
		{
			OnRetryEvent?.Invoke();
		}

		public void CheckNetworkStatusTouch()
		{
			OnCheckNetworkStatusEvent?.Invoke();
		}

	}
}