using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinsMover : MonoBehaviour {
	
	private Transform _target;
	private RectTransform myTransform;
	private float step1Height;
	private int step;
	private Action onEndMove;
	private Vector3 moveVector = Vector3.up;

	public void SetMover(RectTransform target, Action onEndMove) {
		step = 0;
		this.onEndMove = onEndMove;
		this._target = target.transform;

		this.myTransform = GetComponent<RectTransform>();
		step1Height = this.myTransform.anchoredPosition.y + UnityEngine.Random.Range(-20, 20);
		moveVector.x = UnityEngine.Random.Range(-0.5f, 0.5f);
		moveVector.y = UnityEngine.Random.Range(1f, 2f);
	}

	private void Update() {

		switch (step) {
			case 0:
				Move1();
				break;
			case 1:
				break;
			case 2:
				Move2();
				break;
		}

	}

	void Move1() {
		moveVector.y -= 10 * Time.deltaTime;
		myTransform.anchoredPosition += (Vector2)moveVector * Time.deltaTime * 1000;

		if (moveVector.y < 0 && myTransform.anchoredPosition.y < step1Height) {
			step = 1;
			StartCoroutine(WaitNext());
		}

	}

	IEnumerator WaitNext() {
		yield return new WaitForSeconds(0.3f);
		moveVector = (_target.position - transform.position).normalized;
		step = 2;
	}

	void Move2() {
		moveVector += ((_target.position - myTransform.position).normalized * Time.deltaTime * 10);
		moveVector = moveVector.normalized;

		Vector3 newPos = myTransform.position + moveVector * Time.deltaTime * 10;
		if ((newPos - myTransform.position).magnitude <
				(_target.position - myTransform.position).magnitude && (_target.position - myTransform.position).magnitude > 0.1f) {
			myTransform.position = newPos;
		} else {
			onEndMove();
			gameObject.SetActive(false);
			//myTransform.anchoredPosition = _target.anchoredPosition;
		}
	}

}
