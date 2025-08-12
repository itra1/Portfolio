using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Core.Engine.Components.Shop {
	public class ShopProvider : IShopProvider {
		private readonly IShopSettings _shopSettings;
		private readonly DiContainer _diContainer;
		private readonly List<IShopProduct> _items = new();

		private PlatformSystemPlatformType Platform {
			get {
#if UNITY_IOS
		return PlatformSystemPlatformType.iOS;
#elif UNITY_ANDROID
				return PlatformSystemPlatformType.Android;
#elif UNITY_STANDALONE_WIN
				return PlatformSystemPlatformType.Window;
#elif UNITY_STANDALONE_OSX
				return PlatformSystemPlatformType.Mac;
#elif UNITY_WEBGL
				return PlatformSystemPlatformType.WebGL;
#endif
			}
		}

		public List<IShopProduct> Items => _items;

		public ShopProvider(IShopSettings shopSettings, DiContainer diContainer) {
			_shopSettings = shopSettings;
			_diContainer = diContainer;
			FindItems();
		}

		private void FindItems() {
			var items = Resources.LoadAll<ShopProduct>(_shopSettings.ShopProductsFolder).ToList();

			items = items.OrderBy(x => x.Price).ToList();

			foreach (var item in items) {
				if (item.TryGetComponent<IShopProduct>(out var itm) && (itm.Platform & Platform) != 0) {
					_items.Add(itm);
					_diContainer.Inject(itm);
				}
			}


			AppLog.Log($"Products count {_items.Count}");
		}


	}
}
