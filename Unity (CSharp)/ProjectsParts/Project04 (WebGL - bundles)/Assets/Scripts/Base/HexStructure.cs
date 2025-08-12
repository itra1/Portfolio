using UnityEngine;
using System.Collections;

public class HexNetBase : MonoBehaviour
{
	static public float[] netAngles = {0.0f, 60.0f, 120.0f, 180.0f, 240.0f, 300.0f};

	public Vector2 netExt = new Vector2(10, 10);

	public Vector2 cellSize = new Vector2(0.35f, 0.45f);

	public Vector2 fullCellShift = new Vector2(0.0f, 0.0f);

	public Vector3 cellShift = new Vector3(1.0f, 0.0f, 1.0f);

	public Vector2 GetCellPos(int x, int y)
	{
		Vector2 cellPos = new Vector2(x * cellSize.x, -y * cellSize.y * 2.0f / 3.0f);
		
		if ((y & 1) != 0)
			cellPos.x += cellSize.x * 0.5f;
		
		return cellPos + new Vector2(cellSize.x / 2.0f, netExt.y * cellSize.y * 2.0f / 3.0f - cellSize.y / 6.0f);
	}

	public Vector3 GetBaseCellShift()
	{
		return new Vector3(cellShift.x + fullCellShift.x * cellSize.x, cellShift.y, cellShift.z + fullCellShift.y * cellSize.y * 4.0f / 3.0f);
	}

	public Vector3 GetCellPos3D(int x, int y)
	{
		Vector2 cellPos = GetCellPos(x, y);
		return new Vector3(cellPos.x, 0, cellPos.y) + GetBaseCellShift();
	}

	public float FindAngle(float curAngle)
	{
		for (int i = 0; i < netAngles.Length - 1; i++)
		{
			if (curAngle > netAngles[i] + 30 && curAngle <= netAngles[i + 1] + 30)
				return netAngles[i + 1];
		}
		
		return netAngles[0];
	}
	
	public Vector3 FindPosition(Vector3 curPosition)
	{
		Vector3 localPosition = curPosition - GetBaseCellShift();

		//

		int yPos = (int)((netExt[1] * cellSize.y * 2.0f / 3.0f - localPosition.z) / (cellSize.y * 2.0f / 3.0f));

		if (yPos < 0) yPos = 0;
		if (yPos >= netExt[1]) yPos = (int)netExt[1] - 1;

		//

		int xPos = 0;

		if ((yPos & 1) != 0)
			xPos = (int)((localPosition.x - cellSize.x / 2.0f) / cellSize.x);
		else
			xPos = (int)(localPosition.x / cellSize.x);
			
		
		
		if (xPos < 0) xPos = 0;
		if (xPos >= netExt[0]) xPos = (int)netExt[0] - 1;

		Vector3 newPos = GetCellPos3D(xPos, yPos);

		newPos.y = curPosition.y;

		return newPos;
	}
}