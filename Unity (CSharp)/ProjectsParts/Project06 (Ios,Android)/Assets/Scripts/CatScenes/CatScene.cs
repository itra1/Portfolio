using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(ZbCatScene.CatScene))]
[CanEditMultipleObjects]
public class CatSceneEditor : Editor {
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();

		if (GUILayout.Button("Save"))
			foreach (var VARIABLE in targets)
				EditorUtility.SetDirty(VARIABLE);

	}
}

#endif

namespace ZbCatScene {

	public class CatScene : ScriptableObject {

		[HideInInspector]
		public bool isShow = false;

		public bool noAutoSave;
		public bool isPause;

		public string id;

		public List<CatBlockBehaviour> catBlockList;


	}
	
}