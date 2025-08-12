using UnityEngine;

namespace Game.Providers.Shop.Settings
{
	[System.Serializable]
	public class ShopSettings : IShopSettings
	{
		[SerializeField]
		private string _shopProductsPath;

		public string ShopProductsPath => _shopProductsPath;
	}
}
