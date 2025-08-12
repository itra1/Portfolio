using EditRun;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelData))]
public class LevelDataEditor : Editor {
	
	private LevelData _emInstance;

	protected void OnEnable() {
		this._emInstance = this.target as LevelData;

	}

	public override void OnInspectorGUI() {
		base.OnInspectorGUI();
		
		EditorGUILayout.LabelField("Дистанция уровня = " + this._emInstance.levelDistantion + " м");

		if (GUILayout.Button("Сохранить")) {
			EditorUtility.SetDirty(this._emInstance);
		}

	}
}
