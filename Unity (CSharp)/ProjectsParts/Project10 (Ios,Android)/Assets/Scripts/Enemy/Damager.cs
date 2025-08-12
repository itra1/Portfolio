using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Структура по повреждения пользователя
[System.Serializable]
public struct PlayerDamage {
	public bool damage;         // Разрешение повреждения пользователя
	public Player.Jack.DamagType type;      // Тип дамага
	public int damagePower;     // Сила повреждения
	public FloatSpan damageTime;    // Время, между повреждениями
	public bool damageJump;     // Подбрасывать пользователя при повреждении
	public bool damageStay;     // Наносить повреждения, пока пользователь находится внутри
	public bool addBarrier;
	public AudioClip[] contactClip;      // Звук попадания по игроку
}

//Структура по повреждениям NPC
[System.Serializable]
public struct NPCDamage {
	public bool damage;         // Разрешение повреждения пользователя
	public int damagePower;     // Сила повреждения
	public bool damageJump;     // Подбрасывать пользователя при повреждении
	public float damageRadius;  // Радиус наносимых повреждений
	public bool damageStay;     // Наносить повреждения, пока пользователь находится внутри
	public float damageTime;    // Время, между повреждениями
	public float fireChanse;    // Шанс возгорания
	public LayerMask radiusMask;    // Объекты к повреждению в радиусе
}
public class Damager : MonoBehaviour {

	public bool CollisionPlatform; //Срабатывать при контакте о платформу
	public float PlatColRadius; // радиус по определению платформы
	public LayerMask PlatformMask; //Слой с платформой
	public bool actionsOne;     // Одно срабатывание
	bool actions;       // Флаг сработанного
	float resPower;

	public WeaponTypes thisWeaponType;

	List<GameObject> damageContact;

	public PlayerDamage playerDamage;
	public NPCDamage NPCDamage;
	float playerLastDamage;                     // Последнее время дамага игрока
	float enemyLastDamage;                      // Последнее время контакта врага
	bool isGround;

	void OnEnable() {
		damageContact = new List<GameObject>();
		actions = false;
	}

	void FixedUpdate() {

		if (!CollisionPlatform) return;
		Collider2D[] isGrounded = Physics2D.OverlapCircleAll(transform.position, PlatColRadius, PlatformMask);

		if (isGrounded.Length > 0 && CollisionPlatform)
			Bom();
	}

	void OnTriggerEnter2D(Collider2D other) {
		DamageAllEnter(other.gameObject);
	}
	void OnCollisionEnter2D(Collision2D other) {
		DamageAllEnter(other.gameObject);
	}

	void OnTriggerStay2D(Collider2D other) {
		DamageAllStay(other.gameObject);
	}

	/* ************************
	 * Общая атака при контакте
	 * ************************/
	void DamageAllEnter(GameObject objDamage) {
		// Отказ при отвлюченном модуле
		if (this.enabled == false)
			return;

		// Отказ при корректном
		if (actionsOne && actions)
			return;

		foreach (GameObject col in damageContact)
			if (objDamage == col)
				return;


		// Срабатывание при контакте с игроком
		if (LayerMask.LayerToName(objDamage.gameObject.layer) == "Player" && playerDamage.damage) {

			if (gameObject.tag == "spear" && objDamage.GetComponent<Player.Jack.PlayerController>().spearDefenderTime >= Time.time) {
				GetComponent<Shoot>().Mirrow();
			} else
				PlayerDamage(objDamage);

		}

		// Срабатывание при контакте с врагами
		if (objDamage.gameObject.tag == "Enemy" && NPCDamage.damage) {
			if (NPCDamage.damageRadius > 0) // Если повреждение по радиусу
			{
				Collider2D[] npc = Physics2D.OverlapCircleAll(transform.position, NPCDamage.damageRadius, NPCDamage.radiusMask);
				for (int i = 0; i < npc.Length; i++)
					EnemyDamage(npc[i].gameObject);
			} else
				EnemyDamage(objDamage);
		}

		damageContact.Add(objDamage);
	}

	/* ************************
	 * Общая атака при вхождении
	 * ************************/
	void DamageAllStay(GameObject objDamage) {
		// Отказ при отвлюченном модуле
		if (this.enabled == false)
			return;

		// Отказ при корректном
		if (actionsOne && actions)
			return;

		// Срабатывание при контакте с игроком
		if (LayerMask.LayerToName(objDamage.layer) == "Player"              // Игрок
				 && playerDamage.damage                                         // Разрешено повреждать
				 && playerDamage.damageStay                                     // Разрешено повреждать, пока игрок внутри коллайдера
				 && playerLastDamage <= Time.time - Random.Range(playerDamage.damageTime.min, playerDamage.damageTime.max)     // Органичение времени между повреждениями прошло
			 )
			PlayerDamage(objDamage);

		// Срабатывание при контаксте с врагом
		if (objDamage.tag == "Enemy"
				&& NPCDamage.damage
				&& NPCDamage.damageStay
				&& enemyLastDamage <= Time.time - NPCDamage.damageTime) {
			EnemyDamage(objDamage);
			enemyLastDamage = Time.time;
		}
	}

	/* ************************
	 * Страбатывание при контакте с поверхностью
	 * ************************/
	private void Bom() {
		// Срабатывает при контакте с поверхностью
		if (NPCDamage.damageRadius > 0 && NPCDamage.damage) {
			Collider2D[] npc = Physics2D.OverlapCircleAll(transform.position, NPCDamage.damageRadius, NPCDamage.radiusMask);

			for (int i = 0; i < npc.Length; i++)
				EnemyDamage(npc[i].gameObject);
		}
	}
	/* ************************
	 * Дамаг по игроку
	 * ************************/
	private void PlayerDamage(GameObject player) {
		if (playerDamage.contactClip.Length > 0) {
			AudioManager.PlayEffect(playerDamage.contactClip[Random.Range(0, playerDamage.contactClip.Length)], AudioMixerTypes.runnerEffect);
		}

		player.GetComponent<Player.Jack.PlayerController>().ThisDamage(thisWeaponType, Player.Jack.DamagType.live, playerDamage.damagePower, transform.position, true);
		if (playerDamage.addBarrier)
			RunnerController.barrierDamageCount++;
		playerLastDamage = Time.time;
		actions = true;
	}

	/* ************************
	 * Повреждение Enemy
	 * ************************/
	private void EnemyDamage(GameObject npc) {
		if (npc.tag == "Enemy") {
			resPower = NPCDamage.damagePower;
			npc.GetComponent<ClassicEnemy>().Damage(thisWeaponType, ref resPower, transform.position, DamagePowen.level1, NPCDamage.fireChanse);
		}

		actions = true;
	}
}
