using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LanguageUiDragHelper : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler {

	public LanguageUi panel;


	public void OnPointerDown(PointerEventData eventData) {
		panel.ViewDown(eventData);
	}

	public void OnPointerUp(PointerEventData eventData) {
		panel.ViewUp(eventData);
	}
	public void OnDrag(PointerEventData eventData) {
		Debug.Log("OnDrag");
		panel.ViewDrag(eventData);
	}

}
