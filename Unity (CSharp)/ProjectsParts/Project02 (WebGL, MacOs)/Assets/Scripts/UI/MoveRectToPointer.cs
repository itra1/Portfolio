using System.Collections;
using UnityEngine;


public class MoveRectToPointer : MonoBehaviour
{
	private RectTransform _rect;
	public void SetRect(RectTransform rect)
	{
		_rect = rect;
	}

	private void LateUpdate()
	{
		it.Logger.Log(Camera.main.pixelWidth + " : " + Camera.main.pixelHeight);
		_rect.anchoredPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
	}

}
