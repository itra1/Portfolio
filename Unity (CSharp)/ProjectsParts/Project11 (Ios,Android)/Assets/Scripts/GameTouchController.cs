using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameTouchController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler,IDragHandler {

	public void OnPointerDown(PointerEventData eventData) {
		Vector3 target = Camera.main.ScreenToWorldPoint(eventData.position);
		target.z -= Camera.main.transform.position.z;
		ExEvent.GameEvents.OnPointerDown.Call(target);
	}
	public void OnPointerUp(PointerEventData eventData) {
		Vector3 target = Camera.main.ScreenToWorldPoint(eventData.position);
		target.z -= Camera.main.transform.position.z;
		ExEvent.GameEvents.OnPointerUp.Call(target);
	}
	public void OnDrag(PointerEventData eventData) {
		Vector3 target = Camera.main.ScreenToWorldPoint(eventData.position);
		target.z -= Camera.main.transform.position.z;
		ExEvent.GameEvents.OnPointerDrag.Call(target);
	}
}
