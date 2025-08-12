using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Точка квеста
/// </summary>
public class MapPointQuest : MonoBehaviour {

  public Text nameText;
  public Text countText;
  public RectTransform line;

  Animator animComponent;

  void OnEnable() {
    animComponent = GetComponent<Animator>();
  }

  /// <summary>
  /// Установка значения квеста
  /// </summary>
  /// <param name="isActive">Флаг активности</param>
  /// <param name="newName">Текст квеста</param>
  /// <param name="newCount">Счетчик квеста</param>
  public void SetValue(bool isActive, Questions.Question oneQuest, string newCount = null) {

    if(oneQuest.descr != null) {
      //nameText.text = newName;

      nameText.text = LanguageManager.GetTranslate("quest_" + oneQuest.type.ToString()) + (!oneQuest.cumulative ? " " + LanguageManager.GetTranslate("quest_inOneRun") : "");  //quest.actionList.items[questNum].descr;

      while(nameText.preferredWidth > 500) nameText.fontSize--;
    }
    if(newCount != null) countText.text = newCount;

    if(isActive)
      SetActive();
    else
      SetDisactive();

  }

  void SetActive() {
    animComponent.SetTrigger("active");
  }

  public void SetDisactive() {
    animComponent.SetTrigger("disactive");
    line.sizeDelta = new Vector2(nameText.preferredWidth + 10, line.sizeDelta.y);
  }

  public void Close() {
    animComponent.SetTrigger("close");
  }

}
