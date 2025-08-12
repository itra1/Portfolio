using UnityEngine;
using UnityEngine.UI;
using MiniJSON;
using System.Collections.Generic;
using Generals.Network;
using Generals.Network.Http;

/// <summary>
/// Управление сценой навигации
/// </summary>
public class Navigation : MonoBehaviour {

  void Start() {
    SocialFb.OnAuthChange += SocialAuthorizationComplited;
    ShowMainMenu();
  }

  void Update() {
    CheckConnectParam();
  }
  
  void OnDestroy() {
    SocialFb.OnAuthChange -= SocialAuthorizationComplited;
  }

  public Dropdown serverChange;
  public Dropdown socketChange;
  
  public void CheckConnectParam() {
    if(NetworkManager.instance == null) return;
    serverChange.interactable = !NetworkManager.instance.isConnect;
    socketChange.interactable = !NetworkManager.instance.isConnect;
  }

  #region Основное меню
  GameObject menu;
  void ShowMainMenu() {
    menu = UI.instance.MenuPanelShow(true);
    menu.GetComponent<Menu>().OnAuthorization = OnAuthorization;
    menu.GetComponent<Menu>().OnUser = OnUser;
    menu.GetComponent<Menu>().OnProfiles = OnProfiles;
    menu.GetComponent<Menu>().OnCharacter = OnCharacter;
    menu.GetComponent<Menu>().OnBattleBot = OnBattleBot;
    menu.GetComponent<Menu>().OnBattlePlayer = OnBattlePlayer;
  }

  /// <summary>
  /// Авторизация
  /// </summary>
  void OnAuthorization() {
    SocialFb.FBlogin();
  }

  void OnUser() {
    //Network.instance.UserByIdGet(UserAnswer);
  }
  void OnProfiles() {
    //Network.instance.ProfileGet(UserAnswer);
    GameProfilePanekShow();
  }

  void OnCharacter() {
    CharacterPanelShow();
  }
  
  void OnBattleBot() {
    GameManager.instance.StartBattleBot((ServerType)serverChange.value, (SocketType)socketChange.value);
  }
  void OnBattlePlayer() {
    GameManager.instance.StartBattlePlayer((ServerType)serverChange.value, (SocketType)socketChange.value);
  }

  void BattleReadyCanceled(string answer) { }

  void OnBattleComplited(string answer) { }

  void UserAnswer(string data) {
    //Debug.Log(data);
  }

  #endregion

  #region Пользователи
  GameObject authInfoPanel;         // Диалоговое окно авторизации

  /// <summary>
  /// Успешная социальная авторизация
  /// </summary>
  void SocialAuthorizationComplited() {

    menu.GetComponent<Menu>().FacebookBtnActive(false);
    NetworkManager.instance.AuthorizationHTTP(SocialFb.FBToken, SocialNetwork.FB);
  }

  /// <summary>
  /// Ввод имени
  /// </summary>
  /// <param name="name"></param>
  void OnEnterName(string name) {
    PlayerPrefs.SetString("AuthName", name);
  }
  
  /// <summary>
  /// Успошное получение данных
  /// </summary>
  void AuthorizationComplited() {
    if(authInfoPanel != null) authInfoPanel.GetComponent<InfoPanel>().Close();
    Debug.Log(string.Format("Успешная авторизация {0}", User.instance.accountId));
    Generals.Network.NetworkManager.instance.Connect((ServerType)serverChange.value, (SocketType)socketChange.value);
  }
  #endregion

  #region Гейм профиль

  public void GameProfilePanekShow() {
    GameObject panel = UI.instance.GameProfileCreatePanelShow(true);
    panel.GetComponent<GameProfileCreatePanel>().OnButton = GameProfileOnUpdate;
  }

  void GameProfileOnUpdate(GameProfile gameProfile) {

    WWWForm form = new WWWForm();
    form.AddField("name", gameProfile.name);
    form.AddField("rating", gameProfile.rating);
    form.AddField("gold", gameProfile.gold);
    form.AddField("crystals", gameProfile.crystals);
    form.AddField("experience", gameProfile.experience);
    form.AddField("clan_id", gameProfile.clan_id);

    //if(gameProfile.id != null && gameProfile.id != "")
    //  Network.instance.ProfileUpdate(form, GameProfileUpdateAnswer);
    //else
    //  Network.instance.ProfileCreate(form, GameProfileUpdateAnswer);
  }
  
  #endregion

  #region Персонаж

  void CharacterPanelShow() {
    GameObject inst = UI.instance.CharacterCreatePanelShow(true);
    inst.GetComponent<CharacterPanel>().OnButton = CharacterPanelAnswer;
  }

  void CharacterPanelAnswer(Character character) {
    
    WWWForm form = NetworkManager.CreateForm();
    form.AddField("name", character.name);

    NetworkManager.instance.apiService.CreateChatacter(form, (gameCharacter) => {
      Debug.Log("id: " + gameCharacter.id);
      Debug.Log("name: " + gameCharacter.name);
      Debug.Log("ownerId: " + gameCharacter.ownerId);
      User.instance.CharacterPrsing(gameCharacter);

    });
    
  }
  
  #endregion
  
}
