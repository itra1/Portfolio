using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CellLineRenderer : MonoBehaviour {

	public void CellRenderer(Vector3 position, float size) {

		LineRenderer _line = GetComponent<LineRenderer>();

		List<Vector3> postionList = new List<Vector3>();

		postionList.Add(position + new Vector3(size * -0.433f, 0, size * 0.25f));
		postionList.Add(position + new Vector3(0, 0, size / 2));
		postionList.Add(position + new Vector3(size * 0.433f, 0, size * 0.25f));
		postionList.Add(position + new Vector3(size * 0.433f, 0, size * -0.25f));
		postionList.Add(position + new Vector3(0, 0, -size / 2));
		postionList.Add(position + new Vector3(size * -0.433f, 0, size * -0.25f));
		postionList.Add(position + new Vector3(size * -0.433f, 0, size * 0.25f));

		// Заполняем позициями

		_line.positionCount = postionList.Count;
		_line.SetPositions(postionList.ToArray());

	}



}
