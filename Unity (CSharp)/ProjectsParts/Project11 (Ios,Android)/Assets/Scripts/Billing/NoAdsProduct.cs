using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoAdsProduct : BillingProductAbstract {

  public override void Bye(bool isRestore = false) {

    if (!this.isActive) return;

    if (product.hasReceipt) {
      NetManager.Instance.CheckPayment(product.receipt, (result) => {
        if (!result) {
          Debug.Log("Платеж не подтвержден " + (typeof(NoAdsProduct)).ToString());
          return;
        }

        base.Bye(isRestore);

        PlayerManager.Instance.noAds = true;

      }, () => {
        Debug.Log("Ошибка подтверждения платежа " + (typeof(NoAdsProduct)).ToString());
      });

    }



  }

}
