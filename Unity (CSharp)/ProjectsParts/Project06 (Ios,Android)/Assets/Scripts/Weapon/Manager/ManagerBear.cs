using UnityEngine;

namespace Game.Weapon {

  public class ManagerBear: WeaponAssistant {

    protected override void OnClickDown(Vector3 position) {
      if (!CheckReadyShoot(position, position))
        return;

      StartShootAnimation();
    }

    protected override void OnShootAnimEvent() {
      Shoot();
    }

    private void Shoot() {

      GameObject inst = PoolerManager.Spawn(bulletPrefab.name);
      inst.transform.position = new Vector3(CameraController.leftPointX.x - 2, DecorationManager.Instance.loaderLocation.roadSize.min, 0);
      inst.GetComponent<Bear>().SetTargetPosition(pointerDown);
      inst.GetComponent<Bear>().OnDeactive = BearDeactive;
      inst.SetActive(true);
      SetActiveStatus(true);
      base.OnShoot(pointerDown, pointerDown);
    }
    
    private void BearDeactive() {
      OnShootComplited();
      PlayReload();
    }

  }


}