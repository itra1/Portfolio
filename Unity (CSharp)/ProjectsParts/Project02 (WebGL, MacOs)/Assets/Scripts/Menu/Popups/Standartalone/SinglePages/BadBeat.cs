using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using it.Api;
using System.Linq;
using Garilla;
using Garilla.BadBeat;
using it.Network.Rest;

namespace it.Main.SinglePages
{
	public class BadBeat : SinglePage
	{
		[SerializeField] private JackpotWinner jackpotWinner;
		[SerializeField] private JackpotLastWinner lastWinner;

		private void OnDisable()
		{
			BadBeatController.Instance.OnUpdate -= UpdateData;
		}

		private void UpdateData(JackpotInfoResponse data){
			ConfirmData();
		}

		protected override void EnableInit()
		{
			PlayerPrefs.DeleteKey(StringConstants.BUTTON_BADBEAT);
			base.EnableInit();

#if UNITY_STANDALONE
			StandaloneController.Instance.FocusWindow();
#endif

			BadBeatController.Instance.OnUpdate -= UpdateData;
			BadBeatController.Instance.OnUpdate += UpdateData;

			if (BadBeatController.Instance.Data != null)
			{
				ConfirmData();
			}

			//BadBeatController.Instance.GetData((data)=> {
			//	jackpotWinner.SetData(data);
			//});

		}

		private void ConfirmData()
		{
			jackpotWinner.SetData(BadBeatController.Instance.Data);
			if (lastWinner != null)
				lastWinner.SetData(BadBeatController.Instance.Data);
		}

	}
}