using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;
#if UNITY_EDITOR

using UnityEditor;

#endif

namespace it.Game.Tools
{

#if UNITY_EDITOR

  [CustomEditor(typeof(MeshColliderCreator))]
  public class MeshColliderCreatorEditor : Editor
  {

	 public override void OnInspectorGUI()
	 {
		base.OnInspectorGUI();

		if (GUILayout.Button("Create"))
		{
		  ((MeshColliderCreator)target).CreateCollider();
		}
	 }

  }

#endif

  [AddComponentMenu("Tools/MeshColliderCreator")]
  public class MeshColliderCreator : MonoBehaviourBase
  {
	 [SerializeField]
	 public MeshFilter _meshFilter;

#if UNITY_EDITOR



	 public void CreateCollider()
	 {
		if (_meshFilter == null)
		  return;

		GameObject colliderObj = new GameObject();
		colliderObj.transform.SetParent(transform);
		colliderObj.name = "Collider";
		colliderObj.transform.localPosition = Vector3.zero;
		colliderObj.transform.localScale = Vector3.one;
		colliderObj.transform.localEulerAngles = Vector3.zero;

		var mesh = colliderObj.AddComponent<MeshCollider>();
		mesh.sharedMesh = _meshFilter.sharedMesh;
		
		
		DestroyImmediate(this);
	 }
#endif

  }
}