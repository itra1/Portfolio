using UnityEngine.Purchasing;

namespace Platforms.Iap.Adapters.Unity.Common
{
	/// <summary>
	/// Процессор для мобильных устройств с Google платформой
	/// </summary>
	public class TDGoogleIapProcessor : TDUnityIapProcessor
	{
		IGooglePlayStoreExtensions _storeExtension;

		protected override int InitializeRetryMaxCount => 5;

		protected override string ProductName() => "Google";

		protected override void OnInitializedCompleteCallback()
		{
			base.OnInitializedCompleteCallback();
			_storeExtension = _storeListener.ExtenssionProovider.GetExtension<IGooglePlayStoreExtensions>();
		}

		public override void RestorePurchases()
		{
			_storeExtension.RestoreTransactions(OnRestore);
		}

		private void OnRestore(bool success, string error)
		{
			EmitRestorePurchases(success);
		}

	}
}
