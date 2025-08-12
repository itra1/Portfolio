using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinsProduct : BillingProductAbstract {
  
  public override void Bye(bool isRestore = false) {

    if (!this.isActive) return;

    if (product.hasReceipt) {
      NetManager.Instance.CheckPayment(product.receipt, (result) => {

        if (!result) {
          Debug.Log("Платеж не подтвержден " + this.GetType().ToString());
          return;
        }

        base.Bye(isRestore);
        PlayerManager.Instance.coins += (int)(count * Shop.Instance.countCoeff);
        PlayerManager.Instance.noAds = true;
        PlayerManager.Instance.Save();
        Shop.Instance.ByeComplete();
      }, () => {
        Debug.Log("Ошибка подтверждения платежа " + this.GetType().ToString());
      });

    }
  }
}
