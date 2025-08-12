using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(ShopLibrary))]
public class ShopLibraryEditor : Editor {
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();

		if (GUILayout.Button("Найти продукты")) {
			((ShopLibrary)target).shopLibrary = EditorUtils.GetAllPrefabsOfType<ShopProductBehaviour>();
			EditorUtility.SetDirty((ShopLibrary)target);
		}

	}
}
#endif

public class ShopLibrary : ScriptableObject {

	public List<ShopProductBehaviour> shopLibrary;
	

}
