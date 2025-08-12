using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Иконка альтернативного квеста
/// </summary>
public class AlterQuestIcon : MonoBehaviour {

  public GameObject graphic;

  bool isClick;
  public TextMesh[] textTimerArray;

  
  void Start() {
    if (!AlterQuest.Instance.isActiveQuest) {
      OnQuestActive(false);
    }

    AlterQuest.OnQuestActive += OnQuestActive;
    AlterQuest.OnTimeChange += SetTimerValue;
  }

  void OnDestroy() {
    AlterQuest.OnQuestActive -= OnQuestActive;
    AlterQuest.OnTimeChange -= SetTimerValue;
  }

  void OnQuestActive(bool isActive) {
    graphic.SetActive(isActive);
  }

  
  /// <summary>
  /// Изменение значения таймера
  /// </summary>
  /// <param name="timeValue">Структура времени</param>
  public void SetTimerValue(TimeStruct timeValue) {
    textTimerArray[0].text = timeValue.hour.ToString("00");
    textTimerArray[2].text = timeValue.minut.ToString("00");
    textTimerArray[4].text = timeValue.second.ToString("00");
  }

  public void OnMouseDown() {
    isClick = true;
  }

  public void OnMouseUp() {
    if (!isClick) return;
    AlterQuest.Instance.DialogTap();
  }

  public void OnMouseExit() {
    isClick = false;
  }

}
