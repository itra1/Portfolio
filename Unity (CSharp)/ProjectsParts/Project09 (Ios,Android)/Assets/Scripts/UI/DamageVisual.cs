using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Визуальное получение дамага
/// </summary>
public class DamageVisual : MonoBehaviour {

  public Text damageValueText;

  private void OnEnable() {
    Destroy(damageValueText.GetComponent<LightTween>());
    Destroy(GetComponent<LightTween>());
  }

  public void SetText(string damageValue) {
    damageValueText.text = damageValue;
    damageValueText.color = new Color(damageValueText.color.r, damageValueText.color.g, damageValueText.color.b, 1);
    LightTween.ColorTo(damageValueText.gameObject,LightTween.Hash("a",0,
                                                  "time", 0.7f,
                                                  "colortype", LightTween.ColorType.text
                                                  ));
    LightTween.MoveTo(gameObject, LightTween.Hash("y",transform.position.y+1,
                                                  "time", 1f,
                                                  "oncomplete", "OnFinish"
                                                  ));
  }

  void OnFinish() {
    Destroy(damageValueText.GetComponent<LightTween>());
    Destroy(GetComponent<LightTween>());
    gameObject.SetActive(false);
  }

}
