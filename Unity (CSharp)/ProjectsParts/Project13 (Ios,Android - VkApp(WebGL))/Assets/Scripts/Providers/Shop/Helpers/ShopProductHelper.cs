using System.Collections.Generic;
using System.Linq;
using Game.Providers.Shop.Settings;
using UnityEngine;

namespace Game.Providers.Shop.Helpers
{
	public class ShopProductHelper : IShopProductHelper
	{
		private readonly IShopSettings _shopSettings;
		private List<ShopProduct> _productList = new();

		public ShopProductHelper(IShopSettings shopSettings)
		{
			_shopSettings = shopSettings;
			LoadProductsFromResources();

			Debug.Log($"ShopProductCount {_productList.Count}");
		}

		private void LoadProductsFromResources()
		{
			_productList = Resources.LoadAll<ShopProduct>(_shopSettings.ShopProductsPath).ToList();
		}
	}
}
