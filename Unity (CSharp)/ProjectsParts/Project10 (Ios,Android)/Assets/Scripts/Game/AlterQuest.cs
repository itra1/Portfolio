using UnityEngine;
using MiniJSON;
using System.Collections.Generic;

/// <summary>
/// Работа альтернативного квеста
/// </summary>
public class AlterQuest : Singleton<AlterQuest> {
	
  public delegate void TimeChenge(TimeStruct timeValues);
  public delegate void QuestReady(bool isActiveQuest);

  int unixTime;                                         // Время юникса

  int activeQuest;                                      // Активный квест
  int timeEndQuest;                                     // Время окончания квеста
  int timeNextQuest;                                    // Время до появления нового квеста

  public bool isActiveQuest;                            // Активность и готовность квеста
  public static event TimeChenge OnTimeChange;          // Событие изменения таймера активного квеста
  public static event QuestReady OnQuestActive;         // Событие изменения статуса активности квеста

  TimeStruct timeWaitEndQuest;                          // Время ко конца работы квеста
	
  void Start() {
    LoadData();
  }

  void Update() {

    // Рассчитываем оставшееся время
    if(isActiveQuest) CalcTimeQuest();
  }

  /// <summary>
  /// Парсим сохраненные данные
  /// </summary>
  public void LoadData() {
    string alterQuestData = PlayerPrefs.GetString("alterQuest", "{}");
    
    if(alterQuestData == "{}" || alterQuestData == "0") {
      activeQuest = -1;
      timeEndQuest = 0;
      timeNextQuest = -1;
      IncrementQuest();
      return;
    }
    var dataQuest = Json.Deserialize(alterQuestData);

    activeQuest = int.Parse(((Dictionary<string, object>)dataQuest)["activeQuest"].ToString());
    timeEndQuest = int.Parse(( (Dictionary<string, object>)dataQuest )["timeEndQuest"].ToString());
    timeNextQuest = int.Parse(( (Dictionary<string, object>)dataQuest )["timeNextQuest"].ToString());

    if(timeNextQuest >= 0 ) {
      if(timeNextQuest < Util.unixTime) {
        timeNextQuest = -1;
        IncrementQuest();
      }
    }else {
      isActiveQuest = true;
    }

  }

  /// <summary>
  /// Событие закрытия приложения
  /// </summary>
  void OnApplicationQuit() {
    SaveData();
  }

  public void SaveData() {
    Dictionary<string, object> saveData = new Dictionary<string, object>();
    saveData["activeQuest"] = activeQuest;
    saveData["timeEndQuest"] = timeEndQuest;
    saveData["timeNextQuest"] = timeNextQuest;
    PlayerPrefs.SetString("alterQuest", Json.Serialize(saveData));
  }

  /// <summary>
  /// Изменение номер квеста
  /// </summary>
  public void IncrementQuest() {
    activeQuest++;
    timeEndQuest = Util.unixTime + 60 * 60 * 24;
    timeNextQuest = -1;
    isActiveQuest = true;
    if (OnQuestActive != null) OnQuestActive(isActiveQuest);
  }

  /// <summary>
  /// Обработка клика по иконке на карте
  /// </summary>
  public void DialogTap() {

		AlterQuestDialog alterQuestion = UiController.ShowUi<AlterQuestDialog>();
		alterQuestion.gameObject.SetActive(true);
		alterQuestion.SetQuest((Questions.QuestionManager.GetQuestListLevel(activeQuest, true)).items[0]);
		alterQuestion.ForvardClick = DialogForvardClick;
	}

  /// <summary>
  /// Обработка тапа по кнопке Продолжить
  /// </summary>
  void DialogForvardClick() {
    if (Company.Live.LiveCompany.Instance.isReady) {
      GameManager.startFromMap = true;
      //GameManager.activeLevel = levelPoint;
      GameManager.Instance.ChangeLevel(activeQuest,true);
      GameManager.LoadScene("ClassicRun");
    } else {
      //UiController.instance.EnergyPanelShow(true);
    }
  }

  /// <summary>
  /// Обработка изменения времени
  /// </summary>
  void CalcTimeQuest() {
    if (OnTimeChange != null) OnTimeChange(Util.GetTimeDecrement(timeEndQuest));
  }

}
