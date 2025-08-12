using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RunBlockEditPrefs : Editor {

	public static string mapBlockEditon;

	public static void PreloadEditorPrefs() {
		mapBlockEditon = EditorPrefs.GetString("mapBlockEditon",null);
	}

	public static void SaveEditorPrefs() {
		EditorPrefs.SetString("mapBlockEditon", mapBlockEditon);
		
	}

}
