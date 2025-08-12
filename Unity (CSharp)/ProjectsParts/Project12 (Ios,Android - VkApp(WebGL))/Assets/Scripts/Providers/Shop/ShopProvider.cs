using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.Providers.Shop.Factorys;
using Game.Scripts.Providers.Shop.Products;
using Game.Scripts.Providers.Shop.Settings;

namespace Game.Scripts.Providers.Shop
{
	public class ShopProvider : IShopProvider
	{
		private readonly ShopSettings _shopSettings;
		private readonly IProductFactory _productFactory;
		private List<IProduct> _productList;
		public List<IProduct> ProductList => _productList;

		public ShopProvider(ShopSettings shopSettings, IProductFactory productFactory)
		{
			_shopSettings = shopSettings;
			_productFactory = productFactory;
		}

		public async UniTask StartAppLoad(IProgress<float> OnProgress, CancellationToken cancellationToken)
		{
			_productList = _productFactory.GetProductList();

			await UniTask.Yield();
		}
	}
}
