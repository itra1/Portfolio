using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Таймер
/// </summary>
public class GamePlayTimerHelper : MonoBehaviour {

  float fromValue;
  float toValue;
  float speetTimer;
  float value;

  Image timerImage;

  void OnEnable() {
    timerImage = GetComponent<Image>();
  }

  public void SetTimerNewValue(float allTime, float newFromValue = 1 , float newToValue = 0) {
    fromValue = newFromValue;
    toValue = newToValue;
    value = fromValue;
    speetTimer = ( newFromValue - newToValue ) / allTime;
  }

  void Update() {
    if(value > toValue) {
      value -= speetTimer * Time.deltaTime;
      if (value < toValue) value = toValue;
      timerImage.fillAmount = value;
    }
  }

}
