using System;
using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;

public class CameraController : Singleton<CameraController> {

	public AnimationCurve moveSpeedCurve;
	public float moveSpeed;

	public void MoveToPosition(Vector3 targetPosition, Action OnComplited) {
		if(_moveCorotine != null)
			StopCoroutine(_moveCorotine);

		_moveCorotine = StartCoroutine(MoveCoroutine(targetPosition, OnComplited));

	}

	private Coroutine _moveCorotine;

	IEnumerator MoveCoroutine(Vector3 targetPosition, Action OnComplited) {
		Vector3 startPosition = targetPosition;
		float maxDistance = (startPosition - transform.position).magnitude;
		bool _isMove = true;
		
		while (_isMove) {
			Vector3 diff = targetPosition - transform.position;
			float speedKoef = moveSpeedCurve.Evaluate((maxDistance - diff.magnitude) / maxDistance);

			if (diff.magnitude < (diff.normalized * speedKoef * moveSpeed * Time.deltaTime).magnitude) {
				transform.position = targetPosition;
				_isMove = false;
				if (OnComplited != null) OnComplited();
			} else {
				transform.position += diff.normalized * speedKoef * moveSpeed * Time.deltaTime;
			}

			yield return null;
		}

	}


}
