using UnityEngine;
using UnityEngine.Purchasing;

namespace Assets.Scripts.Billing {

	public abstract class BillingProductAbstract : MonoBehaviour {

		public string title;
		public string description;

		public Product product;

		public BillingProductType type;
		public ProductType productType;

		[HideInInspector]
		public decimal price = 3;
		[HideInInspector]
		public string cur;
		
		public string productId {
			get {
#if UNITY_IOS
			return iosId;
#elif UNITY_ANDROID
			return androidId;
#else
			return id;
#endif
			}
		}
		
		public string id;							// Идентификатор продукта
		public string iosId;					// Идентификатор продукта Ios
		public string androidId;			// Идентификатор продукта Android

		public virtual void Bye(bool isRestore = false) {

			Debug.Log("Buy");
			
			if (!isRestore) {
				YAppMetrica.Instance.ReportEvent("Магазин: куплено " + LanguageManager.GetTranslate(title));
				GAnalytics.Instance.LogEvent("Магазин", "Покупка", LanguageManager.GetTranslate(title), 1);
			}

			//if(!isRestore)
			//	FirebaseManager.Instance.LogEvent("bye_" + productId);
		}

	}


}