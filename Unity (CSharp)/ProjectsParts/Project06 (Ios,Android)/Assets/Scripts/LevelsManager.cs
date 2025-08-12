using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelsManager : Singleton<LevelsManager> {

  [SerializeField]
  private List<LevelInfo> levelsList = new List<LevelInfo>();

  public List<LevelInfo> LevelsList {
    get {
      return levelsList;
    }
  }
  
  private void Start() {
    Load();

    if (GameDesign.Instance.allConfig != null)
      levelsList.ForEach(x => x.LoadGd());

  }

  private void Load() {
    levelsList.ForEach(x => x.Load());
  }

  public void Clear() {
    levelsList.ForEach(x => x.Clear());
  }

  public bool CheckComplited(int group, int level) {
    LevelInfo li = levelsList.Find(x => x.Group == group && x.Level == level);

    if (li == null) return false;

    return li.IsComplete;
  }

  [ExEvent.ExEventHandler(typeof(ExEvent.GameEvents.GameDesignLoad))]
  public void GameDesignLoad(ExEvent.GameEvents.GameDesignLoad eventData) {
    levelsList.ForEach(x => x.LoadGd());
  }

  //public void AddConfirmLevel(LevelInfo battleInfo, float timeBattle) {

  //if (levelsData == null) levelsData = new List<LevelInfo>();

  //LevelInfo levelSave = levelsData.Find(x => x.Group == battleInfo.Group && x.Level == battleInfo.Level);

  //if (levelSave == null) {
  //  levelsData.Add(battleInfo);
  //  levelSave = battleInfo;
  //}

  //if (levelSave.Group == battleInfo.Group && levelSave.Level == battleInfo.Level) {
  //if (levelSave.Mode == PointMode.company) {
  //  levelSave.Mode = PointMode.farm;
  //} else if (levelSave.Mode == PointMode.farm) {

  //  // Максимальное число очков фарма
  //  int allFarm = GameDesign.Instance.allConfig.levels.Find(x => x.chapter == battleInfo.Group && x.level == battleInfo.Level).countFarm;

  //  // 10% Шанс на открытие дочерней миссии апендикса
  //  if (levelSave.FarmPoint > 0 && !levelSave.appendixConfirm)
  //    levelSave.appendixConfirm = UnityEngine.Random.value <= 0.1f ? true : false;

  //  levelSave.farmPointActive += battleInfo.farmPointActive;

  //  // Обязательное открытие зависимой дочерней при 80% готовности фарма
  //  if (!levelSave.appendixConfirm && levelSave.FarmPoint / allFarm >= 0.8f)
  //    levelSave.appendixConfirm = true;


  //  if (levelSave.FarmPoint >= GameDesign.Instance.allConfig.levels.Find(x => x.chapter == levelSave.Group && x.level == levelSave.Level).countFarm)
  //    levelSave.Status = PointSatus.complited;

  //} else if (levelSave.Mode == PointMode.appendix) {

  //}

  //}
  //User.SaveUser();

  //}
}
