using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZorbHead : MonoBehaviour {
	public float gravity;
	private Vector3 velocity;
	private float gpoundY;
	private bool _isMove;
	private float angle;
	private float angleSpeed;
	public Transform headGraphic;
	public GameObject imageGraphic;

	public void Init(float gpoundY) {
		this.gpoundY = gpoundY;
		//Destroy(imageGraphic.GetComponent<LightTween>());
		_isMove = true;
		velocity = new Vector3(Random.Range(-3, 3), Random.Range(3, 10), 0);
		angleSpeed = Random.Range(-1000, 1000);
		angle = 0;
	}

	private void Update() {

		if (!_isMove) return;

		velocity.y -= gravity * Time.deltaTime;
		transform.position += velocity * Time.deltaTime;
		angle += angleSpeed * Time.deltaTime;
		headGraphic.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, angle);

		if (transform.position.y <= gpoundY && _isMove) {
			_isMove = false;
			LightTween.SpriteColorTo(imageGraphic.GetComponent<SpriteRenderer>(), new Color(1, 1, 1, 0), 2f, 0, LightTween.EaseType.linear, gameObject, ()=> {
				imageGraphic.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
				Destroy(imageGraphic.GetComponent<LightTween>());
				gameObject.SetActive(false);
			});
		}

	}

}
