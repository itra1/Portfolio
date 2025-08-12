/// Провайдер предоставляет iap
/// 
/// Ресурсы покупок загружаются из каталога прописанного в _resourcesPath
/// 
/// _adapter - Адаптер для работы с целевой системой,

/// Использование In-App Purchases
/// Работает с Google и Apple
/// Не работает с такими магазинами как Huawei store
#define UNITY_IN_APP_PURCHASES

using System.Collections.Generic;
using System.Linq;
using Base.Interfaces;
using Platforms.Iap.Adapters;
using ShopProducts.Base;
using UnityEngine;

namespace Platforms.Iap
{
	/// <summary>
	/// Провайдер, предоставляющий платежи
	/// </summary>
	public partial class TDIapProvider : ITDInitialization
	{
		public const string LogKey = "[TDIapProvider]";

		/// <summary>
		/// Путь к каталогу в ресурсах, где расположены iap'ы
		/// </summary>
		[SerializeField] private const string _resourcesPath = "ShopProducts";

		private List<TDProduct> _products;
		private ITDPurchaseAdapter _adapter;

		public TDIapProvider()
		{
			LoadResources();
		}

		public void Initialization()
		{
			Debug.Log($"{TDIapProvider.LogKey} Initialized");
			CreateAdapter();
		}

		private void CreateAdapter()
		{
			if (_adapter != null)
			{
				Debug.LogWarning($"{TDIapProvider.LogKey} Adapter already created, skipping.");
				return;
			}
			
#if UNITY_EDITOR
			_adapter = new Adapters.UnityEditor.TDEditorPurchaseAdapter(_products);
#elif UNITY_IN_APP_PURCHASES
			_adapter = new Adapters.Unity.TDUnityPurchaseAdapter(_products);
#endif

			_adapter.OnPurchaseResult = PurchaseResult;
		}

		/// <summary>
		/// Загружаем ресурсы
		/// </summary>
		private void LoadResources()
		{
			_products = Resources.LoadAll<TDProduct>(_resourcesPath).ToList();
		}

		public TDProduct GetProductById(string id)
			=> _products.Find(x => x.Id == id);

#if UNITY_EDITOR
		/// <summary>
		/// Вывод в лог списка
		/// </summary>
		[UnityEditor.MenuItem("Tools/Shop/Print products")]
		public static void PrintProducts()
		{
			System.Text.StringBuilder results = new();
			_ = results.Append("Product list:\n");
			var products = Resources.LoadAll<TDProduct>(_resourcesPath).OrderBy(x => x.Id).ToList();

			products.ForEach(x => results.Append($"{x.ToString()}\n"));

			Debug.Log(results.ToString());
		}
#endif
	}
}
