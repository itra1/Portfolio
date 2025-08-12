using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public struct Decor {
  public GameObject prefab;       // Ссылка на префаб
  public int amount;              // Количество платформ, инициализируется в pool
  public float probability;       // Вероятность появления платформы на сцене
  public RegionType[] mapType;      // Блоки, в которых выполняется генерация
  public bool canSpawnAtStart;    // Объект использует при подготовке этапа
  public float width;             // Ширина
  public bool noRepeat;           // Не повторять, тоесть ориентируется по списку повторений
  public FloatSpan sizeDiff;      // Изменение размеры
  public bool isGroup;            // Это группа
  public bool isTop;              // Крепим к верху
}

[System.Serializable]
public enum DecorType { frontBottom, frontTop, middle, back, shipFront, custom, topPlatform }

[System.Serializable]
public struct GenerateDistance {
  public float minDist;
  public float maxDist;
}

// Таблица увеличения дистанции генерации
[System.Serializable]
public struct TableGenerate {
  public float probility;
  public GenerateDistance distanceRun;
  public GenerateDistance generateDistance;
}


public class DecorSpawner: ExEvent.EventBehaviour {

  [Layer]
  public int layerId;
  public int noRepeatItems;
  public int orderSprite;
  private DisplayDiff displayDiff;                            // Смещение видимой части

  public string poolerKey;

  public DecorType typeDecor;
  private RegionType region;

  public List<TableGenerate> generateTable;
  public GenerateDistance generateDistance;
  public int GenerateCount;
  int generateCountThis;

  public float customHeightPosition;

  [SerializeField]
  private int amountAtStart;                 // Количество элементов при старте
  [SerializeField]
  public List<Decor> decors;                    // Массив платформ
  List<KeyValuePair<GameObject, int>> decorList;              // Список преобразованный в массив структур платформ
  List<float> cd;
  List<float> cdAtStart;
  float lastSpawnDistance;
  Transform lastSpawn;
  [SerializeField]
  float decorWidth;
  List<Transform> generateList;

  float diffDist;
  //Для избежания перепрыгивание графики в рамках камеры
  float lastWidth;
  GameObject obj;

  float spawnPositionY;
  float spawnPositionYUp;

  int thisNumber = 0;                 // Счетчик созданных объектов
  List<int> rememberList = new List<int>();                 // Лист запомненных объектов

  public float breackIncrement;
  float thisBreack;

  public int[] numberDistGenerat;

  bool isIpad;
  public FloatSpan distanceGenerate;

  void Start() {
    GetConfig();
    isIpad = GameManager.isIPad;
    generateList = new List<Transform>();

    region = Regions.type;

    RunnerController.petsChange += PetsChange;
    CameraController.recalcPosition += SetPosition;
    GameManager.OnChangeLevel += GetConfig;

    SetPosition();

    // Готовим массив генерации объектов, для контроля повторения
    StructProcessor();
    LevelPooler.Instance.AddPool(poolerKey, decorList, generateList);

    if (typeDecor != DecorType.frontTop) {
      lastSpawn = SpawnAtStart(amountAtStart);
    }

  }

  void SetPosition() {
    displayDiff = CameraController.Instance.CalcDisplayDiff(transform.position.z);

    if (typeDecor == DecorType.custom || typeDecor == DecorType.back)
      spawnPositionY = customHeightPosition;
    else {
      spawnPositionY = CameraController.Instance.transform.position.y + displayDiff.down;
      spawnPositionYUp = CameraController.Instance.transform.position.y + displayDiff.top;
    }
  }

  protected override void OnDestroy() {
    base.OnDestroy();
    RunnerController.petsChange -= PetsChange;
    CameraController.recalcPosition -= SetPosition;
    GameManager.OnChangeLevel -= GetConfig;
  }

  private Player.Jack.PetsTypes _activePet;

  void PetsChange(Player.Jack.PetsTypes pet, bool useFlag, float timeUse) {
    _activePet = useFlag ? Player.Jack.PetsTypes.spider : Player.Jack.PetsTypes.none;
  }

  [ExEvent.ExEventHandler(typeof(ExEvent.RunEvents.RegionChange))]
  void ChangeMap(ExEvent.RunEvents.RegionChange region) {
    this.region = region.newType;
  }

  public void RepoolObjects() {
    if (typeDecor == DecorType.back) {
      Debug.Log("Repooling object " + region);
      LevelPooler.Instance.DeactiveAll();
      lastSpawn = SpawnAtStart(amountAtStart);
    }
  }

  public GameObject GenerateDecoration(Vector3 newPos) {

    if (_activePet == Player.Jack.PetsTypes.spider && typeDecor == DecorType.frontTop)
      return null;

    return Spawn(cd, new Vector3(newPos.x, newPos.y, transform.position.z));
  }

  // Проверка наличияэлемента в массиве
  //bool InArray(int[] arr, int val) {
  //  for (int i = 0; i < arr.Length; i++)
  //    if (arr[i] == val) return true;

  //  return false;
  //}

  void StructProcessor() {
    //Создаем список объектов
    decorList = new List<KeyValuePair<GameObject, int>>();
    cd = new List<float>();
    cdAtStart = new List<float>();

    float sum1 = 0;
    float sum2 = 0;

    cd.Add(sum1);
    cdAtStart.Add(sum2);

    foreach (Decor decor in decors) {
      //Кроме вычисления распределения, создаем список платформ для процесса пулинга одновременно.
      decorList.Add(new KeyValuePair<GameObject, int>(decor.prefab, (decor.amount > 0) ? decor.amount : 1));
      sum1 += decor.probability;
      cd.Add(sum1);

      // Распределение на этапе запуска
      sum2 = decor.canSpawnAtStart ? sum2 + decor.probability : sum2;
      cdAtStart.Add(sum2);
    }

    if (sum1 != 1f)
      cd = cd.ConvertAll(x => x / sum1); //normalize cd, if it's not already normalized

    // Если sum2 расна 0, никакая платформа не имеет преимещество на этапе старта
    if (sum2 == 0f)
      cdAtStart = cd;
    else if (sum2 != 1f)
      cdAtStart = cdAtStart.ConvertAll(x => x / sum2);

  }

  Transform SpawnAtStart(int amount) {

    Vector3 playerGroundPosition = Vector3.zero;

    if (typeDecor == DecorType.frontBottom || typeDecor == DecorType.middle || typeDecor == DecorType.shipFront) {
      playerGroundPosition = new Vector3((GameManager.activeLevelData.moveVector == MoveVector.left ? displayDiff.leftDif(2.5f) : displayDiff.rightDif(2.5f)) * (isIpad ? 1.1f : 1),
        spawnPositionY,
        transform.position.z);
    } else {
      playerGroundPosition = new Vector3((GameManager.activeLevelData.moveVector == MoveVector.left ? displayDiff.rightDif(2.5f) : displayDiff.leftDif(2.5f)) * (isIpad ? 1.1f : 1),
        spawnPositionY,
        transform.position.z);
    }

    GameObject last = Spawn(cdAtStart, playerGroundPosition, -1, true);
    Vector3 lastSpawnPoint = playerGroundPosition;

    for (int i = 0; i < amount - 1; i++) {

      if (typeDecor == DecorType.middle) {
        lastSpawnPoint = new Vector3(lastSpawnPoint.x + (GameManager.activeLevelData.moveVector == MoveVector.left ? -Random.Range(1f, 10f) : Random.Range(1f, 10f)),
          spawnPositionY,
          transform.position.z);
      } else
        lastSpawnPoint = new Vector3(lastSpawnPoint.x + ((lastWidth * (GameManager.activeLevelData.moveVector == MoveVector.left ? -1 : 1)) * (isIpad ? 1.1f : 1)),
          spawnPositionY,
          transform.position.z);

      last = Spawn(cdAtStart, lastSpawnPoint, -1, true);
    }
    return last.transform;
  }

  GameObject Spawn(List<float> cd, Vector3 position, int numb = -1, bool noTop = false) {
    lastSpawnDistance = RunnerController.playerDistantion;

    if (RunnerController.playerDistantion >= nextCalcProbability) {
      CalcPeriod();
    }
    // Инкремент счетчика созданных объектов
    thisNumber++;

    GameObject obj;
    bool chekers = false;
    int cds = 0;
    while (!chekers & numb == -1) {
      cds = BinarySearch.RandomNumberGenerator(cd);

      bool genYes = false;

      foreach (RegionType mapOne in decors[cds].mapType) {
        if (mapOne == region)
          genYes = true;
      }

      if (genYes) {
        if (decors[cds].noRepeat) {

          chekers = !rememberList.Contains(cds);

          //if (!InArray(rememberList, cds)) {
          //  chekers = true;
          //}
        } else
          chekers = true;
      }

      // Запоминаем при необходимости
      if (chekers && noRepeatItems != 0) {

          rememberList[thisNumber % noRepeatItems] = cds;

      }
    }

    int genNum = (numb >= 0) ? numb : cds;

    obj = LevelPooler.Instance.GetPooledObject(poolerKey, genNum, generateList);
    obj.SetActive(true);

    obj.transform.position = new Vector3(position.x, (decors[genNum].isTop ? spawnPositionYUp : spawnPositionY), position.z);

    // Сортировка слоев
    if (!decors[genNum].isGroup && obj.transform.Find("Graphic") != null) {
      SpriteRenderer rend = obj.transform.Find("Graphic").GetComponent<SpriteRenderer>();
      if (rend != null) {
        rend.sortingLayerID = layerId;
        rend.sortingOrder = orderSprite - 2 + thisNumber % 4;
      } else {
        DecorController decor = obj.GetComponent<DecorController>();
        if (decor != null)
          decor.SetOrder(orderSprite - 2 + thisNumber % 4, layerId);
      }

      if (typeDecor == DecorType.frontBottom || typeDecor == DecorType.frontTop) {

        if (rend != null)
          rend.color = new Color(0.5f, 0.5f, 0.5f);
        else {
          if (GetComponent<DecorController>() != null)
            GetComponent<DecorController>().SetColor(new Color(0.5f, 0.5f, 0.5f));
        }

      }
    }

    float scale = Random.Range(decors[genNum].sizeDiff.min, decors[genNum].sizeDiff.max);
    obj.transform.localScale = new Vector3(scale * (isIpad ? 1.1f : 1), scale * (isIpad ? 1.1f : 1), scale * (isIpad ? 1.1f : 1));

    lastWidth = decors[genNum].width;
    return obj;
  }

  /// <summary>
  /// Время проверки на деактивацию объекта
  /// </summary>
  float timeToCheckDeactive;
  void Update() {

    if (timeToCheckDeactive <= Time.time) {
      if (typeDecor == DecorType.back)
        timeToCheckDeactive = Time.time + 20;
      else
        timeToCheckDeactive = Time.time + 3;
      // скрываем объекты, если вышли из зоны видимости
      for (int i = 0; i < generateList.Count; i++) {
        if (generateList[i].gameObject.activeInHierarchy &&
            (generateList[i].position.x < displayDiff.leftDif(3)
            || generateList[i].position.x > displayDiff.rightDif(3))) {
          generateList[i].gameObject.SetActive(false);
        }
      }
    }

    if (generateTable.Count > 3 && generateTable[generateTable.Count - 1].distanceRun.minDist < lastSpawnDistance && generateTable[generateTable.Count - 1].distanceRun.maxDist > lastSpawnDistance)
      InfinityIncrement();

    // Декорации этого типа вызываются извне
    if (typeDecor == DecorType.frontTop) return;

    // Для задника немного отличная генерация
    if (typeDecor == DecorType.back) {

      Vector3 generatePosition = GameManager.activeLevelData.moveVector == MoveVector.left
        ? generatePosition = new Vector3(lastSpawn.position.x - (lastWidth + (isIpad ? 1.1f : 1)), spawnPositionY,
          transform.position.z)
        : generatePosition = new Vector3(lastSpawn.position.x + (lastWidth + (isIpad ? 1.1f : 1)), spawnPositionY,
          transform.position.z);
      
      if ((lastSpawn.position.x + lastWidth) <= (GameManager.activeLevelData.moveVector == MoveVector.left ? displayDiff.leftDif(2.5f) : displayDiff.rightDif(2.5f))) {
        obj = Spawn(cd, generatePosition);
        lastSpawn = obj.transform;
      }
    } else {
      float thisDistantion = RunnerController.Instance.thisDistantionPosition;

      if (lastSpawnDistance + decorDistantionGenerate <= thisDistantion) {

        Vector3 generatePosition;
        if (GameManager.activeLevelData.moveVector == MoveVector.left) {
          generatePosition = new Vector3(displayDiff.leftDif(2.5f), spawnPositionY, transform.position.z);
        } else {
          generatePosition = new Vector3(displayDiff.rightDif(2.5f), spawnPositionY, transform.position.z);
        }

        if (generateCountThis == 0) {
          generateCountThis = Random.Range(1, GenerateCount + 1);
          //lastWidth = GetNextDistantion(thisDistantion);
        } else {
          generateCountThis--;
          if (Random.value <= decorParobabilityGanarate)
            Spawn(cd, generatePosition);
          else
            lastSpawnDistance = RunnerController.Instance.thisDistantionPosition;
        }
      }
    }

  }

  float nextCalcProbability = 0;
  float decorDistantionGenerate = 0;
  float decorParobabilityGanarate = 0;

  /// <summary>
  /// Рассчитываем параметры генерации для следующего периода
  /// </summary>
  void CalcPeriod() {
    for (int i = 0; i < generateTable.Count; i++) {

      if (RunnerController.playerDistantion >= generateTable[i].distanceRun.minDist && RunnerController.playerDistantion < generateTable[i].distanceRun.maxDist) {
        decorDistantionGenerate = Random.Range(distanceGenerate.min, distanceGenerate.max);
        decorParobabilityGanarate = generateTable[i].probility / 100;
        if (i != generateTable.Count - 1)
          nextCalcProbability = generateTable[i + 1].distanceRun.minDist;
        else
          nextCalcProbability = generateTable[i + 1].distanceRun.minDist;
      }
    }

  }

  /// <summary>
  /// Бесконечный инкремент параметров
  /// </summary>
  void InfinityIncrement() {

    TableGenerate one = new TableGenerate();
    one.probility = generateTable[generateTable.Count - 3].probility;
    //one.generateDistance = generateTable[generateTable.Count - 3].generateDistance;
    one.distanceRun.maxDist = generateTable[generateTable.Count - 1].distanceRun.maxDist;
    one.distanceRun.maxDist = generateTable[generateTable.Count - 1].distanceRun.maxDist + 500;

    TableGenerate two = new TableGenerate();
    two.probility = generateTable[generateTable.Count - 2].probility;
    //two.generateDistance = generateTable[generateTable.Count - 2].generateDistance;
    two.distanceRun.maxDist = generateTable[generateTable.Count - 3].distanceRun.maxDist + 500;
    two.distanceRun.maxDist = generateTable[generateTable.Count - 3].distanceRun.maxDist + 1000;

    TableGenerate tre = new TableGenerate();
    tre.probility = generateTable[generateTable.Count - 1].probility;
    //tre.generateDistance = generateTable[generateTable.Count - 1].generateDistance;
    tre.distanceRun.maxDist = generateTable[generateTable.Count - 1].distanceRun.maxDist + 1000;
    tre.distanceRun.maxDist = generateTable[generateTable.Count - 1].distanceRun.maxDist + 1500;

    generateTable.Add(one);
    generateTable.Add(two);
    generateTable.Add(tre);

  }

  #region Настройки

  public string configString;

  public void GetConfig() {

    List<Configuration.Levels.Decoration> itemsData = Config.Instance.config.levels[GameManager.activeLevel].decoration;

    if (System.String.IsNullOrEmpty(configString)) return;

    generateTable.Clear();
    for (int i = 0; i < itemsData.Count; i++) {

      TableGenerate tableGen = new TableGenerate();
      tableGen.distanceRun.minDist = i * 500;
      tableGen.distanceRun.maxDist = (i + 1) * 500;

      switch (configString) {
        case "decor":
          tableGen.probility = itemsData[i].decor;
          break;
        case "decorWall":
          tableGen.probility = itemsData[i].decorWall;
          break;
        case "distantion":
          tableGen.probility = itemsData[i].distantion;
          break;
        case "frontDecor":
          tableGen.probility = itemsData[i].frontDecor;
          break;
        case "roadDecor":
          tableGen.probility = itemsData[i].roadDecor;
          break;
        case "roadDestroy":
          tableGen.probility = itemsData[i].roadDestroy;
          break;
        case "roadAfterDecor":
          tableGen.probility = itemsData[i].roadAfterDecor;
          break;
        case "wallBreack":
          tableGen.probility = itemsData[i].wallBreack;
          break;
        case "wallDestroy":
          tableGen.probility = itemsData[i].wallDestroy;
          break;
        case "wallDestroyMini":
          tableGen.probility = itemsData[i].wallDestroyMini;
          break;
      }

      generateTable.Add(tableGen);
    }

  }

  #endregion

}
