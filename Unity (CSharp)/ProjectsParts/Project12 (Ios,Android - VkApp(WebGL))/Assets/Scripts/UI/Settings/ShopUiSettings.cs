using System.Collections.Generic;
using Game.Scripts.UI.Shop;
using UnityEngine;

namespace Game.Scripts.UI.Settings
{
	[System.Serializable]
	public class ShopUiSettings
	{
		[SerializeField] private List<ProductButtonUi> _shopButtons;
		[SerializeField] private ProductButtonColorsUi _colorsSettings;

		public List<ProductButtonUi> ShopButtons => _shopButtons;

		public ProductButtonColorsUi ColorsSettings => _colorsSettings;
	}
}
