using System.Collections;
using UnityEngine;

namespace Game.Weapon {

  public class ManagerIscander: WeaponStandart {

    /// <summary>
    /// Количество метеоритов
    /// </summary>
    public int countMeteorit;
    private bool isShoot;

    protected override void OnClickDown(Vector3 position) {
      if (!CheckReadyShoot(position, position))
        return;

      isShoot = true;
      OnShootComplited();
      PlayReload();
      BulletDecrement();
      StartCoroutine(GenerateArmageddon(pointerDown));
    }

    
    private IEnumerator GenerateArmageddon(Vector3 tapEnd) {

      for (int i = 0; i < countMeteorit; i++) {
        yield return new WaitForSeconds(0.7f);
        GenerateMeteorit(tapEnd);
      }
      isShoot = false;
      OnShootComplited();
    }

    private void GenerateMeteorit(Vector3 tapEnd) {

      GameObject meteor = PoolerManager.Spawn(bulletPrefab.name);
      meteor.transform.position =
        new Vector3(Random.Range(CameraController.leftPointX.x + 3f, CameraController.rightPoint.x), 5, 0);
      meteor.GetComponent<Bullet>().Shot(Vector3.zero, new Vector3(tapEnd.x, Random.Range(DecorationManager.Instance.loaderLocation.roadSize.min, DecorationManager.Instance.loaderLocation.roadSize.max), 0));
      meteor.SetActive(true);
    }

    protected override void OnShootComplited(bool isReload = true) {
      if (isShoot) {
        base.OnShootComplited(isReload);
        isShoot = false;
      }
    }

  }


}