using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteScreenSize : MonoBehaviour {

	private void OnEnable() {
		StartCoroutine(Scale());
	}

	IEnumerator Scale() {
		yield return new WaitForFixedUpdate();
		ChangeScale();
	}

	private void ChangeScale() {

		Rect scale = GetComponent<SpriteRenderer>().sprite.rect;

		float sourceKoeff = scale.width / scale.height;
		float screenKoeff = (float)Camera.main.pixelWidth / (float)Camera.main.pixelHeight;

		if (sourceKoeff != screenKoeff) {
			float delta = screenKoeff / sourceKoeff;
			transform.localScale = new Vector3(1* delta, 1 , 1);
		}
		
	}

}
