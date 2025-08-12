using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickDerbis : MonoBehaviour {
	public List<GameObject> debrisSprite;
	private SpriteRenderer _activeSprite;

	private bool _isMove;
	private Vector3 _velocity;
	private Vector3 _startPosition;
	private float _rotationSpeed;

	private void OnEnable() {
		_isMove = true;
		_startPosition = transform.position;
		SelectSprite();
		_rotationSpeed = Random.Range(-300, 300);
		_velocity.x = Random.Range(-2, 2);
		_velocity.y = Random.Range(0.5f, 3);
	}

	void SelectSprite() {
		int useSprite = Random.Range(0, debrisSprite.Count);

		for (int i = 0; i < debrisSprite.Count; i++) {
			debrisSprite[i].SetActive(i == useSprite);
		}
		_activeSprite = debrisSprite[useSprite].GetComponent<SpriteRenderer>();
		_activeSprite.color = new Color(1, 1, 1, 1);
	}

	private void Update() {

		if (!_isMove) return;

		Move();
		Rotation();

		if (_velocity.y < 0 && _startPosition.y > transform.position.y)
			StartCoroutine(HideSpriteColor());
	}

	void Move() {
		_velocity.y -= 6 * Time.deltaTime;
		transform.position += _velocity * Time.deltaTime;
	}

	void Rotation() {
		_activeSprite.transform.eulerAngles += new Vector3(0, 0, _rotationSpeed) * Time.deltaTime;
	}

	IEnumerator HideSpriteColor() {
		_isMove = false;

		while (_activeSprite.color.a > 0) {
			_activeSprite.color = new Color(1, 1, 1, _activeSprite.color.a - 0.01f);
			yield return null;
		}
		gameObject.SetActive(false);

	}
}
