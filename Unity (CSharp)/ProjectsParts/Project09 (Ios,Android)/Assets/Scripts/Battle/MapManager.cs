using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(MapManager))]
public class MapManagerEditor : Editor {

  public override void OnInspectorGUI() {
    base.OnInspectorGUI();

    if(GUILayout.Button("Инверсия")) {
      MapManager script = (MapManager)target;
      script.InvercePosition();
    }
  }
}

#endif

/// <summary>
/// Параметры половины поля
/// </summary>
[System.Serializable]
public struct Areas {
  public float topBorder;                                                   // Верхняя граница
  public float bottomBorder;                                                // Нижняя граница
  public float leftBorder;                                                  // Левая граница
  public float rightBorder;                                                 // Правая граница
  public float width { get { return rightBorder - leftBorder; } }           // Ширина
  public float height { get { return topBorder - bottomBorder; } }          // Высота
  public Vector3 center { get { return new Vector3(leftBorder + width / 2, bottomBorder + height / 2, 0); } }    // Центр
}

/// <summary>
/// Менеджер сцены
/// </summary>
public class MapManager : MonoBehaviour {

  public static MapManager instance;                // Ссылка на экземпляр плеера
  public static Actione<int> OnRotateScene;

  List<Collider2D> barrierList = new List<Collider2D>();    // Список преграл

  public GameObject mapParent;

  void Awake() {
    instance = this;
  }

  void Start() {
    // Поворачиваем объекты на карте правильным образом
    SetRotationMap(GameManager.instance.PlayerNumLeft());
    StartCoroutine(FuncRun());
  }

  private void OnDestroy() {
    instance = null;
  }

  IEnumerator FuncRun() {
    yield return new WaitForFixedUpdate();
    GenerateSceneObjects();
  }

  public static Vector2 PositionInvers(Vector2 pos) {
    if(instance == null) return pos;
    if(GameManager.instance.activeTeam == 1) pos.x = instance.area.width - (pos.x - instance.area.leftBorder) + instance.area.leftBorder;
    return pos;
  }

  public static Vector2 VectorInvers(Vector2 vect) {
    if(GameManager.instance.activeTeam == 1) vect.x *= -1;
    return vect;
  }

  public Vector3 PositionMirrow(Vector3 position) {
    position.x = (area.width - (position.x - area.leftBorder)) + area.leftBorder;
    return position;
  }

  public void InvercePosition() {
    SetRotationMap(leftPlayer == 1 ? 0 : 1);
  }

  #region Вращение объектов на карте

  public int leftPlayer = 0;
  void SetRotationMap(int activePlayerNum) {
    leftPlayer = activePlayerNum;
    if(OnRotateScene != null) OnRotateScene(leftPlayer);
  }

  #endregion

  #region Препядствия

  /// <summary>
  /// Добавление в список барьеров
  /// </summary>
  /// <param name="col"></param>
  public void AddBarrier(Collider2D col) {
    barrierList.Add(col);
  }

  /// <summary>
  /// Запрос на список барьеров
  /// </summary>
  /// <returns></returns>
  public List<Collider2D> GetAllBarriers() {
    return barrierList;
  }

  #endregion

  public Areas area;                          // Сцена

  #region Отрисовка
  void OnDrawGizmos() {
    Gizmos.color = Color.red;
    Gizmos.DrawLine(new Vector3(area.leftBorder, area.bottomBorder, 0), new Vector3(area.leftBorder, area.topBorder, 0));
    Gizmos.DrawLine(new Vector3(area.rightBorder, area.bottomBorder, 0), new Vector3(area.rightBorder, area.topBorder, 0));
    Gizmos.DrawLine(new Vector3(area.leftBorder, area.topBorder, 0), new Vector3(area.rightBorder, area.topBorder, 0));
    Gizmos.DrawLine(new Vector3(area.leftBorder, area.bottomBorder, 0), new Vector3(area.rightBorder, area.bottomBorder, 0));
  }

  #endregion

  #region Генерация стартовых объектов

  /// <summary>
  /// Объекты под генерацию
  /// </summary>
  [System.Serializable]
  public struct GeneratePrefabs {
    public GameObject pregab;
    public ObjectType type;
  }

  public List<GeneratePrefabs> generatePrefabs;                     // Объекты под генерацию

  /// <summary>
  /// Генерация объектов
  /// </summary>
  public void GenerateSceneObjects(List<ObjectSpawn> generateList = null) {

    bool gameManagerList = false;

    if(generateList == null) {
      gameManagerList = true;
      generateList = GameManager.instance.generateList;
    }

    if(generateList == null || generateList.Count == 0) return;

    foreach(ObjectSpawn one in generateList) {

      if(one.type == ObjectType.Character) {
        PlayerData playerData = (PlayerData)one.data;
        PlayerJoined playerJoin = GameManager.instance.playerJoined.Find(x=>x.messageId == ((PlayerData)one.data).ownerId);
        GameObject player = PlayersManager.instance.CreatePlayer(playerData.ownerId == User.instance.gameProfile.id, playerData.name);

        player.transform.position = PositionInvers(one.position);
        Player playerComp = player.GetComponent<Player>();
        playerComp.team = playerJoin.team;
        playerComp.userId = playerData.ownerId;
        playerComp.sceneId = one.id;
        playerComp.playerName = playerData.name;
        playerComp.energyMax = playerJoin.maxEnergy;
        playerComp.healthMax = ((PlayerData)one.data).maxHealth;
        playerComp.heightPlayer = one.size.y;
        playerComp.widthPlayer = one.size.x;
        PlayersManager.instance.PlayerCreateFinish(player.GetComponent<Player>());
      }

      if(one.type == ObjectType.Bullet) {
        PlayersManager.instance.OnPlayerShoot(one);
        continue;
      }
      
      GameObject prefab = generatePrefabs.Find(x=>x.type == one.type).pregab;
      if(prefab == null) continue;

      GameObject inst = Instantiate(prefab , PositionInvers(one.position), Quaternion.identity) as GameObject;

      inst.transform.SetParent(mapParent.transform);
      
      if(one.type == ObjectType.Mine) {
        inst.GetComponent<Bullet>().SetId(one.id);
      }

      if(one.type == ObjectType.Barrier) {
        if(inst.transform.position.x < area.center.x)
          inst.GetComponent<Barrier>().graphic.transform.localScale = new Vector3(-1, 1, 1);
        else
          inst.GetComponent<Barrier>().graphic.transform.localScale = new Vector3(1, 1, 1);

        ObjectSpawn.BarrierData bar = (ObjectSpawn.BarrierData)one.data;
        switch(bar.barrierType) {
          case BarrierType.Card:
            inst.GetComponent<Barrier>().SetHealthValue(bar.maxHealth, bar.health);
            break;
        }
        inst.GetComponent<Barrier>().Init(one.id);
      }

      if(inst.GetComponent<BoxCollider2D>() != null) {
        inst.GetComponent<BoxCollider2D>().size = one.size;
        inst.GetComponent<BoxCollider2D>().offset = one.offset;
      }

      if(inst.GetComponent<CircleCollider2D>() != null) {
        inst.GetComponent<CircleCollider2D>().radius = one.size.x;
        inst.GetComponent<CircleCollider2D>().offset = one.offset;
      }
    }

    if(gameManagerList && GameManager.instance.generateList.Count > 0) {
      Helpers.Invoke(this, SendReady, 2f);
    }
  }

  void SendReady() {
    GameManager.instance.generateList.Clear();
    Generals.Network.NetworkManager.SendPacket(new RequestBattleReady());
  }

  #endregion

}
