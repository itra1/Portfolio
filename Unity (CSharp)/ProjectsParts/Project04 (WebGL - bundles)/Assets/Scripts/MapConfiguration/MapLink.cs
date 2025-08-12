using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(MapLink))]
public class MapLinkEditor: Editor {
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();

		if (GUILayout.Button("Сохранить")) {
			EditorUtility.SetDirty(target);
		}

	}
}
#endif

public class MapLink : ScriptableObject {
	
	public string localPath;
	
	public string mapTitle;
	public string mapId;

#if UNITY_EDITOR
	[MenuItem("Assets/Create/Map Link")]
	public static void CreateLevelAsset() {
		EditorUtil.CreateAsset<MapLink>("Map Link", "Map Link", "asset", "Create");
	}
#endif

}
