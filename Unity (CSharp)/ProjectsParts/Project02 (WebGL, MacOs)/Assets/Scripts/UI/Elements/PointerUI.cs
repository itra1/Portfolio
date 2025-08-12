using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PointerUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	public void OnPointerEnter(PointerEventData eventData)
	{
		AppManager.SetPointerCursor();
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		AppManager.SetDefaultCursor();
	}
}
