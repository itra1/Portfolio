 
using System.Collections;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;

namespace it.Popups
{
	public class ChipInNextHandPopup : PopupBase
	{
		[SerializeField] private TextMeshProUGUI _description;

		public void SetValue(decimal value)
		{
			_description.text = string.Format("lobby.chipInNextHand.description".Localized(), it.Helpers.Currency.String(value));
		}

		public void OkTouch()
		{
			Hide();
		}

	}
}