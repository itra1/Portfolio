using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace it.Game.Player
{

#if UNITY_EDITOR
  [CustomEditor(typeof(UserSettings))]
  public class UserSettingsEditor : Editor
  {

	 public override void OnInspectorGUI()
	 {
		base.OnInspectorGUI();

		if (GUILayout.Button("Save"))
		{
		  ((UserSettings)target).Save();
		}

	 }

  }
#endif

  [CreateAssetMenu(fileName = "UserSettings", menuName = "Tools/Create UserSettings", order = 1)]
  public class UserSettings : ScriptableObject
  {


#if UNITY_EDITOR

	 public void Save()
	 {
		EditorUtility.SetDirty(this);
	 }

#endif
  }
}