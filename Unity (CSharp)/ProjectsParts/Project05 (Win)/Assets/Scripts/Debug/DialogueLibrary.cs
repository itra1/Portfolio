using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace it.Dialogue
{
  [CreateAssetMenu(fileName = "DialogueLibrary", menuName = "ScriptableObject/DialogueLibrary", order = 0)]
  public class DialogueLibrary : it.Game.Items.ItemsLibrary
  {
#if UNITY_EDITOR
	 [ContextMenu("Find")]
	 public void FindFromResourcePath()
	 {
		var items = Resources.LoadAll<it.Dialogue.Dialogue>(Game.ProjectSettings.DialogsItems);
		ReadArray(items);
	 }


	 protected void ReadArray(it.Dialogue.Dialogue[] items)
	 {
		_ItemBase.Clear();
		foreach (var elem in items)
		  _ItemBase.Add(new ItemLibrary { name = elem.gameObject.name, uuid = elem.Uuid, path = AssetDatabase.GetAssetPath(elem.gameObject).Replace("Assets/Resources/", "").Replace(".prefab", "") });
	 }

#endif
  }
}