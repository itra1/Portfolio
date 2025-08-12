using System.Collections;
using UnityEngine;

namespace Game.Weapon {


  public class LightningWeapon: Bullet {
    public ParticleSystem lightTarget;

    public override void OnEnable() {
      base.OnEnable();
      StartCoroutine(ActiveCoroutine());
      lightTarget.Play();

    }

    private IEnumerator ActiveCoroutine() {
      yield return new WaitForSeconds(0.2f);
      DeactiveThis(null);
    }

    public override void Shot(Vector3 tapStart, Vector3 tapEnd) { }

    public override void Move() { }
    public override void OnGround() { }
    public override void OnTriggerEnter2D(Collider2D col) {

      if (LayerMask.LayerToName(col.gameObject.layer) == "Enemy") {
        DamageEnemy(col.gameObject);
        //DeactiveThis();
      }
    }

    public override void GetConfig() {
      base.GetConfig();
    }

  }

}