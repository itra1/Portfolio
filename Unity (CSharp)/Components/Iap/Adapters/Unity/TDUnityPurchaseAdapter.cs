using System.Collections.Generic;
using Platforms.Iap.Adapters.Unity.Base;
using Platforms.Iap.Adapters.Unity.Common;
using ShopProducts.Base;
using UnityEngine;
using UnityEngine.Events;

namespace Platforms.Iap.Adapters.Unity
{
	public class TDUnityPurchaseAdapter : ITDPurchaseAdapter
	{
		public const string LogKey = "[Unity In-App Purchses]";

		public UnityAction<bool> OnInitializeResult { get; set; }
		public UnityAction<TDPurchaseResult> OnPurchaseResult { get; set; }
		public UnityAction<bool> OnRestoreResult { get; set; }

		private TDUnityIapProcessor _processor;
		private List<TDProduct> _products;

		public bool IsInitialized => _processor != null && _processor.IsInitialized;

		public TDUnityPurchaseAdapter(List<TDProduct> products)
		{
			Debug.Log($"{TDUnityPurchaseAdapter.LogKey} Create Adapter");
			_products = products;
			MakeProcessor();
		}

		private void MakeProcessor()
		{
#if UNITY_ANDROID
			_processor = new TDGoogleIapProcessor();
#elif UNITY_IOS
			_processor = new TDAppleIapProcessor();
#else
			throw new System.NotImplementedException();
#endif

			_processor.SetProducts(_products);
			_processor.OnPurchaseResult = PurchaseResultCallback;
			_processor.OnRestorePurchases = RestorePurchasesCallback;
			_processor.OnInitializeComplete = InitializeCompleteCallback;
		}

		public bool Purchase(string productId)
		{
			try
			{
				return _processor.Purchase(productId);
			}
			catch (System.Exception ex)
			{
				UnityEngine.Debug.LogError($"{TDUnityPurchaseAdapter.LogKey} purchase error: {ex.Message}");
				return false;
			}
		}

		public void RestorePurchases()
		{
			try
			{
				_processor.RestorePurchases();
			}
			catch (System.Exception ex)
			{
				UnityEngine.Debug.LogError($"{TDUnityPurchaseAdapter.LogKey} Restore purchase error: {ex.Message}");
			}
		}

		private void PurchaseResultCallback(TDPurchaseResult purchaseResult)
		{
			OnPurchaseResult?.Invoke(purchaseResult);
		}

		private void RestorePurchasesCallback(bool result)
		{
			OnRestoreResult?.Invoke(result);
		}

		private void InitializeCompleteCallback(bool result)
		{
			OnInitializeResult?.Invoke(result);
		}

		public float GetPrice(string productId)
		{
			return _processor.GetPrice(productId);
		}

		public string GetPriceString(string productId)
		{
			return _processor.GetPriceString(productId);
		}
	}
}
