using UnityEngine.Events;
using UnityEngine.Purchasing;

namespace Platforms.Iap.Adapters.Unity.Listeners
{
	public interface ITDStoreListener : IDetailedStoreListener
	{
		UnityAction OnInitializedCompleteCallback { get; set; }
		UnityAction<InitializationFailureReason> OnInitializedFailedCallback { get; set; }
		UnityAction<PurchaseEventArgs> OnProcessPurchaseAction { get; set; }
		UnityAction<Product, PurchaseFailureReason> OnPurchaseFailedCallback { get; set; }

		IStoreController StoreController { get; }
		IExtensionProvider ExtenssionProovider { get; }
	}
}
