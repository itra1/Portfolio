using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class DecorMover : MonoBehaviour {

	public float speedKoeff;
	private Rigidbody2D rb;
	private Vector3 velocity = Vector3.zero;

	private void Start() {
		rb = GetComponent<Rigidbody2D>();
	}

	// Update is called once per frame
	private void Update() {
		velocity.x = RunnerController.RunSpeed * speedKoeff;
		if (RunnerController.cameraStop)
			velocity = Vector2.zero;
		rb.velocity = velocity;
	}
}
