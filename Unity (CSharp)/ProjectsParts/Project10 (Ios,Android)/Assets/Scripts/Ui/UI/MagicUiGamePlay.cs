using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MagicUiGamePlay : MonoBehaviour {

	public GameObject panel;

	public Text magicCount;

	private void OnEnable() {
		MagicInit();
	}
	
	void MagicInit() {
		magicCount.text = RunnerController.Instance.allMagicCount.ToString();
	}

	public static void MagicCountSet(int count) {
		//instance.magicCount.text = count.ToString();
	}
}
