using UnityEngine;
using System.Collections;
using Spine.Unity;
using Spawner = Road.Spawners.Pets.PetSpawner;

public class PetDinoController : MonoBehaviour {

  public SkeletonAnimation skeletonAnimation;                     // Кости при использовании

  public float timeWork;                                          // Время работ
  float timeStartWork;                                            // Время начало взаимодействия с Джеком
  int perkLevel;                                                  // Уровень прокачки

  public GameObject shadowObject;                                 // Объект тени
  GameObject shadowInstance;                                      // Экземпляр тени

  public float horisontalSpeed;                                   // Скорость горизонтального полета
  public float jumpSpeed;                                         // Скорость прыжка

  GameObject playerObject;
  [SerializeField]
  [SpineBone]
  string contactJackSlot;            // Слот контакта с джеком

  string currentAnimation;                                        // Текущее состояние анимации
  [SpineAnimation(dataField: "skeletonAnimation")]
  public string runAnim = "";
  [SpineAnimation(dataField: "skeletonAnimation")]
  public string attackAnim = "";
  [SpineAnimation(dataField: "skeletonAnimation")]
  public string jumpAnim = "";
  [SpineAnimation(dataField: "skeletonAnimation")]
  public string shopIdleAnim = "";

  bool enableNow;                                                             // Только что инициализировался

  Vector3 velocity;                                                           // Движение
  public float gravity;                                                       // Гравитация 

  float mobileMoveHorizontal;                                                 // Значение мобильного контроллера, отвечающего за горизонтальное смещение
  bool jump;
  bool canJump;

  bool jumpNow;                                                   // Выполнить прыжок
  public bool thisJump;
  float jumpDist;                                                 // Дистанция прыжка
  float barrierWait;

  float timeAttackWait;

  enum PetDinoPhase { none, uses, free, shop };

  [SerializeField]
  PetDinoPhase petDinoPhase;
  RunnerPhase runnerPhase;

  public AudioClip touchClip;

  void OnEnable() {
    RunnerController.OnChangeRunnerPhase += ChangeState;
    runnerPhase = RunnerController.Instance.runnerPhase;
		InitAudio();
    enableNow = true;
    GetComponent<SphereCollider>().enabled = true;
    GetComponent<SphereCollider>().isTrigger = true;
    if(petDinoPhase != PetDinoPhase.shop) petDinoPhase = PetDinoPhase.none;
    canJump = true;
    ShadowInit();

    perkLevel = Spawner.levelDino;
  }

  void OnDisable() {
    RunnerController.OnChangeRunnerPhase -= ChangeState;
    ShadowDestroy();
  }

  void LateUpdate() {
    if(petDinoPhase == PetDinoPhase.shop) {
      SetAnimation(shopIdleAnim, true);
      enableNow = false;
      return;
    }

    if(enableNow) {
      SetAnimation(runAnim, true);
      enableNow = false;
    }
  }

  public LayerMask groundMask;
  Collider[] isGrounded;
  bool IsGround;

  void Update() {
    if(petDinoPhase == PetDinoPhase.shop) {
      SetAnimation(shopIdleAnim, true);
      audioComp.Pause();
      return;
    }

    isGrounded = Physics.OverlapSphere(transform.position, 0.15f, groundMask);

    if(isGrounded.Length > 0) {
      IsGround = true;
      SetAnimation(runAnim, true);
      if(!audioComp.isPlaying)
        audioComp.UnPause();
    } else {
      IsGround = false;
      SetAnimation(jumpAnim, true);
      if(audioComp.isPlaying)
        audioComp.Pause();
    }

    if(petDinoPhase == PetDinoPhase.uses)
      JackControl();
    else
      RunFree();

    if(transform.position.y > CameraController.displayDiff.topDif(2) || transform.position.y < CameraController.displayDiff.topDif(-2)
        || transform.position.x > CameraController.displayDiff.rightDif(2) || transform.position.x < CameraController.displayDiff.rightDif(-2)) {
      if(petDinoPhase == PetDinoPhase.uses)
        RunnerController.petActivate(Player.Jack.PetsTypes.none, false);
      gameObject.SetActive(false);
    }

    if(petDinoPhase == PetDinoPhase.uses && timeStartWork + timeWork + (perkLevel * 3) < Time.time) PetDisconnect();
  }

  void ChangeState(RunnerPhase state) {
    runnerPhase = state;
  }

  void PetDisconnect() {
    playerObject.GetComponent<Player.Jack.PlayerPets>().PetDisconnect();
  }

  void RunFree() {
    if(petDinoPhase == PetDinoPhase.free)
      velocity.x = RunnerController.RunSpeed * 3;
    else
      velocity.x = RunnerController.RunSpeed * 0.2f;

    velocity.y -= gravity * Time.deltaTime;

    JumpBarrier();

    // Определение расстояния до конца конца обрыва
    for(int i = 0; i < isGrounded.Length; i++) {
      if(isGrounded[i].tag == "jumpUp" && !thisJump) {
        JumpBreak();
      }
    }

    if(jumpNow && IsGround) {
      velocity.y = jumpSpeed;
      thisJump = true;
      jumpNow = false;
    }

    if(thisJump) {
      if(petDinoPhase == PetDinoPhase.free)
        velocity.x = RunnerController.RunSpeed * 3;
      else
        velocity.x = RunnerController.RunSpeed * jumpDist;
    }

    if(thisJump && velocity.y < 0 && IsGround)
      thisJump = false;

    if(IsGround && velocity.y < 0) {
      velocity.y = 0;
      transform.position = new Vector3(transform.position.x, isGrounded[0].transform.position.y, transform.position.z);
    }

    transform.position += velocity * Time.deltaTime;
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

      if(distantionJump > 15f) distantionJump = 15f;
      distantionJump -= RunnerController.RunSpeed * 0.85f;
      if(distantionJump <= 0.1f) distantionJump = 0.1f;

      jumpNow = true;

      jumpDist = distantionJump;
    }
  }

  /// <summary>
  /// Джек активирует мышь
  /// </summary>
  public void JackActivate(GameObject jack) {
    if(petDinoPhase == PetDinoPhase.none) {
      AudioManager.PlayEffect(catchClip, AudioMixerTypes.runnerEffect);
      timeStartWork = Time.time;
      SetAnimation(runAnim, true);
      playerObject = jack;
      petDinoPhase = PetDinoPhase.uses;
      //jack.GetComponent<PlayerPets>().EnableDino(this, skeletonAnimation, contactJackSlot, timeWork + (perkLevel * 3));
      Spawner.Instance.ActivateParticles(transform.position + Vector3.up);
      AudioManager.PlayEffect(touchClip, AudioMixerTypes.runnerEffect);
    }

    //if(petDinoPhase == PetDinoPhase.shop)
    //    jack.GetComponent<PlayerPets>().EnableDino(this, skeletonAnimation, contactJackSlot, timeWork + (perkLevel * 3));
  }

  public void JackDisActive(bool damage = false) {
    if(petDinoPhase == PetDinoPhase.uses) {
      if(damage) AudioManager.PlayEffect(hitClip, AudioMixerTypes.runnerEffect);
      Spawner.Instance.ActivateParticles(transform.position + Vector3.up);

      AudioManager.PlayEffect(touchClip, AudioMixerTypes.runnerEffect);

      SetAnimation(runAnim, true);
      petDinoPhase = PetDinoPhase.free;
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

    velocity.y -= gravity * Time.deltaTime;

    if(canJump && jump && IsGround) {
      velocity.y = jumpSpeed;
      canJump = false;
    }

    if(IsGround && velocity.y < 0) {
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
    if(petDinoPhase == PetDinoPhase.uses) {

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
            PetDisconnect();
            col.GetComponent<BarrierController>().DestroyThis();
          }
        }
      }
    }

    if(col.gameObject.tag == "Player" && petDinoPhase == PetDinoPhase.none && runnerPhase == RunnerPhase.run) JackActivate(col.gameObject);

    if(col.gameObject.tag == "Enemy") {
      if(col.transform.position.x > transform.position.x && petDinoPhase == PetDinoPhase.uses) {
        if(shotTime > Time.time) {
          col.GetComponent<Enemy>().Damage(WeaponTypes.pet, 100, transform.position, DamagePowen.level1, 0);
        }
      }
    }

  }

  float shotTime;
  float shotTimeWait;

  public void Shot() {
    if(petDinoPhase != PetDinoPhase.uses) return;
    if(shotTimeWait <= Time.time) {
      shotTimeWait = Time.time + 0.7f;
      shotTime = Time.time + 0.5f;
      AddAnimation(1, attackAnim, false, 0);
      AudioManager.PlayEffect(attackClip, AudioMixerTypes.runnerEffect);
    }
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
  public AudioClip attackClip;
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
