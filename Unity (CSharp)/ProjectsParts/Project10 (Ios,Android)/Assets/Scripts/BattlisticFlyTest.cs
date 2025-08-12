using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlisticFlyTest : MonoBehaviour {

	public Transform target;

	private float distance;
	private float height;

	private Mathematic mat = new Mathematic();
	private Vector3 velocity = Vector3.zero;
	private Vector3 point = Vector3.zero;

	private float speedX = 0;

	private void Start() {
		distance = (target.position - transform.position).magnitude;

		Vector3 targetPoint = Vector3.zero + (target.position - transform.position);

		height = distance / 4;

		Vector3 middlePoint = Vector3.zero + ((targetPoint - Vector3.zero).normalized * (distance / 2)) + Vector3.up * height;

		mat.ParabolaCalcCoef(Vector3.zero, middlePoint, targetPoint);

		velocity.x = distance / 2;
		point = Vector3.zero;
	}
	
	private void Update() {
		Vector3 beforePoint = point;

		point.x += (distance / 2) * Time.deltaTime;
		point.y = mat.ParabolaGetY(point.x);

		velocity = point - beforePoint;
		velocity.x += 5 * Time.deltaTime;

		transform.position += velocity;

	}

}
