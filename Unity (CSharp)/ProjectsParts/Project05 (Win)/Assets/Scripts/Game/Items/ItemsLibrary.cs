using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace it.Game.Items
{
  /// <summary>
  /// Базовый класс для библиотеки символов
  /// </summary>
  public class ItemsLibrary : ScriptableObject
  {

	 [SerializeField]
	 protected List<ItemLibrary> _ItemBase = new List<ItemLibrary>();

	 protected List<Item> _itemList = new List<Item>();

	 public string GetPath(string guid)
	 {
		return _ItemBase.Find(x => x.uuid == guid).path;
	 }
	 public string[] GetAll()
	 {
		string[] data = new string[_ItemBase.Count];

		for(int i = 0; i< data.Length; i++)
		{
		  data[i] = _ItemBase[i].path;
		}

		return data;
	 }

#if UNITY_EDITOR

	 protected void ReadArray(Item[] items)
	 {
		_ItemBase.Clear();
		foreach (var elem in items)
		  _ItemBase.Add(new ItemLibrary { name = elem.gameObject.name, uuid = elem.Uuid, path = AssetDatabase.GetAssetPath(elem.gameObject).Replace("Assets/Resources/", "").Replace(".prefab", "") });
	 }

	 [ContextMenu("Save")]
	 public void Save()
	 {
		EditorUtility.SetDirty(this);
	 }

#endif

  }
}