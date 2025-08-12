using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace it.Cartoons
{
  //[CreateAssetMenu(fileName = "CartoonLibrary", menuName = "ScriptableObject/CutSceneLibrary", order = 0)]
  public class CartoonLibrary : ScriptableObject
  {

	 [SerializeField]
	 protected List<ItemLibrary> _ItemBase = new List<ItemLibrary>();

	 protected List<Cartoon> _itemList = new List<Cartoon>();

	 public string GetPath(string uuid)
	 {
		return _ItemBase.Find(x => x.uuid.Equals(uuid)).path;
	 }

#if UNITY_EDITOR
	 [ContextMenu("Find")]
	 public void FindFromResourcePath()
	 {
		var items = Resources.LoadAll<it.Cartoons.Cartoon>(Game.ProjectSettings.Catroons);
		ReadArray(items);
	 }

	 protected void ReadArray(Cartoon[] items)
	 {
		_ItemBase.Clear();
		foreach (var elem in items)
		  _ItemBase.Add(new ItemLibrary { name = elem.gameObject.name, uuid = elem.Uuid, path = AssetDatabase.GetAssetPath(elem.gameObject).Replace("Assets/Resources/", "").Replace(".prefab", "") });
	 }
#endif
  }
}