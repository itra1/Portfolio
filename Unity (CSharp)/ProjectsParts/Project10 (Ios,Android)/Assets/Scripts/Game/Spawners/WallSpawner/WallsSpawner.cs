using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Decors {
  public GameObject prefab;
}

public class WallsSpawner: Singleton<WallsSpawner> {

  [HideInInspector]
  public int layerId;                                         // Идентификатор слоя
  public int noRepeatItems;
  protected DisplayDiff displayDiff;                            // Смещение видимой части

  public const string POOLER_KEY = "walls";

  [SerializeField]
  private readonly int amountAtStart;                                          // Число платформ, необходимые при запуске игры
  public List<Wall> walls;                                                // Массив платфор
  [SerializeField]
  public GameObject[] decors;                                         // Массив декораций
  Dictionary<string, KeyValuePair<GameObject, int>> wallList = new Dictionary<string, KeyValuePair<GameObject, int>>();                       // Список преобразованный в массив структур платформ
  List<float> cd = new List<float>();                                 // Список для определения вероятности генерации

  public static Vector3 lastSpawnPosition;

  protected List<Transform> generateList = new List<Transform>();

  public float wallWidth;
  private float lastWidth;

  public RegionType typeMap;
  public RegionType typeMapFull;
  readonly float distanceChangeMap;

  private WallType lastGenerateType;
  readonly int rand;
  readonly bool generate;                  // Флаг генерации объекта
  protected bool generateDecor;             // Флаг генерации декорации
  readonly int nextDecorGenerate;          // Следующий сгенерированный элемент декора
  protected int nextTorchGenerale;          // Следующий сгенерированная лампа
  protected float ruptureWidth = 0;

  protected int thisNumber = 0;                 // Счетчик созданных объектов
  int[] rememberList;                 // Лист запомненных объектов

  protected bool IsIpad;

  public GameObject wallPrefab;

  protected virtual void Start() {

    IsIpad = GameManager.isIPad;

    displayDiff = CameraController.Instance.CalcDisplayDiff(transform.position.z);

    rememberList = new int[noRepeatItems]; // создаем список запомненных
    typeMap = Regions.type;
    typeMapFull = typeMap;

    GameManager.OnChangeLevel += GetConfig;
    RunSpawner.OnGenerateMapObject += OnGenerateMapObject;

    StructProcessor();
    LevelPooler.Instance.AddPool(POOLER_KEY, wallList, generateList);

    torchGenerateCount = 0;

    lastGenerateType = WallType.idle;
  }

  protected override void OnDestroy() {
    base.OnDestroy();
    GameManager.OnChangeLevel -= GetConfig;
    RunSpawner.OnGenerateMapObject -= OnGenerateMapObject;
  }

  void OnGenerateMapObject(MapObject generateObject) {
    if (generateObject == MapObject.torch) {
      torchGenerateCount++;
    }
  }

  [HideInInspector]
  public int torchGenerateCount = 0;

  [ExEvent.ExEventHandler(typeof(ExEvent.RunEvents.RegionChange))]
  void ChangeMap(ExEvent.RunEvents.RegionChange region) {

    // Если последний сгенерированный сегмент не был началом разлома
    if (region.newType == RegionType.ShipRoom) {
      Spawn(cd, new Vector3(displayDiff.rightDif(1.5f), transform.position.y, 0));
    }
    typeMap = region.newType;

    // запоминаем последнюю полную стадию
    typeMapFull = region.newType;

    nextCalcPeriodDistance = RunnerController.playerDistantion;

  }
  void StructProcessor() {

    wallList.Clear();
    cd.Clear();

    wallList.Add(wallPrefab.name, new KeyValuePair<GameObject, int>(wallPrefab, 1));
  }

  Vector3 SpawnAtStart(int amount) {
    Vector3 playerGroundPosition = new Vector3(displayDiff.leftDif(1.2f), transform.position.y, transform.position.z);
    Spawn(cd, playerGroundPosition);
    Vector3 lastSpawnPoint = playerGroundPosition;
    lastWidth = wallWidth * 2;

    Vector3 lastVector = Vector3.zero;

    for (int i = 0; i < amount - 1; i++) {
      lastSpawnPoint = new Vector3(lastSpawnPoint.x + (lastWidth * (IsIpad ? 1.1f : 1)), transform.position.y, transform.position.z);
      lastWidth = wallWidth * 2;
      GameObject last = Spawn(cd, lastSpawnPoint);
      lastVector = last.transform.position;
    }

    return new Vector3(lastVector.x, transform.position.y, transform.position.z);
  }

  GameObject Spawn(List<float> cd, Vector3 position, WallType wallType = WallType.none, bool checkType = true) {
    // Инкремент счетчика созжанных объектов
    thisNumber++;

    GameObject obj;

    if ((float)nextCalcPeriodDistance <= RunnerController.playerDistantion)
      CalcProbability();

    bool chekers = false;
    int cds = 0;
    while (!chekers && wallType == WallType.none) {
      //cds = CalcNumber();
      cds = wallsListGenerate[BinarySearch.RandomNumberGenerator(wallsProbability)];

      chekers = true;

      // Проверка на соответствие элемента текущему типу учатка
      if (chekers) {
        chekers = false;
        foreach (RegionType mapOne in walls[cds].mapType) {
          if (mapOne == typeMapFull)
            chekers = true;
        }
      }

      // Проверка на запрет повторения элементов рядом
      if (chekers) {
        if (walls[cds].noRepeat) {
          foreach (int one in rememberList) {
            if (one == cds)
              chekers = false;
          }
        }
      }

      // Добавляем элемент в лист для не повторяющих
      if (chekers) {
        if (noRepeatItems != 0)
          rememberList[thisNumber % noRepeatItems] = cds;
      }
    }

    int numb = -1;

    if (wallType != WallType.none) {
      List<int> arrNum = new List<int>();

      for (int i = 0; i < walls.Count; i++) {
        if (walls[i].type == wallType)
          arrNum.Add(i);
      }
      int[] arrNumNed = arrNum.ToArray();
      numb = arrNumNed[Random.Range(0, arrNum.Count)];
    }

    int genNum = (numb >= 0) ? numb : cds;

    obj = LevelPooler.Instance.GetPooledObject(POOLER_KEY, genNum, generateList);

    // запоминаем последний сгенерированный тип
    if (checkType)
      lastGenerateType = walls[genNum].type;


    obj.transform.position = new Vector3(position.x, position.y, transform.position.z);

    if (IsIpad) {
      obj.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
    }

    if (System.Array.IndexOf(new WallType[] { WallType.idle }, lastGenerateType) >= 0) {
      lastWidth = wallWidth;
    } else if (System.Array.IndexOf(new WallType[] { WallType.ruptureEnd, WallType.ruptureStart }, lastGenerateType) >= 0) {
      lastWidth = wallWidth * 2f;
    } else {
      lastWidth = wallWidth * 3;
    }

    DecorController decor = obj.GetComponent<DecorController>();

    if (decor != null) {
      if (decor != null)
        decor.SetOrder(0, layerId);
      else {
        Debug.Log(obj.name);
      }
    }

    // Если был сгенерирован разрыв, рассчитываем ширину
    if (lastGenerateType == WallType.ruptureStart) {
      ruptureWidth = Random.Range(0f, 100f);
    } else {
      ruptureWidth = 0;
    }

    lastSpawnPosition = obj.transform.position;
    obj.SetActive(true);
    return obj;
  }

  void SurvivalGenerate() {
    if (RunnerController.playerDistantion > allMaxDistance - 500 && RunnerController.playerDistantion < allMaxDistance) InfinityIncrement();

    int iterator = 0; // Временная заглушка
    while (lastSpawnPosition.x + (lastWidth * (IsIpad ? 1.1f : 1)) + (ruptureWidth * (IsIpad ? 1.1f : 1)) <= displayDiff.rightDif(1.7f)) {
      iterator++;
      GenerateWall();
      if (iterator > 3)
        return;
    }

    if (timeToCheckDeactive <= Time.time) {
      timeToCheckDeactive = Time.time + 10;
      // скрываем объекты, если вышли из зоны видимости
      for (int i = 0; i < generateList.Count; i++) {
        if (generateList[i].gameObject.activeInHierarchy &&
            (generateList[i].position.x < displayDiff.leftDif(3)
            || generateList[i].position.x > displayDiff.rightDif(3))) {
          generateList[i].gameObject.SetActive(false);
        }
      }
    }
  }

  /// <summary>
  /// Время проверки на деактивацию объекта
  /// </summary>
  protected float timeToCheckDeactive;

  List<int> wallsListGenerate = new List<int>();
  List<float> wallsProbability = new List<float>();

  //Dictionary<RegionType,>

  float? nextCalcPeriodDistance = 0;

  void CalcProbability() {
    wallsProbability = new List<float>();
    wallsListGenerate = new List<int>();
    nextCalcPeriodDistance = null;

    float sum1 = 0;
    wallsProbability.Add(sum1);

    for (int i = 0; i < walls.Count; i++) {

      bool getsType = false;

      foreach (RegionType mapOne in walls[i].mapType) {
        if (mapOne == typeMapFull)
          getsType = true;
      }

      if (getsType == false)
        continue;

      if (walls[i].probabili.Count > 0) {

        for (int j = 0; j < walls[i].probabili.Count; j++) {
          if (walls[i].probabili[j].distance.min <= RunnerController.playerDistantion && walls[i].probabili[j].distance.max > RunnerController.playerDistantion) {
            sum1 += walls[i].probabili[j].probabili;
            wallsProbability.Add(sum1);
            wallsListGenerate.Add(i);

            if (j != walls[i].probabili.Count - 1) {
              if (nextCalcPeriodDistance == null || nextCalcPeriodDistance > walls[i].probabili[j + 1].distance.min)
                nextCalcPeriodDistance = walls[i].probabili[j + 1].distance.min;
            }

            if (nextCalcPeriodDistance == null)
              nextCalcPeriodDistance += 500;
          }

        }
      } else {
        sum1 += walls[i].probability;
        wallsProbability.Add(sum1);
        wallsListGenerate.Add(i);
        if (nextCalcPeriodDistance == null)
          nextCalcPeriodDistance += 500;
      }
    }

    if (sum1 != 1f)
      wallsProbability = wallsProbability.ConvertAll(x => x / sum1);
  }

  void GenerateWall() {

    generateDecor = false;              // Декорация не сгенерирована 

    WallType wall = WallType.none;

    if (ruptureWidth > 0) {
      wall = WallType.ruptureEnd;
    }

    GameObject genObject = Spawn(cd, new Vector3(lastSpawnPosition.x + (lastWidth * (IsIpad ? 1.1f : 1)) + (ruptureWidth * (IsIpad ? 1.1f : 1)), transform.position.y, transform.position.z), wall);

    // Генерируем декор фанаря
    if (GameManager.activeLevelData.gameMode == GameMode.survival && thisNumber >= nextTorchGenerale && (lastGenerateType == WallType.idle) && !generateDecor && RunnerController.Instance.activeLevel != ActiveLevelType.ship) {
      if (Random.value <= 0.4f) {
        genObject.GetComponent<DecorController>().SetTorch();
        generateDecor = true;
        nextTorchGenerale = thisNumber + 1;
      }
    } else if (GameManager.activeLevelData.gameMode != GameMode.survival && torchGenerateCount > 0) {
      if (genObject.GetComponent<DecorController>().SetTorch())
        torchGenerateCount--;
    }
  }

  float allMaxDistance = 6000;

  /// <summary>
  /// Бесконечный инкремент параметров
  /// </summary>
  void InfinityIncrement() {
    for (int i = 0; i < walls.Count; i++) {

      if (walls[i].probabili.Count <= 3)
        continue;

      if (walls[i].probabili[walls[i].probabili.Count - 1].distance.max == allMaxDistance) {

        GenerateWallParametrs one = new GenerateWallParametrs();
        one.probabili = walls[i].probabili[walls[i].probabili.Count - 3].probabili;
        one.distance.min = walls[i].probabili[walls[i].probabili.Count - 1].distance.max;
        one.distance.max = walls[i].probabili[walls[i].probabili.Count - 1].distance.max + 500;

        GenerateWallParametrs two = new GenerateWallParametrs();
        two.probabili = walls[i].probabili[walls[i].probabili.Count - 2].probabili;
        two.distance.min = walls[i].probabili[walls[i].probabili.Count - 3].distance.max + 500;
        two.distance.max = walls[i].probabili[walls[i].probabili.Count - 3].distance.max + 1000;

        GenerateWallParametrs tre = new GenerateWallParametrs();
        tre.probabili = walls[i].probabili[walls[i].probabili.Count - 1].probabili;
        tre.distance.min = walls[i].probabili[walls[i].probabili.Count - 1].distance.max + 1000;
        tre.distance.max = walls[i].probabili[walls[i].probabili.Count - 1].distance.max + 1500;

        walls[i].probabili.Add(one);
        walls[i].probabili.Add(two);
        walls[i].probabili.Add(tre);
      }
    }
    allMaxDistance = allMaxDistance + 1500;
  }

  protected virtual void GetConfig() { }

}