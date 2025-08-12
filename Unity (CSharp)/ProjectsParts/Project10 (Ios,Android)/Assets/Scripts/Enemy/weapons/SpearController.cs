using System;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class SpearController : MonoBehaviour {

	//public static event Action OnFixed;              // Событие фиксации на земле
	//public static event Action OnMirrow;             // Событие отражения

	private Rigidbody2D _rb;
	private Rigidbody2D rb {
		set { _rb = value; }
		get {
			if (_rb == null)
				_rb = GetComponent<Rigidbody2D>();
			return _rb;
		}
	}

	[SerializeField]
	private BoxCollider2DHelper groundContactHelper;
	[SerializeField]
	private CapsuleCollider2DHelper playerMissContactHelper;

	public float playerDamageValue;                      // Повреждение игрока
	private bool playerDamage;                           // Флаг разрешающий наносить повреждение игроку

	private float angle;                                            // Угол относительно вертикального вектора в полете

	[SerializeField]
	private AudioClip[] groundClip;                                 // Звуки соприкосновения с зеплей
	[SerializeField]
	private AudioClip gefendClip;                                   // Звуки соприкосновения с зеплей
	[SerializeField]
	private GameObject sfxDefend;                                   // Звук отражения копья игроком

	[HideInInspector]
	public bool disableAudio;                               // Флаг отключить звук

	private bool isMirrow;                                  // Отбито
	public ParticleType particle;

	public float speed = 10;

	public LayerMask playerLayer;

	private bool playerCheck;
	private bool playerContact;

	private bool isActive;
	public bool isGround;

	[SerializeField]
	private int enemyDamage;

	private void OnEnable() {

		rb.bodyType = RigidbodyType2D.Dynamic;
		rb.gravityScale = 0;
		groundContactHelper.OnEnter = OnColliderGroundContact;
		playerMissContactHelper.OnEnter = OnColliderPlayerMiss;
		playerDamage = true;
		isActive = true;
		isGround = false;

		playerContact = false;
		playerCheck = false;
		isMirrow = false;
		_beforePoint = Vector3.zero;
		//OnShoot();
		mat = new Mathematic();
		CalcParabol();
	}

	private Mathematic mat = new Mathematic();
	private Vector3 _point = Vector3.zero;
	private Vector3 _beforePoint = Vector3.zero;
	private Vector3 _velocity;
	private Vector3 _velocityTarget;
	private Vector3 _beforePosition;
	private Vector3 _deltaMove;

	private void CalcParabol() {

		float distance = (Player.Jack.PlayerController.Instance.transform.position - transform.position).magnitude;
		Vector3 targetPoint = Vector3.zero + (Player.Jack.PlayerController.Instance.transform.position - transform.position);
		float height = distance / 4;
		Vector3 middlePoint = Vector3.zero + ((targetPoint - Vector3.zero).normalized * (distance / 2)) + Vector3.up * height;
		mat.ParabolaCalcCoef(Vector3.zero, middlePoint, targetPoint);
		_point = Vector3.zero;
		_beforePosition = transform.position;
	}

	private void CalcNewPoint() {
		_point = _beforePoint + _deltaMove;
		_beforePoint = _point;
		_point.x += speed * (isMirrow ? -0.5f : 1) * Time.deltaTime;
		_point.y = mat.ParabolaGetY(_point.x);
		_velocity = _point - _beforePoint;
	}

	private void Update() {

		if (!isActive) return;
		if (isGround) return;

		if (!isMirrow) {
			ForwardMove();
		} else {
			rb.MoveRotation(angle);
			playerCheck = false;
			_angele = rb.transform.localEulerAngles;
		}

		if (transform.position.x <= CameraController.displayDiff.leftDif(1.5f) || transform.position.y <= 0) {
			gameObject.SetActive(false);
		}

	}

	private void ForwardMove() {

		CalcNewPoint();

		_velocityTarget = _velocity.normalized * speed;
		_beforePosition = transform.position;
		_deltaMove = _velocityTarget * Time.deltaTime;

		_velocityTarget.x += (isMirrow ? 0 : RunnerController.RunSpeed);
		rb.velocity = _velocityTarget;

		angle = Vector3.Angle(Vector3.up, _velocityTarget);
		
		rb.MoveRotation(-angle);
		_angele = rb.transform.localEulerAngles;
	}

	private Vector3 _angele = Vector3.right;

	void OnColliderGroundContact(Collider2D col) {
		if (LayerMask.LayerToName(col.gameObject.layer) == "Ground") {
			FixedGround();
		}
	}

	void OnColliderPlayerMiss(Collider2D col) {
		if (LayerMask.LayerToName(col.gameObject.layer) == "Player") {
			playerCheck = true;

		}
	}

	/// <summary>
	/// Фиксация прилипания к земле
	/// </summary>
	void FixedGround() {

		if (isGround)
			return;
    
		if (!disableAudio && groundClip.Length > 0) {
			AudioManager.PlayEffect(groundClip[Random.Range(0, groundClip.Length)], AudioMixerTypes.runnerEffect, 1);
		}

		if (particle != ParticleType.none)
			AllParticles.Generate(particle, groundContactHelper.transform.position, 20);

		if (playerCheck && !playerContact) {
			Questions.QuestionManager.ConfirmQuestion(Quest.underSpear, 1, transform.position);
			playerCheck = false;
			playerContact = false;
		}

		rb.bodyType = RigidbodyType2D.Static;
		rb.gravityScale = 0;

		rb.velocity = Vector2.zero;
		transform.localEulerAngles = _angele;
		//rb.velocity = Vector3.zero;
		
		ExEvent.GameEvents.SpearGround.CallAsync();

		isGround = true;
		isActive = false;
		playerDamage = false;
		mirrowDamage = false;

	}

	//void FixedUpdate() {

	//	if (isActive) {
	//		//velocity.y -= gravity * Time.deltaTime;
	//		//transform.position += velocity * Time.deltaTime;
	//		//rb.MovePosition(transform.position + velocity * Time.fixedDeltaTime);

	//		if (isMirrow) {
	//			angle = Vector3.Angle(Vector3.up, rb.velocity);
	//			//transform.localEulerAngles = new Vector3(0f, 0f, angle);
	//			rb.MoveRotation(angle);
	//			playerCheck = false;
	//		} else {
	//			angle = Vector3.Angle(Vector3.up, rb.velocity);
	//			rb.MoveRotation(-angle);
	//			//transform.localEulerAngles = new Vector3(0f, 0f, -angle);
	//			/*
	//       Collider[] allCollider = Physics.OverlapSphere(transform.position , 2f , playerLayer);

	//       if (allCollider.Length > 0) {
	//           float playerDistance = Vector3.Distance(transform.position , allCollider[0].transform.position);
	//           if (playerDistance <= 2)
	//               playerCheck = true;
	//       }
	//       */
	//		}
	//	}

	//	//if(transform.position.x <= CameraController.displayDiff.leftDif(1.5f) || transform.position.y <= 0) {
	//	//  //Destroy(gameObject);
	//	//  gameObject.SetActive(false);
	//	//}

	//}

	private float distance;

	/// <summary>
	/// Рассчет вектора броска
	/// </summary>
	//private void OnShoot() {
	//	distance = 10f;
	//	distance = Vector3.Distance(transform.position, PlayerController.Instance.transform.position);
	//	Vector3 ShootVector = new Vector3(1, 1, 0);
	//	distance += RunnerController.RunSpeed * 0.75f * (distance / 5f > 1 ? 1 : distance / 5f);
	//	velocity = ShootVector * Mathf.Sqrt((distance * 15f) / Mathf.Sin(2 * (45 * Mathf.Deg2Rad)));
	//	rb.AddForce(velocity, ForceMode2D.Impulse);
	//}

	private bool mirrowDamage = false;

	/// <summary>
	/// Откажения копья игроком
	/// </summary>
	private void Mirrow() {

		if (isMirrow) return;
		isMirrow = true;
		
		ExEvent.GameEvents.SpearMirrow.CallAsync();

		Questions.QuestionManager.ConfirmQuestion(Quest.spearDefend, 1, transform.position);

		mirrowDamage = true;
		AudioManager.PlayEffect(gefendClip, AudioMixerTypes.runnerEffect);
		GameObject sfx = Instantiate(sfxDefend, transform.position, Quaternion.identity) as GameObject;
		Destroy(sfx, 5);
		transform.localEulerAngles = Vector3.zero;
		rb.velocity = Vector2.zero;
		rb.gravityScale = 3;
		rb.AddForce(Vector2.left, ForceMode2D.Impulse);
		//mat.ParabolaCalcCoef(transform.position + Vector3.left * 4, new Vector3(), transform.position);

	}
	
	private void OnTriggerEnter2D(Collider2D other) {

		//if (LayerMask.LayerToName(other.gameObject.layer) == "Ground") {
		//	FixedGround();
		//}

		if (LayerMask.LayerToName(other.gameObject.layer) == "Player" && playerDamageValue > 0 && playerDamage) {
			// Срабатывание при контакте с игроком

			playerContact = true;
			playerDamage = false;

			if (other.GetComponent<Player.Jack.PlayerController>().spearDefenderTime >= Time.time)
				Mirrow();
			else
				other.GetComponent<Player.Jack.PlayerController>().ThisDamage(WeaponTypes.spear, Player.Jack.DamagType.live, playerDamageValue, transform.position);
		}

		if (!isActive && isMirrow && mirrowDamage && LayerMask.LayerToName(other.gameObject.layer) == "Enemy") {
			// Дамаг по врагу в случае отражения
			mirrowDamage = false;
			ClassicEnemy enemyController = other.GetComponent<ClassicEnemy>();
			if (enemyController != null) {
				enemyController.Damage(WeaponTypes.spear, enemyDamage, transform.position, DamagePowen.level1);
			}
		} else if (!isActive && LayerMask.LayerToName(other.gameObject.layer) == "Enemy" && rb.velocity.y < 0) {
			if (other.gameObject.tag == "Enemy") {
				ClassicEnemy enemyController = other.GetComponent<ClassicEnemy>();
				if (enemyController != null) {
					enemyController.Damage(WeaponTypes.spear, enemyDamage, transform.position, DamagePowen.level1);
				}
			}
			if (other.gameObject.tag == "Spider") {
				Spider spider = other.GetComponent<Spider>();
				if (spider != null) {
					spider.Dead();
				}
			}

		}
	}
}