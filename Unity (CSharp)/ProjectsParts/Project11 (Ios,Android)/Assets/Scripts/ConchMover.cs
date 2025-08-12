using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConchMover : MonoBehaviour {

	private Transform _target;
	private Transform myTransform;

	private Action onEndMove;

	private Vector3 moveVector = Vector3.up;

	public void SetMover(RectTransform target, Action onEndMove) {
		this.onEndMove = onEndMove;
		this._target = target.transform;
		this.myTransform = GetComponent<Transform>();
	}

	private void OnEnable() {

		if(UnityEngine.Random.value <= 0.5f)
			moveVector = new Vector2(-1, 1) * 15;
		else
			moveVector = new Vector2(1, -1) * 15;
	}
	
	private void Update() {

		moveVector += ((_target.position - myTransform.position).normalized * Time.deltaTime*10);
		moveVector = moveVector.normalized;

		Vector3 newPos = myTransform.position + moveVector * Time.deltaTime*10;
		if ((newPos - myTransform.position).magnitude <
		    (_target.position - myTransform.position).magnitude && (_target.position - myTransform.position).magnitude > 0.1f) {
			myTransform.position = newPos;
		}else {
			onEndMove();
			gameObject.SetActive(false);
			//myTransform.anchoredPosition = _target.anchoredPosition;
		}
	}
}
