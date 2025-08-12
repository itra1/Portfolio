using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SwipePanel : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler {

	public Action OnDown;
	public Action OnUp;
	public Action<float> OnSwipe;

	private Vector2 positionStart;
	public bool _isSwipe;

	public void OnDrag(PointerEventData eventData) {

		if (!_isSwipe) {
			if (Mathf.Abs(positionStart.x - eventData.position.x) > 10)
				_isSwipe = true;

			if (!_isSwipe) return;
		}

		if (OnSwipe != null) OnSwipe(eventData.delta.x);
	}

	public void OnPointerDown(PointerEventData eventData) {
		positionStart = eventData.position;
		if (OnDown != null) OnDown();
	}

	public void OnPointerUp(PointerEventData eventData) {
		_isSwipe = false;
		if (OnUp != null) OnUp();
	}
}
