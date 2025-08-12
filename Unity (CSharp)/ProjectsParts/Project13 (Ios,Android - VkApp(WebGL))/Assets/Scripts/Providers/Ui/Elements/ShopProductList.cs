using System.Collections.Generic;
using Core.Engine.Components.Shop;
using Game.Base;
using Game.Providers.Shop.Products;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Providers.Ui.Elements
{
	public class ShopProductList : MonoBehaviour, IInjection
	{
		[SerializeField] private ShopProductPresenter _prefab;
		[SerializeField] private ScrollRect _scrollRect;

		private readonly List<ShopProductPresenter> _productList = new();
		private IShopProvider _shopProvider;

		[Inject]
		public void Constructor(IShopProvider shopProvider)
		{
			_shopProvider = shopProvider;
		}

		public void Show()
		{
			if (_productList.Count == 0)
				CreateProducts();
		}

		public void Hide()
		{

		}

		private void CreateProducts()
		{
			var products = _shopProvider.Items.FindAll(x => x.GetType() == typeof(WeaponCoinsShopProduct));

			for (int i = 0; i < products.Count; i++)
			{
				var instance = MonoBehaviour.Instantiate(_prefab, _scrollRect.content);
				instance.SetData(products[i]);
				_productList.Add(instance);
			}
		}
	}
}
