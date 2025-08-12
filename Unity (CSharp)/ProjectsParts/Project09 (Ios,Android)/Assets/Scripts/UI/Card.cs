using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Card : MonoBehaviour {
  
  public CardInfo cardInfo;                        // Информация по карте
  public Actione<CardInfo> TapCard;   // Событие тапа по карте;
  public Text energyText;                   // Интерфейсный текст
  public Image imageCart;                   // Компонент фонового рисунка

  /// <summary>
  /// Инициализация
  /// </summary>
  /// <param name="newCardInfo">Информация о карте из библиотеки</param>
  public void Init(CardInfo newCardInfo) {
    cardInfo = newCardInfo;
    TapCard = null;
    energyText.text = GameDesign.instance.cardList.Find(x=>x.code == newCardInfo.type.ToString()).energyCost.ToString();
    imageCart.sprite = newCardInfo.sprite;
  }
  
  /// <summary>
  /// Клик по карте
  /// </summary>
  public void Click() {

    // Событие тапа по карте
    if (TapCard != null) TapCard(cardInfo);

  }
}
