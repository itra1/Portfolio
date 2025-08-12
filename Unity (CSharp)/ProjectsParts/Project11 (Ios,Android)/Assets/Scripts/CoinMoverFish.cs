using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinMoverFish : MonoBehaviour {

	private Transform targetMove;

	private Vector3 moveVector = Vector3.up;

	private void OnEnable() {
		transform.localPosition = Vector3.zero;
		moveVector.x = UnityEngine.Random.Range(-2f, 2f);
		moveVector.y = UnityEngine.Random.Range(-3f, 3f);
		//FishManager.Instance.GenerateCoins();
	}
	
	void Move() {
		//moveVector.y -= 5 * Time.deltaTime;
		//transform.position += moveVector * Time.deltaTime * 3;

		moveVector += ((targetMove.position - transform.position).normalized * Time.deltaTime * 10);
		moveVector = moveVector.normalized;

		Vector3 newPos = transform.position + moveVector * Time.deltaTime * 10;
		if ((newPos - transform.position).magnitude <
				(targetMove.position - transform.position).magnitude && (targetMove.position - transform.position).magnitude > 0.1f) {
			transform.position = newPos;
		} else {
			//FishManager.Instance.CoinsMoveComplete();
			//onEndMove();
			gameObject.SetActive(false);
			//myTransform.anchoredPosition = _target.anchoredPosition;
		}

	}

	private void Update() {
		Move();
	}

	public void SetTarget(Transform target) {
		targetMove = target;
	}
	

}
