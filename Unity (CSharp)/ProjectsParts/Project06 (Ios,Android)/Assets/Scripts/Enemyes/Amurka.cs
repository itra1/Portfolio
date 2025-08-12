using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(Amurka))]
public class AmurkaEditor : Editor {

	public override void OnInspectorGUI() {
		base.OnInspectorGUI();

		if (GUILayout.Button("Attack")) {
			((Amurka)target).StartShoot();
		}

		if (GUILayout.Button("Damage")) {
			((Amurka)target).Damage(((Amurka)target).gameObject, 50, 0, 0);
		}

	}

}


#endif


public class Amurka : Enemy {

	public CupidonBallons ballons;

	private Transform playerTransform;
	private float groundY;
	private float timeShoot;
	private bool isDead;

	enum MoveType {
		type1, // Летает на максимальной высоте
		type2, // Летает ближе к земле
		type3, // Подлетает отталкиваясь  от земли
		type4, // Только прыгает по земле
	}

	MoveType moveType;

	protected override void OnEnable() {
		base.OnEnable();
		if (ballons != null) ballons.OnBalloneChange = OnBalloneChange;
		OnBalloneChange(5);
		playerTransform = PlayerController.Instance.transform;
		velocity.y = 2;
		groundY = transform.position.y;
		timeShoot = Time.time;
		isDead = false;
		speedYkoef = 1;
		isAngry = false;
		transform.position = new Vector3(transform.position.x, Random.Range(move1TopBorder, move1BottomBorder), transform.position.z);

	}

	public override void Update() {
		base.Update();

		if (phase == Phase.dead)
			DeadMove();

		if (phase == Phase.dead)
			return;

		BowAngle();
		Attack();

		if (transform.position.x < CameraController.leftPointX.x + CameraController.distanseX / 6 && directionVelocity == -1)
			SetDirectionVelocity(1, false);
		if (transform.position.x > CameraController.leftPointX.x + CameraController.distanseX / 6 * 5 && directionVelocity == 1)
			SetDirectionVelocity(-1, false);

	}

	protected override void SetDeadPhase() {
		base.SetDeadPhase();
		ballons.DestroyAll();
	}

	public override void Attack() {

		switch (moveType) {
			case MoveType.type1:
			case MoveType.type2:
				if (timeShoot + 3 < Time.time) {
					StartShoot();
					timeShoot = Time.time;
				}
				break;
		}

	}

	void OnBalloneChange(int ballonCount) {
		if (phase == Phase.dead) return;

		switch (ballonCount) {
			case 5:
				moveType = MoveType.type1;
				break;
			case 4:
				moveType = MoveType.type2;
				gravity = gravityDown;
				break;
			case 3:
			case 2:
			case 1:
				isAngry = true;
				moveType = MoveType.type3;
				gravity = gravityDown;
				break;
			case 0:
				moveType = MoveType.type4;
				gravity = gravityDown;
				break;
		}
		ground = false;

		SetRunAnimation();

	}

	public Transform bowShoulder;
	void BowAngle() {
		bowShoulder.localEulerAngles = new Vector3(bowShoulder.localEulerAngles.x,
																							bowShoulder.localEulerAngles.y,
																							Vector3.Angle(playerTransform.position - bowShoulder.position, Vector3.up) - 38);
	}

	public override void Move() {

		if (phase == Phase.dead) return;

		switch (moveType) {
			case MoveType.type1:
				Move1();
				break;
			case MoveType.type2:
				Move2();
				break;
			case MoveType.type3:
				Move3();
				break;
			case MoveType.type4:
				Move4();
				break;
		}

	}

	float move1TopBorder = 1;
	float move1BottomBorder = 0;
	float move1MaxTopSpeed = 1;
	float move1MaxBottomSpeed = -1.5f;
	float gravityDown = -3f;
	float gravityUp = 2f;
	float gravity;

	/// <summary>
	/// Летает на максимальной высоте
	/// </summary>
	void Move1() {
		velocity.x = Mathf.Abs(speedX) * directionVelocity * (isBaffSpead ? 4 : 1);

		if (gravity == 0) gravity = gravityUp;

		velocity.y += gravity * Time.deltaTime;

		if (gravity > 0) {
			if (transform.position.y < move1TopBorder) {
				velocity.y = Mathf.Min(move1MaxTopSpeed, velocity.y);
			} else {
				gravity = gravityDown;
			}
		} else {
			if (transform.position.y > move1BottomBorder) {
				//velocity.y += gravity * Time.deltaTime;
				velocity.y = Mathf.Max(move1MaxBottomSpeed, velocity.y);
			} else {
				gravity = gravityUp;
			}
		}

		transform.position += velocity * Time.deltaTime;
		//rb.MovePosition(transform.position + velocity * Time.deltaTime);
	}

	float move2TopBorder = 0;
	float move2BottomBorder = -1;
	/// <summary>
	/// Летает почти касаясь земли
	/// </summary>
	void Move2() {
		velocity.x = Mathf.Abs(speedX) * directionVelocity * (isBaffSpead ? 4 : 1);

		velocity.y += gravity * Time.deltaTime;

		if (gravity > 0) {
			if (transform.position.y < move2TopBorder) {
				velocity.y = Mathf.Min(move1MaxTopSpeed, velocity.y);
			} else {
				gravity = gravityDown;
			}
		} else {
			if (transform.position.y > move2BottomBorder) {
				//velocity.y += gravity * Time.deltaTime;
				velocity.y = Mathf.Max(move1MaxBottomSpeed, velocity.y);
			} else {
				gravity = gravityUp;
			}
		}

		transform.position += velocity * Time.deltaTime;
		//rb.MovePosition(transform.position + velocity * Time.deltaTime);
	}

	float move3TopBorder = -2f;
	float speedYkoef  = 1;

	bool ground;

	/// <summary>
	/// Подлетает отпрыгива
	/// </summary>
	void Move3() {
		velocity.x = Mathf.Abs(speedX) * directionVelocity * (isBaffSpead ? 4 : 1);

		velocity.y += gravity * Time.deltaTime;

		if (gravity > 0) {
			if (transform.position.y < move3TopBorder) {
				velocity.y = Mathf.Min(move1MaxTopSpeed, velocity.y);
			} else {
				gravity = gravityDown;
			}
		} else {
			if (transform.position.y > groundY) {
				velocity.y += gravity * Time.deltaTime;
				velocity.y = Mathf.Max(move1MaxBottomSpeed, velocity.y);
			} else if (!ground) {
				SetAnimation(jump21Anim, false);
				speedYkoef = 0;
				ground = true;
				//gravity = gravityUp;
			}
		}
		transform.position += velocity * speedYkoef * Time.deltaTime;
		//rb.MovePosition(transform.position + velocity * speedYkoef * Time.deltaTime);

	}

	float move4TopBorder = -2.5f;

	/// <summary>
	/// Прыгает по земле без шаров
	/// </summary>
	void Move4() {
		velocity.x = Mathf.Abs(speedX) * directionVelocity * (isBaffSpead ? 4 : 1);

		velocity.y += gravity * Time.deltaTime;

		if (gravity > 0) {
			if (transform.position.y < move4TopBorder) {
				velocity.y = Mathf.Min(move1MaxTopSpeed, velocity.y);
			} else {
				gravity = gravityDown;
			}
		} else {
			if (transform.position.y > groundY) {
				velocity.y = Mathf.Max(move1MaxBottomSpeed, velocity.y);
			} else if (!ground) {
				SetAnimation(jump21Anim, false);
				speedYkoef = 0;
				ground = true;
			}
		}

		transform.position += velocity * speedYkoef * Time.deltaTime;
		//rb.MovePosition(transform.position + velocity * speedYkoef * Time.deltaTime);

	}

	void DeadMove() {
		gravity = gravityDown;
		if (transform.position.y > groundY) {
			velocity.x = 0;
			velocity.y += gravity * 20 * Time.deltaTime;
			rb.MovePosition(transform.position + velocity * Time.deltaTime);
		} else {
			if (!isDead) {
				isDead = true;
				SetAnimation(deadAnimEnd, false, false);

			}
		}
	}


	public override void AnimComplited(Spine.TrackEntry trackEntry) {
		base.AnimEnd(trackEntry);

		if (trackEntry.ToString() == jump11Anim) {
			speedYkoef = 0;
			SetAnimation(jump12Anim, false, false);
		} else if (trackEntry.ToString() == jump12Anim) {
			speedYkoef = 1;
			gravity = gravityUp;
			velocity.y = move1MaxTopSpeed;
			ground = false;
			SetAnimation(jump13Anim, false, false);
			ballons.GetComponent<Animator>().SetTrigger("jump");
		} else if (trackEntry.ToString() == jump13Anim) {
			StartShoot();
		} else if (trackEntry.ToString() == jump21Anim) {
			speedYkoef = 0;
			SetAnimation(jump22Anim, false, false);
		} else if (trackEntry.ToString() == jump22Anim) {
			speedYkoef = 1;
			gravity = gravityUp;
			velocity.y = move1MaxTopSpeed;
			ground = false;
			SetAnimation(jump23Anim, false, false);
			ballons.GetComponent<Animator>().SetTrigger("jump");
		} else if (trackEntry.ToString() == jump23Anim) {
			ground = false;
			StartShoot();
		}

		if (trackEntry.ToString() == idleAttackAnim) {
			SetPhase(Phase.run);
			ground = false;
			SetRunAnimation();
		}

		if (trackEntry.ToString() == damageAnim) {
			ground = false;
			SetPhase(Phase.run);
		}

		if (trackEntry.ToString() == deadAnimEnd) {
			Hide();
		}

	}

	public override void AnimEvent(TrackEntry trackEntry, Spine.Event e) {
		if (e.data.Name == "hit") {
			GenerateBullet();
		}
	}

	public override void SetRunAnimation() {

		switch (moveType) {
			case MoveType.type1:
			case MoveType.type2:
				SetAnimation(runAnim, true);
				break;
			case MoveType.type3:
			case MoveType.type4:
				SetAnimation(runAnimAlt, true);
				break;
		}

	}


	protected override void GetDamage(float newSpeedDelay = 0, float newTimeSpeedDelay = 0) {
		//speedYkoef = 1;
		//ChangeHealthPanel();
		SetDamageAnim();
		//SetPhase(EnemyPhases.damage);
	}

	#region Аттака

	public string bulletPrefab;
	public Transform transformGenerateBullet;
	float bulletSpeed = 5;

	public void StartShoot() {
		SetAnimation(idleAttackAnim, false);
	}

	public void GenerateBullet() {
		GameObject bulletInstance = PoolerManager.Spawn(bulletPrefab);
		bulletInstance.transform.position = transformGenerateBullet.position;
		bulletInstance.GetComponent<CupidonArrow>().SetParametrs(bulletSpeed, attackDamage * (isAngry ? 3 : 1));
		bulletInstance.SetActive(true);
	}

	#endregion

	protected override void GetConfig() {
		base.GetConfig();
		bulletSpeed = enemy.param1.Value;
	}

	[SpineAnimation(dataField: "skeletonAnimation")]
	public string jump11Anim = "";
	[SpineAnimation(dataField: "skeletonAnimation")]
	public string jump12Anim = "";
	[SpineAnimation(dataField: "skeletonAnimation")]
	public string jump13Anim = "";
	[SpineAnimation(dataField: "skeletonAnimation")]
	public string jump21Anim = "";
	[SpineAnimation(dataField: "skeletonAnimation")]
	public string jump22Anim = "";
	[SpineAnimation(dataField: "skeletonAnimation")]
	public string jump23Anim = "";
	[SpineAnimation(dataField: "skeletonAnimation")]
	public string idleAttackAnim = "";
	[SpineAnimation(dataField: "skeletonAnimation")]
	public string engryAttackAnim = "";
	[SpineAnimation(dataField: "skeletonAnimation")]
	public string runAnimAlt = "";
	[SpineAnimation(dataField: "skeletonAnimation")]
	public string deadAnimEnd = "";

}
