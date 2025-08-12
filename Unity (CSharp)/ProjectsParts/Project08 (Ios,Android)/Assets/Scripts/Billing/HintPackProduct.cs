using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintPackProduct : BillingProductAbstract {
	
	public int countRandBack;

	public override void Bye(bool isRestore = false) {

		if (!this.isActive) return;

		if (product.hasReceipt) {
			NetManager.Instance.CheckPayment(product.receipt, (result) => {
				if (!result) {
					Debug.Log("Платеж не подтвержден " + (typeof(HintPackProduct)).ToString());
					return;
				}

				base.Bye(isRestore);

				for (int i = 0; i < countRandBack; i++) {

					int num = UnityEngine.Random.Range(0, 3);

					HintType useType = (HintType)num;

					switch (useType) {
						case HintType.anyLetter:
							PlayerManager.Instance.hintAnyLetter++;
							break;
						case HintType.firstLetter:
							PlayerManager.Instance.hintFirstLetter++;
							break;
						case HintType.firstWord:
							PlayerManager.Instance.hintFirstWord++;
							break;
					}
				}

			}, () => {
				Debug.Log("Ошибка подтверждения платежа " + (typeof(HintPackProduct)).ToString());
			});

		}


		
	}

	[System.Serializable]
	public struct ProductData {
		public HintType type;
		public int count;
	}

}
