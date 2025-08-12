using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TextClickPoint : MonoBehaviour, IPointerDownHandler {

	Stack<RectTransform> stack = new Stack<RectTransform>();


	public void OnPointerDown(PointerEventData eventData) {
		stack.Clear();
		RectTransform rct = GetComponent<RectTransform>();

		RectTransform use = rct;
		stack.Push(rct);
		while (use.parent != null) {
			use = use.parent.GetComponent<RectTransform>();
			stack.Push(use);
		}
		Vector2 innerPosition = new Vector2(-Screen.width/2 + eventData.position.x, -Screen.height / 2 + eventData.position.y);
		RectTransform before = stack.Pop();
		Vector2 scale = Vector2.one;
		while (stack.Count > 0) {
			use = stack.Pop();
			//Vector2 tergetAncor = new Vector2(before.rect.width/2+ use.anchoredPosition.x, before.rect.height/2 + use.anchoredPosition.y);
			innerPosition = innerPosition - new Vector2(use.anchoredPosition.x * scale.x, use.anchoredPosition.y * scale.y);
			scale = new Vector2(scale.x * use.localScale.x, scale.y * use.localScale.y);
			Debug.Log(innerPosition + " " + new  Vector2(innerPosition.x / scale.x, innerPosition.y / scale.y));
			before = use;
		}
		Debug.Log(new Vector2(innerPosition.x / scale.x, innerPosition.y / scale.y));
	}
}
