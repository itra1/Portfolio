using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(MiniMapCreator))]
public class MiniMapCreatorEditor: Editor {
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();

		if (GUILayout.Button("Ready")) {
			((MiniMapCreator)target).Ready();
		}
		if (GUILayout.Button("Create map")) {
			((MiniMapCreator) target).CreateMiniMap();
		}

	}
}


#endif

public class MiniMapCreator : MonoBehaviour {

	public Camera cameraObj;

	public Transform rightBorder;
	public Transform leftBorder;
	public Transform topBorder;
	public Transform bottomBorder;

	public string path;
	public int scale = 1;
	public string number;

	public void Ready() {

		MapBehaviour mb = GameObject.FindObjectOfType<MapBehaviour>();

		transform.position = Vector3.Lerp(mb.miniMap._regionPointStart, mb.miniMap._regionPointEnd, 0.5f);

		rightBorder.position = new Vector3(mb.miniMap._regionPointEnd.x, 70, mb.miniMap._regionPointEnd.z + ((mb.miniMap._regionPointStart.z - mb.miniMap._regionPointEnd.z) / 2));
		leftBorder.position = new Vector3(mb.miniMap._regionPointStart.x, 70, mb.miniMap._regionPointEnd.z + ((mb.miniMap._regionPointStart.z - mb.miniMap._regionPointEnd.z) / 2));
		topBorder.position = new Vector3(mb.miniMap._regionPointStart.x + ((mb.miniMap._regionPointEnd.x - mb.miniMap._regionPointStart.x) / 2), 70, mb.miniMap._regionPointEnd.z);
		bottomBorder.position = new Vector3(mb.miniMap._regionPointStart.x + ((mb.miniMap._regionPointEnd.x - mb.miniMap._regionPointStart.x) / 2), 70, mb.miniMap._regionPointStart.z);
	}

	public void CreateMiniMap() {
		
		Debug.Log("Start create");
		Ready();

    Debug.Log(System.String.Format("{0}\\Levels\\{1}\\map{1}mm.png", Application.dataPath, number) );

		ScreenCapture.CaptureScreenshot(System.String.Format("{0}\\Levels\\{1}\\map{1}mm.png", Application.dataPath, number), scale);

		Debug.Log("End create");
	}

}
