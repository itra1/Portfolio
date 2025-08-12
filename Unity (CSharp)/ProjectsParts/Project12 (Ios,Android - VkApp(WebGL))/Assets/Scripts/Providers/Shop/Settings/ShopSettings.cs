using System.Collections.Generic;
using Game.Scripts.UI.Settings;
using Game.Scripts.UI.Shop;
using UnityEngine;

namespace Game.Scripts.Providers.Shop.Settings
{
	[System.Serializable]
	public class ShopSettings
	{
		[SerializeField] private string _productPath;
		[SerializeField] private List<ProductButtonUi> _shopButtons;
		[SerializeField] private ProductButtonColorsUi _colorsSettings;

		public string ProductPath => _productPath;
		public List<ProductButtonUi> ShopButtons => _shopButtons;
		public ProductButtonColorsUi ColorsSettings => _colorsSettings;
	}
}
