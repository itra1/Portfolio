using UnityEngine;

namespace Game.Weapon {

  public class ManagerMedicineСhest: WeaponStandart {
    private float healthRepeat;

    protected override void OnShoot(Vector3 tapStart, Vector3 tapEnd) {

      base.OnShoot(tapStart, tapEnd);
      Game.User.UserHealth.Instance.Value += healthRepeat;
    }

    public override void GetConfig() {
      base.GetConfig();

      healthRepeat = (int)wepConfig.param1.Value;
    }

  }

}