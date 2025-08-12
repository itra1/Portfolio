using UnityEngine;
using System.Collections;

/// <summary>
/// Использование карты
/// </summary>
public struct CardUseData {
  public string playerId;
  public string code;
  public int cardLevel;
  public bool success;
}

/// <summary>
/// Использование пакета
/// </summary>
public class CardUsed : Packet {

  public static event Actione<CardUseData> OnUseCars;

  CardUseData cardData;

  public override void ReadImpl() {

    cardData = new CardUseData();

    cardData.playerId = ReadASCII();
    cardData.code = ReadASCII();
    cardData.success = (ReadC() == 1 ? true : false);
    if(!cardData.success) return;
    cardData.cardLevel = ReadC();
  }

  public override void Process() {
    if(OnUseCars != null) OnUseCars(cardData);
  }

}
