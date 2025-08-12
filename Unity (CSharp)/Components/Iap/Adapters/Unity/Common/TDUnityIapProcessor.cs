using System.Collections.Generic;
using System.Threading.Tasks;
using Platforms.Iap.Adapters.Unity.Base;
using Platforms.Iap.Adapters.Unity.Listeners;
using ShopProducts.Base;
using ShopProducts.Common;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Purchasing;

namespace Platforms.Iap.Adapters.Unity.Common
{
	/// <summary>
	/// Базовый процессор для Unity In-App Purchases
	/// </summary>
	public abstract class TDUnityIapProcessor
	{
		public UnityAction<bool> OnInitializeComplete;
		public UnityAction<TDPurchaseResult> OnPurchaseResult;
		public UnityAction<bool> OnRestorePurchases;

		protected List<TDProduct> _products;
		protected ITDStoreListener _storeListener;

		private ConfigurationBuilder _builder;

		/// <summary>
		/// The current repetition of initialization
		/// </summary>
		private int _currentInitializeRetry = 0;

		public bool IsInitialized
			=> _storeListener != null && _storeListener.StoreController != null;

		public IStoreController StoreController => _storeListener.StoreController;

		/// <summary>
		/// The maximum number of attempts to connect
		/// </summary>
		protected abstract int InitializeRetryMaxCount { get; }

		public void SetProducts(List<TDProduct> products)
		{
			_currentInitializeRetry = 0;
			_products = products;
			Initialize();
		}

		private void Initialize()
		{
			_currentInitializeRetry++;
			_builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

			_storeListener = new TDUnityIapProcessorListener
			{
				OnInitializedCompleteCallback = OnInitializedCompleteCallback,
				OnInitializedFailedCallback = OnInitializedFailedCallback,
				OnProcessPurchaseAction = OnProcessPurchaseAction,
				OnPurchaseFailedCallback = OnPurchaseFailedCallback
			};

			_products.ForEach(product =>
			{
				_ = _builder.AddProduct(product.ProductId, GetProductType(product.Type));
			});
			UnityPurchasing.Initialize(_storeListener, _builder);
		}

		protected virtual void OnInitializedCompleteCallback()
		{
			Debug.Log($"{TDUnityPurchaseAdapter.LogKey} Initialized Complete, retry index {_currentInitializeRetry}");
			OnInitializeComplete?.Invoke(true);
			PringProducts();
		}

		private void OnInitializedFailedCallback(InitializationFailureReason reason)
		{
			Debug.LogError($"{TDUnityPurchaseAdapter.LogKey} Initialized Failed {reason.ToString()}, retry index {_currentInitializeRetry}");
			if (_currentInitializeRetry <= InitializeRetryMaxCount)
			{
				_ = RepeatInit();
				return;
			}

			OnInitializeComplete?.Invoke(false);
		}

		private async Task RepeatInit()
		{
			await Task.Delay(50);
			Initialize();
		}

		private void PringProducts()
		{
			Debug.Log($"{TDUnityPurchaseAdapter.LogKey} Products");

			foreach (var item in _products)
			{
				var product = StoreController.products.WithID(item.ProductId);
				if (product == null)
				{
					Debug.Log($"{TDUnityPurchaseAdapter.LogKey} no exists product {item.Id} with id {item.ProductId}");
				}
				else
				{
					Debug.Log($"{TDUnityPurchaseAdapter.LogKey} product {item.Id} data {Newtonsoft.Json.JsonConvert.SerializeObject(product)}");
				}
			}
		}

		private void OnPurchaseFailedCallback(Product product, PurchaseFailureReason reson)
		{
			var appProduct = _products.Find(x => x.ProductId == product.definition.id);

			OnPurchaseResult?.Invoke(new TDPurchaseResult
			{
				IsSuccess = false,
				ProductId = appProduct.Id,
				StoreName = ProductName(),
				StoreId = product.definition.id,
				LocalizedPrice = product.metadata.localizedPrice,
				IsoCurrencyCode = product.metadata.isoCurrencyCode,
			});
		}

		private void OnProcessPurchaseAction(PurchaseEventArgs purchaseArgs)
		{
			var productStoreId = purchaseArgs.purchasedProduct.definition.id;
			var appProduct = _products.Find(x => x.ProductId == productStoreId);
			OnPurchaseResult?.Invoke(new TDPurchaseResult
			{
				IsSuccess = true,
				ProductId = appProduct.Id,
				StoreName = ProductName(),
				StoreId = productStoreId,
				LocalizedPrice = purchaseArgs.purchasedProduct.metadata.localizedPrice,
				IsoCurrencyCode = purchaseArgs.purchasedProduct.metadata.isoCurrencyCode,
				Receipt = purchaseArgs.purchasedProduct.receipt
			});
		}

		/// <summary>
		/// Запуск покупок
		/// </summary>
		/// <param name="productId">Идентификатор продукта</param>
		/// <returns></returns>
		public bool Purchase(string productId)
		{
			var appProduct = _products.Find(x => x.Id == productId);

			Product product = StoreController.products.WithID(appProduct.ProductId);
			if (product == null)
			{
				Debug.LogError($"{TDUnityPurchaseAdapter.LogKey} Cant find product {productId} store id {appProduct.ProductId}");
				return false;
			}

			if (!product.availableToPurchase)
			{
				Debug.LogError($"{TDUnityPurchaseAdapter.LogKey} Product {productId} store id {appProduct.ProductId} not available ");
				return false;
			}

			StoreController.InitiatePurchase(appProduct.ProductId);

			return true;
		}

		/// <summary>
		/// Восстановление покупок
		/// </summary>
		public abstract void RestorePurchases();

		private ProductType GetProductType(string type)
			=> type switch
			{
				TDProductType.Subscription => ProductType.Subscription,
				TDProductType.NonConsumable => ProductType.NonConsumable,
				_ => ProductType.Consumable, // Consumable
			};

		protected abstract string ProductName();

		protected void EmitRestorePurchases(bool isSuccess)
		{
			OnRestorePurchases?.Invoke(isSuccess);
		}
		public float GetPrice(string productId)
		{
			var appProduct = _products.Find(x => x.Id == productId);

			Product product = StoreController.products.WithID(appProduct.ProductId);
			if (product == null)
			{
				Debug.LogError($"{TDUnityPurchaseAdapter.LogKey} Cant find product {productId} store id {appProduct.ProductId}");
				return appProduct.Price;
			}

			return (float) product.metadata.localizedPrice;
		}

		public string GetPriceString(string productId)
		{
			var appProduct = _products.Find(x => x.Id == productId);

			Product product = StoreController.products.WithID(appProduct.ProductId);
			if (product == null)
			{
				Debug.LogError($"{TDUnityPurchaseAdapter.LogKey} Cant find product {productId} store id {appProduct.ProductId}");
				return appProduct.Price.ToString();
			}

			return product.metadata.localizedPriceString;
		}
	}
}
