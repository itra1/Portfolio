using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace it.Game.Tools
{

#if UNITY_EDITOR

  [CustomEditor(typeof(MassPrefabColliderCreator))]
  public class MassPrefabColliderCreatorEditor : Editor
  {

	 public override void OnInspectorGUI()
	 {
		base.OnInspectorGUI();

		if (GUILayout.Button("Create"))
		{
		  ((MassPrefabColliderCreator)target).CreateColliders();
		}
	 }

  }

#endif
  /// <summary>
  /// Массовое создание коллайдеров, только для камней
  /// </summary>
  public class MassPrefabColliderCreator : MonoBehaviourBase
  {

#if UNITY_EDITOR

	 [SerializeField]
	 private bool _destroyExistsCollider = false;

	 [SerializeField]
	 public List<GameObject> _prefabs = new List<GameObject>();
	 [SerializeField]
	 public List<GameObject> _Result = new List<GameObject>();

	 public void CreateColliders()
	 {
		
		foreach(var elem in _prefabs)
		{
		  CreateColliderIfNeed(elem);
		}
		foreach (var res in _Result)
		{
		  PrefabUtility.ApplyPrefabInstance(res, InteractionMode.AutomatedAction);
		}

	 }

	 private void CreateColliderIfNeed(GameObject prefab)
	 {
		GameObject inst = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
		inst.transform.SetParent(transform);
		inst.transform.localPosition = Vector3.zero;
		inst.transform.localScale = Vector3.one;
		inst.transform.localEulerAngles = Vector3.zero;

		Transform col = inst.transform.Find("Collider");

		if (col != null)
		{
		  DestroyImmediate(inst);
		  return;
		}

		if (_destroyExistsCollider)
		{
		  MeshCollider[] colliers = inst.GetComponentsInChildren<MeshCollider>();

		  foreach(var elem in colliers)
		  {
			 DestroyImmediate(elem);
		  }
		}

		GameObject colliderObj = new GameObject();
		colliderObj.transform.SetParent(inst.transform);
		colliderObj.name = "Collider";
		colliderObj.transform.localPosition = Vector3.zero;
		colliderObj.transform.localScale = Vector3.one;
		colliderObj.transform.localEulerAngles = Vector3.zero;

		MeshFilter[] meshes = inst.GetComponentsInChildren<MeshFilter>();

		int vertex = 1999999999;
		Mesh targetMesh = null;
		
		foreach(var oneMesh in meshes)
		{
		  if(oneMesh.sharedMesh.vertexCount < vertex)
		  {
			 targetMesh = oneMesh.sharedMesh;
			 vertex = oneMesh.sharedMesh.vertexCount;
		  }
		}


		var mesh = colliderObj.AddComponent<MeshCollider>();
		mesh.sharedMesh = targetMesh;
		_Result.Add(inst);

	 }

#endif
  }
}