using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class WeaponsSpawner : Singleton<WeaponsSpawner> {

  [HideInInspector]
  public List<WeaponBehaviour> instanceList = new List<WeaponBehaviour>();

  public WeaponBehaviour activeWeapon = null;

  public void Start () {
    GenerateManagers();
    SetActiveWeapon(instanceList[0].uuid);
  }

  private void GenerateManagers() {

    instanceList.Clear();

    WeaponManager.Instance.GetWeaponsEquipped().OrderBy(x => x.SortIndex).ToList().ForEach(wep => {

      GameObject wepInst = Instantiate(wep.gameObject);
      wepInst.transform.SetParent(transform);

      instanceList.Add(wepInst.GetComponent<WeaponBehaviour>());

    });

    ExEvent.BattleEvents.ReadyWeapon.Call(instanceList);

  }

  public void SetActiveWeapon(string uuid) {


    instanceList.ForEach(wep => {
      wep.SetSelected(uuid);

      if (wep.IsSelected)
        activeWeapon = wep;
    });
    
  }

  public WeaponBehaviour GetActiveWeapon() {
    return activeWeapon;
  }


  [ExEvent.ExEventHandler(typeof(ExEvent.BattleEvents.EnemyClick))]
  private void OnTryShoot(ExEvent.BattleEvents.EnemyClick eventData) {

    if (GameTime.timeScale == 0) return;

    if ((BattleController.Instance.battlePhase & BattleController.BattlePhase.end) != 0)
      return;

    activeWeapon.Shoot(eventData.enemy);


  }


}
