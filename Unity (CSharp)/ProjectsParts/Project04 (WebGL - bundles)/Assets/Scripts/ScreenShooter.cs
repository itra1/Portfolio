using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

[CustomEditor(typeof(ScreenShooter))]
public class ScreenShooterEditor : Editor {
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();

		if (GUILayout.Button("Screeshot")) {
			((ScreenShooter) target).ScreenShoot();
		}

	}
}

#endif


public class ScreenShooter : MonoBehaviour {

	public GameObject cameraInstant;

	public string path;
	public int scale = 1;

	public void ScreenShoot() {
		//cameraInstant.SetActive(true);
		ScreenCapture.CaptureScreenshot(path, scale);
		//cameraInstant.SetActive(false);
	}

}
