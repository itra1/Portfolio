using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

[CustomEditor(typeof(UiElementsLibrary))]
class UiElementsLibraryEditor:Editor {
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();

		if (GUILayout.Button("Find All Panel")) {
			((UiElementsLibrary) target).guiList = EditorUtil.GetAllPrefabsOfType<UiPanel>();
      EditorUtility.SetDirty(((Shop)target).gameObject);
    }

	}
}


#endif

public class UiElementsLibrary : MonoBehaviour {
	
	public List<UiPanel> guiList;

}
