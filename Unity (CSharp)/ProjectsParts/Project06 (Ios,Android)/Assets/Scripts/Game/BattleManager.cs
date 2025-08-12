using ExEvent;
using Game.User;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using ZbCatScene;
using Zombich.CollectionItems;
using Random = UnityEngine.Random;
using Game.Weapon;

/// <summary>
/// Фазы боя
/// </summary>
[Flags]
public enum BattlePhase
{
  start = 1,
  timer = 2,
  battle = 3,
  end = 4,
  pause = 5
}

public enum DropType
{
  weapon, superCoins, coins
}

[System.Serializable]
public struct Drop
{
  public DropType type;
  public WeaponType weaponType;
  public int count;
}

[System.Serializable]
public struct WeaponBullets
{
  public WeaponType weaponType;
  public int count;
}

/// <summary>
/// Общий менеджер по работе с моевой сценой
/// </summary>
public class BattleManager: Singleton<BattleManager>
{

  public static event System.Action<int> OnGoldCoins;
  public static event System.Action<int> OnSilverCoins;

  public static BattlePhase _battlePhase = BattlePhase.start;

  public static BattlePhase battlePhase
  {
    get { return _battlePhase; }
    set
    {
      _battlePhase = value;
      ExEvent.BattleEvents.BattlePhaseChange.Call(_battlePhase);
    }
  }
   
  float startTime;    // Время начала миссии
  float endTime;      // Время окончания миссии

  public static event Action<float> battleTimer;

  public static event Action OnBattleStart;

  protected override void Awake()
  {
    base.Awake();
  }

  [HideInInspector]
  public float timeBattle;
  [HideInInspector]
  public float speedIncrement;

  int _silverCoins;
  public int silverCoins
  {
    get
    {
      return _silverCoins;
    }
    set
    {
      _silverCoins = value;
      if (OnSilverCoins != null)
        OnSilverCoins(_silverCoins);
    }
  }

  int _goldCoins;             // Количество монет в бою
  public int goldCoins
  {
    get
    {
      return _goldCoins;
    }
    set
    {
      _goldCoins = value;
      if (OnGoldCoins != null)
        OnGoldCoins(_goldCoins);
    }
  }

  public int allCoins
  {
    get
    {
      return silverCoins + (goldCoins * 100);
    }
  }

  private void Start()
  {
    PlayerPrefs.DeleteKey("JustUnlockedWeapon");
    PlayerPrefs.DeleteKey("JustUnlockedWeapon_gr");
    PlayerPrefs.DeleteKey("JustUnlockedWeapon_lvl");
    ShowGamePlay();
    OpenBlack(null);
    SetStart();
  }

  private void ShowGamePlay()
  {
    UIController.ShowUi<Game.UI.Battle.BattleGamePlay>().gameObject.SetActive(true);
  }

  [ExEvent.ExEventHandler(typeof(ExEvent.GameEvents.KeyDown))]
  private void KeyDown(ExEvent.GameEvents.KeyDown keyDown)
  {


    if (keyDown.keyCode == KeyCode.Escape)
    {

      if (battlePhase == BattlePhase.battle || battlePhase == BattlePhase.start)
      {
        Pause();
      }
      if (battlePhase == BattlePhase.end)
        ExitBattle();
    }
  }

  public void SetStart()
  {
    battlePhase = BattlePhase.start;
    _lastPower = false;
    isLastPower = false;
    Time.timeScale = 1;
    Invoke("LoadReady", 1);

    UserHealth.Instance.StartBattle();
    ExEvent.BattleEvents.StartBattle.Call();
    InitDrop();
    
  }

  void LoadReady()
  {

    if (UserManager.Instance.ActiveBattleInfo.Mode == PointMode.survival)
    {
      ShowTimer();
      return;
    }


    if (CatSceneManager.Instance.isSpecLevel && ZbCatScene.CatSceneManager.Instance.ShowCatScene("36_2", () =>
    {
      ShowTimer();
    }))
      return;

    if (UserManager.Instance.ActiveBattleInfo.Group == 1
        && UserManager.Instance.ActiveBattleInfo.Level == 1
        && ZbCatScene.CatSceneManager.Instance.ShowCatScene(1, () =>
        {
          ShowTimer();
        }))
      return;

    if (UserManager.Instance.ActiveBattleInfo.Group == 1
        && UserManager.Instance.ActiveBattleInfo.Level == 2
        && ZbCatScene.CatSceneManager.Instance.ShowCatScene(4, () =>
        {
          ShowTimer();
        }))
      return;

    if (UserManager.Instance.ActiveBattleInfo.Group == 2
        && UserManager.Instance.ActiveBattleInfo.Level == 6
        && ZbCatScene.CatSceneManager.Instance.ShowCatScene(17, () =>
        {
          ShowTimer();
        }))
      return;

    ShowTimer();
  }

  void ShowTimer()
  {
    InitBackMusic();
    Timer timerInst = UIController.ShowUi<Timer>();
    timerInst.gameObject.SetActive(true);
    timerInst.OnStart = StartBattle;
    timerInst.OnNum = (num) =>
    {
      BattleEvents.NumberTimer.Call(num);
    };
  }

  public void NewWawe()
  {
    battlePhase = BattlePhase.battle;

    bearProtection = false;
    startTime = Time.time;
    timeBattle = 0;
    goldCoins = 0;
    silverCoins = 0;
  }

  public void StartBattle()
  {
    NewWawe();

    if (OnBattleStart != null)
      OnBattleStart();
  }


  [HideInInspector]
  public Drop[] readyDrop;
  [HideInInspector]
  public Drop[] getDrop;
  [HideInInspector]
  public List<Drop> getDropList;

  void InitDrop()
  {

    if (UserManager.Instance.ActiveBattleInfo.Mode == PointMode.survival || UserManager.Instance.ActiveBattleInfo.Mode == PointMode.arena )
    {
      getDropList = new List<Drop>();
      readyDrop = new Drop[0];
      getDrop = new Drop[0];
      return;
    }

    Configuration.Level activeLevel = GameDesign.Instance.allConfig.levels.Find(x => x.chapter == UserManager.Instance.ActiveBattleInfo.Group && x.level == UserManager.Instance.ActiveBattleInfo.Level);
    List<Drop> dropList = new List<Drop>();
    Drop one = new Drop();
    one = new Drop();
    one.type = DropType.weapon;
    one.weaponType = (WeaponType)activeLevel.drop1Id;
    one.count = activeLevel.drop1Count;
    dropList.Add(one);
    if (UserManager.Instance.ActiveBattleInfo.Mode == PointMode.farm && UserManager.Instance.ActiveBattleInfo.FarmPointActive >= 2)
    {
      one = new Drop();
      one.type = DropType.weapon;
      one.weaponType = (WeaponType)activeLevel.drop2Id;
      one.count = activeLevel.drop2Count;
      if (dropList.Exists(x => x.weaponType == one.weaponType))
      {
        one.count += dropList.Find(x => x.weaponType == one.weaponType).count;
        dropList.RemoveAll(x => x.weaponType == one.weaponType);
      }
      dropList.Add(one);
    }
    if (UserManager.Instance.ActiveBattleInfo.Mode == PointMode.farm && UserManager.Instance.ActiveBattleInfo.FarmPointActive >= 3)
    {
      one = new Drop();
      one.type = DropType.weapon;
      one.weaponType = (WeaponType)activeLevel.drop3Id;
      one.count = activeLevel.drop3Count;
      if (dropList.Exists(x => x.weaponType == one.weaponType))
      {
        one.count += dropList.Find(x => x.weaponType == one.weaponType).count;
        dropList.RemoveAll(x => x.weaponType == one.weaponType);
      }
      dropList.Add(one);
    }
    one = new Drop();
    one.type = DropType.superCoins;
    one.count = activeLevel.coins;
    dropList.Add(one);
    readyDrop = dropList.ToArray();
    getDrop = dropList.ToArray();
    for (int i = 0; i < getDrop.Length; i++)
      getDrop[i].count = 0;
  }

  void OnEnable()
  {
    PlayerController.hitTarget += Damage;
    HouseController.hitTarget += Damage;
  }

  void OnDisable()
  {
    PlayerController.hitTarget -= Damage;
    HouseController.hitTarget -= Damage;
  }

  void Update()
  {

    if (battlePhase != BattlePhase.battle)
      return;

    // Кат сцена
    if (CatSceneManager.Instance.isSpecLevel)
    {
      if (UserHealth.Instance.Percent <= 0.1f)
      {
        battlePhase = BattlePhase.pause;
        ZbCatScene.CatSceneManager.Instance.ShowCatScene("36_3", () => { });
      }
    }

    timeBattle += Time.deltaTime * speedIncrement;

    if (battleTimer != null)
      battleTimer(timeBattle);
    
    if (bearProtection && Time.time > timeStopBearProtection)
      bearProtection = false;
  }

  private bool _lastPower = false;
  private float _damageLocked;
  public static bool isLastPower { get; set; }

  /// <summary>
  /// Получение повреждения плеером
  /// </summary>
  /// <param name="damage"></param>
  void Damage(float damage)
  {
    if (battlePhase != BattlePhase.battle)
      return;

    // Если стоит эффект из последних сил, то уро не наносим
    if (_damageLocked >= Time.time)
      return;

    UserHealth.Instance.Value -= damage * (bearProtection ? persentProtection : 1);

    // Установка эффекта Из последних сил
    if (UserHealth.Instance.Value <= 0 && !_lastPower)
    {
      _lastPower = true;
      isLastPower = true;
      UserHealth.Instance.Value = 1;
      _damageLocked = Time.time + 5;
      Invoke("DeactiveLastPower", 5);
    }

    if (UserHealth.Instance.Percent <= 0)
    {
      LiveEnd();
    }

  }

  // Отключение эффекта Из последних сил
  void DeactiveLastPower()
  {
    isLastPower = false;
  }

  bool bearProtection;
  float timeStopBearProtection;
  float persentProtection;

  public void ActiveBearProtection(float newPersentProtection, float newTimeProtection)
  {
    bearProtection = true;
    timeStopBearProtection = Time.time + newTimeProtection;
    persentProtection = newPersentProtection;
  }

  #region Финал миссии


  //public void TimeEnd() {
  //	if (battlePhase == BattlePhase.end)
  //		return;
  //	endTime = Time.time;
  //	ChangePhase(BattlePhase.end);
  //	//Time.timeScale = 0;
  //	//Invoke("BattleComplited", 5);
  //	StartCoroutine(StartBatleComplited());
  //}

  public void ZombyComplited()
  {
    endTime = Time.time;
    battlePhase = BattlePhase.end;
    //Time.timeScale = 0;
    //Invoke("BattleComplited", 5);

    BattleEventEffects.Instance.VisualEffect(BattleEffectsType.win, Vector3.zero);
    StartCoroutine(StartBatleComplited());
  }

  public void LiveEnd()
  {

    if (battlePhase == BattlePhase.end)
      return;

    if (UserManager.Instance.ActiveBattleInfo.Mode == PointMode.arena)
    {
      UserHealth.Instance.StartBattle();
      EnemysSpawn.Instance.DeactiveAllEnemy(true);
      return;
    }

    endTime = Time.time;
    battlePhase = BattlePhase.end;

    BattleEventEffects.Instance.VisualEffect(BattleEffectsType.loose, Vector3.zero);

    StartCoroutine(StartBatleFailed());

  }

  IEnumerator StartBatleComplited()
  {
    yield return new WaitForSecondsRealtime(1);
    BattleComplited();
  }
  IEnumerator StartBatleFailed()
  {
    yield return new WaitForSecondsRealtime(1);
    BattleFailed();
  }

  public void BattleComplitedChit()
  {
    battlePhase = BattlePhase.end;
    BattleComplited();
  }

  public void BattleFailedChit()
  {
    battlePhase = BattlePhase.end;
    BattleFailed();
  }

  private void ShowBattleResult(bool isComplited)
  {
    if (isComplited)
      ShowWinDialog();
    else
      ShowLoseDialog();

    //if (menuMusic != null && menuMusic.isPlaing)
    //  menuMusic.Stop(0);

    //BattleResult br = UIController.ShowUi<BattleResult>();
    //br.gameObject.SetActive(true);

    //br.SetData(isComplited ? BattleResult.ResultType.complited : BattleResult.ResultType.loser);
    //br.SetDrop(getDrop);

    //br.OnExit = ExitBattle;

    //br.OnRestart = () =>
    //{
    //  br.gameObject.SetActive(false);
    //  RestartBattle();
    //};

    //br.gameObject.SetActive(true);

  }


  private void ShowWinDialog() {

    WinDialog dialog = UIController.ShowUi<WinDialog>();
    dialog.gameObject.SetActive(true);
    
    dialog.OnExit = ExitBattle;
    dialog.SetDrop(getDrop);
    dialog.OnRestart = () => {
      dialog.gameObject.SetActive(false);
      RestartBattle();
    };
  }

  private void ShowLoseDialog() {

    LoseDialog dialog = UIController.ShowUi<LoseDialog>();
    dialog.gameObject.SetActive(true);

    dialog.OnExit = ExitBattle;
    dialog.SetDrop(getDrop);
    dialog.OnRestart = () => {
      dialog.gameObject.SetActive(false);
      RestartBattle();
    };
  }

  private void BattleEnd(bool isComplited)
  {
     Enemy[] enemies = EnemysSpawn.GetAllEnemy;
        for (int i= 0; i < enemies.Length; i++) {
            enemies[i].attackAudioBlock.audioGroup = null;
            Debug.Log("yyy");
        }
    //Camera.main.gameObject.GetComponent<AudioListener>(). = false
    CalcDrop();
    UserManager.Instance.lastComplited = isComplited;
    AddBearMascotte(isComplited);
    ShowBattleResult(isComplited);
  }

  void CalcDrop()
  {

    UserManager.Instance.silverCoins.Value += goldCoins * 100;

    foreach (WeaponManager wm in WeaponGenerator.Instance.weaponsControllers)
    {
      if (!Game.User.UserWeapon.Instance.ExistWeaponType(wm.weaponType))
        Game.User.UserWeapon.Instance.AddWeapon(wm.weaponType, wm.BulletCount);
    }

    // Добавляем от 30% до 80% от истраченных помидор
    List<Drop> dropArray = new List<Drop>(getDrop);
    if (!dropArray.Exists(x => x.weaponType == WeaponType.tomato))
    {
      Drop drop = new Drop();
      drop.type = DropType.weapon;
      drop.weaponType = WeaponType.tomato;
      drop.count = Mathf.RoundToInt(GetBulletCount(drop.weaponType) * UnityEngine.Random.Range(0.3f, 0.8f));
      if (drop.count > 0)
      {
        dropArray.Add(drop);
        getDrop = dropArray.ToArray();
      }
    } else
    {
      for (int i = 0; i < getDrop.Length; i++)
        if (weaponBullets.Length > i && weaponBullets[i].weaponType == WeaponType.tomato)
          weaponBullets[i].count += Mathf.RoundToInt(GetBulletCount(WeaponType.tomato) * UnityEngine.Random.Range(0.3f, 0.8f));
    }

    if (getDropList != null && getDropList.Count > 0)
      getDrop = getDropList.ToArray();

    for (int i = 0; i < getDrop.Length; i++)
    {
      if (getDrop[i].type == DropType.weapon)
        Game.User.UserWeapon.Instance.AddBullet(getDrop[i].weaponType, getDrop[i].count);
      else
        UserManager.Instance.goldCoins.Value += getDrop[i].count;
    }

    List<Drop> dropArrayNew = new List<Drop>(getDrop);

    Drop silverDrop = new Drop();
    silverDrop.type = DropType.coins;
    silverDrop.count = silverCoins /*- (goldCoins * 100)*/;
    dropArrayNew.Add(silverDrop);

    getDrop = dropArrayNew.ToArray();
  }

  /// <summary>
  /// Успешное выполнение миссии
  /// </summary>
  public void BattleComplited()
  {

    UserManager.Instance.LevelComplited(endTime - startTime);

    if (UserManager.Instance.ActiveBattleInfo.Mode == PointMode.company)
    {
    }
    BattleEnd(true);

  }

  void AddBearMascotte(bool isWinner)
  {

    Dictionary<string, object> dataMascotte = new Dictionary<string, object>();
    dataMascotte["winner"] = isWinner;

    CollectionItemsManager.instance.AddItem(ItemType.bearMascotte, dataMascotte);

  }

  /// <summary>
  /// Миссия пройграна
  /// </summary>
  public void BattleFailed()
  {
    BattleEnd(false);
  }

  #endregion

  #region Перезапуск уровня

  public void RestartBattle()
  {
    SetStart();
    EnemysSpawn.Instance.DeactiveAllEnemy(false, true);
    Time.timeScale = 1;
  }

  public void ExitBattle()
  {
    Time.timeScale = 1;
    if (menuMusic != null)
      menuMusic.Stop(0.5f);
    BattleEvents.OnCloseBattle.Call();
    CloseBlack(() =>
    {
      if (UserManager.Instance.ActiveBattleInfo.Mode == PointMode.survival || UserManager.Instance.ActiveBattleInfo.Mode == PointMode.arena || (UserManager.Instance.ActiveBattleInfo.Group == 1 && UserManager.Instance.ActiveBattleInfo.Level == 1 && !UserManager.Instance.lastComplited))
        SceneManager.LoadScene("Menu");
      else
        SceneManager.LoadScene("Map");
    });
  }

  #endregion
  
  #region Бонусы

  public static void AddBonus(BonusTypes type, float value)
  {
    if (type == BonusTypes.live)
    {
      UserHealth.Instance.Value += value;
    }

  }

  #endregion

  #region Music
  //public AudioPoint backMusic;
  public AudioPoint menuMusic;

  private string[] m_backgroundsAudio = new string[] {
    "BackGrounds/battleSound1"
    ,"BackGrounds/battleSound2"
  };
  private string[] m_backgroundsSurvivalAudio = new string[] {
    "BackGrounds/survival"
  };

  void InitBackMusic()
  {
    if (UserManager.Instance.ActiveBattleInfo.Mode == PointMode.company || UserManager.Instance.ActiveBattleInfo.Mode == PointMode.farm) {
      int soundTrack = 0;
      switch (UserManager.Instance.ActiveBattleInfo.PointNum)
      {
        case 0:
          soundTrack = 0;
          //AudioManager.Instance.PlayBackGroundSound(m_backgroundsAudio[0]);
          //audioSource.Play(this,0);
          //AudioManager.backMusic.Play(audioSource, 0);
      break;
        case 1:
          soundTrack = 1;
          //audioSource.Play(this, 1);
          //audioSource[1].isLocked = true;
          //AudioManager.Instance.PlayBackGroundSound(m_backgroundsAudio[1]);
          //AudioManager.backMusic.Play(audioSource, 1);
          break;
        case 2:
          //audioSource.Play(this, 2);
          //audioSource[2].isLocked = true;
          //AudioManager.backMusic.Play(audioSource, 2);
          //break;
        default:
          //audioSource.PlayRandom(this);
          soundTrack = Random.Range(0, m_backgroundsAudio.Length);
          //audioSource[num].isLocked = true;
          //AudioManager.backMusic.Play(audioSource, num);
          break;
      }
      AudioManager.Instance.PlayBackGroundSound(m_backgroundsAudio[soundTrack]);

    } else if (UserManager.Instance.ActiveBattleInfo.Mode == PointMode.survival) {

      AudioManager.Instance.PlayBackGroundSound(m_backgroundsSurvivalAudio[0]);
      //audioSurvival.Stop();
      //audioSurvival.Play(this, 0);
      //backMusic = AudioManager.PlayEffects(audioSurvival[0], AudioMixerTypes.musicPlay);
    }

  }
  #endregion

  #region Pause

  bool isPause = false;

  public void Pause()
  {

    if (battlePhase == BattlePhase.end)
      return;

    isPause = !isPause;

    if (!CatSceneManager.isActive)
      Time.timeScale = isPause ? 0 : 1;

    ExEvent.BattleEvents.Pause.Call(isPause);

    if (isPause)
      PausePanelShow();
  }

  void PausePanelShow()
  {
    PausePanel instUi = UIController.ShowUi<PausePanel>();
    instUi.gameObject.SetActive(true);

    instUi.OnClose = () =>
    {
      Pause();
      instUi.OnDeactive = () => {

        instUi.gameObject.SetActive(false);
      };
    };

    instUi.OnDeactive = () =>
    {
      //Destroy(instUi);
      //Pause();
    };

    instUi.OnRestart = () =>
    {
      Time.timeScale = 1;
      instUi.OnDeactive = () => { instUi.gameObject.SetActive(false); };

      Pause();
      RestartBattle();
    };

    instUi.OnExit = () =>
    {
      Time.timeScale = 1;
      ExitBattle();
    };

    instUi.OnComplited = () =>
    {
      instUi.gameObject.SetActive(false);
      Time.timeScale = 1;
      BattleComplitedChit();
    };

    instUi.OnFailed = () =>
    {
      instUi.gameObject.SetActive(false);
      Time.timeScale = 1;
      BattleFailedChit();
    };
    instUi.OnDeactive = () => { isPause = false; };
  }

  #endregion

  #region Используемые снарядыы

  public WeaponBullets[] weaponBullets { get; set; }

  /// <summary>
  /// Инкремент используемого оружия
  /// </summary>
  /// <param name="weaponType">Тип оружия</param>
  /// <param name="count">Количество используемых снарядов</param>
  public void AddUseWeapon(WeaponType weaponType, int count = 1)
  {

    for (int i = 0; i < weaponBullets.Length; i++)
      if (weaponBullets[i].weaponType == weaponType)
        weaponBullets[i].count += count;

  }

  int GetBulletCount(WeaponType weaponType)
  {

    for (int i = 0; i < weaponBullets.Length; i++)
      if (weaponBullets[i].weaponType == weaponType)
        return weaponBullets[i].count;
    return 0;

  }

  public void ClearBulletCount()
  {

    if (weaponBullets == null)
      return;

    for (int i = 0; i < weaponBullets.Length; i++)
      weaponBullets[i].count = 0;

  }

  #endregion

  private void OpenBlack(Action OnComplited)
  {
    FillBlack inst = UIController.ShowUi<FillBlack>();
    inst.gameObject.SetActive(true);
    inst.PlayAnim(FillBlack.AnimType.full, FillBlack.AnimVecor.open, () =>
    {
      if (OnComplited != null)
        OnComplited();
      inst.gameObject.SetActive(false);
    }
    );

  }


  private void CloseBlack(Action OnComplited)
  {
    FillBlack inst = UIController.ShowUi<FillBlack>();
    inst.PlayAnim(FillBlack.AnimType.full, FillBlack.AnimVecor.close, OnComplited);
    inst.gameObject.SetActive(true);

  }


}
