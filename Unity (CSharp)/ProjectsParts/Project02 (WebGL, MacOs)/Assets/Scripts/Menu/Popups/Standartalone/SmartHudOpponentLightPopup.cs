using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
 
using it.Network.Rest;

namespace it.Popups
{
	public class SmartHudOpponentLightPopup : SmartHudPopup
	{
		[SerializeField] private it.Inputs.CurrencyLabel _balanceLabel;
		[SerializeField] private TextMeshProUGUI _note;
		public void SetData(decimal balance, UserLimited user, UserStat stat, UserNote note)
		{
			_balanceLabel.SetValue(balance);
			_note.text = note.message;

			var textRect = _note.GetComponent<RectTransform>();

			base.SetData(user, stat);
		}
	}
}