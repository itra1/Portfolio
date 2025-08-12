using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Снаряд шутера
/// </summary>
public class ShooterBullet : ExEvent.EventBehaviour {

  Rigidbody2D rb;

  float speed;
  float damage;
  //Vector3 aim;
  Vector3 lineAttack;
  public bool enemyDamage;

	[ExEvent.ExEventHandler(typeof(ExEvent.BattleEvents.BattlePhaseChange))]
	public void PhaseChange(ExEvent.BattleEvents.BattlePhaseChange phase) {
		if (phase.phase == BattlePhase.start) {
			gameObject.SetActive(false);
		}
	}

	void OnEnable() {
    enemyDamage = false;
    rb = GetComponent<Rigidbody2D>();
        Invoke("BulletDestroy", 15f);
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
    lineAttack = ((PlayerController.Instance.transform.position+Vector3.up+ Vector3.up * Random.Range(-1.5f,1.5f)) - transform.position).normalized;
    transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, Vector3.Angle(Vector3.up, lineAttack) - 90);
  }

	private void FixedUpdate() {
		rb.velocity = lineAttack * speed*1.5f;
	}



	//void Update() {
 //   rb.MovePosition(transform.position + lineAttack * speed * Time.deltaTime);
 // }

  void OnTriggerEnter2D(Collider2D target) {
    if (LayerMask.LayerToName(target.gameObject.layer) == "Player") {
			IPlayerDamage player = target.GetComponent<IPlayerDamage>();
      if (player != null) {
        player.Damage(damage);
                BulletDestroy();
      }
    }

    if(enemyDamage && LayerMask.LayerToName(target.gameObject.layer) == "Enemy") {
      DamageEnemy(target.gameObject);
            BulletDestroy();
    }

  }
    
    void BulletDestroy()
    {
        if (IsInvoking("BulletDestroy")) CancelInvoke("BulletDestroy");  
        gameObject.SetActive(false);
    }
  protected void DamageEnemy(GameObject enemy) {
    Enemy enemyController = enemy.GetComponent<Enemy>();
    if(enemyController != null) {
      enemyController.Damage(gameObject, damage, 0, 0);
    }

  }

}
