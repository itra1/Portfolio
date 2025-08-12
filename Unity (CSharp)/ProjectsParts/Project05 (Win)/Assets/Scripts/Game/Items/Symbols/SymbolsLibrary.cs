using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace it.Game.Items.Symbols
{

#if UNITY_EDITOR

  [CustomEditor(typeof(SymbolsLibrary))]
  public class SymbolsLibraryEditor : Editor
  {
	 public override void OnInspectorGUI()
	 {
		base.OnInspectorGUI();

		if (GUILayout.Button("Find"))
		{

		  ((SymbolsLibrary)target).FindFromResourcePath();
		}

		if (GUILayout.Button("Save"))
		{
		  ((SymbolsLibrary)target).Save();
		}

	 }
  }

#endif

  [CreateAssetMenu(fileName = "SymbolsLibrary", menuName = "ScriptableObject/SymbolsLibrary", order = 0)]
  public class SymbolsLibrary : ItemsLibrary
  {


#if UNITY_EDITOR

	 [ContextMenu("Find")]
	 public void FindFromResourcePath()
	 {
		var items = Resources.LoadAll<Game.Items.Symbols.Symbol>(Game.ProjectSettings.SymbolsItams);
		ReadArray(items);
	 }

#endif

  }


}