using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Информация уровня
/// </summary>
[Serializable]
public class LevelInfo {

  [SerializeField]
	private int group;
  private int? tmpGroup;
  /// <summary>
  /// Чаптер
  /// </summary>
  public int Group { get { return group; }
    set {
      tmpGroup = value;
      isTemp = true;
    }
  }

  public void SetGroup(int group){
    this.group = group;
  }

  [SerializeField]
	private int level;
  private int? tmpLevel;
  /// <summary>
  /// Уровень
  /// </summary>
  public int Level { get { return level; }
    set {
      tmpLevel = value;
      isTemp = true;
    }
  }
  
  public void SetLevel(int level){
    this.level = level;
  }
  public bool IsComplete {
    get {
      return Mode != PointMode.company || Status == PointSatus.complited;
    }
  }

  [SerializeField]
	private int pointNum;
  /// <summary>
  /// Номер локации
  /// </summary>
  public int PointNum { get { return pointNum; } }

  [SerializeField]
	private PointMode mode;
  private PointMode? saveMode;
  /// <summary>
  /// Режим
  /// </summary>
  public PointMode Mode {
    get { return saveMode != null ? saveMode.Value : mode; }
    set {
      saveMode = value;
      Save();
    }
  }

  private bool isTemp { get; set; }

  [SerializeField]
	private PointSatus status;                         // Статус точки
  private PointSatus? saveStatus;
  /// <summary>
  /// Статус
  /// </summary>
  public PointSatus Status {
    get { return saveStatus != null ? saveStatus.Value : status; }
    set {

      if (saveStatus == value)
        return;

      saveStatus = value;

      switch (saveStatus){
        case PointSatus.ready:
          if(PrevLevels != null)
          PrevLevels.ForEach(x => {
            if (x.Status == PointSatus.blocked)
              x.Status = PointSatus.closed;
          });
          break;
        case PointSatus.open:
        case PointSatus.complited:
          if (PrevLevels != null)
            PrevLevels.ForEach(x => {
            if ((x.Status & (PointSatus.blocked | PointSatus.closed))!= 0)
              x.Status = PointSatus.ready;
          });
          break;
        default:
          break;
      }
              
      Save();
    }
  }

  public List<CompanyProgress> companyProgress;     // Прогресс компании разбитый на попытки прохождения
	
  public bool IsSleep { get; set; }
	public float Progress { get; set; }              // Прогресс компании

  /// <summary>
  /// Очки фарма
  /// </summary>
  public int FarmPoint { get; private set; }
  /// <summary>
  /// Использованные очки
  /// </summary>
  public int FarmPointActive { get; set; }
  [SerializeField]
	private List<int> prevPoint;                         // Точки, от которых зависим
  public List<int> PrevPoints { get { return prevPoint; } }

  public List<LevelInfo> PrevLevels { get; private set; }

  public bool isApendixComplete;

	public DateTime? TimeChange { get; private set; }                            // Время последнего изменения точки
	/// <summary>
	/// Устанвока даты изменения элемента
	/// </summary>
	public void Change() {
		TimeChange = DateTime.Now;
	}
	/// <summary>
	/// Добавление элемента в прогресс
	/// </summary>
	/// <param name="value">Новая степень прогресса</param>
	public void AddProgress(float value) {
		CompanyProgress prog = new CompanyProgress();
		prog.unixTime = Core.unixTime;
		prog.value = value;
		Progress += value;
		companyProgress.Add(prog);
	}
	/// <summary>
	/// Структура массива игр
	/// </summary>
	[System.Serializable]
	public struct CompanyProgress {
		public int unixTime;          // Время выполнения элемента прогресса
		public float value;           // Значение достижения
	}

	public LevelInfo Clone() {
		LevelInfo li = new LevelInfo();
		li.group = group;                                                     // Название компании
		li.level = level;                                                     // Уровень в компании
		li.pointNum = pointNum;                                               // Номер точки
		li.mode = mode;
		li.status = status;                                                   // Статус точки
		li.companyProgress = new List<CompanyProgress>(companyProgress);      // Прогресс компании разбитый на попытки прохождения
		li.Progress = Progress;                   // Прогресс компании
		li.FarmPointActive = FarmPointActive;                                 // Выбранное время миссии
		li.prevPoint = new List<int> (prevPoint);                             // Точки, от которых зависим
		li.isApendixComplete = isApendixComplete;
		li.TimeChange = TimeChange;                                           // Время последнего изменения точки
		return li;
	}

  private string PlayerPrefsKey { get { return "levelInfo_" + group + "_" + level; } }

  public void Save() {

    if (isTemp) return;

    SaveData sd = new SaveData() {
      status = saveStatus,
      mode = saveMode,
      farmPoint = FarmPointActive,
      timeChange = TimeChange,
      progress = Progress,
      isApendixComplete = isApendixComplete
    };
    PlayerPrefs.SetString(PlayerPrefsKey, Newtonsoft.Json.JsonConvert.SerializeObject(sd));
  }

  public void Load() {

    PrevLevels = LevelsManager.Instance.LevelsList.FindAll(x => prevPoint.Contains(x.PointNum));

    if (!PlayerPrefs.HasKey(PlayerPrefsKey))
      return;

    SaveData sd = Newtonsoft.Json.JsonConvert.DeserializeObject<SaveData>(PlayerPrefs.GetString(PlayerPrefsKey));
    saveStatus = sd.status;
    saveMode = sd.mode;
    FarmPointActive = sd.farmPoint;
    TimeChange = sd.timeChange;
    Progress = sd.progress;
    isApendixComplete = sd.isApendixComplete;

  }

  public void LoadGd() {
    try {
      FarmPoint = GameDesign.Instance.allConfig.levels.Find(x => x.chapter == Group && x.level == Level).countFarm;
    } catch {
      Debug.Log(Group + " : " + Level);
    }
  }

  public void Clear() {
    saveStatus = null;
    saveMode = null;
    FarmPointActive = 0;
    TimeChange = null;
    Progress = 0;
  }

  private struct SaveData {
    public PointSatus? status;
    public PointMode? mode;
    public int farmPoint;
    public DateTime? timeChange;
    public float progress;
    public bool isApendixComplete;
  }

  public void SetComplete(float time) {
    if(Mode == PointMode.company) {
      Mode = PointMode.farm;

      Status = FarmPoint != 0 ? PointSatus.open : PointSatus.complited;
      
      Game.User.UserManager.SaveUser();
      return;
    }

    if(Mode == PointMode.farm) {
      
      if (FarmPoint > 0 && !isApendixComplete)
        isApendixComplete = UnityEngine.Random.value <= 0.1f ? true : false;

      FarmPointActive++;
      // Обязательное открытие зависимой дочерней при 80% готовности фарма
      if (!isApendixComplete && FarmPoint / FarmPointActive >= 0.8f)
        isApendixComplete = true;
      if (FarmPoint <= FarmPointActive)
        Status = PointSatus.complited;

      Save();
    }
    Game.User.UserManager.SaveUser();
  }

}

/// <summary>
/// Режим миссии
/// </summary>
public enum PointMode {
	company = 0,
	farm = 1,
	appendix = 2,
	survival = 3,
	arena = 4
}

/// <summary>
/// Статус миссии
/// </summary>
[Flags]
public enum PointSatus {
	blocked = 1,                            // Заблокирована — находится в главе, закрытой туманом войны.
	closed = 2,                             // Закрыта — находится в открытой главе, но не пройдета предшествующая ей миссия.
  ready = 4,                              // Открыта для прохождения, но не пройдена основная часть
	open = 8,                               // Открыта, пройдена основная часть, но доступен фарм.
	complited = 16,                         // Открыта, полность пройдена.
  IsActive = (ready | open | complited),  // Открытая точка
  IsVisible = (IsActive | closed),        // Видимая точка
  IsClose = (blocked | closed | complited) // Запрет открытия
}