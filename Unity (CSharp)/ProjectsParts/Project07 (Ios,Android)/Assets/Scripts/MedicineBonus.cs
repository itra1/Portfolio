using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MedicineBonus: BonusBehaviour, Tutorial.IFocusObject {
  protected override void GetBonus() {
    base.GetBonus();
    UserManager.Instance.UseMedBonus(count);
  }

  public SortingLayer sortingDef;
  public Canvas canvas;

  protected override void Start() {
    targetPanel = BattleController.Instance.panel.iconSoftCash.position;
    base.Start();

  }

  protected void OnEnable() {

    StartCoroutine(WaitFunc(() => {

      Tutorial.TutorialManager.Instance.Show(Tutorial.Type.medicine, ()=> { }, this);
    }, 0.5f));
    
  }

  public override void OnPointerDown(PointerEventData eventData) {
    base.OnPointerDown(eventData);
    if (TutorClick != null) TutorClick();
  }

  private Action TutorClick;
  public void Focus(bool isFocus, Action OnClick) {

    if (isFocus) {
      TutorClick = OnClick;
      image.sortingLayerName = "UI";
      image.sortingOrder = 1000;
      canvas.sortingLayerName = "UI";
      canvas.sortingOrder = 1000;
    } else {
      image.sortingLayerName = "Enemy";
      image.sortingOrder = 0;
      canvas.sortingLayerName = "Enemy";
      canvas.sortingOrder = 10;
    }

  }
}
