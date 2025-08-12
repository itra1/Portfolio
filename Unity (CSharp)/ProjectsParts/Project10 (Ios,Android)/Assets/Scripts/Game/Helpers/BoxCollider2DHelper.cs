using System;
using UnityEngine;

public class BoxCollider2DHelper : MonoBehaviour {

	public Action<Collider2D> OnEnter;
	//BoxCollider2D col;
	
	//private void OnEnable() {
	//	col = GetComponent<BoxCollider2D>();
	//}

	private void OnTriggerEnter2D(Collider2D collision) {
		if (OnEnter != null) OnEnter(collision);
	}

}
