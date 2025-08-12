using Player.Jack;
using System.Collections;
using UnityEngine;

/// <summary>
/// Стартовый объект на сцене
/// </summary>
public class StartDecor: Singleton<StartDecor> {

  public enum AnimationType { none, destroyHome, boss }       // Тип стартовой анимации
  public AnimationType animationType;                         // Текущая используемая анимация

  private readonly string _waitIdle = "MainScreen_IdolStart";
  private readonly string _getCristal = "MainScreen_Steal";
  private readonly string _idlewishCristal = "MainScreen_IdolEnd";
  public GameObject cristal;
  public BoostType activeBoots;
  public ParticleSystem[] DustParticles;
  public GameObject stonesObject;
  public PlatformDecor[] platforms;
  private readonly bool cotonineStart;
  public float timeWaitStart;
  public GameObject homeObject;
  public GameObject logoObject;
  private PlayerMove playerPlay;

  private bool down;
  private float decs;
  private bool act;

  private int animPhases;
  private int toPlatformStep;
  private bool playerMuve;
  public AudioClip startClip;
  private Vector3 startPlayerPosition;

  /// <summary>
  /// Инициализация стартового объектв при классическом забеге
  /// </summary>
  public void StartClassicRun() {
    if (logoObject != null)
      logoObject.SetActive(false);
    playerPlay = PlayerController.Instance.GetComponent<PlayerMove>();
    startPlayerPosition = transform.position;

    animPhases = 0;
    PlayerController.Instance.animation.SetAnimation(_waitIdle, true);
    PlayerController.Instance.animation.onAnimEvent += AnimEvent;
    PlayerController.Instance.GraphicVector(false);
    PlayerController.Instance.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
    act = true;
  }

  /// <summary>
  /// Реакция на тап по экрану
  /// </summary>
  //public void OnTapScreen() {

  //  if (!UiController.checkStart())
  //    return;
  //  if (runner.runnerPhase != RunnerPhase.start)
  //    return;

  //  if (runner.activeLevel == ActiveLevelType.classic) {
  //	if(GameManager.gameMove == GameMode.survival)
  //		StartClassicGame(true);
  //	else
  //		StartClassicGame(false);
  //  }
  //  if (runner.activeLevel == ActiveLevelType.ship)
  //    RunnerController.instance.StartRun();

  //}

  /// <summary>
  /// Запуск данных
  /// </summary>
  /// <param name="level"></param>
  public void StartLevelFromDialog(int level) {
    //RunnerController.instance.ForceStart();
    if (!Company.Live.LiveCompany.Instance.isReady) return;
    StartClassicGame();
  }

  [ExEvent.ExEventHandler(typeof(ExEvent.RunEvents.StartFirstRunAnim))]
  public void StartAnim(ExEvent.RunEvents.StartFirstRunAnim e) {
    if (RunnerController.Instance.activeLevel == ActiveLevelType.classic) {
      if (GameManager.activeLevelData.gameMode == GameMode.survival)
        StartClassicGame(true);
      else
        StartClassicGame(false);
    }
    if (RunnerController.Instance.activeLevel == ActiveLevelType.ship)
      RunnerController.Instance.StartRun();

  }

  /// <summary>
  /// Запуск анимации
  /// </summary>
  public void StartClassicGame(bool isForce = true) {

    if (!isForce) {
      //GameObject levelDialog = UiController.DialogLevelOpen();
      //levelDialog.GetComponent<LevelInfo>().SetLevel(GameManager.activeLevel);
      //levelDialog.GetComponent<LevelInfo>().OnForvardButton = StartLevelFromDialog;
      //return;
    }

    //if(GameManager.activeLevelData.gameMode != GameMode.survival)
      PlayAnim();

    //if(!PlayerManager.instance.energy.isEnergy) {
    //	//UiController.instance.EnergyPanelShow(true);
    //	return;
    //}
  }

  /// <summary>
  /// Ссылка на созданный экземпляр врага
  /// </summary>
  private GameObject enemy;

  private readonly bool isGroungs;

  void Update() {

    //if (animationType == AnimationType.destroyHome)
    //  UpdateDestroyHome();

    if (transform.position.x <= CameraController.displayDiff.leftDif(3f)) {
      gameObject.SetActive(false);
    }
  }

  /// <summary>
  /// Обработка апдейта при анимации destroyHome
  /// </summary>
  void UpdateDestroyHome() {
    if (down && homeObject.transform.position.y > 2f && toPlatformStep == 0) {
      decs += 0.021f * Time.deltaTime;
      homeObject.transform.position = new Vector3(homeObject.transform.position.x, homeObject.transform.position.y - decs, homeObject.transform.position.z);
      PlayerController.Instance.transform.position = new Vector3(PlayerController.Instance.transform.position.x, PlayerController.Instance.transform.position.y - decs, PlayerController.Instance.transform.position.z);

      for (int i = 0; i < platforms.Length; i++) {
        if (platforms[i].transform.parent == PlatformSpawn.Instance.transform && platforms[i].transform.position.x <= CameraController.displayDiff.rightDif(0.1f) && platforms[i].gameObject.activeInHierarchy)
          platforms[i].transform.position = new Vector3(platforms[i].transform.position.x, platforms[i].transform.position.y - decs, platforms[i].transform.position.z);
      }
    } else if (down && act) {
      act = false;
      toPlatformStep = 1;
      playerMuve = true;

      PlayerController.Instance.SetStartParam();
      playerPlay.rigidbody.AddForce(new Vector2(6, 16), ForceMode2D.Impulse);
      PlayerController.Instance.GraphicVector(true);


      Invoke(ActiveBoost, 0.1f);
    }

    if (toPlatformStep == 1) {

      //if (velocity.y < 0) {
      //  toPlatformStep = 3;
      //}
    }

    if (playerMuve) {

      if (PlayerController.Instance.isGround && PlayerController.Instance.rigidbody.velocity.y <= 0) {
        PlayerController.Instance.SerRunParam();
        playerMuve = false;
        RunnerController.Instance.StartRun();
        PlayerController.Instance.move.rigidbody.velocity = Vector2.zero;
        if (activeBoots != BoostType.none)
          RunnerController.Instance.StartBoost();
        else {
          if (TutorialManager.Instance.Istutorial) {
            enemy = EnemySpawner.GenerateTutor(EnemyTypes.aztec);
            enemy.SetActive(true);
            enemy.transform.position = new Vector3(startPlayerPosition.x - 3, 0, 0);
            enemy.GetComponent<EnemyMove>().SetFirstRigidbody();
            enemy.GetComponent<EnemyMove>().rb.AddForce(new Vector2(6, 16), ForceMode2D.Impulse);
            enemy.GetComponent<EnemyMove>().thisJump = true;
            //velocity.y = 30;
            //      velocity.x = 6f;
          }
        }
      }
    }

    if (enemy != null) {
      //velocity.y -= 20 * Time.deltaTime;
      //enemy.transform.position += velocity * Time.deltaTime;

      if (enemy.GetComponent<Enemy>().isGround) {
        enemy.GetComponent<EnemyMove>().SetClassicRigidbody();
        enemy = null;
      }

    }

    if (toPlatformStep == 1 || toPlatformStep == 3) {
      if (homeObject.transform.position.y > -1f) {
        decs += 0.04f * Time.deltaTime;
        homeObject.transform.position = new Vector3(homeObject.transform.position.x, homeObject.transform.position.y - decs, homeObject.transform.position.z);

        if (homeObject.transform.position.y <= -0.8f)
          CameraController.Instance.GetComponent<Animator>().SetBool("pirats", false);
      }
    }
  }

  /// <summary>
  /// Запуск стартовой анимации
  /// </summary>
  void PlayAnim() {

    AudioManager.PlayEffect(startClip, AudioMixerTypes.runnerEffect);

    platforms = LevelPooler.Instance.GetTransformParent(PlatformSpawn.Instance.POOL_KEY).GetComponentsInChildren<PlatformDecor>();

    if (animationType == AnimationType.destroyHome)
      GetAnimationAnim();

    if (animationType == AnimationType.boss)
      NextPhasesBoss();
  }

  void ActiveBoost() {
    RunnerController.Instance.booster = activeBoots;
    PlayerController.Instance.animation.ResetAnimation();
    if (activeBoots == BoostType.none || activeBoots == BoostType.speed)
      PlayerController.Instance.GetGemEnd();

    PlayerController.Instance.GetComponent<PlayerBoosters>().SetBoost();
  }


  public void StartRun(BoostType boostType, bool force = false) {
    //GameManager.actionProcess = true;
    activeBoots = boostType;
    PlayAnim();
    StartClassicGame();
  }

  public void SetActiveBoost(BoostType boostType) {
    activeBoots = boostType;
  }

  /// <summary>
  /// Джек хватает кристалл
  /// </summary>
  private void GetAnimationAnim() {
    animPhases = 1;
    CameraController.Instance.Zoom(false);
    Debug.Log("GetAnimationAnim");
    GetComponent<Animator>().SetTrigger("cristall");
    GetComponent<Animator>().SetTrigger("tapIdleEnd");
    PlayerController.Instance.animation.SetAnimation(_getCristal, false);
    Invoke(Earthquake, 1f);
  }

  private void Earthquake() {
    animPhases = 2;
    CameraController.Instance.GetComponent<Animator>().SetBool("pirats", true);
    Invoke(StartDownObjectMove, 0.3f);
  }

  private void StartDownObjectMove() {
    animPhases = 3;
    stonesObject.SetActive(true);
    decs = 0;
    down = true;
    StartCoroutine(DecorMoveDown());
  }

  private IEnumerator DecorMoveDown() {

    while (homeObject.transform.position.y > 0f) {
      decs += 0.021f * Time.deltaTime;
      homeObject.transform.position = new Vector3(homeObject.transform.position.x, homeObject.transform.position.y - decs, homeObject.transform.position.z);
      PlayerController.Instance.transform.position = new Vector3(PlayerController.Instance.transform.position.x, PlayerController.Instance.transform.position.y - decs, PlayerController.Instance.transform.position.z);

      for (int i = 0; i < platforms.Length; i++) {
        if (platforms[i].transform.position.x <= PlayerController.Instance.transform.position.x + 5 && platforms[i].gameObject.activeInHierarchy)
          platforms[i].transform.position = new Vector3(platforms[i].transform.position.x, platforms[i].transform.position.y - decs, platforms[i].transform.position.z);
      }
      yield return null;
    }

    PlayerController.Instance.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
    PlayerController.Instance.SetStartParam();
    PlayerController.Instance.move.rigidbody.AddForce(new Vector2(6, 16), ForceMode2D.Impulse);
    PlayerController.Instance.GraphicVector(true);

    Invoke(ActiveBoost, 0.1f);

    while (!PlayerController.Instance.isGround || PlayerController.Instance.rigidbody.velocity.y > 0)
      yield return null;

    PlayerController.Instance.SerRunParam();
    playerMuve = false;
    RunnerController.Instance.PlayRun(activeBoots);
    //if (activeBoots != BoostType.none)
    //  RunnerController.Instance.StartBoost();
    //else {
    if (TutorialManager.Instance.Istutorial) {
      //enemy = EnemySpawner.GenerateTutor(EnemyTypes.aztec);
      //enemy.SetActive(true);
      //enemy.transform.position = new Vector3(startPlayerPosition.x - 3, 0, 0);
      //enemy.GetComponent<EnemyMove>().SetFirstRigidbody();
      //enemy.GetComponent<EnemyMove>().rb.AddForce(new Vector2(6, 16), ForceMode2D.Impulse);
      //enemy.GetComponent<EnemyMove>().thisJump = true;
      //velocity.y = 30;
      //      velocity.x = 6f;
    }
    //}

  }

  /// <summary>
  /// Обработка изменения фаз
  /// </summary>
  void NextPhasesDestroyHome() {
    if (animPhases == 0) {
      Debug.Log("NextPhasesDestroyHome");
      animPhases = 1;
      GetComponent<Animator>().SetTrigger("cristall");
      GetComponent<Animator>().SetTrigger("tapIdleEnd");
      PlayerController.Instance.animation.SetAnimation(_getCristal, false);
      Invoke(NextPhasesDestroyHome, 1f);
      return;
    }

    if (animPhases == 1) {
      animPhases = 2;
      CameraController.Instance.GetComponent<Animator>().SetBool("pirats", true);
      Invoke(NextPhasesDestroyHome, 0.3f);
      return;
    }

    if (animPhases == 2) {
      animPhases = 3;
      stonesObject.SetActive(true);
      decs = 0;
      down = true;
      return;
    }

  }

  /// <summary>
  /// Изменение анимации при включенной анимации босса
  /// </summary>
  void NextPhasesBoss() {

    if (animPhases == 0) {
      Debug.Log("NextPhasesBoss");
      animPhases = 1;
      GetComponent<Animator>().SetTrigger("cristall");
      GetComponent<Animator>().SetTrigger("tapIdleEnd");
      PlayerController.Instance.animation.SetAnimation(_getCristal, false);
      Invoke(NextPhasesBoss, 1f);
      return;
    }

    if (animPhases == 1) {
      animPhases = 2;
      CameraController.Instance.GetComponent<Animator>().SetBool("pirats", true);
      //MapGenerator.instance.GenerateEnemyBoss(0);
      //Invoke("NextPhasesDestroyHome" , 0.3f);
      return;
    }

  }

  public void StartBossPlay() {
    PlayerController.Instance.GraphicVector(true);
    CameraController.Instance.GetComponent<Animator>().SetBool("pirats", false);
  }

  void AnimEvent(Spine.AnimationState state, int trackIndex, Spine.Event e) {
    cristal.SetActive(false);
    foreach (ParticleSystem part in DustParticles)
      part.Play();
  }


}
