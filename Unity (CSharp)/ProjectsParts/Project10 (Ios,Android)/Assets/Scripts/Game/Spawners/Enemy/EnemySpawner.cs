using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(EnemySpawner))]
public class EnemySpawnerEditor: Editor {
  public override void OnInspectorGUI() {
    base.OnInspectorGUI();
  }
}

#endif

/// <summary>
/// Генератор enemy
/// </summary>
public class EnemySpawner: Singleton<EnemySpawner> {

  public const string POOLER_KEY = "enemy";

  public List<GameObject> enemyLibrary;
  private List<EnemyTypes> generateList = new List<EnemyTypes>();

  private readonly float generateTime;                     // Время для генерации

  public bool isGenerate;

  private IEnemySpawner enemySpawner;

  public EnemySpawnerLevels spawnerLevels;
  public EnemySpawnerSurvival spawnerSurvival;
  public EnemySpawnerShip spawnerShip;

  public static event EventAction<bool> StopEnemy;

  [HideInInspector]
  public bool enemyStop;

  public void OnStopEnemy(bool enemyStop) {
    this.enemyStop = enemyStop;
    if (StopEnemy != null) StopEnemy(enemyStop);
  }


  void Start() {
    RunnerController.OnChangeRunnerPhase += OnChangeRunnerPhase;

    if (RunnerController.Instance.activeLevel == ActiveLevelType.ship) {
      enemySpawner = spawnerShip;
    } else {
      if (GameManager.activeLevelData.gameMode == GameMode.survival) {
        enemySpawner = spawnerSurvival;
      } else
        enemySpawner = spawnerLevels;
    }

    enemySpawner.Init(this);

    UpdateValue();
    enemyStop = false;
  }

  protected override void OnDestroy() {
    RunnerController.OnChangeRunnerPhase -= OnChangeRunnerPhase;
    try {
      enemySpawner.DeInit();
    } catch { }
  }

  void OnChangeRunnerPhase(RunnerPhase runnerPhase) {

  }

  [ExEvent.ExEventHandler(typeof(ExEvent.RunEvents.SpecialBarrier))]
  public void OnSpecialBarrier(ExEvent.RunEvents.SpecialBarrier specialBarrier) {
    enemySpawner.OnSpecialBarrier(specialBarrier);
  }


  public void CreatePoolSpawn(List<EnemyTypes> elemyList) {
    LevelPooler.Instance.AddPool(POOLER_KEY, StructProcessor(elemyList));
  }

  void Update() {
    if (isGenerate) enemySpawner.Update();
  }

  [HideInInspector]
  public ClothesBonus enemyClothes;
  [HideInInspector]
  public ClothesBonus moneyClothes;

  void UpdateValue() {
    enemyClothes = Config.GetActiveCloth(ClothesSets.noEnemy);
    moneyClothes = Config.GetActiveCloth(ClothesSets.money);
  }

  [ExEvent.ExEventHandler(typeof(ExEvent.RunEvents.SpecialPlatform))]
  void OnHandingPlatform(ExEvent.RunEvents.SpecialPlatform specialPlatform) {
    enemySpawner.OnSpecialPlatform(specialPlatform);
  }

  public GameObject GetInstantEnemy(EnemyTypes enemyType) {
    return GetInstantEnemy(enemyLibrary.Find(x => x.GetComponent<Enemy>().enemyType == enemyType).gameObject.name);
  }
  public GameObject GetInstantEnemy(string prefabName) {
    GameObject inst = LevelPooler.Instance.GetPooledObject(POOLER_KEY, prefabName);
    inst.transform.parent = transform;
    return inst;
  }

  #region Dead cloud

  public GameObject deadCloud;                    // Эффект облока
                                                  //private GameObject deadCloudInstance;
  [HideInInspector]
  public bool deadCloudYes;     // Эффект активирован
  public void CreateDeadCloud() {
    enemySpawner.DeadCloud();
    RunnerController.Instance.PlayerDead();
    //if (!deadCloudYes) {
    //	deadCloudInstance = MonoBehaviour.Instantiate(deadCloud, Vector3.zero, Quaternion.identity) as GameObject;
    //	deadCloudYes = true;
    //}
  }

  #endregion

  /// <summary>
  /// Генерация простых бегунов при вызове гигантом
  /// </summary>
  /// <returns></returns>
  public GameObject GenEnemyForGigant() {

    GameObject obj = GetInstantEnemy(EnemyTypes.aztec);
    obj.transform.position = new Vector3(CameraController.displayDiff.transform.position.x + CameraController.displayDiff.left * 1.3f, RunnerController.Instance.mapHeight + 1f, 0);
    obj.transform.parent = transform;
    obj.SetActive(true);
    //obj.GetComponent<GeneralEnemy>().ChangePhase(runnerPhase);
    return obj;
  }
  /// <summary>
  /// Генерация выстрелов при сметри
  /// </summary>
  /// <param name="num"></param>
  /// <param name="position"></param>
  public void Shoot(int num, Vector3 position) {
    GameObject spear = Pooler.GetPooledObject("EnemySpear");
    spear.transform.position = position;
    spear.SetActive(true);
  }
  /// <summary>
  /// Подготовка пула врагов
  /// </summary>
  /// <returns></returns>
  Dictionary<string, KeyValuePair<GameObject, int>> StructProcessor(List<EnemyTypes> enemyList) {

    Dictionary<string, KeyValuePair<GameObject, int>> enemyListName = new Dictionary<string, KeyValuePair<GameObject, int>>();

    foreach (EnemyTypes oneEn in enemyList) {
      if (oneEn == EnemyTypes.none) continue;

      GameObject pref = enemyLibrary.Find(x => x.GetComponent<Enemy>().enemyType == oneEn);
      if (!enemyListName.ContainsKey(pref.name))
        enemyListName.Add(pref.name, new KeyValuePair<GameObject, int>(pref, 3));

    }

    return enemyListName;
  }
  public float lastEnemyIdlePlay;           // Время, после которого разрешается что то произносить

  /// <summary>
  /// Генерация при туториоле
  /// </summary>
  /// <param name="aztecName"></param>
  /// <returns></returns>
  public static GameObject GenerateTutor(EnemyTypes enemy = EnemyTypes.aztec) {

    GameObject obj = Instance.GetInstantEnemy(enemy);
    obj.transform.position = new Vector3(CameraController.displayDiff.transform.position.x + CameraController.displayDiff.left * 1.3f, RunnerController.Instance.mapHeight + 1f, 0);

    obj.transform.parent = Instance.transform;
    //obj.GetComponent<Enemy>().ChangePhase(instance.runnerPhase);
    obj.SetActive(true);
    return obj;
  }


  //private int fillEnemyPoint;
  public void EnemyDead(GameObject deadObject) {

    GeneralEnemy[] enemyes = GetComponentsInChildren<GeneralEnemy>();
    int active = 0;

    foreach (GeneralEnemy one in enemyes)
      if (one.isActiveAndEnabled)
        active++;

    if (active <= 1) {

      float nextPodDistance = RunnerController.playerDistantion;
      if (generateList.Count <= 0) {
        int num = Random.value < 0.5f ? 1 : 2;
        for (int i = 0; i < num; i++) {
          generateList.Add(enemyLibrary[Random.Range(0, 2)].GetComponent<Enemy>().enemyType);
        }
        //fillEnemyPoint = 20;
      }
    }
  }

}