using UnityEngine;
/// <summary>
/// Общий контроллер врагов на корабле
/// </summary>
public abstract class ShipEnemy: GeneralEnemy {

  public override void OnTriggerEnter2D(Collider2D oth) {

    if (oth.tag == "Player") {
      if (runnerPhase == RunnerPhase.boost) {
        Damage(WeaponTypes.none, 100000, Vector3.zero, 0);
      }

      if (runnerPhase == RunnerPhase.dead) {
        EnemySpawner.Instance.CreateDeadCloud();
        gameObject.SetActive(false);
      }
    }

    if (runnerPhase == RunnerPhase.run && oth.tag == "Player" && !shoot.runFullAttackBack && shoot.bodyStep != 3)
      oth.GetComponent<Player.Jack.PlayerController>().ThisDamage(WeaponTypes.none, playerDamage.type, playerDamage.damagePower, transform.position);

    if (LayerMask.LayerToName(oth.gameObject.layer) == "Barrier") {
      if (oth.tag == "RollingStone") {
        Damage(WeaponTypes.none, (int)RunnerController.barrierDamage(oth.tag, false), transform.position, DamagePowen.level1, 0, true);
        oth.GetComponent<BarrierController>().DestroyThis();
      }
    }
  }
}
