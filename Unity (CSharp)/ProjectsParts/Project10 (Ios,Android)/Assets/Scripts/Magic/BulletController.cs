using UnityEngine;
using Spine.Unity;

public class BulletController : MonoBehaviour {
	[HideInInspector]
	public BulletAbilitySpawner magic;
	public GameObject fire;

	Vector3 velocity;

	//public GameObject bombPref;                                                 // Эффект взрыва
	public LayerMask contactMask;                                               // Слои контакта
	public float contactRadius;                                                 // Радиус определения объектов
	public int power;                                                           // Сила повреждения
	public float speed;
	public CircleCollider2DHelper helper;

	public SkeletonAnimation skeletonAnimation;         // Ссылка на скелетную анимацию
	[SpineAnimation(dataField: "skeletonAnimation")]
	public string idle = "";                  // Анимация дамага сзади

	public AudioClip[] boomClip;

	void Start() {
		if (skeletonAnimation)
			skeletonAnimation.state.SetAnimation(0, idle, true);

		//helper.OnEnter = OnEnter;

		velocity.x = -((CameraController.displayDiff.right * 2.2f) * Random.Range(1f, 1.5f)) + RunnerController.RunSpeed;
		velocity.y = 0f;
		GetComponent<Rigidbody2D>().AddForce(velocity, ForceMode2D.Impulse);
	}

	void OnEnter(Collider2D obj) {
		if (LayerMask.LayerToName(obj.gameObject.layer) == "Ground") {
			//GameObject bom = Instantiate(bombPref, transform.position, Quaternion.identity) as GameObject;

			GameObject inst = Pooler.GetPooledObject("ShipBoom");
			inst.transform.position = transform.position;
			inst.SetActive(true);

			AudioManager.PlayEffect(boomClip[Random.Range(0, boomClip.Length)], AudioMixerTypes.runnerEffect);
			if (magic != null) {
				magic.BloomBomb();
			}

			fire.transform.parent = transform.parent;
			var emi = fire.GetComponent<ParticleSystem>().emission;
			emi.enabled = false;
			//fire.GetComponent<ParticleSystem>().enableEmission = false;
			Destroy(fire, 10);

			Collider[] barrier = Physics.OverlapSphere(transform.position, 8);

			for (int i = 0; i < barrier.Length; i++) {
				if (barrier[i].GetComponent<BarrierController>()) {
					barrier[i].GetComponent<BarrierController>().DestroyThis();
				}
			}

			Collider[] enemy = Physics.OverlapSphere(transform.position, contactRadius, contactMask);

			for (int i = 0; i < enemy.Length; i++) {
				if (enemy[i].GetComponent<Enemy>()) {
					enemy[i].GetComponent<Enemy>().Damage(WeaponTypes.bullet, power, transform.position, DamagePowen.level3);
				}
			}
			Destroy(gameObject);

		}
	}

	void Update() {
		velocity.y -= 40f * Time.deltaTime;
		transform.position += velocity * Time.deltaTime;
	}

	void OnTriggerEnter2D(Collider2D obj) {
		OnEnter(obj);
	}

}
