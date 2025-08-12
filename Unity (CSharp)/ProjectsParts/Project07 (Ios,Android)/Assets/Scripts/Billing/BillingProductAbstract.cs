using UnityEngine;
using UnityEngine.Purchasing;

public abstract class BillingProductAbstract : MonoBehaviour {

	public string title;
	public string description;

	public bool isActive = true;

	public Product product;

	public IapType type;
	public ProductType productType;

	public decimal price = 3;
	public string cur;

	public int count;

	public string productId {
		get {
#if UNITY_IOS
			return iosId;
#elif UNITY_ANDROID
			return androidId;
#else
			return "";
#endif
		}
	}

	[SerializeField]
	private string iosId;					// Идентификатор продукта Ios

	[SerializeField]
	private string androidId;			// Идентификатор продукта Android

	public virtual void Bye(bool isRestore = false) {
		Debug.Log("Bye");
	}

}
