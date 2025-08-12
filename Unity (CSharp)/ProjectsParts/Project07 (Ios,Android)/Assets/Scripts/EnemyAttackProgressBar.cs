using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyAttackProgressBar: MonoBehaviour {

  public Color defColor;
  public Color activeColor;

  public Image backImage;
  public RectTransform lineImage;

  private float percent = 1;

  private void Start() {
  }

  private void OnEnable() {
    changeColor = StartCoroutine(ColorChange());
  }

  private void OnDisable() {
    StopCoroutine(changeColor);
    backImage.color = defColor;
  }

  public void SetPercent(float value) {
    this.percent = value;

    if(value >= 0.3f)
      backImage.color = defColor;

    lineImage.localScale = new Vector2(value, lineImage.localScale.y);
  }
  Coroutine changeColor;
  IEnumerator ColorChange() {
    while (true) {


      if (this.percent < 0.1f) {
        backImage.color = backImage.color == defColor ? activeColor : defColor;
        yield return new WaitForSeconds(0.1f);
      } else if (this.percent < 0.3f) {
        backImage.color = backImage.color == defColor ? activeColor : defColor;
        yield return new WaitForSeconds(0.3f);
      } else {
        backImage.color = defColor;
      }
      yield return null;
    }
  }

}
