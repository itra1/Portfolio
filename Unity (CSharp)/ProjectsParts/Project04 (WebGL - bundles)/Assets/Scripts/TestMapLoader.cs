using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestMapLoader : MonoBehaviour {

	private void Start() {
		return;
		SceneManager.LoadScene(2,LoadSceneMode.Additive);
	}


}
