using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using it.Network.Rest;
using UnityEngine.EventSystems;

namespace it.Main.SinglePages
{
	public class DragRankPanel : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
	{
		public UnityEngine.Events.UnityAction OnPointerDownAction;
		public UnityEngine.Events.UnityAction OnPointerUpAction;
		public UnityEngine.Events.UnityAction OnPointerDragAction;

		public RectTransform RectTransform;
		//public float _scale;

		//private void Awake()
		//{
		//	float width = GetComponentInParent<Canvas>().GetComponent<RectTransform>().rect.width;
		//}

		public void OnDrag(PointerEventData eventData)
		{
			RectTransform.anchoredPosition = new Vector2(RectTransform.anchoredPosition.x + eventData.delta.x, RectTransform.anchoredPosition.y); 
			OnPointerDragAction?.Invoke();
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			OnPointerDownAction?.Invoke();
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			OnPointerUpAction?.Invoke();
		}
	}
}