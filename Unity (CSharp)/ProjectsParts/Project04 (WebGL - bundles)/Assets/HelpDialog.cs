using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HelpDialog : MonoBehaviour,IPointerDownHandler {
  public void OnPointerDown(PointerEventData eventData) {
    gameObject.SetActive(false);
  }

  private void OnMouseDown() {
    gameObject.SetActive(false);

  }


}
