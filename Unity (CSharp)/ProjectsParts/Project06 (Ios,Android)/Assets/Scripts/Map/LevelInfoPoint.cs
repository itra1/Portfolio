using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(LevelInfoPoint))]
[CanEditMultipleObjects]
public class LevelInfoEditor : Editor {
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();
		if (GUILayout.Button("Rename")) {
			foreach (var VARIABLE in targets) {
				((LevelInfoPoint)VARIABLE).Rename();
			}
		}
	}
}

#endif

public class LevelInfoPoint : MonoBehaviour {

	public LevelInfo levelInfo;

	public void Rename() {
#if UNITY_EDITOR
		AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(gameObject),
			String.Format("{0}_LevelInfo_{1}_{2}", levelInfo.PointNum, levelInfo.Group, levelInfo.Level));
#endif
	}
  
}
