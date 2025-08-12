using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HardCacheBillingProduct : BillingProductAbstract {

  public override void Bye(bool isRestore = false)
  {
    AppMetrica.Instance.ReportEvent("buy_hardv");
    UserManager.Instance.HardCash += count;
  }


}
