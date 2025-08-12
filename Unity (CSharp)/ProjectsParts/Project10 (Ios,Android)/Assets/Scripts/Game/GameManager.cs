using UnityEngine;
using UnityEngine.SceneManagement;
using MiniJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using EditRun;
using ExEvent;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_IOS
using UnityEngine.iOS;
#endif

/// <summary>
/// Режим игры
/// </summary>
[Flags]
public enum GameMode {
  none = 0,
  survival = 1,                 // Бег на выживание
  levelsClassic = 2,            // Классический бег
  levelsConstructor = 4         // Бег с конструктором
}

/// <summary>
/// Вариант используемых локаций
/// </summary>
public enum GameLocation {
  classic = 0,                    // Классический забег
  ship = 1                        // Забег на корабле
}

/// <summary>
/// Вектор направления движения
/// </summary>
public enum MoveVector {
  right = 0,              // Движение вправо
  left = 1,               // Движение влево
  top = 2,                // Движение вверх
  bottom = 3              // Движение вниз
}

/// <summary>
/// Режим движения
/// </summary>
public enum GameMechanic {
  run = 0,                    // Простой бег
  jetPack = 1                 // Режим джетпака
}

/// <summary>
/// Общий контроллер игры
/// </summary>
public class GameManager: Singleton<GameManager> {

  public LevelData _activeLevelData;

  public static LevelData activeLevelData {
    get { return Instance._activeLevelData; }
    set { Instance._activeLevelData = value; }
  }

  public bool isDebug;                              // Дебаг

  public enum saveType { _int, _float, _string, _bool }

  [Serializable]
  public struct SavesParameters {
    public string name;
    public string parametr;
    public string startValue;
    public string value;
    public string description;
    public saveType type;
  }

  public static event Action OnStartLoadScene;

  public static bool startFromMap;             // Запуск забега с карты

  public SavesParameters[] saveParameters;

  public static bool levelRestart;                    // Перезапуск левела
  public static bool isFirstStart;

  public List<MapRegion> mapRun;                               // Настройки карты
  public static int levelCount;                       // Количество уровней

  public bool showComics;
  protected override void Awake() {
    base.Awake();
    
    DontDestroyOnLoad(this);

    InitDebugParametrs();
    // Получаем настройки
    GetConfig();

    //InitLevel();

#if UNITY_IOS || UNITY_ANDROID
    //SetTargetFrameRate(FRAME_RATE);
#endif

    InitFacebook();
    InitApiKbs();
    CheckVersion();
    InitPlayerParametrs();
  }

  void Start() {
    Helpers.Invoke(this, CheckPush, 0.2f);
  }

  protected override void OnDestroy() {
    SaveDebugParametrs();
    base.OnDestroy();
  }

  #region Facebook

  public GameObject facebookPrefab;
  private GameObject facebookInstance;

  void InitFacebook() {

#if PLUGIN_FACEBOOK
		if (facebookInstance == null) {
			facebookInstance = Instantiate(facebookPrefab) as GameObject;
			facebookInstance.transform.parent = transform;
		}
#endif
  }

  private void Update() {

    if (Input.GetKeyDown(KeyCode.Escape)) {
      BackEvent();
    }

  }


  #endregion

  #region Стек вызова панелей

  protected Stack<PanelUi> _stackPanels;

  public void AddStack(PanelUi panel) {

    if (_stackPanels == null) _stackPanels = new Stack<PanelUi>();

    _stackPanels.Push(panel);
  }
  public void PopStack(PanelUi panel) {
    if (_stackPanels.Count > 0)
      _stackPanels.Pop();
  }

  public void BackEvent() {

    if (_stackPanels.Count > 1) {
      PanelUi panel = _stackPanels.Peek();
      panel.BackButton();
      return;
    }
    Debug.Log("Stack count " + _stackPanels.Count);

    switch (SceneManager.GetActiveScene().name) {
      case "Menu":
        Application.Quit();
        break;
      case "Map":
        LoadScene("Menu");
        break;
      case "ClassicRun":
        LoadScene(((activeLevelData.gameMode & GameMode.survival) != 0) ? "Map" : "Menu");
        break;
      case "ShipRun":
        LoadScene("Menu");
        break;
    }
  }
  /// <summary>
  /// Событие загрузки сцены
  /// </summary>
  /// <param name="scene"></param>
  /// <param name="mode"></param>
  private void OnChangeMainScene() {
    _stackPanels.Clear();
    Debug.Log("Stack count " + _stackPanels.Count);
  }

  #endregion

  #region ApiKbs

  public GameObject apiKbsPrefab;
  GameObject apiKbsInstance;

  void InitApiKbs() {

    if (apiKbsInstance == null) {
      apiKbsInstance = Instantiate(apiKbsPrefab) as GameObject;
      apiKbsInstance.transform.parent = transform;
    }
  }

  #endregion

  #region Управление сценой

  string activeSene;
  public static string needScene = "";

  // Загрузка уровня
  public static void LoadScene(string sceneName, bool force = false) {
    if (Instance == null || sceneName == "" || Instance.waitLoadGateScene) return;
    if (force)
      Instance.LoadLevelNow(sceneName);
    else
      Instance.LoadScene_(sceneName);
  }

  void LoadLevelNow(string sceneName) {
    needScene = sceneName;
    if (isFirstStart) {
      if (GameManager.OnStartLoadScene != null) GameManager.OnStartLoadScene();
      SceneManager.LoadScene("Comics");
    } else {

      if (isLoad && sceneName == "ClassicRun") {
        if (GameManager.OnStartLoadScene != null) GameManager.OnStartLoadScene();
        isLoad = false;
        LoadScene_("Loader");
        //SceneManager.LoadScene("Loader");
      } else {
        if (GameManager.OnStartLoadScene != null) GameManager.OnStartLoadScene();
        SceneManager.LoadScene(sceneName);
      }

    }
  }

  void LoadScene_(string sceneName) {
    Time.timeScale = 1f;
    FillBlackShow(FillBlack.AnimType.round, FillBlack.AnimVecor.swich, Vector3.zero, () => {
      //LoadLevelNow(sceneName);
      SceneManager.LoadScene(sceneName);
    });
  }

  void FillBlackShow(FillBlack.AnimType animType, FillBlack.AnimVecor animVector, Vector3 targetCenter, System.Action fillFull = null, System.Action fillStart = null, System.Action fillEnd = null, bool isUnscale = false) {

    FillBlack.Instance.PlayAnim(animType, animVector, targetCenter, fillFull, fillStart, fillEnd);
  }

  // Выход из игры
  public void Exit() {
    Application.Quit();
  }
  #endregion

  #region Параметры игрока

  void InitPlayerParametrs() {

    if (!PlayerPrefs.HasKey("GameRun")) StartFirstGame();
    int run = PlayerPrefs.GetInt("GameRun", 0);

    if (run == 0)
      PlayerPrefs.SetInt("firstStart", (int)(System.DateTime.Now - new System.DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds);  // Дата заупска игры

    PlayerPrefs.SetInt("GameRun", ++run);
    PlayerPrefs.SetString("GameRunLast", DateTime.Now.ToString());// Фиксируем запуск игры

    if (!(GameManager.Instance.isDebug && !GameManager.Instance.DebagValue("comics"))) {
      if (showComics && PlayerPrefs.GetInt("firstComics", 0) == 0) {
        isFirstStart = true;
        PlayerPrefs.SetInt("firstComics", 1);
      }
    }

  }

  public static void StartFirstGame() {

    if (Instance == null) return;

    DeleteAllPlayerPrefs();
    InitFirstPlayerPrefs();
#if PLUGIN_VOXELBUSTERS
    Apikbs.ClearAllLocalNotification();
#endif
  }

  public static bool DayAfterStart(int dayNum = 0) {
    int firstStart = PlayerPrefs.GetInt("firstStart");

    if (60 * 60 * 24 * dayNum + firstStart < (int)(System.DateTime.Now - new System.DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds)
      return true;
    else
      return false;
  }

  /// <summary>
  /// Сброс параметров до начальных
  /// </summary>
  public void Reset() {
    StartFirstGame();
  }
  /// <summary>
  /// Очистка параметров игры
  /// </summary>
  public static void DeleteAllPlayerPrefs() {
    PlayerPrefs.DeleteAll();
  }
  /// <summary>
  /// Инифиализация первоначальных значений
  /// </summary>
  public static void InitFirstPlayerPrefs() {
    Instance.InitFirstPlayerPrefs_();
  }
  public void InitFirstPlayerPrefs_() {
    for (int i = 0; i < saveParameters.Length; i++) {
      switch (saveParameters[i].type) {
        case saveType._float:
          PlayerPrefs.SetFloat(saveParameters[i].parametr, float.Parse(saveParameters[i].startValue));
          break;
        case saveType._int:
          PlayerPrefs.SetInt(saveParameters[i].parametr, int.Parse(saveParameters[i].startValue));
          break;
        default:
          PlayerPrefs.SetString(saveParameters[i].parametr, saveParameters[i].startValue);
          break;
      }
    }

    PlayerPrefs.SetString("GameRunLast", DateTime.Now.ToString());  // Дата заупска игры

#if PLUGIN_VOXELBUSTERS
    for (int i = 0; i < 100; i++) {
      string tmp = PlayerPrefs.GetString("localPush" + i + "day", "");
      if (tmp != "")
        Apikbs.ClearLocalNotification(tmp);
    }
#endif

  }

  #endregion

  #region Versions
  // Проверка на версионность

  public static void CheckVersion() {
    updateGate();
  }

  public static void updateGate() {
    float bestDist = UserManager.Instance.survivleMaxRunDistance;

    if (bestDist > 4600)
      PlayerPrefs.SetInt("openGate", 3);
    else if (bestDist > 3100)
      PlayerPrefs.SetInt("openGate", 2);
    else if (bestDist > 1600)
      PlayerPrefs.SetInt("openGate", 1);
    PlayerPrefs.Save();

    Questions.QuestionManager.UpdateFromFbNow();
  }

  #endregion

  /// <summary>
  /// Проверка пушей
  /// </summary>
  public static void CheckPush() {
#if PLUGIN_VOXELBUSTERS
    for (int i = 0; i < 100; i++) {
      string tmp = PlayerPrefs.GetString("localPush" + i + "day", "");
      if (tmp != "") Apikbs.ClearLocalNotification(tmp);
    }

    string[] arrMass = new string[30];
    string[] texts = new string[5] { LanguageManager.GetTranslate("push_StellCistall"), LanguageManager.GetTranslate("push_AllAbord"), LanguageManager.GetTranslate("push_StellShip"), LanguageManager.GetTranslate("push_Hoho"), LanguageManager.GetTranslate("push_Captain") };

    int delta21hour = (21 - System.DateTime.Now.Hour) * 60 * 60;

    for (int i = 1; i < arrMass.Length; i++) {
      arrMass[i] = PlayerPrefs.GetString("localPush" + (i * 3) + "day", "");
      if (arrMass[i] != "") Apikbs.ClearLocalNotification(arrMass[i]);
      arrMass[i] = Apikbs.CreateLocalNotification(delta21hour + 60 * 60 * 24 * (i * 3), VoxelBusters.NativePlugins.eNotificationRepeatInterval.NONE, "Jack Rover", texts[UnityEngine.Random.Range(0, texts.Length)]);
      PlayerPrefs.SetString("localPush" + (i * 3) + "day", arrMass[i]);
    }
#endif
  }
  /// <summary>
  /// Проверка, что это ipad
  /// </summary>
  public static bool isIPad {
    get {

#if UNITY_IOS
			if (Device.generation == DeviceGeneration.iPad1Gen
			|| Device.generation == DeviceGeneration.iPad2Gen
			|| Device.generation == DeviceGeneration.iPad3Gen
			|| Device.generation == DeviceGeneration.iPad4Gen
			|| Device.generation == DeviceGeneration.iPadAir1
			|| Device.generation == DeviceGeneration.iPadAir2
			|| Device.generation == DeviceGeneration.iPadMini1Gen
			|| Device.generation == DeviceGeneration.iPadMini2Gen
			|| Device.generation == DeviceGeneration.iPadMini3Gen
			|| Device.generation == DeviceGeneration.iPadMini4Gen
			|| Device.generation == DeviceGeneration.iPadPro1Gen
			|| Device.generation == DeviceGeneration.iPadUnknown
			)
				return true;
			else
				return false;
#else
      return false;
#endif
    }
  }

  #region Загрузка сцены с воротами

  public string gateScene;

  private AsyncOperation async = null;
  public delegate void CloseGateScene();
  private Action closeGateScene;
  bool waitLoadGateScene;

  public static void ShowGateScene(Action subsct = null, bool showAnim = true, int? gateNum = null) {

    if (Instance.waitLoadGateScene) return;
    Instance.waitLoadGateScene = true;

    Instance.closeGateScene = subsct;

    Instance.FillBlackShow(FillBlack.AnimType.full, FillBlack.AnimVecor.swich, Vector3.zero, () => {

      Instance.StartCoroutine(Instance.LoadGate(showAnim, gateNum));
    }, null, null, true);
  }


  IEnumerator LoadGate(bool showAnim = true, int? gateNum = null) {
    SceneManager.LoadScene(Instance.gateScene, LoadSceneMode.Additive);

    yield return null;
    Time.timeScale = 1;
    CameraController.Instance.gameObject.SetActive(false);
    if (GuiCamera.instance != null)
      GuiCamera.instance.gameObject.SetActive(false);

    GameObject.Find("SceneGate").transform.Find("GatesLargeController").GetComponent<GatesLagreController>().InitParametrs(showAnim, gateNum);
    //yield return async;

  }

  public static void HideGateScene() {
    Time.timeScale = 1;

    FillBlack.Instance.PlayAnim(FillBlack.AnimType.full, FillBlack.AnimVecor.swich, Vector3.zero, () => {
      Instance.waitLoadGateScene = false;
      SceneManager.UnloadSceneAsync(Instance.gateScene);
      CameraController.Instance.gameObject.SetActive(true);
      if (GuiCamera.instance != null)
        GuiCamera.instance.gameObject.SetActive(true);

      if (Instance.closeGateScene != null) Instance.closeGateScene();
    });

  }

  #endregion

  #region Сцена магазина

  public delegate void SceneShop();
  public SceneShop ShopSceneLoad;
  public SceneShop ShopSceneClose;

  /// <summary>
  /// Открытие магазина
  /// </summary>
  /// <param name="eventSceneLoad">Подпись на соьытие загрузки сцены</param>
  /// <param name="eventSceneClose">Подпись на событие выгрузки сцены</param>
  public void ShowShopScene(SceneShop eventSceneLoad, SceneShop eventSceneClose) {
    ShopSceneLoad = eventSceneLoad;
    ShopSceneClose = eventSceneClose;
    StartCoroutine(LoadShop());
  }

  IEnumerator LoadShop() {
    //yield return new WaitForSeconds(1);
    //async = SceneManager.LoadSceneAsync("Shop", LoadSceneMode.Additive);
    SceneManager.LoadScene("Shop", LoadSceneMode.Additive);

    yield return null;

    ShopSceneLoad();
    yield return async;
  }

  public void HideShopPanel() {
    ShopSceneClose();
  }

  #endregion

  #region Editor

  public int SetValueSaveParameters() {
    return saveParameters.Length;
  }

  public void SetTargetFrameRate(int targetFrameRate) {
    Application.targetFrameRate = targetFrameRate;
  }

  #endregion

  void GetConfig() {
    levelCount = Config.Instance.config.levels.Count;
  }


  #region Уровень

  public static event Action OnChangeLevel;

  public static int activeLevel;                  // Активный уровень
  public static int level;                        // Достигнутый максимальный уровень
  public static bool isAlterQuest;                // Режим альтернативного квеста
  public static bool isLoad;

  public int allKeys = 0;
  public int needKeys = 0;
  public bool generateGate;

  /// <summary>
  /// ВЫбор режима загрузки
  /// </summary>
  /// <param name="gameMode"></param>
  /// <param name="gameLocation"></param>
  public void OnSelectGameMode(GameMode gameMode, GameLocation gameLocation) {
    StartCoroutine(OnSelectGameModeCor(gameMode, gameLocation));
  }

  private IEnumerator OnSelectGameModeCor(GameMode gameMode, GameLocation gameLocation) {

    isLoad = true;

    LevelData loadLevelData = LoadLevelResource(gameMode, gameLocation);
    //LevelData loadLevelData = null;
    yield return new WaitForFixedUpdate();

    // Ошибка, если ничего не нашли
    if (loadLevelData == null) {
      Debug.LogError(String.Format("Ошибка загрузки режима игры {0} {1} активный уровень {2}", gameMode, gameLocation, activeLevel));
      yield break;
    }

    activeLevelData = loadLevelData;

    // Загружаем карту после запуска (если таковая указана)
    if (!String.IsNullOrEmpty(loadLevelData.sceneStart)) {
      LoadScene(loadLevelData.sceneStart);
      yield break;
    }

    // Выполняем дефалтный запуск уровня
    LoadScene(gameLocation == GameLocation.classic ? "ClassicRun" : "ShipRun");
  }

  /// <summary>
  /// Загрузка данных о сцене
  /// </summary>
  /// <param name="gameMode"></param>
  /// <param name="gameLocation"></param>
  /// <returns></returns>
  private LevelData LoadLevelResource(GameMode gameMode, GameLocation gameLocation) {

    // Если режим выживания
    if ((gameMode & GameMode.survival) != 0) {
      switch (gameLocation) {
        case GameLocation.classic:
          return Resources.Load<LevelData>("Runs/Levels/SurvivalClassic");
        case GameLocation.ship:
        default:
          return Resources.Load<LevelData>("Runs/Levels/ShipSurvival");
      }
    }

    // Если режим бега
    switch (gameMode) {
      case GameMode.levelsConstructor:

        LevelDataOrders levelOrder = Resources.Load<LevelDataOrders>("Runs/Levels/LevelOrder");
        LevelData ld = null;
        if (levelOrder.orderLevels.Count > GameManager.activeLevel)
          return levelOrder.orderLevels[GameManager.activeLevel].levelObject;


        return Resources.Load<LevelData>("Runs/Levels/LevelsClassic");
      //return Resources.Load<LevelData>("Runs/Levels/ClassicLevel" + activeLevel);
      default:
        return Resources.Load<LevelData>("Runs/Levels/LevelsClassic");
    }

  }

  /// <summary>
  /// Инициализация при старте
  /// </summary>
  public void InitLevel() {

    //#if UNITY_EDITOR
    //		gameMode = EditorPrefs.GetBool("mapEditorMode", false) ? GameMode.none : GameMode.levels;
    //#else
    //#endif

    activeLevelData.gameMode = GameMode.levelsClassic;

    level = PlayerPrefs.GetInt("level", 0);
    activeLevel = level;
    ChangeLevel(activeLevel);
  }
  /// <summary>
  /// Изменение уровня
  /// </summary>
  /// <param name="newLevel">Новый уровень</param>
  public void ChangeLevel(int newLevel, bool alterQuest = false) {

    isAlterQuest = alterQuest;

    // Запуск альтернативного квеста
    if (isAlterQuest) {
      activeLevel = newLevel;
      if (OnChangeLevel != null) OnChangeLevel();
      ExEvent.GameEvents.LevelChange.CallAsync(activeLevel);
      return;
    }

    if (Config.Instance.config.levels.Count <= newLevel) {
      //isSurvival = true;
      activeLevel = Config.Instance.config.levels.Count - 1;
      if (OnChangeLevel != null) OnChangeLevel();
      ExEvent.GameEvents.LevelChange.CallAsync(activeLevel);
      return;
    } else {
      //isSurvival = false;
    }

    // Достигнут новый уровень
    if (newLevel > level) {
      level = newLevel;

      PlayerPrefs.SetInt("level", level);
    }

    if (newLevel == level) {
      allKeys = UserManager.Instance.keys;
      needKeys = GameManager.Instance.mapRun[PlayerPrefs.GetInt("openGate")].keys;
      generateGate = true;
    } else {
      generateGate = false;
    }

    // Изменение уровня
    if (newLevel != activeLevel) {
      activeLevel = newLevel;
      if (OnChangeLevel != null) OnChangeLevel();
      ExEvent.GameEvents.LevelChange.CallAsync(activeLevel);
    }
  }

  #endregion

  #region Дебаг

  public void InitDebugParametrs() {

    if (PlayerPrefs.GetString("debug", "{}") == "{}" || PlayerPrefs.GetString("debug", "{}") == "") {
      for (int i = 0; i < debugParametrs.Length; i++) {
        debugParametrs[i].value = debugParametrs[i].startValue;
        SaveDebugParametrs();
        InitDebugParametrs();
      }
      return;
    }

    List<object> debugParametrsList = (List<object>)Json.Deserialize(PlayerPrefs.GetString("debug", "{}"));

    foreach (object deb in debugParametrsList) {
      Dictionary<string, object> debDict = (Dictionary<string, object>)deb;

      for (int i = 0; i < debugParametrs.Length; i++) {
        if (debugParametrs[i].parametr == debDict["parametr"].ToString())
          debugParametrs[i].value = debDict["value"].ToString();
      }
    }
  }

  public void SaveDebugParametrs() {

    List<object> debugParametrList = new List<object>();

    for (int i = 0; i < debugParametrs.Length; i++) {
      Dictionary<string, object> elem = new Dictionary<string, object>();
      elem["parametr"] = debugParametrs[i].parametr;
      elem["value"] = debugParametrs[i].value;
      debugParametrList.Add(elem);
    }
    PlayerPrefs.SetString("debug", Json.Serialize(debugParametrList));
  }

  public bool DebagValue(string debugParametr) {
    for (int i = 0; i < debugParametrs.Length; i++) {
      if (debugParametrs[i].parametr == debugParametr)
        return bool.Parse(debugParametrs[i].value);
    }
    return false;
  }

  public SavesParameters[] debugParametrs;

  #endregion

}