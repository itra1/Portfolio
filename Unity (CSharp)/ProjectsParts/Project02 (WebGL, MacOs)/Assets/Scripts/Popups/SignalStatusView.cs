using it.Popups;
using UnityEngine;

namespace it.UI
{
	public class SignalStatusView : it.Popups.PingViewBase
	{
		[SerializeField] GameObject[] rects = new GameObject[4];

		public override void Awake()
		{
			Host = ServerManager.Server;
			base.Awake();
		}

		public override void SetPing(long value)
		{
			long badPing = 300;
			long resquality = 4 - value * 4 / badPing;
			if (resquality < 0) resquality = 0;
			for (int i = 0; i <= 3; i++)
			{
				rects[i].SetActive(false);
			}
			if (resquality > 0)
				for (int i = 0; i <= resquality - 1; i++)
				{
					rects[i].SetActive(true);
				}
		}

		public override void Clear()
		{
			for (int i = 0; i <= 3; i++)
			{
				rects[i].SetActive(false);
			}
		}
	}
}
