using UnityEngine;

public static class RectTransformHelper
{

	public static void FullRect(this RectTransform rt)
	{
		rt.localPosition = Vector3.zero;
		rt.anchorMin = Vector3.zero;
		rt.anchorMax = Vector3.one;
		rt.anchoredPosition = Vector3.zero;
		rt.localScale = Vector3.one;
	}

}
