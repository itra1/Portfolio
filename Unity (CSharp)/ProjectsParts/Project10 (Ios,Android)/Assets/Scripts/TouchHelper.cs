using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class TouchHelper : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IDragHandler {
	
	public Action OnTapDown;
	public Action OnTapUp;
	public Action<Vector2> OnTapDrag;

	public void OnPointerUp(PointerEventData eventData) {
		if (OnTapUp != null) OnTapUp();
	}

	public void OnPointerDown(PointerEventData eventData) {
		if (OnTapDown != null) OnTapDown();
	}

	public void OnDrag(PointerEventData eventData) {
		if (OnTapDrag != null) OnTapDrag(eventData.delta);
	}

}
