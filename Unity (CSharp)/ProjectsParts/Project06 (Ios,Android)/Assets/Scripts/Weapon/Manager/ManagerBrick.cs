using UnityEngine;

namespace Game.Weapon {

  /// <summary>
  /// Управляющий контроллер кирпича
  /// </summary>
  public class ManagerBrick: WeaponSingleThrow {

    //protected override void OnShoot(Vector3 tapStart, Vector3 tapEnd) {
    //  base.OnShoot(tapStart, tapEnd);
    //  CreateBullet().GetComponent<Bullet>().Shot(tapStart, tapEnd);
    //}

    //private GameObject CreateBullet() {
    //  GameObject inst = PoolerManager.Spawn(bulletPrefab.name);
    //  inst.transform.position = PlayerController.Instance.hand.position;
    //  inst.SetActive(true);
    //  return inst;
    //}
  }


}