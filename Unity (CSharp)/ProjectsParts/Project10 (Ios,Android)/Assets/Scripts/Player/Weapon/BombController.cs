using UnityEngine;
using System.Collections;
using Spine;

public class BombController : MonoBehaviour {

	public Rigidbody2D rb;
	public CircleCollider2DHelper bomCollider;

	public float speedX;                                // Скорость
	public float speed { get { return speedX * _vectorKoeff; } }


  private float _angle;                                        // Угол поворота
	
  public float damagePower;                           // Сила повреждения
  [HideInInspector]
  public bool isActive;                  // Флаг активности, что может наносить повреждение
	
  public int damageCount;
	private int _damageReady;

  public WeaponTypes thisWeaponType;                  // Текущий тип оружия
  public GameObject graph;                            // Ссылка на графиескую составляющую, используется для поворота картинки
  public Transform particle;                          // Точка генерации горения фитиля
  public GameObject fitil;                            // Ссылка на точку генерации дыма
  private GameObject _sfx;                                     // Сам дым

	private float _forceY;

  public AudioClip bobmBoomClip;

	private float _vectorKoeff;
	
	private void OnEnable() {
		_vectorKoeff = GameManager.activeLevelData.moveVector == MoveVector.left ? -1 : 1;

		bomCollider.OnEnter = BombCollider;
		bomCollider.gameObject.SetActive(false);
		_damageReady = damageCount;
		_forceY = 10f;
		rb.bodyType = RigidbodyType2D.Dynamic;
		isActive = true;
		graph.SetActive(true);
		rb.AddForce(new Vector2((RunnerController.RunSpeed + speed) , _forceY), ForceMode2D.Impulse);
		if(_sfx == null) _sfx = Instantiate(fitil, particle.position, Quaternion.identity) as GameObject;
		if (isActive) _sfx.transform.position = particle.transform.position;
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
			if (_damageReady <= 0)	return;
			_damageReady--;
			collision.GetComponent<Enemy>().Damage(thisWeaponType, damagePower, Vector3.zero, DamagePowen.level3, 0.2f, false);
		}

	}

	void Update() {
		// Вращение
		if (isActive) {
			_angle += 1000 * Time.deltaTime * _vectorKoeff;
			graph.transform.eulerAngles = new Vector3(0, 0, _angle);
			_sfx.transform.position = particle.transform.position;
		}
		
  }

	private void OnCollisionEnter2D(Collision2D collision) {
		if (LayerMask.LayerToName(collision.gameObject.layer) == "Ground") {
			_forceY /= 1.5f;
			rb.velocity = new Vector2(rb.velocity.x, 0);
			rb.AddForce(new Vector2((RunnerController.RunSpeed + speed), _forceY), ForceMode2D.Impulse);
		}
	}

	void OnTriggerEnter2D(Collider2D col) {
    if(!isActive) return;

    if(col.tag == "Enemy") {
			graph.SetActive(false);
			isActive = false;
			rb.velocity = Vector2.zero;
			rb.bodyType = RigidbodyType2D.Static;
			AudioManager.PlayEffect(bobmBoomClip, AudioMixerTypes.runnerEffect);
			
			ParticleSystem[] particle = _sfx.GetComponentsInChildren<ParticleSystem>();
			for (int i = 0; i < particle.Length; i++) {
				var emi = particle[i].emission;
				emi.rateOverTime = new ParticleSystem.MinMaxCurve(0);
			}

			GameObject inst = Pooler.GetPooledObject("BombBoom");
      inst.transform.position = transform.position;
      inst.SetActive(true);
			StartCoroutine(BombColliderActive());
    }
  }

	IEnumerator BombColliderActive() {
		bomCollider.gameObject.SetActive(true);
		yield return new WaitForFixedUpdate();
		yield return new WaitForFixedUpdate();
		bomCollider.gameObject.SetActive(false);
		gameObject.SetActive(false);
	}

}
