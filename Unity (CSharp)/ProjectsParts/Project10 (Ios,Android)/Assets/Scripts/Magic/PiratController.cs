using UnityEngine;
using System.Collections;
using Spine.Unity;

public class PiratController : MonoBehaviour {
	
	[HideInInspector]
	public PiratAbilitySpawner magic;

	public Rigidbody2D rb;
	float speed;
	public float gravity = 65;
	Vector3 velocity = new Vector3();
	public float groundRadius;
	public LayerMask groundMask;
	bool isGround;

	public ParticleSystem[] particlesGrount;

	public SkeletonAnimation skeletonAnimation;         // Ссылка на скелетную анимацию
	[SpineAnimation(dataField: "skeletonAnimation")]
	public string idle = "";                  // Анимация дамага сзади
	ParticleSystem.EmissionModule[] emission;
	void OnEnable() {
		speed = -2;
		velocity.x = speed;
		magic.PiratEffect(true);
		StartCoroutine(Init());

		emission = new ParticleSystem.EmissionModule[particlesGrount.Length];
		for (int i = 0; i < emission.Length; i++)
			emission[i] = particlesGrount[i].emission;
		SetEmission(false);
	}

	IEnumerator Init() {
		yield return new WaitForEndOfFrame();
		skeletonAnimation.state.SetAnimation(0, idle, true);
	}

	private void OnDisable() {
		magic.PiratEffect(false);
		magic.Deactive();
	}

	private void FixedUpdate() {
		rb.AddForce(velocity, ForceMode2D.Force);

		Collider[] isGrounded = Physics.OverlapSphere(transform.position, groundRadius, groundMask);
		isGround = isGrounded.Length > 0 ? true : false;

		if (isGround) {
			velocity.y = 0;
			if (!emission[0].enabled)
				SetEmission(true);
		} else {
			if (emission[0].enabled)
				SetEmission(false);
		}

	}

	//void Update() {
	//	//velocity.y -= gravity * Time.deltaTime;

		
	//	//transform.position += velocity * Time.deltaTime;

	//	//if (transform.position.x < CameraController.displayDiff.transform.position.x + CameraController.displayDiff.left * 2 || transform.position.y < 0) {
			
	//	//	Destroy(gameObject);
	//	//}

	//}

	void SetEmission(bool flag) {
		for (int i = 0; i < emission.Length; i++)
			emission[i].enabled = flag;
	}

}
