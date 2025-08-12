using UnityEngine;

namespace Game.Weapon {


  public class ManagerAlian: WeaponAssistant {
    private float radiusCheck;
    private float timeActive;
    [SerializeField]
    private LayerMask enemyLayer;

    protected override void OnClickDown(Vector3 position) {
      if (!CheckReadyShoot(position, position))
        return;

      StartShootAnimation();

    }

    protected override void OnShootAnimEvent() {
      Shoot();
      PlayReload();
      OnShootComplited();
    }

    private void Shoot() {

      Collider2D[] enemys = Physics2D.OverlapCircleAll(pointerDown, radiusCheck, enemyLayer);

      foreach (Collider2D enem in enemys) {
        if (enem.GetComponent<Enemy>() != null)
          enem.GetComponent<Enemy>().SetMelafon(timeActive / enemys.Length);
      }
    }

    //protected override void OnShoot(Vector3 tapStart, Vector3 tapEnd) {

    //  Collider2D[] enemys = Physics2D.OverlapCircleAll(tapEnd, radiusCheck, enemyLayer);

    //  foreach (Collider2D enem in enemys) {
    //    if (enem.GetComponent<Enemy>() != null)
    //      enem.GetComponent<Enemy>().SetMelafon(timeActive / enemys.Length);
    //  }

    //  base.OnShoot(tapStart, tapEnd);

    //}

    public override void GetConfig() {
      base.GetConfig();

      radiusCheck = wepConfig.param1.Value;
      timeActive = wepConfig.param2.Value;
    }

  }

}
