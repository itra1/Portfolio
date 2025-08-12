using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using TMPro;

public class AttackLabel : MonoBehaviour {

  public TextMeshPro text;
  public Animation animComponent;
  public SortingGroup sorterGroup;

  public void SetData(int sorting) {
    animComponent.Play("show");
    sorterGroup.sortingOrder = sorting + 1;
    text.sortingOrder = -(int)((text.transform.position.y * 100) - 1);
  }

  public void AnimComplete() {
    gameObject.SetActive(false);
  }
}
