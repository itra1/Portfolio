using System.Collections;
using System.Collections.Generic;
using EditRun;
using UnityEngine;

public class MapParent : MonoBehaviour {

	public RunBlock runBlock;
	
	private void OnDrawGizmos() {

		Gizmos.color = Color.gray;
		Gizmos.DrawLine(new Vector3(0, 4, 0), new Vector3(3000, 4, 0));
		Gizmos.DrawLine(new Vector3(0, 3, 0), new Vector3(3000, 3, 0));
		Gizmos.DrawLine(new Vector3(-1.25f, -5, 0), new Vector3(-1.25f, 30, 0));

	}
	
}
