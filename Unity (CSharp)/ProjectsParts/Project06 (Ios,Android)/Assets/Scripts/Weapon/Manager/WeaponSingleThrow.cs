using UnityEngine;

namespace Game.Weapon {
  public class WeaponSingleThrow: WeaponStandart {

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
      inst.GetComponent<Bullet>().Shot(PlayerController.Instance.ShootPoint, pointerDown);
      inst.SetActive(true);
    }

  }
}