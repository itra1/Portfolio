using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;

public class MadamSufrajo : Enemy {

	bool isMove;
	bool attackReady;
	bool stopRun;
	float bulletSpeed;
	bool isFirstShow;
	bool stunOut;


	protected override void OnEnable() {
		base.OnEnable();
		attackReady = false;
		stopRun = false;
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

		if (!isMove) return;

		velocity.x = Mathf.Abs(speedX) * directionVelocity * (isBaffSpead ? 4 : 1);
		if (isStun) {
			velocity.x *= 1 - stunDelay;

		}
		transform.position += velocity * Time.deltaTime;
		//rb.MovePosition(transform.position + velocity * Time.deltaTime);
	}

	public Transform transformGenerateBullet;           // Позиция для генерации сняряда
	public string bulletPrefab;                         // Название префаба из пулер менеджера


	public override void SetRunAnimation() {
		base.SetRunAnimation();
		PlayStepAudio();
	}
	/// <summary>
	/// Генерация жкземпляра
	/// </summary>
	void ShootBulletInstantiate() {
		GameObject bulletInstance = PoolerManager.Spawn(bulletPrefab);
		bulletInstance.GetComponent<CatapultStone>().SetAngle(transformGenerateBullet.localEulerAngles);
		bulletInstance.GetComponent<CatapultStone>().speed = bulletSpeed;
		bulletInstance.transform.position = transformGenerateBullet.position;
		bulletInstance.GetComponent<CatapultStone>().SetParametrs(attackDamageBase, attackDamageBase);
		bulletInstance.SetActive(true);
		//bulletInstance.GetComponent<ShooterBullet>().enemyDamage = false;
	}

	public override void Attack() {

		if (attackLastTime + attackWaitTime > Time.time) return;

		attackLastTime = Time.time;
		SetAnimation(attackAnim[Random.Range(0, attackAnim.Count)], false);
	}

	protected override void CheckStun() {
		if (stunTime < Time.time && !stunOut) {
			stunOut = true;
			if (stunnAnim != "") SetAnimation(stunnEnd, false, false);
			isStun = false;
			//CheckPhase();
		}
	}

	protected override void GetConfig() {
		base.GetConfig();
		bulletSpeed = enemy.param1.Value;
	}

	//protected override void CheckPhase() {
	//	base.CheckPhase();
	//}

	protected override void SetStunPhase() {
		stunOut = false;
		if (stunnAnim != "") SetAnimation(stunnStart, false, false);
	}

	public override void AnimComplited(TrackEntry trackEntry) {
		base.AnimComplited(trackEntry);

		if (trackEntry.ToString() == stunnStart) {
			if (stunnAnim != "") SetAnimation(stunnAnim, true, false);
		}
		if (trackEntry.ToString() == stunnEnd) {
			isStun = false;
			stunOut = false;
			CheckPhase();
			return;
		}

		if (trackEntry.ToString() == runAnim) {
			CheckDistance();
			if (attackReady)
				SetPhase(Phase.attack);
			else
				PlayStepAudio();
		}

		if (attackAnim.Exists(x => x == trackEntry.ToString())) {
			CheckDistance();
			if (!stopRun)
				SetPhase(Phase.run);
		}
	}

	protected override void SetDeadPhase() {
		base.SetDeadPhase();
		stepAudioEffect.Stop(0, false);
	}
	public override bool Damage(GameObject damager, float value, float newSpeedDelay = 0, float newTimeSpeedDelay = 0, Component parent = null) {
		stepAudioEffect.Stop(0, true);
		bool damage = base.Damage(damager, value, newSpeedDelay, newTimeSpeedDelay, parent);
		isMove = false;
		return damage;
	}

	protected override void EndDamage() {
		CheckPhase();
		//if(attackReady)
		//  SetPhase(Phase.attack);
		//else
		//  SetPhase(Phase.run);
	}

	protected override void CheckPhase() {
		if (isStun && stunDelay == 1) {
			SetPhase(Phase.stun);
			return;
		}

		CheckDistance();

		if (!stopRun && !playerContact)
			SetPhase(Phase.run);


		if (attackReady || playerContact)
			SetPhase(Phase.attack);
	}

	public override void AnimEvent(Spine.TrackEntry trackEntry, Spine.Event e) {
		base.AnimEvent(trackEntry, e);
		if (e.Data.Name == "moveStart") {
			isMove = true;
		}
		if (e.Data.Name == "moveEnd") {
			isMove = false;
		}
		if (e.Data.Name == "hit") {
			PlayAttackAudio();
			ShootBulletInstantiate();
		}
	}
	
	void CheckDistance() {
		if (!attackReady && transform.position.x <= CameraController.rightPointWorld.x - 2) attackReady = true;
		if (!stopRun && transform.position.x <= CameraController.leftPointX.x + CameraController.distanseX / 3) stopRun = true;
	}

	[SpineAnimation(dataField: "skeletonAnimation")]
	public string stunnEnd = "";     // Анимация хотьбы назад
	[SpineAnimation(dataField: "skeletonAnimation")]
	public string stunnStart = "";     // Анимация перед движением назад
	
	#region Звуки
	
	public AudioBlock showAudioBlock;
	public AudioBlock moveAudioBlock;

	protected virtual void PlayShowAudio() {
		showAudioBlock.PlayRandom(this);
	}

	protected void PlayStepAudio() {
		moveAudioBlock.PlayRandom(this);
	}

	AudioPoint stepAudioEffect;

	#endregion

}
