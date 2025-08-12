/// <summary>
/// Контроллер пета летучей мыши
/// </summary>

using UnityEngine;
using System.Collections;
using Spine.Unity;
using Spawner = Road.Spawners.Pets.PetSpawner;
using Player.Jack;

public class PetBatController: MonoBehaviour {

  public SkeletonAnimation skeletonAnimation;                     // Кости при использовании

  //public GameObject activateParticle;

  public float timeWork;                                          // Время работ
  float timeStartWork;                                            // Время начало взаимодействия с Джеком
  int perkLevel;                                                  // Уровень прокачки

  public GameObject shadowObject;                                 // Объект тени
  GameObject shadowInstance;                                      // Экземпляр тени

  public float horisontalSpeed;                                   // Скорость горизонтального полета
  public float jumpSpeed;                                         // Скорость прыжка

  public GameObject frontWind;
  public GameObject backWind;

  GameObject playerObject;
  [SerializeField] [SpineBone] string contactJackSlot;            // Слот контакта с джеком

  string currentAnimation;                                        // Текущее состояние анимации
  [SpineAnimation(dataField: "skeletonAnimation")]
  public string flyAnim = "";
  [SpineAnimation(dataField: "skeletonAnimation")]
  public string flyJackAnim = "";
  [SpineAnimation(dataField: "skeletonAnimation")]
  public string shopIdleAnim = "";

  enum PetBatPhase { none, uses, free, shop };

  [SerializeField] PetBatPhase petBatPhase;

  bool enableNow;                                                             // Тоько что инициализировался

  Vector3 velocity;                                                           // Движение
  public float gravity;                                                       // Гравитация 

  float mobileMoveHorizontal;                                                 // Значение мобильного контроллера, отвечающего за горизонтальное смещение
  bool jump;
  bool canJump;

  RunnerPhase runnerPhase;

  public AudioClip touchClip;

  void OnEnable() {
    RunnerController.OnChangeRunnerPhase += ChangeState;
    runnerPhase = RunnerController.Instance.runnerPhase;

    enableNow = true;
    currentAnimation = "";
    if (petBatPhase == PetBatPhase.shop) {
      ResetAnimation(skeletonAnimation);
      return;
    }

    petBatPhase = PetBatPhase.none;
    frontWind.SetActive(true);
    backWind.SetActive(true);
    skeletonAnimation.gameObject.SetActive(true);
    GetComponent<CapsuleCollider>().enabled = true;
    GetComponent<CapsuleCollider>().isTrigger = true;
    canJump = true;
    ShadowInit();
    SetAnimation(skeletonAnimation, flyAnim, true);

    perkLevel = Spawner.levelBat;
  }

  void OnDisable() {
    RunnerController.OnChangeRunnerPhase -= ChangeState;
    ShadowDestroy();
  }

  void LateUpdate() {
    if (petBatPhase == PetBatPhase.shop) {
      SetAnimation(skeletonAnimation, shopIdleAnim, true);
      enableNow = false;
      return;
    }

    if (enableNow) {
      SetAnimation(skeletonAnimation, flyAnim, true);
      enableNow = false;
    }
  }

  void Update() {
    // Анимация в магазине
    if (petBatPhase == PetBatPhase.shop) {
      SetAnimation(skeletonAnimation, shopIdleAnim, true);
      return;
    }

    if (petBatPhase == PetBatPhase.uses)
      JackControl();
    else
      Fly();

    if (transform.position.y > CameraController.displayDiff.topDif(2) || transform.position.y < CameraController.displayDiff.topDif(-2)
        || transform.position.x > CameraController.displayDiff.rightDif(2) || transform.position.x < CameraController.displayDiff.rightDif(-2)) {
      if (petBatPhase == PetBatPhase.uses)
        RunnerController.petActivate(PetsTypes.none, false);
      gameObject.SetActive(false);
    }

    if (petBatPhase == PetBatPhase.uses && timeStartWork + timeWork + (perkLevel * 3) < Time.time) PetDisconnect();
  }
  /// <summary>
  /// Изменение фазы
  /// </summary>
  /// <param name="state"></param>
  void ChangeState(RunnerPhase state) {
    runnerPhase = state;
  }

  /// <summary>
  /// Отключение пета
  /// </summary>
  void PetDisconnect() {
    playerObject.GetComponent<PlayerPets>().PetDisconnect();
  }

  /// <summary>
  /// Свободный полет
  /// </summary>
  void Fly() {
    //velocity.x += RunnerController.RunSpeed;

    if (petBatPhase == PetBatPhase.free) {
      velocity.x = RunnerController.RunSpeed * 2;
      velocity.y = 2;
    } else {
      velocity.x = RunnerController.RunSpeed * 0.2f;
      velocity.y = 0;
    }

    if (lastJump + 0.5f < Time.time) {
      Jump();
      lastJump = Time.time;
    }

    transform.position += velocity * Time.deltaTime;
  }

  float lastJump;

  /// <summary>
  /// Обработка прыжка
  /// </summary>
  void Jump() {

    frontWind.GetComponent<Animator>().SetTrigger("jump");
    backWind.GetComponent<Animator>().SetTrigger("jump");
  }

  /// <summary>
  /// Джек активирует мышь
  /// </summary>
  public void JackActivate(GameObject jack) {
    if (petBatPhase == PetBatPhase.none) {
      AudioManager.PlayEffect(catchClip, AudioMixerTypes.runnerEffect);
      timeStartWork = Time.time;
      petBatPhase = PetBatPhase.uses;
      playerObject = jack;
      skeletonAnimation.gameObject.SetActive(true);
      SetAnimation(skeletonAnimation, flyJackAnim, true);
      //jack.GetComponent<PlayerPets>().EnableBat(this, skeletonAnimation, contactJackSlot, timeWork + (perkLevel * 3));
      Spawner.Instance.ActivateParticles(transform.position + Vector3.up);
      AudioManager.PlayEffect(touchClip, AudioMixerTypes.runnerEffect);
    }

    //if (petBatPhase == PetBatPhase.shop)
    //    jack.GetComponent<PlayerPets>().EnableBat(this, skeletonAnimation, contactJackSlot, timeWork + (perkLevel * 3));
  }

  public void JackDisActive(bool damage = false) {
    if (petBatPhase == PetBatPhase.uses) {
      Spawner.Instance.ActivateParticles(transform.position + Vector3.up);
      AudioManager.PlayEffect(touchClip, AudioMixerTypes.runnerEffect);
      SetAnimation(skeletonAnimation, flyAnim, true);
      if (damage) AudioManager.PlayEffect(hitClip, AudioMixerTypes.runnerEffect);
      petBatPhase = PetBatPhase.free;
    }
  }

  /// <summary>
  /// Состояние когда управляется джеком
  /// </summary>
  void JackControl() {
    velocity.x = mobileMoveHorizontal * horisontalSpeed;

    // Ограничения горизонтального перемещения
    if ((transform.position.x < CameraController.displayDiff.transform.position.x + CameraController.displayDiff.left * 0.8f && velocity.x < 0)
        || (transform.position.x > CameraController.displayDiff.transform.position.x + CameraController.displayDiff.right * 0.8f && velocity.x > 0))
      velocity.x = 0;

    velocity.x += RunnerController.RunSpeed;

    velocity.y -= gravity * Time.deltaTime;

    if (canJump && jump) {
      velocity.y = jumpSpeed;
      canJump = false;
      frontWind.GetComponent<Animator>().SetTrigger("jump");
      backWind.GetComponent<Animator>().SetTrigger("jump");
      AudioManager.PlayEffect(windClip[Random.Range(0, windClip.Length)], AudioMixerTypes.runnerEffect);
    }

    if (transform.position.y > CameraController.displayDiff.topDif(0.9f) && velocity.y > 0) velocity.y = 0;
    if (transform.position.y < RunnerController.Instance.mapHeight + 1 && velocity.y < 0) velocity.y = 0;

    transform.position += velocity * Time.deltaTime;
  }

  #region Реакция на кнопки

  public void Movement(float mov) {
    mobileMoveHorizontal = mov;
  }

  public void CanJump(bool flag) {
    if (flag)
      jump = true;
    else {
      canJump = true;
      jump = false;
    }
  }

  #endregion

  void OnTriggerEnter2D(Collider2D col) {
    if (col.gameObject.tag == "Player" && petBatPhase == PetBatPhase.none && runnerPhase == RunnerPhase.run) JackActivate(col.gameObject);

    if (petBatPhase == PetBatPhase.uses) {
      // При контакте с монетой, добавляем
      if (col.tag == "Coins") {
        col.GetComponent<Coin>().AddCouns();
      }

    }

  }


  #region Animation

  /// <summary>
  /// Применение основной анимации
  /// </summary>
  /// <param name="spine">Кости</param>
  /// <param name="anim">Aнимация</param>
  /// <param name="loop">Зацикливание</param>
  public void SetAnimation(SkeletonAnimation spine, string anim, bool loop) {
    if (!gameObject) return; // Вероятно исправляет ошибку вылета

    if (currentAnimation != anim) {
      spine.state.SetAnimation(0, anim, loop);
      currentAnimation = anim;
    }
  }


  /* ***************************
   * Резет анимации
   * ***************************/
  public void ResetAnimation(SkeletonAnimation spine) {
    spine.Initialize(true);
    skeletonAnimation.state.Event += AnimEvent;
    currentAnimation = null;
  }

  void AnimEvent(Spine.AnimationState state, int trackIndex, Spine.Event e) {
    frontWind.GetComponent<Animator>().SetTrigger("jump");
    backWind.GetComponent<Animator>().SetTrigger("jump");

    if (petBatPhase != PetBatPhase.shop) {
      AudioManager.PlayEffect(windClip[Random.Range(0, windClip.Length)], AudioMixerTypes.runnerEffect);
    }
  }

  /* ***************************
   * Добавленная анимация
   * ***************************/
  public void AddAnimation(SkeletonAnimation spine, int index, string animName, bool loop, float delay) {
    spine.state.AddAnimation(index, animName, loop, delay);
  }

  #endregion


  #region Тень

  void ShadowInit() {
    shadowInstance = Instantiate(shadowObject, transform.position, Quaternion.identity) as GameObject;
    shadowInstance.GetComponent<ShadowBehaviour>().matherObject = gameObject.transform;
    shadowInstance.GetComponent<ShadowBehaviour>().diff = 0.3f;
    shadowInstance.transform.parent = transform;
  }

  void ShadowDestroy() {
    if (shadowInstance) Destroy(shadowInstance);
  }

  IEnumerator SetShadowDead() {
    yield return new WaitForSeconds(0.1f);
    if (shadowInstance)
      shadowInstance.GetComponent<ShadowBehaviour>().SetDiff(new Vector3(0.5f, 0, 0), new Vector3(0.3f, 0, 0));
    yield return 0;
  }

  public void ShadowFixed(bool flag) {
    if (shadowInstance)
      shadowInstance.GetComponent<ShadowBehaviour>().fixedsize = flag;
  }

  public void ShadowSetDiff(Vector3 newPos, Vector3 newScale) {
    if (shadowInstance)
      shadowInstance.GetComponent<ShadowBehaviour>().SetDiff(newPos, newScale);
  }

  public void ShadowSetDeff() {
    if (shadowInstance)
      shadowInstance.GetComponent<ShadowBehaviour>().SetDeff();
  }

  public void ShadowShow(bool flag) {
    if (shadowInstance) shadowInstance.SetActive(flag);
  }
  #endregion


  #region Звуки

  public AudioClip catchClip;
  public AudioClip[] windClip;
  public AudioClip hitClip;

  #endregion

}
