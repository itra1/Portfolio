using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(ZbCatScene.CatBlockDialog))]
[CanEditMultipleObjects]
public class CatBlockDialogEditor : Editor {
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();

		if (GUILayout.Button("Save"))
			foreach (var VARIABLE in targets)
				EditorUtility.SetDirty(VARIABLE);
		
	}
}

#endif

namespace ZbCatScene {
	// Блок текста
	[System.Serializable]
	public class CatBlockDialog : CatBlockBehaviour {
		public string dialog;
	}
}