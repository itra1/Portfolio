using System;
using UnityEngine;

public class CircleCollider2DHelper : MonoBehaviour {
	public Action<Collider2D> OnEnter;
	//CircleCollider2D col;

	private void OnEnable() {
		//col = GetComponent<CircleCollider2D>();
	}

	private void OnTriggerEnter2D(Collider2D collision) {
		if (OnEnter != null)
			OnEnter(collision);
	}
	
}
