using UnityEngine;

namespace Game.Weapon {

  public class TomatTomatGunWeapon: Bullet {
    private Vector3 startTomatGun;

    public override void OnEnable() {
      base.OnEnable();
      groundLevel = DecorationManager.Instance.loaderLocation.roadSize.min;
    }

    public override void Shot(Vector3 tapStart, Vector3 tapEnd) {

      velocity = (tapEnd - transform.position) * Time.deltaTime;
    }



    public override void Move() {
      // rigidBody.MovePosition(transform.position + velocity.normalized * speed * Time.deltaTime);
      transform.position += velocity.normalized * speed * Time.deltaTime;
    }

  }


}