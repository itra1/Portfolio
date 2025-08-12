using UnityEngine;
using System.Collections;

public class PistolController : MonoBehaviour {

	public Rigidbody2D rb;
	public Transform graphic;
	public float damagePower;                                   // Сила повреждения
	public float speedX;
	public float speed { get { return speedX * _vectorKoeff; } }
	Vector3 velocity;

	public GameObject bam;
	public WeaponTypes thisWeaponType;                // Текущий тип оружия
	bool isActive;
	private float _vectorKoeff;

	private void OnEnable() {
		_vectorKoeff = (GameManager.activeLevelData.moveVector == MoveVector.left ? -1 : 1);
		graphic.position = new Vector3(Mathf.Abs(graphic.position.x) * _vectorKoeff, graphic.position.y, graphic.position.z);
		isActive = true;
		GameObject inst = Pooler.GetPooledObject("PistolPaf");
		inst.transform.position = transform.position;
		inst.SetActive(true);

		rb.velocity = new Vector2(RunnerController.RunSpeed + speed, 0);
	}

	private void OnDisable() {
		try {
			if (isActive) {
				Questions.QuestionManager.ConfirmQuestion(Quest.weaponMiss, 1, Player.Jack.PlayerController.Instance.transform.position);
			}
		} catch { }
	}

	//void Update() {
	//   velocity.x = RunnerController.RunSpeed + speed;
	//   transform.position += velocity * Time.deltaTime;

	//   if(transform.position.x <= CameraController.displayDiff.leftDif(2)) {
	//     Vector3 questeff = transform.position;
	//     if(GameObject.Find("Player"))
	//       questeff = GameObject.Find("Player").transform.position;

	//     Questions.QuestionManager.ConfirmQuestion(Quest.weaponMiss, 1, questeff);
	//     Destroy(gameObject);
	//   }
	// }

	void OnTriggerEnter2D(Collider2D col) {
		if (col.tag == "Enemy" && damagePower > 0) {
			col.GetComponent<Enemy>().Damage(thisWeaponType, damagePower, transform.position, DamagePowen.level2, 0, false);
			damagePower = 0;
			GameObject inst = Pooler.GetPooledObject("PistolBoom");
			inst.transform.position = transform.position;
			inst.SetActive(true);
			isActive = false;
			gameObject.SetActive(false);
		}
	}

}
