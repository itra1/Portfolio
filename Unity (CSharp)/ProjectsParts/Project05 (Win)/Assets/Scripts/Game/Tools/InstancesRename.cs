using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace it.Game.Tools
{

#if UNITY_EDITOR

  [CustomEditor(typeof(InstancesRename))]
  public class InstancesRenameEditor : Editor
  {

	 public override void OnInspectorGUI()
	 {
		base.OnInspectorGUI();

		if (GUILayout.Button("Rename"))
		{
		  ((InstancesRename)target).Rename();
		}
	 }

  }

#endif

  public class InstancesRename : MonoBehaviourBase
  {

	 [SerializeField]
	 public List<GameObject> _instance = new List<GameObject>();

	 [SerializeField]
	 private string _increment;

	 public void Rename()
	 {
		for(int i = 0; i < _instance.Count; i++)
		{
		  _instance[i].name = _increment + i.ToString();
		}
	 }

  }
}