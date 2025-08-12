using System.Collections;
using UnityEngine;

namespace Game.Weapon {


  public class HollyGrenadeFunnelWeapon: Bullet {
    private bool firstEnemy;

    public override void OnEnable() {
      base.OnEnable();
      StartCoroutine(DeactiveFunnelTime());
      firstEnemy = false;
    }

    private IEnumerator DeactiveFunnelTime() {
      yield return new WaitForFixedUpdate();
      yield return new WaitForFixedUpdate();
      DeactiveObject();
    }


    public override void Move() { }
    public override void OnGround() { }

    /// <summary>
    /// Деактивация объекта
    /// </summary>
    private void DeactiveObject() {

      gameObject.SetActive(false);
    }

    public override void OnTriggerEnter2D(Collider2D col) { }

    private void OnTriggerStay2D(Collider2D col) {
      if (LayerMask.LayerToName(col.gameObject.layer) == "Enemy") {

        if (!firstEnemy) {
          firstEnemy = true;
          BattleEventEffects.Instance.VisualEffect(BattleEffectsType.granadeHit, col.transform.position, this);
        }

        DamageEnemy(col.gameObject);
      }
      DamageBallonsTry(col);
    }
  }

}
