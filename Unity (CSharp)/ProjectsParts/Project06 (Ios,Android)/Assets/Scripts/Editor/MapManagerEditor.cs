using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(MapManager))]
public class MapManagerEditor : Editor {

  public override void OnInspectorGUI() {
    base.OnInspectorGUI();

		if(GUILayout.Button("Save as points")) {
			//((MapManager)target).SavePoints();
		}
		if(GUILayout.Button("Load as points")) {
			((MapManager)target).LoadDataPoints();
		}


		//if(GUILayout.Button("Search Points")) {
		//  ((MapManager)target).SearchPoints();
		//}

		//if(GUILayout.Button("Save Points")) {
		//  ((MapManager)target).SavePoints();
		//}

	}

}
