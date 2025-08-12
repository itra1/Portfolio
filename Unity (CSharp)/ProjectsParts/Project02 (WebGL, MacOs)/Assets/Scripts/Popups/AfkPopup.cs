//#define TIMER
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using it.Network.Rest;

namespace it.Popups
{
	public class AfkPopup : PopupBase
	{
		public TMP_Text Timer;
		private Action<bool> callback;
#if TIMER
		private DateTime endTime;
		private bool isActiveTimer = false;
#endif

		public void Init(TablePlayerSession selectTableMePlayer, Action<bool> callback)
		{
			this.callback = callback;
#if TIMER
			Timer.gameObject.SetActive(selectTableMePlayer.RestTimeoutAt != null);
			if (selectTableMePlayer.RestTimeoutAt != null)
			{
				endTime = DateTime.Parse(selectTableMePlayer.RestTimeoutAt);
				isActiveTimer = true;
			}
#endif
		}

		public void TouchReturn()
		{
			Hide();
			callback(true);
		}

		public void TouchLeave()
		{
			Hide();
			callback(false);
		}

		public override void Hide()
		{
#if TIMER
			isActiveTimer = false;
#endif
			base.Hide();
		}
#if TIMER
		private void SetTextTime()
		{
			if (!isActiveTimer) return;
			var diff = (endTime - GameController.NowTime).TotalSeconds;
			if (diff > 60)
			{
				TimeSpan ts = TimeSpan.FromSeconds(diff);
				Timer.text = ts.ToString(@"mm\:ss");
			}
			else if (diff >= 0)
			{
				Timer.text = (int)diff + "s";
			}
			else
			{
				Hide();
			}
		}

		private void FixedUpdate()
		{
			SetTextTime();
		}
#endif
	}
}