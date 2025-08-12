using UnityEngine;
using System.Collections.Generic;

public struct CardsQueueData {
  public int maxActiveCard;
  public List<string> cardCode;
}

/// <summary>
/// Очередь карт
/// </summary>
public class CardsQueue : Packet {
  
  CardsQueueData cardQueue;

  public override void ReadImpl() {
    cardQueue = new CardsQueueData();
    cardQueue.cardCode = new List<string>();
    cardQueue.maxActiveCard = ReadC();

    int countArr = ReadC();
    
    for(int i = 0; i < countArr; i++) {
      cardQueue.cardCode.Add(ReadASCII());
    }

  }

  public override void Process() {
    User.instance.AddCardsQueue(cardQueue);
  }

}
