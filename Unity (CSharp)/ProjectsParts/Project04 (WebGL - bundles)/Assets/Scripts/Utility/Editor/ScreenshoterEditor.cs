using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Screenshoter))]

public class ScreenshoterEditor : Editor 
{
	public override void OnInspectorGUI() 
	{
		DrawDefaultInspector();

		if (GUILayout.Button ("Prepare Map")) 
		{
			Screenshoter screenshoter = target as Screenshoter;
		}
	}
}
