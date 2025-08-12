using UnityEngine;
using System;
using System.Collections.Generic;

public class SwordController : MonoBehaviour {
	
  //public static event Action OnLeftDestroy;
	public Rigidbody2D rb;

  public float damagePower;                                   // Сила повреждения
  public WeaponTypes thisWeaponType;                // Текущий тип оружия

  public float speedX;
	public float speed { get { return speedX * _vectorKoeff; } }
	Vector3 velocity;
  public int damageCount;
  List<int> enemyDamages = new List<int>();

	bool isActive = true;                                           // Флаг активности, что может наносить повреждение
	private float _vectorKoeff;

	private void OnEnable() {
		_vectorKoeff = (GameManager.activeLevelData.moveVector == MoveVector.left ? -1 : 1);
		enemyDamages.Clear();
		isActive = true;

		if (RunnerController.Instance.runnerPhase == RunnerPhase.boss) {
			damageCount = 1;
			speedX *= -1;
		}

		rb.velocity = new Vector2(RunnerController.RunSpeed + speed, 0);

	}

	private void OnDisable() {
		ExEvent.GameEvents.SwordLeftDestroy.CallAsync();
		try {
			if (isActive) {
				Questions.QuestionManager.ConfirmQuestion(Quest.weaponMiss, 1, Player.Jack.PlayerController.Instance.transform.position);
			}
		} catch { }
	}
	
  void OnTriggerEnter2D(Collider2D col) {
    if(!isActive) return;

    if(col.tag == "Enemy" && enemyDamages.Count < damageCount && !enemyDamages.Exists(x => x == col.GetInstanceID())) {
      enemyDamages.Add(col.GetInstanceID());
      if(col.GetComponent<Enemy>()) {
        col.GetComponent<Enemy>().Damage(thisWeaponType, damagePower, transform.position, DamagePowen.level1, 0, false);
        if(damagePower <= 0) isActive = false;
      }
    }
  }
}
