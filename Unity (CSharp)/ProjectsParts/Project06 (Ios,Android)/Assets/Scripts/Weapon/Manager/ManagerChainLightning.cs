using UnityEngine;

namespace Game.Weapon {

  public class ManagerChainLightning: WeaponStandart {

    protected override void OnClickDown(Vector3 position) {
      if (!CheckReadyShoot(position, position))
        return;

      StartShootAnimation();
    }

    protected override void OnShootAnimEvent() {
      Shoot();
      PlayReload();
      BulletDecrement();
      OnShootComplited();
    }

    private void Shoot() {
      GameObject inst = PoolerManager.Spawn(bulletPrefab.name);
      inst.transform.position = PlayerController.Instance.ShootPoint;
      inst.SetActive(true);
      inst.GetComponent<Bullet>().Shot(pointerDown, pointerDown);
    }


  }

}