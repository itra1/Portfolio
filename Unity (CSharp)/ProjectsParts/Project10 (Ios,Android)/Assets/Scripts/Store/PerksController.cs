using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Класс по управлению Модификаторами (Perks)
/// </summary>
[System.Serializable]
public struct PerksItems {
  public GameObject[] Perks; // Набор элементов
  public GameObject PerksButton; // Набор элементов
  public StoreItemType type;
  [HideInInspector] public int inits;  // Номер активного пакета
  [HideInInspector] public bool activ;  // Флаг активности
}
public class PerksController: MonoBehaviour {

  public PerksItems[] PerkGroups; // Открытая группа, используется что бы получить ссылки на объекты
  private PerksItems[] thisPerkGroups; // Закрытая группа группа, с неею происходят операции
  private StoreController store;

  void Start() {
    store = GameObject.Find("Store").GetComponent<StoreController>();
    UpdateThisGroups();
  }

  // Раскрывающийся список
  public void ShowGroup(int GroupNum) {
    CloseOthersGroup(GroupNum); // Закрываем все остальные группы

    if (thisPerkGroups[GroupNum].activ == false) {
      Vector2 inits = thisPerkGroups[GroupNum].Perks[0].transform.position;
      for (int i = thisPerkGroups[GroupNum].inits; i < thisPerkGroups[GroupNum].Perks.Length; i++){ // Начинаем со второй

        thisPerkGroups[GroupNum].Perks[i].transform.position = new Vector2(inits.x
                                                                         , inits.y + (inits.y + 5f) * (i - thisPerkGroups[GroupNum].inits));
        thisPerkGroups[GroupNum].Perks[i].SetActive(true);
      }
      thisPerkGroups[GroupNum].activ = true;
    } else {
      CloseOthersGroup();
    }
  }

  // Сокрытие всех списков, за исключением переданной
  private void CloseOthersGroup(int GroupNum = -1) // Зaкрывает все, кроме открытой(переданой) группы
  {

    for (int j = 0; j < thisPerkGroups.Length; j++) {
      if (j != GroupNum /*&& thisPerkGroups[j].activ == true */){ // Остальные не закрытые
      
        Vector2 inits = thisPerkGroups[j].Perks[0].transform.position;
        for (int i = 0; i < thisPerkGroups[j].Perks.Length; i++){ // Начинаем со второй

          thisPerkGroups[j].Perks[i].transform.position = inits;
          if (i != thisPerkGroups[j].inits)
            thisPerkGroups[j].Perks[i].SetActive(false);
        }
        thisPerkGroups[j].activ = false;
      }
    }
  }

  /// <summary>
  /// Открытая группа, использвется для внешнего доступа
  /// </summary>
  public void UpdateGroups(){
    UpdateThisGroups();
  }

  private void UpdateThisGroups() {
    StoreItems[] storeItem = GameObject.Find("Store").GetComponent<StoreController>().storeItem;
    thisPerkGroups = PerkGroups;

    thisPerkGroups[0].inits = PlayerPrefs.GetInt("armorBoxex");
    thisPerkGroups[1].inits = PlayerPrefs.GetInt("magnet");
    thisPerkGroups[2].inits = PlayerPrefs.GetInt("saves");
    thisPerkGroups[3].inits = PlayerPrefs.GetInt("weapon");
    thisPerkGroups[4].inits = PlayerPrefs.GetInt("access");
    thisPerkGroups[5].inits = PlayerPrefs.GetInt("jump");

    for (int j = 0; j < thisPerkGroups.Length; j++) {
      thisPerkGroups[j].activ = false; // сворачиваем группы
      thisPerkGroups[j].Perks[thisPerkGroups[j].inits].SetActive(true); //отображаем только 1 картинку

      for (int i = 0; i < thisPerkGroups[j].Perks.Length; i++) {
        if (i > thisPerkGroups[j].inits) {
          thisPerkGroups[j].Perks[i].GetComponent<Button>().interactable = false;
          for (int ii = 0; ii < storeItem.Length; ii++) {
            if (storeItem[ii].group == thisPerkGroups[j].type //Если одного типа
             && storeItem[ii].level == i               // на один уровень больше
             && storeItem[ii].level == PlayerPrefs.GetInt(storeItem[ii].group.ToString()) + 1 //Уровень на 1 больше
             && storeItem[ii].price <= UserManager.coins
                )
              thisPerkGroups[j].Perks[i].GetComponent<Button>().interactable = true;
          }
        }
      }
    }

    CloseOthersGroup();
  }

  // Купить элемент, по идентификатору покупки
  public void ByeItem(int id) {
    store.DialogBuy(id);
  }
}
