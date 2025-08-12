using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Область тапа для реакции боевки
/// </summary>
public class ScreenTap : Singleton<ScreenTap>, IPointerDownHandler, IPointerUpHandler, IDragHandler, IScrollHandler {

  public void OnPointerDown(PointerEventData eventData) {
    ExEvent.ScreenEvents.PointerDown.Call(eventData.position);
  }

  public void OnPointerUp(PointerEventData eventData) {
    ExEvent.ScreenEvents.PointerUp.Call(eventData.position);
  }

  public void OnDrag(PointerEventData eventData) {
    ExEvent.ScreenEvents.PointerDrag.Call(eventData.position, eventData.delta);

	}

  public void OnScroll(PointerEventData eventData) {
    ExEvent.ScreenEvents.Scroll.Call(eventData.scrollDelta.y);
  }

	public void ActiveImage() {
		GetComponent<Image>().enabled = true;
	}

	public void DeactiveImege() {
		GetComponent<Image>().enabled = false;
	}
  
}
