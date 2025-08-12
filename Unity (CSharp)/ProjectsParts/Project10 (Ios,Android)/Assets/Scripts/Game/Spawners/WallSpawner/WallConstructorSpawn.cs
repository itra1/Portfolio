using System.Collections.Generic;
using UnityEngine;

public class WallConstructorSpawn: WallSpawn {

  public int noRepeatItems;

  [SerializeField]
  private readonly int amountAtStart;                                          // Число платформ, необходимые при запуске игры
  public List<Wall> walls;                                                // Массив платфор

  public GameObject[] decors;                                         // Массив декораций
  Dictionary<string, KeyValuePair<GameObject, int>> wallList = new Dictionary<string, KeyValuePair<GameObject, int>>();                       // Список преобразованный в массив структур платформ

  private static Vector3 lastSpawnPosition;

  protected List<Transform> instanceList = new List<Transform>();

  public float wallWidth;
  private float lastWidth;

  public RegionType typeMap;
  public RegionType typeMapFull;
  readonly float distanceChangeMap;

  private WallType lastType;
  private readonly bool generate;                  // Флаг генерации объекта
  protected bool generateDecor;             // Флаг генерации декорации
  private readonly int nextDecorGenerate;          // Следующий сгенерированный элемент декора
  protected int nextTorchGenerale;          // Следующий сгенерированная лампа
  protected float ruptureWidth = 0;

  protected int counter = 0;                 // Счетчик созданных объектов
  private int[] rememberArr;                 // Лист запомненных объектов

  public GameObject wallPrefab;
  readonly float distantionCalc = 0;                                       // Дистанция рассчета новой генерации
  readonly float perionGeneration;                                         // Период генерации
  readonly List<WallCategory> categoryList = new List<WallCategory>();     // Вероятность генерации платформ по категории
  readonly List<float> categoryCD = new List<float>();                     // Вероятность генерации платформ по категории
  public List<GenerateParametrs> generateParametrs = new List<GenerateParametrs>();       // Параметры генерации

  protected override void Start() {

    GameManager.OnChangeLevel += GetConfig;
    RunSpawner.OnGenerateMapObject += OnGenerateMapObject;

    CalcDisplay();

    rememberArr = new int[noRepeatItems]; // создаем список запомненных
    typeMap = Regions.type;
    typeMapFull = typeMap;
    lastSpawnPosition = Vector3.zero;

    StructProcessor();
    LevelPooler.Instance.AddPool(POOL_KEY, wallList, instanceList);

    torchGenerateCount = 0;

    lastType = WallType.idle;

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

  private Vector3 SpawnAtStart(int amount) {
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
    counter++;

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
          foreach (int one in rememberArr) {
            if (one == cds)
              chekers = false;
          }
        }
      }

      // Добавляем элемент в лист для не повторяющих
      if (chekers) {
        if (noRepeatItems != 0)
          rememberArr[counter % noRepeatItems] = cds;
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

    GameObject obj = LevelPooler.Instance.GetPooledObject(POOL_KEY, genNum, instanceList);

    if (checkType)
      lastType = walls[genNum].type;

    obj.transform.position = IsIpad
      ? obj.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f)
      : new Vector3(position.x, position.y, transform.position.z);


    if (System.Array.IndexOf(new WallType[] { WallType.idle }, lastType) >= 0) {
      lastWidth = wallWidth;
    } else if (System.Array.IndexOf(new WallType[] { WallType.ruptureEnd, WallType.ruptureStart }, lastType) >= 0) {
      lastWidth = wallWidth * 2f;
    } else {
      lastWidth = wallWidth * 3;
    }

    DecorController decor = obj.GetComponent<DecorController>();

    if (decor != null) {
      decor.SetOrder(0, layer);
    }

    // Если был сгенерирован разрыв, рассчитываем ширину
    if (lastType == WallType.ruptureStart) {
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
      for (int i = 0; i < instanceList.Count; i++) {
        if (instanceList[i].gameObject.activeInHierarchy &&
            (instanceList[i].position.x < displayDiff.leftDif(3)
            || instanceList[i].position.x > displayDiff.rightDif(3))) {
          instanceList[i].gameObject.SetActive(false);
        }
      }
    }
  }

  /// <summary>
  /// Время проверки на деактивацию объекта
  /// </summary>
  protected float timeToCheckDeactive;

  private List<int> wallsListGenerate = new List<int>();
  private List<float> wallsProbability = new List<float>();

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
    if (GameManager.activeLevelData.gameMode == GameMode.survival && counter >= nextTorchGenerale && (lastType == WallType.idle) && !generateDecor && RunnerController.Instance.activeLevel != ActiveLevelType.ship) {
      if (Random.value <= 0.4f) {
        genObject.GetComponent<DecorController>().SetTorch();
        generateDecor = true;
        nextTorchGenerale = counter + 1;
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

  /// <summary>
	/// Получение настроек
	/// </summary>
	private void GetConfig() {

    List<Configuration.Levels.Road> itemsData = Config.Instance.config.activeLevel.road;
    List<Configuration.Levels.Decoration> decorData = Config.Instance.config.activeLevel.decoration;

    generateParametrs = new List<GenerateParametrs>();
    float allWallParam = 1;

    for (int i = 0; i < itemsData.Count; i++) {
      GenerateParametrs newParam = new GenerateParametrs();
      newParam.category = new List<WallCategories>();
      allWallParam = 1;

      newParam.distantion = itemsData[i].distantion;
      if (decorData[i].decor > 0)
        newParam.count = decorData[i].decor;

      if (decorData[i].decorWall > 0) {
        WallCategories box = new WallCategories();
        box.type = WallCategory.decor;
        box.value = decorData[i].decorWall * 0.01f;
        allWallParam = box.value;
        newParam.category.Add(box);
      }

      if (decorData[i].wallBreack > 0) {
        WallCategories box = new WallCategories();
        box.type = WallCategory.hole;
        box.value = decorData[i].wallBreack * 0.01f;
        allWallParam = box.value;
        newParam.category.Add(box);
      }

      if (decorData[i].wallDestroy > 0) {
        WallCategories box = new WallCategories();
        box.type = WallCategory.destroy;
        box.value = decorData[i].wallDestroy * 0.01f;
        allWallParam = box.value;
        newParam.category.Add(box);
      }

      if (decorData[i].wallDestroyMini > 0) {
        WallCategories box = new WallCategories();
        box.type = WallCategory.gap;
        box.value = decorData[i].wallDestroyMini * 0.01f;
        allWallParam = box.value;
        newParam.category.Add(box);
      }

      if (allWallParam > 0) {
        WallCategories box = new WallCategories();
        box.type = WallCategory.idle;
        box.value = allWallParam;
        newParam.category.Add(box);
      }

      generateParametrs.Add(newParam);
    }

  }
   
  /// <summary>
  /// Параметры дистанции
  /// </summary>
  [System.Serializable]
  public struct GenerateParametrs {
    public float distantion;
    public int count;
    public List<WallCategories> category;

  }

  /// <summary>
  /// Вероятность генерации
  /// </summary>
  [System.Serializable]
  public struct WallCategories {
    public WallCategory type;
    public float value;
  }

}
