using UnityEngine.Purchasing;

namespace Platforms.Iap.Adapters.Unity.Common
{
	/// <summary>
	/// Процессор для Apple устройств
	/// </summary>
	public class TDAppleIapProcessor : TDUnityIapProcessor
	{
		IAppleExtensions _storeExtension;

		protected override int InitializeRetryMaxCount => 5;

		protected override string ProductName() => "Apple";

		protected override void OnInitializedCompleteCallback()
		{
			base.OnInitializedCompleteCallback();
			_storeExtension = _storeListener.ExtenssionProovider.GetExtension<IAppleExtensions>();
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
