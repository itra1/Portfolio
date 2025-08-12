using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VionPustini : Enemy {

	public Transform transformGenerateBullet;     // Позиция для генерации сняряда
	public Transform pricladTransform;                  // Координаты приклада
	bool isFirstShow;

	int countInit;
	protected override void OnEnable() {
		base.OnEnable();
		isFirstShow = true;
	}

	public override void Update() {
		base.Update();

		if (isFirstShow && transform.position.x < CameraController.rightPoint.x) {
			isFirstShow = false;
			PlayShowAudio();
		}

	}


	public override void Move() {
		if (transform.position.x <= CameraController.middleTopPointWorldX.x) { SetPhase(Phase.attack); return; }
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
		SetAnimation(attackAnim[Random.Range(0, attackAnim.Count)], false, false);
	}

	protected override void CheckPhase() {

		if (transform.position.x <= CameraController.middleTopPointWorldX.x) {
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

	public override void ResetAnimation() {
		if (countInit > 0) return;
		countInit++;
		skeletonAnimation.Initialize(true);
		skeletonAnimation.state.Event += AnimEvent;
		skeletonAnimation.state.End += AnimEnd;
		skeletonAnimation.state.Complete += AnimComplited;
		skeletonAnimation.state.Start += AnimStart;
		skeletonAnimation.state.Dispose += AnimDispose;
		skeletonAnimation.state.Interrupt += AnimInterrupt;
		skeletonAnimation.OnRebuild = OnRebild;
		currentAnimation = null;
	}

	/// <summary>
	/// Генерация жкземпляра
	/// </summary>
	void ShootBulletInstantiate() {
		PlayAttackAudio();
		GameObject bulletInstance = PoolerManager.Spawn(bulletPrefab);
		bulletInstance.transform.position = transformGenerateBullet.position;
		bulletInstance.GetComponent<ShooterBullet>().SetParametrs(bulletSpeed, attackDamage, (transformGenerateBullet.position - pricladTransform.position).normalized);
		bulletInstance.SetActive(true);
	}

	#endregion

	protected override void GetConfig() {
		base.GetConfig();
		bulletSpeed = enemy.param1.Value;
		//bulletDamageBase = float.Parse(xmlConfig["Damage (base)"].ToString());
	}

	public AudioBlock showAudioBlock;

	protected virtual void PlayShowAudio() {
		showAudioBlock.PlayRandom(this);
	}

}
