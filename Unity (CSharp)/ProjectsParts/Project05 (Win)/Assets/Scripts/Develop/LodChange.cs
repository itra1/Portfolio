using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Develop
{
  public class LodChange : MonoBehaviour
  {

	 [SerializeField]
	 private LODGroup[] _loadInstances;

	 [ContextMenu("Change")]
	 public void Change()
	 {

		_loadInstances = FindObjectsOfType<LODGroup>();

		foreach (var elem in _loadInstances)
		{
		  LOD[] lodArray = elem.GetLODs();

		  int index = 0;
		  for (int i = lodArray.Length - 1; i >= 0; i--)
		  {
			 if (index == 0)
			 {
				lodArray[i].screenRelativeTransitionHeight = 0.006f;
			 }
			 else
				lodArray[i].screenRelativeTransitionHeight = index * 0.025f;


			 index++;
		  }

		  elem.SetLODs(lodArray);

		}

	 }

  }
}