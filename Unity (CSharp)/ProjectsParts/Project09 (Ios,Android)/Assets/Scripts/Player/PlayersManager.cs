using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Управляет состоянием играков
/// </summary>
public class PlayersManager : MonoBehaviour {

  public static PlayersManager instance;                    // Ссылка на созданный экземпляр 

  public static event Actione<Player> PlayerCreated;        // Событие создания персонажа

  public float healthPlayer;                                // Начальное значение жизней игрока
  public float energyPlayer;                                // Начальое значение энергии играков
  public float[] starsParam;                                // Параметры расположения звезд на полосе жизней
  public GameObject playerPrefab;
  private List<KeyValuePair<int,Player>> playerInScene = new List<KeyValuePair<int, Player>>();
  public static event Actione<bool> OnPlayerDead;               // Событие сметри игрока
  private Player firstPlayer;
  private void Awake() {
    playerInScene.Clear();
  }

  public void Start() {
    instance = this;
    Inputs.OnVectorMove += OnMoveVector;
    Inputs.OnPointerRightScreenUp += OnPointerUp;
    Inputs.OnPointerRightScreenDown += OnPointerDown;
    HealthUpdate.OnHealth += OnHealth;
    EnergyUpdate.OnEnergy += OnEnergy;
    MoveObjects.OnMove += OnMoveChange;
  }

  private void OnDestroy() {
    Inputs.OnVectorMove -= OnMoveVector;
    Inputs.OnPointerRightScreenUp -= OnPointerUp;
    Inputs.OnPointerRightScreenDown -= OnPointerDown;
    HealthUpdate.OnHealth -= OnHealth;
    EnergyUpdate.OnEnergy -= OnEnergy;
    MoveObjects.OnMove -= OnMoveChange;
  }

  private void Update() {
    SendMoveVector();
  }

  /// <summary>
  /// Игрок по идентификатору
  /// </summary>
  /// <param name="playerId"></param>
  /// <returns></returns>
  public Player GetPlayerByUserID(string playerId) {
    try {
      Player pl = playerInScene.Find(x => x.Value.userId == playerId).Value;
      return pl;
    } catch {
      return null;
    }
  }
  /// <summary>
  /// Игрок по id сцены
  /// </summary>
  /// <param name="sceneId"></param>
  /// <returns></returns>
  public Player GetPlayerBySceneId(int sceneId) {
    try {
      Player pl = playerInScene.Find(x => x.Value.sceneId == sceneId).Value;
      return pl;
    } catch {
      return null;
    }
  }
  /// <summary>
  /// Событие смещение плеера
  /// </summary>
  /// <param name="moveObject">Входящий пакет</param>
  protected void OnMoveChange(MoveObjecsElem moveObject) {
    Player pl = GetPlayerBySceneId(moveObject.id);
    if(pl != null) pl.OnMoveChange(moveObject);
  }
  /// <summary>
  /// Изменение значения жизней с сервера
  /// </summary>
  /// <param name="healthData">Пакет данны о изменении жизей</param>
  void OnHealth(HealthUpdate.HealthData healthData) {
    Player player = GetPlayerBySceneId(healthData.sceneId);
    if(player != null) player.SetDamage(healthData);
  }
  /// <summary>
  /// Изменение значения энергии с сервера
  /// </summary>
  /// <param name="energyData"></param>
  void OnEnergy(EnergyUpdate.EnergyData energyData) {
    Player player = GetPlayerByUserID(energyData.playerId);
    if(player != null) player.EnergyChange(energyData);
  }
  /// <summary>
  /// Совершение выстрела
  /// </summary>
  /// <param name="bulletData">Данные по снаряду</param>
  public void OnPlayerShoot(ObjectSpawn objectData) {
    if(objectData.type != ObjectType.Bullet) return;
    Player player = GetPlayerBySceneId(((ObjectSpawn.BulletData)objectData.data).parent);
    if(player != null) player.OnShoot(objectData);
  }

  /// <summary>
  /// Создание одного персонажа
  /// </summary>
  /// <param name="playerType"></param>
  /// <param name="playerName"></param>
  /// <param name="isBot"></param>
  public GameObject CreatePlayer(bool isFirst, string playerName, bool isBot = false) {

    GameObject playerInst = Instantiate(playerPrefab);
    Player player = playerInst.GetComponent<Player>();

    // Создание экземпляра
    if(isFirst) {
      player.SetScenePosition(PlayerScenePosition.LEFT);
      firstPlayer = player;
    } else {
      player.SetScenePosition(PlayerScenePosition.RIGHT);
    }

    player.isFirst = isFirst;
    player.playerName = playerName;
    player.healthMax = healthPlayer;
    player.energyMax = energyPlayer;
    player.energyRepeat = BattleScene.energyRepeat;
    BattleScene.EnergySpeedRepeatChange += player.ChangeEnegryRepeat;

    // Процесс инициализации персонажа
    player.OnChangePlayer += ChangePlayerState;
    player.Init();
    return playerInst;
  }

  public void PlayerCreateFinish(Player player) {
    playerInScene.Add(new KeyValuePair<int, Player>(player.sceneId, player));
    if(PlayerCreated != null) PlayerCreated(player);
  }

  /// <summary>
  /// Изменение состояние игрока
  /// </summary>
  /// <param name="player"></param>
  public void ChangePlayerState(Player player) {
    if(player.healthValue <= 0) PlayerDead(player.isFirst);
  }
  
  /// <summary>
  /// Смерть игрока
  /// </summary>
  /// <param name="typePlayer">Тип плеера</param>
  void PlayerDead(bool firstPlayer) {
    playerInScene.ForEach(x => x.Value.OnChangePlayer -= ChangePlayerState);
    if(OnPlayerDead != null) OnPlayerDead(firstPlayer);
  }

  #region Входные сигналы управлени
  /// <summary>
  /// Окончание атаки
  /// </summary>
  /// <param name="pointerPosition">Экранные координаты</param>
  void OnPointerUp(Vector3 pointerPosition) {
    firstPlayer.OnPointerUp(pointerPosition);
  }
  /// <summary>
  /// Отпускание клавиши мышы
  /// </summary>
  /// <param name="pointerPosition">Экранные координаты</param>
  void OnPointerDown(Vector3 pointerPosition) {
    firstPlayer.OnPointerDown(pointerPosition);
  }
  #endregion

  #region Джойстик

  Vector2 moveVector;
  Vector2 moveVectorLast;
  int lastSendMoveVector;
  bool isMove;
	bool isStoped;

	public void IsStoped(bool isStoped) {
		this.isStoped = isStoped;
		if(isStoped)
			this.moveVector = Vector2.zero;
	}

  public virtual void OnMoveVector(Vector2 moveVector) {
		if(isStoped) return;
    this.moveVector = moveVector;

    if(!isMove && moveVector != Vector2.zero) {
      isMove = true;
      firstPlayer.isMove = isMove;
    }
    
    if(isMove && moveVector == Vector2.zero) {
      isMove = false;
      firstPlayer.isMove = isMove;
    }
  }

  void SendMoveVector() {
    
    if(!(moveVector == moveVectorLast && moveVector == Vector2.zero)
      && (Constants.GTIME - lastSendMoveVector > 150 || Vector2.Angle(moveVectorLast, moveVector) > 30)) {
      lastSendMoveVector = Constants.GTIME;
      moveVectorLast = moveVector;
      Generals.Network.NetworkManager.SendPacket(new RequestJoysticPosition(moveVector));
    }
  }

  #endregion

}