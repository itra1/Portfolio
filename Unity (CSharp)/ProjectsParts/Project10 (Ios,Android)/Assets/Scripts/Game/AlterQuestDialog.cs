using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

/// <summary>
/// Диалоговое окно окно альтернативного квеста
/// </summary>
public class AlterQuestDialog : PanelUi {

  public Text descriptionText;
  public Text countText;
  public Text maxCountText;

  public Text[] timersText;

  public GameObject buttonPrice;
  public GameObject buttonForvard;

  public delegate void ButtonClick();
  public ButtonClick ForvardClick;

  protected override void OnEnable() {
		base.OnEnable();
    isClose = false;
    AlterQuest.OnQuestActive += OnQuestActive;
    AlterQuest.OnTimeChange += SetTimerValue;
    Company.Live.LiveCompany.OnChange += EnergyChange;
    EnergyChange(Company.Live.LiveCompany.Instance.value);
  }

  public void SetQuest(Questions.Question oneQuest) {
    descriptionText.text = LanguageManager.GetTranslate("quest_" + oneQuest.type.ToString());
    countText.text = Mathf.Round(oneQuest.value).ToString();
    maxCountText.text = oneQuest.needvalue.ToString();
  }

  protected override void OnDisable() {
		base.OnEnable();
    AlterQuest.OnQuestActive -= OnQuestActive;
    AlterQuest.OnTimeChange -= SetTimerValue;
    Company.Live.LiveCompany.OnChange -= EnergyChange;
  }

  void OnQuestActive(bool isActive) {
    if (!isActive) Close();
  }
  
  public void ButtonForvard() {
    if (ForvardClick != null) ForvardClick();
  }

  /// <summary>
  /// Изменение значения таймера
  /// </summary>
  /// <param name="timeValue">Структура времени</param>
  public void SetTimerValue(TimeStruct timeValue) {
    timersText[0].text = timeValue.hour.ToString("00");
    timersText[2].text = timeValue.minut.ToString("00");
    timersText[4].text = timeValue.second.ToString("00");
  }

  bool isClose;       // Флаг закрытия

  public void Close() {
    isClose = true;
    GetComponent<Animator>().SetTrigger("close");
  }

  protected void CloseThis() {
    if (isClose)
      gameObject.SetActive(false);
  }

  public void EnergyChange(float newEnergyValue) {
    if(Company.Live.LiveCompany.Instance.isReady) {
      buttonPrice.SetActive(false);
      buttonForvard.SetActive(true);
    }else {
      buttonPrice.SetActive(true);
      buttonForvard.SetActive(false);
    }
  }

	public override void BackButton() {
		Close();
	}
}
