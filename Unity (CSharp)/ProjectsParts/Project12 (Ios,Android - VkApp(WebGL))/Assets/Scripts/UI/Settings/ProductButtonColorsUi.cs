using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.UI.Settings
{
	[System.Serializable]
	public class ProductButtonColorsUi
	{

		[SerializeField] private List<ProductButtonColorsUiItem> _colorItems;

		public List<ProductButtonColorsUiItem> ColorItems => _colorItems;
	}
}
