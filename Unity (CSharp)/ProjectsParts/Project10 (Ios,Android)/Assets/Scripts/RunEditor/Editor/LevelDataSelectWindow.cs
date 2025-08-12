using System;
using System.Collections;
using System.Collections.Generic;
using EditRun;
using UnityEngine;
using UnityEditor;

public class LevelDataSelectWindow : EditorWindow {

	public Action<LevelData> onSelect;

	private List<LevelData> runList;

	private Vector2 _scrollListPosition = Vector2.zero;
	private GUIStyle boxStyle;
	private GUIStyle paddingStyle;

	private void OnEnable() {
		LoadPrefabsMapsList();
		this.titleContent = new GUIContent("Выбор уровня");

		this.boxStyle = new GUIStyle(EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).box);
		this.paddingStyle = new GUIStyle();
		this.paddingStyle.padding = new RectOffset(3, 3, 3, 3);
	}

	private void OnGUI() {
		DrawListMapLevel();
	}


	void LoadPrefabsMapsList() {
		runList = GetAllPrefabsOfType<LevelData>();
	}


	void DrawListMapLevel() {

		_scrollListPosition = GUILayout.BeginScrollView(_scrollListPosition);

		float thumbnailMaxHeight = 5;
		RectOffset thumbnailPadding = new RectOffset(6, 6, 6, 3);
		float labelHeight = 8;

		RectOffset padding = this.paddingStyle.padding;

		foreach (var element in runList) {

			Color boxColor = new Color(0.897f, 0.897f, 0.897f, 1f);

			EditorGUILayout.Space();
			GUILayout.Space(3f);
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(8f);

			float thumbnailHeightWithPadding = thumbnailMaxHeight + thumbnailPadding.top + thumbnailPadding.bottom;
			Rect controlRect = GUILayoutUtility.GetRect(0.0f, (thumbnailHeightWithPadding + labelHeight), GUILayout.ExpandWidth(true));
			new Rect(controlRect.x, controlRect.y, controlRect.width, (controlRect.height));

			GUI.color = boxColor;
			GUI.Box(new Rect(controlRect.x - padding.left, controlRect.y - padding.top, controlRect.width + (padding.left + padding.right), controlRect.height + (padding.top + padding.bottom)), "", this.boxStyle);
			GUI.color = Color.white;

			GUI.color = Color.black;
			GUI.Label(new Rect(padding.left * 2, controlRect.y, 400, 18), String.Format("{0} ({1})", (String.IsNullOrEmpty(element.title) ? "Обычный блок" : element.title), element.name));
			GUI.color = Color.white;

			if (Event.current.type == EventType.MouseUp && controlRect.Contains(Event.current.mousePosition)) {
				if (onSelect != null) onSelect(element);
				Close();
			}

			GUILayout.Space(8f);
			EditorGUILayout.EndHorizontal();
			GUILayout.Space(3f);
		}
		GUILayout.EndScrollView();
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
