using UnityEngine;
using System.Collections;

public class HeadZombieController : MonoBehaviour {

	public Rigidbody2D rb;

	float angle;                                        // Угол поворота
	float rotateSpeed;
	public float speed;
	//Vector3 velocity;                                   // Рассчет смещения
	//public float gravity;                               // Значение графитации
	
	//public LayerMask groundMask;                        // Слой с поверхностью
	//public float groundRadius;                          // Радиус определения поверхности
	//bool isActive = false;                              // Флаг фиксации на замле
	
	public GameObject graph;                            // Ссылка на графиескую составляющую, используется для поворота картинки

	public ParticleSystem myParticle;

	public ParticleType particle;
	bool contact;
	ParticleSystem.EmissionModule emi;
	//AudioSource audioComp;

	private void OnEnable() {
		emi = myParticle.emission;
		//emi.rate = new ParticleSystem.MinMaxCurve(0);
		emi.rateOverTime = new ParticleSystem.MinMaxCurve(0);
		//isActive = true;
		//audioComp = GetComponent<AudioSource>();
		Vector3 velocity = Vector3.zero;
		//velocity.x = Random.Range(RunnerController.RunSpeed * 3, RunnerController.RunSpeed * 5f);
		velocity.x = RunnerController.RunSpeed + Random.Range(speed, speed + 2) + (RunnerController.RunSpeed * 0.5f);
		velocity.y = Random.Range(10f, 16f);
		rotateSpeed = Random.Range(1000, 1500);

		rb.AddForce(velocity, ForceMode2D.Impulse);
		rb.angularVelocity = rotateSpeed;

		//myParticle.transform.parent = transform.parent;
	}

	//void Update() {
	//	// Вращение
	//	//angle -= rotateSpeed * Time.deltaTime;
	//	//if (!fix)
	//	//	graph.transform.eulerAngles = new Vector3(0, 0, angle);

	//	// Движение
	//	//Movement();

	//}

	private void OnCollisionEnter2D(Collision2D collision) {
		if (LayerMask.LayerToName(collision.gameObject.layer) == "Ground") {
			//emi.rate = new ParticleSystem.MinMaxCurve(50);
			emi.rateOverTime = new ParticleSystem.MinMaxCurve(50);
		}
	}
	private void OnCollisionExit2D(Collision2D collision) {
		if (LayerMask.LayerToName(collision.gameObject.layer) == "Ground") {
			//emi.rate = new ParticleSystem.MinMaxCurve(0);
			emi.rateOverTime = new ParticleSystem.MinMaxCurve(0);
		}
	}

	//private void OnCollisionStay2D(Collision2D collision) {

	//}

	//void Movement() {
	//	// Гравитация
	//	velocity.y -= gravity * Time.deltaTime;

	//	Collider[] isGrounded = Physics.OverlapSphere(transform.position, groundRadius, groundMask);
	//	if (isGrounded.Length > 0 && velocity.y < 0) {
	//		velocity.y = -velocity.y / 4f;

	//		velocity.x -= RunnerController.RunSpeed * Time.deltaTime;

	//		if (!contact) {
	//			contact = true;
	//			AllParticles.Generate(particle, new Vector3(transform.position.x, isGrounded[0].transform.position.y, transform.position.z), 20);
	//		}
	//	}
		
	//	if (isGrounded.Length > 0) {
	//		if (isActive) {
	//			myParticle.transform.position = new Vector3(transform.position.x, isGrounded[0].transform.position.y, transform.position.z);
	//			if (!audioComp.isPlaying)
	//				audioComp.Play();
	//			if (!myParticle.isPlaying)
	//				myParticle.Play();
	//		} else {
	//			myParticle.Play();
	//			//var emi = myParticle.emission;
	//			//myParticle.emissionRate = 0;
	//			emi.rate = new ParticleSystem.MinMaxCurve(0);
	//			if (audioComp.isPlaying)
	//				audioComp.Pause();
	//		}
	//	} else {
	//		//myParticle.emissionRate = 50;
	//		emi.rate = new ParticleSystem.MinMaxCurve(50);
	//	}

	//	//velocity.x -= RunnerController.RunSpeed*2 * Time.deltaTime;

	//	if (velocity.x <= 0)
	//		velocity.x = 0;

	//	if (velocity.x <= 0 && isActive && isGrounded.Length > 0) {
	//		isActive = false;
	//		GetComponent<Damager>().enabled = false;
	//	}

	//	if (isActive)
	//		transform.position += velocity * Time.deltaTime;

	//	if (transform.position.x < CameraController.displayDiff.transform.position.x + CameraController.displayDiff.left * 2
	//			|| transform.position.y < 0) {
	//		//myParticle.emissionRate = 0;
	//		emi.rate = new ParticleSystem.MinMaxCurve(0);
			
	//		Destroy(myParticle, 30);

	//		Destroy(gameObject);
	//	}
	//}

	private void OnTriggerEnter2D(Collider2D collision) {
		if (rb.velocity.x > RunnerController.RunSpeed && LayerMask.LayerToName(collision.gameObject.layer) == "Player")
			collision.GetComponent<Player.Jack.PlayerController>().ThisDamage(WeaponTypes.head, Player.Jack.DamagType.live, 1, transform.position);
	}

}
