using System;
using System.Collections.Generic;
using Firebase.Analytics;
using Platforms.Iap.Adapters.Unity.Base;
using ShopProducts.Base;
using UnityEngine;
using UnityEngine.Events;

namespace Platforms.Iap
{
	public partial class TDIapProvider
	{
		private Dictionary<string, UnityAction<bool>> _callbacks = new();

		/// <summary>
		/// Запуск процесса покупки
		/// </summary>
		/// <param name="productId">TDProductIds</param>
		public bool Purchase(string productId, UnityAction<bool> CallbackResult = null)
		{
			TimelessTransponder.instance.ShowPurchaseSpinner();
			
			var appProduct = GetProductById(productId);

			if (appProduct == null)
			{
				Debug.LogError($"{TDIapProvider.LogKey} No product with an id {productId}");
				CallbackResult?.Invoke(false);
				return false;
			}

			if (_adapter == null || !_adapter.IsInitialized)
			{
				Debug.LogError($"{TDIapProvider.LogKey} The store adapter is not initialized");
				CallbackResult?.Invoke(false);
				return false;
			}
			try
			{
				if (CallbackResult != null)
					_callbacks.Add(productId, CallbackResult);
				return _adapter.Purchase(productId);
			}
			catch (Exception ex)
			{
				TimelessTransponder.instance.HidePurchaseSpinner();
				Debug.LogError($"{TDIapProvider.LogKey} Error init purchase {productId} store id {appProduct.ProductId}: {ex.Message}");
			}
			return false;
		}

		private void PurchaseResult(TDPurchaseResult purchaseResult)
		{
			TimelessTransponder.instance.HidePurchaseSpinner();
			
			var product = GetProductById(purchaseResult.ProductId);

			if (!purchaseResult.IsSuccess)
			{
				product.Failed();
				EmitPurchaseCallback(product, purchaseResult);
				return;
			}

			//TODO Надо разгрести это все
			MonoBehaviour.print($"{TDIapProvider.LogKey} Paurchase {purchaseResult.ProductId}");

			TDTransactionManager.SetInt("playerAllPurchaseCounter", TDTransactionManager.GetInt("playerAllPurchaseCounter") + 1);
			
			string gpaCode = "";
#if UNITY_ANDROID && !UNITY_EDITOR
			try
			{
				string receipt = purchaseResult.Receipt;
				if (!string.IsNullOrEmpty(receipt))
				{
					var match = System.Text.RegularExpressions.Regex.Match(receipt, @"GPA\.[\d\-]+");
					if (match.Success)
					{
						gpaCode = match.Value;
					}
				}
				else
				{
					gpaCode = "";
				}
			}
			catch (System.Exception ex)
			{
				Debug.LogError("Error extracting GPA code: " + ex.Message);
			}
#endif

			TDMixpanel.PurchaseEvent(
				purchaseResult.StoreId,
				purchaseResult.IsoCurrencyCode,
				(float)purchaseResult.LocalizedPrice,
				product.Category,
				gpaCode // передаем GPA код, если он есть
			);


			// Продукт
			product.Complete();
			EmitPurchaseCallback(product, purchaseResult);

#if UNITY_EDITOR

			if (TimelessTransponder.instance.isSyncEnable)
			{
				TDFirebaseManager.Instance.UploadToCloud(TDAuthentication.Instance.userID);
			}

#else
      TDFirebaseManager.Instance.UploadToCloud(TDAuthentication.Instance.userID);
#endif

			TimelessTransponder.instance.SaveDataToCloud(true);

			FirebaseAnalytics.LogEvent(
				"purchase",
				new Parameter[]
				{
					new ("product_id", product.ProductId),
					new ("price", (float)purchaseResult.LocalizedPrice),
					new ("currency", purchaseResult.IsoCurrencyCode),
					new ("category", product.Category)
				}
			);
		}

		private void EmitPurchaseCallback(TDProduct product, TDPurchaseResult purchaseResult)
		{
			if (!_callbacks.ContainsKey(product.Id))
				return;

			_callbacks[product.Id](purchaseResult.IsSuccess);
			_ = _callbacks.Remove(product.Id);
		}

		public void RestorePurchases()
		{
			_adapter.RestorePurchases();
		}

		public float GetPrice(string productId)
		{
			var appProduct = GetProductById(productId);

			try
			{
				return _adapter.GetPrice(productId);
			}
			catch (Exception ex)
			{
				Debug.LogError($"{TDIapProvider.LogKey} Error get price {productId}: {ex.Message}");
				return appProduct.Price;
			}
		}

		public string GetPriceString(string productId)
		{
			var appProduct = GetProductById(productId);

			try
			{
				return _adapter.GetPriceString(productId);
			}
			catch (Exception ex)
			{
				Debug.LogError($"{TDIapProvider.LogKey} Error get price {productId}: {ex.Message}");
				return appProduct.Price.ToString();
			}
		}


		/// <summary>
		/// Получить дату окончания подписки
		/// </summary>
		/// <param name="productId">Идентификатоор продукта</param>
		/// <returns></returns>
		public DateTime GetSubscriptionExpirationDate(string productId)
		{
			var appProduct = GetProductById(productId);

			if (appProduct == null)
			{
				Debug.LogError($"{TDIapProvider.LogKey} No product with an id {productId}");
				return DateTime.MinValue;
			}

			if(appProduct is not TDSubscription subscriptionProduct){

				Debug.LogError($"{TDIapProvider.LogKey} Product with an id {productId} not a subscription");
				return DateTime.MinValue;
			}

			try
			{

				return subscriptionProduct.IsActiveSubscription() ? subscriptionProduct.ExpireDate : DateTime.MinValue;

			}catch(System.Exception ex)
			{
				Debug.LogError($"{TDIapProvider.LogKey} Error subscription Product with an id {productId} can't get the expiration date {ex.Message}");
				return DateTime.MinValue;
			}
		}
	}
}
