using UnityEngine;
using System.Collections;

namespace it.Game.Utils
{
  [ExecuteInEditMode]
  public class MaterialInstances : MonoBehaviour
  {
	 private void Start()
	 {
		Renderer rend = GetComponent<Renderer>();

		if(rend.materials.Length > 1)
		{
		  Debug.LogError("Материалов больше одного");
		  return;
		}

		rend.material = Instantiate(rend.material);
		DestroyImmediate(this);
	 }
  }
}