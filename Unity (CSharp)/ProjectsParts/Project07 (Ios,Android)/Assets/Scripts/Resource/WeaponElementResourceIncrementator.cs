using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponElementResourceIncrementator: ResourceIncrementatorBehaviour {

  public string weaponUuid;
  
  public override void Increment(float value)
  {
    var wep = WeaponManager.Instance.GetWeaponByUuid(weaponUuid);

    wep.weaponData.IsByed = true;
    wep.weaponData.IsEquipped = true;

    try {
      WeaponsSpawner.Instance.Start();
    } catch { }
  }
}
