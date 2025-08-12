using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintAnyLetterProduct : BillingProductAbstract {

	public override void Bye(bool isRestore = false) {

		if(!this.isActive) return;

		if (product.hasReceipt) {
			NetManager.Instance.CheckPayment(product.receipt, (result) => {

				if (!result) {
					Debug.Log("Платеж не подтвержден " + (typeof(HintAnyLetterProduct)).ToString());
					return;
				}

				base.Bye(isRestore);
				PlayerManager.Instance.hintAnyLetter += count;
			}, () => {
				Debug.Log("Ошибка подтверждения платежа " + (typeof(HintAnyLetterProduct)).ToString());
			});

		}
	}
}
