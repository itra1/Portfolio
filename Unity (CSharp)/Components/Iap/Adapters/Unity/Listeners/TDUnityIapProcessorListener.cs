using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

namespace Platforms.Iap.Adapters.Unity.Listeners
{
	public class TDUnityIapProcessorListener : ITDStoreListener
	{
		public UnityAction OnInitializedCompleteCallback { get; set; }
		public UnityAction<InitializationFailureReason> OnInitializedFailedCallback { get; set; }
		public UnityAction<PurchaseEventArgs> OnProcessPurchaseAction { get; set; }
		public UnityAction<Product, PurchaseFailureReason> OnPurchaseFailedCallback { get; set; }

		public IStoreController StoreController { get; private set; }
		public IExtensionProvider ExtenssionProovider { get; private set; }

		public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
		{
			StoreController = controller;
			ExtenssionProovider = extensions;
			OnInitializedCompleteCallback?.Invoke();
		}

		public void OnInitializeFailed(InitializationFailureReason error)
		{
			MonoBehaviour.print($"{TDUnityPurchaseAdapter.LogKey} Unity iap initialize error {error}");
			OnInitializeFailed(error, null);
		}

		public void OnInitializeFailed(InitializationFailureReason error, string message)
		{
			MonoBehaviour.print($"{TDUnityPurchaseAdapter.LogKey} Unity iap initialize error {error} messssage {message}");
			OnInitializedFailedCallback?.Invoke(error);
		}

		public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
		{
			OnPurchaseFailed(product, failureDescription.reason);
		}

		public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
		{
			OnPurchaseFailedCallback?.Invoke(product, failureReason);
		}

		public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
		{
			OnProcessPurchaseAction?.Invoke(purchaseEvent);

			return PurchaseProcessingResult.Complete;
		}
	}
}
