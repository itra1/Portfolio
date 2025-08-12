using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Core.Engine.Components.Shop;
using Cysharp.Threading.Tasks;
using Game.Providers.Shop.Settings;
using UnityEngine;
using Zenject;

namespace Game.Providers.Shop
{
	public class ShopProvider : IShopProvider
	{
		private readonly IShopSettings _shopSettings;
		private readonly DiContainer _diContainer;
		private readonly List<IShopProduct> _items = new();
		public bool IsLoaded { get; private set; }

		private PlatformSystemPlatformType Platform
		{
			get
			{
#if UNITY_IOS
		return PlatformSystemPlatformType.iOS;
#elif UNITY_ANDROID
				return PlatformSystemPlatformType.Android;
#elif UNITY_STANDALONE_WIN
				return PlatformSystemPlatformType.Window;
#elif UNITY_STANDALONE_OSX
				return PlatformSystemPlatformType.Mac;
#elif UNITY_WEBGL
				return PlatformSystemPlatformType.WebGl;
#endif
			}
		}

		public List<IShopProduct> Items => _items;

		public ShopProvider(IShopSettings shopSettings, DiContainer diContainer)
		{
			_shopSettings = shopSettings;
			_diContainer = diContainer;
		}

		public async UniTask FirstLoad(IProgress<float> OnProgress, CancellationToken cancellationToken)
		{
			FindItems();
			await UniTask.Yield();
		}

		private void FindItems()
		{
			var items = Resources.LoadAll<ShopProduct>(_shopSettings.ShopProductsPath).ToList();

			items = items.OrderBy(x => x.Price).ToList();

			foreach (var item in items)
			{
				if (item.TryGetComponent<IShopProduct>(out var itm) && (itm.Platform & Platform) != 0)
				{
					_items.Add(itm);
					_diContainer.Inject(itm);
				}
			}
		}
	}
}
