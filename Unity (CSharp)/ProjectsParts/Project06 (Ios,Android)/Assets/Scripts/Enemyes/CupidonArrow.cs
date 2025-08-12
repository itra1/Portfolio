using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CupidonArrow : ExEvent.EventBehaviour {

	Rigidbody2D rb;

	float speed;
	float damage;
	Vector3 lineAttack;

	void OnEnable() {
		rb = GetComponent<Rigidbody2D>();
	}

	[ExEvent.ExEventHandler(typeof(ExEvent.BattleEvents.BattlePhaseChange))]
	public void PhaseChange(ExEvent.BattleEvents.BattlePhaseChange phase) {
		if (phase.phase == BattlePhase.start) {
			gameObject.SetActive(false);
		}
	}

	/// <summary>
	/// Установка значения
	/// </summary>
	/// <param name="newSpeed">Скорость движения</param>
	/// <param name="newDamage">Наносимый урон</param>
	public void SetParametrs(float newSpeed, float newDamage, Vector3 newLineAttack) {
		lineAttack = newLineAttack;
		speed = newSpeed;
		damage = newDamage;
		transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, Vector3.Angle(Vector3.up, lineAttack) - 90);
	}

	public void SetParametrs(float newSpeed, float newDamage) {
		speed = newSpeed;
		damage = newDamage;
		lineAttack = ((PlayerController.Instance.transform.position+Vector3.up) - transform.position).normalized;
		transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, Vector3.Angle(Vector3.up, lineAttack) - 90);
	}

	void Update() {
		rb.MovePosition(transform.position + lineAttack * speed * Time.deltaTime);
	}

	void OnTriggerEnter2D(Collider2D target) {
		if(LayerMask.LayerToName(target.gameObject.layer) == "Player") {
			PlayerController player = target.GetComponent<PlayerController>();
			if(player != null) {
				player.Damage(damage);
				gameObject.SetActive(false);
			}
		}
	}

	protected void DamageEnemy(GameObject enemy) {
		Enemy enemyController = enemy.GetComponent<Enemy>();
		if(enemyController != null) {
			enemyController.Damage(gameObject, damage, 0, 0);
		}
	}

}
