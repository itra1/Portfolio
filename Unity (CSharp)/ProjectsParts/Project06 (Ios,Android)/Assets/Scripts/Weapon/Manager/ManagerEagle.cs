using UnityEngine;

namespace Game.Weapon {

  /// <summary>
  /// Орел
  /// </summary>
  public class ManagerEagle: WeaponAssistant {


    protected override void OnClickDown(Vector3 position) {
      if (!CheckReadyShoot(position, position))
        return;

      StartShootAnimation();
    }

    protected override void OnShootAnimEvent() {
      Shoot();
      OnShootComplited();
    }

    private void Shoot() {

      GameObject inst = PoolerManager.Spawn(bulletPrefab.name);
      inst.transform.position = pointerDown;
      inst.GetComponent<Eagle>().SetPointTap(pointerDown);
      inst.GetComponent<Eagle>().OnDeactive = WeaponDeactive;
      inst.SetActive(true);
      SetActiveStatus(true);
      
    }

    private void WeaponDeactive() {
      PlayReload();
      OnShootComplited();
    }

  }

}