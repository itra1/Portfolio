using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatronBonus : BonusBehaviour {

  public WeaponGroup.GroupType group;

  protected override void GetBonus() {
    base.GetBonus();

    WeaponGroup wg = WeaponManager.Instance.groupList.Find(x => x.type == group);
    wg.bulletCount++;
    UserManager.Instance.GetPatronBonus(1);
    //UserManager.Instance.GetSoftCashBonus(count);
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
