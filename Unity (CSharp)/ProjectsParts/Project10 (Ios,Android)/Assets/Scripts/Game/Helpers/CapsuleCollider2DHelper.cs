using System;
using UnityEngine;

public class CapsuleCollider2DHelper : MonoBehaviour {
	public Action<Collider2D> OnEnter;
	//CapsuleCollider2D col;

	private void OnEnable() {
		//col = GetComponent<CapsuleCollider2D>();
	}

	private void OnTriggerEnter2D(Collider2D collision) {
		if (OnEnter != null)
			OnEnter(collision);
	}
}
