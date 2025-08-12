using UnityEngine;

public class PointerHandle: MonoBehaviour {
  private MapPointGraphic _mapPointer;

  private MapPointGraphic MapPointer {
    get {
      if (_mapPointer == null)
        _mapPointer = transform.parent.GetComponent<MapPointGraphic>();
      return _mapPointer;
    }
  }

  public void OnPointerDown() {
    MapPointer.OnPointerDown();
  }
  public void OnPointerUp() {
    MapPointer.OnPointerUp();
  }
  public void OnPointerEnter() {
    MapPointer.OnPointerEnter();
  }
  public void OnPointerExit() {
    MapPointer.OnPointerExit();

  }
}
