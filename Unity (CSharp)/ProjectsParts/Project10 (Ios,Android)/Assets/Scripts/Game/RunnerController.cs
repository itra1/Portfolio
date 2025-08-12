using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using ExEvent;
using Player.Jack;

public enum ActiveLevelType {
  classic, ship
}

[System.Serializable]
public struct Boosters {
  public BoostType type;
  public float speed;                     // Скорость
  public float time;                      // Время 
}

[System.Serializable]
public struct speedChangeParametrs {
  public float minDistance;
  public float maxDistance;
  public float minSpeed;
  public float maxSpeed;
}

[System.Serializable]
public struct BarrierDamageParametrs {
  public barrierTages barrierTag;
  public float toPlayer;
  public float toEnemy;
}

/// <summary>
/// Фазы игры
/// </summary>
[System.Serializable]
[Flags]
public enum RunnerPhase {
  start = 0,              // Старт
  roulette = 1,           // Рулетка
  preBoost = 2,           // Фаза перед бустом
  boost = 4,              // Буст
  postBoost = 8,          // Фаза перед бустом
  run = 16,               // Бег
  planer = 32,            // Планер
  dead = 64,              // Смерть
  tutorial = 128,         // Обучение
  final = 256,            // Финальная анимация
  boss = 512,             // Бой с босом
  endRun = 1024,          // Конец забега
  lowEnergy = 2048				// Мало энергии
}

/// <summary>
/// Игровой процесс
/// </summary>
public class RunnerController: Singleton<RunnerController> {

  public ActiveLevelType activeLevel;                         // Текущий уровень
  private RunnerPhase _runnerPhase;                             // Состояние игрока
  public RunnerPhase runnerPhase {
    get { return _runnerPhase; }
    set {
      if (_runnerPhase == value) return;
      RunnerPhase old = _runnerPhase;
      _runnerPhase = value;
      ChangeStatePahse(_runnerPhase);
      if (OnChangeRunnerPhase != null) OnChangeRunnerPhase(_runnerPhase);
      RunEvents.RunPhaseChange.CallAsync(old, _runnerPhase);
    }
  }
  public static event Action<RunnerPhase> OnChangeRunnerPhase;

  // Пауза
  [HideInInspector]
  public float timeStartControl;                              // Время, когда игроку передано управление
  private readonly string pauseButton = "Pause";
  public int[] enemiesDeadCount = new int[6];

  public speedChangeParametrs[] speedParametrs;               // Параметры изменения скорости
  private speedChangeParametrs speedParametrsNow;
  private float? speedParametrsNextCalc = 0;

  [HideInInspector]
  public float runSpeedMin;
  public float runSpeedNowReal;                               // Текущая скорость по счетчику
  public float runSpeedNowMax;                                // Скорость, что была до повреждения

  public float runSpeedActual { get { return runSpeedNowReal * 0.65f; } }                                   // Текущая скорость уменьшеная для графики (что бы не переписывать все контроллеры)

  public static float RunSpeed { get { return Instance.runSpeedActual * Instance._runSpeedKoef * (GameManager.activeLevelData.moveVector == MoveVector.left ? -1 : 1); } }

  private float _runSpeedKoef;

  private float needSpeed;                                    // Скорость, которую необходимо достич
  public bool noChange;                                       // Запрет на изменение высот платформы
                                                              //public GameObject player;                                   // Игрок
  public bool isGod;                                          // Режим бога

  [HideInInspector]
  public int generateItemBlock;                               // Число объектов, блокирующих генерацию эдементов
  [HideInInspector]
  public float ChangeMapLast;                                 // Время последнего изменения уровня
  [HideInInspector]
  public float mapHeight;                                 // Текущая высота уровня

  public float thisDistantionPosition { get; private set; }                              // текущая позиция игрока
  public static float playerDistantion { get; private set; }

  public static float distanceInRegion { get { return playerDistantion - _distanceChangeRegion; } }

  private List<MonoBehaviour> changeMapBlocking = new List<MonoBehaviour>();                               // Число объектов, блокирующих изменение уровня

  public void AddBlockChangeMap(MonoBehaviour obj) {
    if (changeMapBlocking.Contains(obj)) return;
    changeMapBlocking.Add(obj);
  }

  public void RemoveBlockChangeMap(MonoBehaviour obj) {
    changeMapBlocking.Remove(obj);
  }

  public bool IsMapChangeBlock {
    get { return noChange || changeMapBlocking.Count > 0; }
  }

  [HideInInspector]
  public int moneyInRace;                                     // Монеты заработанные за забег
  [HideInInspector]
  public int moneyInRaceCalc;           // Монеты заработанные за забег
  [HideInInspector]
  public int moneyBlocker;              // Блокироваки генерации монет
  [HideInInspector]
  public bool moneyBlock { get { return moneyBlocker > 0 ? true : false; } }

  [HideInInspector]
  public int cristallInRace;           // Монеты заработанные за забег
  [HideInInspector]
  public int blackPointsInRace;        // Монеты заработанные за забег
  [HideInInspector]
  public int blackPointsInRaceCalc;    // Монеты заработанные за забег
  [HideInInspector]
  public List<int> questionInRaceList; // Список выполненных квестов
  [HideInInspector]
  public int keysInRace;                // Ключи в забеге
  [HideInInspector]
  public int keysInRaceCalc;            // Ключи в забеге для расчетов
  [HideInInspector]
  public bool levelComplited;

  public Boosters[] boosters;                             // Типы бустов старта
  public BoostType booster;                               // Буст
  private float bonusTime;                                        // Время работы буста
  protected RunnerGamePlay gamePlay;                              // ссылка на класс обхекта GamePlay
  public WeaponTypes playerWeapon;
  public int playerWeaponCount;
  public float enemySpeed;                            // Скорость, с которой враги догоняют игрока

  public static event Action<bool> SetPause;

  private bool goToLevelReady = false;
  private int goToLevel = 0;

  public AudioClip moomEffect;

  [HideInInspector]
  bool _stopCalcDistantion; //Останавливаем рассчет дистанции

  public bool doubleJump = true;

  public static bool stopCalcDistantion {
    get {
      return Instance._stopCalcDistantion;
    }
    set {
      if (Instance != null)
        Instance._stopCalcDistantion = value;
    }
  }

  protected override void Awake() {
    base.Awake();

    runSpeedMin = speedParametrs[0].minSpeed;

    foreach (speedChangeParametrs speed in speedParametrs) {
      if (runSpeedMin > speed.minSpeed)
        runSpeedMin = speed.minSpeed;
    }
    thisDistantionPosition = 0;                                 // Сбрасывается дистанция
    playerDistantion = thisDistantionPosition;
  }

  void GamePlayShow() {
    gamePlay = UiController.ShowUi<RunnerGamePlay>();
    gamePlay.gameObject.SetActive(true);
  }

  private void Start() {

    //RunSpawner.OnSpecialBarrier += OnSpecialBarrier;
    gateController.OnEndGate += CheckPointGate;
    AudioManager.Instance.SetSoundParametrs();
    UpdateValue();
    InitCountsQuest();
    InitMetricDistantion();
    InitFinalyAnim();

    questionInRaceList = new List<int>();

    Time.timeScale = 1;
    needSpeed = 0;

    FillBlack.Instance.PlayAnim(FillBlack.AnimType.round, FillBlack.AnimVecor.open, Vector3.zero, () => {
      EventEndPlashka();
    });

    // Устанавливаем значение жизней и выносливости
    SetStartHealth();

    PlayerPrefs.SetString("RunGo", DateTime.Now.ToString());    // Отметка о забеге

    AddBlockChangeMap(this);                                        // Сбрасвается блокировкиизменения уровня
    moneyInRace = 0;                                            // Обновляем монеты за уровень
    moneyInRaceCalc = 0;
    _runSpeedKoef = 1f;

    if (activeLevel == ActiveLevelType.ship)
      InitPowerValue();

    Questions.QuestionManager.Instance.InitQuestionsGroup();

    if (GameManager.activeLevelData.gameMode == GameMode.survival) {

      Helpers.Invoke(this, ForceStart, 0.5f);
      return;
    }

    if (GameManager.activeLevelData.gameMode == GameMode.survival) {
    } else {
      CameraController.Instance.Zoom(true);
    }

    if (GameManager.levelRestart) {
      StartCoroutine(restartLevel());
      AudioManager.BackMusic(gameClip, AudioMixerTypes.runnerMusic);
    } else {
      SetMenuMusic();
    }
  }

  protected override void OnDestroy() {
    base.OnDestroy();
    //RunSpawner.OnSpecialBarrier -= OnSpecialBarrier;
    gateController.OnEndGate -= CheckPointGate;
    OnDestroyFinalyGate();
  }

  static float _distanceChangeRegion = 0;

  /// <summary>
  /// Изменение региона
  /// </summary>
  /// <param name="newType"></param>
  [ExEvent.ExEventHandler(typeof(ExEvent.RunEvents.RegionChange))]
  void ChangeRegion(RunEvents.RegionChange region) {
    _distanceChangeRegion = playerDistantion;
  }

  /// <summary>
  /// Форсированный запуск
  /// </summary>
  public void ForceStart() {
    GameManager.startFromMap = false;
    StartRunFromMap();
    StartRun();
  }

  void SetStartPhase() {
    runnerPhase = RunnerPhase.start;
  }

  IEnumerator restartLevel() {
    yield return new WaitForSeconds(0.5f);
    GameManager.levelRestart = false;
    if (GameManager.activeLevelData.gameMode != GameMode.survival)
      StartDecor.Instance.StartClassicGame();
  }
  /// <summary>
  /// Событие установки плеера на паузу
  /// </summary>
  void OnApplicationPause() {
    if (!isPause && (runnerPhase == RunnerPhase.run | runnerPhase == RunnerPhase.boost)) {
      Pause();
    }
  }

  /// <summary>
  /// Событие закрытия приложения
  /// </summary>
  void OnApplicationQuit() {
    SaveResult();
  }

  void Update() {

    if (Input.GetButtonDown(pauseButton))
      Pause();

    // Если бег выполняется, рассчитываем позицию
    if (runnerPhase == RunnerPhase.run || runnerPhase == RunnerPhase.boost || runnerPhase == RunnerPhase.postBoost) {
      if (!stopCalcDistantion)
        CalcThisPosition();
    }

    // Стартовый буст
    if (runnerPhase == RunnerPhase.boost) {

      if (bonusTime < Time.time) {
        runnerPhase = RunnerPhase.postBoost;
        RemoveBlockChangeMap(this);
      }
    }

    // При смерти планенько уменьшаем скорость игрока
    if (runnerPhase == RunnerPhase.dead)
      needSpeed = needSpeed - 0.1f <= 0 ? 0 : needSpeed - 0.1f;

    if (runnerPhase == RunnerPhase.lowEnergy) {
      needSpeed = needSpeed - 0.1f <= 0 ? 0 : needSpeed - 0.1f;
      needSpeed = Mathf.Max(needSpeed, 2);
    }

    // Пока бежим постепенно уменьшаем запас здоровья
    if (((runnerPhase & (RunnerPhase.run | RunnerPhase.endRun | RunnerPhase.boss | RunnerPhase.tutorial)) != 0) && Time.timeScale > 0) {
      if (runSpeedNowReal < runSpeedMin)
        needSpeed += 0.2f;
      else {

        if (speedParametrsNextCalc != null && speedParametrsNextCalc <= thisDistantionPosition) {
          bool useDist = false;
          for (int i = 0; i < speedParametrs.Length; i++) {
            if (thisDistantionPosition >= speedParametrs[i].minDistance && thisDistantionPosition < speedParametrs[i].maxDistance) {
              useDist = true;
              speedParametrsNow = speedParametrs[i];

              if (i != speedParametrs.Length - 1)
                speedParametrsNextCalc = speedParametrs[i + 1].minDistance;
              else
                speedParametrsNextCalc = null;
            }
          }
          if (!useDist)
            speedParametrsNextCalc = null;
        }

        if (thisDistantionPosition >= speedParametrsNow.minDistance && thisDistantionPosition < speedParametrsNow.maxDistance) {
          needSpeed = runSpeedNowReal + (((speedParametrsNow.maxSpeed - speedParametrsNow.minSpeed) / (speedParametrsNow.maxDistance - speedParametrsNow.minDistance)) /* * Time.deltaTime */ );
          needSpeed = needSpeed < speedParametrsNow.maxSpeed ? needSpeed : speedParametrsNow.maxSpeed;
        }

        // Запоминаем большую скорость
        if (needSpeed > runSpeedNowMax)
          runSpeedNowMax = needSpeed;
        else {
          // При понижении уменьшении, скорость возростает быстрее
          needSpeed = runSpeedNowReal + (((speedParametrs[0].maxSpeed - speedParametrs[0].minSpeed) / (speedParametrs[0].maxDistance - speedParametrs[0].minDistance)) * 4 * Time.deltaTime);
          if (needSpeed > runSpeedNowMax)
            needSpeed = runSpeedNowMax;
        }
      }

      if (activeLevel == ActiveLevelType.ship)
        UpdatePower();

      // Умер
      if (healthManager.actualValue <= 0) {

        if (healthManager.type == HealthType.energy) {
          runnerPhase = RunnerPhase.lowEnergy;
        } else {
          runnerPhase = RunnerPhase.dead;
        }

      }
    }

    //if (!(GameManager.Instance.generateGate && GameManager.Instance.allKeys + RunnerController.Instance.keysInRaceCalc >= GameManager.Instance.needKeys) && levelDistance <= playerDistantion)
    //	CheckEndLevel();

    if (runnerPhase == RunnerPhase.endRun && PlayerController.Instance.transform.position.x > CameraController.displayDiff.rightDif(1.3f)) {
      PlayerController.Instance.gameObject.SetActive(false);
    }

  }

  private void FixedUpdate() {

    CalcSpeed();
  }

  // Переход на другую страницу
  IEnumerator EndRunner(int level) {
    Time.timeScale = 1f;
    yield return new WaitForSeconds(1f);
    SceneManager.LoadScene(level);
    yield return 0;
  }

  public void EventEndPlashka() {

    //GameManager.actionProcess = false;
    if (runnerPhase == RunnerPhase.start) {

      //StartScreenObjects.Instance.GetComponent<Animator>().SetTrigger("logo");
      StartCoroutine(WaitToTap());

      // Проверка днеыного баланса
      if (GameManager.startFromMap || !ShowDialogs()) {
        //UiController.mainMenu(true);
        ShowRunnerMenu(true);
      }
    }
  }

  #region Dialogs

  public bool ShowDialogs() {

    if (GameManager.Instance.isDebug && !GameManager.Instance.DebagValue("startDialogs")) return false;

    int gameRun = PlayerPrefs.GetInt("GameRun");                         // Число запусков игры
    int gameRunCheck = PlayerPrefs.GetInt("GameRunCheck");                    // Проверенное число запусков игры

    if (gameRun == gameRunCheck)
      return false;

    PlayerPrefs.SetInt("GameRunCheck", gameRun);

    if ((gameRun == 2 || (gameRun >= 10 && gameRun % 5 == 0)) && PlayerPrefs.GetInt("notificationRemoteRegister", 0) <= 0) {
      UiController.ShowPushDialog(() => { ShowRunnerMenu(true); });
      return true;
    } else if ((gameRun == 4 || (gameRun >= 10 && gameRun % 4 == 0)) && PlayerPrefs.GetInt("rate") == 0) {
      UiController.RateDialogShow(() => { ShowRunnerMenu(true); });
      return true;
    } else if ((gameRun == 8 || (gameRun >= 10 && gameRun % 15 == 0))) {
      UiController.ShareDialogShow(() => { ShowRunnerMenu(true); });
      return true;
    }
    return false;
  }

  #endregion

  public void GoToLevel(int level) {
    Time.timeScale = 1f;

    if (moneyInRaceCalc > 0 || cristallInRace > 0 || blackPointsInRace > 0)
      SaveResult();

    goToLevelReady = true;
    goToLevel = level;
  }

  public void ShowBlackBg() {
    if (!goToLevelReady)
      return;
    SceneManager.LoadScene(goToLevel);
  }

  IEnumerator WaitToTap() {
    yield return new WaitForSeconds(3f);
    if (runnerPhase == RunnerPhase.start)
      StartDecor.Instance.GetComponent<Animator>().SetTrigger("tap");
  }

  public void ActiveGamePlay() {
    GamePlayShow();
  }

  /// <summary>
  ///  Событие по изменению фазы забега
  /// </summary>
  /// <param name="newPhase"></param>
  void ChangeStatePahse(RunnerPhase newPhase) {

    if ((runnerPhase == RunnerPhase.boost || runnerPhase == RunnerPhase.postBoost) && newPhase == RunnerPhase.run)
      PlayerController.Instance.GetComponent<PlayerBoosters>().EndBoosters();

    if (newPhase == RunnerPhase.run) {
      needSpeed = runSpeedMin;
      StartCoroutine(WaitForNoChange(5));

    }
    if ((runnerPhase == RunnerPhase.start || runnerPhase == RunnerPhase.postBoost) && newPhase == RunnerPhase.run)
      Invoke(startRunDialogs, 0.5f);
    
    if (runnerPhase == RunnerPhase.dead && newPhase == RunnerPhase.run)
      PlayerController.Instance.animation.ResetAnimation();
    
    if (newPhase == RunnerPhase.postBoost)
      StartCoroutine(GoToPlayPhase());
    
    if (newPhase == RunnerPhase.boost)
      noShowBoostDialog = true;
    
    if (runnerPhase == RunnerPhase.start && newPhase != RunnerPhase.start)
      needSpeed = runSpeedMin;
    
    // При переходе в режим простого забега, отключаем блокировку изменения уровня
    if (runnerPhase == RunnerPhase.boost && newPhase == RunnerPhase.run)
      StartCoroutine(WaitForNoChange(3));

    if (newPhase == RunnerPhase.dead)
      if (playerDistantion >= 2200 && playerDistantion <= 2400)
        Questions.QuestionManager.ConfirmQuestion(Quest.deadDistance2200_2400, 1, transform.position);

    // При смерти плавненько завершаем
    if (newPhase == RunnerPhase.dead && !IsInvoking(GameOver)) {

      if (!gateDamage && CameraController.Instance.transform.position.y > 0) {

        if (!playerRessurection && (UserManager.Instance.ressurection > 0 || (UnityAdsController.adsReady && GameManager.DayAfterStart(1)))) {
          playerRessurection = true;
          ShowRessurectionPanel();
          //UiController.RessurectionOpen();

        } else {
          if (!IsInvoking(GameOver))
            Invoke(GameOver, 2);
        }

      } else {
        if (!IsInvoking(GameOver))
          Invoke(GameOver, 2);
      }

      if (gateDamage)
        needSpeed = 0;

      CalcSpeed(true);
    }
    runnerPhase = newPhase;
  }

  bool playerRessurection;                // Флаг, что плеер уже воскрешался
  void ShowRessurectionPanel() {
    RessurectionPanel ressurect = UiController.ShowUi<RessurectionPanel>();
    ressurect.gameObject.SetActive(true);

    ressurect.OnClose = () => {
      if (!IsInvoking(GameOver))
        Invoke(GameOver, 4);
      Destroy(ressurect.gameObject);
    };

    ressurect.OnVideo = () => {
      if (healthManager.actualValue <= 0)
        PlayerAddLive(1);
      powerValue = powerMax / 2;
      runnerPhase = RunnerPhase.run;
      ressurect.OnClose = () => {
        Destroy(ressurect.gameObject);
      };
    };

    ressurect.OnRessurection = () => {
      if (healthManager.actualValue <= 0)
        PlayerAddLive(1);
      powerValue = powerMax / 2;
      runnerPhase = RunnerPhase.run;
      ressurect.OnClose = () => {
        Destroy(ressurect.gameObject);
      };
    };

  }

  IEnumerator GoToPlayPhase() {

    float beforboostspeed = runSpeedMin + ((speedParametrs[0].maxSpeed - speedParametrs[0].minSpeed) / (speedParametrs[0].maxDistance - speedParametrs[0].minDistance)) * (thisDistantionPosition / runSpeedMin);
    float incr = (runSpeedNowReal - beforboostspeed) / 10;

    while (runSpeedNowReal > beforboostspeed) {
      yield return new WaitForSeconds(0.1f);
      needSpeed = runSpeedNowReal - incr;
      if (needSpeed < beforboostspeed)
        needSpeed = beforboostspeed;

      CalcSpeed(true);
    }
    runnerPhase = RunnerPhase.run;
    yield return 0;
  }

  IEnumerator AnimationReset(GameObject obj, float time) {
    yield return new WaitForSeconds(time);
    Animator objAnim = obj.GetComponent<Animator>();
    if (objAnim)
      objAnim.Rebind();
    yield return 0;
  }

  IEnumerator ObjectDeactive(GameObject obj, float time) {
    yield return new WaitForSeconds(time);
    obj.SetActive(false);
    yield return 0;
  }

  public static int barrierDamageCount; // Флаг удара о препядствие
  public static int doubleJumpCount;
  public static int spiderkill;

  void InitCountsQuest() {
    playerDamageCount = 0;
    shotCount = 0;
    barrierDamageCount = 0;
    doubleJumpCount = 0;
    spiderkill = 0;
  }

  int nextFixtDistReport;

  void InitMetricDistantion() {
    nextFixtDistReport = PlayerPrefs.GetInt("metricDistantionBlock") + 300;
  }

  int lastEventDistantion;
  public static Action<float> changeDistance;
  public delegate void ChangeDistanceQuest(float newDistance, bool damage, bool shoot, bool barrieDamage, bool doubleJump, bool spiderkill, bool activePetSpider, bool activePetDino);
  public static ChangeDistanceQuest changeDistanceQuest;

  // Рассчет текущей позиции
  private void CalcThisPosition() {
    thisDistantionPosition = thisDistantionPosition + runSpeedNowReal * (Time.deltaTime * .6f);
    //interfaces.ChangeDistantionPoint(thisDistantionPosition);
    playerDistantion = thisDistantionPosition;

    if (nextFixtDistReport <= thisDistantionPosition) {
      PlayerPrefs.SetInt("metricDistantionBlock", nextFixtDistReport);

      YAppMetrica.Instance.ReportEvent("Прохождение: преодалено " + nextFixtDistReport + " метров");

      GAnalytics.Instance.LogEvent("Прохождение", "Преодалено", nextFixtDistReport + " метров", 1);

      nextFixtDistReport += 300;
    }

    if (lastEventDistantion < thisDistantionPosition) {
      lastEventDistantion++;
      if (changeDistance != null)
        changeDistance(thisDistantionPosition);

      if (runnerPhase == RunnerPhase.run) {
        if (changeDistanceQuest != null)
          changeDistanceQuest(thisDistantionPosition, (playerDamageCount == 0), (shotCount == 0), (barrierDamageCount == 0), (doubleJumpCount == 0), (spiderkill == 0), (activePet == PetsTypes.spider), (activePet == PetsTypes.dino));
      }
    }

  }

  IEnumerator WaitForNoChange(float time = 5) {
    yield return new WaitForSeconds(time);
    RemoveBlockChangeMap(this);
  }

  /// <summary>
  /// Старт забега
  /// </summary>
  public void StartPlay() {
    //if (!UiController.checkStart())
    //	return;
    if (runnerPhase != RunnerPhase.start && runnerPhase != RunnerPhase.tutorial)
      return;
    StartRun();
  }

  /// <summary>
  /// Активация буста при старте
  /// </summary>
  /// <param name="bust"></param>
  public void ActivateBoostInRun(BoostType bust) {

    Questions.QuestionManager.AddUseBoost(bust);
    AddBlockChangeMap(this);
    booster = bust;
    StartBoost();
  }

  void StartRunFromMap() {
    PlayerController.Instance.GraphicVector(true);
  }

  /// <summary>
  /// Старт забега
  /// </summary>
  /// <param name="bust">Тип пуста. При none запускается простой забег</param>
  public void StartRun(BoostType bust = BoostType.none) {

    StartDecor.Instance.StartClassicRun();
    ExEvent.RunEvents.StartFirstRunAnim.Call();
    //PlayRun(bust);

  }

  public void PlayRun(BoostType bust = BoostType.none) {

    //if (!PlayerManager.instance.energy.isEnergy) {
    //	UiController.instance.EnergyPanelShow(true);
    //	return;
    //}
    ShowRunnerMenu(false);

    ParentCamera.CameraStop = false;
    booster = bust;

    //UiController.mainMenu(false);
    
    if (TutorialManager.Instance.Istutorial)
      Company.Live.LiveCompany.Instance.UseRun();

    CameraController.Instance.Zoom(false);
    PlayerController.Instance.GraphicVector(true);
    levelComplited = false;
    ActiveGamePlay();

    AudioManager.BackMusic(gameClip, AudioMixerTypes.runnerMusic);

    /// В случае не выполнения туториола (первого запуска) предлогаем игроку пройти туториал

    if (!TutorialManager.Instance.Istutorial) {
      runnerPhase = RunnerPhase.tutorial;
      return;
    }

    if (booster != BoostType.none)
      StartBoost();
    else {
      runnerPhase = RunnerPhase.run;
    }

    ShowRunnerMenu(false);

  }

  public void ShowRunnerMenu(bool isShow) {
    if (GameManager.activeLevelData.gameMode == GameMode.survival) {
      SurvivalMenuShow(isShow);
    } else {
      LevelMenuShow(isShow);
    }

  }

  private SurvivalMenuPanel menuSurvivle;

  private void SurvivalMenuShow(bool isShow) {

    if (menuSurvivle == null)
      menuSurvivle = UiController.ShowUi<SurvivalMenuPanel>();

    if (!isShow) {
      menuSurvivle.ClosePanel();
      menuSurvivle.OnClose = () => {
        Destroy(menuSurvivle);
      };
      return;
    }

    SurvivalMenuPanel survivalMenu = menuSurvivle.GetComponent<SurvivalMenuPanel>();

    menuSurvivle.gameObject.SetActive(isShow);

    survivalMenu.OnStartRun = () => {
      //StartScreenObjects.instance.OnTapScreen();

      if (!Company.Live.LiveCompany.Instance.isReady) {
        EnergySaleShow<HeartSaleDialog>(null);
        survivalMenu.isActiveAction = false;
      } else
        ExEvent.RunEvents.StartFirstRunAnim.Call();

    };

    survivalMenu.OnMap = () => {
      GameManager.LoadScene("Map");
    };

    survivalMenu.OnShop = () => {
      survivalMenu.OnClose = ShopDialogShow;
    };

    survivalMenu.OnCredit = () => {
      survivalMenu.OnClose = () => {
        CreditPanel creditPanel = UiController.ShowUi<CreditPanel>();
        creditPanel.gameObject.SetActive(true);
        creditPanel.OnClose = () => {
          ShowRunnerMenu(true);
        };
      };
    };

    survivalMenu.OnClose = () => {
      if (runnerPhase != RunnerPhase.start)
        Destroy(menuSurvivle);
    };

  }

  void ShopDialogShow() {
    ShopDialog shopDialog = UiController.ShowUi<ShopDialog>();
    shopDialog.gameObject.SetActive(true);

    shopDialog.OnClose = () => {

    };

    shopDialog.OnAction = (shopType, btnPos) => {
      Debug.Log(shopType);
      ShowShopPages(shopType, btnPos, () => {
        shopDialog.gameObject.SetActive(false);
      });
    };

  }

  #region Shop

  shopTypes needShop;
  public void ShowShopPages(shopTypes shopPages, Vector3 pos, Action OnComplited = null) {
    needShop = shopPages;


    FillBlack.Instance.PlayAnim(FillBlack.AnimType.round, FillBlack.AnimVecor.swich, pos, () => {
      //fullBlack.bg.raycastTarget = false;
      GameManager.Instance.ShowShopScene(() => {
      //FillBlack.Instance.PlayAnim(FillBlack.AnimType.round, FillBlack.AnimVecor.open, Vector3.zero);
        OnToShop();
        if (OnComplited != null) OnComplited();
      }, () => {

        FillBlack.Instance.PlayAnim(FillBlack.AnimType.round, FillBlack.AnimVecor.swich, Vector3.zero, () => {

          SceneManager.UnloadSceneAsync("Shop");
          CameraController.Instance.gameObject.SetActive(true);
          GuiCamera.instance.gameObject.SetActive(true);
          //SceneManager.UnloadScene("Shop");
          RunnerController.Instance.ShowRunnerMenu(true);
        });

      });
    });

  }

  void OnToShop() {

    CameraController.Instance.gameObject.SetActive(false);
    GuiCamera.instance.gameObject.SetActive(false);
    GameObject shopPanel = GameObject.Find("ShopMain");
    shopPanel.SetActive(true);
    shopPanel.GetComponent<ShopController>().ShowPages(needShop);

  }

  public void HideShopPanel() {

    CameraController.Instance.gameObject.SetActive(true);
    GuiCamera.instance.gameObject.SetActive(true);

    FillBlackShow(FillBlack.AnimType.round, FillBlack.AnimVecor.open, Vector3.zero);

    RunnerController.Instance.ShowRunnerMenu(true);
  }

  #endregion

  private LevelMenu lvlMenu;
  void LevelMenuShow(bool isShow) {
    if (lvlMenu == null)
      lvlMenu = UiController.ShowUi<LevelMenu>();

    if (!isShow)
      lvlMenu.gameObject.SetActive(false);

    if (isShow)
      lvlMenu.gameObject.SetActive(true);


    lvlMenu.OnShop = () => {
      lvlMenu.OnClose = () => {
        ShopDialogShow();
      };
      lvlMenu.HidePanel();
    };

    lvlMenu.OnForward = () => {

      LevelInfoShow(() => {

        lvlMenu.OnClose = () => {
          StartRun();
          Destroy(menuSurvivle);
        };
        lvlMenu.HidePanel();
      }, null);

    };

    lvlMenu.OnHome = () => {
      lvlMenu.OnClose = () => {
        GameManager.LoadScene("Menu");
      };
      lvlMenu.HidePanel();
    };

  }


  bool CheckeExistsEnergy() {
    return !(Company.Live.LiveCompany.Instance.value < Company.Live.LiveCompany.Instance.oneRunPrice);
  }

  void LevelInfoShow(Action OnStart, Action OnCancel) {
    LevelInfo levelInfo = UiController.ShowUi<LevelInfo>();
    levelInfo.gameObject.SetActive(true);
    levelInfo.SetLevel(GameManager.activeLevel);

    levelInfo.OnClose = OnCancel;

    levelInfo.OnForward = () => {

      if (!Company.Live.LiveCompany.Instance.isReady) {
        EnergySaleShow<HeartSaleDialog>(null);
      } else {
        levelInfo.OnClose = OnStart;
        levelInfo.Close();
      }

    };
  }

  public void EnergySaleShow<T>(Action OnClose) where T : PanelUi {
    T salePanel = UiController.ShowUi<T>();
    salePanel.gameObject.SetActive(true);
    salePanel.OnClose = OnClose;
  }

  public void StartBoost() {
    PlayerController.Instance.GetComponent<PlayerBoosters>().SetBoost();
    runnerPhase = RunnerPhase.boost;

    SetBoost();
  }

  public void StopSpeed() {
    needSpeed = 0;
    CalcSpeed(true);
  }

  void SetBoost(bool noSpeed = false) {

    foreach (Boosters boost in boosters) {
      if (booster == boost.type) {

        bonusTime = boost.time + Time.time;

        if (!noSpeed) {
          needSpeed = boost.speed;
          StartCoroutine(GoToBoostPhase());
        }
      }
    }
  }

  IEnumerator GoToBoostPhase() {
    float needSpeedReady = needSpeed;

    float incr = (needSpeedReady - runSpeedNowReal) / 10;

    while (needSpeedReady > runSpeedNowReal) {
      yield return new WaitForSeconds(0.1f);
      needSpeed = runSpeedNowReal + incr;
      CalcSpeed(true);
    }
    yield return 0;
  }

  public void SetPlaner() {
    generateItemBlock++;
    AddBlockChangeMap(this);
    booster = BoostType.planer;
    needSpeed = boosters[5].speed;
    bonusTime = boosters[5].time + Time.time;
    runnerPhase = RunnerPhase.planer;
  }

  /// <summary>
  /// Разрешение генерации
  /// </summary>
  public bool generateItems {
    get {
      return generateItemBlock <= 0;
    }
  }

  public void PlayerDead() {
    runnerPhase = RunnerPhase.dead;
  }

  /// <summary>
  /// Конец игры
  /// </summary>
  public void GameOver() {

    gamePlay.HideElements();
    gamePlay.gameObject.SetActive(false);
    SaveResult();
    GameOverContinue();
  }

  void GameOverContinue() {

    if (_finalyComics)
      OpenFinalyComics();
    else if (questionInRaceList.Count > 0)
      QuesionResultShow(() => {

        FillBlack.Instance.PlayAnim(FillBlack.AnimType.full, FillBlack.AnimVecor.open, Vector2.zero, () => {
          Time.timeScale = 1;
          ShowResultRunPanel();
        });

      });
    else
      ShowResultRunPanel();
    //UiController.statictic();
  }

  void CheckPointGate(int gateNum) {
    //readyEnd = true;
    CheckEndLevel();
  }

  /// <summary>
  /// Игрок упал
  /// </summary>
  public void PlayerFall() {
    GarpunPanel garpun = UiController.ShowUi<GarpunPanel>();
    garpun.gameObject.SetActive(true);

    garpun.OnClose = () => {
      PlayerController.Instance.PlayerFallConfirm(false);
    };

    garpun.OnGarpun = () => {

      UserManager.Instance.savesPerk--;

      garpun.OnClose = () => { };
    };

    garpun.OnVideo = () => {

      garpun.OnClose = () => {
        PlayerController.Instance.PlayerFallConfirm(true);
      };
    };

  }

  #region Работа со сценой
  string loadToScene;             // Загружаемая сцена

  void QuesionResultShow(Action OnComplited = null) {

    QuestionsEndGame questionEnd = UiController.ShowUi<QuestionsEndGame>();
    questionEnd.gameObject.SetActive(true);

    questionEnd.OnClose = () => {
      if (OnComplited != null) OnComplited();
    };

    questionEnd.OnGate = () => {
      AudioManager.SetSoundMixer(AudioSnapshotTypes.runnerEffect0, 0.5f);
      AudioManager.SetSoundMixer(AudioSnapshotTypes.runnerMusic0, 0.5f);

      questionEnd.OnClose = () => {
        GameManager.ShowGateScene(() => {
          AudioManager.SetSoundMixer(AudioSnapshotTypes.runnerEffect50, 0.5f);
          AudioManager.SetSoundMixer(AudioSnapshotTypes.runnerMusic50, 0.5f);
          if (OnComplited != null) OnComplited();
        });
      };

      questionEnd.ClosePanelNow();

    };
  }

  #endregion

  void ShowResultRunPanel() {

    StaticPanelController statistic = UiController.ShowUi<StaticPanelController>();
    statistic.gameObject.SetActive(true);

    statistic.OnClose = () => { };

    statistic.OnForward = () => {
      Destroy(PlayerController.Instance);

      if (GameManager.activeLevelData.gameMode == GameMode.survival) {
        GameManager.startFromMap = true;
        GameManager.LoadScene(SceneManager.GetActiveScene().name);
      } else {
        LevelInfoShow(() => {
          GameManager.startFromMap = true;
          GameManager.LoadScene(SceneManager.GetActiveScene().name);
        }, null);
      }

    };

    statistic.OnHome = GoHome;

  }


  /// <summary>
  /// Сохраняем данные забега
  /// </summary>
  public void SaveResult() {

    int allCoins = UserManager.coins + moneyInRaceCalc;
    int allCristall = UserManager.Instance.cristall + cristallInRace;
    int allBlackMark = UserManager.Instance.blackMark + blackPointsInRaceCalc;
    int allKeys = UserManager.Instance.keys + keysInRaceCalc;

    if (keysInRaceCalc > 0) {
      levelComplited = true;
      GameManager.Instance.ChangeLevel(GameManager.level + keysInRaceCalc);

      YAppMetrica.Instance.ReportEvent("Квесты: " + allKeys + " ключей");
      GAnalytics.Instance.LogEvent("Квесты", "Собранно", allKeys + " ключей", 1);

    }

    UserManager.coins = allCoins;
    UserManager.Instance.cristall = allCristall;
    UserManager.Instance.blackMark = allBlackMark;
    UserManager.Instance.keys = allKeys;

    if (GameManager.activeLevelData.gameMode == GameMode.survival) {
      float maxDistance = UserManager.Instance.survivleMaxRunDistance;
      float newMaxDistance = (maxDistance > thisDistantionPosition ? maxDistance : thisDistantionPosition);
      UserManager.Instance.survivleMaxRunDistance = newMaxDistance;
    }



    PlayerPrefs.Save();

#if UNITY_IOS && !UNITY_EDITOR
       // GameCenterController.instance.ReportDistance(newMaxDistance);
#endif
#if (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
       // Apikbs.instance.SaveLeaderboardFbValue(newMaxDistance);
#endif
    moneyInRaceCalc = 0;
    blackPointsInRaceCalc = 0;
    keysInRaceCalc = 0;
  }
  /// <summary>
  /// Рассчет текущей скорости
  /// </summary>
  /// <param name="fors"></param>
  void CalcSpeed(bool fors = false) {

    if (stopRun)
      return;

    if (activeLevel == ActiveLevelType.ship) {
      // Скорость изменяется в зависимости от запаса силы

      runSpeedNowReal = runnerPhase == RunnerPhase.run
                      ? speedParametrs[0].minSpeed + ((speedParametrs[0].maxSpeed - speedParametrs[0].minSpeed) * powerValue / powerMax)
                      : 0;

      //runSpeedActual = runSpeedNowReal * 0.65f;
      //RunSpeed = runSpeedActual;
      return;
    }

    if (activeLevel == ActiveLevelType.classic) {
      // На стандартном уровне скорость линейно увеличивается на участках
      if (needSpeed != runSpeedNowReal && !fors) {
        if (runSpeedNowReal > needSpeed) {
          runSpeedNowReal -= 0.3f;
          runSpeedNowReal = runSpeedNowReal <= needSpeed ? needSpeed : runSpeedNowReal;
        } else if (runSpeedNowReal < needSpeed) {
          runSpeedNowReal += 0.3f;
          runSpeedNowReal = runSpeedNowReal >= needSpeed ? needSpeed : runSpeedNowReal;
        }
      } else
        runSpeedNowReal = needSpeed;

      //runSpeedActual = runSpeedNowReal * 0.65f;
      //RunSpeed = runSpeedActual;
    }
  }

  public static bool cameraStop;

  public void LevelEnd() {
    CheckEndLevel();
  }

  void CheckEndLevel() {

    //if (!readyEnd) {
    //	readyEnd = true;
    //	//levelDistance += 50;
    //	generateItemBlock += 1;
    //} else if (!complitedLevel) {
    //  runnerPhase = RunnerPhase.endRun;
    //	Helpers.Invoke(this, GameOver, 3);
    //	complitedLevel = true;
    //	ParentCamera.CameraStop = true;
    //	stopCalcDistantion = true;

    //}
    generateItemBlock += 1;
    runnerPhase = RunnerPhase.endRun;
    Helpers.Invoke(this, GameOver, 3);
    //complitedLevel = true;
    ParentCamera.CameraStop = true;
    stopCalcDistantion = true;
  }

  bool stopRun = false;                   // Остановка забега
                                          /// <summary>
                                          /// Моментальная остановка забега
                                          /// </summary>
                                          /// <param name="flag"></param>
  public void StopRunNow(bool flag = true) {
    needSpeed = 0;
    CalcSpeed(true);
    stopRun = flag;
  }
  /// <summary>
  /// Сохраняем запись об открытии ворот
  /// </summary>
  /// <param name="numGate">Номер ворот</param>
  /// <param name="keysCount">Количество ключей</param>
  public void OpenGate(int numGate, int keysCount) {
    PlayerPrefs.SetInt("openGate", numGate);
    UserManager.Instance.keys -= keysCount;

    YAppMetrica.Instance.ReportEvent("Квесты: открыты ворота №" + numGate);

    GAnalytics.Instance.LogEvent("Квесты", "Открыто", "ворота №" + numGate, 1);

  }

  public static event Action<int> OnCoinsInRace;

  /// <summary>
  /// Добавление собранных монет
  /// </summary>
  /// <param name="cnt">Сначение номинала</param>
  public static void addRaceCoins(int cnt) {
    Instance.AddRaceCoins(cnt);
  }

  public void AddRaceCoins(int cnt) {
    if (cnt <= 0)
      return;

    moneyInRace += cnt;
    moneyInRaceCalc += cnt;
    Questions.QuestionManager.ConfirmQuestion(Quest.getCoins, cnt);
    if (OnCoinsInRace != null) OnCoinsInRace(moneyInRace);
    //RunnerGamePlay.ChangeCoinsCount(moneyInRace);
  }

  /// <summary>
  /// Добавление черной метки
  /// </summary>
  /// <param name="cnt"></param>
  public void AddBlackMark(int cnt) {
    if (cnt <= 0)
      return;

    blackPointsInRace += cnt;
    blackPointsInRaceCalc += cnt;
  }

  #region Игрок

  public List<HealthManagerBase> healthManagersList;

  public HealthManagerBase healthManager { get; private set; }

  private float playerDamageCount = 0;              // Количество ударов полученное игроком

  void SetStartHealth() {

    if (healthManager == null) {

      GameObject manager = Instantiate(healthManagersList.Find(x => x.type == GameManager.activeLevelData.healthType).gameObject);
      manager.transform.SetParent(transform);
      healthManager = manager.GetComponent<HealthManagerBase>();
    }
    healthManager.Init();

  }

  /// <summary>
  /// Повреждения наносимые пользователю
  /// </summary>
  /// <param name="damage">Сила повреждения</param>
  public void PlayerDamager(float damage) {

    if (GameManager.Instance.isDebug && isGod)
      return;

    healthManager.LiveChange(-damage);

    playerDamageCount++;
    CameraController.Instance.GoBloom();

    // Изменение скорости при дамаге
    needSpeed = runSpeedNowReal * 0.8f;
    needSpeed = needSpeed < runSpeedMin ? runSpeedMin : needSpeed;

    if (gateDamage)
      needSpeed = 0;

    CalcSpeed(true);
  }

  public void AddHeartPerk() {
    if (UserManager.Instance.liveAddPerk > 0) {
      healthManager.LiveChange(1);
      UserManager.Instance.liveAddPerk--;
    }
  }

  // Подобранный бонус соружием
  public void PlayerAddLive(float value) {
    healthManager.LiveChange(value);
  }

  #endregion

  #region Player Attack
  // Проверка возможности выстрелить
  public bool PlayerCanShoot { get { return playerWeaponCount > 0; } }
  private int shotCount = 0;

  // Выстрел игрока
  public bool PlayerShoot() {
    if (playerWeaponCount <= 0)
      return false; // Снаряды закончились
    shotCount++;
    --playerWeaponCount;
    ExEvent.GameEvents.WeaponActiveChange.Call(playerWeapon, playerWeaponCount);
    //gamePlay.SetWeapon(playerWeapon, playerWeaponCount);
    return true;
  }

  #endregion

  private ClothesBonus weaponClothes;

  /* *****************************
	 * Добавляем количество снарядов
	 * count - число добавляемых
	 * *****************************/
  public void WeaponAdd(BoxType boxType) {

    //Questions.QuestionManager.AddWeapon(1, player.transform.position);
    Questions.QuestionManager.ConfirmQuestion(Quest.getWeapon, 1, PlayerController.Instance.transform.position);

    int count = (isDoubleWeapon || weaponClothes.full ? 2 : 1);

    switch (boxType) {
      case BoxType.weaponTrap:
        playerWeaponCount = (playerWeapon == WeaponTypes.trap ? playerWeaponCount + count : count);
        playerWeapon = WeaponTypes.trap;
        break;
      case BoxType.weaponSabel:
        playerWeaponCount = (playerWeapon == WeaponTypes.sabel ? playerWeaponCount + count : count);
        playerWeapon = WeaponTypes.sabel;
        break;
      case BoxType.weaponGun:
        playerWeaponCount = (playerWeapon == WeaponTypes.gun ? playerWeaponCount + count : count);
        playerWeapon = WeaponTypes.gun;
        break;
      case BoxType.weaponMolotov:
        playerWeaponCount = (playerWeapon == WeaponTypes.molotov ? playerWeaponCount + count : count);
        playerWeapon = WeaponTypes.molotov;
        break;
      case BoxType.weaponBomb:
        playerWeaponCount = (playerWeapon == WeaponTypes.bomb ? playerWeaponCount + count : count);
        playerWeapon = WeaponTypes.bomb;
        break;
      case BoxType.weaponShip:
        playerWeaponCount = (playerWeapon == WeaponTypes.ship ? playerWeaponCount + count : count);
        playerWeapon = WeaponTypes.ship;
        break;
      case BoxType.weaponChocolate:
        playerWeaponCount = (playerWeapon == WeaponTypes.chocolate ? playerWeaponCount + count : count);
        playerWeapon = WeaponTypes.chocolate;
        break;
      case BoxType.weaponFlowers:
        playerWeaponCount = (playerWeapon == WeaponTypes.flowers ? playerWeaponCount + count : count);
        playerWeapon = WeaponTypes.flowers;
        break;
      case BoxType.weaponCandy:
        playerWeaponCount = (playerWeapon == WeaponTypes.candy ? playerWeaponCount + count : count);
        playerWeapon = WeaponTypes.candy;
        break;
    }

    //gamePlay.SetWeapon(playerWeapon, playerWeaponCount);
    ExEvent.GameEvents.WeaponActiveChange.Call(playerWeapon, playerWeaponCount);
  }

  void UpdateValue() {
    weaponClothes = Config.GetActiveCloth(ClothesSets.moreBox);
    SetStartHealth();
    InitMagic();
  }

  #region Pause
  private bool isPause;
  /// <summary>
  /// Установка паузы в забеге
  /// </summary>
  /// <param name="anim">Выполнять анимацию появления интерфейса паузы</param>
  public void Pause(bool anim = true) {
    if (isPause) {
      //UiController.pause(!isPause);
      AudioManager.HidePercent50Sound(false, true);
      //ShowPauseUi(!isPause);
      //applicatePause = false;
      isPause = false;

      //if (runnerPhase == RunnerPhase.tutorial) UiController.Tutorial(true);
    } else {
      StopCoroutine(PauseToPlay());
      Time.timeScale = 0f;
      //UiController.pause(!isPause, anim);
      ShowPauseUi(!isPause, anim);
      AudioManager.HidePercent50Sound(true, true);
      isPause = true;
      if (SetPause != null)
        SetPause(isPause);
    }
  }


  /// <summary>
  /// Показать ГУИ паузы
  /// </summary>
  /// <param name="active">Показать</param>
  /// <param name="animate">Анимировать появление</param>
  void ShowPauseUi(bool active, bool animate = true) {
    PausePanel pausePanel = UiController.ShowUi<PausePanel>();
    pausePanel.gameObject.SetActive(true);
    pausePanel.Show(true);

    pausePanel.OnHome = () => {
      SaveResult();

      if (questionInRaceList.Count > 0) {
        QuesionResultShow(() => {
          GoHome();
        });
      } else
        GoHome();
    };

    pausePanel.OnContinue = () => {
      Pause();
    };

    pausePanel.OnClose = () => {
      if (true) {
        ShowTimerUi();
      }
    };

  }

  void GoHome() {
    if (GameManager.activeLevelData.gameMode == GameMode.levelsClassic)
      GameManager.LoadScene(SceneManager.GetActiveScene().name);
    else
      GameManager.LoadScene("Map");
  }

  /// <summary>
  /// Показать таймер
  /// </summary>
  void ShowTimerUi() {
    TimerPauseController timerPanel = UiController.ShowUi<TimerPauseController>();
    timerPanel.gameObject.SetActive(true);
    timerPanel.GoTimer();
    timerPanel.OnStart = () => {
      isPause = false;
      Time.timeScale = 0.2f;
      StartCoroutine(PauseToPlay());
    };
  }

  #endregion


  public void SetBossPhase() {
    runnerPhase = RunnerPhase.boss;
  }

  public void SetRunPhase() {
    runnerPhase = RunnerPhase.run;
  }

  IEnumerator PauseToPlay() {
    while (Time.timeScale < 1) {
      Time.timeScale += 0.2f;
      Time.timeScale = (Time.timeScale > 1 ? 1 : Time.timeScale);
      yield return new WaitForSeconds(0.1f);
    }
    if (SetPause != null)
      SetPause(isPause);
  }

  #region Определение нанесения повреждения
  [Header("Повреждения от объектов")]
  public BarrierDamageParametrs[] barrierDamageValue;

  public static float barrierDamage(string typeBarrier, bool isPlayer = false) {
    return Instance.BarrierDamage(typeBarrier, isPlayer);
  }

  public float BarrierDamage(string typeBarrier, bool isPlayer = false) {
    foreach (BarrierDamageParametrs barrier in barrierDamageValue) {
      if (barrier.barrierTag.ToString() == typeBarrier) {
        if (isPlayer)
          return barrier.toPlayer;
        else
          return barrier.toEnemy;
      }
    }
    return 0;
  }

  #endregion

  #region Магия

  private int bulletMagicCount;                           //Запас магии ядра
  private int piratMagicCount;                            // Запас магии пирата
                                                          /// <summary>
                                                          /// Инициализация магии
                                                          /// </summary>
  void InitMagic() {
    bulletMagicCount = PlayerPrefs.GetInt(ShopElementType.bulletMagic.ToString());
    piratMagicCount = PlayerPrefs.GetInt(ShopElementType.piratMagic.ToString());
  }
  /// <summary>
  /// Общий запас магии
  /// </summary>
  public int allMagicCount {
    get {
      return piratMagicCount + bulletMagicCount;
    }
  }
  /// <summary>
  /// Активация магии
  /// </summary>
  public void MagicButtom() {

    if (piratMagicCount > 0) {
      MagicSpawner.Instance.Magic(MagicTypes.pirats);
      piratMagicCount--;
      PlayerPrefs.SetInt(ShopElementType.piratMagic.ToString(), piratMagicCount);
    } else if (bulletMagicCount > 0) {
      MagicSpawner.Instance.Magic(MagicTypes.bullet);
      bulletMagicCount--;
      PlayerPrefs.SetInt(ShopElementType.bulletMagic.ToString(), piratMagicCount);
    } else
      return;

    //RunnerGamePlay.MagicCountSet(allMagicCount);
  }

  #endregion

  #region 14 февраля
  // Навигация для перехода на уровень 14 февраля
  public void Button14february() {
    UiController.ClickButtonAudio();
    GoToLevel(3);
  }

  #endregion

  #region Запас сил для 14 февраля

  public float powerMax;                          // Максимальный запас сил джека
  public static float powerValue;                 // Текущий запас сил

  // Инициализация запаса сил
  void InitPowerValue() {
    powerValue = powerMax;
  }

  // Событие обнрвления
  void UpdatePower() {
    // Расход сил со скорость 1 в секунду
    powerValue -= 1 * Time.deltaTime;
    RunnerGamePlay.SetPowerValue(powerMax, powerValue);

    if (powerValue <= 0)
      runnerPhase = RunnerPhase.dead;
  }

  /// <summary>
  /// Уменьшение запаса сил
  /// </summary>
  /// <param name="value">Значение, на которое необходимо уменьшить</param>
  public void PowerDamage(float value) {
    powerValue -= value;
    RunnerGamePlay.SetPowerValue(powerMax, powerValue);
  }

  /// <summary>
  /// Добавление запаса сил
  /// </summary>
  /// <param name="value"></param>
  public void AddPower(float value) {
    powerValue += value;

    if (powerMax < powerValue)
      powerValue = powerMax;

    RunnerGamePlay.SetPowerValue(powerMax, powerValue);
  }

  #endregion

  /// <summary>
  /// Перезагрузка сцены
  /// </summary>
  void ReloadScene() {
    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
  }

  #region Стартовые диалоги
  int startDialogCount = -1;
  bool noShowBoostDialog;
  public static void StartRunDialogs() {
    if (Instance.startDialogCount == -1)
      Instance.startDialogCount = 3;
    if (Instance.startDialogCount == 0)
      return;

    // Запрет па выполнение вопторного буста
    if (Instance.noShowBoostDialog && Instance.startDialogCount == 1)
      Instance.startDialogCount--;

    if (Instance.startDialogCount == 3) {
      Instance.startDialogCount--;
      Instance.InitDoubleVeapon();
    } else if (Instance.startDialogCount == 2) {
      Instance.startDialogCount--;
      Instance.InitAddHeart();
    } else if (Instance.startDialogCount == 1) {
      Instance.startDialogCount--;
      Instance.InitBoostInRun();
    }
  }

  public void startRunDialogs() {
    StartRunDialogs();
  }

  #endregion

  #region Двойное оружие

  [HideInInspector]
  public bool isDoubleWeapon;

  public void SetDoubleWeapon() {
    if (UserManager.Instance.doubleWeapon > 0) {
      UserManager.Instance.doubleWeapon--;
      isDoubleWeapon = true;
    }
  }

  void InitDoubleVeapon() {

    int doubleWeapon = UserManager.Instance.doubleWeapon;

    if (doubleWeapon > 0 && !weaponClothes.full)
      ShowDoubleWeaponPanel();
    //UiController.DoubleWeaponShow();
    else
      StartRunDialogs();


  }

  void ShowDoubleWeaponPanel() {
    DoubleWeaponPanel doubleWeapon = UiController.ShowUi<DoubleWeaponPanel>();
    doubleWeapon.gameObject.SetActive(true);

    doubleWeapon.OnClose = () => {
      StartRunDialogs();
    };

    doubleWeapon.OnAction = () => {
      SetDoubleWeapon();
    };

  }


  void InitAddHeart() {

    if (UserManager.Instance.liveAddPerk > 0)
      ShowHeartAddPanel();
    //UiController.HeartAddShow();
    else
      StartRunDialogs();
  }

  #endregion

  void ShowHeartAddPanel() {

    HeartAddPanel heartAdd = UiController.ShowUi<HeartAddPanel>();
    heartAdd.gameObject.SetActive(true);
    heartAdd.OnClose = StartRunDialogs;
    heartAdd.OnActive = AddHeartPerk;

  }

  #region Выбор буста

  void InitBoostInRun() {
    int allCount = (UserManager.Instance.runBoost > 0 ? 1 : 0)
                 + (UserManager.Instance.skateBoost > 0 ? 1 : 0)
                 + (UserManager.Instance.barrelBoost > 0 ? 1 : 0)
                 + (UserManager.Instance.millWhellBoost > 0 ? 1 : 0)
                 + (UserManager.Instance.shipBoost > 0 ? 1 : 0);

    if (allCount > 0) {
      //UiController.BoostPanelShow();
      ShowBoostSelectPanel();
    } else {
      StartRunDialogs();
    }
  }

  void ShowBoostSelectPanel() {
    BoostPanel boostPanel = UiController.ShowUi<BoostPanel>();
    boostPanel.gameObject.SetActive(true);

    boostPanel.OnClose = () => {
      StartRunDialogs();
      Destroy(boostPanel.gameObject);
    };

    boostPanel.OnActive = ActivateBoostInRun;

  }

  #endregion

  private bool gateDamage;
  public GameObject gateBoobSfx;

  /// <summary>
  /// Получение повреждений от ворот
  /// </summary>
  /// <param name="pos">Координаты ворот</param>
  public void GateDamage(Vector3 pos) {

    runnerPhase = RunnerPhase.run;

    PlayerController.Instance.ThisDamage(WeaponTypes.gate, DamagType.live, 1000, transform.position);
    PlayerController.Instance.transform.position = pos + Vector3.left;

    GameObject inst = Instantiate(gateBoobSfx, PlayerController.Instance.transform.position, Quaternion.identity) as GameObject;
    Destroy(inst, 20);

    gateDamage = true;

    //Questions.QuestionManager.addDeadOfGate(transform.position);
    Questions.QuestionManager.ConfirmQuestion(Quest.deadOfGate, 1, transform.position);
    StopRunNow();
  }


  #region Музыка

  public AudioClip menyClip;                // Музыка меню
  public AudioClip gameClip;                // Музыка забега
                                            /// <summary>
                                            /// Установка музыки для меню
                                            /// </summary>
  public static void SetMenuMusic() {
    AudioManager.BackMusic(Instance.menyClip, AudioMixerTypes.music);
  }

  #endregion

  /// <summary>
  /// Генерация специального препядствия
  /// </summary>
  [ExEvent.ExEventHandler(typeof(ExEvent.RunEvents.SpecialBarrier))]
  void OnSpecialBarrier(ExEvent.RunEvents.SpecialBarrier special) {

    if (special.isActivate && special.barrier == SpecialBarriersTypes.stickyFlor)
      AddBlockChangeMap(this);
    else if (!special.isActivate && special.barrier == SpecialBarriersTypes.stickyFlor)
      RemoveBlockChangeMap(this);
  }

  #region Петы

  Player.Jack.PetsTypes activePet;

  // Событие изменения состояния петов
  public static event System.Action<PetsTypes, bool, float> petsChange;

  /// <summary>
  /// Изменение статуса общения с петом
  /// </summary>
  /// <param name="pet">Тип пета</param>
  /// <param name="useFlag">Флаг использования</param>
  public static void petActivate(Player.Jack.PetsTypes pet, bool useFlag, float timeUse = 0) {

    Instance.activePet = useFlag ? pet : PetsTypes.none;

    Instance._runSpeedKoef = Instance.activePet == PetsTypes.dino ? 2.1f : 1f;

    if (petsChange != null)
      petsChange(pet, useFlag, timeUse);
  }

  #endregion

  #region Финальная анимация

  bool _finalyComics;
  bool readyFinaly;
  bool finaleComicsLoad;

  void InitFinalyAnim() {

#if UNITY_EDITOR
    PlayerPrefs.SetInt("endComics", 0);
#endif

    if (GameManager.Instance.showComics) {
      readyFinaly = PlayerPrefs.GetInt("endComics", 0) == 0 ? true : false;
      if (readyFinaly) {
        gateController.OnEndGate += GateCheck;
      }
    }
  }

  /// <summary>
  /// Деактиваия финальных ворот
  /// </summary>
  void OnDestroyFinalyGate() {
    if (readyFinaly) {
      gateController.OnEndGate -= GateCheck;
    }
  }

  void GateCheck(int gateNum) {

    if (gateNum < 2)
      return;
    _finalyComics = true;
    PlayerPrefs.SetInt("endComics", 1);
    OnDestroyFinalyGate();
  }

  /// <summary>
  /// Запуск финального комикса
  /// </summary>
  void OpenFinalyComics() {
    if (!_finalyComics)
      CloseFinalyComics();
    FinalGate();
  }
  /// <summary>
  /// Событие закрытия финального комикса
  /// </summary>
  void CloseFinalyComics() {
    Comics.OnCloseComics -= CloseFinalyComics;

    if (finaleComicsLoad) {
      AudioManager.SetSoundMixer(AudioSnapshotTypes.runnerEffect50, 0.5f);
      AudioManager.SetSoundMixer(AudioSnapshotTypes.runnerMusic50, 0.5f);
    }

    //UiController.OpenLevel();
    FillBlackShow(FillBlack.AnimType.round, FillBlack.AnimVecor.open, Vector3.zero);
    GameOverContinue();
  }
  /// <summary>
  /// Загрузка фанального помикса при пересечении последних ворот
  /// </summary>
  public void FinalGate() {

    if (finaleComicsLoad)
      return;
    finaleComicsLoad = true;
    _finalyComics = false;

    AudioManager.SetSoundMixer(AudioSnapshotTypes.runnerEffect0, 0.5f);
    AudioManager.SetSoundMixer(AudioSnapshotTypes.runnerMusic0, 0.5f);

    PlayerController.Instance.gameObject.SetActive(false);

    AudioManager.StopBackMusic();
    FillBlackShow(FillBlack.AnimType.round, FillBlack.AnimVecor.close, Vector3.zero);
    //UiController.CloseLevel();

    Comics.OnCloseComics += CloseFinalyComics;
    Invoke("LoadFinalyComics", 2);
  }
  /// <summary>
  /// Загрузка финального комикса
  /// </summary>
  void LoadFinalyComics() {
    SceneManager.LoadScene("Comics", LoadSceneMode.Additive);
  }

  #endregion

  void FillBlackShow(FillBlack.AnimType animType, FillBlack.AnimVecor animVector, Vector3 targetCenter, System.Action OnCmplited = null) {

    FillBlack.Instance.PlayAnim(animType, animVector, targetCenter, () => {
      if (animVector == FillBlack.AnimVecor.open)
      if (OnCmplited != null) OnCmplited();
    });
  }


}