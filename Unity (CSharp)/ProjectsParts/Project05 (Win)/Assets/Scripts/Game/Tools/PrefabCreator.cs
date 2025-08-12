using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

#endif
namespace it.Game.Tools
{

#if UNITY_EDITOR

  [CustomEditor(typeof(PrefabCreator))]
  public class PrefabCreatorEditor : Editor
  {

	 public override void OnInspectorGUI()
	 {
		base.OnInspectorGUI();

		if (GUILayout.Button("Create Prefabs"))
		{
		  ((PrefabCreator)target).CreatePrefabs();
		}
	 }

  }

#endif
  public class PrefabCreator : MonoBehaviourBase
  {
	 [SerializeField]
	 private string _path;

	 [SerializeField]
	 public List<GameObject> _instances = new List<GameObject>();

	 public void CreatePrefabs()
	 {
#if UNITY_EDITOR

		foreach (var inst in _instances)
		{
		  string path = Application.dataPath + "/" + _path + "/" + inst.name + ".prefab";
		  GameObject prefab = PrefabUtility.SaveAsPrefabAsset(inst, path);
		  Transform tr = inst.transform;
		  GameObject newPrefab = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
		  newPrefab.transform.SetParent(inst.transform.parent);
		  newPrefab.transform.SetPositionAndRotation(inst.transform.position, inst.transform.rotation);
		  newPrefab.transform.SetSiblingIndex(inst.transform.GetSiblingIndex());
		  DestroyImmediate(inst);
		}

		_instances.Clear();

#endif
	 }
  }
}