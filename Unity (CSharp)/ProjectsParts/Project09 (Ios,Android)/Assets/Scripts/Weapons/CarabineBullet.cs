using UnityEngine;

public class CarabineBullet : Bullet {

  protected override void DeactiveSFX() {
    base.DeactiveSFX();
    CameraManager.instance.StartVibration();

    GameObject sfx = PoolerManager.GetPooledObject("CarabineBoom");
    sfx.transform.position = transform.position;
    sfx.SetActive(true);
  }

}
