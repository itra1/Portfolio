using UnityEngine;
using System.Collections;
using Spine;

/// <summary>
/// Стрелок
/// </summary>
public class Hunweibin : Enemy {

	public Transform transformGenerateBullet;     // Позиция для генерации сняряда

	private float centerX;

	protected override void OnEnable() {
		base.OnEnable();

		centerX = CameraController.middleTopPointWorldX.x + Random.Range(-1.5f, 1.5f);
	}

	public override void Move() {
		if (transform.position.x <= centerX) { SetPhase(Phase.attack); return; }
		base.Move();
	}

	#region Аттака

	public string bulletPrefab;               // Название префаба из пулер менеджера
	float bulletSpeed;                        // Скорость полета пули

	/// <summary>
	/// Выполнение актаки
	/// </summary>
	public override void Attack() {
		//base.Attack();
		if (attackLastTime + attackWaitTime > Time.time) return;
		SetPhase(Phase.attack);

		attackLastTime = Time.time;
		SetAnimation(attackAnim[Random.Range(0, attackAnim.Count)], false);

	}

	protected override void CheckPhase() {

		if (transform.position.x <= centerX) {
			SetPhase(Phase.attack);
			return;
		}

		base.CheckPhase();

	}

	public override void AnimEvent(Spine.TrackEntry trackEntry, Spine.Event e) {
		base.AnimEvent(trackEntry, e);
		if (e.Data.Name == "shot") {
			ShootBulletInstantiate();
		}
	}

	/// <summary>
	/// Генерация жкземпляра
	/// </summary>
	void ShootBulletInstantiate() {
		PlayAttackAudio();
		GameObject bulletInstance = PoolerManager.Spawn(bulletPrefab);
		bulletInstance.transform.position = transformGenerateBullet.position;
		bulletInstance.GetComponent<ShooterBullet>().SetParametrs(bulletSpeed, attackDamage);
		bulletInstance.SetActive(true);
	}

	#endregion

	protected override void GetConfig() {
		base.GetConfig();
		bulletSpeed = enemy.param1.Value;
		//bulletDamageBase = float.Parse(xmlConfig["Damage (base)"].ToString());
	}

}
