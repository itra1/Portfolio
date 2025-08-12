using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(ZbCatScene.CatSceneLibrary))]
[CanEditMultipleObjects]
public class CatSceneLibraryEditor : Editor {
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();

		if (GUILayout.Button("Save"))
			foreach (var VARIABLE in targets)
				EditorUtility.SetDirty(VARIABLE);

	}
}

#endif

namespace ZbCatScene {
	

	public class CatSceneLibrary : ScriptableObject {

		public List<HeroInfo> hero;
		public List<CatScene> catSceneList;

	}

	[System.Serializable]
	public struct HeroInfo {
		public string name;
		public HeroType type;
		public Sprite graphic;
	}
	
	public enum HeroType {
		none,
		kuzmich,
		zombyPeasent,
		zomby3Peasent,
		hunter,
		bear,
		zombyYaLegenda,
		zombyElLich,
		generalPrecedent
	}

}