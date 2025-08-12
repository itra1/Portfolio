using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletsProduct: ProductBase {

  public int countPatron;
  public WeaponGroup.GroupType groupType;


  public override bool Buy() {
    if (!CheckCash()) return false;
    ChangeCash();

    WeaponGroup wg = WeaponManager.Instance.groupList.Find(x => x.type == groupType);
    QuestionManager.Instance.AddValueQuest( QuestionManager.Type.buyBullet, countPatron);
    wg.bulletCount += countPatron;
    return true;
  }


}
