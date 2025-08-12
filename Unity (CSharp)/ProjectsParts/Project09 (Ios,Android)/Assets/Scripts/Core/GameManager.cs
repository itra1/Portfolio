using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Generals.Network;

/// <summary>
/// Общий контроллер игрового процесса
/// </summary>
public class GameManager : MonoBehaviour {

  public static GameManager instance;                 // Ссылка на собственный экземпляр
  public bool isDebud;                                // Включенный режим отладки

  /// <summary>
  /// Первоначальная инициализация
  /// </summary>
  void Awake() {

    // Проверка на дублирование контроллера
    if(instance != null) {
      Destroy(gameObject);
      return;
    }

    instance = this;
    status = AuthStatus.RESULT_ACCESS_TOKEN_EMPTY;
    SceneManager.activeSceneChanged += OnActiveSceneChange;
    AuthLoginResult.OnAuthLogin += OnAuthLoginSockets;
    AuthLoginResult.OnAuthLogin += OnReadyBattle;
    Generals.Network.Http.HttpApi.OnLoginSucceeded += SocialAuthorizationComplited;

    // Запрет на уничтожение экземпляра
    DontDestroyOnLoad(gameObject);
  }

  [HideInInspector]
  public bool networkConnect;         // Разрешение на выполнение сетевого соединения

  void Start() { }

  void OnDestroy() {
    SceneManager.activeSceneChanged -= OnActiveSceneChange;
    AuthLoginResult.OnAuthLogin -= OnAuthLoginSockets;
    AuthLoginResult.OnAuthLogin -= OnReadyBattle;
    Generals.Network.Http.HttpApi.OnLoginSucceeded -= SocialAuthorizationComplited;
  }

  void SocialAuthorizationComplited() {
    NetworkManager.instance.apiService.GetProfile(User.instance.GetGameProfile);
    NetworkManager.instance.apiService.GetCardsCatalog(GameDesign.instance.CardParsing);
    NetworkManager.instance.apiService.GetChatacter(User.instance.GetAllCheracters);
  }

	[HideInInspector]
  public AuthStatus status;

  /// <summary>
  /// авторизация на сервере сокетов
  /// </summary>
  /// <param name="obj"></param>
  private void OnAuthLoginSockets(AuthStatus status) {
    this.status = status;
  }

  Scene activeAcene;

  void OnActiveSceneChange(Scene from, Scene to) {
    activeAcene = to;
  }

  #region Бой

  public bool battleReady;                                              // Готовность к бою
	[HideInInspector]
  public int activeTeam;                                                // Активная команды
  public List<PlayerJoined> playerJoined = new List<PlayerJoined>();    // Список подключенных к бою играков
  public BattleData battle;                                             // Данные боя
  public BattleUsers[] battleUsers;                                     // Игроки боя

  public void AddPlayers(PlayerJoined newPlayer) {
    playerJoined.Add(newPlayer);
    if(newPlayer.messageId == User.instance.gameProfile.id)
      activeTeam = newPlayer.team;
  }

  [System.Serializable]
  public struct BattleData {
    public string id;
    public string battle_log_id;
    public ulong created_at;
  }

  [System.Serializable]
  public struct BattleUsers {
    public UserBattle user;
    public CharacterBattle character;

    [System.Serializable]
    public struct UserBattle {
      public string id;
      public int active;
      public string userName;
      public string token;
    }

    [System.Serializable]
    public struct CharacterBattle {
      public string id;
      public string game_profile_id;
      public string name;
      public string description;
    }
  }

  public int PlayerNumLeft() {
    if(battleUsers.Length == 0) return 0;
    for(int i = 0; i < battleUsers.Length; i++) {
      if(battleUsers[i].user.id == User.instance.user_id)
        return i;
    }
    return 0;
  }

  private bool startWishBot = false;
  private bool battleConnect = false;
  private ServerType serverType;
  private SocketType socketType;

  /// <summary>
  /// Запуск игры с ботом
  /// </summary>
  public void StartBattleBot(ServerType serverType, SocketType socketType) {
    startWishBot = true;
    StartBattle(serverType, socketType);
  }

  public void StartBattlePlayer(ServerType serverType, SocketType socketType) {
    startWishBot = false;
    StartBattle(serverType, socketType);
  }

  /// <summary>
  /// Запуск игры с играком
  /// </summary>
  public void StartBattle(ServerType serverType, SocketType socketType) {
    this.serverType = serverType;
    this.socketType = socketType;
    battleConnect = true;
		if(NetworkManager.networkState == NetState.ServerConnected) {
			OnReadyBattle(this.status);
		} else {
			NetworkManager.instance.Connect(serverType, socketType);
		}
  }

  /// <summary>
  /// Запуск процесса подключения
  /// </summary>
  public void OnReadyBattle(AuthStatus authStatus) {
    
    this.status = authStatus;

    if(authStatus != AuthStatus.RESULT_OK) {
      
      Debug.LogError("Ошибка сокет авторизации: " + authStatus);

      if(authStatus == AuthStatus.RESULT_ACCESS_TOKEN_INVALID)
        Generals.Network.Http.HttpApi.instance.RefreshToken(() => {
          NetworkManager.instance.Connect(serverType, socketType);
        });

      return;
    }
    if(!battleConnect) return;

    if(string.IsNullOrEmpty(User.instance.accountId)) {
      UI.instance.InfoPanelShow("Необходимо авторизироваться", InfoPanel.ButtonGroup.ok);
      return;
    }

    if(User.instance.gameProfile.id == null || User.instance.gameProfile.id == "") {
      UI.instance.InfoPanelShow("Необходимо создать игровой профиль", InfoPanel.ButtonGroup.ok);
      return;
    }

    if(User.instance.characters.Count <= 0) {
      UI.instance.InfoPanelShow("Необходимо создать персонажа", InfoPanel.ButtonGroup.ok);
      return;
    }

    playerJoined.Clear();
    Generals.Network.NetworkManager.SendPacket(new RequestBattleStart(startWishBot));
  }

  /// <summary>
  /// Постановка в очередь
  /// </summary>
  public void PlayerAddInQueue() {
    if(battleReady) return;
    battleReady = true;
    UI.instance.InfoPanelShow("Ожидание игроков", InfoPanel.ButtonGroup.cancel, null, BattleReadyCancel, null);
  }

  /// <summary>
  /// Подключились
  /// </summary>
  public void BattleJoinStart(string battleid, int team) {
    if(!battleReady) return;
    battle.id = battleid;
    battle.created_at = Util.unixTimeUnvMilliseconds;
    generateList = new List<ObjectSpawn>();
  }

  /// <summary>
  /// Закончили подключаться
  /// </summary>
  public void BattleJoinEnd() {
    if(!battleReady) return;
    battleReady = false;
    SceneManager.LoadScene("Battle");
  }

  void BattleReadyCancel(GameObject panel) {
    WWWForm form = new WWWForm();
    form.AddField("character_id", User.instance.characters[0].id);
    //Network.instance.BattleCancel(form, BattleReadyCanceled);
  }
  void BattleReadyCanceled(string answer) { }

  #endregion

  #region Объекты сцены

  public List<ObjectSpawn> generateList = new List<ObjectSpawn>();              // Обьекты, генерируемые в бою

  public void BattleAddSpawnObject(List<ObjectSpawn> newObject) {
    if(activeAcene.name == "Battle") {
      MapManager.instance.GenerateSceneObjects(newObject);
      return;
    }

    foreach(ObjectSpawn one in newObject)
      generateList.Add(one);
  }

  #endregion

}
