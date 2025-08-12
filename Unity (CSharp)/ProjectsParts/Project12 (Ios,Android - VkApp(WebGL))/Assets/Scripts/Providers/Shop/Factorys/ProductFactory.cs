using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Game.Scripts.Providers.Shop.Products;
using Game.Scripts.Providers.Shop.Settings;
using itra.Attributes;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Providers.Shop.Factorys
{
	public class ProductFactory : IProductFactory
	{
		private Dictionary<string, Type> _productTypes = new();
		private List<ProductPropertyBase> _productInfoList = new();
		private DiContainer _diContainer;
		private ShopSettings _settings;

		public ProductFactory(DiContainer diContainer, ShopSettings settings)
		{
			_diContainer = diContainer;
			_settings = settings;

			LoadProductsInfo();
			FindClasses();
		}

		private void LoadProductsInfo()
		{
			_productInfoList = Resources.LoadAll<ProductPropertyBase>(_settings.ProductPath).ToList();
		}

		private void FindClasses()
		{
			var classes = (from t in Assembly.GetExecutingAssembly().GetTypes()
										 where t.IsClass
											 && !t.IsAbstract
											 && t.IsSubclassOf(typeof(Product))
										 select t).ToList();
			_productTypes.Clear();

			foreach (var item in classes)
			{
				var className = item.GetCustomAttribute<PrefabNameAttribute>().Name;
				_productTypes.Add(className, item);
			}
		}

		public List<IProduct> GetProductList()
		{
			List<IProduct> result = new();

			foreach (var item in _productInfoList)
			{
				if (!_productTypes.ContainsKey(item.Type))
				{
					AppLog.LogError($"No exists shop product class by type {item.Type}");
					continue;
				}
				var instanceProduct = (Product) Activator.CreateInstance(_productTypes[item.Type]);
				_diContainer.Inject(instanceProduct);
				instanceProduct.SetProductProperty(item);
				result.Add(instanceProduct);
			}

			return result;
		}
	}
}
