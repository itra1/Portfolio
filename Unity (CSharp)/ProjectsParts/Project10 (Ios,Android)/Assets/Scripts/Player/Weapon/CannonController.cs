using UnityEngine;
using System.Collections;

public class CannonController : MonoBehaviour {

	public Rigidbody2D rb;
	public CircleCollider2DHelper bomCollider;
	public GameObject graph;

	public float damagePower;                                   // Сила повреждения
	public WeaponTypes thisWeaponType;                          // Текущий тип оружия
	
	public AudioClip boomClip;
	private float _vectorKoeff;

	void OnEnable() {
		_vectorKoeff = (GameManager.activeLevelData.moveVector == MoveVector.left ? -1 : 1);
		bomCollider.OnEnter = BombCollider;
		bomCollider.gameObject.SetActive(false);
		graph.SetActive(true);
		rb.bodyType = RigidbodyType2D.Static;

		StartCoroutine(StartMove());
	}

	IEnumerator StartMove() {
		yield return new WaitForSeconds(Random.Range(0.3f, 1));
		Vector3 pos = new Vector3(CameraController.displayDiff.transform.position.x + CameraController.displayDiff.right + 0.5f,
																			CameraController.displayDiff.transform.position.y + CameraController.displayDiff.top + 0.5f, 0);

		if (GameManager.activeLevelData.moveVector == MoveVector.left) {
			pos = new Vector3(CameraController.displayDiff.transform.position.x + CameraController.displayDiff.left - 0.5f,
																			CameraController.displayDiff.transform.position.y + CameraController.displayDiff.top + 0.5f, 0);
		}

		transform.position = pos;
		Vector2 velocity;
		velocity.x = -((CameraController.displayDiff.right * 2) * Random.Range(1f, 1.5f)) * _vectorKoeff + RunnerController.RunSpeed;
		velocity.y = 0f;
		rb.bodyType = RigidbodyType2D.Dynamic;
		rb.AddForce(velocity, ForceMode2D.Impulse);
	}

	void BombCollider(Collider2D collision) {

		if (collision.tag == "Enemy") {
			collision.GetComponent<Enemy>().Damage(thisWeaponType, damagePower, Vector3.zero, DamagePowen.level3, 0.2f, false);
		}

	}

	private void OnCollisionEnter2D(Collision2D collision) {

		Debug.Log(LayerMask.LayerToName(collision.gameObject.layer));
		if (LayerMask.LayerToName(collision.gameObject.layer) == "Ground") {

			rb.bodyType = RigidbodyType2D.Static;
			AudioManager.PlayEffect(boomClip, AudioMixerTypes.runnerEffect);
			GameObject inst = Pooler.GetPooledObject("BombBoom");
			inst.transform.position = transform.position;
			inst.SetActive(true);
			graph.SetActive(false);

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
