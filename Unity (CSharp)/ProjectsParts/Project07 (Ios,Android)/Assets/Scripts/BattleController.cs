using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleController: Singleton<BattleController> {

  public GamePlay panel;
  public SpriteRenderer backSprite;

  public int activeState;
  public bool isBoss;

  public bool adsReady;

  [System.Flags]
  public enum BattlePhase {
    none = 0,
    battle = 1,
    loss = 2,
    win = 4,
    end = 6
  }

  private BattlePhase _battlePhase;
  public BattlePhase battlePhase {
    get {
      return _battlePhase;
    }
    set {
      if (_battlePhase == value) return;

      BattlePhase oldPhase = _battlePhase;

      _battlePhase = value;
      ExEvent.BattleEvents.BattlePhaseChange.CallAsync(_battlePhase, oldPhase);

    }
  }

  protected override void Awake() {
    base.Awake();
    isBoss = false;

    battlePhase = BattlePhase.none;
  }

  // Use this for initialization
  void Start() {
    OpenGameUi();
    backSprite.sprite = UserManager.Instance.ActiveLocation.BackGround;
    StartCoroutine(StartBattleCor());
  }

  IEnumerator StartBattleCor() {
    StartBattle();

    yield return new WaitForSeconds(0.3f);

    Tutorial.TutorialManager.Instance.Show(Tutorial.Type.intro);
    Tutorial.TutorialManager.Instance.Show(Tutorial.Type.tap);

  }

  public void OpenGameUi() {
    panel = UiController.GetUi<GamePlay>();
    panel.gameObject.SetActive(true);
    panel.onSetting = OpenSettingDialog;
  }

  void OpenSettingDialog() {
    SettingDialog panel = UiController.GetUi<SettingDialog>();
    panel.Show();
  }

  private void BattleComplete() {
    Debug.Log("BattleComplete");
    UserManager.Instance.ActiveLocation.Complete();

    QuestionManager.Instance.AddValueQuest(QuestionManager.Type.completeLocation, 1);
    UserManager.Instance.Gold += UserManager.Instance.ActiveLocation.goldWin;

    if(UserManager.Instance.ActiveLocation.Level != 0)
    AppMetrica.Instance.ReportEvent("lvl"+ (UserManager.Instance.ActiveLocation.Level +1)+ "_end");

    Location loc = LocationManager.Instance.GetNextLocation(UserManager.Instance.ActiveLocation);
    bool incBlock = loc == null ? UserManager.Instance.IncrementBlock() : false;
    

    ShowWinDialog(() => {
      if (!incBlock)
      {
        LoadMap();
        return;
      }

      ShowBlockCompleteDialog(LoadMap);

    }, () => {

      if (loc != null) {
        ShowNextLocationDialog(loc);
      } else {

        if (!incBlock) {
          LoadMap();
          return;
        }

        ShowBlockCompleteDialog(LoadMap);
      }

    });

  }

  private void ShowNextLocationDialog(Location loc) {
    MapDialog dialog = UiController.GetUi<MapDialog>();
    dialog.Show();
    dialog.SetData(loc);
    dialog.OnNext = () => {
      BlackRound.Instance.Play(() => {
        UserManager.Instance.ActiveLocation = loc;
        backSprite.sprite = UserManager.Instance.ActiveLocation.BackGround;
        StartBattle();
      });
      dialog.CloseButton();
    };

    dialog.OnBack = LoadMap;

  }

  private void ShowWinDialog(System.Action onBase, System.Action onNext)
  {
    WinDialog dialog = UiController.GetUi<WinDialog>();
    dialog.Show();
    dialog.OnBase = () => {
      onBase?.Invoke();
    };
    dialog.OnNext = () => {
      dialog.Hide();
      onNext?.Invoke();
    };
  }

  private void LoadMap() {
    GameManager.Instance.LoadScene("Map");
  }

  private void ShowBlockCompleteDialog(System.Action onNext)
  {
    BlockCompleteDialog dialog = UiController.GetUi<BlockCompleteDialog>();
    dialog.Show();
    dialog.OnNext = onNext;
  }

  [ExEvent.ExEventHandler(typeof(ExEvent.UserEvents.UserDied))]
  public void UserDied(ExEvent.UserEvents.UserDied eventData) {
    battlePhase = BattlePhase.loss;
    BattleLoss();
  }

  private void BattleLoss() {
    LoseDialog dialog = UiController.GetUi<LoseDialog>();
    dialog.Show();

    dialog.OnBase = () => {
      GameManager.Instance.LoadScene("Base");
    };
    dialog.OnRepeat = () => {

      BlackRound.Instance.Play(() => {
        dialog.Hide();
        StartBattle();
      });


    };
    dialog.OnVideoAds = () => {

      UnityAdsVideo.Instance.PlayVideo((res) => {

        if (res) {
          dialog.Hide();
          ContinueBattle();
        } else {
          GameManager.Instance.LoadScene("Base");
        }

      });

    };

  }

  private void StartBattle() {
    //activeLevel = 0;
    activeState = -1;
    adsReady = true;
    ExEvent.BattleEvents.BattleStart.Call();
    battlePhase = BattlePhase.battle;

    NextWave(UserManager.Instance.ActiveLocation.stageCount <= 0 && UserManager.Instance.ActiveLocation.bossExists);

  }

  private void ContinueBattle() {

    UserManager.Instance.UseMedBonus(UserManager.Instance.maxLive / 2, true);

    battlePhase = BattlePhase.battle;
  }

  public void WaveComplete() {

    if (activeState < UserManager.Instance.ActiveLocation.stageCount - 1) {
      NextWave();

      if (UserManager.Instance.ActiveLocation.Level == 1 && activeState == 5)
        Tutorial.TutorialManager.Instance.Show(Tutorial.Type.weapon1);
      if (UserManager.Instance.ActiveLocation.Level == 101 && activeState == UserManager.Instance.ActiveLocation.stageCount/2)
      //if (UserManager.Instance.ActiveLocation.Block == 0 && UserManager.Instance.ActiveLocation.Level == 0 && activeState == 1)
        Tutorial.TutorialManager.Instance.Show(Tutorial.Type.weapon2);

      return;
    } else if (isBoss) {
      BattleComplete();
      return;
    } else if (UserManager.Instance.ActiveLocation.bossExists) {
      isBoss = true;
      NextWave(true);
      return;
    }

    if (Tutorial.TutorialManager.Instance.IsActive) {
      Tutorial.TutorialManager.Instance.Show(Tutorial.Type.complete, () => {
        //Tutorial.TutorialManager.Instance.Finished();
        BattleComplete();
      });
    } else {
      BattleComplete();
    }
    return;
  }

  [ContextMenu("Force complete")]
  public void ForceComplete()
  {
    activeState = UserManager.Instance.ActiveLocation.stageCount;
    EnemySpawner.Instance.DieAllForceAll();
  }

  private void NextWave(bool isBoss = false) {
    if (!isBoss)
      activeState++;

    GameManager.Instance.PlayBackGroundSound("BackGround/" + UserManager.Instance.ActiveLocation.backgroundClipName);
    
    if (isBoss)
    {
      this.isBoss = true;
      //UserManager.Instance.ActiveLocation.audioBackGroundBoss.PlayBackGround();
    } else {
      //UserManager.Instance.ActiveLocation.audioBackGroundLevels.PlayBackGround();
    }

    ExEvent.BattleEvents.NextWave.Call(isBoss, activeState, UserManager.Instance.ActiveLocation.stageCount);
  }


}
