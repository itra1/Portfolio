using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Tutorial {
  public class HealthLineHelper: MonoBehaviour, IFocusObject {

    private void OnEnable() {
      if (transform.parent.position.y > 9.5f)
        StartCoroutine(InitCor());
    }

    IEnumerator InitCor() {



      yield return new WaitForSeconds(1f);

      TutorialManager.Instance.Show(Type.enemyStats,null,this);

    }

    public void Focus(bool isFocus, Action OnClick = null) {
      if (isFocus) {
        GetComponent<Canvas>().sortingLayerName = "UI";
        GetComponent<Canvas>().sortingOrder = 1000;
      } else {
        GetComponent<Canvas>().sortingLayerName = "Enemy";
        GetComponent<Canvas>().sortingOrder = 0;
      }
    }
  }
}