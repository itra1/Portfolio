using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickScreen : MonoBehaviour, IPointerDownHandler {
  public void OnPointerDown(PointerEventData eventData) {

    QuestionManager.Instance.AddValueQuest(QuestionManager.Type.click, 1);

    ExEvent.BattleEvents.EnemyClick.Call(null);
  }

}
