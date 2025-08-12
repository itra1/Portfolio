using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CoctailController : MonoBehaviour {

	public Rigidbody2D rb;
	public CircleCollider2DHelper bomCollider;
	public GameObject graph;

	public float damagePower;                                                // Сила повреждения
  public WeaponTypes thisWeaponType;                    // Текущий тип оружия
  public LayerMask damageMask;
  public float damageRadius;

  public float speed;
  float angle;

  public int damageCount;
	int damageReady;
  List<int> enemyDamages = new List<int>();

	public LayerMask groundMask;
  public float groundRadius;

  public GameObject boom;
  public GameObject fires;

  public AudioClip boomClip;
  Vector3 target;
  //Vector3 velocity = Vector3.zero;                    // Рассчет смещения
  public float gravity;                               // Значение графитации
  float speedX;
	public float speedActual { get { return speedX * _vectorKoeff; } }
	float rotateSeed;
	bool isActive;
	private float _vectorKoeff;

	void OnEnable() {
		_vectorKoeff = (GameManager.activeLevelData.moveVector == MoveVector.left ? -1 : 1);
		bomCollider.OnEnter = BombCollider;
		bomCollider.gameObject.SetActive(false);
		damageReady = damageCount;
		isActive = true;
		enemyDamages.Clear();
		graph.SetActive(true);

		Vector3 target = new Vector3(Random.Range(CameraController.displayDiff.leftDif(0.95f), CameraController.displayDiff.leftDif(0.8f)),
                        3f,
                        transform.position.z);
		if (target.x > transform.position.x) target.x = transform.position.x;

		if (GameManager.activeLevelData.moveVector == MoveVector.left) {
			target = new Vector3(Random.Range(CameraController.displayDiff.rightDif(0.8f), CameraController.displayDiff.rightDif(0.95f)),
												3f,
												transform.position.z);
			if (target.x < transform.position.x) target.x = transform.position.x;
		}
		
		Vector3 velocity = Vector3.zero;                    // Рассчет смещения

		velocity.y = 25f + (target.y - transform.position.y);
    speedX = target.x - transform.position.x;

		if (GameManager.activeLevelData.moveVector == MoveVector.left) {
			//velocity.y = 25f + (transform.position.y - target.y);
			speedX = transform.position.x - target.x;
		}
		
		velocity.x = RunnerController.RunSpeed + speedActual;
		rotateSeed = Random.Range(1000, 1500) * _vectorKoeff;
		rb.AddForce(velocity, ForceMode2D.Impulse);

	}

	private void OnDisable() {
		try {
			if (isActive) {
				Questions.QuestionManager.ConfirmQuestion(Quest.weaponMiss, 1, Player.Jack.PlayerController.Instance.transform.position);
			}
		} catch { }
	}

	void BombCollider(Collider2D collision) {

		if (collision.tag == "Enemy") {
			if (damageReady <= 0)
				return;
			damageReady--;
			collision.GetComponent<Enemy>().Damage(thisWeaponType, damagePower, Vector3.zero, DamagePowen.level3, 0.2f, false);
		}

	}

	void Update() {
    angle += rotateSeed * Time.deltaTime;
    transform.eulerAngles = new Vector3(0, 0, angle);

    //// Гравитация
    //velocity.y -= gravity * Time.deltaTime;

    //velocity.x = RunnerController.RunSpeed + speedX;
    //transform.position += velocity * Time.deltaTime;

    //Collider[] isGrounded = Physics.OverlapSphere(transform.position, groundRadius, groundMask);

    //if(isGrounded.Length > 0) {
    //  AudioManager.PlayEffects(boomClip, AudioMixerTypes.runnerEffect);

      //Collider[] damageObjects = Physics.OverlapSphere(transform.position, damageRadius, damageMask);

      //for(int i = 0; i < damageObjects.Length; i++) {
      //  if(damagePower > 0 && damageObjects[i].GetComponent<Enemy>() && enemyDamages.Count < damageCount && !enemyDamages.Exists(x => x == damageObjects[i].GetInstanceID())) {
      //    enemyDamages.Add(damageObjects[i].GetInstanceID());
      //    damageObjects[i].GetComponent<Enemy>().Damage(thisWeaponType, damagePower, transform.position, DamagePowen.level2, 0.5f, false);
      //  }
      //}

      //if(damageObjects.Length <= 0) {
      //  Vector3 questeff = transform.position;
      //  if(GameObject.Find("Player"))
      //    questeff = GameObject.Find("Player").transform.position;

      //  Questions.QuestionManager.ConfirmQuestion(Quest.weaponMiss, 1, questeff);
      //}

      // Взрыв
      //Vector3 pos = new Vector3(transform.position.x, transform.position.y + 0.1f, 0);
      //GameObject bm = Instantiate(boom, pos, Quaternion.identity) as GameObject;
      //Destroy(bm, 2f);
      //if(Random.value <= 0.7f) Instantiate(fires, transform.position + new Vector3(0, 0, 0), Quaternion.identity);
      //Destroy(gameObject);
    //}
  }

	private void OnCollisionEnter2D(Collision2D collision) {
		if (isActive == false)
			return;

		isActive = false;
		AudioManager.PlayEffect(boomClip, AudioMixerTypes.runnerEffect);
		Vector3 pos = new Vector3(transform.position.x, transform.position.y + 0.1f, 0);
		GameObject bm = Instantiate(boom, pos, Quaternion.identity) as GameObject;
		Destroy(bm, 2f);
		if (Random.value <= 0.7f)
			Instantiate(fires, transform.position + new Vector3(0, 0, 0), Quaternion.identity);
		StartCoroutine(BombColliderActive());
	}

	IEnumerator BombColliderActive() {
		bomCollider.gameObject.SetActive(true);
		yield return new WaitForFixedUpdate();
		yield return new WaitForFixedUpdate();
		bomCollider.gameObject.SetActive(false);
		gameObject.SetActive(false);
	}

}
