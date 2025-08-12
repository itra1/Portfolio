using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using Player.Jack;


[System.Serializable]
public struct FriendBest {
  public GameObject pref;
  public string fb;
  public string firstName;
  public string lastName;
  public int dist;
  public string avaUrl;
  public Texture ava;
}
[System.Serializable]
public struct SpecialBarriersProbability {
  public SpecialBarriersTypes type;
  public float shareProbability;
}
[System.Serializable]
public struct SpecialBarrierGenerateParam {
  public FloatSpan distance;
  [Range(0, 1)]
  public float probability;
  public SpecialBarriersProbability[] types;
  public bool generate;
}
[System.Serializable]
public struct barrierProbability {
  public barrierGenerateTypes type;
  public float value;
}
[System.Serializable]
public struct BarrierGenerateParam {
  public float distance;
  [Range(0, 1)]
  public float probility;
  public int count;
  public List<barrierProbability> probability;
}
[System.Serializable]
public struct GroupPrefabs {
  public barrierGenerateTypes type;
  public List<GameObject> prefab;
}

/// <summary>
/// Генератор ландшафта в забеге
/// </summary>
public class RunSpawner: Singleton<RunSpawner> {

  private float runnerDistantion;                                         // Текущая дистанция по счетчику
  public LayerMask graundMask;
  [HideInInspector]
  public RunnerPhase runnerPhase;
  private int bestDist;                                                       // Лучшая дистанция игрока

  private Action GenerateAction;

  public const string POOLER_KEY = "runSpawner"; 

  public static event EventAction<MapObject> OnGenerateMapObject;

  public ObjectsLevel objectLevels;

  float _lastGenerate;        // Время последней генерации
  bool _lockedGenerate;
  public bool lockedGenerate {
    get { return _lockedGenerate; }
    set {
      _lockedGenerate = value;
      if (!_lockedGenerate) _lastGenerate = RunnerController.playerDistantion;
    }
  }
  public float lastGenerate {
    get { return _lastGenerate; }
    set { _lastGenerate = value; }
  }

  public bool CheckGenerate(float dist = 0) {
    return !_lockedGenerate && (_lastGenerate + dist <= RunnerController.playerDistantion);
  }

  void Start() {
    if (GameManager.activeLevelData.gameMode == GameMode.none) return;
    StartBase();
  }

  private void StartBase() {

    GenerateAction = StartGenerate;

    GetConfig();
    CoinsSpawnInit();

    RunnerController.OnChangeRunnerPhase += ChangePhase;
    RunnerController.petsChange += PetsChange;
    PlayerController.MagnetEvent += MagnitEvent;
    GameManager.OnChangeLevel += GetConfig;

    if (GameManager.activeLevelData.gameMode != GameMode.survival) objectLevels.Init(this);

    bestDist = (int)UserManager.Instance.survivleMaxRunDistance;
    gateNumOpen = PlayerPrefs.GetInt("openGate");

    if (gateGenerate || GameManager.Instance.generateGate) InitGate();

    StructProcessor();
    LevelPooler.Instance .AddPool(POOLER_KEY, barrierList);
    UpdateValue();
  }

  protected override void OnDestroy() {
    base.OnDestroy();
    RunnerController.OnChangeRunnerPhase -= ChangePhase;
    RunnerController.petsChange -= PetsChange;
    PlayerController.MagnetEvent -= MagnitEvent;
    GameManager.OnChangeLevel -= GetConfig;
  }

  private void Update() {
    if (GameManager.activeLevelData.gameMode == GameMode.none) return;
    UpdateBase();
  }

  private void UpdateBase() {

    runnerDistantion = RunnerController.playerDistantion;

    UpdateAudio();

    //if (GameManager.activeLevelData.gameMode != GameMode.survival) return;

    if (GameManager.activeLevelData.gameMode == GameMode.survival) {
      GenerateRun();
    }

    if (GameManager.activeLevelData.gameMode == GameMode.survival) {
      if (FbLogin)
        GenFriends();
      else
        GetMy();
    }

    CounsGenerateUpdate();

    // Факелы и т.п.
    if (runnerPhase == RunnerPhase.run && GameManager.activeLevelData.gameMode != GameMode.survival)
      objectLevels.Update();

    //GenerateAction();

  }

  void StartGenerate() { }

  void RunGenerate() { }

  void BoostGenerate() { }

  #region Изменение состояний
  
  /// <summary>
  /// Изменение фазы забега
  /// </summary>
  /// <param name="newPhase"></param>
  void ChangePhase(RunnerPhase newPhase) {

    // Определяем дистанцию старта
    if ((newPhase == RunnerPhase.run || newPhase == RunnerPhase.boost) && !friendsInicialization) {

      friendsInicialization = true;

      InitAudio();

      if (allfriends == null) {
        FbLogin = FBController.CheckFbLogin;
        if (FbLogin) {
          FillFriendPool();
          SetNewFriendPool();
        } else {
          SetMePool();
        }
      }

      // if(barrier != null)
      //barrier.CalcNextGenerate();

      barrierGenerateReady = true;
      nextGenerateCoins = runnerDistantion + Random.Range(coinsDistanceGenerate.min, coinsDistanceGenerate.max);
      coinsGenerateReady = true;
    }

    if (newPhase == RunnerPhase.run) InitAudio();

    runnerPhase = newPhase;

  }

  #endregion

  #region Действия комплектов

  public ClothesBonus defendbarrierClothes;
  public ClothesBonus goldClothes;
  public ClothesBonus beackClothes;
  public ClothesBonus hendingClothes;

  void UpdateValue() {
    defendbarrierClothes = Config.GetActiveCloth(ClothesSets.defendBarrier);
    goldClothes = Config.GetActiveCloth(ClothesSets.money);
    beackClothes = Config.GetActiveCloth(ClothesSets.noBreack);
    hendingClothes = Config.GetActiveCloth(ClothesSets.noHendingBarrier);
  }

  #endregion

  #region Препядствия
  float nextGenerate;                                                 // Следующая генерация
  barrierGenerateTypes nextType;                                      // Следуюй тип генерации
  //public FloatSpan barrierDistanceGenerate;                           // Дистанции генерации преград
  //public List<BarrierGenerateParam> barrierParam;                     // Параметры генерации
  bool barrierGenerateReady;
  bool noBarrier;
  float nextProbility;
  //int ghostCount;
  [HideInInspector]
  public float generateRightBorder { get { return CameraController.displayDiff.rightDif(2.5f); } }
  //public delegate void dBreackGenerate(float dist);
  public static event Action<float> GenerateBreackEvent;
  public List<GroupPrefabs> barrierPref;                              // Набор объектов для пулинга
  List<KeyValuePair<GameObject, int>> barrierList;                    // Массив сгенерированных объектов
  /// <summary>
  /// Генерация препядствия
  /// </summary>
  /// <param name="needDistantion"></param>
  public static void GenerateBreackNow(float needDistantion) {
    if (GenerateBreackEvent != null) GenerateBreackEvent(needDistantion);
  }
  /// <summary>
  /// Генерация при бусте
  /// </summary>
  void GenerateBoost() {
    if (coinsGenerateReady && nextGenerateCoins < runnerDistantion) {
      if (Random.value <= 0.3f)
        OnGenerateCoins(new Vector3(CameraController.displayDiff.rightDif(2.5f), RunnerController.Instance.mapHeight + Random.Range(1f, 2.5f), transform.position.z));
      CalcNextSpawnCoins();
    }

    if (gateGenerate || (GameManager.Instance.generateGate && GameManager.Instance.allKeys + RunnerController.Instance.keysInRaceCalc >= GameManager.Instance.needKeys)) GenerateGate();
  }

  bool groupGenerate;

  // Генерация при простом режиме
  void GenerateRun() {

    if ((gateGenerate || (GameManager.Instance.generateGate && GameManager.Instance.allKeys + RunnerController.Instance.keysInRaceCalc >= GameManager.Instance.needKeys)) && GenerateGate()) {
      CalcNextSpawnCoins();
      BarrierSpawner.Instance.CalcNextGenerate();
      return;
    }

    if (nextGenerate <= runnerDistantion && barrierGenerateReady) {
      if ((nextProbility >= Random.value || groupGenerate) && !(beackClothes.full && nextType == barrierGenerateTypes.breack))
        Debug.Log("Survival");
      //GenerateBarrierSurvival();
      else
        noBarrier = true;
    }

    if (nextGenerateCoins <= runnerDistantion && RunnerController.Instance.generateItems && coinsGenerateReady && !lockedGenerate) {
      if ((activeMagnet || Random.value <= (0.3f + (goldClothes.head ? 0.25f : 0) + (goldClothes.spine ? 0.25f : 0) + (goldClothes.accessory ? 0.25f : 0))) && lastCoinsGenerate + 15 <= runnerDistantion) {
        OnGenerateCoins(new Vector3(generateRightBorder, RunnerController.Instance.mapHeight + Random.Range(1f, 2.5f), 0));
        lastCoinsGenerate = runnerDistantion;
      }
      coinsGenerateReady = false;
    }

    if (noBarrier) {
      noBarrier = false;
      barrierGenerateReady = false;
    }

    // Если сгенерировали преграду или сгенерировали блок монет, рассчитываем слудеющее появление
    if (!coinsGenerateReady || lockedGenerate)
      CalcNextSpawnCoins();

    if (!barrierGenerateReady) {
      BarrierSpawner.Instance.CalcNextGenerate();
    }
  }


  /// <summary>
  /// Подготовка структуры под генерацию
  /// </summary>
  void StructProcessor() {
    //Создаем список объектов
    barrierList = new List<KeyValuePair<GameObject, int>>();

    foreach (GroupPrefabs obj1 in barrierPref) {
      foreach (GameObject barrier in obj1.prefab) {
        barrierList.Add(new KeyValuePair<GameObject, int>(barrier, 1));
      }
    }
  }

  //int GetStartNumber(barrierGenerateTypes barrType) {
  //	int count = 0;

  //	foreach (GroupPrefabs obj1 in barrierPref) {
  //		if (obj1.type != barrType) {
  //			count += obj1.prefab.Count;
  //		} else {
  //			return count;
  //		}
  //	}
  //	return -1;
  //}

  //int GetThisNumber(barrierGenerateTypes barrType) {
  //	int count = 0;

  //	foreach (GroupPrefabs obj1 in barrierPref) {
  //		if (obj1.type != barrType) {
  //			count++;
  //		} else {
  //			return count;
  //		}
  //	}

  //	return -1;
  //}

  //void CalcNextGenerate() {

  //	barrier.CalcNextGenerate();

  //	if (GameManager.gameMode == GameMode.survival)
  //		CalcNextGenerateOld();
  //	else
  //		CalcNextGenerateNew();
  //}

  /// <summary>
  /// Генерация 
  /// </summary>
  //void CalcNextGenerateOld() {
  //	nextGenerate = runnerDistantion + Random.Range(barrierDistanceGenerate.min, barrierDistanceGenerate.max);
  //	nextType = GetRandType();
  //	barrierGenerateReady = true;
  //}

  //void InitBarrier() {
  //	barrierLastDistanceGroup = 0;
  //	barrierNextDistanceGroup = 0;
  //	barrierCountInGroup = 0;
  //	fastGenerateDistance = 0;
  //}

  //float barrierLastDistanceGroup;           // Последняя дистанция блока
  //float barrierNextDistanceGroup;           // Новое окончание дистанции
  //int barrierCountInGroup;
  int barrierCountExists;
  float barrierDistanceInGroup;
  //float fastGenerateDistance;
  //void CalcNextGenerateNew() {

  //	if (barrierNextDistanceGroup <= runnerDistantion) {
  //		barrierLastDistanceGroup = barrierNextDistanceGroup;
  //		fastGenerateDistance = barrierLastDistanceGroup;
  //		for (int i = 0; i < barrierParam.Count; i++) {
  //			if ((i > 0 && barrierParam[i].distance > runnerDistantion && barrierParam[i - 1].distance < runnerDistantion)
  //				|| (i == 0 && barrierParam[i].distance > runnerDistantion)) {
  //				barrierNextDistanceGroup = barrierParam[i].distance;
  //				barrierCountInGroup = barrierParam[i].count;
  //				barrierCountExists = barrierCountInGroup;
  //			}
  //		}
  //	}

  //	if (barrierCountExists <= 0) {
  //		return;
  //	}

  //	nextType = GetRandType();
  //	barrierGenerateReady = true;
  //	barrierDistanceInGroup = ((barrierNextDistanceGroup - barrierLastDistanceGroup) - 20) / barrierCountInGroup;
  //	nextGenerate = fastGenerateDistance + Random.Range(10f, barrierDistanceInGroup);
  //	fastGenerateDistance = nextGenerate;
  //	barrierCountExists--;
  //}

  #region Генерация монет
  public FloatSpan coinsDistanceGenerate;                             // Дистанции генерации преград
  [HideInInspector]
  public bool coinsGenerateReady;
  float nextGenerateCoins;
  //public static event Action<Vector3,int> OnCoinsGenerate;
  public float lastCoinsGenerate;

  void CoinsSpawnInit() {
    CalcNextSpawnCoins();
  }

  public void CalcNextSpawnCoins() {
    nextGenerateCoins = runnerDistantion + Random.Range(coinsDistanceGenerate.min, coinsDistanceGenerate.max);
    coinsGenerateReady = true;
  }

  public void OnGenerateCoins(Vector3 position, int barrierType = 0) {
    ExEvent.RunEvents.CoinsGenerate.Call(position, barrierType);
  }

  void CounsGenerateUpdate() {
    //Debug.Log(nextGenerateCoins);
    if (nextGenerateCoins < RunnerController.playerDistantion && CheckGenerate(10)) {
      if (activeMagnet || Random.value <= (0.3f + (goldClothes.head ? 0.25f : 0) + (goldClothes.spine ? 0.25f : 0) + (goldClothes.accessory ? 0.25f : 0))) {
        ExEvent.RunEvents.CoinsGenerate.Call(new Vector2(generateRightBorder, RunnerController.Instance.mapHeight + Random.Range(1f, 2.5f)));
        lastGenerate = RunnerController.playerDistantion;
      }
      CalcNextSpawnCoins();
    }
  }

  #endregion

  //barrierGenerateTypes GetRandType() {
  //	cd.Clear();
  //	for (int i = 0; i < barrierParam.Count; i++) {
  //		if (
  //			(GameManager.gameMode == GameMode.survival && (nextGenerate >= barrierParam[i].distance && (i == barrierParam.Count - 1 || nextGenerate < barrierParam[i + 1].distance)))
  //			|| (GameManager.gameMode != GameMode.survival && (nextGenerate <= barrierParam[i].distance))
  //			) {
  //			float sum = 0;
  //			if (GameManager.gameMode == GameMode.survival)
  //				nextProbility = barrierParam[i].probility;
  //			else
  //				nextProbility = 1;
  //			cd.Add(sum);

  //			if (barrierParam[i].probability.Count == 0) {
  //				nextGenerate = barrierParam[i].distance;
  //			}

  //			foreach (barrierProbability proba in barrierParam[i].probability) {

  //				if (beackClothes.full && proba.type == barrierGenerateTypes.breack) {
  //					sum += 0;
  //				} else if (hendingClothes.full && barrierGenerateTypes.hending == proba.type) {
  //					sum += 0;
  //				} else if (defendbarrierClothes.full
  //								&& System.Array.IndexOf(new barrierGenerateTypes[] { barrierGenerateTypes.stone,
  //																																			barrierGenerateTypes.stoneAndHending,
  //																																			barrierGenerateTypes.stoneDouble,
  //																																			barrierGenerateTypes.stoneSkelet,
  //																																			barrierGenerateTypes.stoneSkeletDouble}, proba.type) >= 0) {
  //					sum += proba.value * 0.5f;
  //				} else {
  //					sum += proba.value;
  //				}
  //				cd.Add(sum);
  //			}
  //			if (sum != 1f) cd = cd.ConvertAll(x => x / sum);

  //			return barrierParam[i].probability[BinarySearch.RandomNumberGenerator(cd)].type;
  //		}
  //	}
  //	return barrierGenerateTypes.breack;
  //}
  //void GenerateBarrierSurvival() {

  //	if (!RunnerController.instance.generateItems) return;

  //	groupGenerate = false;

  //	if (activePet != PlayerPetsTypes.none && (nextType == barrierGenerateTypes.hending || nextType == barrierGenerateTypes.stoneAndHending))
  //		nextType = barrierGenerateTypes.stone;

  //	// Яма
  //	if (nextType == barrierGenerateTypes.breack) {

  //		if (GenerateBreackEvent != null) {
  //			GenerateBreackEvent(0);
  //		}
  //		barrierGenerateReady = false;
  //	}

  //	// Висячий предмет
  //	if (nextType == barrierGenerateTypes.hending) {
  //		if (Random.value <= 0.5f) {
  //			GameObject obj = Instantiate(barrierPref[1].prefab[Random.Range(0, barrierPref[2].prefab.Count)], new Vector3(generateRightBorder, RunnerController.instance.thisMapHeight + Random.Range(2.5f, 3f), transform.position.z), Quaternion.identity) as GameObject;
  //			obj.transform.parent = barriersFolder.transform.parent;
  //			obj.SetActive(true);
  //		} else {
  //			int generateNumber = GetThisNumber(nextType);
  //			int startNumb = GetStartNumber(nextType);

  //			GameObject obj = pool.getPooledObject(Random.Range(startNumb, startNumb + barrierPref[generateNumber].prefab.Count));
  //			obj.transform.position = new Vector3(generateRightBorder, RunnerController.instance.thisMapHeight + Random.Range(2.5f, 3f), transform.position.z);
  //			obj.SetActive(true);
  //		}

  //		if (coinsGenerateReady && Random.value <= 0.4f && lastCoinsGenerate + 15 <= runnerDistantion) {
  //			if (GenerateCoinsGroupEvent != null) GenerateCoinsGroupEvent(new Vector3(generateRightBorder, RunnerController.instance.thisMapHeight + Random.Range(1f, 2.5f), transform.position.z), 2);
  //			lastCoinsGenerate = runnerDistantion;
  //			coinsGenerateReady = false;
  //		}

  //		barrierGenerateReady = false;
  //	}

  //	// Камень / Двой камень / Камень с висячим предметом
  //	if (nextType == barrierGenerateTypes.stone || nextType == barrierGenerateTypes.stoneDouble || nextType == barrierGenerateTypes.stoneAndHending) {
  //		int generateNumber = GetThisNumber(barrierGenerateTypes.stone);
  //		int startNumb = GetStartNumber(barrierGenerateTypes.stone);

  //		GameObject obj = pool.getPooledObject(Random.Range(startNumb, startNumb + barrierPref[generateNumber].prefab.Count));
  //		obj.transform.position = new Vector3(generateRightBorder, RunnerController.instance.thisMapHeight + 1.5f + (isHandingPlatform ? 4 : 0), transform.position.z);
  //		obj.SetActive(true);

  //		if (activePet == PlayerPetsTypes.spider && obj.GetComponent<BarrierController>())
  //			obj.GetComponent<BarrierController>().SetTop();
  //		else
  //			obj.GetComponent<BarrierController>().SetDown();

  //		if (coinsGenerateReady && Random.value <= 0.4f && lastCoinsGenerate + 15 <= runnerDistantion) {
  //			if (GenerateCoinsGroupEvent != null) GenerateCoinsGroupEvent(new Vector3(generateRightBorder, RunnerController.instance.thisMapHeight + Random.Range(1f, 2.5f), transform.position.z), 1);
  //			lastCoinsGenerate = runnerDistantion;
  //			coinsGenerateReady = false;
  //		}

  //		if (nextType == barrierGenerateTypes.stoneDouble) {
  //			nextGenerate = runnerDistantion + Random.Range(7 * 0.8f, 7 * 1.2f);
  //			groupGenerate = true;
  //			nextType = barrierGenerateTypes.stone;
  //		} else if (nextType == barrierGenerateTypes.stoneAndHending) {
  //			nextGenerate = runnerDistantion + Random.Range(7 * 0.8f, 7 * 1.2f);
  //			groupGenerate = true;
  //			nextType = barrierGenerateTypes.hending;
  //		} else
  //			barrierGenerateReady = false;
  //	}

  //	if (nextType == barrierGenerateTypes.barrels || nextType == barrierGenerateTypes.boxes || nextType == barrierGenerateTypes.puddle || nextType == barrierGenerateTypes.stickyFloor) {
  //		int generateNumber = GetThisNumber(nextType);
  //		int startNumb = GetStartNumber(nextType);

  //		GameObject obj = pool.getPooledObject(Random.Range(startNumb, startNumb + barrierPref[generateNumber].prefab.Count));
  //		obj.transform.position = new Vector3(generateRightBorder, RunnerController.instance.thisMapHeight + 1, transform.position.z);
  //		obj.SetActive(true);
  //		barrierGenerateReady = false;
  //	}

  //	// Скелеты из земли
  //	if (nextType == barrierGenerateTypes.stoneSkelet || nextType == barrierGenerateTypes.stoneSkeletDouble) {

  //		int generateNumber = GetThisNumber(barrierGenerateTypes.stoneSkelet);
  //		int startNumb = GetStartNumber(barrierGenerateTypes.stoneSkelet);

  //		GameObject obj = pool.getPooledObject(Random.Range(startNumb, startNumb + barrierPref[generateNumber].prefab.Count));
  //		obj.transform.position = new Vector3(generateRightBorder, RunnerController.instance.thisMapHeight, transform.position.z);
  //		obj.SetActive(true);

  //		if (coinsGenerateReady && Random.value <= 0.4f && lastCoinsGenerate + 15 <= runnerDistantion) {
  //			if (GenerateCoinsGroupEvent != null) GenerateCoinsGroupEvent(new Vector3(generateRightBorder, RunnerController.instance.thisMapHeight + Random.Range(1f, 2.5f), transform.position.z), 1);
  //			lastCoinsGenerate = runnerDistantion;
  //			coinsGenerateReady = false;
  //		}

  //		if (nextType == barrierGenerateTypes.stoneSkeletDouble) {
  //			nextGenerate = runnerDistantion + Random.Range(7 * 0.8f, 7 * 1.2f);
  //			groupGenerate = true;
  //			nextType = barrierGenerateTypes.stoneSkelet;
  //		} else
  //			barrierGenerateReady = false;
  //	}

  //	// Призраки
  //	if (nextType == barrierGenerateTypes.ghost || nextType == barrierGenerateTypes.ghostGroup) {

  //		int generateNumber = GetThisNumber(barrierGenerateTypes.ghost);
  //		int startNumb = GetStartNumber(barrierGenerateTypes.ghost);

  //		GameObject obj = pool.getPooledObject(Random.Range(startNumb, startNumb + barrierPref[generateNumber].prefab.Count));
  //		obj.transform.position = new Vector3(CameraController.displayDiff.rightDif(1.3f), RunnerController.instance.thisMapHeight + (Random.value <= 0.5f ? 1 : 3.5f), transform.position.z);
  //		obj.SetActive(true);

  //		if (nextType == barrierGenerateTypes.ghostGroup) {
  //			nextGenerate = runnerDistantion + Random.Range(15 * 0.8f, 15 * 1.2f);
  //			nextType = barrierGenerateTypes.ghost;
  //			ghostCount = Random.Range(1, 5);
  //		}

  //		if (ghostCount > 0) {
  //			nextGenerate = runnerDistantion + Random.Range(15 * 0.8f, 15 * 1.2f);
  //			groupGenerate = true;
  //			nextType = barrierGenerateTypes.ghost;
  //			ghostCount--;
  //		}

  //		if (ghostCount <= 0)
  //			barrierGenerateReady = false;
  //	}
  //}

  /// <summary>
  /// Бесконечный инкремент параметров
  /// </summary>
  //void InfinityIncrement() {

  //  BarrierGenerateParam newBarrier = new BarrierGenerateParam();
  //  newBarrier.distance = barrierParam[barrierParam.Count - 1].distance + 500;
  //  newBarrier.probility = barrierParam[barrierParam.Count - 1].probility + 0.02f;
  //  newBarrier.probability = barrierParam[barrierParam.Count - 1].probability;
  //  barrierParam.Add(newBarrier);

  //}

  #endregion

  #region Петы

  [HideInInspector]
  public PetsTypes activePet;
  void PetsChange(PetsTypes pet, bool useFlag, float timeUse) {
    if (useFlag)
      activePet = pet;
    else
      activePet = PetsTypes.none;
  }

  #endregion

  #region Магнит

  bool activeMagnet;
  public void MagnitEvent(bool flag) {
    activeMagnet = flag;
  }

  #endregion

  #region Расстановка могилок друзей

  bool friendsInicialization;                                         // Флаг инициализации друзей
  bool FbLogin;
  public GameObject[] friendPref;                                     // Набор могилок
  List<FriendBest> allfriends;                                        // Список генераций
  FriendBest nextFriendGen;                                           // Следующая генерация
  float lastFriendGen;                                                // Последняя генерация
  public Material friendAva;
  bool maxDistancePomblited;          // Флаг о преодалении максимальной дистанции

  // заполняем список могилок для пулинга
  void FillFriendPool() {

    allfriends = new List<FriendBest>();

    List<LeaderboardItem> friensAll = Apikbs.Instance.LeaderboardFb;
    friensAll.Reverse();

    foreach (LeaderboardItem one in friensAll) {
      FriendBest newFriend = new FriendBest();
      newFriend.fb = one.fb;
      newFriend.dist = int.Parse(one.bestDistantion);
      newFriend.firstName = one.firstName;
      newFriend.lastName = one.lastName;
      if (newFriend.fb == FBController.GetUserId) {
        newFriend.pref = friendPref[0];
        newFriend.dist = Mathf.Max(newFriend.dist, bestDist);
      } else
        newFriend.pref = friendPref[Random.Range(1, friendPref.Length)];

      newFriend.avaUrl = one.picture;
      allfriends.Add(newFriend);
    }

  }


  /// <summary>
  /// Генерация максимальной дистанции игрока
  /// </summary>
  void GetMy() {

    if (!maxDistancePomblited && nextFriendGen.dist > 0 && runnerDistantion >= nextFriendGen.dist - 2) {

      Collider[] objCol = Physics.OverlapSphere(new Vector3(CameraController.displayDiff.rightDif(2.3f), RunnerController.Instance.mapHeight, 0), 3f, graundMask);

      bool YesGenerate = true;

      foreach (Collider oneColl in objCol) {
        if (oneColl.tag == "jumpUp" | oneColl.tag == "jumpDown")
          YesGenerate = false;
      }

      if (YesGenerate & objCol.Length > 0) {
        maxDistancePomblited = true;
        GameObject inst = Instantiate(nextFriendGen.pref, new Vector3(CameraController.displayDiff.rightDif(2.3f), objCol[0].transform.position.y, transform.position.z), Quaternion.identity) as GameObject;
        inst.GetComponent<BestDist>().tableTextNoPhoto.text = nextFriendGen.dist + " M";
        inst.GetComponent<BestDist>().SetBestDist();
        inst.GetComponent<BestDist>().panelPhoto.SetActive(false);
        inst.GetComponent<BestDist>().panelNoPhoto.SetActive(true);

        nextFriendGen.dist = 0;
      }
    }
  }
  // Подготовка следующую могилку под генерацию
  void SetNewFriendPool() {
    nextFriendGen = new FriendBest();
    foreach (FriendBest one in allfriends) {
      nextFriendGen.dist = one.dist;
      nextFriendGen.firstName = one.firstName;
      nextFriendGen.lastName = one.lastName;
      nextFriendGen.pref = one.pref;
      nextFriendGen.avaUrl = one.avaUrl;
      StartCoroutine(DownloadAllSprite(nextFriendGen.avaUrl));
      allfriends.Remove(one);
      return;
    }

    nextFriendGen.dist = 0;
  }
  // Подготока своей максимальной дистанции
  void SetMePool() {
    nextFriendGen = new FriendBest();
    nextFriendGen.dist = bestDist;
    nextFriendGen.pref = friendPref[0];
    return;

  }
  // Генерация флагов дистанций друзей
  void GenFriends() {
    if (nextFriendGen.dist > 0 && runnerDistantion >= nextFriendGen.dist - 2) {

      Collider[] objCol = Physics.OverlapSphere(new Vector3(CameraController.displayDiff.rightDif(2.3f), RunnerController.Instance.mapHeight, 0), 3f, graundMask);

      bool YesGenerate = true;

      foreach (Collider oneColl in objCol) {
        if (oneColl.tag == "jumpUp" | oneColl.tag == "jumpDown")
          YesGenerate = false;
      }

      if (YesGenerate & objCol.Length > 0) {
        GameObject inst = Instantiate(nextFriendGen.pref, new Vector3(CameraController.displayDiff.rightDif(2.3f), objCol[0].transform.position.y, transform.position.z), Quaternion.identity) as GameObject;
        inst.GetComponent<BestDist>().tableText.text = nextFriendGen.dist + " M";
        inst.GetComponent<BestDist>().mesh.material.mainTexture = nextFriendGen.ava;

        // Подсвечиваем, если дистанция больше собственной ранее
        if (bestDist <= nextFriendGen.dist) inst.GetComponent<BestDist>().SetBestDist();

        SetNewFriendPool();
      }
    }
  }
  // Загрузка картинки под магилки
  IEnumerator DownloadAllSprite(string avaUrl) {
    WWW www = new WWW(avaUrl);
    yield return www;
    nextFriendGen.ava = www.texture;
    RunnerGamePlay.SetFriend(nextFriendGen.ava, nextFriendGen.dist);
  }
  #endregion

  #region Расстановка ворот

  public GameObject gateWood;
  public GameObject gateStone;
  private int gateNumOpen;
  private int gateNum;
  public bool gateGenerate;                         // Генерация ворот
  private float nextDistanceGenerateGate;           // Дистанция генерации новых ворот

  /// <summary>
  /// Инифиализация ворот
  /// </summary>
  void InitGate() {
    if (RunnerController.Instance.activeLevel == ActiveLevelType.ship) return;
    nextDistanceGenerateGate = GameManager.Instance.mapRun[0].distance;
  }

  /// <summary>
  /// Генерация ворот
  /// </summary>
  /// <returns></returns>
  bool GenerateGate() {

    if (RunnerController.Instance.activeLevel == ActiveLevelType.ship) return false;

    if (GameManager.Instance.mapRun.Count == 0 || lockedGenerate) return false;

    if (nextDistanceGenerateGate <= RunnerController.playerDistantion) {
      for (int i = 0; i < GameManager.Instance.mapRun.Count; i++) {
        if (GameManager.Instance.mapRun[i].distance <= RunnerController.Instance.thisDistantionPosition
          && !GameManager.Instance.mapRun[i].isComplited) {
          GameManager.Instance.mapRun[i].isComplited = true;

          if (i < GameManager.Instance.mapRun.Count - 1) {
            nextDistanceGenerateGate = GameManager.Instance.mapRun[i + 1].distanceStart + GameManager.Instance.mapRun[i + 1].distance;
          }

          GameObject gate;
          if (i == 2) {
            gate = Instantiate(gateStone, new Vector3(CameraController.displayDiff.rightDif(2.5f), RunnerController.Instance.mapHeight, transform.position.z), Quaternion.identity) as GameObject;
          } else {
            gate = Instantiate(gateWood, new Vector3(CameraController.displayDiff.rightDif(2.5f), RunnerController.Instance.mapHeight, transform.position.z), Quaternion.identity) as GameObject;
          }
          gate.GetComponent<gateController>().gateNum = i + 1;
          gate.GetComponent<gateController>().keyCount = (int)GameManager.Instance.mapRun[i].keys;
          gate.GetComponent<gateController>().SetLabel();
          gate.SetActive(true);

          if (gateNumOpen >= i + 1) {
            gate.GetComponent<gateController>().OpenDef();
          }
          return true;
        }
      }
    }
    return false;
  }

  #endregion

  /// <summary>
  /// Генерация эффекта выполнения квеста
  /// </summary>
  /// <param name="pos"></param>
  public static void questionSfx(Vector3 pos) {
    if (Instance == null) return;

    GameObject inst = Pooler.GetPooledObject("QuestionDone");
    inst.transform.position = pos;
    inst.SetActive(true);

  }

  #region Звуки

  public AudioClip[] ambiendClips;
  public FloatSpan tineWaitAmbientAudio;
  float timeAudioAmbientPlay;

  void InitAudio() {
    timeAudioAmbientPlay = Time.time + Random.Range(tineWaitAmbientAudio.min, tineWaitAmbientAudio.max);
  }

  void UpdateAudio() {
    if (ambiendClips.Length > 0) {
      if (timeAudioAmbientPlay < Time.time) {
        AudioManager.PlayEffect(ambiendClips[Random.Range(0, ambiendClips.Length)], AudioMixerTypes.runnerEffect);
        timeAudioAmbientPlay = Time.time + Random.Range(tineWaitAmbientAudio.min, tineWaitAmbientAudio.max);
      }
    }
  }

  #endregion


  #region Специальные препядствия

  //public SpecialBarrierGenerateParam[] specialBarrierParam;                           // Параметры генерации
  //bool isSpecialBarrier;                                                              // Флаг выполнения спициального барьера

  //public static bool ActiveSpecialAttack {
  //	get { return instance.isSpecialBarrier; }
  //}

  //public delegate void SpecialBarrierEnd();

  //public delegate void SpecialBarrier(bool isActivate, SpecialBarriersTypes? barrier, SpecialBarrierEnd calback = null, int? numPgorgamm = null);
  //public static SpecialBarrier OnSpecialBarrier;                                      // Событие начала специальной атаки

  //float? nextDistanceActive;                                                          // Дистанция срабатывания препядствия
  //float? nextDistanceCalc;                                                            // Дистанция для рассчета срабатывания
  //SpecialBarriersTypes? activeSpecialBarrier;                                         // Тип барьера для активации

  /// <summary>
  /// Инициализация специального барьера
  /// </summary>
  //void InitSpecialBarrier() {

  //	if (runnerPhase != RunnerPhase.run)
  //		return;

  //	for (int i = 0; i < specialBarrierParam.Length; i++) {
  //		if (!specialBarrierParam[i].generate && specialBarrierParam[i].distance.min <= RunnerController.playerDistantion && i != 0) {
  //			specialBarrierParam[i].generate = true;
  //		}
  //	}
  //	CalcDistForCheckProbSpecialBarrier();
  //}
  /// <summary>
  /// Рассчет дистанции для проверки вероятности генерации
  /// </summary>
  //void CalcDistForCheckProbSpecialBarrier() {
  //	for (int i = 0; i < specialBarrierParam.Length; i++) {
  //		if (i == specialBarrierParam.Length - 2 && specialBarrierParam[i].distance.min <= RunnerController.playerDistantion)
  //			InfinityIncrementSpecialBarrier();
  //		if (nextDistanceCalc == null && !specialBarrierParam[i].generate && specialBarrierParam[i].distance.min > RunnerController.playerDistantion) {
  //			nextDistanceCalc = specialBarrierParam[i].distance.min;
  //		}
  //	}
  //}

  /// <summary>
  /// Рассчет дистанции для активации
  /// </summary>
  //void CalcDistForActivSpecialBarrier() {
  //	for (int i = 0; i < specialBarrierParam.Length; i++) {
  //		if (nextDistanceActive == null && !specialBarrierParam[i].generate && nextDistanceCalc <= specialBarrierParam[i].distance.min) {
  //			specialBarrierParam[i].generate = true;

  //			if (Random.value <= specialBarrierParam[i].probability) {
  //				nextDistanceActive = specialBarrierParam[i].distance.min + Random.Range(20, specialBarrierParam[i].distance.max - specialBarrierParam[i].distance.min);
  //				activeSpecialBarrier = CalcNextSpecialBarrier(specialBarrierParam[i].types);
  //			} else {
  //				specialBarrierParam[i].generate = false;
  //				nextDistanceActive = null;
  //				nextDistanceCalc = (specialBarrierParam[i].distance.max - specialBarrierParam[i].distance.min) / 2 + specialBarrierParam[i].distance.min;
  //			}
  //			return;
  //		} else if (nextDistanceActive == null && !specialBarrierParam[i].generate && nextDistanceCalc > specialBarrierParam[i].distance.min && nextDistanceCalc < specialBarrierParam[i].distance.max) {

  //			specialBarrierParam[i].generate = true;
  //			if (Random.value <= specialBarrierParam[i].probability) {
  //				nextDistanceActive = nextDistanceCalc + Random.Range(20, specialBarrierParam[i].distance.max - (float)nextDistanceCalc);
  //				activeSpecialBarrier = CalcNextSpecialBarrier(specialBarrierParam[i].types);
  //			} else {
  //				nextDistanceActive = null;
  //				nextDistanceCalc = null;
  //			}
  //		}
  //	}
  //}

  /// <summary>
  /// Активация специального барьера
  /// </summary>
  //void ActivateSpecialBarrier() {
  //	if (OnSpecialBarrier != null && activeSpecialBarrier != null) {
  //		OnSpecialBarrier(true, activeSpecialBarrier, SpecialBarrierEndCallBack);
  //		isSpecialBarrier = true;
  //	} else
  //		SpecialBarrierEndCallBack();
  //}

  /// <summary>
  /// Выполняется каждый апдейт
  /// </summary>
  //void UpdateSpecalBarrier() {
  //	if (runnerPhase != RunnerPhase.run) return;
  //	if (!isSpecialBarrier) {
  //		if (nextDistanceCalc == null)
  //			CalcDistForCheckProbSpecialBarrier();
  //		if (nextDistanceCalc != null && nextDistanceCalc <= RunnerController.playerDistantion && nextDistanceActive == null)
  //			CalcDistForActivSpecialBarrier();
  //		if (nextDistanceActive != null && activeSpecialBarrier != null && nextDistanceActive <= RunnerController.playerDistantion)
  //			ActivateSpecialBarrier();
  //	}
  //}

  /// <summary>
  /// Калбак об окончании атаки
  /// </summary>
  //void SpecialBarrierEndCallBack() {
  //	if (OnSpecialBarrier != null)
  //		OnSpecialBarrier(false, activeSpecialBarrier, null);
  //	nextDistanceCalc = null;
  //	nextDistanceActive = null;
  //	isSpecialBarrier = false;
  //	activeSpecialBarrier = null;
  //}

  /// <summary>
  /// Рассчет необходимого типа барьера
  /// </summary>
  /// <returns></returns>
  //SpecialBarriersTypes CalcNextSpecialBarrier(SpecialBarriersProbability[] arrayForCalc) {
  //	cd = new List<float>();
  //	float sum = 0;
  //	cd.Add(sum);

  //	foreach (SpecialBarriersProbability param in arrayForCalc) {
  //		sum += param.shareProbability;
  //		cd.Add(sum);
  //	}

  //	if (sum != 1f)
  //		cd = cd.ConvertAll(x => x / sum);

  //	return arrayForCalc[BinarySearch.RandomNumberGenerator(cd)].type;
  //}

  //void InfinityIncrementSpecialBarrier() {
  //	SpecialBarrierGenerateParam[] tmp = new SpecialBarrierGenerateParam[specialBarrierParam.Length + 1];

  //	for (int i = 0; i < specialBarrierParam.Length; i++)
  //		tmp[i] = specialBarrierParam[i];

  //	tmp[tmp.Length - 1].distance.min = specialBarrierParam[specialBarrierParam.Length - 1].distance.max;
  //	tmp[tmp.Length - 1].distance.max = specialBarrierParam[specialBarrierParam.Length - 1].distance.max + 200;
  //	tmp[tmp.Length - 1].generate = false;
  //	tmp[tmp.Length - 1].probability = specialBarrierParam[specialBarrierParam.Length - 1].probability;
  //	tmp[tmp.Length - 1].types = specialBarrierParam[specialBarrierParam.Length - 1].types;
  //	specialBarrierParam = tmp;

  //}

  #endregion

  #region Висячие платформы

  //public delegate void OnHandingPlatforms(bool isActivate, int type, SpecialBarrierEnd calback = null);
  //public static OnHandingPlatforms OnHandingPlatform;                                      // Событие начала специальной атаки

  //float nextDistActHandPlatform;                                                           // Дистанция срабатывания препядствия
  //float nextDistCalcHandPlatform;                                                          // Дистанция для рассчета срабатывания
  [HideInInspector]
  //public bool isHandingPlatform;                                                                  // Выполняется генерация навестных платформ
  //int activeHandPlatform;                                                                  // Текущий активный тип висячих платформ
  //bool isHendngPlatformGenerateReady;                                                      // Готовность к генерации платформ

  //public static bool activNowHendPlatform {
  //	get { return (instance.isHandingPlatform && ((int)instance.activeHandPlatform == 3 || (int)instance.activeHandPlatform == 5)); }
  //}

  /// <summary>
  /// Вероятность генерации платформы одного тип
  /// </summary>
  //[System.Serializable]
  //public struct HandingPlatform {
  //	[Range(1,5)]
  //	public int type;
  //	public int probability;
  //}

  //public List<HandingPlatformParametrs> handingPlatformParametrs;

  /// <summary>
  /// Инициализация специального барьера
  /// </summary>
  //void InitHandingPlatform() {
  //	if (runnerPhase != RunnerPhase.run) return;
  //	isHendngPlatformGenerateReady = false;
  //	//HandingPlatformCalcDist();
  //}
  /// <summary>
  /// Рассчет дистанции для проверки вероятности генерации
  /// </summary>
  //void HandingPlatformCalcDist() {

  //	if (handingPlatformParametrs[handingPlatformParametrs.Count - 2].distance <= RunnerController.playerDistantion)
  //		InfinityIncrementHandingPlatform();

  //	HandingPlatformParametrs nextCheck = handingPlatformParametrs.Find(x => x.distance > nextDistCalcHandPlatform);

  //	if (nextCheck.types.Count > 0) {
  //		activeHandPlatform = nextCheck.GetPlatformType();
  //		isHendngPlatformGenerateReady = true;
  //		nextDistActHandPlatform = Random.Range(nextDistCalcHandPlatform + 10, nextCheck.distance - 30);
  //	}
  //	nextDistCalcHandPlatform = nextCheck.distance;
  //}
  /// <summary>
  /// Рассчет необходимого типа барьера
  /// </summary>
  /// <returns></returns>
  //int CalcNextSpecialBarrier(HandingPlatform[] arrayForCalc) {
  //	cd = new List<float>();
  //	float sum = 0;
  //	cd.Add(sum);

  //	foreach (HandingPlatform param in arrayForCalc) {
  //		sum += param.probability;
  //		cd.Add(sum);
  //	}

  //	if (sum != 1f)
  //		cd = cd.ConvertAll(x => x / sum);

  //	return arrayForCalc[BinarySearch.RandomNumberGenerator(cd)].type;
  //}
  /// <summary>
  /// Активация специального барьера
  /// </summary>
  //void ActivateHandingPlatform() {
  //	isHendngPlatformGenerateReady = false;

  //	if (OnHandingPlatform != null) {
  //		OnHandingPlatform(true, (int)activeHandPlatform, HandingPlatformEndCallBack);
  //		isHandingPlatform = true;
  //	} else
  //		HandingPlatformEndCallBack();
  //}
  /// <summary>
  /// Событие отключения генерации платформ
  /// </summary>
  //void HandingPlatformEndCallBack() {
  //	if (OnHandingPlatform != null) OnHandingPlatform(false, (int)activeHandPlatform, null);
  //	isHandingPlatform = false;
  //}
  /// <summary>
  /// Выполняется каждый апдейт
  /// </summary>
  //void UpdateHandingPlatform() {
  //	if (runnerPhase != RunnerPhase.run) return;

  //	if (isHendngPlatformGenerateReady && nextDistActHandPlatform <= RunnerController.playerDistantion) ActivateHandingPlatform();
  //	if (nextDistCalcHandPlatform <= RunnerController.playerDistantion) HandingPlatformCalcDist();
  //}
  /// <summary>
  /// Бесконечный инкремент висячих платформ
  /// </summary>
  //void InfinityIncrementHandingPlatform() {
  //	HandingPlatformParametrs tmp = new HandingPlatformParametrs();
  //	tmp.distance = handingPlatformParametrs[handingPlatformParametrs.Count - 1].distance
  //								 + (handingPlatformParametrs[handingPlatformParametrs.Count - 2].distance
  //								 - handingPlatformParametrs[handingPlatformParametrs.Count - 3].distance);
  //	tmp.types = handingPlatformParametrs[handingPlatformParametrs.Count - 1].types;
  //	tmp.probability = handingPlatformParametrs[handingPlatformParametrs.Count - 1].probability;
  //	handingPlatformParametrs.Add(tmp);
  //}

  #endregion

  //#region Босс

  ///// <summary>
  ///// Ссылка на префаб обезьяны
  ///// </summary>
  //public GameObject gorillaBossPrefab;

  ///// <summary>
  ///// Ссылка на созданный экземпляр босса
  ///// </summary>
  //GameObject gorillaBossInstance;

  ///// <summary>
  ///// Событие появления босса
  ///// </summary>
  //public static event BossGenerate OnBossGenerate;
  //public delegate void BossGenerate(bool isBoss);

  //void CheckPointGate(int gateNum) {

  //  if(IsInvoking("GenerateBossEnemyGate")) return;

  //  GenerateEnemyBoss(gateNum);
  //}

  ///// <summary>
  ///// Сгенерировать босса
  ///// </summary>
  ///// <param name="numPhase">Номер появления босса</param>
  //public void GenerateEnemyBoss(int numPhase) {

  //  RunnerController.instance.changeMapBlocking++;
  //  RunnerController.instance.generateItemBlock++;

  //  switch(numPhase) {
  //    case 0:
  //      GenerateBossEnemyStart();
  //      break;
  //    default:
  //      gorillaShowNumber = numPhase;
  //      Invoke("GenerateBossEnemyGate", 5);
  //      break;
  //  }

  //  if(OnBossGenerate != null)
  //    OnBossGenerate(true);
  //}

  //int gorillaShowNumber;

  ///// <summary>
  ///// Первоначальное появление гориллы
  ///// </summary>
  //void GenerateBossEnemyStart() {

  //  gorillaBossInstance = Instantiate(gorillaBossPrefab);
  //  gorillaBossInstance.transform.position = new Vector3(CameraController.displayDiff.leftDif(1.2f), RunnerController.mapHeight, 0);
  //  gorillaBossInstance.SetActive(true);
  //  gorillaBossInstance.GetComponent<EnemyBoss>().OnBossDead += BossDead;
  //  gorillaBossInstance.GetComponent<GorillaBoss>().SetShowNumber(0);
  //}

  //void GenerateBossEnemyGate() {
  //  gorillaBossInstance.transform.position = new Vector3(CameraController.displayDiff.rightDif(1.2f), RunnerController.mapHeight, 0);
  //  gorillaBossInstance.SetActive(true);
  //  gorillaBossInstance.GetComponent<EnemyBoss>().OnBossDead += BossDead;
  //  gorillaBossInstance.GetComponent<GorillaBoss>().SetShowNumber(gorillaShowNumber);
  //}

  //void BossDead() {
  //  if(OnBossGenerate != null)
  //    OnBossGenerate(false);
  //  RunnerController.instance.changeMapBlocking--;
  //  RunnerController.instance.generateItemBlock--;
  //}

  //#endregion

  #region Объект

  public void GenerateMapObject(MapObject newPanPbject) {
    if (OnGenerateMapObject != null) OnGenerateMapObject(newPanPbject);
  }

  #endregion

  #region Настройки


  public void GetConfig() {

  }

  //void GetSpecialBarrierSurvival() {
  //	ConfigData configData = Config.instant.config.GetConfigData();
  //	List<object> itemsData = (List<object>)configData.GetLevelData()["items"];
  //	Dictionary <string,object> oneItem;
  //	int startCol = 1;
  //	float distance;
  //	bool isEnd = false;

  //	List<SpecialBarrierGenerateParam> specialBarrierParamTmp  = new List<SpecialBarrierGenerateParam>();

  //	for (int i = 0; i < itemsData.Count; i++) {
  //		if (i < startCol || isEnd) continue;

  //		oneItem = (Dictionary<string, object>)itemsData[i];
  //		if (oneItem["trap"].ToString() == "0") continue;
  //		SpecialBarrierGenerateParam oneBarrier = new SpecialBarrierGenerateParam();

  //		try {
  //			distance = float.Parse(oneItem["distantion"].ToString());
  //		} catch {
  //			distance = specialBarrierParamTmp[specialBarrierParamTmp.Count - 1].distance.min + 200;
  //			isEnd = true;
  //		}

  //		oneBarrier.distance.min = distance;

  //		if (i < itemsData.Count - 2)
  //			oneBarrier.distance.max = float.Parse(((Dictionary<string, object>)itemsData[i + 1])["distantion"].ToString());

  //		if (i == itemsData.Count - 1)
  //			oneBarrier.distance.max = oneBarrier.distance.min + 200;

  //		oneBarrier.generate = false;
  //		oneBarrier.probability = float.Parse(oneItem["trap"].ToString()) * 0.01f;
  //		oneBarrier.types = new SpecialBarriersProbability[2];
  //		oneBarrier.types[0].type = SpecialBarriersTypes.airArrow;
  //		oneBarrier.types[0].shareProbability = 50;
  //		oneBarrier.types[1].type = SpecialBarriersTypes.airStone;
  //		oneBarrier.types[1].shareProbability = 50;
  //		specialBarrierParamTmp.Add(oneBarrier);
  //	}
  //	specialBarrierParam = specialBarrierParamTmp.ToArray();
  //}
  //void GetSpecialBarrierLevel() {
  //	ConfigData configData = Config.instant.config.GetConfigData();
  //	List<object> itemsData = (List<object>)configData.GetLevelData()["specialBarrier"];
  //	Dictionary <string,object> oneItem;
  //	int startCol = 1;
  //	float distance;
  //	bool isEnd = false;

  //	List<SpecialBarrierGenerateParam> specialBarrierParamTmp  = new List<SpecialBarrierGenerateParam>();

  //	for (int i = 0; i < itemsData.Count; i++) {
  //		if (i < startCol || isEnd) continue;

  //		oneItem = (Dictionary<string, object>)itemsData[i];
  //		if (oneItem["trap"].ToString() == "0") continue;
  //		SpecialBarrierGenerateParam oneBarrier = new SpecialBarrierGenerateParam();

  //		try {
  //			distance = float.Parse(oneItem["distantion"].ToString());
  //		} catch {
  //			distance = specialBarrierParamTmp[specialBarrierParamTmp.Count - 1].distance.min + 200;
  //			isEnd = true;
  //		}

  //		oneBarrier.distance.min = distance;

  //		if (i < itemsData.Count - 2)
  //			oneBarrier.distance.max = float.Parse(((Dictionary<string, object>)itemsData[i + 1])["distantion"].ToString());

  //		if (i == itemsData.Count - 1)
  //			oneBarrier.distance.max = oneBarrier.distance.min + 200;

  //		oneBarrier.generate = false;
  //		oneBarrier.probability = float.Parse(oneItem["trap"].ToString()) * 0.01f;
  //		oneBarrier.types = new SpecialBarriersProbability[2];
  //		oneBarrier.types[0].type = SpecialBarriersTypes.airArrow;
  //		oneBarrier.types[0].shareProbability = float.Parse(oneItem["airArrow"].ToString()) * 0.01f;
  //		oneBarrier.types[1].type = SpecialBarriersTypes.airStone;
  //		oneBarrier.types[1].shareProbability = float.Parse(oneItem["airStone"].ToString()) * 0.01f;
  //		specialBarrierParamTmp.Add(oneBarrier);
  //	}
  //	specialBarrierParam = specialBarrierParamTmp.ToArray();
  //}

  //void GetPlatformParametrsSurvival() {
  //	configData = Config.instant.config.GetConfigData();
  //	Dictionary <string,object> oneItem;
  //	List<object> itemsData = (List<object>)configData.GetLevelData()["platforms"];

  //	handingPlatformParametrs.Clear();

  //	for (int i = 0; i < itemsData.Count; i++) {

  //		oneItem = (Dictionary<string, object>)itemsData[i];

  //		HandingPlatformParametrs onePlatform = new HandingPlatformParametrs();
  //		onePlatform.distance = i * 500;
  //		onePlatform.probability = float.Parse(oneItem["hendingPlatform"].ToString()) * 0.01f;
  //		onePlatform.types = new List<HandingPlatformParametrs.HandingPlatform>();

  //		for (int platTypeNum = 1; platTypeNum <= 5; platTypeNum++) {
  //			if (oneItem.ContainsKey("tipe" + platTypeNum) && oneItem["tipe" + platTypeNum].ToString() != "0") {
  //				HandingPlatformParametrs.HandingPlatform oneHand = new HandingPlatformParametrs.HandingPlatform();
  //				oneHand.type = platTypeNum;
  //				oneHand.probability = int.Parse(oneItem["tipe" + platTypeNum].ToString());
  //				onePlatform.types.Add(oneHand);
  //			}
  //		}
  //		handingPlatformParametrs.Add(onePlatform);
  //	}
  //}

  //void GetPlatformParametrsLevel() {
  //	configData = Config.instant.config.GetConfigData();
  //	Dictionary <string,object> oneItem;
  //	List<object> itemsData = (List<object>)configData.GetLevelData()["platforms"];

  //	handingPlatformParametrs.Clear();

  //	for (int i = 0; i < itemsData.Count; i++) {

  //		oneItem = (Dictionary<string, object>)itemsData[i];

  //		HandingPlatformParametrs onePlatform = new HandingPlatformParametrs();
  //		onePlatform.distance = float.Parse(oneItem["distantion"].ToString());
  //		onePlatform.probability = float.Parse(oneItem["hendingPlatform"].ToString()) * 0.01f;
  //		onePlatform.types = new List<HandingPlatformParametrs.HandingPlatform>();

  //		for (int platTypeNum = 1; platTypeNum <= 5; platTypeNum++) {
  //			if (oneItem.ContainsKey("tipe" + platTypeNum) && oneItem["tipe" + platTypeNum].ToString() != "0") {
  //				HandingPlatformParametrs.HandingPlatform oneHand = new HandingPlatformParametrs.HandingPlatform();
  //				oneHand.type = platTypeNum;
  //				oneHand.probability = int.Parse(oneItem["tipe" + platTypeNum].ToString());
  //				onePlatform.types.Add(oneHand);
  //			}
  //		}
  //		handingPlatformParametrs.Add(onePlatform);
  //	}
  //}

  //void GetBarrierParametrsSurvival() {
  //	configData = Config.instant.config.GetConfigData();
  //	Dictionary <string,object> oneItem;
  //	List<object> itemsData = (List<object>)configData.GetLevelData()["barriers"];

  //	barrierParam = new List<BarrierGenerateParam>();

  //	for (int i = 0; i < itemsData.Count; i++) {

  //		oneItem = (Dictionary<string, object>)itemsData[i];

  //		BarrierGenerateParam oneBarrier = new BarrierGenerateParam();
  //		oneBarrier.distance = i * 500;
  //		try {
  //			oneBarrier.probility = float.Parse(oneItem["barrierProbablility"].ToString()) * 0.01f;
  //		} catch {
  //			oneBarrier.probility = barrierParam[barrierParam.Count - 1].probility + 0.02f;
  //		}

  //		List<barrierProbability> barrList = new List<barrierProbability>();

  //		if (oneItem.ContainsKey("break") && oneItem["break"].ToString() != "0") {
  //			barrierProbability oneBarr = new barrierProbability();
  //			oneBarr.value = float.Parse(oneItem["break"].ToString());
  //			oneBarr.type = barrierGenerateTypes.breack;
  //			barrList.Add(oneBarr);
  //		}
  //		if (oneItem.ContainsKey("doubleSkeleton") && oneItem["doubleSkeleton"].ToString() != "0") {
  //			barrierProbability oneBarr = new barrierProbability();
  //			oneBarr.value = float.Parse(oneItem["doubleSkeleton"].ToString());
  //			oneBarr.type = barrierGenerateTypes.stoneDouble;
  //			barrList.Add(oneBarr);
  //		}
  //		if (oneItem.ContainsKey("doubleStone") && oneItem["doubleStone"].ToString() != "0") {
  //			barrierProbability oneBarr = new barrierProbability();
  //			oneBarr.value = float.Parse(oneItem["doubleStone"].ToString());
  //			oneBarr.type = barrierGenerateTypes.stoneDouble;
  //			barrList.Add(oneBarr);
  //		}
  //		if (oneItem.ContainsKey("ghost") && oneItem["ghost"].ToString() != "0") {
  //			barrierProbability oneBarr = new barrierProbability();
  //			oneBarr.value = float.Parse(oneItem["ghost"].ToString());
  //			oneBarr.type = barrierGenerateTypes.ghost;
  //			barrList.Add(oneBarr);
  //		}
  //		if (oneItem.ContainsKey("ghostGroup") && oneItem["ghostGroup"].ToString() != "0") {
  //			barrierProbability oneBarr = new barrierProbability();
  //			oneBarr.value = float.Parse(oneItem["ghostGroup"].ToString());
  //			oneBarr.type = barrierGenerateTypes.ghostGroup;
  //			barrList.Add(oneBarr);
  //		}
  //		if (oneItem.ContainsKey("handingBarrier") && oneItem["handingBarrier"].ToString() != "0") {
  //			barrierProbability oneBarr = new barrierProbability();
  //			oneBarr.value = float.Parse(oneItem["handingBarrier"].ToString());
  //			oneBarr.type = barrierGenerateTypes.hending;
  //			barrList.Add(oneBarr);
  //		}
  //		if (oneItem.ContainsKey("spiderNet") && oneItem["spiderNet"].ToString() != "0") {
  //			barrierProbability oneBarr = new barrierProbability();
  //			oneBarr.value = float.Parse(oneItem["spiderNet"].ToString());
  //			oneBarr.type = barrierGenerateTypes.stickyFloor;
  //			barrList.Add(oneBarr);
  //		}
  //		if (oneItem.ContainsKey("stone") && oneItem["stone"].ToString() != "0") {
  //			barrierProbability oneBarr = new barrierProbability();
  //			oneBarr.value = float.Parse(oneItem["stone"].ToString());
  //			oneBarr.type = barrierGenerateTypes.stone;
  //			barrList.Add(oneBarr);
  //		}
  //		if (oneItem.ContainsKey("stoneAndHanding") && oneItem["stoneAndHanding"].ToString() != "0") {
  //			barrierProbability oneBarr = new barrierProbability();
  //			oneBarr.value = float.Parse(oneItem["stoneAndHanding"].ToString());
  //			oneBarr.type = barrierGenerateTypes.stoneAndHending;
  //			barrList.Add(oneBarr);
  //		}
  //		if (oneItem.ContainsKey("stoneSkeleton") && oneItem["stoneSkeleton"].ToString() != "0") {
  //			barrierProbability oneBarr = new barrierProbability();
  //			oneBarr.value = float.Parse(oneItem["stoneSkeleton"].ToString());
  //			oneBarr.type = barrierGenerateTypes.stoneSkelet;
  //			barrList.Add(oneBarr);
  //		}

  //		oneBarrier.probability = barrList;
  //		barrierParam.Add(oneBarrier);
  //	}
  //}

  //void GetBarrierParametrsLevel() {
  //	configData = Config.instant.config.GetConfigData();
  //	Dictionary <string,object> oneItem;
  //	List<object> itemsData = (List<object>)configData.GetLevelData()["barriers"];

  //	barrierParam = new List<BarrierGenerateParam>();

  //	for (int i = 0; i < itemsData.Count; i++) {

  //		oneItem = (Dictionary<string, object>)itemsData[i];
  //		BarrierGenerateParam oneBarrier = new BarrierGenerateParam();

  //		oneBarrier.distance = float.Parse(oneItem["distantion"].ToString() != "0" ? oneItem["distantion"].ToString() : "0");
  //		oneBarrier.count = int.Parse(oneItem["barrier"].ToString());

  //		List<barrierProbability> barrList = new List<barrierProbability>();

  //		if (oneItem.ContainsKey("break") && oneItem["break"].ToString() != "0") {
  //			barrierProbability oneBarr = new barrierProbability();
  //			oneBarr.value = float.Parse(oneItem["break"].ToString());
  //			oneBarr.type = barrierGenerateTypes.breack;
  //			barrList.Add(oneBarr);
  //		}
  //		if (oneItem.ContainsKey("doubleSkeleton") && oneItem["doubleSkeleton"].ToString() != "0") {
  //			barrierProbability oneBarr = new barrierProbability();
  //			oneBarr.value = float.Parse(oneItem["doubleSkeleton"].ToString());
  //			oneBarr.type = barrierGenerateTypes.stoneDouble;
  //			barrList.Add(oneBarr);
  //		}
  //		if (oneItem.ContainsKey("doubleStone") && oneItem["doubleStone"].ToString() != "0") {
  //			barrierProbability oneBarr = new barrierProbability();
  //			oneBarr.value = float.Parse(oneItem["doubleStone"].ToString());
  //			oneBarr.type = barrierGenerateTypes.stoneDouble;
  //			barrList.Add(oneBarr);
  //		}
  //		if (oneItem.ContainsKey("ghost") && oneItem["ghost"].ToString() != "0") {
  //			barrierProbability oneBarr = new barrierProbability();
  //			oneBarr.value = float.Parse(oneItem["ghost"].ToString());
  //			oneBarr.type = barrierGenerateTypes.ghost;
  //			barrList.Add(oneBarr);
  //		}
  //		if (oneItem.ContainsKey("ghostGroup") && oneItem["ghostGroup"].ToString() != "0") {
  //			barrierProbability oneBarr = new barrierProbability();
  //			oneBarr.value = float.Parse(oneItem["ghostGroup"].ToString());
  //			oneBarr.type = barrierGenerateTypes.ghostGroup;
  //			barrList.Add(oneBarr);
  //		}
  //		if (oneItem.ContainsKey("handingBarrier") && oneItem["handingBarrier"].ToString() != "0") {
  //			barrierProbability oneBarr = new barrierProbability();
  //			oneBarr.value = float.Parse(oneItem["handingBarrier"].ToString());
  //			oneBarr.type = barrierGenerateTypes.hending;
  //			barrList.Add(oneBarr);
  //		}
  //		if (oneItem.ContainsKey("spiderNet") && oneItem["spiderNet"].ToString() != "0") {
  //			barrierProbability oneBarr = new barrierProbability();
  //			oneBarr.value = float.Parse(oneItem["spiderNet"].ToString());
  //			oneBarr.type = barrierGenerateTypes.stickyFloor;
  //			barrList.Add(oneBarr);
  //		}
  //		if (oneItem.ContainsKey("stone") && oneItem["stone"].ToString() != "0") {
  //			barrierProbability oneBarr = new barrierProbability();
  //			oneBarr.value = float.Parse(oneItem["stone"].ToString());
  //			oneBarr.type = barrierGenerateTypes.stone;
  //			barrList.Add(oneBarr);
  //		}
  //		if (oneItem.ContainsKey("stoneAndHanding") && oneItem["stoneAndHanding"].ToString() != "0") {
  //			barrierProbability oneBarr = new barrierProbability();
  //			oneBarr.value = float.Parse(oneItem["stoneAndHanding"].ToString());
  //			oneBarr.type = barrierGenerateTypes.stoneAndHending;
  //			barrList.Add(oneBarr);
  //		}
  //		if (oneItem.ContainsKey("stoneSkeleton") && oneItem["stoneSkeleton"].ToString() != "0") {
  //			barrierProbability oneBarr = new barrierProbability();
  //			oneBarr.value = float.Parse(oneItem["stoneSkeleton"].ToString());
  //			oneBarr.type = barrierGenerateTypes.stoneSkelet;
  //			barrList.Add(oneBarr);
  //		}

  //		oneBarrier.probability = barrList;
  //		barrierParam.Add(oneBarrier);
  //	}
  //}

  #endregion

}


/// <summary>
/// Генерация платформы по дистанции
/// </summary>
[System.Serializable]
public struct HandingPlatformParametrs {
  [Range(0, 1)]
  public float probability;
  public float distance;
  public List<HandingPlatform> types;
  /// <summary>
  /// Платформа для генерации
  /// </summary>
  /// <returns></returns>
  public int GetPlatformType() {

    List<float> cd = new List<float>();
    float sum = 0;
    cd.Add(sum);
    foreach (HandingPlatform one in types) {
      sum += one.probability;
      cd.Add(sum);
    }

    cd = cd.ConvertAll(x => x / sum);
    return types[BinarySearch.RandomNumberGenerator(cd)].type;
  }

  [System.Serializable]
  public struct HandingPlatform {
    [Range(1, 5)]
    public int type;
    public float probability;
  }

}

public enum MapObject {
  torch,
  spider
}