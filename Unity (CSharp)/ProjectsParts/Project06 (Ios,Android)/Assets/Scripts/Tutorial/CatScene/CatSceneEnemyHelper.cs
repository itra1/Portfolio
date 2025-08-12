using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatSceneEnemyHelper : MonoBehaviour {

	public string catSceneNum;
	public bool isUse = false;
	public float distantion;

	private void Update() {
		if (!isUse && transform.position.x <= distantion) {
			isUse = true;
			ZbCatScene.CatSceneManager.Instance.ShowCatScene(catSceneNum, () => { });
		}
	}


}
