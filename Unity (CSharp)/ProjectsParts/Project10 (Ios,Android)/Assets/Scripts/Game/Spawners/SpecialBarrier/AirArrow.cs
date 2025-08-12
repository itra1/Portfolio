using UnityEngine;
using System.Collections;

/// <summary>
/// Контроллер полета стрелы при специальном препядствии
/// </summary>
public class AirArrow : MonoBehaviour {
	
	//Vector3 velocity;                                   // Движение копья
	public Rigidbody2D rb;
	public BoxCollider2DHelper boxCollider;
	bool isActive;
	bool isGround;
	
	//public float gravity;                      // Скорость снижения
	[SerializeField]
	float speedX;                      // Скорость снижения
	
	public AudioClip gefendClip;                            // Звуки соприкосновения с зеплей
	public GameObject sfxDefend;

	public AudioClip[] groundClip;

	public ParticleType particle;

	public System.Action OnDestroyEvent;

	private void OnEnable() {
		rb.bodyType = RigidbodyType2D.Dynamic;
		isActive = true;
		isGround = false;
		//velocity.x = RunnerController.RunSpeed + speedX;
		boxCollider.OnEnter = OnCollider;
		rb.AddForce(new Vector2(RunnerController.RunSpeed + speedX,0), ForceMode2D.Impulse);
	}

	float angle;

	void OnCollider(Collider2D col) {
		isActive = false;
	}

	private void FixedUpdate() {
		if(!isGround)
			rb.MoveRotation(360 - Vector3.Angle(Vector3.up, rb.velocity));
	}

	//void Update() {
	//	if (!isFixed) {
	//		velocity.y -= gravity * Time.deltaTime;
	//		transform.position += velocity * Time.deltaTime;
	//		angle = Vector3.Angle(Vector3.up, velocity);
	//		transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, angle * (isMirrow ? 1 : -1));
	//	}
	//}

	void OnTriggerEnter2D(Collider2D obj) {
		if (LayerMask.LayerToName(obj.gameObject.layer) == "Ground") {
			isActive = false;
			isGround = true;
			rb.bodyType = RigidbodyType2D.Static;

			if (OnDestroyEvent != null) OnDestroyEvent();

			if (groundClip.Length > 0) {
				AudioManager.PlayEffect(groundClip[Random.Range(0, groundClip.Length)], AudioMixerTypes.runnerEffect);
			}

			if (particle != ParticleType.none)
				AllParticles.Generate(particle, new Vector3(transform.position.x, obj.transform.position.y, transform.position.z), 20);

		}

		if (isActive && LayerMask.LayerToName(obj.gameObject.layer) == "Player") {
			if (obj.GetComponent<Player.Jack.PlayerController>().spearDefenderTime >= Time.time) {
				//Questions.QuestionManager.addSpearDefender(transform.position);
				Questions.QuestionManager.ConfirmQuestion(Quest.spearDefend, 1, transform.position);
				Mirrow();
			} else
				obj.GetComponent<Player.Jack.PlayerController>().ThisDamage(WeaponTypes.airArrow, Player.Jack.DamagType.live, 1, transform.position);
		}
	}
	
	public void Mirrow() {
		isActive = false;
		AudioManager.PlayEffect(gefendClip, AudioMixerTypes.runnerEffect);
		GameObject sfx = Instantiate(sfxDefend, transform.position, Quaternion.identity) as GameObject;
		Destroy(sfx, 5);
		//velocity.y = 0;
		//velocity.x *= -1;
		//transform.localEulerAngles = Vector3.zero;
		rb.velocity = Vector2.zero;
		rb.AddForce(new Vector2(-1,0), ForceMode2D.Impulse);
	}

}
