using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MinimapClicker : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler {

	public MiniMapBehaviour mm;

	private bool isClick;

	public RectTransform rectTr;

	public void OnDrag(PointerEventData eventData) {
		isClick = false;


		rectTr.anchoredPosition += eventData.delta;
		mm.CheckPosition();
	}

	public void OnPointerDown(PointerEventData eventData) {
		isClick = true;
	}

	public void OnPointerUp(PointerEventData eventData) {
		if (!isClick) return;

		isClick = false;

		mm.SetCameraClickPosition(GetLocalPoint(eventData));
		
		//Debug.Log(eventData.);

	}

	Stack<RectTransform> stack = new Stack<RectTransform>();
	public Vector2 GetLocalPoint(PointerEventData eventData) {
		stack.Clear();
		RectTransform rct = GetComponent<RectTransform>();

		RectTransform use = rct;
		stack.Push(rct);
		while (use.parent != null) {
			use = use.parent.GetComponent<RectTransform>();
			stack.Push(use);
		}

		Vector2 innerPosition = new Vector2(-Screen.width / 2 + eventData.position.x, -Screen.height / 2 + eventData.position.y);
		RectTransform before = stack.Pop();
		Vector2 scale = Vector2.one;
		while (stack.Count > 0) {
			use = stack.Pop();
			//Vector2 tergetAncor = new Vector2(before.rect.width/2+ use.anchoredPosition.x, before.rect.height/2 + use.anchoredPosition.y);
			innerPosition = innerPosition - new Vector2(use.anchoredPosition.x * scale.x, use.anchoredPosition.y * scale.y);
			scale = new Vector2(scale.x * use.localScale.x, scale.y * use.localScale.y);
			//Debug.Log(innerPosition + " " + new Vector2(innerPosition.x / scale.x, innerPosition.y / scale.y));
			before = use;
		}
		return new Vector2(innerPosition.x / scale.x, innerPosition.y / scale.y);
	}

}
