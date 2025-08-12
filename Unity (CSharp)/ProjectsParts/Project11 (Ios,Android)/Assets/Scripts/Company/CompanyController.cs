using ExEvent;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using GC = GameCompany;
using GCSave = GameCompany.Save;

namespace GameCompany {
  [System.Serializable]
  public class CompanyController {

    public List<GC.Company> companies;
    public List<GCSave.Company> saveCompanies = new List<GCSave.Company>();
    public List<GC.Company> downloadCompanies;

    public int lastLevelComplete = -1;

    private string _actualCompany = "en";
    public bool isLoadProcess = false;
    private bool needLoad = false;

    private bool _firstLoad;

    public int freePlayLocation;

    public bool firstLoad {
      get { return _firstLoad; }
      set {
        _firstLoad = value;
      }
    }

    public string actualCompany {
      get { return String.IsNullOrEmpty(_actualCompany) ? "en" : _actualCompany; }
      set {
        _actualCompany = value;
        PlayerManager.Instance.Save();
        ExEvent.PlayerEvents.OnChangeCompany.Call(_actualCompany);
      }
    }

    public List<GC.Company> SaveLevels() {
      return companies;
    }

    public void LoadLevels(List<GC.Company> compamy) {
      companies = compamy;
    }

    public void SaveAllLevels() {
      for (int i = 0; i < saveCompanies.Count; i++) {

        for (int j = 0; j < saveCompanies[i].levels.Count; j++) {
          PlayerPrefs.SetString(String.Format("{0}_{1}", saveCompanies[i].shortCompany, j), JsonConvert.SerializeObject(saveCompanies[i].levels[j]));
        }

      }
    }

    public void SaveOneLevel(string company, int levelNum) {

      PlayerManager.Instance.Save();

      for (int i = 0; i < saveCompanies.Count; i++) {
        if (saveCompanies[i].shortCompany == company) {
          string dataString =
            JsonConvert.SerializeObject(saveCompanies.Find(x => x.shortCompany == company).levels[levelNum]);
          PlayerPrefs.SetString(String.Format("{0}_{1}", company, levelNum), dataString);

        }
      }
    }

    public void LoadAllLevels() {

      for (int i = 0; i < saveCompanies.Count; i++) {

        int num = 0;
        bool loop = true;

        while (loop) {

          string key = String.Format("{0}_{1}", saveCompanies[i].shortCompany, num);

          if (PlayerPrefs.HasKey(key)) {
            string dataString = PlayerPrefs.GetString(key);
            //string[] arr = key.Split(new char[] {'_'});
            var data = saveCompanies.Find(x => x.shortCompany == saveCompanies[i].shortCompany);

            if (data.levels == null)
              data.levels = new List<GCSave.Level>();

            data.levels.Add(JsonConvert.DeserializeObject<GCSave.Level>(dataString));

            num++;
          } else {
            loop = false;
          }

        }
      }
    }
    //#if UNITY_EDITOR
    public void ForceLevelComplete() {

      var company = GetActualSaveCompany();


      for (int slv = 0; slv < company.levels.Count; slv++) {

        if (!company.levels[slv].isComplited) {

          var level = GetActualCompany().levels[slv];

          for (int i = 0; i < level.words.Count; i++) {
            if (!company.levels[slv].words.Exists(x => x.word == level.words[i].word && level.words[i].primary && !x.isOpen)) {
              ReadWord(level.words[i].word);
            }
          }


          return;
        }

      }


    }
    //#endif
    public bool CheckFullInit() {
      return true;
    }

    public void FirstInitiate() {

      saveCompanies.Clear();

      for (int i = 0; i < companies.Count; i++) {

        GCSave.Company sc = new GCSave.Company();
        sc.shortCompany = companies[i].short_name;
        sc.bonusLevels = new List<GCSave.Level>();
        sc.levels = new List<GCSave.Level>();
        saveCompanies.Add(sc);

        for (int ll = 0; ll < companies[i].levels.Count; ll++) {

          GCSave.Level lvl = new GCSave.Level();
          lvl.id = companies[i].levels[ll].id;
          lvl.words = new List<GCSave.Word>();
          sc.levels.Add(lvl);

        }

        for (int ll = 0; ll < companies[i].bonusLevels.Count; ll++) {

          GCSave.Level lvl = new GCSave.Level();
          lvl.id = companies[i].bonusLevels[ll].id;
          lvl.words = new List<GCSave.Word>();
          sc.bonusLevels.Add(lvl);

        }

      }

    }


    public SaveProgress SaveCompany() {

      var saveprogress = new SaveProgress() {
        //company = companies,
        //progress = saveCompanies,
        progress = new List<GCSave.Company>(),
        lastBonus = lastBonus,
        activeCompany = actualCompany,
        pushLevelNoComplete = pushInfo,
        lastLevelComplete = lastLevelComplete
      };

      for (int i = 0; i < saveCompanies.Count; i++) {
        saveprogress.progress.Add(GCSave.Company.CreateForSave(saveCompanies[i]));
      }

      return saveprogress;

    }

    public void LoadCompany(SaveProgress progress) {
      //companies = progress.company;
      saveCompanies = progress.progress;
      lastBonus = progress.lastBonus;
      PlayerManager.Instance.company.LoadAllLevels();
      pushInfo = progress.pushLevelNoComplete;

      actualCompany = progress.activeCompany;
      lastLevelComplete = progress.lastLevelComplete;

    }

    public void AllDownload(Action OnComplete = null) {

      if (isLoadProcess) return;
      isLoadProcess = true;
      // Первоначальная инициализация активной компании
      //actualCompany = LanguageManager.Instance.activeLanuage.code;

      int locationCount = 0;

      AddDownloadQueue((CallbackFunc) => {
        DownloadCompany(() => {
          locationCount++;

          companies.ForEach((targetCompany) => {

            AddDownloadQueue((CallbackFunc2) => {

              DownloadLocations(targetCompany, (locationList) => {


                for (int locIndex = 0; locIndex < locationList.Count; locIndex++) {

                  if (locationList[locIndex].locationType == LevelType.bonus) {

                    var targetLocation = locationList[locIndex];

                    AddDownloadQueue((CallbackFunc3) => {

                      DownloadLevels(targetCompany, targetLocation,
                        () => {
                          locationCount--;

                          if (String.IsNullOrEmpty(PlayerManager.Instance.translateLanuage)) {

                            var ll = LanguageManager.Instance.lanuageLibrary.Find(x => x.type == Application.systemLanguage);
                            if (ll == null) {
                              ll = LanguageManager.Instance.lanuageLibrary.Find(x => x.type == SystemLanguage.English);
                            }

                            var trns = targetCompany.levels[0].words[0].translations.Find(x => x.lang == ll.code);
                            if (trns == null) {
                              trns = targetCompany.levels[0].words[0].translations[0];
                            }
                            PlayerManager.Instance.translateLanuage = trns.lang;

                          }
                          if (downloadOrder.Count == 0) {
                            PlayerManager.Instance.Save(true);
                            PlayerEvents.OnLoad.Call();
                            firstLoad = true;
                            isLoadProcess = false;
                            CheckNeedLoad();
                            if (OnComplete != null) OnComplete();
                          }
                          CallbackFunc3();
                        },
                        () => {
                          CallbackFunc3();
                        });
                    });


                  } else {

                    locationCount++;
                    var targetLocation = locationList[locIndex];

                    AddDownloadQueue((CallbackFunc3) => {
                      DownloadLevels(targetCompany, targetLocation, () => {
                        //PlayerEvents.OnLoad.Call();

                        locationCount--;

                        if (downloadOrder.Count == 0) {
                          PlayerManager.Instance.Save(true);
                          firstLoad = true;
                          isLoadProcess = false;
                          CheckNeedLoad();
                          PlayerEvents.OnLoad.Call();
                          if (OnComplete != null) OnComplete();
                        }
                        CallbackFunc3();
                      }, () => {
                        CallbackFunc3();
                      });
                    });

                  }

                }

                CallbackFunc2();
              }, () => {
                CallbackFunc2();
              });
            });
          }
          );
          CallbackFunc();
        }, () => {
          CallbackFunc();
          locationCount = 0;

        });
      });
    }

    public Queue<Action<Action>> downloadOrder = new Queue<Action<Action>>();
    public bool downloadProcess = false;
    private void AddDownloadQueue(Action<Action> exec) {
      downloadOrder.Enqueue(exec);
      if (!downloadProcess)
        PlayerManager.Instance.NextDownloadElement();
    }

    public IEnumerator NextDownloadQueue() {
      if (downloadProcess || downloadOrder.Count == 0) yield break;
      downloadProcess = true;
      Action<Action> func = downloadOrder.Dequeue();

      func(() => {
        Debug.Log("download complete");
        downloadProcess = false;
        PlayerManager.Instance.NextDownloadElement();
      });
    }

    public void DownloadCompany(Action OnComplete, Action OnFailed) {

      NetManager.Instance.GetCompanies((downloadConpany) => {

        // Отказ при получении локации
        if (downloadConpany == null) {
          if (OnFailed != null) OnFailed();
          return;
        }

        for (int i = 0; i < downloadConpany.Count; i++) {

          GC.Company gc = companies.Find(x => x.short_name == downloadConpany[i].short_name);

          if (gc != null) {
            continue;
          }

          gc = downloadConpany[i];
          gc.levels = new List<GC.Level>();
          gc.bonusLevels = new List<GC.Level>();
          companies.Add(gc);

          GCSave.Company saveCompany = saveCompanies.Find(x => x.shortCompany == downloadConpany[i].short_name);

          if (saveCompany == null) {
            saveCompany = new GCSave.Company();
            saveCompany.shortCompany = downloadConpany[i].short_name;
            saveCompany.levels = new List<GCSave.Level>();
            saveCompany.bonusLevels = new List<GCSave.Level>();
            saveCompanies.Add(saveCompany);
          }
        }

        bool systemLangExists = false;
        for (int i = 0; i < companies.Count; i++) {
          if (companies[i].short_name == actualCompany)
            systemLangExists = true;
        }
        if (OnComplete != null) OnComplete();

      }
      );
    }
    private void DownloadLocations(GC.Company company, Action<List<GC.Location>> OnComplete, Action OnFailed = null) {

      NetManager.Instance.GetLocations(company.short_name, (downloadLocation) => {

        if (downloadLocation == null) {
          if (OnFailed != null) OnFailed();
          return;
        }

        if (OnComplete != null) OnComplete(downloadLocation);
        return;

        //GCSave.Company saveCompany = saveCompanies.Find(x => x.shortCompany == company.short_name);
        //int useNum = -1;
        //for (int i = 0; i < downloadLocation.Count; i++) {

        //  if (downloadLocation[i].locationType != LevelType.bonus)
        //    useNum++;

        //  // Если запись отсутствует
        //  if ((downloadLocation[i].locationType == LevelType.bonus && company.bonusLocation == null) || company.locations.Count < i) {

        //    GC.Location loc = downloadLocation[i];
        //    loc.levels = new List<Level>();
        //    loc.ParseLevelType();

        //    if (loc.locationType == LevelType.bonus)
        //      company.bonusLocation = loc;
        //    else
        //      company.locations.Add(loc);

        //    GCSave.Location saveLoc = new GCSave.Location();
        //    saveLoc.levels = new List<GCSave.Level>();
        //    saveLoc.id = loc.id;

        //    if (loc.locationType == LevelType.bonus)
        //      saveCompany.bonusLocation = saveLoc;
        //    else
        //      saveCompany.locations.Add(saveLoc);

        //    continue;
        //  }

        //  if (downloadLocation[i].locationType != LevelType.bonus) {


        //    if (!String.IsNullOrEmpty(company.locations[useNum].version) &&
        //        company.locations[useNum].version == downloadLocation[i].version) {
        //      company.locations[useNum].isChange = false;
        //      continue;
        //    }
        //  } else {
        //    if (!String.IsNullOrEmpty(company.bonusLocation.version) &&
        //        company.bonusLocation.version == downloadLocation[i].version) {
        //      company.bonusLocation.isChange = false;
        //      continue;
        //    }
        //  }


        //  // Если запись присутствует
        //  bool existsSave = false;

        //  if (downloadLocation[i].locationType != LevelType.bonus) {
        //    for (int s1 = 0; s1 < saveCompany.locations[useNum].levels.Count; s1++) {
        //      if (saveCompany.locations[useNum].levels[s1].words.Count > 0)
        //        existsSave = true;
        //    }
        //  }

        //  if (!existsSave) {
        //    GC.Location loc = downloadLocation[i];
        //    loc.levels = new List<Level>();
        //    loc.isChange = true;
        //    loc.ParseLevelType();

        //    if (loc.locationType == LevelType.bonus)
        //      company.bonusLocation = loc;
        //    else
        //      company.locations[useNum] = loc;

        //    GCSave.Location saveLoc = new GCSave.Location();
        //    saveLoc.levels = new List<GCSave.Level>();
        //    saveLoc.id = loc.id;

        //    if (loc.locationType == LevelType.bonus)
        //      saveCompany.bonusLocation = saveLoc;
        //    else
        //      saveCompany.locations[useNum] = saveLoc;

        //  }


        //}

        //if (OnComplete != null) OnComplete();
      }
      );

    }

    private void DownloadLevels(GC.Company comp, GC.Location location, Action OnLoad, Action OnFailed = null) {
      Debug.Log("DownloadLevels");
      //if (!location.isChange && location.levels.Count > 0) {

      //  if (OnLoad != null) OnLoad();
      //  return;
      //}

      List<GCSave.Level> saveLevels = location.locationType == LevelType.bonus
                                    ? GetlSaveCompany(comp.short_name).bonusLevels
                                    : GetlSaveCompany(comp.short_name).levels;

      NetManager.Instance.GetLevels(location.id, "", (downloadLevels) => {

        if (downloadLevels == null) {
          if (OnFailed != null) OnFailed();
          return;
        }

        for (int i = 0; i < downloadLevels.Count; i++) {

          // Если запись отсутствует

          GC.Level lvl = downloadLevels[i];


          //location.levels.Add(lvl);
          SelectCoinsWord(lvl);

          GCSave.Level saveLoc = new GCSave.Level();
          saveLoc.words = new List<GCSave.Word>();
          saveLoc.id = lvl.id;
          saveLevels.Add(saveLoc);

          if (location.locationType == LevelType.bonus) {
            comp.bonusLevels.Add(lvl);
          } else {
            comp.levels.Add(lvl);
          }


          continue;


          //// Если запись присутствует
          //bool existsSave = saveLevels[i].words.Count > 0;

          //if (!existsSave) {

          //  GC.Level lvl = downloadLevels[i];

          //  SelectCoinsWord(lvl);
          //  location.levels[i] = lvl;

          //  GCSave.Level saveLoc = new GCSave.Level();
          //  saveLoc.words = new List<GCSave.Word>();
          //  saveLoc.id = lvl.id;

          //  saveLevels[i] = saveLoc;
          //}
        }

        if (OnLoad != null) OnLoad();

      }
      );

    }


    public bool billingRestore = false;

    public void CheckDownloadIfNeed(Action onComplete = null) {
      try {

        if (!firstLoad) return;
        if (billingRestore) return;

        if (isLoadProcess) {
          needLoad = true;
          return;
        }

        if (Locker.Instance != null)
          Locker.Instance.SetLocker(true);

        billingRestore = false;
        AllDownload(() => {
          try {
            if (Locker.Instance != null)
              Locker.Instance.SetLocker(false);
            if (onComplete != null)
              onComplete();
          } catch {
            if (Locker.Instance != null)
              Locker.Instance.SetLocker(false);
            if (onComplete != null)
              onComplete();
          }
        });
      } catch {
        if (onComplete != null)
          onComplete();
      }
    }

    public void CheckNeedLoad() {
      if (needLoad) {
        needLoad = false;
        CheckDownloadIfNeed();
      }
    }

    public bool CheckExistNextLevel() {
      return GetActualCompany().levels.Count > actualLevelNum + 1;
    }

    void SelectCoinsWord(GC.Level lvl) {

      List<GC.Word> lvlWord = lvl.GetAllPrimaryWords();

      int selectWord = UnityEngine.Random.Range(1, 3);
      if (lvlWord.Count < selectWord)
        selectWord = lvlWord.Count;

      List<string> useWordList = new List<string>();

      while (selectWord > 0) {

        string useWord = "";

        do {
          useWord = lvlWord[UnityEngine.Random.Range(0, lvlWord.Count)].word;
        } while (useWordList.Contains(useWord));
        selectWord--;
        useWordList.Add(useWord);

        int coins = UnityEngine.Random.Range(1, 3);
        if (useWord.Length < coins)
          coins = useWord.Length;

        List<int> useLetterList = new List<int>();

        while (coins > 0) {

          int useCoin = 0;
          do {
            useCoin = UnityEngine.Random.Range(0, useWord.Length);
          } while (useLetterList.Contains(useCoin));
          coins--;
          useLetterList.Add(coins);

          var gameWord = lvlWord.Find(x => x.word == useWord);

          if (gameWord != null) {
            if (gameWord.coinsLetters == null)
              gameWord.coinsLetters = new List<int>();
            gameWord.coinsLetters.Add(useCoin);
          }

        }
      }
    }

    public void SaveCompanyWord(GameCompany.Save.Word word, bool isCompleted = false, int starCount = 0) {

      GCSave.Company useCompany = saveCompanies.Find(x => x.shortCompany == actualCompany);
      GCSave.Level useLevel = GetActualSaveLevel();
      GCSave.Word useWord = useLevel.words.Find(x => x.word == word.word);

      if (useWord != null && useWord.isOpen)
        return;

      if (!PlayerManager.Instance.company.isBonusLevel && !Tutorial.Instance.isTutorial && starCount > 0) {
        //AddStar();
        PlayerManager.Instance.stars += starCount;
      }

      if (useWord != null) {
        if (word.isOpen) {
          useWord.isOpen = word.isOpen;
          return;
        } else {
          for (int i = 0; i < word.hintLetters.Count; i++)
            if (!useWord.hintLetters.Contains(word.hintLetters[i]))
              useWord.hintLetters.Add(word.hintLetters[i]);
          return;
        }
      }

      useLevel.words.Add(word);
    }

    /// <summary>
    /// Праворка доступности уровня
    /// </summary>
    /// <param name="levelId">Идентификатор уровня</param>
    /// <returns>Статус открытого уровня</returns>
    public bool CheckOpenLevel(int levelNum) {

      if (levelNum == 0)
        return true;

      GC.Company company = GetActualCompany();
      GC.Save.Company saveCompany = GetActualSaveCompany();

      for (int j = 0; j < saveCompany.levels.Count; j++) {

        if (j >= levelNum)
          continue;

        if (!saveCompany.levels[j].isComplited)
          return false;
      }

      return true;

    }

    public GCSave.Level GetSaveLevel() {
      try {

        if (isBonusLevel)
          return saveCompanies.Find(x => x.shortCompany == actualCompany).bonusLevels[lastBonus - 1];

        if (Tutorial.Instance.isTutorial)
          return saveCompanies.Find(x => x.shortCompany == actualCompany).bonusLevels[Tutorial.Instance.tutorialLevel];

        return saveCompanies.Find(x => x.shortCompany == actualCompany).levels[actualLevelNum];
      } catch {
        return null;
      }
    }

    public GCSave.Level GetSaveLevel(string shortLanuage, int levelNum) {
      try {
        return saveCompanies.Find(x => x.shortCompany == shortLanuage).levels[levelNum];
      } catch {
        return null;
      }
    }


    /// <summary>
    /// Получить текущий уровень
    /// </summary>
    /// <returns></returns>
    public GC.Level GetActualLevel() {
      try {

        if (isBonusLevel)
          return GetActualCompany().bonusLevels[lastBonus - 1];

        if (Tutorial.Instance.isTutorial)
          return GetActualCompany().bonusLevels[Tutorial.Instance.tutorialLevel];

        return GetActualCompany().levels[actualLevelNum];
      } catch {
        return null;
      }
    }

    /// <summary>
    /// Получить текущий уровень
    /// </summary>
    /// <returns></returns>
    public GC.Company GetActualCompany() {
      try {
        return companies.Find(x => x.short_name == actualCompany);
      } catch {
        return null;
      }
    }

    public GCSave.Company GetActualSaveCompany() {
      try {
        return saveCompanies.Find(x => x.shortCompany == actualCompany);
      } catch {
        return null;
      }
    }

    public GCSave.Company GetlSaveCompany(string shortCode) {
      try {
        return saveCompanies.Find(x => x.shortCompany == shortCode);
      } catch {
        return null;
      }
    }


    public GCSave.Level GetActualSaveLevel() {
      try {

        if (isBonusLevel)
          return GetActualSaveCompany().bonusLevels[lastBonus - 1];

        if (Tutorial.Instance.isTutorial)
          return GetActualSaveCompany().bonusLevels[Tutorial.Instance.tutorialLevel];

        return GetActualSaveCompany().levels[actualLevelNum];
      } catch {
        return null;
      }
    }

    public int actualLevelNum { get; set; }

    /// <summary>
    /// Чтение слова
    /// </summary>
    /// <param name="word"></param>
    public void ReadWord(string word) {

      GC.Level lvl = GetActualLevel();

      GCSave.Level saweLevel = GetSaveLevel();

      bool oldComplete = saweLevel != null && saweLevel.isComplited;

      GC.Word wrd = lvl.words.Find(x => x.word == word);

      if (wrd == null) {
        GameEvents.OnWordSelect.Call(word, SelectWord.no);
        return;
      }

      if (saweLevel != null && saweLevel.words.Exists(x => x.word == word && x.isOpen)) {
        GameEvents.OnWordSelect.Call(word, (wrd.primary ? SelectWord.repeat : SelectWord.specialRepeat));
        return;
      }

      if (!wrd.primary)
        ConchManager.Instance.AddValue();

      GameEvents.OnWordSelect.Call(word, (wrd.primary ? SelectWord.yes : SelectWord.specialYes));

      GCSave.Word saveWord = new GCSave.Word();
      saveWord.word = word;
      saveWord.isOpen = true;


      SaveCompanyWord(saveWord, true, wrd.starCount);

      if (!oldComplete && battlePhase == BattlePhase.game) {
        ChackLevelComplited();
      }

      SaveOneLevel(actualCompany, actualLevelNum);

      GameEvents.OnWordSave.Call();

      RecurceFindWord();

    }

    private void RecurceFindWord() {

      List<GameLetter> gameLetter = (WorldManager.Instance.GetWorld(WorldType.game) as GameWorld).gameCrossword.letterList.FindAll(f => f.isOpen);

      GC.Level lvl = GetActualLevel();

      GCSave.Level saweLevel = GetSaveLevel();

      List<Word> wordNew = lvl.words.FindAll(x => x.primary && !saweLevel.words.Exists(y => y.word == x.word));

      bool exists = false;

      for (int i = 0; i < wordNew.Count; i++) {

        if (exists) continue;

        if (!wordNew[i].crosswordWord.letterList.Exists(w => !gameLetter.Exists(e => e.crosswordLetter.position == w.position))) {
          ReadWord(wordNew[i].word);
          exists = true;
        }

      }

    }

    /// <summary>
    /// Проверка, что уровень выполнен
    /// </summary>
    void ChackLevelComplited() {

      GC.Level level = GetActualLevel();
      GCSave.Level saveLevel = GetSaveLevel();
      GC.Company loc = GetActualCompany();

      if (saveLevel.words.Count == 0) return;

      bool isFull = true;

      for (int i = 0; i < level.words.Count; i++) {
        if (!level.words[i].primary) continue;
        if (!saveLevel.words.Exists(c => c.word == level.words[i].word && c.isOpen)) {
          isFull = false;
        }
      }

      if (isFull) {
        saveLevel.isComplited = true;
        RemoveNoCompletePush();

        if (PlayerManager.Instance.company.actualLevelNum > PlayerManager.Instance.company.lastLevelComplete)
          PlayerManager.Instance.company.lastLevelComplete = PlayerManager.Instance.company.actualLevelNum;

        battlePhase = BattlePhase.win;
        ExEvent.GameEvents.OnBattleChangePhase.Call(battlePhase);
        PlayerManager.Instance.Save();
        PlayerManager.Instance.CreateAllPush();
        try {
          FirebaseManager.Instance.LogEvent("level_complete");

          if (loc.levels.Count == actualLevelNum - 1)
            FirebaseManager.Instance.LogEvent("location_complete");


        } catch {
        }
      }

    }

    BattlePhase battlePhase = BattlePhase.none;
    public void OnLevelLoad() {
      battlePhase = BattlePhase.game;
      ExEvent.GameEvents.OnBattleChangePhase.Call(battlePhase);
    }

    public void BonusResume() {
      Debug.Log("BonusResume");
      battlePhase = BattlePhase.game;
      ExEvent.GameEvents.OnDailyBonusResume.Call();
    }

    #region Бонусный левел

    public bool isBonusLevel;         // Активный бонусный уровень
    [HideInInspector]
    public int lastBonus = 0;       // Активный бонус
    [HideInInspector]
    public GamePhase lastPhase;           // Последняя открытая сцена

    public GC.Level GetNextScene() {
      return GetActualCompany().bonusLevels[lastBonus - 1];
    }

    public void BonusLevelTimeEnd() {
      if (battlePhase != BattlePhase.game) return;
      battlePhase = BattlePhase.full;
      ExEvent.GameEvents.OnBattleChangePhase.Call(battlePhase);
    }

    #endregion

    #region Push

    private PushInfoSave pushInfo;

    public void CreateNoCompleteLevelPush() {

      GC.Level level = GetActualLevel();
      GCSave.Level saveLevel = GetSaveLevel();

      List<string> wordList = new List<string>();

      for (int i = 0; i < level.words.Count; i++) {
        if (!level.words[i].primary) continue;
        if (!saveLevel.words.Exists(c => c.word == level.words[i].word && c.isOpen)) {
          wordList.Add(level.words[i].word);
        }
      }

      if (wordList.Count <= 0) return;
      RemoveNoCompletePush();

      string word = wordList[UnityEngine.Random.Range(0, wordList.Count)];

      string sendText = String.Format("{0} '{1}' {2}", LanguageManager.GetTranslate("push.helpWord1"), word, LanguageManager.GetTranslate("push.helpWord2"));

      string pushId = PushManager.Instance.CreatePush(sendText, DateTime.Now.AddHours(12));

      pushInfo = new PushInfoSave() {
        langCompany = actualCompany,
        levelNum = actualLevelNum,
        word = word,
        pushId = pushId
      };

    }

    private void RemoveNoCompletePush() {

      if (pushInfo == null) return;

      PushManager.Instance.RemovePush(pushInfo.pushId);
      pushInfo = null;
      /*
			if (actualCompany == pushInfo.langCompany
				&& actualLocationNum == pushInfo.levelNum
				&& actualLevelNum == pushInfo.levelNum
				&& !string.IsNullOrEmpty(pushInfo.pushId)) {
				PushManager.Instance.RemovePush(pushInfo.pushId);
				pushInfo = null;
			}
			*/
    }

    #endregion

    #region Работа с файлом

    // TODO Добисать работу с файлами

    public void WriteToFile() {

      companies.ForEach(comp => {

        string path = String.Format("{0}/Resources/Company/{1}.json", Application.dataPath, comp.short_name);

        System.IO.File.WriteAllLines(path, new string[] { Newtonsoft.Json.JsonConvert.SerializeObject(comp) });

        Debug.Log("Wrote Company = " + comp.short_name);

      });

    }

    public void ReadFile(string code) {
      string path = String.Format("{0}/Resources/Company/{1}.json", Application.streamingAssetsPath, code);
      System.IO.File.ReadAllText(path);
    }

    #endregion

    #region Crossword

    public void CreateCrossword() {

      Debug.Log("Create crossword");

      companies.ForEach(cmp => {

        try {
          cmp.levels.ForEach(lvl => {
            CreateLevelCrossword(lvl);
          });
        } catch { }

        cmp.bonusLevels.ForEach(lvl => {
          try {
            CreateLevelCrossword(lvl);
          } catch { }
        });

      });

    }

    public void RemoveNotExistsCrossword() {

      companies.ForEach(cmp => {
        cmp.levels.RemoveAll(x => x.crosswordLetter.Count <= 0);
        cmp.bonusLevels.RemoveAll(x => x.crosswordLetter.Count <= 0);
      });

    }

    public void CreateOneLevel(string langCode, int index) {

      Company comp = companies.Find(x => x.short_name == langCode);
      CreateLevelCrossword(comp.levels[index]);
    }

    public void CreateOneLevel(string lang, int? level) {

      if (string.IsNullOrEmpty(lang) || level == null) return;

      Company cmp = companies.Find(x => x.short_name == lang);
      if (cmp == null) return;

    }

    private void CreateLevelCrossword(Level level) {

      int repeatCount = 20;

      while (repeatCount > 0) {
        repeatCount--;

        List<Crossword.Word> crosswordWordsList = new List<Crossword.Word>();
        List<Crossword.Letter> crowwsordLetterList = new List<Crossword.Letter>();

        if (Crossword.Crossword.Create(level, level.words.FindAll(x => x.primary), ref crosswordWordsList, ref crowwsordLetterList)) {

          Debug.Log("Complete");

          repeatCount = 0;

          crosswordWordsList.ForEach(word => {

            Word wrd = level.words.Find(w => w.word == word.word);

            wrd.crosswordWord = word;
            level.crosswordLetter = crowwsordLetterList;

          });

          CreateCoins(level.crosswordLetter);
        }
      }


    }

    #endregion

    private void CreateCoins(List<Crossword.Letter> letterList) {

      int coinsCount = UnityEngine.Random.Range(1, 3);

      List<Crossword.Letter> useLetter = new List<Crossword.Letter>();

      while (coinsCount > 0) {

        Crossword.Letter useWord;

        do {
          useWord = letterList[UnityEngine.Random.Range(0, letterList.Count)];
        } while (useLetter.Contains(useWord));
        useLetter.Add(useWord);
        useWord.isCoin = true;
        coinsCount--;

      }
    }


    #region Экспорт в файл

#if UNITY_EDITOR

    public void ReportLevelsTranslate() {

      string recordText = "";

      companies.ForEach(comp => {

        recordText = "";
        int levelNum = 0;

        comp.levels[0].words[0].translations.ForEach(tr => {
          recordText += "\t" + tr.lang;
        });

        comp.levels.ForEach(lev => {
          levelNum++;
          recordText += "\nLevel: " + levelNum.ToString();
          recordText += PrintLevelTranslate(lev);
          recordText += "\n";

        });

        WriteData(comp.short_name, recordText, false);

        recordText = "";
        levelNum = 0;
        comp.levels[0].words[0].translations.ForEach(tr => {
          recordText += "\t" + tr.lang;
        });

        comp.bonusLevels.ForEach(lev => {
          levelNum++;
          recordText += "\nLevel: " + levelNum.ToString();
          recordText += PrintLevelTranslate(lev);
          recordText += "\n";

        });

        WriteData(comp.short_name, recordText, true);

      });

    }

    private string PrintLevelTranslate(Level level) {

      string outData = "";

      string wordLine = "";

      level.words.ForEach(wrd => {
        wordLine += "\n" + wrd.word;

        wrd.translations.ForEach(tr => {
          if (!String.IsNullOrEmpty(tr.value))
            wordLine += "\t" + tr.value;
          else
            wordLine += "\t";

        });

      });

      outData += wordLine;

      return outData;
    }


    public void ReportLevelsCrossword() {

      string recordText = "";

      companies.ForEach(comp => {

        recordText = "";
        int levelNum = 0;

        comp.levels.ForEach(lev => {
          levelNum++;
          recordText += "\nLevel: " + levelNum.ToString();
          recordText += PrintLevelCrossword(lev);
          recordText += "\n\n==============================\n";

        });

        WriteData(comp.short_name, recordText, false);

        recordText = "";
        levelNum = 0;

        comp.bonusLevels.ForEach(lev => {
          levelNum++;
          recordText += "\nLevel: " + levelNum.ToString();
          recordText += PrintLevelCrossword(lev);
          recordText += "\n\n==============================\n";

        });

        WriteData(comp.short_name, recordText, true);

      });

    }

    private string PrintLevelCrossword(Level level) {

      string outData = "";

      string lettersLine = "";

      level.letters.ForEach(let => {
        lettersLine += " " + let;
      });

      outData += "\nLetters: " + lettersLine;

      string wordLine = "";

      level.words.ForEach(wrd => {
        if (wrd.primary)
          wordLine += " " + wrd.word;
      });

      outData += "\nWords: " + wordLine;

      outData += "\nCrossword:\n";

      if (level.crosswordLetter.Count == 0)
        outData += "*** NOT CROSSWORD ***";
      else
        outData += GetPrintCrossword(level.crosswordLetter);

      return outData;
    }

    private void WriteData(string langCode, string recordText, bool isBonus = false) {
      using (FileStream fstream = new FileStream(Application.dataPath + "/Texts/Reports/Levels_" + langCode + "_" + (isBonus ? "bonus" : "") + ".txt", FileMode.OpenOrCreate)) {
        // преобразуем строку в байты
        byte[] array = System.Text.Encoding.UTF8.GetBytes(recordText);
        // запись массива байтов в файл
        fstream.Write(array, 0, array.Length);
        Debug.Log("Текст записан в файл");
      }
    }

    public static string GetPrintCrossword(List<Crossword.Letter> allLetters) {

      Vector2Int minPosition = Vector2Int.zero;
      Vector2Int maxPosition = Vector2Int.zero;

      allLetters.ForEach(let => {

        if (minPosition.x >= let.position.x)
          minPosition.x = let.position.x;

        if (minPosition.y >= let.position.y)
          minPosition.y = let.position.y;

        if (maxPosition.x <= let.position.x)
          maxPosition.x = let.position.x;

        if (maxPosition.y <= let.position.y)
          maxPosition.y = let.position.y;


      });
      string data = "";

      for (int y = minPosition.y; y <= maxPosition.y; y++) {
        for (int x = minPosition.x; x <= maxPosition.x; x++) {

          Crossword.Letter letDat = allLetters.Find(ex => ex.position == new Vector2Int(x, y));

          if (letDat == null) {
            data += "\t ";
          } else {
            data += "\t" + letDat.letter;
          }

        }
        data += "\n";
      }
      return data;

    }

#endif

    #endregion


  }




  public class PushInfoSave {

    public string langCompany;
    public int locationNum;
    public int levelNum;
    public string word;
    public string pushId;
  }

  public class SaveProgress {
    public List<GC.Company> company;
    public List<GCSave.Company> progress;
    public int lastBonus;
    public string activeCompany;
    public PushInfoSave pushLevelNoComplete;
    public int lastLevelComplete;
  }


}

public enum BattlePhase {
  none,
  game,
  win,
  full
}