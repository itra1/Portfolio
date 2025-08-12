using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AudioManagement {

	[CustomEditor(typeof(AudioLibrary))]
	public class AudioLibraryEditor : Editor {

		public override void OnInspectorGUI() {
			base.OnInspectorGUI();

			if (GUILayout.Button("Find all prefab")) {
				((AudioLibrary)target).audioClipList = GetAllPrefabsOfType<AudioClip>();

				EditorUtility.SetDirty((AudioLibrary)target);
			}

			if (GUILayout.Button("Create prefabs")) {
				//((AudioLibrary)target).audioClipList = GetAllPrefabsOfType<AudioClip>();
				//ProcassElement((AudioLibrary)target);

				EditorUtility.SetDirty((AudioLibrary)target);
			}

			if (GUILayout.Button("Find all prefabs")) {
				((AudioLibrary)target).audioGroupList = GetAllPrefabsOfType<AudioGroup>();

				EditorUtility.SetDirty((AudioLibrary)target);
			}

		}


		//public void ProcassElement(AudioLibrary script) {

		//	foreach (var element in script.audioClipList) {

		//		string path = AssetDatabase.GetAssetPath(element);
		//		path = path.Split(new char[] { '.' })[0];

		//		string newName = element.name.Replace(" ", "_") + "_audioClip";
		//		newName = element.name.Replace("+", "");

		//		string fullPath = path + ".prefab";
		//		if (AssetDatabase.FindAssets(fullPath).Length > 0) return;

		//		GameObject go = new GameObject();
		//		go.name = newName;

		//		AudioSource asgo = go.AddComponent<AudioSource>();
		//		asgo.playOnAwake = false;

		//		AudioPoint apgo = go.AddComponent<AudioPoint>();

		//		AudioClipData acd = new AudioClipData();
		//		apgo.clipData = acd;
		//		acd.audioClip = element;
		//		apgo.source = asgo;
		//		acd.volume = 1;
		//		acd.pitch = new SpanFloat() { min = 1, max = 1 };

		//		GameObject pref = PrefabUtility.CreatePrefab(fullPath, go, ReplacePrefabOptions.ReplaceNameBased);
		//		DestroyImmediate(go);

		//		if (pref == null) {
		//			Debug.LogError(path);
		//		}

		//		script.audioGroupList.Add(pref.GetComponent<AudioPoint>());

		//	}
		//}

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
				result.Add(s);
			}
			return result.ToArray();
		}

	}
}