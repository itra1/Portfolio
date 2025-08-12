using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class RouletteButton: MonoBehaviour, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler {

  public RouletteController roulett;

  Slider sliderComp;

  bool isMuve;
  bool isActive;

  float needvalue;

  void OnEnable() {
    sliderComp = GetComponent<Slider>();
    isMuve = false;
  }

  void OnGUI() {

    if (isMuve == false && needvalue > 0) {
      sliderComp.value += 0.03f;

      if (sliderComp.value >= needvalue)
        needvalue = 0;

    } else if (isMuve == false && sliderComp.value > 0) {
      sliderComp.value -= (sliderComp.value > 0.03f ? 0.03f : sliderComp.value);
    }
  }

  public void OnPointerExit(PointerEventData eventData) {
    isActive = false;
  }


  public void OnPointerUp(PointerEventData eventData) {

    if (isActive == true || sliderComp.value > 0) {
      isActive = false;
      roulett.StopRotation();
    }
    isMuve = false;

    if (sliderComp.value <= 0)
      needvalue = 0.5f;
  }

  public void OnPointerDown(PointerEventData eventData) {
    isMuve = true;
    isActive = true;
  }

  void StartRotate() {
    //roulett.StartRoulett();
  }

}
