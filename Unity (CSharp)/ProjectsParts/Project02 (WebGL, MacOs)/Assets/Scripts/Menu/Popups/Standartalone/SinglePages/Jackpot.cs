using UnityEngine;
using Garilla;
using Garilla.Jackpot;
using it.Network.Rest;

namespace it.Main.SinglePages
{
	public class Jackpot : SinglePage
	{
		[SerializeField] private JackpotPayouts jackpotPayouts;
		[SerializeField] private JackpotWinner jackpotWinner;
		[SerializeField] private JackpotLastWinner lastWinner;

		private void OnDisable()
		{
			JackpotController.Instance.OnUpdate -= JackpotUpdate;
		}

		protected override void EnableInit()
		{
			JackpotController.Instance.OnUpdate -= JackpotUpdate;
			JackpotController.Instance.OnUpdate += JackpotUpdate;


			if (JackpotController.Instance.Data != null)
				ConfirmData();
			else
			{
				JackpotController.Instance.RequestData(jackpot =>
				{
					ConfirmData();
				});
			}
			//AppApi.GetJackpot((result) =>
			//		{
			//			if (result.IsSuccess)
			//			{
			//				jackpotPayouts.SetData(result.Result);
			//				jackpotWinner.SetData(result.Result);
			//				if (lastWinner != null)
			//					lastWinner.SetData(result.Result);
			//			}
			//		});
		}

		private void JackpotUpdate(JackpotInfoResponse jackpotData)
		{
			ConfirmData();
		}

		private void ConfirmData()
		{
			jackpotPayouts.SetData(JackpotController.Instance.Data);
			jackpotWinner.SetData(JackpotController.Instance.Data);
			if (lastWinner != null)
				lastWinner.SetData(JackpotController.Instance.Data);
		}

		public void WhatIsLinkTouch()
		{
			LinkManager.OpenUrl("whatIsJackpot");
		}

	}
}