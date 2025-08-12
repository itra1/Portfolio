using UnityEngine;
public class LocationAllProduct : BillingProductAbstract {
	
	public override void Bye(bool isRestore = false) {

		if (!this.isActive) return;

		if (product.hasReceipt) {
			NetManager.Instance.CheckPayment(product.receipt, (result) => {
				if (!result) {
					Debug.Log("Платеж не подтвержден " + (typeof(LocationAllProduct)).ToString());
					return;
				}

				base.Bye(isRestore);

				//throw new NotImplementedException();

				if (PlayerManager.Instance.company.AddByeAllLocation(() => {
					if (!BillingManager.Instance.androidRestore)
						Locker.Instance.SetLocker(false);
				})) {

					if (!BillingManager.Instance.androidRestore)
						Locker.Instance.SetLocker(true);
				}
				ExEvent.PlayerEvents.OnByeLocation.Call(null,true);

			}, () => {
				Debug.Log("Ошибка подтверждения платежа " + (typeof(LocationAllProduct)).ToString());
			});

		}

	}


}
