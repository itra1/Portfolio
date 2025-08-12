using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace it.Game.Panels
{
	public class PrizeItem : MonoBehaviour
	{
		[SerializeField] private it.Inputs.CurrencyLabel _currencyLabel;

		public void SetValue(decimal value)
		{
			_currencyLabel.SetValue(value);
		}
		public void SetValue(double value)
		{
			_currencyLabel.SetValue(value);
		}

	}

}