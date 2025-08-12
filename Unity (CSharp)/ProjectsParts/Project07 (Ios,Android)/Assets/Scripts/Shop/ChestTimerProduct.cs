using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestTimerProduct : ChestProduct {

  public string uuid;
  
  public override bool Buy() {
    base.Buy();

    AppMetrica.Instance.ReportEvent("open_chest");
    DailyChestManager.Instance.ByeChest(uuid);
    return true;
  }

}
