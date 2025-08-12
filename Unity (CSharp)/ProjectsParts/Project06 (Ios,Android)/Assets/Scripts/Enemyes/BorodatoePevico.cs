using UnityEngine;
using System.Collections.Generic;
using Spine.Unity;
using Spine;

/// <summary>
/// Контроллер врага со средней скоростью
/// </summary>
public class BorodatoePevico : Enemy {

	public TrailRenderer trail;

	private float _timeReload;
	private float _koefSpeedInJump;

	float rightStop = CameraController.rightPoint.x - (CameraController.distanseX / 10f);
	float leftStop = CameraController.middleTopPointWorldX.x;
	bool specialMove = false;
	float timeNextJump = 1000000f;
	float targetX;
	float distanceJump;
	float minDistanceJump = CameraController.distanseX / 6f;
	float maxDistanceJump = CameraController.distanseX / 3f;
	int direction;
	float speedXinJump = 1;
	[HideInInspector]
	public bool isJumping;
	bool isJumpMuve;
	float jumpSpeed = 20;
	bool isFirstShow;
	public Animator voiceWave;

	protected override void OnEnable() {
		isJumping = false;
		isJumpMuve = false;
		specialMove = false;
		isFirstShow = true;
		rightStop = CameraController.rightPoint.x - (CameraController.distanseX / 10f);
		leftStop = CameraController.middleTopPointWorldX.x;
		minDistanceJump = CameraController.distanseX / 6f;
		maxDistanceJump = CameraController.distanseX / 3f;
		speedX = speedXinJump;
		trail.enabled = false;
		SetDirectionVelocity(-1);
		base.OnEnable();
	}

	#region move 
	public override void Update() {

		if (phase == Phase.dead) return;

		if (isFirstShow && transform.position.x < CameraController.rightPoint.x) {
			isFirstShow = false;
			PlayShowAudio();
		}

		CheckState();

		if (phase == Phase.run && specialMove) {

			if (!isJumping && Time.time > timeNextJump) {
				SetDirectionVelocity(direction);
				SetAnimation(jumpAnim, true);
				isJumping = true;
			}

			if (isJumping && isJumpMuve) {
				speedX = jumpSpeed;
				base.Move();
			}

			if (isJumpMuve && transform.position.x < targetX + 0.3f && transform.position.x > targetX - 0.3f) {
				skeletonAnimation.timeScale = 1;
				isJumping = false;
				isJumpMuve = false;
				GetComponent<PolygonCollider2D>().enabled = true;
				SetDirectionVelocity(-1);
				Attack();
				FindPointJump();
			}
			return;
		}

		base.Update();

		if (!specialMove && transform.position.x <= rightStop) {
			trail.enabled = true;
			specialMove = true;
			SetAnimation(stopAnim, true);
			FindPointJump();
		}
	}

	public override void SetPhase(Phase phase, bool force = false) {
		if(phase != Phase.attack)
			base.SetPhase(phase, force);
	}

	public void JumpBack() {
		specialMove = true;
		targetX = rightStop;
		distanceJump = Mathf.Abs(targetX - transform.position.x);
		SetDirectionVelocity(1);
		SetAnimation(jumpAnim, true);
		isJumping = true;
		isJumpMuve = true;
	}

	protected override void SetRunPhase() {

		base.SetRunPhase();

		if (specialMove) {
			FindPointJump();
		}
	}

	public override void SetRunAnimation() {
		if (specialMove) {
			SetAnimation(stopAnim, true, false);
			FindPointJump();
		} else
			base.SetRunAnimation();
	}

	protected override void EndDamage() {
		base.EndDamage();
		isJumping = false;
		//SetAnimation(stopAnim, true);
	}

	public override void SetBleeding(float timeWorkFlameOfFire, float periodWorkFlameOfFire, float newDamageFlameOfFire) {
		//base.SetBlood(timeWorkFlameOfFire, periodWorkFlameOfFire, newDamageFlameOfFire);
	}

	public override void Attack() {
		SetAnimation(attackAnim[UnityEngine.Random.Range(0, attackAnim.Count)], false);
	}
	float energyNow;
	void HitDamage() {
		Game.Weapon.WeaponGenerator.Instance.OverchargeAllWeapon();
		voiceWave.SetTrigger("voice");
	}

	public void JumpMove() {
		SetDirectionVelocity(direction);
		speedX = speedXinJump * _koefSpeedInJump;
		SetAnimation(jumpAnim, true);
		base.Move();
	}

	public void FindPointJump() {

		timeNextJump = Time.time + _timeReload;

		maxDistanceJump = Mathf.Min(maxDistanceJump, Mathf.Max(rightStop - transform.position.x, transform.position.x - leftStop));

		distanceJump = Random.Range(minDistanceJump, maxDistanceJump);
		direction = Random.value <= 0.5f ? -1 : 1;

		if (direction == -1) {

			if (transform.position.x - distanceJump > leftStop) {
				targetX = transform.position.x - distanceJump;
				distanceJump = Mathf.Abs(targetX - distanceJump);
				return;
			}

			if (transform.position.x + distanceJump < rightStop) {
				targetX = transform.position.x + distanceJump;
				direction = 1;
				distanceJump = Mathf.Abs(targetX - distanceJump);
				return;
			}
		}

		if (direction == 1) {
			if (transform.position.x + distanceJump < rightStop) {
				targetX = transform.position.x + distanceJump;
				distanceJump = Mathf.Abs(targetX - distanceJump);
				return;
			}

			if (transform.position.x - distanceJump > leftStop) {
				targetX = transform.position.x - distanceJump;
				direction = -1;
				distanceJump = Mathf.Abs(targetX - distanceJump);
				return;
			}
		}
	}
	#endregion

	[SpineAnimation(dataField: "skeletonAnimation")]
	public string stopAnim = ""; // Анимация простоя
	[SpineAnimation(dataField: "skeletonAnimation")]
	public string jumpAnim = ""; // Анимация прыжка

	public override void AnimEvent(TrackEntry trackEntry, Spine.Event e) {
		base.AnimEvent(trackEntry, e);

		if (trackEntry.ToString() == "jump" && e.data.name == "fly") {
			isJumpMuve = true;
			skeletonAnimation.timeScale = (distanceJump / jumpSpeed);
			GetComponent<PolygonCollider2D>().enabled = false;
		}

		if (trackEntry.ToString() == "attack" && e.data.name == "hit") {
			HitDamage();
			PlayAttackAudio();
		}

	}

	public override void AnimComplited(TrackEntry trackEntry) {

		if (trackEntry.ToString() == "attack")
			SetAnimation(stopAnim, true);

		base.AnimComplited(trackEntry);

	}

	protected override void GetConfig() {
		base.GetConfig();
		_timeReload = enemy.cooldown.Value;
		_koefSpeedInJump = enemy.param1.Value;
	}

	protected override void TargetEnter(Collider2D newTarget) {
		base.TargetEnter(newTarget);

		//if (newTarget.GetComponent<IPlayerDamage>() != null) {
		//	JumpBack();
		//}

	}

	#region Звуки

	public AudioBlock showAudioBlock;

	protected virtual void PlayShowAudio() {
		showAudioBlock.PlayRandom(this);
	}


	#endregion

}
