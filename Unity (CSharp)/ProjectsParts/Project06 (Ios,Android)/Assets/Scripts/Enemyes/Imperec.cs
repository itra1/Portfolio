using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;

/// <summary>
/// Имперец боевой
/// </summary>
public class Imperec : Enemy {

	float speedBack;                    // Скорость отхода
	float timeBackDamage;               // Время отхода при получении урона
	float timeBack;                     // суммарное время отхода

	bool isBack;
	bool isChangeVector;

	float activeSpeed {
		get {
			if (isChangeVector) return 0;
			return (isBack ? -speedBack : speedX);
		}
	}

	public override void Update() {
		base.Update();

		if (phase == Phase.damage) Move();
	}

	protected override void SetDamagePhase() {
		//base.SetDamagePhase();
	}

	public override void Move() {

		velocity.x = activeSpeed * directionVelocity * (isBaffSpead ? 4 : 1);
		if (isStun) {
			velocity.x *= 1 - stunDelay;
		}
		if (velocity.x > 0 && transform.position.x >= CameraController.rightPoint.x) return;
		transform.position += velocity * Time.deltaTime;
		//rb.MovePosition(transform.position + velocity * Time.deltaTime);
	}

	public override void AnimComplited(TrackEntry trackEntry) {
		if (trackEntry.ToString() == runBackAnim || trackEntry.ToString() == runAnim) {
			if (isBack && timeBack < Time.time) {
				isBack = false;
				isChangeVector = true;
				SetAnimation(preFwdAnim, false, false);
			}
		}
		if (trackEntry.ToString() == preBackAnim) {
			SetAnimation(runBackAnim, true, false);
			isChangeVector = false;
		}
		if (trackEntry.ToString() == preFwdAnim) {
			SetAnimation(runAnim, true, false);
			isChangeVector = false;
		}

		if (trackEntry.ToString() == deadAnim) {
			Hide();
		}

	}

	public override void Attack() {
		if (!isBack && !isChangeVector)
			base.Attack();
	}

	protected override void TargetEnter(Collider2D newTarget) {
		if (!isBack)
			base.TargetEnter(newTarget);
	}

	public override void AnimEnd(Spine.TrackEntry trackEntry) { }

	//public override void SetStun(float newSpeedDelay = 0, float newTimeSpeedDelay = 0) { }
	protected override void SetDamageAnim() { }

	protected override void GetDamage(float stunDelay = 0, float stunTime = 0) {
		//ChangeHealthPanel();

		if (isChangeVector)
			isChangeVector = false;

		if (timeBack <= Time.time && !isChangeVector) {
			isChangeVector = true;
			isBack = true;
			if (phase == Phase.run) {
				SetAnimation(preBackAnim, false, false);
				SetPhase(Phase.run);
			} else {
				SetPhase(Phase.run);
				damageList.Clear();
				SetAnimation(runBackAnim, true, false);
				isChangeVector = false;
			}

		} else if (isChangeVector && !isBack) {
			isBack = true;
			isChangeVector = false;
			SetAnimation(runBackAnim, true, false);
		}

		timeBack = Time.time + timeBackDamage;
	}

	//bool damageResult;
	//public override bool Damage(GameObject damager, float value, float newSpeedDelay = 0, float newTimeSpeedDelay = 0, Component parent = null) {
	//	damageResult = base.Damage(damager, value, newSpeedDelay, newTimeSpeedDelay, parent);

	//	return damageResult;
	//}

	protected override void GetConfig() {
		base.GetConfig();

		speedBack = enemy.param1.Value;
		timeBackDamage = enemy.param2.Value;
	}

	[SpineAnimation(dataField: "skeletonAnimation")]
	public string runBackAnim = "";     // Анимация хотьбы назад
	[SpineAnimation(dataField: "skeletonAnimation")]
	public string preBackAnim = "";     // Анимация перед движением назад
	[SpineAnimation(dataField: "skeletonAnimation")]
	public string preFwdAnim = "";     // Анимация перед движением вперед


}
