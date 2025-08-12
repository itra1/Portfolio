using UnityEngine;
using MiniJSON;
using System.Collections.Generic;

[System.Serializable]
public struct GameProfile {
  public string id;
  public string name;
  public bool isUpdated;
  public int rating;
  public int gold;
  public int crystals;
  public int experience;
  public string clan_id;
}

[System.Serializable]
public struct Character {
  public string id;
  public string name;
  public string ownerId;
}


/// <summary>
/// Данные пользователя
/// </summary>
public class User : MonoBehaviour {

  public static User instance;                                          // Ссылка на собственный экземпляр
  public static Actione OnFillUserData;

  public GameProfile gameProfile;                                       // Игровой профиль
  public List<Character> characters;                                    // Персонажи
  
  private string _user_id;                                              // id пользователя
  public string user_id { get { return _user_id; } set { _user_id = value; } }
  private string _accountId;
  public string accountId { get { return _accountId; } }
  private string _profileId;
  public string profileId { get { return _profileId; } }

	[HideInInspector]
  public string _avatar_fb;                                             // Аватар фейсбука
	[HideInInspector]
  public string _country;                                               // Язык
	[HideInInspector]
  public string _city;                                                  // Город
  
  void Awake() {

    if(instance != null) {
      Destroy(this);
      return;
    }
    instance = this;
  }

  void Start() { }

  void OnDestroy() { }
  
  void OnApplicationExit() { }

  void LoadData() { }

  void SaveData() { }

  public void OnAuthComplited(Generals.Network.Http.HttpApi.AuthMessage message) {

    this._accountId = message.accountId;
    this._profileId = message.profileId;
    
    if(OnFillUserData != null) OnFillUserData();
  }

  public void GetGameProfile(GameProfile gameProfile) {
    this.gameProfile = gameProfile;
  }

  public void GetAllCheracters(List<Character> charact) {
    characters.Clear();
    characters = charact;
  }

  public void CharacterPrsing(Character charact) {
    characters.RemoveAll(x => x.id == charact.id);
    characters.Add(charact);
  }

  #region Карты в бою

  public static event Actione<CardsQueueData> OnCardsQueue;

  public CardsQueueData cardsQueueData;

  public void AddCardsQueue(CardsQueueData cardsQueueData) {
    this.cardsQueueData = cardsQueueData;
    if(OnCardsQueue != null) OnCardsQueue(this.cardsQueueData);
  }


#endregion

}