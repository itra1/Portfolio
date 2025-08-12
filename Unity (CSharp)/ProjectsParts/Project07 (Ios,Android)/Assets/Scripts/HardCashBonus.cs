using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HardCashBonus: BonusBehaviour {
  protected override void GetBonus() {
    base.GetBonus();
    UserManager.Instance.GetHardCashBonus((int)count);
  }

  protected override void Start() {
    targetPanel = BattleController.Instance.panel.iconHardCash.position;
    base.Start();
  }

}
