using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthProgressBar: MonoBehaviour {

  public RectTransform lineImage;
  
  public void SetPercent(float value) {
    lineImage.localScale = new Vector2(value, lineImage.localScale.y);
  }

}
