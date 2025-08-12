using System;
using System.Collections.Generic;
using ExEvent;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;
using ZbCatScene;
using Zombich.CollectionItems;

[Flags]
public enum GameMode {
  company = 1,
  survival = 2,
  crusade = 4
}

namespace Game.User {
  /// <summary>
  /// Информация о игроке
  /// </summary>
  public class UserManager: Singleton<UserManager> {
    [HideInInspector]
    public UserMedicineChest medicineChest = new UserMedicineChest();

    public Counter<int> silverCoins = new Counter<int>();
    public Counter<int> goldCoins = new Counter<int>();
    public Counter<int> healthLevel = new Counter<int>();
    public Counter<int> difficultyLevel = new Counter<int>();

    [HideInInspector] public bool lastComplited;

    private UserProgress _userProgress;
    public UserProgress UserProgress {
      get { return _userProgress; }
      set { _userProgress = value; }
    }

    public static GameMode gameMode; // Режим игры
    [HideInInspector] public bool toCrussade; // Переход на крестовый поход

    public AudioBlock newLevelAudio;

    public int Experience {
      get { return UserProgress.experience; }
      set {
        UserProgress.experience = value;
        int newLevel = UserProgress.PlayerLevel();

        if (newLevel > UserProgress.playerLevel) {
          UserProgress.playerLevel = newLevel;
          newLevelAudio.PlayRandom(this);
          ExEvent.UserEvents.OnLevelChange.Call(UserProgress.playerLevel);
        }
      }
    }

    private CollectionItemsManager collectionItemManager;

    protected override void Awake() {
      base.Awake();
      UserProgress = new UserProgress();
      medicineChest = new UserMedicineChest();
      medicineChest.Init();

      gameObject.AddComponent<UserHealth>();
      gameObject.AddComponent<UserRage>();

      collectionItemManager = GetComponent<CollectionItemsManager>();
      XMLParcer.ParsingComplited += GetConfig;
    }

    private void Start() {
      LoadData();
      healthLevel.OnChangeValue += OnHealthChange;
    }

    private void OnHealthChange(int obj) {
      UserEvents.OnHealthLevel.CallAsync(healthLevel.Value);
    }

    protected override void OnDestroy() {
      base.OnDestroy();
    }

    private void Update() {
      if (gameMode == GameMode.crusade) {
        medicineChest.Update();
      }
    }

    /// <summary>
    /// Обработка выхода из игры
    /// </summary>
    private void OnApplicationQuit() {
      SaveData();
    }

    public void ResetDefaultData() {
      PlayerPrefs.DeleteAll();
      LevelsManager.Instance.Clear();
      LoadStartParametrs();
      CatSceneManager.Instance.Reset();
      LoadData();
      AudioManager.Instance.SetDefaultParametrs();
      SceneManager.LoadScene("Menu");
    }

    public float MapZoom { get; set; } // Зум на карте

    private void LoadStartParametrs() {
      difficultyLevel.Value = 1;
      UserProgress.Init();

      LevelsManager.Instance.Clear();

      Game.User.UserWeapon.Instance.SetDefaultWeapon();

      MapZoom = 0;
    }

    /// <summary>
    /// Загрузка данны по игроку
    /// </summary>
    public void LoadData() {

      if (!PlayerPrefs.HasKey("userData")) {
        LevelsManager.Instance.Clear();
        LoadStartParametrs();
        return;
      }

      string userDataString = PlayerPrefs.GetString("userData", "");

      Dictionary<string, object> userData = JsonConvert.DeserializeObject<Dictionary<string, object>>(userDataString);

      UserProgress = JsonConvert.DeserializeObject<UserProgress>(userData["userProgress"].ToString());
      toCrussade = bool.Parse(userData["toCrussade"].ToString());
      MapZoom = float.Parse(userData["mapZoom"].ToString());
      try {
        survivalFonUse = JsonConvert.DeserializeObject<List<int>>(userData["survivalFon"].ToString());
      } catch {
      }

      if (userData.ContainsKey("lastLevel") && userData["lastLevel"] != null) {
        ActiveBattleInfo =
          LevelsManager.Instance.LevelsList.Find(x => x.PointNum == int.Parse(userData["lastLevel"].ToString()));
      }

      UserWeapon.Instance.Load();

      collectionItemManager.SetData(
        JsonConvert.DeserializeObject<List<CollectionItem>>(userData["collectionItems"].ToString()));

      if (userData.ContainsKey("catScenes"))
        ZbCatScene.CatSceneManager.Instance.Load(
          JsonConvert.DeserializeObject<Dictionary<string, bool>>(userData["catScenes"].ToString()));
    }

    /// <summary>
    /// Сохранение данныхх игрока
    /// </summary>
    public static void SaveUser() {
      try {
        Instance.SaveData();
      } catch (Exception e) {
        Debug.LogError("Error save user: " + e.Message);
      }
    }

    /// <summary>
    /// Сохранение данных по игроку
    /// </summary>
    public void SaveData() {

      Dictionary<string, object> userData = new Dictionary<string, object>();
      userData["userProgress"] = UserProgress;
      userData["toCrussade"] = toCrussade;
      userData["mapZoom"] = MapZoom;
      userData["lastLevel"] = ActiveBattleInfo != null ? ActiveBattleInfo.PointNum : 1;
      Game.User.UserWeapon.Instance.Save();
      /*
      userData["weapon"] = weaponData;
      userData["weaponSelectCompany"] = weaponSelectCompany;
      userData["weaponSelectSurvival"] = weaponSelectSurvival;
      */
      userData["collectionItems"] = collectionItemManager.GetData();
      userData["survivalFon"] = survivalFonUse;
      userData["catScenes"] = ZbCatScene.CatSceneManager.Instance.Save();

      Debug.Log(JsonConvert.SerializeObject(userData));

      PlayerPrefs.SetString("userData", JsonConvert.SerializeObject(userData));
    }

    #region Survival fon

    public List<int> survivalFonUse = new List<int>();

    public int GetSurvivalFon() {
      List<Configuration.Level> listArr =
        GameDesign.Instance.allConfig.levels.FindAll(x => x.isSurvivle && !survivalFonUse.Contains(x.fon));

      if (listArr.Count == 0) {
        survivalFonUse.Clear();
        listArr = GameDesign.Instance.allConfig.levels.FindAll(x => x.isSurvivle && !survivalFonUse.Contains(x.fon));
      }

      int useMap = listArr[UnityEngine.Random.Range(0, listArr.Count)].fon;
      survivalFonUse.Add(useMap);

      return useMap;
    }

    #endregion

    #region Уровни на карте

    public bool CheckLevelComplited(int group, int level) {
      return LevelsManager.Instance.CheckComplited(group, level);
    }

    public void SetPointStatus(ref MapPointParametr[] pointList) {

      int readyGroup = 1;

      for (int i = 0; i < pointList.Length; i++) {

        // Проверяем открытые группы
        if (pointList[i].pointInfo.PrevPoints.Count > 0) {
          for (int j = 0; j < pointList[i].pointInfo.PrevPoints.Count; j++) {
            try {
              if (pointList[pointList[i].pointInfo.PrevPoints[j]].pointInfo.Mode == PointMode.farm)
                readyGroup = pointList[i].pointInfo.Group;
            } catch {
              Debug.Log(i);
            }
          }
        }

        if (pointList[i].pointInfo.Group > readyGroup) {
          pointList[i].pointInfo.Status = PointSatus.blocked;
          continue;
        }

        if (pointList[i].pointInfo.Status == PointSatus.complited) {
          pointList[i].pointInfo.Status = PointSatus.complited;
          continue;
        }

        if (i == 0
            || (i > 0 && pointList[i - 1].pointInfo.Mode == PointMode.farm &&
                pointList[i - 1].pointInfo.Group == pointList[i].pointInfo.Group)
            || (i < pointList.Length - 1 && pointList[i + 1].pointInfo.Mode == PointMode.farm &&
                pointList[i + 1].pointInfo.Group == pointList[i].pointInfo.Group)) {
          pointList[i].pointInfo.Status = PointSatus.open;
          continue;
        }

        if (pointList[i].pointInfo.PrevPoints.Count > 0) {
          bool set = false;
          for (int j = 0; j < pointList[i].pointInfo.PrevPoints.Count; j++) {
            if (pointList[i].pointInfo.Mode == PointMode.appendix) {
              if (pointList[pointList[i].pointInfo.PrevPoints[j]].pointInfo.isApendixComplete) {
                pointList[i].pointInfo.Status = PointSatus.open;
                set = true;
              }
            } else {
              if (pointList[pointList[i].pointInfo.PrevPoints[j]].pointInfo.Mode == PointMode.farm) {
                pointList[i].pointInfo.Status = PointSatus.open;
                set = true;
              }
            }
          }

          if (set)
            continue;
        }

        pointList[i].pointInfo.Status = PointSatus.closed;
      }
    }

    #endregion

    #region Прогресс уровней

    public LevelInfo ActiveBattleInfo { set; get; } // Активная точка

    public void LevelComplited(float timeBattle) {

      ActiveBattleInfo.SetComplete(timeBattle);
      if (gameMode == GameMode.company && ActiveBattleInfo.Group == 35 && ActiveBattleInfo.Level == 6) {
        gameMode = GameMode.crusade;
        toCrussade = true;
      }

    }

    #endregion

    #region Настройки

    public void GetConfig(Configuration.Config data) {

      Configuration.Summary playerEnergy = data.summary.Find(x => x.name == "PlayerEnergy");
      UserProgress.energyMax = playerEnergy.param1;
      UserProgress.energyRecoverSpeed = playerEnergy.param2;

      silverCoins.Initiate("silverCoins", (int)data.summary.Find(x => x.name == "PlayerCoins").param1);
      goldCoins.Initiate("goldCoins", 0);
      healthLevel.Initiate("healthLevel", 0);
      difficultyLevel.Initiate("difficultyLevel", 1);
      UserHealth.Instance.Initiate(data.summary.Find(x => x.name == "PlayerHealth").param1);
    }

    #endregion

  }

  public class LostWeapon {
    public Game.Weapon.WeaponType type;
    public int count;
  }

  public enum UserStat {
    none = 1,
    power = 2,
    health = 3,
    energy = 4
  }
}