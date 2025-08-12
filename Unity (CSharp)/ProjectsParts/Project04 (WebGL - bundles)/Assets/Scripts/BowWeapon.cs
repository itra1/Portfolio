using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowWeapon : MonoBehaviour {

	public Transform stringTransf;
	public Transform parentString;
	private Vector3 _startPosition;

	private bool _isMove = false;

	private void Start() {
		_startPosition = stringTransf.localPosition;

		GetComponent<WeaponBehaviour>().OnStartAttack = () => {
			_isMove = true;
		};
		GetComponent<WeaponBehaviour>().OnEndAttack = () => {
			_isMove = false;
			stringTransf.localPosition = _startPosition;
		};
	}

	private void OnDisable() {
		stringTransf.position = _startPosition;
	}
	
	private void LateUpdate() {

		if (stringTransf == null || !_isMove) return;

		stringTransf.position = parentString.position;
	}

}
