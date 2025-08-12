using System.Collections.Generic;
using UnityEngine;

public enum PlatformType {
  idle,
  bridge,
  bridgeIdle,
  breaks,
  breaksEnd,
  none
};

public enum PlatformCategory {
  idle,
  destroy,
  decor
}

public enum PlatformMaterial {
  stone,
  wood,
  log
}

[System.Serializable]
public struct GeneratePlatformParametrs {
  public FloatSpan distance;
  public float probabili;
}

[System.Serializable]
public struct Platform {
  public GameObject prefab;
  public PlatformType type;           // Тип платформы
  public PlatformCategory category;
  public int amount;                  // Количество платформ, инициализируется в pool
  public float probability;           // Вероятность появления платформы на сцене
  public List<GeneratePlatformParametrs> probabili;
  public RegionType[] mapType;      // Блоки, в которых выполняется генерация
  public bool canSpawnAtStart;        // Объект использует при подготовке этапа
  public bool noRepeat;               // Не повторять, тоесть ориентируется по списку повторений
}

/// <summary>
/// Генератор платформ
/// </summary>
public class PlatformSpawner: Singleton<PlatformSpawner> {

  [HideInInspector]
  public int layerId;
  //public RunnerController runner;                     // Ссылка на геймконтроллер     
  //public GameObject cameraMain;                       // Ссылка на объект основной камеры
  //[HideInInspector]
  //public Transform cameraMainTransf;                 // Положение камеры
  [HideInInspector]
  public DisplayDiff displayDiff;                    // Смещение видимой части

  public int noRepeatItems;
  public float colorValue;

  public const string POOLER_KEY = "platforms";

  [HideInInspector]
  public bool generateBreack;
  [HideInInspector]
  public float needsDistBreack;

  public float HeightMax;
  public float HeightMin;

  [HideInInspector]
  public float thisHeight;
  
  public Platform[] platforms;  // Массив платформ
  private Dictionary<string, KeyValuePair<GameObject, int>> platformList; //Список преобразованный в массив структур платформ
  private List<float> cd;
  public static Vector2 lastSpawnPosition;
  private List<Transform> islandList = new List<Transform>();
  [SerializeField]
  private readonly float platformWidth;

  public Vector2 bridgeLenght;
  public Vector2 breackLenght;

  private int bridgeCount = 0;
  private PlatformType type;
  private readonly bool thisBreak;
  private float breakWidth;
  private GameObject obj;

  private int thisNumber = 0;                 // Счетчик созданных объектов
  private int[] rememberList;                 // Лист запомненных объектов

  public static System.Action breackGenEnd;

  public RegionType region;

  public IPlatformSpawner spawner;

  public PlatformSpawnerLevel spawnerLevel;
  public PlatformSpawnerSurvival spawnerSurvival;

  public GameObject stonePlatformPrefab;
  public GameObject woodPlatformPrefab;
  public GameObject logPlatformPrefab;

  void Start() {

    if (GameManager.activeLevelData.gameMode == GameMode.survival)
      spawner = spawnerSurvival;
    else
      spawner = spawnerLevel;

    spawner.Init(this);

    // Получаем положение камеры
    //cameraMainTransf = cameraMain.transform;
    displayDiff = CameraController.Instance.CalcDisplayDiff(transform.position.z);

    RunSpawner.GenerateBreackEvent += GenerateBreack;
    HandingPlatform.generateHendingPlatform += GenerateHendingPlatform;
    GameManager.OnChangeLevel += GetConfig;

    thisHeight = HeightMin;
    RunnerController.Instance.mapHeight = thisHeight;
    rememberList = new int[noRepeatItems];

    ChangeMap(new ExEvent.RunEvents.RegionChange(Regions.type));

    StructProcessor();
    LevelPooler.Instance.AddPool(POOLER_KEY, platformList);

    type = PlatformType.idle;
  }

  protected override void OnDestroy() {
    base.OnDestroy();
    RunSpawner.GenerateBreackEvent -= GenerateBreack;
    HandingPlatform.generateHendingPlatform -= GenerateHendingPlatform;
    GameManager.OnChangeLevel -= GetConfig;
  }

  bool handingPlatformWishBreack;

  void GenerateHendingPlatform(bool isActive, int type) {
    if (isActive && (type == 3 || type == 5)) {
      handingPlatformWishBreack = true;
      GenerateBreack(999999);
    } else if (!isActive && handingPlatformWishBreack) {
      handingPlatformWishBreack = false;
      breakWidth = 0;

      spawner.Spawn(new Vector2(displayDiff.rightDif(3), thisHeight), PlatformType.breaksEnd);
    }
  }

  [ExEvent.ExEventHandler(typeof(ExEvent.RunEvents.RegionChange))]
  void ChangeMap(ExEvent.RunEvents.RegionChange region) {
    this.region = region.newType;
  }

  void GenerateBreack(float dist) {
    needsDistBreack = dist;
    generateBreack = true;
  }

  // Рассчет распределения и создания списка пула
  void StructProcessor() {
    //Создаем список объектов
    platformList = new Dictionary<string, KeyValuePair<GameObject, int>>();
    cd = new List<float>();

    float sum1 = 0;

    cd.Add(sum1);

    platformList.Add(stonePlatformPrefab.name, new KeyValuePair<GameObject, int>(stonePlatformPrefab, 1));
    sum1 += 1;
    cd.Add(sum1);
    platformList.Add(woodPlatformPrefab.name, new KeyValuePair<GameObject, int>(woodPlatformPrefab, 1));
    sum1 += 1;
    cd.Add(sum1);
    platformList.Add(logPlatformPrefab.name, new KeyValuePair<GameObject, int>(logPlatformPrefab, 1));
    sum1 += 1;
    cd.Add(sum1);

    cd = cd.ConvertAll(x => x / sum1);

  }

  // Генерация платформ перед запуском
  Vector2 SpawnAtStart(int amount) {
    for (int i = 0; i < amount - 1; i++) {
      GeneratePlatform();
    }
    //return new Vector2(lastVector.x , thisHeight);
    return obj.transform.position;
  }

  public GameObject Spawn(GameObject spawnPrefab, Vector3 position) {
    return null;
  }

  public GameObject Spawn(List<float> cd, Vector2 pos, PlatformType platformType = PlatformType.none) {
    // Инкремент счетчика созжанных объектов
    thisNumber++;

    bool chekers = false;
    int cds = 0;

    if ((float)nextCalcPeriodDistance <= RunnerController.playerDistantion) CalcProbability();

    while (!chekers && platformType == PlatformType.none) {

      cds = platformsListGenerate[BinarySearch.RandomNumberGenerator(platformProbability)];

      while (!RunnerController.Instance.IsMapChangeBlock) {
        if (platforms[cds].type == PlatformType.idle) break;

        cds = platformsListGenerate[BinarySearch.RandomNumberGenerator(platformProbability)];
      }
      if (platforms[cds].noRepeat) {
        if (!InArray(rememberList, cds))
          chekers = true;
      } else
        chekers = true;

      // Запоминаем при необходимости
      if (noRepeatItems != 0) rememberList[thisNumber % noRepeatItems] = cds;
    }

    int numb = -1;

    if (platformType != PlatformType.none) {
      List<int> arrNum = new List<int>();

      for (int i = 0; i < platforms.Length; i++) {
        if (platforms[i].type == platformType) {
          if (platforms[i].mapType.Length > 0) {
            foreach (RegionType mapOne in platforms[i].mapType) {
              if (mapOne == region)
                arrNum.Add(i);
            }
          } else
            arrNum.Add(i);
        }
      }
      int[] arrNumNed = arrNum.ToArray();
      try {
        numb = arrNumNed[Random.Range(0, arrNum.Count)];
      } catch {
        Debug.Log(platformType);
      }
    }

    int genNum = (numb >= 0) ? numb : cds;

    type = platforms[genNum].type;
    GameObject obj = LevelPooler.Instance.GetPooledObject(POOLER_KEY, platforms[genNum].prefab.name);

    if (type == PlatformType.bridge) bridgeCount = Random.Range((int)bridgeLenght.x, (int)bridgeLenght.y);

    if (type == PlatformType.breaks) {
      if (needsDistBreack > 0) {
        breakWidth = needsDistBreack;
        needsDistBreack = 0;
      } else
        breakWidth = Random.Range(breackLenght.x, breackLenght.y);

      // Изменение высоты платформы не допускается во время тутора
      if (RunnerController.Instance.runnerPhase != RunnerPhase.tutorial) {
        float chace = Random.value;
        if (chace > 0 && chace < 0.9 /*&& !spiderPetActive*/)
          thisHeight = Random.Range(HeightMin, HeightMax);
      }
    }

    obj.transform.position = new Vector3(pos.x, pos.y, transform.position.z);
    obj.SetActive(true);
    obj.GetComponent<PlatformDecor>().Inicialize();

    SpriteRenderer rend = obj.transform.Find("Graphic").GetComponent<SpriteRenderer>();

    if (obj.GetComponent<DecorController>())
      obj.GetComponent<DecorController>().countThis = thisNumber;

    if (rend != null) {
      rend.sortingLayerID = layerId;
    } else {
      DecorController decor = obj.GetComponent<DecorController>();
      if (decor != null)
        decor.SetOrder(0, layerId);
    }

    if (region == RegionType.Forest) {
      GenerateIsland(obj.transform.position);
    }

    lastSpawnPosition = obj.transform.position;
    return obj;
  }

  // Проверка наличияэлемента в массиве
  bool InArray(int[] arr, int val) {
    for (int i = 0; i < arr.Length; i++)
      if (arr[i] == val) return true;

    return false;
  }

  /// <summary>
  /// Время проверки на деактивацию объекта
  /// </summary>
  float timeToCheckDeactive;

  int updateIncrem;
  void Update() {

    spawner.Update();

    //if(GameManager.isSurvival)
    //	platformSurvival.Update();
    ////SurvivalGenerate();
    //else
    //	platformLevel.Update();
  }

  void SurvivalGenerate() {

    if (RunnerController.Instance.thisDistantionPosition > allMaxDistance - 500 && RunnerController.Instance.thisDistantionPosition < allMaxDistance) InfinityIncrement();

    updateIncrem = 0; // Временная заглушка

    while ((lastSpawnPosition.x + platformWidth + breakWidth) <= displayDiff.rightDif(3)) {
      updateIncrem++;
      GeneratePlatform();
      if (updateIncrem > 7) return;
    }

    if (timeToCheckDeactive <= Time.time) {
      timeToCheckDeactive = Time.time + 3;
      //for(int i = 0; i < generateList.Count; i++) {
      //	if(generateList[i].gameObject.activeInHierarchy &&
      //			(generateList[i].position.x < displayDiff.leftDif(4)
      //			|| generateList[i].position.x > displayDiff.rightDif(4))) {
      //		generateList[i].gameObject.SetActive(false);
      //	}
      //}
      for (int i = 0; i < islandList.Count; i++) {
        if (islandList[i].gameObject.activeInHierarchy &&
            (islandList[i].position.x < displayDiff.leftDif(4)
            || islandList[i].position.x > displayDiff.rightDif(4))) {
          islandList[i].gameObject.SetActive(false);
        }
      }
    }
  }

  void GeneratePlatform() {
    obj = null;

    if ((type == PlatformType.bridge | type == PlatformType.bridgeIdle) && bridgeCount >= 0 && !obj) {
      if (generateBreack)
        bridgeCount = 0;

      if (bridgeCount >= 1) {
        obj = Spawn(cd, new Vector2(lastSpawnPosition.x + platformWidth, thisHeight), PlatformType.bridgeIdle);
        bridgeCount--;
      } else if (bridgeCount == 0) {
        obj = Spawn(cd, new Vector2(lastSpawnPosition.x + platformWidth, thisHeight), PlatformType.idle);
        bridgeCount--;
      }
    }

    //if (( type == PlatformType.bridgeWood || type == PlatformType.bridgeWoodIdle ) & bridgeCount >= 0 & !obj) {
    //  if (generateBreack)
    //    bridgeCount = 0;

    //  if (bridgeCount >= 1) {
    //    obj = SpawnPlatform(cd , new Vector2(lastSpawnPoint.x + platformWidth , thisHeight) , PlatformType.bridgeWoodIdle);
    //    bridgeCount--;
    //  } else if (bridgeCount == 0) {
    //    obj = SpawnPlatform(cd , new Vector2(lastSpawnPoint.x + platformWidth , thisHeight) , PlatformType.idle);
    //    bridgeCount--;
    //  }
    //}

    //if (( type == PlatformType.bridgeLog | type == PlatformType.bridgeLogIdle ) & bridgeCount >= 0 & !obj) {
    //  if (generateBreack)
    //    bridgeCount = 0;

    //  if (bridgeCount >= 1) {
    //    obj = SpawnPlatform(cd , new Vector2(lastSpawnPoint.x + platformWidth , thisHeight) , PlatformType.bridgeLogIdle);
    //    bridgeCount--;
    //  } else if (bridgeCount == 0) {
    //    obj = SpawnPlatform(cd , new Vector2(lastSpawnPoint.x + platformWidth , thisHeight) , PlatformType.idle);
    //    bridgeCount--;
    //  }
    //}

    //обрыв
    if (type == PlatformType.breaks & !obj) {
      if ((lastSpawnPosition.x + platformWidth) + breakWidth <= CameraController.Instance.transform.position.x + displayDiff.right * 2f) {
        obj = Spawn(cd, new Vector2(lastSpawnPosition.x + platformWidth + breakWidth, thisHeight), PlatformType.breaksEnd);
        breakWidth = 0;
        //thisBreak = false;
        RunnerController.Instance.mapHeight = obj.transform.position.y;
        OnBreackEnd();
      }
    }

    // Обычный сектор
    if ((type == PlatformType.idle || type == PlatformType.breaksEnd) && !obj) {
      if (!generateBreack)
        obj = Spawn(cd, new Vector2(lastSpawnPosition.x + platformWidth, thisHeight));
      else {
        obj = Spawn(cd, new Vector2(lastSpawnPosition.x + platformWidth, thisHeight), PlatformType.breaks);
        generateBreack = false;
      }
    }

  }

  public void OnBreackEnd() {
    if (breackGenEnd != null)
      breackGenEnd();
  }

  List<int> platformsListGenerate = new List<int>();
  List<float> platformProbability = new List<float>();
  float? nextCalcPeriodDistance = 0;

  void CalcProbability() {
    platformProbability = new List<float>();
    platformsListGenerate = new List<int>();
    nextCalcPeriodDistance = null;

    float sum1 = 0;
    platformProbability.Add(sum1);

    for (int i = 0; i < platforms.Length; i++) {

      bool getsType = false;

      foreach (RegionType mapOne in platforms[i].mapType) {
        if (mapOne == region)
          getsType = true;
      }

      if (getsType == false) continue;

      if (platforms[i].probabili.Count > 0) {

        for (int j = 0; j < platforms[i].probabili.Count; j++) {
          sum1 += platforms[i].probabili[j].probabili;
          platformProbability.Add(sum1);
          platformsListGenerate.Add(i);

          if (j != platforms[i].probabili.Count - 1) {
            if (nextCalcPeriodDistance == null || nextCalcPeriodDistance > platforms[i].probabili[j + 1].distance.min)
              nextCalcPeriodDistance = platforms[i].probabili[j + 1].distance.min;
          }
        }

      } else if (platforms[i].probability > 0) {
        sum1 += platforms[i].probability;
        platformProbability.Add(sum1);
        platformsListGenerate.Add(i);
        if (nextCalcPeriodDistance == null) nextCalcPeriodDistance += 500;
      }
    }

    if (sum1 != 1f) platformProbability = platformProbability.ConvertAll(x => x / sum1);

  }

  float allMaxDistance = 6000;

  /// <summary>
  /// Бесконечный инкремент параметров
  /// </summary>
  void InfinityIncrement() {
    for (int i = 0; i < platforms.Length; i++) {

      if (platforms[i].probabili.Count <= 3) continue;

      if (platforms[i].probabili[platforms[i].probabili.Count - 1].distance.max == allMaxDistance) {

        GeneratePlatformParametrs one = new GeneratePlatformParametrs();
        one.probabili = platforms[i].probabili[platforms[i].probabili.Count - 3].probabili;
        one.distance.min = platforms[i].probabili[platforms[i].probabili.Count - 1].distance.max;
        one.distance.max = platforms[i].probabili[platforms[i].probabili.Count - 1].distance.max + 500;

        GeneratePlatformParametrs two = new GeneratePlatformParametrs();
        two.probabili = platforms[i].probabili[platforms[i].probabili.Count - 2].probabili;
        two.distance.min = platforms[i].probabili[platforms[i].probabili.Count - 3].distance.max + 500;
        two.distance.max = platforms[i].probabili[platforms[i].probabili.Count - 3].distance.max + 1000;

        GeneratePlatformParametrs tre = new GeneratePlatformParametrs();
        tre.probabili = platforms[i].probabili[platforms[i].probabili.Count - 1].probabili;
        tre.distance.min = platforms[i].probabili[platforms[i].probabili.Count - 1].distance.max + 1000;
        tre.distance.max = platforms[i].probabili[platforms[i].probabili.Count - 1].distance.max + 1500;

        platforms[i].probabili.Add(one);
        platforms[i].probabili.Add(two);
        platforms[i].probabili.Add(tre);
      }
    }
    allMaxDistance = allMaxDistance + 1500;
  }

  public void GenerateIsland(Vector3 newTransform) {
    if (Random.value > 0.25f) return;

    GameObject islandInstance = Pooler.GetPooledObject("Island");
    islandInstance.transform.position = newTransform;
    islandInstance.SetActive(true);
    islandList.Add(islandInstance.transform);

  }

  #region Настройки

  public void GetConfig() {

    List<Configuration.Levels.Decoration> itemsData = Config.Instance.config.levels[GameManager.activeLevel].decoration;

    for (int w = 0; w < platforms.Length; w++) {
      platforms[w].probabili.Clear();
    }

    for (int i = 0; i < itemsData.Count; i++) {

      for (int w = 0; w < platforms.Length; w++) {

        //if(platforms[w].category == PlatformCategory.log) {
        //  GeneratePlatformParametrs one = new GeneratePlatformParametrs();
        //  one.distance.min = i * 500;
        //  one.distance.max = (i + 1) * 500;
        //  one.probabili = float.Parse(oneItem["logs"].ToString());
        //  platforms[w].probabili.Add(one);
        //}
        //if(platforms[w].category == PlatformCategory.wood) {
        //  GeneratePlatformParametrs one = new GeneratePlatformParametrs();
        //  one.distance.min = i * 500;
        //  one.distance.max = (i + 1) * 500;
        //  one.probabili = float.Parse(oneItem["woodBridge"].ToString());
        //  platforms[w].probabili.Add(one);
        //}
        if (platforms[w].category == PlatformCategory.destroy) {
          GeneratePlatformParametrs one = new GeneratePlatformParametrs();
          one.distance.min = i * 500;
          one.distance.max = (i + 1) * 500;
          one.probabili = itemsData[i].roadDestroy;
          platforms[w].probabili.Add(one);
        }
        //if(platforms[w].category == PlatformCategory.afterDecor) {
        //  GeneratePlatformParametrs one = new GeneratePlatformParametrs();
        //  one.distance.min = i * 500;
        //  one.distance.max = (i + 1) * 500;
        //  one.probabili = float.Parse(oneItem["roadAfterDecor"].ToString());
        //  platforms[w].probabili.Add(one);
        //}
        if (platforms[w].category == PlatformCategory.decor) {
          GeneratePlatformParametrs one = new GeneratePlatformParametrs();
          one.distance.min = i * 500;
          one.distance.max = (i + 1) * 500;
          one.probabili = itemsData[i].roadDecor;
          platforms[w].probabili.Add(one);
        }

      }

    }

  }

  #endregion
}
