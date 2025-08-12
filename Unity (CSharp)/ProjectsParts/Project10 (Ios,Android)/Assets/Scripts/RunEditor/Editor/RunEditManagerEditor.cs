using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EditRun {

	[CustomEditor(typeof(EditRunLibrary))]
	public class RunEditManagerEditor : Editor {

		public override void OnInspectorGUI() {
			base.OnInspectorGUI();

			if (GUILayout.Button("Find all spawn object")) {
				((EditRunLibrary)target).objectList = GetAllPrefabsOfType<SpawnObjectInfo>();

				EditorUtility.SetDirty(target);
			}
      if (GUILayout.Button("Save")) {

        EditorUtility.SetDirty(target);

      }
    }

		public List<T> GetAllPrefabsOfType<T>() where T : class {
			List<T> list = new List<T>();
			var allPrefabs = GetAllPrefabs();
			foreach (var single in allPrefabs) {
				T obj = AssetDatabase.LoadAssetAtPath(single, typeof(T)) as T;
				if (obj != null) {
					//Debug.Log("Found prefab of class: "+typeof(T)+" : "+obj);
					list.Add(obj);
				}
			}
			return list;
		}

		public static string[] GetAllPrefabs() {
			string[] temp = AssetDatabase.GetAllAssetPaths();
			List<string> result = new List<string>();
			foreach (string s in temp) {
				if (s.Contains(".asset")) result.Add(s);
			}
			return result.ToArray();
		}

	}
}