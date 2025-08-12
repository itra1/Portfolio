using UnityEngine;
using System.Collections;

public class ArrowAirAttack : MonoBehaviour {

	public Rigidbody2D rb;

	[HideInInspector]
	public AirStoneAttack parent;
	Vector3 velocity;

	public AudioClip[] groundClip;

	bool fix;
	public ParticleType particle;

	void Start() {

		velocity.x = 6 + RunnerController.RunSpeed;
		velocity.y = -10f;

		rb.bodyType = RigidbodyType2D.Dynamic;
		//rb.velocity = velocity;
		rb.AddForce(new Vector2(velocity.x, 0), ForceMode2D.Impulse);
	}


	void Update() {
		
		if (!fix) {
			float angle = Vector3.Angle(Vector3.up, velocity);
			rb.MoveRotation(rb.angularVelocity + angle * Time.deltaTime);
		}
		
		if (transform.position.y < 0) {
			Destroy(gameObject);
		}

	}

	void OnTriggerEnter2D(Collider2D obj) {
		if (!fix && LayerMask.LayerToName(obj.gameObject.layer) == "Ground") {
			fix = true;

			rb.bodyType = RigidbodyType2D.Static;
			//parent.ArrowFlyEnd();

			if (groundClip.Length > 0) {
				AudioManager.PlayEffect(groundClip[Random.Range(0, groundClip.Length)], AudioMixerTypes.runnerEffect);
			}

			if (particle != ParticleType.none)
				AllParticles.Generate(particle, new Vector3(transform.position.x, obj.transform.position.y, transform.position.z), 20);
			
		}

		if (!fix && LayerMask.LayerToName(obj.gameObject.layer) == "Player")
			obj.GetComponent<Player.Jack.PlayerController>().ThisDamage(WeaponTypes.airArrow, Player.Jack.DamagType.live, 1, transform.position);
	}

}
