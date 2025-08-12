using System.Collections.Generic;
using UnityEngine;


public enum WallType {
  idle                    // Простой сектор, 1х по ширине
  , ruptureStart          // Начальный сектор разрыва, 1 тип
  , ruptureEnd            // Конечный сектор разрыва, 1 тип
  , none
};
public enum WallCategory {
  idle
  , gap
  , destroy
  , hole
  , decor
}

[System.Serializable]
public struct GenerateWallParametrs {
  public FloatSpan distance;
  public float probabili;
}

[System.Serializable]
public struct Wall {
  //public GameObject prefab;
  public WallType type;
  public WallCategory category;
  public int amount;                      // Количество платформ, инициализируется в pool
  public float probability;               // Вероятность появления платформы на сцене
  public List<GenerateWallParametrs> probabili;
  public RegionType[] mapType;      // Блоки, в которых выполняется генерация
  public bool canSpawnAtStart;            // Объект использует при подготовке этапа
  public bool noRepeat;                   // Не повторять, тоесть ориентируется по списку повторений

}

public class WallSurvivalSpawn: WallSpawn {

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

  private float distantionCalc = 0;                                       // Дистанция рассчета новой генерации
  private readonly float perionGeneration;                                         // Период генерации
  private List<WallCategory> categoryList = new List<WallCategory>();     // Вероятность генерации платформ по категории
  private List<float> categoryCD = new List<float>();                     // Вероятность генерации платформ по категории
  public List<GenerateParametrs> generateParametrs = new List<GenerateParametrs>();       // Параметры генерации

  protected override void Start() {

    GetConfig();
    lastSpawnPosition = Vector3.left * 50;
    CalcDisplay();

    rememberArr = new int[noRepeatItems]; // создаем список запомненных
    typeMap = Regions.type;
    typeMapFull = typeMap;

    GameManager.OnChangeLevel += GetConfig;
    RunSpawner.OnGenerateMapObject += OnGenerateMapObject;

    StructProcessor();

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
      //Spawn(new Vector3(displayDiff.rightDif(1.5f), transform.position.y, 0));
    }
    typeMap = region.newType;

    // запоминаем последнюю полную стадию
    typeMapFull = region.newType;

    nextCalcPeriodDistance = RunnerController.playerDistantion;

  }
  void StructProcessor() {

    wallList.Clear();
    cd.Clear();

    wallList.Add(prefab.name, new KeyValuePair<GameObject, int>(prefab, 1));

    LevelPooler.Instance.AddPool(POOL_KEY, wallList, instanceList);
  }

  public GameObject Spawn(Vector3 position, WallType wallType = WallType.none) {
    // Инкремент счетчика созжанных объектов
    counter++;

    WallCategory needCat = categoryList[BinarySearch.RandomNumberGenerator(categoryCD)];

    if (wallType == WallType.none)
      wallType = WallType.idle;
    else
      needCat = WallCategory.idle;

    if (needCat == WallCategory.hole) wallType = WallType.ruptureStart;

    GameObject obj = LevelPooler.Instance.GetPooledObject(POOL_KEY, prefab.name, instanceList);

    obj.GetComponent<WallDecor>().Init(wallType, needCat);
    obj.transform.position = new Vector3(position.x, position.y, transform.position.z);

    if (IsIpad) obj.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);

    WallDecor decor = obj.GetComponent<WallDecor>();

    if (decor != null) {
      if (decor != null)
        decor.SetOrder(0, layer);
      else {
        Debug.Log(obj.name);
      }
    }

    lastType = wallType;

    // Если был сгенерирован разрыв, рассчитываем ширину
    if (lastType == WallType.ruptureStart) {
      ruptureWidth = Random.Range(0f, 13.2f);
    } else {
      ruptureWidth = 0;
    }

    lastSpawnPosition = obj.transform.position;
    obj.SetActive(true);
    return obj;
  }


  protected override void Update() {
    base.Update();

    if (distantionCalc <= RunnerController.playerDistantion) {
      GenerateParametrs newGeneration = generateParametrs.Find(x => x.distantion > distantionCalc);
      CalcProbability(newGeneration);
      distantionCalc = newGeneration.distantion;
    }
    Generate();
    DestroyOld();
  }

  void Generate() {

    while (lastSpawnPosition.x + (wallWidth * (IsIpad ? 1.1f : 1)) + (ruptureWidth * (IsIpad ? 1.1f : 1)) <= displayDiff.rightDif(1.7f)) {
      GenerateWall();
    }
  }

  void DestroyOld() {
    if (timeToCheckDeactive > Time.time) return;
    timeToCheckDeactive = Time.time + 10;
    instanceList.ForEach(CheckOut);
  }

  void CheckOut(Transform one) {
    if (!one.gameObject.activeInHierarchy) return;
    if (one.position.x < displayDiff.leftDif(3)) one.gameObject.SetActive(false);
  }

  void CalcProbability(GenerateParametrs parametrs) {
    categoryCD.Clear();
    categoryList.Clear();

    float sum = 0;
    categoryCD.Add(sum);

    foreach (WallCategories cat in parametrs.category) {
      sum += cat.value;
      categoryCD.Add(sum);
      categoryList.Add(cat.type);
    }

    if (sum < 1) {
      sum += 1 - sum;
      categoryCD.Add(sum);
      categoryList.Add(WallCategory.idle);
    }

    categoryCD = categoryCD.ConvertAll(x => x / sum);
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

  List<int> generateList = new List<int>();
  List<float> probability = new List<float>();

  //Dictionary<RegionType,>

  float? nextCalcPeriodDistance = 0;

  void CalcProbability() {
    probability = new List<float>();
    generateList = new List<int>();
    nextCalcPeriodDistance = null;

    float sum1 = 0;
    probability.Add(sum1);

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
            probability.Add(sum1);
            generateList.Add(i);

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
        probability.Add(sum1);
        generateList.Add(i);
        if (nextCalcPeriodDistance == null)
          nextCalcPeriodDistance += 500;
      }
    }

    if (sum1 != 1f)
      probability = probability.ConvertAll(x => x / sum1);
  }

  void GenerateWall() {

    generateDecor = false;              // Декорация не сгенерирована 

    WallType wall = WallType.none;

    if (ruptureWidth > 0)
      wall = WallType.ruptureEnd;

    GameObject genObject = Spawn(new Vector3(lastSpawnPosition.x + (wallWidth * (IsIpad ? 1.1f : 1)) + (ruptureWidth * (IsIpad ? 1.1f : 1)), transform.position.y, transform.position.z), wall);
    //Debug.Log(genObject.transform.position);

    // Генерируем декор фанаря
    if (GameManager.activeLevelData.gameMode == GameMode.survival && counter >= nextTorchGenerale && (lastType == WallType.idle) && !generateDecor && RunnerController.Instance.activeLevel != ActiveLevelType.ship) {
      if (Random.value <= 0.4f) {
        genObject.GetComponent<WallDecor>().SetTorch();
        generateDecor = true;
        nextTorchGenerale = counter + 1;
      }
    } else if (GameManager.activeLevelData.gameMode != GameMode.survival && torchGenerateCount > 0) {
      if (genObject.GetComponent<WallDecor>().SetTorch())
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

  protected void GetConfig() {

    List<Configuration.Survivles.Decoration> genParam = Config.Instance.config.survival.decoration;

    generateParametrs = new List<GenerateParametrs>();
    float allWallParam = 1;

    for (int i = 0; i < genParam.Count; i++) {
      GenerateParametrs newParam = new GenerateParametrs();
      newParam.category = new List<WallCategories>();

      //newParam.distantion = float.Parse(oneItem["distantion"].ToString());
      newParam.distantion = i * 500 + 500;
      newParam.count = 0;
      allWallParam = 1;

      if (genParam[i].decorWall > 0) {
        WallCategories box = new WallCategories();
        box.type = WallCategory.decor;
        box.value = genParam[i].decorWall * 0.01f;
        allWallParam = box.value;
        newParam.category.Add(box);
      }

      if (genParam[i].wallBreack > 0) {
        WallCategories box = new WallCategories();
        box.type = WallCategory.hole;
        box.value = genParam[i].wallBreack * 0.01f;
        allWallParam = box.value;
        newParam.category.Add(box);
      }

      if (genParam[i].wallDestroy > 0) {
        WallCategories box = new WallCategories();
        box.type = WallCategory.destroy;
        box.value = genParam[i].wallDestroy * 0.01f;
        allWallParam = box.value;
        newParam.category.Add(box);
      }

      if (genParam[i].wallDestroyMini > 0) {
        WallCategories box = new WallCategories();
        box.type = WallCategory.gap;
        box.value = genParam[i].wallDestroyMini * 0.01f;
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
