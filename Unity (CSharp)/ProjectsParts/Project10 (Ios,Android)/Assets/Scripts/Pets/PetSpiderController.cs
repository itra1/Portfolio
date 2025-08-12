using UnityEngine;
using System.Collections;
using Spine.Unity;
using Spawner = Road.Spawners.Pets.PetSpawner;
using Player.Jack;

public class PetSpiderController : MonoBehaviour {

  public SkeletonAnimation skeletonAnimation;                     // Кости при использовании
  public GameObject shadowObject;                                 // Объект тени
  GameObject shadowInstance;                                      // Экземпляр тени
  [SerializeField]
  Transform graphic;                                      // Родитель графики

  //public GameObject activateParticle;

  public float timeWork;                                          // Время работ
  float timeStartWork;                                            // Время начало взаимодействия с Джеком
  int perkLevel;                                                  // Уровень прокачки

  public float horisontalSpeed;                                   // Скорость горизонтального полета
  public float jumpSpeed;                                         // Скорость прыжка

  GameObject playerObject;
  [SerializeField]
  [SpineBone]
  string contactJackSlot;            // Слот контакта с джеком

  string currentAnimation;                                        // Текущее состояние анимации
  [SpineAnimation(dataField: "skeletonAnimation")]
  public string runSlowAnim = "";
  [SpineAnimation(dataField: "skeletonAnimation")]
  public string runFastAnim = "";
  [SpineAnimation(dataField: "skeletonAnimation")]
  public string shopIdleAnim = "";

  bool enableNow;                                                             // Тоько что инициализировался

  Vector3 velocity;                                                           // Движение
  public float gravity;                                                       // Гравитация 

  float mobileMoveHorizontal;                                                 // Значение мобильного контроллера, отвечающего за горизонтальное смещение
  bool jump;
  bool canJump;


  bool jumpNow;                                                   // ВЫполнить прыжок
  public bool thisJump;
  float jumpDist;                                                 // Дистанция прыжка
  float barrierWait;

  enum PetSpiderPhase { none, uses, free, shop };
  [SerializeField]
  PetSpiderPhase petSpiderPhase;

  enum SpiderPosition { down, top };
  [SerializeField]
  SpiderPosition spiderPosition;

  float heightDistRevert;
  RunnerPhase runnerPhase;

  public AudioClip touchClip;

  void OnEnable() {
    RunnerController.OnChangeRunnerPhase += ChangeState;
    runnerPhase = RunnerController.Instance.runnerPhase;

		InitAudio();
    enableNow = true;
    GetComponent<CapsuleCollider>().enabled = true;
    GetComponent<CapsuleCollider>().isTrigger = true;
    if(petSpiderPhase != PetSpiderPhase.shop) petSpiderPhase = PetSpiderPhase.none;
    canJump = true;
    ShadowInit();
    spiderPosition = SpiderPosition.down;

    heightDistRevert = (CameraController.displayDiff.top * 2) / 3;
    perkLevel = Spawner.levelSpider;
  }

  void OnDisable() {

    RunnerController.OnChangeRunnerPhase -= ChangeState;
    ShadowDestroy();
  }

  void LateUpdate() {
    if(petSpiderPhase == PetSpiderPhase.shop) {
      SetAnimation(shopIdleAnim, true);
      enableNow = false;
      if(audioComp.isPlaying) audioComp.Pause();
      return;
    }

    if(enableNow) {
      SetAnimation(runSlowAnim, true);
      enableNow = false;
    }
  }

  public LayerMask groundMaskDown;
  public LayerMask groundMaskTop;
  Collider[] isGrounded;
  bool IsGround;

  void Update() {
    if(petSpiderPhase == PetSpiderPhase.shop) {
      SetAnimation(shopIdleAnim, true);
      return;
    }

    if(spiderPosition == SpiderPosition.down)
      isGrounded = Physics.OverlapSphere(transform.position, 0.15f, groundMaskDown);
    else
      isGrounded = Physics.OverlapSphere(transform.position, 0.15f, groundMaskTop);

    IsGround = (isGrounded.Length > 0 ? true : false);

    if(isGrounded.Length > 0) {
      if(!audioComp.isPlaying) audioComp.UnPause();
    } else {
      if(audioComp.isPlaying) audioComp.Pause();

      if(spiderPosition == SpiderPosition.top) {
        Collider[] isGrounded2 = Physics.OverlapSphere(transform.position, heightDistRevert, groundMaskTop);
        if(isGrounded2.Length > 0) {
          graphic.localScale = new Vector3(1, -1, 1);
          if(petSpiderPhase == PetSpiderPhase.uses) playerObject.GetComponent<PlayerPets>().ScalingGraphic(true);
        }
      } else {
        Collider[] isGrounded2 = Physics.OverlapSphere(transform.position, heightDistRevert, groundMaskDown);
        if(isGrounded2.Length > 0) {
          graphic.localScale = new Vector3(1, 1, 1);
          if(petSpiderPhase == PetSpiderPhase.uses) playerObject.GetComponent<PlayerPets>().ScalingGraphic(false);
        }
      }
    }

    if(petSpiderPhase == PetSpiderPhase.uses) JackControl();
    else RunFree();

    if(petSpiderPhase == PetSpiderPhase.uses && timeStartWork + timeWork + (perkLevel * 3) < Time.time) PetDisconnect();

    if(transform.position.y > CameraController.displayDiff.topDif(2) || transform.position.y < CameraController.displayDiff.topDif(-2)
        || transform.position.x > CameraController.displayDiff.rightDif(2) || transform.position.x < CameraController.displayDiff.rightDif(-2)) {
      if(petSpiderPhase == PetSpiderPhase.uses)
        RunnerController.petActivate(PetsTypes.none, false);
    }
  }

  void ChangeState(RunnerPhase state) {
    runnerPhase = state;
  }
  void JumpBarrier() {
    if(Time.time <= barrierWait) return;

    if(!thisJump) {
      Vector3 orig = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
      Ray ray = new Ray(orig, Vector3.right);
      RaycastHit[] rayhit;
      rayhit = Physics.RaycastAll(ray, 2f);

      for(int j = 0; j < rayhit.Length; j++) {
        if(LayerMask.LayerToName(rayhit[j].transform.gameObject.layer) == "Barrier") {
          barrierWait = Time.time + 0.7f;
          if(Random.value <= 0.99f) {
            jumpNow = true;
            jumpDist = 5f - RunnerController.RunSpeed * 0.7f;
            if(jumpDist <= 0)
              jumpDist = 0;
          }
        }
      }
    }
  }

  void JumpBreak() {
    if(!thisJump) {

      Vector3 orig = new Vector3(transform.position.x, transform.position.y - 1.5f, transform.position.z);
      Ray ray = new Ray(orig, Vector3.right);
      RaycastHit[] rayhit = Physics.SphereCastAll(ray, 3, 10);

      float distantionJump = 15f;

      foreach(RaycastHit one in rayhit) {
        if(one.transform.tag == "jumpDown") {
          float dist = Vector3.Distance(one.transform.position, orig);
          if(dist < distantionJump) distantionJump = dist;
        }
      }

      if(distantionJump > 15f)
        distantionJump = 15f;

      distantionJump -= RunnerController.RunSpeed * 0.9f;

      if(distantionJump <= 0.1f) distantionJump = 0.1f;

      jumpNow = true;
      jumpDist = distantionJump;
    }
  }

  void PetDisconnect() {
    playerObject.GetComponent<PlayerPets>().PetDisconnect();
  }

  void RunFree() {
    if(petSpiderPhase == PetSpiderPhase.free)
      velocity.x = RunnerController.RunSpeed * 3;
    else
      velocity.x = RunnerController.RunSpeed * 0.2f;


    if(spiderPosition == SpiderPosition.down)
      velocity.y -= gravity * Time.deltaTime;
    else
      velocity.y += gravity * Time.deltaTime;

    JumpBarrier();

    // Определение расстояния до конца конца обрыва
    for(int i = 0; i < isGrounded.Length; i++) { if(isGrounded[i].tag == "jumpUp" && !thisJump) JumpBreak(); }


    if(jumpNow && IsGround) {
      thisJump = true;
      jumpNow = false;
    }

    if(thisJump) {
      if(petSpiderPhase == PetSpiderPhase.free)
        velocity.x = RunnerController.RunSpeed * 3;
      else
        velocity.x = RunnerController.RunSpeed * jumpDist;
    }

    if(thisJump && velocity.y < 0 && IsGround) thisJump = false;

    if(IsGround && velocity.y < 0) {
      velocity.y = 0;
      transform.position = new Vector3(transform.position.x, isGrounded[0].transform.position.y, transform.position.z);
    }

    transform.position += velocity * Time.deltaTime;
  }


  /// <summary>
  /// Джек активирует мышь
  /// </summary>
  public void JackActivate(GameObject jack) {
    if(petSpiderPhase == PetSpiderPhase.none) {
      AudioManager.PlayEffect(catchClip, AudioMixerTypes.runnerEffect);
      timeStartWork = Time.time;
      SetAnimation(runSlowAnim, true);
      playerObject = jack;
      petSpiderPhase = PetSpiderPhase.uses;
      //jack.GetComponent<PlayerPets>().EnableSpider(this, skeletonAnimation, contactJackSlot, timeWork + (perkLevel * 3));

      Spawner.Instance.ActivateParticles(transform.position + Vector3.up);

      AudioManager.PlayEffect(touchClip, AudioMixerTypes.runnerEffect);
      spiderPosition = SpiderPosition.top;
    }

    //if (petSpiderPhase == PetSpiderPhase.shop)
    //    jack.GetComponent<PlayerPets>().EnableSpider(this, skeletonAnimation, contactJackSlot, timeWork + (perkLevel * 3));
  }

  public void JackDisActive(bool damage = false) {
    if(petSpiderPhase == PetSpiderPhase.uses) {
      if(damage) AudioManager.PlayEffect(hitClip, AudioMixerTypes.runnerEffect);

      Spawner.Instance.ActivateParticles(transform.position + Vector3.up);
      SetAnimation(runFastAnim, true);
      AudioManager.PlayEffect(touchClip, AudioMixerTypes.runnerEffect);
      petSpiderPhase = PetSpiderPhase.free;
    }
  }

  /// <summary>
  /// Состояние когда управляется джеком
  /// </summary>
  void JackControl() {
    velocity.x = mobileMoveHorizontal * horisontalSpeed;

    // Ограничения горизонтального перемещения
    if((transform.position.x < CameraController.displayDiff.transform.position.x + CameraController.displayDiff.left * 0.8f && velocity.x < 0)
        || (transform.position.x > CameraController.displayDiff.transform.position.x + CameraController.displayDiff.right * 0.8f && velocity.x > 0))
      velocity.x = 0;

    velocity.x += RunnerController.RunSpeed;

    if(spiderPosition == SpiderPosition.down)
      velocity.y -= gravity * Time.deltaTime;
    else
      velocity.y += gravity * Time.deltaTime;

    if(canJump && jump && IsGround) {
      canJump = false;
      velocity.y = -jumpSpeed;

    }

    if(IsGround) {
      if((spiderPosition == SpiderPosition.down && velocity.y < 0) || (spiderPosition == SpiderPosition.top && velocity.y > 0))
        velocity.y = 0;
      transform.position = new Vector3(transform.position.x, isGrounded[0].transform.position.y, transform.position.z);
    }

    transform.position += velocity * Time.deltaTime;
  }

  #region Реакция на кнопки

  public void Movement(float mov) {
    mobileMoveHorizontal = mov;
  }

  public void CanJump(bool flag) {
    if(flag)
      jump = true;
    else {
      canJump = true;
      jump = false;
    }
  }

  #endregion


  void OnTriggerEnter2D(Collider2D col) {
    if(petSpiderPhase == PetSpiderPhase.uses) {

      // При контакте с монетой, добавляем
      if(col.tag == "Coins") {
        col.GetComponent<Coin>().AddCouns();
      }

      // Если столкнулись с камнем
      if(LayerMask.LayerToName(col.gameObject.layer) == "Barrier") {
        if(col.tag == "RollingStone") {
          if(velocity.y < 0) {
            col.GetComponent<BarrierController>().DestroyThis();
            velocity.y = 15f;
            //Questions.QuestionManager.addJumpOnStone(transform.position);
            Questions.QuestionManager.ConfirmQuestion(Quest.jumpOnStone, 1, transform.position);
          } else {
            //PetDisconnect();
            col.GetComponent<BarrierController>().DestroyThis();
          }
        }
      }
    }


    if(col.gameObject.tag == "Player" && petSpiderPhase == PetSpiderPhase.none && runnerPhase == RunnerPhase.run) JackActivate(col.gameObject);
  }


  #region Animation

  /// <summary>
  /// Применение основной анимации
  /// </summary>
  /// <param name="spine">Кости</param>
  /// <param name="anim">Aнимация</param>
  /// <param name="loop">Зацикливание</param>
  public void SetAnimation(string anim, bool loop) {
    if(!gameObject) return; // Вероятно исправляет ошибку вылета

    if(currentAnimation != anim) {
      skeletonAnimation.state.SetAnimation(0, anim, loop);
      currentAnimation = anim;
    }
  }


  /* ***************************
   * Резет анимации
   * ***************************/
  public void ResetAnimation() {
    skeletonAnimation.Initialize(true);
    currentAnimation = null;
  }


  /* ***************************
   * Добавленная анимация
   * ***************************/
  public void AddAnimation(int index, string animName, bool loop, float delay) {
    skeletonAnimation.state.AddAnimation(index, animName, loop, delay);
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
    if(shadowInstance) Destroy(shadowInstance);
  }

  IEnumerator SetShadowDead() {
    yield return new WaitForSeconds(0.1f);
    if(shadowInstance)
      shadowInstance.GetComponent<ShadowBehaviour>().SetDiff(new Vector3(0.5f, 0, 0), new Vector3(0.3f, 0, 0));
    yield return 0;
  }

  public void ShadowFixed(bool flag) {
    if(shadowInstance)
      shadowInstance.GetComponent<ShadowBehaviour>().fixedsize = flag;
  }

  public void ShadowSetDiff(Vector3 newPos, Vector3 newScale) {
    if(shadowInstance)
      shadowInstance.GetComponent<ShadowBehaviour>().SetDiff(newPos, newScale);
  }

  public void ShadowSetDeff() {
    if(shadowInstance)
      shadowInstance.GetComponent<ShadowBehaviour>().SetDeff();
  }

  public void ShadowShow(bool flag) {
    if(shadowInstance) shadowInstance.SetActive(flag);
  }
  #endregion

  #region Звуки

  public AudioClip catchClip;
  public AudioClip hitClip;
  public AudioClip footStepClip;


  AudioSource audioComp;

  void InitAudio() {
    audioComp = GetComponent<AudioSource>();
    audioComp.clip = footStepClip;
    audioComp.loop = true;
    audioComp.Play();
  }

  #endregion

}
