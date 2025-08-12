using UnityEngine;
using System.Collections;

/// <summary>
/// Снаряд амура
/// </summary>
public class AmurBullet : ExEvent.EventBehaviour {

  Rigidbody2D rb;

  float speed;
  float damage;
  Vector3 aim;
  Vector3 lineAttack;

	[ExEvent.ExEventHandler(typeof(ExEvent.BattleEvents.BattlePhaseChange))]
	public void PhaseChange(ExEvent.BattleEvents.BattlePhaseChange phase) {
		if (phase.phase == BattlePhase.start) {
			gameObject.SetActive(false);
		}
	}

	void OnEnable() {
    rb = GetComponent<Rigidbody2D>();
    aim = PlayerController.Instance.transform.position;

    lineAttack = ( aim - transform.position ).normalized;
    transform.localEulerAngles = new Vector3(transform.localEulerAngles.x , transform.localEulerAngles.y , Vector3.Angle(Vector3.up , lineAttack) - 90);
  }

  /// <summary>
  /// Установка значения
  /// </summary>
  /// <param name="newSpeed">Скорость движения</param>
  /// <param name="newDamage">Наносимый урон</param>
  public void SetParametrs(float newSpeed , float newDamage) {
    speed = newSpeed;
    damage = newDamage;
  }

  void Update() {
    rb.MovePosition(transform.position + lineAttack * speed * Time.deltaTime);
  }

  void OnTriggerEnter2D(Collider2D target) {
    if (LayerMask.LayerToName(target.gameObject.layer) == "Player") {
      PlayerController player = target.GetComponent<PlayerController>();
      if (player != null) {
        player.Damage(damage);
        gameObject.SetActive(false);
      }
    }
  }

}
