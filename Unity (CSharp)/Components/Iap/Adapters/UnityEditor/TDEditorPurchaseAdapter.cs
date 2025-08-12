using System.Collections.Generic;
using Platforms.Iap.Adapters.Unity.Base;
using ShopProducts.Base;
using UnityEngine;
using UnityEngine.Events;

namespace Platforms.Iap.Adapters.UnityEditor
{
	public class TDEditorPurchaseAdapter : ITDPurchaseAdapter
	{
		public const string LogKey = "[Editor In-App Purchses]";

		public UnityAction<bool> OnInitializeResult { get; set; }
		public UnityAction<TDPurchaseResult> OnPurchaseResult { get; set; }
		public UnityAction<bool> OnRestoreResult { get; set; }

		private List<TDProduct> _products;

		public bool IsInitialized => true;

		public TDEditorPurchaseAdapter(List<TDProduct> products)
		{
			_products = products;
		}

		public bool Purchase(string productId)
		{
			var appProduct = GetProductById(productId);

			if (appProduct == null)
			{
				Debug.LogError($"{TDEditorPurchaseAdapter.LogKey} No product with an id {productId}");
				return false;
			}

			OnPurchaseResult?.Invoke(new TDPurchaseResult()
			{
				IsSuccess = true,
				StoreName = "Editor",
				ProductId = productId,
				StoreId = productId,
				IsoCurrencyCode = "EDIT",
				LocalizedPrice = (decimal) appProduct.Price
			});
			return true;
		}

		public TDProduct GetProductById(string id)
			=> _products.Find(x => x.Id == id);

		public void RestorePurchases()
		{
			OnRestoreResult?.Invoke(true);
		}

		public float GetPrice(string productId)
		{
			var appProduct = GetProductById(productId);

			if (appProduct == null)
			{
				Debug.LogError($"{TDEditorPurchaseAdapter.LogKey} No product with an id {productId}");
				return 0;
			}
			return appProduct.Price;
		}

		public string GetPriceString(string productId)
		{
			return GetPrice(productId).ToString();
		}
	}
}
