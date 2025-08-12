using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZorbPoint : MonoBehaviour {

	float distanceStart;
	float time = 0.1f;
	float distanceMin = 0.8f;
	float timeNow;

	float targetDistantion;
	float speed;

	bool toBack = false;
	bool isMove = false;

	private void OnEnable() {
		distanceStart = transform.localPosition.magnitude;
	}

	public void ChangePosition(float percent) {
		isMove = true;
		toBack = false;
		targetDistantion = distanceStart - (distanceStart - distanceMin) * percent;
		speed = (distanceStart - targetDistantion) / time;
		timeNow = time;
	}

	void Update() {
		if(!isMove) return;

		//transform.localPosition += ((distanceStart - targetDistantion) / transform.localPosition.normalized.magnitude) * (toBack ? 1 : -1) * transform.localPosition.normalized * Time.deltaTime;
		transform.localPosition += transform.localPosition.normalized * (toBack ? 1 : -1) * speed * Time.deltaTime;
		if(!toBack) {
			if(transform.localPosition.magnitude <= targetDistantion) {
				timeNow = time;
				toBack = true;
			}
		} else {
			if(transform.localPosition.magnitude >= distanceStart) {
				transform.localPosition = transform.localPosition.normalized * distanceStart;
				isMove = false;
				toBack = false;
			}
		}
		timeNow -= Time.deltaTime;
	}
	
}
