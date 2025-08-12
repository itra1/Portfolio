using UnityEngine;
using System.Collections;

public class GreedPositingObject : MonoBehaviour
{
  public GameObject[] _objects;
  public float _distance;

  [ContextMenu("Positing")]
  public void Positing()
  {
	 int leghtSide = (int)Mathf.Ceil(Mathf.Sqrt(_objects.Length));

	 for(int x = 0; x < leghtSide; x++)
	 {
		for(int y = 0; y < leghtSide; y++)
		{
		  int index = x * leghtSide + y;
		  if (index >= _objects.Length)
			 break;

		  _objects[index].transform.position = new Vector3(x * _distance, 0, y * _distance);

		}
	 }

  }

}
