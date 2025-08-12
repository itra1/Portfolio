using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformSurvivalSpawn: PlatformSpawn {
  
  public FloatSpan height;
  public FloatSpan bridgeLenght;
  public FloatSpan breackLenght;

  public int noRepeatItems;

  public RegionType region;
  private PlatformType type;

  public List<Platform> platforms;

  private List<Transform> decorList = new List<Transform>();
  private List<int> rememberList = new List<int>();

  private int number = 0;

  [HideInInspector]
  public float actualHeight;

  private float breackeDistance;
  private bool isBreacke;

  private bool isStopGenerate;
  private List<PlatformMaterial> materialList = new List<PlatformMaterial>();   // Вероятность генерации платформ по материалу
  private List<float> materialCD = new List<float>();                           // Вероятность генерации платформ по материалу
  private List<PlatformCategory> categoryList = new List<PlatformCategory>();   // Вероятность генерации платформ по категории
  private List<float> categoryCD = new List<float>();                           // Вероятность генерации платформ по категории
  private List<PlatformType> typeList = new List<PlatformType>();               // Вероятность генерации платформ по категории
  private List<float> typeCD = new List<float>();                               // Вероятность генерации платформ по категории
  private float timeToCheckDeactive;                                            // Время для новой проверки
  private List<Transform> generateList = new List<Transform>();                 // Лист сгенерированных объектов
  private float breakWidth;                                                     // Размер ямы
  private PlatformType lastType;                                                // Последний сгенерированный тип
  private int bridgeCount = 0;                                                  // Секция в построении моста

  public float platformWidth { get { return 2.5f; } }

  /// <summary>
	/// Параметры дистанции
	/// </summary>
	[System.Serializable]
  public struct GenerateParametrs {
    public float distantion;
    public int count;
    public List<PlatformMaterials> material;
    public List<PlatformCategories> category;
  }

  /// <summary>
  /// Вероятность генерации
  /// </summary>
  [System.Serializable]
  public struct PlatformMaterials {
    public PlatformMaterial type;
    public float value;
  }

  /// <summary>
  /// Вероятность генерации
  /// </summary>
  [System.Serializable]
  public struct PlatformCategories {
    public PlatformCategory type;
    public float value;
  }


  private float distantionCalc = 0;
  public List<GenerateParametrs> generateParametrs = new List<GenerateParametrs>();

  protected override void Start() {
    base.Start();
    displayDiff = CameraController.Instance.CalcDisplayDiff(transform.position.z);

    RunSpawner.GenerateBreackEvent += GenerateBreack;
    HandingPlatform.generateHendingPlatform += GenerateHendingPlatform;
    GameManager.OnChangeLevel += GetConfig;

    actualHeight = height.min;
    RunnerController.Instance.mapHeight = actualHeight;
    rememberList.Clear();

    ChangeMap(new ExEvent.RunEvents.RegionChange(Regions.type));

    List<KeyValuePair<GameObject, int>> prefList = new List<KeyValuePair<GameObject, int>>() {
      new KeyValuePair<GameObject, int>(stonePlatformPrefab ,20),
      new KeyValuePair<GameObject, int>(woodPlatformPrefab, 10),
      new KeyValuePair<GameObject, int>(logPlatformPrefab, 10)
    };
    LevelPooler.Instance.AddPool(POOL_KEY, LevelPooler.Instance.CreateStructure(prefList, out cd));

    type = PlatformType.idle;
    GetConfig();
    TypeProbability();
  }

  protected override void OnDestroy() {
    base.OnDestroy();

    RunSpawner.GenerateBreackEvent -= GenerateBreack;
    HandingPlatform.generateHendingPlatform -= GenerateHendingPlatform;
    GameManager.OnChangeLevel -= GetConfig;

  }

  private void GenerateBreack(float dist) {
    breackeDistance = dist;
    isBreacke = true;
  }

  void GenerateHendingPlatform(bool isActive, int type) {
    if (isActive && (type == 3 || type == 5)) {
      isStopGenerate = true;
      GenerateBreack(999999);
    } else if (!isActive && isStopGenerate) {
      isStopGenerate = false;
      //breakWidth = 0;

      Spawn(new Vector2(displayDiff.rightDif(3), actualHeight), PlatformType.breaksEnd);
    }
  }

  public void GenerateIsland(Vector3 targetPosition, float pobability = 0.25f) {
    if (Random.value > pobability) return;

    GameObject instant = Pooler.GetPooledObject("Island");
    instant.transform.position = targetPosition;
    instant.SetActive(true);
    decorList.Add(instant.transform);

  }

  protected override void Update() {
    base.Update();

    if (distantionCalc <= RunnerController.playerDistantion) {
      GenerateParametrs newGeneration = generateParametrs.Find(x => x.distantion > distantionCalc);
      //GenerateParametrs generateActive = generateParametrs.Find(x => x.distantion == distantionCalc);
      CalcProbability(newGeneration);
      distantionCalc = newGeneration.distantion;
    }
    Generate();
    DestroyOld();

  }

  private void DestroyOld() {
    if (timeToCheckDeactive > Time.time) return;
    timeToCheckDeactive = Time.time + 10;
    generateList.ForEach(CheckOut);
  }

  private void CheckOut(Transform one) {
    if (!one.gameObject.activeInHierarchy) return;
    if (one.position.x < displayDiff.leftDif(4)) one.gameObject.SetActive(false);
  }

  void Generate() {
    while ((PlatformSpawn.lastSpawnPosition.x + platformWidth + breakWidth) <= displayDiff.rightDif(3)) {
      GeneratePlatforms();
    }
  }

  void GeneratePlatforms() {
    GameObject obj = null;

    if ((lastType == PlatformType.bridge | lastType == PlatformType.bridgeIdle) && bridgeCount >= 0 && !obj) {
      if (isBreacke)
        bridgeCount = 0;

      if (bridgeCount >= 1) {
        obj = Spawn(new Vector2(PlatformSpawn.lastSpawnPosition.x + platformWidth, actualHeight), PlatformType.bridgeIdle);
        bridgeCount--;
      } else if (bridgeCount == 0) {
        obj = Spawn(new Vector2(PlatformSpawn.lastSpawnPosition.x + platformWidth, actualHeight), PlatformType.idle);
        bridgeCount--;
      }
    }

    //обрыв
    if (lastType == PlatformType.breaks & !obj) {
      if ((PlatformSpawn.lastSpawnPosition.x + platformWidth) + breakWidth <= displayDiff.rightDif(3)) {
        obj = Spawn(new Vector3(PlatformSpawn.lastSpawnPosition.x + platformWidth + breakWidth, actualHeight), PlatformType.breaksEnd);
        breakWidth = 0;
        //thisBreak = false;
        RunnerController.Instance.mapHeight = obj.transform.position.y;
        OnBreackEnd();
      }
    }

    // Обычный сектор
    if ((lastType == PlatformType.idle || lastType == PlatformType.breaksEnd) && !obj) {
      if (!isBreacke)
        obj = Spawn(new Vector3(PlatformSpawn.lastSpawnPosition.x + platformWidth, actualHeight));
      else {
        obj = Spawn(new Vector3(PlatformSpawn.lastSpawnPosition.x + platformWidth, actualHeight), PlatformType.breaks);
        isBreacke = false;
      }
    }
  }

  public override GameObject Spawn(Vector3 pos, PlatformType platformType = PlatformType.none) {

    // Инкремент счетчика созданных объектов
    number++;

    PlatformMaterial needMath = materialList[BinarySearch.RandomNumberGenerator(materialCD)];

    string prefabName = "";

    switch (needMath) {
      case PlatformMaterial.stone:
        prefabName = stonePlatformPrefab.name;
        break;
      case PlatformMaterial.log:
        prefabName = logPlatformPrefab.name;
        break;
      case PlatformMaterial.wood:
        prefabName = stonePlatformPrefab.name;
        break;
    }

    PlatformCategory needCat = categoryList[BinarySearch.RandomNumberGenerator(categoryCD)];

    if (platformType == PlatformType.none)
      platformType = typeList[BinarySearch.RandomNumberGenerator(typeCD)];

    if (platformType == PlatformType.bridge || platformType == PlatformType.bridgeIdle || platformType == PlatformType.breaks || platformType == PlatformType.breaksEnd)
      needCat = PlatformCategory.idle;


    lastType = platformType;
    GameObject obj = LevelPooler.Instance.GetPooledObject(POOL_KEY, prefabName, generateList);

    obj.GetComponent<PlatformDecor>().Init(platformType, needCat);

    if (lastType == PlatformType.bridge)
      bridgeCount = Random.Range((int)bridgeLenght.min, (int)bridgeLenght.max);

    if (lastType == PlatformType.breaks) {
      obj.tag = "jumpUp";
      if (breackeDistance > 0) {
        breakWidth = breackeDistance;
        breackeDistance = 0;
      } else
        breakWidth = Random.Range(breackLenght.min, breackLenght.max);

      // Изменение высоты платформы не допускается во время тутора
      if (RunnerController.Instance.runnerPhase != RunnerPhase.tutorial) {
        float chace = Random.value;
        if (chace > 0 && chace < 0.9 /*&& !spiderPetActive*/)
          actualHeight = Random.Range(height.min, height.max);
      }
    }

    obj.transform.position = new Vector3(pos.x, pos.y, transform.position.z);
    PlatformSpawn.lastSpawnPosition = obj.transform.position;
    obj.SetActive(true);
    obj.GetComponent<PlatformDecor>().Inicialize();

    if (lastType != PlatformType.breaks) {
      if (lastType == PlatformType.breaksEnd) {
        breakWidth = 0;
        obj.tag = "jumpDown";
      } else {
        obj.tag = "Untagged";
      }
    }

    SpriteRenderer rend = obj.transform.Find("Graphic").GetComponent<SpriteRenderer>();

    if (obj.GetComponent<PlatformDecor>())
      obj.GetComponent<PlatformDecor>().countThis = number;

    if (rend != null) {
      rend.sortingLayerID = layerId;
    } else {
      PlatformDecor decor = obj.GetComponent<PlatformDecor>();
      if (decor != null)
        decor.SetOrder(0, layerId);
    }

    if (region == RegionType.Forest) {
      GenerateIsland(obj.transform.position);
    }

    PlatformSpawn.lastSpawnPosition = obj.transform.position;
    return obj;
  }

  void TypeProbability() {
    typeList.Clear();
    typeCD.Clear();

    float sum = 0;
    typeCD.Add(sum);

    sum += 1;
    typeCD.Add(sum);
    typeList.Add(PlatformType.idle);

    sum += 0.01f;
    typeCD.Add(sum);
    typeList.Add(PlatformType.bridge);

    typeCD = typeCD.ConvertAll(x => x / sum);

  }

  /// <summary>
  /// Получение настроек
  /// </summary>
  public void GetConfig() {

    List<Configuration.Survivles.Decoration> genParam = Config.Instance.config.survival.decoration;

    generateParametrs = new List<GenerateParametrs>();

    float platformMaterialSum = 1;

    for (int i = 0; i < genParam.Count; i++) {

      GenerateParametrs newParam = new GenerateParametrs();
      newParam.material = new List<PlatformMaterials>();
      newParam.category = new List<PlatformCategories>();
      platformMaterialSum = 1;

      //newParam.distantion = float.Parse(oneItem["distantion"].ToString());
      newParam.distantion = i * 500 + 500;
      newParam.count = 0;

      if (genParam[i].road > 0) {
        PlatformMaterials box = new PlatformMaterials();
        box.type = PlatformMaterial.stone;
        box.value = genParam[i].road * 0.01f;
        newParam.material.Add(box);
      }

      if (genParam[i].woodBridge > 0) {
        PlatformMaterials box = new PlatformMaterials();
        box.type = PlatformMaterial.wood;
        box.value = genParam[i].woodBridge * 0.01f;
        newParam.material.Add(box);
      }

      if (genParam[i].logs > 0) {
        PlatformMaterials box = new PlatformMaterials();
        box.type = PlatformMaterial.log;
        box.value = genParam[i].logs * 0.01f;
        newParam.material.Add(box);
      }

      if (genParam[i].roadDecor > 0) {
        PlatformCategories box = new PlatformCategories();
        box.type = PlatformCategory.decor;
        box.value = genParam[i].roadDecor * 0.01f;
        platformMaterialSum -= box.value;
        newParam.category.Add(box);
      }

      if (genParam[i].roadDestroy > 0) {
        PlatformCategories box = new PlatformCategories();
        box.type = PlatformCategory.destroy;
        box.value = genParam[i].roadDestroy * 0.01f;
        platformMaterialSum -= box.value;
        newParam.category.Add(box);
      }

      if (platformMaterialSum > 0) {
        PlatformCategories box = new PlatformCategories();
        box.type = PlatformCategory.idle;
        box.value = platformMaterialSum;
        newParam.category.Add(box);
      }

      generateParametrs.Add(newParam);
    }

  }

  //public void GetConfig() {

  //  List<Configuration.Levels.Decoration> itemsData = Config.Instance.config.levels[GameManager.activeLevel].decoration;

  //  for (int w = 0; w < platforms.Count; w++) {
  //    platforms[w].probabili.Clear();
  //  }

  //  for (int i = 0; i < itemsData.Count; i++) {

  //    for (int w = 0; w < platforms.Count; w++) {

  //      //if(platforms[w].category == PlatformCategory.log) {
  //      //  GeneratePlatformParametrs one = new GeneratePlatformParametrs();
  //      //  one.distance.min = i * 500;
  //      //  one.distance.max = (i + 1) * 500;
  //      //  one.probabili = float.Parse(oneItem["logs"].ToString());
  //      //  platforms[w].probabili.Add(one);
  //      //}
  //      //if(platforms[w].category == PlatformCategory.wood) {
  //      //  GeneratePlatformParametrs one = new GeneratePlatformParametrs();
  //      //  one.distance.min = i * 500;
  //      //  one.distance.max = (i + 1) * 500;
  //      //  one.probabili = float.Parse(oneItem["woodBridge"].ToString());
  //      //  platforms[w].probabili.Add(one);
  //      //}
  //      if (platforms[w].category == PlatformCategory.destroy) {
  //        GeneratePlatformParametrs one = new GeneratePlatformParametrs();
  //        one.distance.min = i * 500;
  //        one.distance.max = (i + 1) * 500;
  //        one.probabili = itemsData[i].roadDestroy;
  //        platforms[w].probabili.Add(one);
  //      }
  //      //if(platforms[w].category == PlatformCategory.afterDecor) {
  //      //  GeneratePlatformParametrs one = new GeneratePlatformParametrs();
  //      //  one.distance.min = i * 500;
  //      //  one.distance.max = (i + 1) * 500;
  //      //  one.probabili = float.Parse(oneItem["roadAfterDecor"].ToString());
  //      //  platforms[w].probabili.Add(one);
  //      //}
  //      if (platforms[w].category == PlatformCategory.decor) {
  //        GeneratePlatformParametrs one = new GeneratePlatformParametrs();
  //        one.distance.min = i * 500;
  //        one.distance.max = (i + 1) * 500;
  //        one.probabili = itemsData[i].roadDecor;
  //        platforms[w].probabili.Add(one);
  //      }

  //    }

  //  }

  //}

  [ExEvent.ExEventHandler(typeof(ExEvent.RunEvents.RegionChange))]
  void ChangeMap(ExEvent.RunEvents.RegionChange region) {
    this.region = region.newType;
  }

  void CalcProbability(GenerateParametrs parametrs) {
    materialCD.Clear();
    materialList.Clear();
    categoryCD.Clear();
    categoryList.Clear();

    float sum1 = 0;
    materialCD.Add(sum1);

    foreach (PlatformMaterials mat in parametrs.material) {
      sum1 += mat.value;
      materialCD.Add(sum1);
      materialList.Add(mat.type);
    }

    float sum2 = 0;
    categoryCD.Add(sum2);

    foreach (PlatformCategories cat in parametrs.category) {
      sum2 += cat.value;
      categoryCD.Add(sum2);
      categoryList.Add(cat.type);
    }

    materialCD = materialCD.ConvertAll(x => x / sum1);
    categoryCD = categoryCD.ConvertAll(x => x / sum2);

  }

  public void OnBreackEnd() {
    if (breackGenEnd != null)
      breackGenEnd();
  }

  [ContextMenu("Clone platforms")]
  public void ClonePlatforms() {

    PlatformSpawner source = FindObjectOfType<PlatformSpawner>();

    this.platforms = new List<Platform>(source.platforms);

  }

}
