using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinsBonus: BonusBehaviour {
  protected override void GetBonus() {
    base.GetBonus();
    UserManager.Instance.GetSoftCashBonus((int)count);
  }

  protected override void Start() {
    targetPanel = BattleController.Instance.panel.iconSoftCash.position;
    base.Start();
  }

  protected void OnEnable() {

    StartCoroutine(WaitFunc(() => {

    }, 0.5f));

  }

}
