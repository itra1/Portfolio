using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace it.Game.Tools
{

#if UNITY_EDITOR

  [CustomEditor(typeof(MassInstanceColliderCreator))]
  public class MassInstanceColliderCreatorEditor : Editor
  {

	 public override void OnInspectorGUI()
	 {
		base.OnInspectorGUI();

		if (GUILayout.Button("Create"))
		{
		  ((MassInstanceColliderCreator)target).CreateColliders();
		}
	 }

  }

#endif

  public class MassInstanceColliderCreator : MonoBehaviourBase
  {

	 [SerializeField]
	 private bool _destroyExistsCollider = false;

	 [SerializeField]
	 public List<GameObject> _instance = new List<GameObject>();

	 public void CreateColliders()
	 {

		foreach (var elem in _instance)
		{
		  CreateColliderIfNeed(elem);
		}

	 }


	 private void CreateColliderIfNeed(GameObject inst)
	 {
		Transform col = inst.transform.Find("Collider");

		if (col != null)
		{
		  DestroyImmediate(inst);
		  return;
		}

		if (_destroyExistsCollider)
		{
		  MeshCollider[] colliers = inst.GetComponentsInChildren<MeshCollider>();

		  foreach (var elem in colliers)
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

		foreach (var oneMesh in meshes)
		{
		  if (oneMesh.sharedMesh.vertexCount < vertex)
		  {
			 targetMesh = oneMesh.sharedMesh;
			 vertex = oneMesh.sharedMesh.vertexCount;
		  }
		}


		var mesh = colliderObj.AddComponent<MeshCollider>();
		mesh.sharedMesh = targetMesh;

	 }


  }
}