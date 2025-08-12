using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletResourceIncrementator: ResourceIncrementatorBehaviour {

  public WeaponGroup.GroupType bulletType;

  public override void Increment(float value) {

    WeaponManager.Instance.groupList.Find(x => x.type == bulletType).bulletCount += (int)value;

  }
}
