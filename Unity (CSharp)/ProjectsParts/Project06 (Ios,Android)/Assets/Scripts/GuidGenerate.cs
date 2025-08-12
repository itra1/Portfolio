using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(GuidGenerate))]
[CanEditMultipleObjects]
public class GuidGenerateEditor: Editor {

	public override void OnInspectorGUI() {
		base.OnInspectorGUI();
		if (GUILayout.Button("Generate")) {
			foreach (var tr in targets) {
				((GuidGenerate)tr).GenerateGuide();
			}
		}
	}
}

#endif

public class GuidGenerate : MonoBehaviour {

	public void GenerateGuide() {

		IGuid ig = GetComponent<IGuid>();

		if(ig == null) return;

		ig.Guid = System.Guid.NewGuid().ToString();
		
	}

}
