using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapPoint: MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler {

  public GameObject activeGraphic,
                    futureGraphic,
                    completeGraphic,
                    bonusGraphic;

  public TMPro.TextMeshPro numberText;

  public Color normal;
  public Color future;

  private Type type;
  public int number;

  public enum Type {
    future,
    active,
    complete,
    bonus
  }

  protected void Start() {

    ConfirmGraphic();
  }

  public void SetType(int number) {
    this.number = number;
    numberText.text = (number + 1).ToString();
    GetComponent<UnityEngine.Rendering.SortingGroup>().sortingOrder = this.number;
  }

  private void ConfirmGraphic() {

    if (number <= PlayerManager.Instance.company.lastLevelComplete) {
      this.type = Type.complete;
      numberText.color = normal;
    } else if (number == PlayerManager.Instance.company.lastLevelComplete + 1) {
      this.type = Type.active;
      numberText.color = normal;
    } else {
      this.type = Type.future;
      numberText.color = future;
    }

    activeGraphic.SetActive(this.type == Type.active);
    futureGraphic.SetActive(this.type == Type.future);
    completeGraphic.SetActive(this.type == Type.complete);
    bonusGraphic.SetActive(this.type == Type.bonus);
  }

  public void Click() {

    if (this.type == Type.future) {
      AudioManager.Instance.library.PlayClickInactiveAudio();
      return;
    }

    AudioManager.Instance.library.PlayClickAudio();

    MapManager.Instance.SetMapPoint(this);
  }

  private bool isDown = false;
  public void OnPointerDown(PointerEventData eventData) {

    isDown = true;
    MapManager.Instance.MapScroll.ScreenTapDown();
  }

  public void OnPointerUp(PointerEventData eventData) {

    if (isDown) {
      Click();
    }
    isDown = false;
    MapManager.Instance.MapScroll.ScreenTapUp();
  }

  public void OnDrag(PointerEventData eventData) {
    isDown = false;
    MapManager.Instance.MapScroll.ScrollCamera(eventData.delta.y);
  }

}
