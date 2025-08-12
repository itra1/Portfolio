using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

namespace Assets.Scripts.Billing {
	
	/// <summary>
	/// Биллинг контроллер
	/// </summary>
	public class BillingController : Singleton<BillingController>, IStoreListener {

		public List<BillingProductAbstract> productLidt;

		public static event Action OnChangeStatus;
		public bool _isProcessBye;

		public bool isProcessBye {
			get { return _isProcessBye; }
			set {
				if (_isProcessBye == value) return;
				_isProcessBye = value;
				if (OnChangeStatus != null) OnChangeStatus();
			}
		}

		private bool _isInizialized = false;

		public bool isInizialized {
			get { return _isInizialized; }
			set {
				_isInizialized = value;
				//ExEvent.GameEvents.OnBillingInit.Call(_isInizialized);
			}
		}

		public static event Action OnCompleted;

		public List<BillingProductAbstract> GetBillingProduct(BillingProductType type) {
			return productLidt.FindAll(x => x.type == type);
		}

		/// <summary>
		/// Выполнение продукта
		/// </summary>
		/// <param name="id">Идентификатор продукта</param>
		public void ByeProduct(string id, Action Callback = null) {

			var prod = productLidt.Find(x => x.id == id);
			if (prod != null)
				ByeProduct(prod, Callback);
		}
		
		public void ByeProduct(BillingProductAbstract product, Action Callback = null) {
			BuyProductID(product.productId, Callback);
		}

		private static IStoreController m_StoreController;
		private static IExtensionProvider m_StoreExtensionProvider;

		private void Start() {
			if (m_StoreController == null) {
				InitializePurchasing();
			}
		}

		public void InitializePurchasing() {

			if (IsInitialized())
				return;

      var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

      productLidt.ForEach((prod) => {
        builder.AddProduct(prod.productId, prod.productType);
      });

      UnityPurchasing.Initialize(this, builder);

    }

    public string GetTransactionToken(int locationNum) {
			//try {
			//	var allLoca = GetBillingProduct(IapType.locationAll)[0];

			//	if (allLoca.product.hasReceipt) {
			//		byte[] toBytes = Encoding.UTF8.GetBytes(ParceReceipt(allLoca.product.receipt));
			//		//Debug.Log(System.Convert.ToBase64String(toBytes));
			//		return System.Convert.ToBase64String(toBytes);
			//	}

			//} catch {
			//	return "";
			//}
			return "";
		}

		private bool IsInitialized() {
			return m_StoreController != null && m_StoreExtensionProvider != null;
		}

		void BuyProductID(string productId, Action Callback) {

			callbackOrder.Add(new CallBackOrder() {
				productId = productId,
				Callback = Callback
			});

			StartCoroutine(BuyProductIDAsyn(productId));
		}

		private IEnumerator BuyProductIDAsyn(string productId) {
			isProcessBye = true;
			yield return null;
			if (IsInitialized()) {
				Product product = m_StoreController.products.WithID(productId);

				if (product != null && product.availableToPurchase) {
					Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
					m_StoreController.InitiatePurchase(product);
				} else {
					Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
				}
			} else {
				Debug.Log("BuyProductID FAIL. Not initialized.");
			}
		}

		public void RestorePurchases() {

			//#if UNITY_EDITOR
			//		RestoreEditor();
			//		return;
			//#endif

			if (!IsInitialized()) {
				Debug.Log("RestorePurchases FAIL. Not initialized.");
				return;
			}

			if (Application.platform == RuntimePlatform.IPhonePlayer ||
					Application.platform == RuntimePlatform.OSXPlayer) {

				Debug.Log("RestorePurchases started ...");

        var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();

        apple.RestoreTransactions((result) => {

          if (result) {

            productLidt.ForEach((elem) => {
              if (elem.product.hasReceipt && elem.product.definition.type != ProductType.Consumable) {
                elem.Bye();
              }
            });

            Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
          }
        });

      } else {

				Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
			}
		}

		private void RestoreEditor() {

			//PlayerManager.Instance.company.billingRestore = true;
			productLidt.ForEach((elem) => {
				if (elem.productType != ProductType.Consumable) {
					elem.Bye();
				}
			});
			//PlayerManager.Instance.company.billingRestore = false;
			//PlayerManager.Instance.company.CheckDownloadIfNeed();
		}

		public bool androidRestore;

		public void OnInitialized(IStoreController controller, IExtensionProvider extensions) {
			Debug.Log("OnInitialized: PASS");

			m_StoreController = controller;
			m_StoreExtensionProvider = extensions;

			//PlayerManager.Instance.company.billingRestore = true;
			productLidt.ForEach((prod) => {

				Product product = m_StoreController.products.WithID(prod.productId);
				prod.price = product.metadata.localizedPrice;
				prod.cur = product.metadata.isoCurrencyCode;
				prod.product = product;
				//androidRestore = true;

#if !UNITY_EDITOR
			//try {
			//	if (prod.productType != ProductType.Consumable &&
			//	    ((LocationProduct) prod).locationNum == 1) {
			//		Debug.Log(((LocationProduct)prod).locationNum);
			//		prod.Bye();
			//	}
			//}
			//catch {
			//}
#else

				if (product.hasReceipt) {
					if (prod.productType != ProductType.Consumable) {
						prod.Bye();
					}
				}

#endif

			});
			//PlayerManager.Instance.company.billingRestore = false;
			//PlayerManager.Instance.company.CheckDownloadIfNeed();

			isInizialized = true;
		}

		public void OnInitializeFailed(InitializationFailureReason error) {
			Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
		}

		public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args) {

			productLidt.ForEach((prod) => {
				if (String.Equals(args.purchasedProduct.definition.id, prod.productId, StringComparison.Ordinal)) {
					Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
					prod.Bye();
					try {
						//FirebaseManager.Instance.LogEvent("purchase");
					} catch {
					}
					isProcessBye = false;

					for (int i = 0; i < callbackOrder.Count; i++) {
						if (prod.productId == callbackOrder[i].productId) {
							if (callbackOrder[i].Callback != null) callbackOrder[i].Callback();
							callbackOrder.RemoveAt(i);
							break;
						}
					}
				}

			});

			if (OnCompleted != null) OnCompleted();
			return PurchaseProcessingResult.Complete;
		}

		public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason) {
			Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}",
				product.definition.storeSpecificId, failureReason));

			if (failureReason == PurchaseFailureReason.DuplicateTransaction)

				for (int i = 0; i < callbackOrder.Count; i++) {
					if (product.definition.storeSpecificId == callbackOrder[i].productId) {
						if (callbackOrder[i].Callback != null) callbackOrder[i].Callback();
						callbackOrder.RemoveAt(i);
						break;
					}
				}

			isProcessBye = false;

			if (OnCompleted != null) OnCompleted();
#if UNITY_IOS
		if (product.definition.type != ProductType.Consumable) {
			RestorePurchases();
		}
#endif
		}

		private List<CallBackOrder> callbackOrder = new List<CallBackOrder>();

		class CallBackOrder {
			public string productId;
			public Action Callback;
		}

		public string ParceReceipt(string source) {

			ReceitLevel1 receitLevel1 = JsonUtility.FromJson<ReceitLevel1>(source);
			ReceitLevel2 receitLevel2 = JsonUtility.FromJson<ReceitLevel2>(receitLevel1.Payload);
			ReceitLevel3 receitLevel3 = JsonUtility.FromJson<ReceitLevel3>(receitLevel2.json);
			ResultReceipt2 level2 = new ResultReceipt2();
			level2.json = receitLevel3;
			level2.signature = receitLevel2.signature;

			ResultReceipt level1 = new ResultReceipt();
			level1.Payload = level2;
			level1.Store = receitLevel1.Store;
			level1.TransactionID = receitLevel1.TransactionID;
			return Newtonsoft.Json.JsonConvert.SerializeObject(level1);
		}

		[System.Serializable]
		private class ReceitLevel1 {
			public string Store;
			public string TransactionID;
			public string Payload;
		}

		[System.Serializable]
		private class ReceitLevel2 {
			public string json;
			public string signature;
		}

		[System.Serializable]
		private class ReceitLevel3 {
			public string orderId;
			public string packageName;
			public string productId;
			public long purchaseTime;
			public string purchaseState;
			public string purchaseToken;
		}

		[System.Serializable]
		private class ResultReceipt {
			public string Store;
			public string TransactionID;
			public ResultReceipt2 Payload;
		}
		[System.Serializable]
		private class ResultReceipt2 {
			public ReceitLevel3 json;
			public string signature;
		}

	}

	public enum BillingProductType {
		gold,
		energy,
		special,
    heart
	}

}