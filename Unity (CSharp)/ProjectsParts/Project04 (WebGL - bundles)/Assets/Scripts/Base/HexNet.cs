using UnityEngine;
using System.Collections;

[ExecuteInEditMode]

public class HexNet : HexNetBase
{
	void Start()
	{
		
	}

	protected void DrawCell(int x, int z)
	{
		Vector3 cellPos = transform.position + GetCellPos3D(x, z);

		Debug.DrawLine(cellPos + new Vector3(cellSize.x / 2.0f, 0, cellSize.y / 6.0f), cellPos + new Vector3(cellSize.x / 2.0f, 0, -cellSize.y / 6.0f));
		Debug.DrawLine(cellPos + new Vector3(-cellSize.x / 2.0f, 0, cellSize.y / 6.0f), cellPos + new Vector3(-cellSize.x / 2.0f, 0, -cellSize.y / 6.0f));

		Debug.DrawLine(cellPos + new Vector3(-cellSize.x / 2.0f, 0, cellSize.y / 6.0f), cellPos + new Vector3(0, 0, cellSize.y / 2.0f));
		Debug.DrawLine(cellPos + new Vector3(cellSize.x / 2.0f, 0, cellSize.y / 6.0f), cellPos + new Vector3(0, 0, cellSize.y / 2.0f));

		Debug.DrawLine(cellPos + new Vector3(-cellSize.x / 2.0f, 0, -cellSize.y / 6.0f), cellPos + new Vector3(0, 0, -cellSize.y / 2.0f));
		Debug.DrawLine(cellPos + new Vector3(cellSize.x / 2.0f, 0, -cellSize.y / 6.0f), cellPos + new Vector3(0, 0, -cellSize.y / 2.0f));
	}

	void Update()
	{
		for (int x = 0; x < netExt.x; x++)
		{
			for (int y = 0; y < netExt.y; y++)
			{
				DrawCell(x, y);
			}
		}
	}
}