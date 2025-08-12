using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class UserManager: Singleton<UserManager> {

  public float maxLive = 100;
  public float nowLive;
  public float minLive;
  private bool play = true;
  private Location m_ActiveLocation;

  private int m_SelectBlock = 0;
  private int m_SelectLevel = 0;
  private int m_CompleteLastLevel = 0;
  private int m_MaxCompleteLevel = 0;
  private int m_RunCount = 0;

  public Location ActiveLocation {
    get { return m_ActiveLocation; }
    set {
      m_ActiveLocation = value;
      SelectLevel = m_ActiveLocation.Level;
      m_ActiveLocation.Initiate();
    }
  }

  private float patron = 0;

  //изменение softCash событие
  public static event System.Action<int> SoftCashChange;
  private int m_Gold;
  private int m_GoldIncrement;
  public int Gold {
    get { return m_Gold - m_GoldIncrement; }
    set {
      m_GoldIncrement = (int)(Random.value * 1000);
      m_Gold = value + m_GoldIncrement;
      PlayerPrefs.SetInt("gold", m_Gold);
      PlayerPrefs.SetInt("goldIncrement", m_GoldIncrement);
      if (SoftCashChange != null)
        SoftCashChange(m_Gold - m_GoldIncrement);
    }
  }

  //изменение hardCash событие
  public static event System.Action<int> HardCashChange;
  private int m_HardCash;
  private int m_HardCashIncrement;
  public int HardCash {
    get { return m_HardCash - m_HardCashIncrement; }
    set {
      m_HardCashIncrement = (int)(Random.value * 1000);
      m_HardCash = value + m_HardCashIncrement;
      PlayerPrefs.SetInt("hardCash", m_HardCash);
      PlayerPrefs.SetInt("hardCashIncrement", m_HardCashIncrement);
      if (HardCashChange != null)
        HardCashChange(m_HardCash - m_HardCashIncrement);
    }
  }

  [ContextMenu("Add 1000 hard")]
  private void AddHardCacheContext() {
    HardCash += 1000;
  }
  [ContextMenu("Add 1000 coins")]
  private void AddCoinContext() {
    Gold += 1000;
  }

  protected override void Awake() {
    base.Awake();
    Load();
  }

  public void Start() {
    nowLive = maxLive;

    if (m_RunCount <= 0) {
      AppMetrica.Instance.ReportEvent("install");
    }

    RunCount++;
  }
  public void Save() {
  }

  public void Load() {

    m_RunCount = PlayerPrefs.GetInt("runCount", 0);


    m_GoldIncrement = PlayerPrefs.GetInt("goldIncrement", 0);
    m_HardCashIncrement = PlayerPrefs.GetInt("hardCashIncrement", 0);
    m_Gold = PlayerPrefs.GetInt("gold", 0);
    m_HardCash = PlayerPrefs.GetInt("hardCash", 0);
    m_MaxCompleteLevel = PlayerPrefs.GetInt("maxCompleteLevel", -1);
    m_SelectLevel = PlayerPrefs.GetInt("selectLevel", 0);
    m_SelectBlock = PlayerPrefs.GetInt("selectBlock", 0);
    m_CompleteLastLevel = PlayerPrefs.GetInt("completeLastLevel", 0);

    Debug.Log("Load CompleteLastLevel: " + UserManager.Instance.CompleteLastLevel);
    Debug.Log("Load MaxCompleteLevel: " + UserManager.Instance.MaxCompleteLevel);


    ExEvent.GameEvents.OnLoad.Call();

  }

  public void UseMedBonus(float bonus, bool isRestore = false) {
    nowLive += bonus;

    if (isRestore)
      play = true;

    if (nowLive > maxLive)
      nowLive = maxLive;
    QuestionManager.Instance.AddValueQuest(QuestionManager.Type.getMegicine, 1);
    ExEvent.UserEvents.UserMedicineBonus.Call(nowLive, maxLive);
  }

  public void GetSoftCashBonus(int bonus) {
    Gold += bonus;
  }

  public void GetHardCashBonus(int bonus) {
    HardCash += bonus;
  }

  public void GetPatronBonus(float bonus) {
    QuestionManager.Instance.AddValueQuest(QuestionManager.Type.getBullets, bonus);
    patron += bonus;
  }

  [HideInInspector]
  public int damageCount = 0;

  public void Damage(float value) {
    if (!play)
      return;
    if (value < nowLive) {
      nowLive -= value;
      ExEvent.UserEvents.UserDamage.Call(maxLive, nowLive, value);

      damageCount++;

    } else {
      play = false;
      nowLive = 0;
      ExEvent.UserEvents.UserDied.Call(nowLive);
    }

  }

  [ExEvent.ExEventHandler(typeof(ExEvent.BattleEvents.BattleStart))]
  private void BattleStart(ExEvent.BattleEvents.BattleStart eventData) {
    play = true;
    nowLive = maxLive;
    damageCount = 0;
  }

  public int SelectBlock {
    get { return m_SelectBlock; }
    set {
      m_SelectBlock = value;
      PlayerPrefs.SetInt("selectBlock", m_SelectBlock);
    }
  }

  public int SelectLevel {
    get { return m_SelectLevel; }
    set {
      m_SelectLevel = value;
      PlayerPrefs.SetInt("selectLevel", m_SelectLevel);
    }
  }
  public int CompleteLastLevel {
    get { return m_CompleteLastLevel; }
    set {
      m_CompleteLastLevel = value;
      PlayerPrefs.SetInt("completeLastLevel", m_CompleteLastLevel);
    }
  }

  public int MaxCompleteLevel {
    get { return m_MaxCompleteLevel; }
    set {

      if (value <= m_MaxCompleteLevel)
        return;

      m_MaxCompleteLevel = value;

      PlayerPrefs.SetInt("maxCompleteLevel", m_MaxCompleteLevel);
    }
  }

  public int MaxOpenBlock {
    get {
      return (int)MaxCompleteLevel / 100;
    }
  }

  public bool ReadyMoveToBlock(int block) {
    return block >= 0 && block <= LocationManager.Instance.GetMaxBlock() && block <= UserManager.Instance.MaxOpenBlock;
  }

  public bool IncrementBlock() {
    int beforeBlock = MaxOpenBlock;
    int maxBlock = LocationManager.Instance.GetMaxBlock();

    if (beforeBlock + 1 > maxBlock+1)
      return false;

    MaxCompleteLevel = (beforeBlock + 1) * 100;

    SelectBlock = Mathf.Min(beforeBlock + 1, maxBlock);

    return true;
  }


  public int RunCount {
    get {
      return m_RunCount;
    }
    private set {
      m_RunCount = value;
      PlayerPrefs.SetInt("runCount", m_RunCount);
    }

  }

  #region Buy Actions

  private Queue<System.Action> byeActionsList = new Queue<System.Action>();

  public void AddAction(System.Action action) {
    byeActionsList.Enqueue(action);
  }
  public void ConfirmAction() {
    if (byeActionsList.Count <= 0)
      return;
    (byeActionsList.Dequeue())();
  }

  public void ConfirmAllActions() {
    while (byeActionsList.Count > 0)
      ConfirmAction();
  }

  #endregion

}
