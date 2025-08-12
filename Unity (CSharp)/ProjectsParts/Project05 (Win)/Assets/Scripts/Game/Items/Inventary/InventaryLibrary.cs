using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace it.Game.Items.Inventary
{

#if UNITY_EDITOR

  [CustomEditor(typeof(InventaryLibrary))]
  public class InventaryBaseEditor : Editor
  {
	 public override void OnInspectorGUI()
	 {
		base.OnInspectorGUI();

		if(GUILayout.Button("Find"))
		{

		  ((InventaryLibrary)target).FindFromResourcePath();
		}

		if (GUILayout.Button("Save"))
		{
		  ((InventaryLibrary)target).Save();
		}

	 }
  }

#endif

  [CreateAssetMenu(fileName = "InventaryLibrary", menuName = "ScriptableObject/InventaryLibrary", order = 0)]
  public class InventaryLibrary : ItemsLibrary
  {



#if UNITY_EDITOR
	 [ContextMenu("Find")]
	 public void FindFromResourcePath()
	 {
		var items =  Resources.LoadAll<Game.Items.Inventary.InventaryItem>(Game.ProjectSettings.InventaryItems);
		ReadArray(items);
	 }



#endif


  }
}