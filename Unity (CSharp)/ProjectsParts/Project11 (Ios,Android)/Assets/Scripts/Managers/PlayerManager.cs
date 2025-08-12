using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExEvent;
using KingBird.Ads;
using Newtonsoft.Json;
using GCSave = GameCompany.Save;
using GC = GameCompany;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(PlayerManager))]
public class PlayerManagerEditor: Editor {

  int indexLevel = 0;
  string langReport = "";

  public override void OnInspectorGUI() {
    base.OnInspectorGUI();

    if (GUILayout.Button("Save")) {
      ((PlayerManager)target).Save(true);
    }

    if (GUILayout.Button("Удалить все пуши")) {
      PushManager.Instance.RemoveAllPush();
    }

    if (GUILayout.Button("Пройти уровень")) {
      ((PlayerManager)target).ForceLevelComplete();
    }

    if (GUILayout.Button("Load all locations")) {
      ((PlayerManager)target).company.companies.Clear();
      ((PlayerManager)target).company.saveCompanies.Clear();
      ((PlayerManager)target).company.isLoadProcess = false;
      ((PlayerManager)target).company.downloadProcess = false;
      ((PlayerManager)target).company.downloadOrder.Clear();
      ((PlayerManager)target).company.AllDownload(null);
    }

    if (GUILayout.Button("Write levels")) {
      ((PlayerManager)target).company.WriteToFile();
    }
    EditorGUILayout.BeginHorizontal();

    if (GUILayout.Button("Create report crossword")) {
      ((PlayerManager)target).company.ReportLevelsCrossword();
    }

    if (GUILayout.Button("Create report translate")) {
      ((PlayerManager)target).company.ReportLevelsTranslate();
    }

    EditorGUILayout.EndHorizontal();

    EditorGUILayout.BeginHorizontal();


    if (GUILayout.Button("Create Crossword")) {
      ((PlayerManager)target).company.CreateCrossword();
    }

    if (GUILayout.Button("Remove not crossword")) {
      ((PlayerManager)target).company.RemoveNotExistsCrossword();
    }

    EditorGUILayout.EndHorizontal();

    //EditorGUILayout.BeginHorizontal();

    //langReport = EditorGUILayout.TextField("Lang", langReport);
    //indexLevel = EditorGUILayout.IntField("Index", indexLevel);

    //if (GUILayout.Button("Create Crossword level")) {
    //  ((PlayerManager)target).company.CreateOneLevel(langReport, indexLevel);
    //}

    //EditorGUILayout.EndHorizontal();

  }
}

#endif

public class PlayerManager: Singleton<PlayerManager> {

  [HideInInspector]
  public GC.CompanyController company;

  private DateTime lastEnter = DateTime.Now;

  public int countGameHelper = 0;
  public bool noAds;

  public int _playCount = 0;   // Количиство запусков

  private int startCoins = 300;

  public int playCount {
    get { return _playCount; }
    set {
      _playCount = value;
    }
  }

  private int _coins, _coinsRnd;
  public int coins {
    get { return _coins - _coinsRnd; }
    set {
      _coinsRnd = RandomValue();
      _coins = value + _coinsRnd;
      PlayerEvents.CoinsChange.Call(_coins - _coinsRnd);
      PlayerPrefs.SetInt("coins", coins);
    }
  }

  private int _stars, _starsRnd;
  public int stars {
    get { return _stars - _starsRnd; }
    set {
      _starsRnd = RandomValue();
      _stars = value + _starsRnd;
      PlayerEvents.StarsChange.Call(_stars - _starsRnd);
    }
  }

  private bool _audioMisic = true, _audioEffects = true;

  public bool audioMisic {
    get { return _audioMisic; }
    set {
      _audioMisic = value;
      GameEvents.OnChangeMusic.Call(_audioMisic);
    }
  }

  public bool audioEffects {
    get { return _audioEffects; }
    set {
      _audioEffects = value;
      GameEvents.OnChangeEffects.Call(_audioEffects);
    }
  }

  private bool isLoading;

  public AchiveManager achives = new AchiveManager();
  private void Start() {
    //playCount++;
    achives.Init();
    StartCoroutine(LoadCor());
    //Load(() => {
    //	SplashUi splash = (SplashUi)UIManager.Instance.GetPanel(UiType.splash);
    //	splash.ToGame();
    //});
  }

  IEnumerator LoadCor() {
    yield return null;
    Load(() => {
      playCount++;
      Save();
    });
  }

  public void Save(bool full = false) {

    if (isLoading) return;

    Dictionary<string, string> saveData = new Dictionary<string, string>();
    saveData.Add("stars", stars.ToString());
    saveData.Add("noAds", noAds.ToString());

    saveData.Add("hintFirstLetter", hintFirstLetter.ToString());
    saveData.Add("hintEnyLetter", hintAnyLetter.ToString());
    saveData.Add("hintFirstWord", hintFirstWord.ToString());
    saveData.Add("unlimitedHint", unlimitedHint.ToString());
    saveData.Add("countGameHelper", countGameHelper.ToString());

    saveData.Add("audioMisic", audioMisic.ToString());
    saveData.Add("audioEffects", audioEffects.ToString());
    saveData.Add("translateLanuage", _translateLanuage);

    saveData.Add("playCount", playCount.ToString());

    saveData.Add("company", JsonConvert.SerializeObject(company.SaveCompany()));
    saveData.Add("achives", JsonConvert.SerializeObject(achives.Save()));

    saveData.Add("lastEnter", lastEnter.ToString());

    PlayerPrefs.SetString("progress", JsonConvert.SerializeObject(saveData));

    if (full) {
      PlayerPrefs.SetString("company", JsonConvert.SerializeObject(company.SaveLevels()));
      PlayerManager.Instance.company.SaveAllLevels();
    }

  }

  private void Load(Action OnComplete) {
    try {

      PushManager.Instance.Load();

      GameService.Instance.Load();

      _translateLanuage = LanguageManager.Instance.LoadDefaultParamsTranslateWords();

      coins = PlayerPrefs.GetInt("coins", 300);

      if (!PlayerPrefs.HasKey("progress")) {
        company.FirstInitiate();
        //company.AllDownload(OnComplete);
        CreateAllPush();
        Save(true);
        return;
      }
      isLoading = true;

      if (PlayerPrefs.HasKey("company"))
        company.LoadLevels(JsonConvert.DeserializeObject<List<GC.Company>>(PlayerPrefs.GetString("company")));
      //Debug.Log(PlayerPrefs.GetString("company"));

      Dictionary<string, string> saveData =
        JsonConvert.DeserializeObject<Dictionary<string, string>>(PlayerPrefs.GetString("progress"));


      if (saveData.ContainsKey("stars"))
        stars = int.Parse(saveData["stars"]);

      if (saveData.ContainsKey("noAds"))
        noAds = bool.Parse(saveData["noAds"]);

      if (saveData.ContainsKey("playCount"))
        playCount = int.Parse(saveData["playCount"]);

      if (saveData.ContainsKey("translateLanuage"))
        _translateLanuage = saveData["translateLanuage"];

      if (saveData.ContainsKey("hintFirstLetter"))
        hintFirstLetter = int.Parse(saveData["hintFirstLetter"]);

      if (saveData.ContainsKey("hintEnyLetter"))
        hintAnyLetter = int.Parse(saveData["hintEnyLetter"]);

      if (saveData.ContainsKey("hintFirstWord"))
        hintFirstWord = int.Parse(saveData["hintFirstWord"]);

      if (saveData.ContainsKey("unlimitedHint"))
        unlimitedHint = bool.Parse(saveData["unlimitedHint"]);

      AdsKingBird.Instance.Load();

      if (saveData.ContainsKey("countGameHelper"))
        countGameHelper = int.Parse(saveData["countGameHelper"]);

      if (saveData.ContainsKey("achives"))
        achives.Load(JsonConvert.DeserializeObject<List<int>>(saveData["achives"]));

      DailyBonus.Instance.Load();

      if (saveData.ContainsKey("company"))
        company.LoadCompany(JsonConvert.DeserializeObject<GC.SaveProgress>(saveData["company"]));

      audioMisic = saveData.ContainsKey("audioMisic") ? Boolean.Parse(saveData["audioMisic"]) : true;

      audioEffects = saveData.ContainsKey("audioEffects") ? Boolean.Parse(saveData["audioEffects"]) : true;

#if UNITY_EDITOR
      audioMisic = false;
      audioEffects = false;
#endif

      ConchManager.Instance.Load();

      LoadPush();
      
      PlayerEvents.OnLoad.Call();

      isLoading = false;

      if (OnComplete != null) OnComplete();

      //company.AllDownload(OnComplete);

    } catch (Exception er) {
      Logger.Instance.AddLogger(er.Message, er.StackTrace, LogType.Log);
    }
  }

  public void NextDownloadElement() {
    StartCoroutine(company.NextDownloadQueue());
  }

  public int RandomValue() {
    return (int)(UnityEngine.Random.value * 1000);
  }

  [ExEvent.ExEventHandler(typeof(ExEvent.GameEvents.OnChangeGamePhase))]
  public void OnChangeGamePhase(ExEvent.GameEvents.OnChangeGamePhase phase) {

    if (phase.next != GamePhase.game) {
      if (isHintFirstLetter)
        GameEvents.OnHintFirstLetterCompleted.Call(false);
      if (isHintAnyLetter)
        GameEvents.OnHintAnyLetterCompleted.Call(false);
      if (isHintFirstWord)
        GameEvents.OnHintFirstWordCompleted.Call(false);
    }

  }

  private string _translateLanuage;

  public string translateLanuage {
    get {
      return
    System.String.IsNullOrEmpty(_translateLanuage) ? "en" : _translateLanuage;
    }
    set {
      _translateLanuage = value;
      ExEvent.PlayerEvents.OnChangeTranslate.Call(_translateLanuage);
    }
  }

  [ExEvent.ExEventHandler(typeof(ExEvent.GameEvents.OnLetterLoaded))]
  private void OnLevelLoad(ExEvent.GameEvents.OnLetterLoaded load) {
    this.company.OnLevelLoad();
  }

  public void ForceLevelComplete() {
    //#if UNITY_EDITOR
    if (GameManager.gamePhase != GamePhase.game) return;
    company.ForceLevelComplete();
    //#endif
  }

  #region Подсказки

  public bool _unlimitedHint;

  public bool unlimitedHint {
    get { return _unlimitedHint; }
    set {
      if (value && _unlimitedHint)
        return;
      _unlimitedHint = value;
      PlayerEvents.SetUnlimitedHint.Call(_unlimitedHint);
    }
  }

  private int _hintFirstLetter = 0, _hintFirstLetterRnd;
  private int _hintAnyLetter = 0, _hintAnyLetterRnd;
  private int _hintFirstWord = 0, _hintFirstWordRnd;

  public int hintFirstLetter {
    get { return _hintFirstLetter - _hintFirstLetterRnd; }
    set {
      _hintFirstLetterRnd = RandomValue();
      _hintFirstLetter = value + _hintFirstLetterRnd;
      PlayerEvents.HintFirstLetterChange.Call(_hintFirstLetter - _hintFirstLetterRnd);
    }
  }

  public int hintAnyLetter {
    get { return _hintAnyLetter - _hintAnyLetterRnd; }
    set {
      _hintAnyLetterRnd = RandomValue();
      _hintAnyLetter = value + _hintAnyLetterRnd;
      PlayerEvents.HintEnyLetterChange.Call(_hintAnyLetter - _hintAnyLetterRnd);
    }
  }

  public int hintFirstWord {
    get { return _hintFirstWord - _hintFirstWordRnd; }
    set {
      _hintFirstWordRnd = RandomValue();
      _hintFirstWord = value + _hintFirstWordRnd;
      PlayerEvents.HintFirstWordChange.Call(_hintFirstWord - _hintFirstWordRnd);
    }
  }

  public bool isHintFirstLetter { get; set; }
  public bool isHintAnyLetter { get; set; }
  public bool isHintFirstWord { get; set; }

  /// <summary>
  /// Подсказка открытия первой буквы первого слова
  /// </summary>
  public void HintFirstLetter() {

    if (GameManager.gamePhase != GamePhase.game || isHintFirstLetter) return;

    if (!unlimitedHint && hintFirstLetter <= 0) {
      if (coins <= Properties.Instance.hintFirstLetterPrice) {
        GameManager.Instance.Shop();
        return;
      };
      coins -= Properties.Instance.hintFirstLetterPrice;
      AudioManager.Instance.library.PlayByeAudio();
      hintFirstLetter++;
    }

    isHintFirstLetter = true;
    ExEvent.GameEvents.OnHintFirstLetterReady.Call();
  }

  [ExEventHandler(typeof(GameEvents.OnHintFirstLetterCompleted))]
  private void HintFirstLetterCompleted(GameEvents.OnHintFirstLetterCompleted hint) {
    isHintFirstLetter = false;
    if (!hint.isCompleted) return;

    GCSave.Word saveWord = new GCSave.Word();
    saveWord.word = hint.word;

    saveWord.hintLetters = new List<int> { (int)hint.numLetter };

    hintFirstLetter -= 1;

    company.SaveCompanyWord(saveWord, false);
  }

  /// <summary>
  /// Подсказка открытия любой буквы
  /// </summary>
  public void HintAnyLetter() {

    if (GameManager.gamePhase != GamePhase.game || isHintAnyLetter) return;

    if (!unlimitedHint && hintAnyLetter <= 0) {
      if (coins <= Properties.Instance.hintAnyLetterPrice) {
        GameManager.Instance.Shop();
        return;
      };
      coins -= Properties.Instance.hintAnyLetterPrice;
      AudioManager.Instance.library.PlayByeAudio();
      hintAnyLetter++;
    }

    isHintAnyLetter = true;
    ExEvent.GameEvents.OnHintAnyLetterReady.Call();



  }

  [ExEventHandler(typeof(GameEvents.OnHintAnyLetterCompleted))]
  private void HintAnyLetterCompleted(GameEvents.OnHintAnyLetterCompleted hint) {
    isHintAnyLetter = false;
    if (!hint.isCompleted) return;

    GCSave.Word saveWord = new GCSave.Word();
    saveWord.word = hint.word;

    saveWord.hintLetters = new List<int> { (int)hint.numLetter };

    hintAnyLetter -= 1;

    company.SaveCompanyWord(saveWord, false);

  }

  /// <summary>
  /// Подсказка открытия первого слова
  /// </summary>
  public void HintFirstWord() {

    if (GameManager.gamePhase != GamePhase.game || isHintFirstWord) return;

    if (!unlimitedHint && hintFirstWord <= 0) {
      if (coins <= Properties.Instance.hintFirstWordPrice) {
        GameManager.Instance.Shop();
        return;
      };
      coins -= Properties.Instance.hintFirstWordPrice;
      AudioManager.Instance.library.PlayByeAudio();
      hintFirstWord++;
    }

    isHintFirstWord = true;
    ExEvent.GameEvents.OnHintFirstWordReady.Call();

  }

  [ExEventHandler(typeof(GameEvents.OnHintFirstWordCompleted))]
  private void HintFirstWordCompleted(GameEvents.OnHintFirstWordCompleted hint) {
    isHintFirstWord = false;
    if (!hint.isCompleted) return;

    GCSave.Word saveWord = new GCSave.Word();
    saveWord.word = hint.word;

    List<int> letters = new List<int>();

    for (int i = 0; i < hint.word.Length; i++)
      letters.Add(i);

    saveWord.hintLetters = letters;

    hintFirstWord -= 1;

    company.SaveCompanyWord(saveWord, false);
  }

  #endregion

  #region Пуши

  private string day1randomPush = "";
  private string hour24outPush = "";
  private string hour12_14Push = "";
  private string hour21_22Push = "";
  private string day3Push = "";

  public void CreateAllPush() {
    RemoveOldPush();
    CreateHour12_14Push();
    CreateHour21_22Push();
    CreateDay3Push();
    CreateDay1RandomPush();
    CreateHour24OutPush();
    SavePush();
  }

  private void SavePush() {
    PlayerPrefs.SetString("day1randomPush", day1randomPush);
    PlayerPrefs.SetString("hour24outPush", hour24outPush);
    PlayerPrefs.SetString("hour12_14Push", hour12_14Push);
    PlayerPrefs.SetString("hour21_22Push", hour21_22Push);
    PlayerPrefs.SetString("day3Push", day3Push);
  }

  private void LoadPush() {

    day1randomPush = PlayerPrefs.GetString("day1randomPush", null);
    hour24outPush = PlayerPrefs.GetString("hour24outPush", null);
    hour12_14Push = PlayerPrefs.GetString("hour12_14Push", null);
    hour21_22Push = PlayerPrefs.GetString("hour21_22Push", null);
    day3Push = PlayerPrefs.GetString("day3Push", null);

    CreateAllPush();
  }

  private void CreateDay1RandomPush() {
    DateTime date = DateTime.Now.AddHours(3);
    if (date.Hour > 16) {
      date = date.Subtract(date.TimeOfDay).AddDays(1).AddHours(UnityEngine.Random.Range(9, 18)).AddMinutes(UnityEngine.Random.Range(0, 60));
    } else {
      date = date.Subtract(date.TimeOfDay).AddHours(UnityEngine.Random.Range(date.Hour, 18)).AddMinutes(UnityEngine.Random.Range(0, 60));
    }
    day1randomPush = PushManager.Instance.CreatePush(LanguageManager.GetTranslate("push.day1random"), date, true);
  }
  private void CreateHour24OutPush() {
    hour24outPush = PushManager.Instance.CreatePush(LanguageManager.GetTranslate("push.24hourOut"), DateTime.Now.AddHours(24));
  }
  private void CreateHour12_14Push() {
    DateTime date = DateTime.Now.AddHours(3);
    if (date.Hour > 12) {
      date = date.Subtract(date.TimeOfDay).AddDays(1).AddHours(UnityEngine.Random.Range(12, 14)).AddMinutes(UnityEngine.Random.Range(0, 60));
    } else {
      date = date.Subtract(date.TimeOfDay).AddHours(UnityEngine.Random.Range(12, 14)).AddMinutes(UnityEngine.Random.Range(0, 60));
    }
    hour12_14Push = PushManager.Instance.CreatePush(LanguageManager.GetTranslate("push.12-14hour"), date, true);
  }
  private void CreateHour21_22Push() {
    DateTime date = DateTime.Now.AddHours(3);
    if (date.Hour > 21) {
      date = date.Subtract(date.TimeOfDay).AddDays(1).AddHours(21).AddMinutes(UnityEngine.Random.Range(0, 60));
    } else {
      date = date.Subtract(date.TimeOfDay).AddHours(21).AddMinutes(UnityEngine.Random.Range(0, 60));
    }
    hour21_22Push = PushManager.Instance.CreatePush(LanguageManager.GetTranslate("push.21-22hout"), date, true);
  }
  private void CreateDay3Push() {
    return;
    day3Push = PushManager.Instance.CreatePush(LanguageManager.GetTranslate("push.3dayOut"), DateTime.Now.AddDays(3), true);
  }

  private void RemoveOldPush() {
    if (!String.IsNullOrEmpty(day1randomPush)) {
      PushManager.Instance.RemovePush(day1randomPush);
    }

    if (!String.IsNullOrEmpty(hour24outPush)) {
      PushManager.Instance.RemovePush(hour24outPush);
    }

    if (!String.IsNullOrEmpty(hour12_14Push)) {
      PushManager.Instance.RemovePush(hour12_14Push);
    }

    if (!String.IsNullOrEmpty(hour21_22Push)) {
      PushManager.Instance.RemovePush(hour21_22Push);
    }

    if (!String.IsNullOrEmpty(day3Push)) {
      PushManager.Instance.RemovePush(day3Push);
    }
  }

  #endregion

}

public enum SelectWord {
  no,
  repeat,
  yes,
  specialYes,
  specialRepeat
}

public enum HintType {
  firstLetter = 0,
  anyLetter = 1,
  firstWord = 2
}