using System.Collections;
using UnityEngine;
using UnityEditor;

public class RectTransformResetAncorsEditor : Editor
{
	[MenuItem("CONTEXT/RectTransform/Print ancor position", false)]
	public static void PrintAncor(MenuCommand command)
	{
		RectTransform _rt = command.context as RectTransform;
		it.Logger.Log(_rt.anchoredPosition);
	}
	[MenuItem("CONTEXT/RectTransform/Custom Ancors", false)]
	public static void AddBetterLocator(MenuCommand command)
	{
		RectTransform _rt = command.context as RectTransform;
		RectTransform _pRt = _rt.parent.GetComponent<RectTransform>();

		Vector2 min = _rt.anchorMin;
		Vector2 max = _rt.anchorMax;

		float width = _rt.rect.width;
		float height = _rt.rect.height;

		float leftX = _rt.rect.width * _rt.pivot.x;
		float rightX = _rt.rect.width - leftX;
		float dowmY = _rt.rect.height * _rt.pivot.y;
		float upX = _rt.rect.height - dowmY;

		min = new Vector2((_rt.anchoredPosition.x - leftX + _pRt.rect.width / 2) / _pRt.rect.width,
																(_rt.anchoredPosition.y - dowmY + _pRt.rect.height / 2) / _pRt.rect.height
		);
		max = new Vector2((_rt.anchoredPosition.x + rightX + _pRt.rect.width / 2) / _pRt.rect.width,
																(_rt.anchoredPosition.y + upX + _pRt.rect.height / 2) / _pRt.rect.height
		);

		_rt.anchorMin = min;
		_rt.anchorMax = max;
		_rt.pivot = Vector2.one / 2;
		_rt.sizeDelta = Vector2.zero;
		_rt.anchoredPosition = Vector2.zero;

		//if (_rt.anchorMin == Vector2.zero && _rt.anchorMax == Vector2.one)
		//{
		//	float width = _rt.rect.width;
		//	float height = _rt.rect.height;
		//	//_rt.pivot = Vector2.one / 2;

		//	it.Logger.Log(_rt.anchoredPosition);
		//	it.Logger.Log(_rt.sizeDelta);

		//	//min = new Vector2((_rt.anchoredPosition.x - _rt.rect.width / 2 + _pRt.rect.width / 2) / _pRt.rect.width,
		//	//														(_rt.anchoredPosition.y - _rt.rect.height / 2 + _pRt.rect.height / 2) / _pRt.rect.height
		//	//);
		//	//max = new Vector2((_rt.anchoredPosition.x + _rt.rect.width / 2 + _pRt.rect.width / 2) / _pRt.rect.width,
		//	//														(_rt.anchoredPosition.y + _rt.rect.height / 2 + _pRt.rect.height / 2) / _pRt.rect.height
		//	//);

		//	//_rt.anchorMin = min;
		//	//_rt.anchorMax = max;
		//}
		//else if (_rt.anchorMin == Vector2.one / 2 && _rt.anchorMax == Vector2.one / 2)
		//{
		//	_rt.anchorMin = Vector2.one / 2;
		//	_rt.anchorMax = Vector2.one / 2;
		//	_rt.pivot = Vector2.one / 2;

		//	min = new Vector2((_rt.anchoredPosition.x - _rt.rect.width / 2 + _pRt.rect.width / 2) / _pRt.rect.width,
		//															(_rt.anchoredPosition.y - _rt.rect.height / 2 + _pRt.rect.height / 2) / _pRt.rect.height
		//	);
		//	max = new Vector2((_rt.anchoredPosition.x + _rt.rect.width / 2 + _pRt.rect.width / 2) / _pRt.rect.width,
		//															(_rt.anchoredPosition.y + _rt.rect.height / 2 + _pRt.rect.height / 2) / _pRt.rect.height
		//	);

		//	_rt.anchorMin = min;
		//	_rt.anchorMax = max;
		//	_rt.anchoredPosition = Vector2.zero;
		//	_rt.sizeDelta = Vector2.zero;
		//}

	}

}
