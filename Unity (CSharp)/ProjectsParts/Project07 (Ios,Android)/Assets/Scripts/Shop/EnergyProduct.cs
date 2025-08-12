using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyProduct : ProductBase {

  public int count;

  public override bool Buy() {

    if (!CheckCash()) return false;
    ChangeCash();

    UserManager.Instance.AddAction(() =>
    {
      AppMetrica.Instance.ReportEvent("buy_energy");
      UserEnergy.Instance.energyPlus += count;
    });
    return true;

  }
}
