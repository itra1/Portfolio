using System.Collections;
using UnityEngine;

namespace Game.Weapon {


  /// <summary>
  /// Управляющий контроллер топата
  /// </summary>
  public class ManagerShaman: WeaponAssistant {
    private float timeShootReady;
    private bool isShoot;


    protected override void OnClickDown(Vector3 position) {
      if (!CheckReadyShoot(PlayerController.Instance.ShootPoint, position))
        return;

      Shoot();
    }

    public override bool CheckReadyShoot(Vector3 tapStart, Vector3 tapEnd) {
      if (tapStart.x > tapEnd.x || tapEnd.x - tapStart.x < 0.5f)
        return false;
      return base.CheckReadyShoot(tapStart, tapEnd);
    }

    private void Shoot() {
      if (isShoot)
        return;
      isShoot = true;
      StartCoroutine(Shoot(pointerDown));
    }


    private IEnumerator Shoot(Vector3 target) {
      yield return new WaitForSeconds(timeShootReady);
      GameObject inst = PoolerManager.Spawn(bulletPrefab.name);
      inst.transform.position = target;
      inst.GetComponent<Bullet>().Shot(Vector3.zero, target);
      inst.SetActive(true);
      isShoot = false;
      base.OnShoot(Vector3.zero, target);
      PlayReload();
      OnShootComplited();
    }

    public override void GetConfig() {
      base.GetConfig();

      timeShootReady = wepConfig.param1.Value;

    }

  }


}