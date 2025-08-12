using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageValueVisual : MonoBehaviour {

	public TextMesh textValue;
	private Vector3 _startPosition;
	public float _speed;

	private void OnEnable() {
		textValue.color = Color.white;
		_startPosition = transform.position;
	}

	private void Update() {
		Move();
		if (transform.position.y > _startPosition.y + 1) ColorChange();
	}

	void Move() {
		transform.position += Vector3.up * _speed * Time.deltaTime;
	}

	void ColorChange() {
		textValue.color =new Color(1,1,1, textValue.color.a - 2f*Time.deltaTime);
		if(textValue.color.a <= 0)
			gameObject.SetActive(false);
	}

	public void SetValue(float value) {
		textValue.text = value.ToString();
	}

}
