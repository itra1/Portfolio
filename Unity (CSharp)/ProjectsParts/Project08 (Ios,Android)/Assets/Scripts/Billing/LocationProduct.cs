using UnityEngine;

public class LocationProduct : BillingProductAbstract {

	public int locationNum;

	public override void Bye(bool isRestore = false) {

		if (!this.isActive) return;

		if (product.hasReceipt) {
			NetManager.Instance.CheckPayment(product.receipt, (result) => {
				if (!result) {
					Debug.Log("Платеж не подтвержден " + (typeof(LocationProduct)).ToString());
					return;
				}
				
				base.Bye(isRestore);

				if (PlayerManager.Instance.company.AddByeLocation(locationNum, () => {
					if (!BillingManager.Instance.androidRestore)
						Locker.Instance.SetLocker(false);
				})) {
					if (!BillingManager.Instance.androidRestore)
						Locker.Instance.SetLocker(true);
				}
				ExEvent.PlayerEvents.OnByeLocation.Call(locationNum, true);

			}, () => {
				Debug.Log("Ошибка подтверждения платежа "+ (typeof(LocationProduct)).ToString());
			});

		}

	}
	
}
