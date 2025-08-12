using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapTouch: MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler {
  
  public void OnDrag(PointerEventData eventData) {
    MapManager.Instance.MapScroll.ScrollCamera(eventData.delta.y);
  }

  public void OnPointerDown(PointerEventData eventData) {
    MapManager.Instance.MapScroll.ScreenTapDown();
  }

  public void OnPointerUp(PointerEventData eventData) {
    MapManager.Instance.MapScroll.ScreenTapUp();
  }
  
}
