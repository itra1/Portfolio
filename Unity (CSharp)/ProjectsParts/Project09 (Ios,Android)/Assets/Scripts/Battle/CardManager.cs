using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Типы карт
/// </summary>
public enum CardTypes {
  pistol,
  machinegun,
  carabine,
  poisonArrow,
  bazooka,
  iceGrenade,
  grenade,
  hiddenPlayer,
  speedyShoot,
  barrier,
  mines,
  none
}
public enum CardCategory {
  weaponShooters,              // Стрелковое оружие (пистолеты и автоматы)
  player,
  specialMagic
}
/// <summary>
/// Информация о карте
/// </summary>
[System.Serializable]
public struct CardInfo {
  public string name;                                     // Название
  public CardTypes type;                                  // Тип карты
  public CardCategory category;                           // Категория карты
  public Sprite sprite;                                   // Спрайт карты
}

/// <summary>
/// параметры оружия
/// </summary>
[System.Serializable]
public struct WeaponParams {
  public CardTypes card;
  public WeaponType weapon;
}

/// <summary>
/// Генератор карт
/// </summary>
public class CardManager : MonoBehaviour {

  public static CardManager instance;                     // Ссылка на созданный экземпляр

  List<CardInfo> readyCards = new List<CardInfo>();
  public static event Actione<List<CardInfo>> OnCreateCard;

  CardsQueueData cardsData;

  bool isSendRequest;

  void Awake() {
    instance = this;
  }

  private void Start() {
    User.OnCardsQueue += OnCardsQueue;
    CardUsed.OnUseCars += OnUseCars;
    OnCardsQueue(User.instance.cardsQueueData);
  }

  private void OnDestroy() {
    User.OnCardsQueue -= OnCardsQueue;
    CardUsed.OnUseCars -= OnUseCars;
  }

  void OnCardsQueue(CardsQueueData cardsData) {
    this.cardsData = cardsData;
    GenerateNewList();
  }

  public void GenerateNewList() {
    readyCards.Clear();
    for(int i = 0; i < cardsData.maxActiveCard; i++) {
      readyCards.Add( GameDesign.instance.cardLibrary.Find(x => x.type.ToString() == cardsData.cardCode[i]) );
    }
    if(OnCreateCard != null) OnCreateCard(readyCards);
  }

  public void UseCardTry(CardInfo cardInfo) {

    if(BattleScene.instance == null || BattleScene.instance.battlePhase != BattlePhase.battle) return;

    if(isSendRequest) return;
    isSendRequest = true;
    Generals.Network.NetworkManager.SendPacket(new RequestUseCard(cardInfo.type.ToString()));
  }

  public CardInfo GetCardInfoByCode(string code) {
    CardTypes cardType = (CardTypes)Enum.Parse(typeof(CardTypes),code);
    return GetCardInfoByType(cardType);
  }

  /// <summary>
  /// Запрос на получения данных по карте
  /// </summary>
  /// <param name="cardType">Тип карты</param>
  /// <returns>Структура параметров</returns>
  public CardInfo GetCardInfoByType(CardTypes cardType) {
    return GameDesign.instance.cardLibrary.Find(x => x.type == cardType);
  }
  
  /// <summary>
  /// Применение карты
  /// </summary>
  /// <param name="cardType"></param>
  public void UseCard(Player player, CardInfo cardType) {
    
    CardInfo cardInfo = GetCardInfoByType(cardType.type);
    
    if(player.isFirst) {
      readyCards.RemoveAll(x => x.type == cardType.type);
      //GetCard();
    }
    
    // Карты оружия
    if(cardInfo.category == CardCategory.weaponShooters) {
      player.SetNewWeapon(GameDesign.instance.weaponCardParam.Find(x => x.card == cardType.type).weapon);
    }

    // Карты модификации плеера
    if(cardInfo.category == CardCategory.player) {
      if(cardInfo.type == CardTypes.hiddenPlayer) player.SetHidden();
      if(cardInfo.type == CardTypes.speedyShoot) player.SetSpeedyShooter();
    }
    
  }

  void OnUseCars(CardUseData cardUse) {

    isSendRequest = false;
    if(!cardUse.success) return;

    Player player = PlayersManager.instance.GetPlayerByUserID(cardUse.playerId);
    CardInfo cardInfo = GetCardInfoByCode(cardUse.code);
    UseCard(player, cardInfo);
  }
  
}
