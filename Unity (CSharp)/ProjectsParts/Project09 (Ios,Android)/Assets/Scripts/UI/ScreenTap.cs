using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class ScreenTap : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IDragHandler {
  
  public static event OnPointerDelegate OnPointUp;                          // Событие окончания тача
  public static event OnPointerDelegate OnPointDown;                        // Событие начала тача
  public static event OnPointerDelegate OnPointDrag;                        // Событие выполнения смахивания
  public delegate void OnPointerDelegate(Vector3 pointerPosition);
  /// <summary>
  /// Отпускание курсора
  /// </summary>
  /// <param name="eventData"></param>
  public void OnPointerUp(PointerEventData eventData) {
    if (OnPointUp != null) OnPointUp(eventData.position);
  }
  /// <summary>
  /// Событие зажатия клавиши
  /// </summary>
  /// <param name="eventData"></param>
  public void OnPointerDown(PointerEventData eventData) {
    if(OnPointDown != null) OnPointDown(eventData.position);
  }
  /// <summary>
  /// Драг курсора
  /// </summary>
  /// <param name="eventData"></param>
  public void OnDrag(PointerEventData eventData) {
    if(OnPointDrag != null) OnPointDrag(eventData.position);
  }
}
