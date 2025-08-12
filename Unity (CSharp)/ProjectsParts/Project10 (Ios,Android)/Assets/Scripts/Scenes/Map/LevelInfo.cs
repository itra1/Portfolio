using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Random = UnityEngine.Random;

/// <summary>
/// Точка карты
/// </summary>
public class LevelInfo : PanelUiBase {
	
	public Action OnClose;
	public Action OnForward;

  public Text title;                        // Заголовок
  public Text energyCount;                  // Количество энергии на уровень
  public float energyValue;                 // Запас энергии
  private bool isClose;                     // Закрытие
  private Animator animComponent;           // Компонент аудио
  public GameObject questDataPrefab;        // Префаб одного квеста
  public List<GameObject> questions = new List<GameObject>();        // Массив квестов
  public List<object> questList;            // Массив квестов
  private int levelPoint;                   // Активный уровень

  public GameObject forwardButton;
  public GameObject lockedButton;
  public GameObject priceButton;

  void OnEnable() {
    isClose = false;
    animComponent = GetComponent<Animator>();
    Company.Live.LiveCompany.OnChange += EnergyChange;
  }

  void OnDisable() {
    isClose = false;
    Company.Live.LiveCompany.OnChange -= EnergyChange;

		if (OnClose != null)	OnClose();

	}


  /// <summary>
  /// Установка значения уровня
  /// </summary>
  /// <param name="level"></param>
  public void SetLevel(int level) {
    questions.Clear();
    levelPoint = level;
    energyValue = Company.Live.LiveCompany.Instance.value;

    CheckEnergy();

    if(GameManager.activeLevelData.gameMode == GameMode.survival) {
      title.text = LanguageManager.GetTranslate("lq_survival");
    }else {
      title.text = LanguageManager.GetTranslate("lq_level") + " " + (level + 1);
    }
    
    if(((GameManager.activeLevelData.gameMode & (GameMode.levelsClassic | GameMode.levelsConstructor)) != 0 ) && GameManager.level >= level) {

			Questions.LevelList questList = Questions.QuestionManager.GetQuestListLevel(level);
      
      for(int i = 0; i < questList.items.Length; i++) {
        GameObject questInstance = Instantiate(questDataPrefab);
        questInstance.GetComponent<RectTransform>().SetParent(questDataPrefab.transform.parent);
        questInstance.transform.localPosition = questDataPrefab.transform.localPosition;
        RectTransform prefab = questDataPrefab.GetComponent<RectTransform>();
        questInstance.GetComponent<RectTransform>().anchoredPosition = new Vector2(prefab.anchoredPosition.x + Random.Range(-7, 7), prefab.anchoredPosition.y - 105 * i);
        questInstance.SetActive(true);
        if(levelPoint < GameManager.level) {
          questInstance.GetComponent<MapPointQuest>().SetValue(false, questList.items[i], questList.items[i].needvalue.ToString() + "/" + questList.items[i].needvalue.ToString());
        } else {
          if(questList.items[i].needvalue <= questList.items[i].value)
            questInstance.GetComponent<MapPointQuest>().SetValue(false, questList.items[i], questList.items[i].needvalue.ToString() + "/" + questList.items[i].needvalue.ToString());
          else
            questInstance.GetComponent<MapPointQuest>().SetValue(true, questList.items[i], (questList.items[i].cumulative ? Mathf.Round(questList.items[i].value).ToString() : "0") + "/" + questList.items[i].needvalue.ToString());
        }
        questions.Add(questInstance);
      }
    }
    
  }

  /// <summary>
  /// Закрытие
  /// </summary>
  public void Close() {
    isClose = true;
    animComponent.SetTrigger("close");
    if(questions != null) questions.ForEach(x=> x.GetComponent<MapPointQuest>().Close());
  }

  /// <summary>
  /// Закрытие
  /// </summary>
  public void OnCloseEvent() {
    if(!isClose) return;
    gameObject.SetActive(false);
    if(questions != null) questions.ForEach(x => Destroy(x));
  }

  /// <summary>
  /// Кнопка
  /// </summary>
  public void ButtonForward() {

		if (OnForward != null)
			OnForward();
		
		//if (levelPoint <= GameManager.level || GameManager.instance.isDebug) {
  //    energyValue = PlayerManager.instance.energy.energy;
  //    if(energyValue != -1 && energyValue < PlayerManager.instance.energy._energyOne) {
  //      UiController.instance.EnergyPanelShow(true);
  //    } else {
  //      Close();
  //    }
  //  }
  }

  /// <summary>
  /// Событие изменения значения энергии
  /// </summary>
  /// <param name="energyValue"></param>
  public void EnergyChange(float newEnergyValue) {
    energyValue = newEnergyValue;
    CheckEnergy();
  }

  void CheckEnergy() {

    /// Обработка при дебаге
    if(GameManager.Instance.isDebug) {
      lockedButton.SetActive(false);
      if(Company.Live.LiveCompany.Instance.isReady) {
        priceButton.SetActive(false);
        forwardButton.SetActive(true);
      }else {
        priceButton.SetActive(true);
        forwardButton.SetActive(false);
      }
      return;
    }

    if(levelPoint > GameManager.level) {
      forwardButton.SetActive(false);
      lockedButton.SetActive(true);
      priceButton.SetActive(false);
      return;
    }

    lockedButton.SetActive(false);

    if(energyValue == -1 || Company.Live.LiveCompany.Instance.isReady) {
      forwardButton.SetActive(true);
      priceButton.SetActive(false);
    }else {
      forwardButton.SetActive(false);
      priceButton.SetActive(true);
    }
  }
	
	
}
