using UnityEngine;
using System.Collections;
using Spine.Unity;

public class Mazila : Enemy {

	public Transform transformGenerateBullet;           // Позиция для генерации сняряда
	public Transform pricladTransform;                  // Координаты приклада

	private float alterSpeed;
	private int animNeedLoop = 3;
	private bool isGoBack;
	private int countInit;

	private float timeBetweenShoot = 4;			// Время между выстрелами
	private float timeNextShoot;						// Врем следующего выстрела
	private bool shootReady;								// Готовность к стрельбе

	private bool isCenter;

	protected override void OnEnable() {
		isGoBack = false;
		base.OnEnable();
		CalcNextTimeAttack();
		skeletonAnimation.skeleton.a = 1;
		isCenter = false;
	}
	public override void Update() {
		base.Update();
		if (phase == Phase.dead) return;

		isCenter = (transform.position.x < CameraController.middleTopPointWorldX.x);

		//if (transform.position.x < CameraController.middleTopPointWorldX.x)
		//	Attack();
		
		if(timeNextShoot < Time.time) shootReady = true;

		//if (phase == Phase.run && transform.position.x > CameraController.middleTopPointWorldX.x)
		//	Move();
	}

	void CalcNextTimeAttack() {
		timeNextShoot = Time.time + timeBetweenShoot;
		shootReady = false;
	}

	public override void Move() {

		velocity.x = alterSpeed * directionVelocity * (isBaffSpead ? 4 : 1);
		if (isStun) {
			velocity.x *= 1 - stunDelay;
		}
		if (velocity.x > 0 && transform.position.x >= CameraController.rightPoint.x) return;
		transform.position += velocity * Time.deltaTime;
		//Debug.Log(velocity);
		//rb.MovePosition(transform.position + velocity * Time.deltaTime);
	}

	protected override void CheckPhase() {
		if (isStun && stunDelay == 1) {
			SetPhase(Phase.stun);
			return;
		}
		SetPhase(Phase.run);
		CheckState();

	}

	#region Аттака

	public string bulletPrefab;                         // Название префаба из пулер менеджера
	float bulletSpeed;                                  // Скорость полета пули
	bool startAttack;

	/// <summary>
	/// Выполнение актаки
	/// </summary>
	public override void Attack() {

		if (attackLastTime + attackWaitTime > Time.time) return;

		attackLastTime = Time.time;
		SetAnimation(attackAnim[Random.Range(0, attackAnim.Count)], false, false);
		startAttack = true;
	}


	/// <summary>
	/// Генерация жкземпляра
	/// </summary>
	void ShootBulletInstantiate() {
		GameObject bulletInstance = PoolerManager.Spawn(bulletPrefab);
		bulletInstance.transform.position = transformGenerateBullet.position;
		bulletInstance.GetComponent<ShooterBullet>().SetParametrs(bulletSpeed, attackDamage, (transformGenerateBullet.position - pricladTransform.position).normalized);
		bulletInstance.SetActive(true);
		bulletInstance.GetComponent<ShooterBullet>().enemyDamage = true;
	}

	#endregion
	public override void SetRunAnimation() {
		if (isGoBack) SetAnimation(runBackAnim, true, false);
		else SetAnimation(runAnim, true, false);
	}
	protected override void SetDamagePhase() { }

	public override void AnimEvent(Spine.TrackEntry trackEntry, Spine.Event e) {
		base.AnimEvent(trackEntry, e);
		if (e.Data.Name == "shot") {
			if (startAttack) {
				startAttack = false;
				PlayAttackAudio();
			}
			ShootBulletInstantiate();
		}
	}

	int loopCount;
	protected override void SetRunPhase() {
		base.SetRunPhase();
		alterSpeed = isGoBack ? -speedX : speedX;
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

	public override void AnimComplited(Spine.TrackEntry trackEntry) {
		base.AnimComplited(trackEntry);
		if (phase == Phase.dead) return;

		if (trackEntry.ToString() == runAnim || trackEntry.ToString() == runBackAnim) loopCount++;

		if (trackEntry.ToString() == runAnim && shootReady) {
			isGoBack = true;
			SetAnimation(preBackAnim, false, false);
			animNeedLoop = 1 + (!isCenter? 0 : Random.Range(2,3));
			
			alterSpeed = 0;
			loopCount = 0;
		}
		if (trackEntry.ToString() == runBackAnim && loopCount >= animNeedLoop && !isCenter) {

			SetAnimation(preFwdAnim, false, false);
			alterSpeed = 0;
			loopCount = 0;

		}
		if (trackEntry.ToString() == preBackAnim) {
			SetAnimation(runBackAnim, true, false);
			alterSpeed = -speedX;
			Attack();
		}
		if (trackEntry.ToString() == preFwdAnim) {
			CalcNextTimeAttack();
			isGoBack = false;
			SetAnimation(runAnim, true, false);
			alterSpeed = speedX;
		}

		if (attackAnim.Exists(x => x == trackEntry.ToString())) {
			if (isCenter) {
				SetAnimation(runBackAnim, true, false);
				return;
			}
			alterSpeed = speedX;
			SetAnimation(preFwdAnim, false, false);
			SetPhase(Phase.run);
			animNeedLoop = 4;
		}
	}

	public override void AnimStart(Spine.TrackEntry trackEntry) {

		if (attackAnim.Exists(x => x == trackEntry.ToString())) {
			//SetPhase(EnemyPhases.run);
			alterSpeed = -speedX * 0.5f;
		}
	}

	protected override void GetConfig() {
		base.GetConfig();

#if UNITY_EDITOR
		speedX = 0.7f;
		
#endif

		bulletSpeed = enemy.param1.Value;
		alterSpeed = speedX;

	}

	[SpineAnimation(dataField: "skeletonAnimation")]
	public string runBackAnim = "";     // Анимация хотьбы назад
	[SpineAnimation(dataField: "skeletonAnimation")]
	public string preBackAnim = "";     // Анимация перед движением назад
	[SpineAnimation(dataField: "skeletonAnimation")]
	public string preFwdAnim = "";     // Анимация перед движением вперед

}
