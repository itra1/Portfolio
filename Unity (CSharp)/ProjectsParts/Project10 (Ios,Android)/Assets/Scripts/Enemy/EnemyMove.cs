using UnityEngine;
using Spine.Unity;

/// <summary>
/// Движение врагов
/// </summary>
public class EnemyMove : MonoBehaviour {

	private GeneralEnemy _generalEnemy;                                          // Ссылка на основной контроллер
	private GeneralEnemy generalEnemy {
		get {
			if (_generalEnemy == null)
				_generalEnemy = GetComponent<GeneralEnemy>();
			return _generalEnemy;
		}
	}

	private Enemy _enemy;
	private Enemy enemy {
		get {
			if (_enemy == null)
				_enemy = GetComponent<Enemy>();
			return _enemy;
		}
	}

	private Rigidbody2D _rb;
	public Rigidbody2D rb {
		get {
			if (_rb == null)
				_rb = GetComponent<Rigidbody2D>();
			return _rb;
		}
	}

	private ClassicEnemy _classicEnemy;
	private ClassicEnemy classicEnemy {
		get {
			if (_classicEnemy == null)
				_classicEnemy = GetComponent<ClassicEnemy>();
			return _classicEnemy;
		}
	}

	private RunnerController runner {
		get { return RunnerController.Instance; }
	}                                        // Контроллер раннера

	private EnemyShoot _enemyShoot;
	private EnemyShoot enemyShoot {
		get {
			if (_enemyShoot == null)
				_enemyShoot = GetComponent<EnemyShoot>();
			return _enemyShoot;
		}
	}

	[SerializeField]
	private SkeletonAnimation skeletonAnimation;           // Анимация Скелета
	private Mathematic parabol;                                             // Объект для рассчета прыжка или атаки
	[HideInInspector]
	public Player.Jack.Boundary boundary;                     // Ограничения по локации
																								//public bool week;                                               // Флаг слабости
	private Player.Jack.PlayerController player {
		get { return Player.Jack.PlayerController.Instance; }
	}
	private int isRouletteStep;
	public Vector3 lastPosition = Vector3.zero;                // Последнее положение

	private bool bound;                                             // Флаг принимает ограничение по движению
	[HideInInspector]
	public bool inBoundary;                       // В пределах нужной области
	Vector3 velocity = Vector3.zero;                                // Рассчет смещения
																																	//Vector3 target = Vector3.zero;
	private float speedX;
	public float groundRadius;                                      // Радиус определения земли
	public LayerMask groundMask;                                    // Слой земли
	[HideInInspector]
	public bool isGround;                         // Флаг контакта с землей

	public bool thisJump;
	public float jumpSpeed;                                         // Скорость прыжка
	private float jumpDist;                                                 // Дистанция прыжка
	private float barrierWait;

	[HideInInspector]
	public bool tossNow = false;                                    // Бросили
	public float tossSpeed;
	private bool thisToss;
	private Vector3 lastTransform;

	[SpineAnimation(dataField: "skeletonAnimation")]
	public string runIdleAnim = "";                                 // Анимация бега
	[SpineAnimation(dataField: "skeletonAnimation")]
	public string runIdleAnimNoHead = "";                           // Анимация бега без головы
	[SpineAnimation(dataField: "skeletonAnimation")]
	public string jumpIdleAnim = "";                                // Анимация полета

	public float dragTime;

	[HideInInspector]
	public bool stopEnemy;

	public bool isStoped {
		get { return EnemySpawner.Instance.enemyStop; }
		set { EnemySpawner.Instance.enemyStop = value; }
	}         // Остановка

	void OnEnable() {
		EnemySpawner.StopEnemy += StopEnemy;

		SetClassicRigidbody();
		transform.localEulerAngles = Vector2.zero;
		rb.freezeRotation = true;

		tossNow = false;
		thisJump = false;

	}

	public void SetClassicRigidbody() {
		rb.gravityScale = 6;
		rb.angularDrag = 0;
		rb.drag = 0;
		rb.mass = 1;
	}

	public void SetFirstRigidbody() {
		rb.gravityScale = 2;
		rb.angularDrag = 0;
		rb.drag = 0;
		rb.mass = 1;
	}

	void OnDisable() {
		EnemySpawner.StopEnemy -= StopEnemy;
		inBoundary = false;
	}

	void StopEnemy(bool isStop) {
		isStoped = isStop;
	}


	/// <summary>
	/// Рассчет допустимой области движения
	/// </summary>
	void CalcBoundary() {
		if (isStoped) {
			boundary.xMax = CameraController.displayDiff.leftDif(1.3f);
			boundary.xMin = CameraController.displayDiff.leftDif(1.5f);
			return;
		}

		boundary.xMin = CameraController.displayDiff.leftDif(1);
		if (runner.activeLevel == ActiveLevelType.ship)
			boundary.xMax = CameraController.displayDiff.leftDif(0.8f * (RunnerController.powerValue / runner.powerMax));
		else
			boundary.xMax = CameraController.displayDiff.leftDif(0.6f);

		if (boundary.xMax - 1 > transform.position.x && boundary.xMin + 1 < transform.position.x)
			inBoundary = true;
		else
			inBoundary = false;
	}

	private Collider[] isGrounded;


	public float RunSpeedToPlayer {
		get {
			return runner.runSpeedActual * 1.035f;
		}
	}

	private void Move() {
		CalcBoundary();
		if (enemy.isGround && rb.velocity.y <= 0)
			thisJump = false;
		velocity = rb.velocity;
		velocity.x = RunSpeedToPlayer / (stopEnemy ? 1.5f : 1);

		if (!thisJump && (generalEnemy.runnerPhase == RunnerPhase.run || generalEnemy.runnerPhase == RunnerPhase.tutorial)) {
			if (boundary.xMax + 1f < transform.position.x)
				speedX = -2f;
			else if (boundary.xMax - 0.5f < transform.position.x && speedX >= 0)
				speedX = -Random.Range(0.3f, 0.7f);

			// Сбегаться в случае если игрок в толпе
			if (player && player.transform.position.x < transform.position.x && boundary.xMax > player.transform.position.x)
				speedX = -2f;

			if (boundary.xMin + 0.5f > transform.position.x)
				speedX = 2;
		}

		if (enemyShoot.runAttack) {

			if (isStoped || transform.position.x >= CameraController.displayDiff.rightDif(enemyShoot.runAttackDistanceKoef) || transform.position.x - 0.5f > player.transform.position.x) {
				enemyShoot.runAttack = false;
				generalEnemy.lastDistanceShoot = RunnerController.playerDistantion;
			} else
				speedX = 2;
		}

		if (generalEnemy.runnerPhase == RunnerPhase.boost)
			speedX = -RunSpeedToPlayer / 1.5f;

		if ((generalEnemy.runnerPhase & (RunnerPhase.dead | RunnerPhase.lowEnergy)) != 0)
			speedX = 10;

		velocity.x += (thisJump ? jumpDist : speedX);

		if (dragTime >= Time.time || enemyShoot.runFullAttackBack)
			velocity.x = 0;

		if (isStoped && !thisJump) {
			if (transform.position.x > boundary.xMax)
				velocity.x = RunnerController.RunSpeed / 5;
			else {
				//velocity.y = 0;
				velocity.x = RunnerController.RunSpeed;
			}
		}

		if (!enemyShoot.bodyAttack && !tossNow) {

			if (!thisJump)
				rb.velocity = velocity;

			if (enemy.isBreak && !thisJump) {
				enemy.isBreak = false;
				// Перепрыгиваем яму
				if (generalEnemy.JumpReady() && generalEnemy.JumpBreackReady()) {
					JumpBreak();
				}
			} else if (enemy.isBarrier) {
				enemy.isBarrier = false;
				JumpBarrier();
			}
		} else if (tossNow) {
			angle = Vector3.Angle(Vector3.up, rb.velocity);
			//rb.MoveRotation(-angle);
			rb.angularVelocity = -angle;
		}
	}

	private float angle;


	private void Update() {

		if (generalEnemy.isGround) {
			if (!enemyShoot.bodyAttack) {
				if (generalEnemy.JumpReady())
					generalEnemy.SetAnimation(runIdleAnim, true);
				else
					generalEnemy.SetAnimation(runIdleAnimNoHead, true);
			}
		} else {
			if (!enemyShoot.bodyAttack)
				generalEnemy.SetAnimation(jumpIdleAnim, true);
		}

		Move();

	}

	/// <summary>
	/// Движение Врага
	/// </summary>
	//public void Movement() {
	//	lastPosition = transform.position;
	//	CalcBoundary();
	//	thisRunnerSpeed = runner.runSpeedNow;

	//	velocity.x = thisRunnerSpeed / (stopEnemy ? 1.5f : 1);

	//	if(dragTime >= Time.time && transform.position.x < CameraController.displayDiff.leftDif(1.3f))
	//		transform.position = new Vector3(CameraController.displayDiff.leftDif(1.3f), transform.position.y, transform.position.z);

	//	if(!thisJump && (generalEnemy.runnerPhase == RunnerPhase.run || generalEnemy.runnerPhase == RunnerPhase.tutorial)) {
	//		if(boundary.xMax + 1f < transform.position.x)
	//			speedX = -2f;
	//		else if(boundary.xMax - 0.5f < transform.position.x && speedX >= 0)
	//			speedX = -Random.Range(0.3f, 0.7f);

	//		// Сбегаться в случае если игрок в толпе
	//		if(player && player.transform.position.x < transform.position.x && boundary.xMax > player.transform.position.x)
	//			speedX = -2f;

	//		if(boundary.xMin + 0.5f > transform.position.x)
	//			speedX = Random.Range(0.3f, 0.7f);
	//	}

	//	if(jumpNow) {
	//		generalEnemy.ResetAnimation();
	//		thisJump = true;
	//		jumpNow = false;

	//		if(generalEnemy.runnerPhase == RunnerPhase.run & Random.value <= 0.3f) generalEnemy.PlayIdleAudio();
	//	}

	//	// Гравитация
	//	//if(!shoot.bodyAttack && !tossNow) velocity.y -= gravity * Time.deltaTime;

	//	// Определение расстояния до конца конца обрыва
	//	for(int i = 0; i < groungsArray.Length; i++) {
	//		if(groungsArray[i].tag == "jumpUp" && !thisJump) {
	//			// Перепрыгиваем яму
	//			if(generalEnemy.JumpReady() && generalEnemy.JumpBreackReady())
	//				JumpBreak();
	//		}
	//	}

	//	// Перепрыгиваем препятствие
	//	JumpBarrier();

	//	// Выбегаем вперед при атаке забегом
	//	if(shoot.runAttack) {

	//		if(isStoped || transform.position.x >= CameraController.displayDiff.rightDif(shoot.runAttackDistanceKoef) || transform.position.x - 0.5f > player.transform.position.x) {
	//			shoot.runAttack = false;
	//			generalEnemy.lastDistanceShoot = RunnerController.playerDistantion;
	//		} else
	//			speedX = 2;
	//	}

	//	// Выбегаем вперед при атаке забегом
	//	if(shoot.runFullAttack) {

	//		if(transform.position.x >= CameraController.displayDiff.rightDif(0.75f)) {
	//			shoot.runFullAttackBack = true;
	//			shoot.runFullAttack = false;
	//		} else
	//			speedX = 2;
	//	}

	//	if(generalEnemy.runnerPhase == RunnerPhase.boost)
	//		speedX = -thisRunnerSpeed / 1.5f;
	//	if(generalEnemy.runnerPhase == RunnerPhase.dead)
	//		speedX = 10;

	//	velocity.x += (thisJump ? jumpDist : speedX);

	//	if(dragTime >= Time.time || shoot.runFullAttackBack)
	//		velocity.x = 0;

	//	if(isStoped && !thisJump) {
	//		if(transform.position.x > boundary.xMax)
	//			velocity.x = RunnerController.RunSpeed / 5;
	//		else {
	//			//velocity.y = 0;
	//			velocity.x = RunnerController.RunSpeed;
	//		}
	//	}

	//	if(thisJump && velocity.y < 0 && isGround)
	//		thisJump = false;

	//	if(!shoot.bodyAttack && !tossNow) {
	//		//PlayerMove(ref velocity);
	//		//transform.position += velocity * Time.deltaTime;
	//	}

	//	if(isGround) {
	//		if(!shoot.bodyAttack) {
	//			if(generalEnemy.JumpReady())
	//				generalEnemy.SetAnimation(runIdleAnim, true);
	//			else
	//				generalEnemy.SetAnimation(runIdleAnimNoHead, true);
	//		}
	//	} else {
	//		if(!shoot.bodyAttack)
	//			generalEnemy.SetAnimation(jumpIdleAnim, true);
	//	}

	//}

	/// <summary>
	/// Устновка унимации бега
	/// </summary>
	public void SetRunIdleAnimation() {
		generalEnemy.SetAnimation(runIdleAnim, true);
	}

	Vector3 checkPosition;

	RaycastHit[] groundHits;


	/// <summary>
	/// Определение ямы
	/// </summary>
	void JumpBreak() {
		if (enemyShoot.bodyStep >= 2 || isStoped || thisJump)
			return;

		Vector3 checkVectorJumpHit = new Vector3(transform.position.x, transform.position.y - 1.5f, transform.position.z);
		Collider2D[] checkHit = Physics2D.OverlapAreaAll(new Vector2(transform.position.x, transform.position.y - 4), new Vector2(transform.position.x + 7, transform.position.y + 4));

		float distantionJump = 7f;

		foreach (Collider2D one in checkHit) {
			if (one.transform.tag == "jumpDown" && one.transform.position.x > transform.position.x + 1) {
				float dist = Vector3.Distance(one.transform.position, checkVectorJumpHit);
				if (dist < distantionJump)
					distantionJump = dist;
			}
		}

		if (distantionJump > 7f)
			distantionJump = 7f;

		//distantionJump -= thisRunnerSpeed * 0.4f;
		if (distantionJump <= 0.1f)
			distantionJump = 0.1f;
		thisJump = true;
		jumpDist = distantionJump;
		rb.velocity = new Vector2(rb.velocity.x, 0);
		rb.AddForce(new Vector2(jumpDist, jumpSpeed), ForceMode2D.Impulse);
	}

	/// <summary>
	/// Определение барьера
	/// </summary>
	void JumpBarrier() {
		if (Time.time <= barrierWait || enemyShoot.bodyStep >= 2 || thisJump)
			return;

		//Vector3 checkVectorJumpHit = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
		//if (Physics2D.Raycast(checkVectorJumpHit, Vector3.right, 2f, LayerMask.NameToLayer("Barrier"))) {
		barrierWait = Time.time + 0.7f;
		if (Random.value <= 0.99f) {
			thisJump = true;
			jumpDist = 7f - RunnerController.RunSpeed * 0.5f;
			rb.velocity = new Vector2(jumpDist, 0);
			rb.AddForce(new Vector2(jumpDist, jumpSpeed), ForceMode2D.Impulse);
			thisJump = true;
			if (jumpDist <= 0)
				jumpDist = 0;
		}
		//}

		//Vector3 jumpCheckVector = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);

		//RaycastHit2D[] rayhit = Physics2D.CircleCastAll(jumpCheckVector, 3, Vector3.up);

		//for (int j = 0; j < rayhit.Length; j++) {
		//	if (LayerMask.LayerToName(rayhit[j].transform.gameObject.layer) == "Barrier") {
		//		if (generalEnemy.JumpBarrierReady()) {
		//			barrierWait = Time.time + 0.7f;
		//			jumpNow = true;
		//			jumpDist = 5f - thisRunnerSpeed * 0.7f;
		//			if (jumpDist <= 0)
		//				jumpDist = 0;
		//		}

		//	}
		//}
	}

	private void OnCollisionEnter2D(Collision2D collision) {

		if (tossNow && LayerMask.LayerToName(collision.gameObject.layer) == "Ground") {
			if (classicEnemy != null)
				classicEnemy.GenBones(DamagePowen.level1, true);
			gameObject.SetActive(false);
		}

	}


	/// <summary>
	/// Полет после брока
	/// </summary>
	public void Toss() {
		tossNow = true;
		rb.gravityScale = 2;
		rb.freezeRotation = false;
		rb.AddForce(new Vector2(tossSpeed + RunnerController.RunSpeed, Random.Range(4f, 6f)), ForceMode2D.Impulse);

		//transform.localRotation = Quaternion.identity;

		//if (!tossNow) {
		//	tossNow = true;
		//	velocity.y = Random.Range(4f, 6f);

		//	if (PlayerController.instance != null) {
		//		target = PlayerController.instance.transform.position;
		//		tossSpeed = (target.x - transform.position.x);
		//		if (tossSpeed <= 5)
		//			tossSpeed = 5;
		//		else
		//			tossSpeed = Random.Range(tossSpeed - (tossSpeed / 20), tossSpeed + (tossSpeed / 20)) * 1.6f;
		//	}
		//} else {
		//	velocity.y -= gravityToss * Time.deltaTime;
		//	velocity.x = tossSpeed + RunnerController.RunSpeed;

		//	float angle = Vector3.Angle(Vector3.up, velocity);
		//	transform.localEulerAngles = new Vector3(0f, 0f, -angle);

		//	transform.position += velocity * Time.deltaTime;

		//	Collider[] isGrounded = Physics.OverlapSphere(transform.position + new Vector3(0, 0, 0), groundRadius, groundMask);
		//	isGround = isGrounded.Length > 0 ? true : false;

		//	if (lastTransform.y > transform.position.y && isGround)
		//		tossNow = false;

		//	if (isGround) {

		//		if (classicEnemy != null)
		//			classicEnemy.GenBones(DamagePowen.level1, true);
		//		gameObject.SetActive(false);
		//	}
		//}
		//lastTransform = transform.position;

	}

}
