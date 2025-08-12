using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Develop
{
#if UNITY_EDITOR
  [CustomEditor(typeof(FindAndRepeatPrefabs))]
  public class FindAndRepeatPrefabsEditor : Editor
  {
	 public override void OnInspectorGUI()
	 {
		base.OnInspectorGUI();
		if (GUILayout.Button("Find"))
		{
		  ((FindAndRepeatPrefabs)target).Find();
		}
		if (GUILayout.Button("Replase"))
		{
		  ((FindAndRepeatPrefabs)target).Replace();
		}
		if (GUILayout.Button("Mass Replace"))
		{
		  ((FindAndRepeatPrefabs)target).MassReplace();
		}
	 }
  }

#endif

  public class FindAndRepeatPrefabs : MonoBehaviour
  {
	 [SerializeField]
	 private List<GameObject> _instanceList;

	 [SerializeField]
	 public GameObject _prefab;

	 [SerializeField]
	 private List<GameObject> _prefabList;

#if UNITY_EDITOR

	 [ContextMenu("Find")]
	 public void Find()
	 {
		Debug.Log("Start find");
		FillList(_prefab);
		Debug.Log("End find");

	 }

	 private void FillList(GameObject prefab)
	 {
		_instanceList.Clear();

		Regex regex = new Regex(@"^" + prefab.name);

		Transform[] files = FindObjectsOfType<Transform>();

		for (int i = 0; i < files.Length; i++)
		{
		  if (!files[i].gameObject.name.Contains(prefab.name))
			 continue;

		  if (regex.Matches(files[i].gameObject.name).Count <= 0)
			 continue;

		  if (files[i].gameObject.name.Contains("LOD"))
			 continue;

		  if (PrefabUtility.HasPrefabInstanceAnyOverrides(files[i].gameObject, false))
			 continue;

		  _instanceList.Add(files[i].gameObject);

		}
	 }

	 [ContextMenu("Replase")]
	 public void Replace()
	 {
		if (_instanceList.Count <= 0)
		  return;

		ReplaceList(_prefab);
	 }

	 private void ReplaceList(GameObject prefab)
	 {
		foreach (var elem in _instanceList)
		{
		  ReplaceOne(elem, prefab);
		}
	 }

	 private void ReplaceOne(GameObject elem, GameObject prefab)
	 {
		GameObject inst = PrefabUtility.InstantiatePrefab(prefab, elem.transform.parent) as GameObject;
		inst.transform.localScale = elem.transform.localScale;
		inst.transform.localPosition = elem.transform.localPosition;
		inst.transform.localRotation = elem.transform.localRotation;
		DestroyImmediate(elem);
	 }

	 [ContextMenu("MassReplace")]
	 public void MassReplace()
	 {
		foreach(var elem in _prefabList)
		{
		  FillList(elem);
		  ReplaceList(elem);
		}
	 }

#endif
  }
}