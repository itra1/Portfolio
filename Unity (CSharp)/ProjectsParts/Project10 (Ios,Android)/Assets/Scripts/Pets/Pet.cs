using UnityEngine;
using System.Collections;
using Spine.Unity;
using Spawner = Road.Spawners.Pets.PetSpawner;
using Player.Jack;

namespace Pet {

  /// <summary>
  /// Контроллер петов
  /// </summary>
  public abstract class Pet: MonoBehaviour {

    public Rigidbody2D rb;
    public Transform groundPoint;
    protected enum Phase { none, uses, free, shop }; // Фаза пета
    public Player.Jack.PetsTypes type; // Тип пета
    public SkeletonAnimation skeletonAnimation; // Ссылка на спайновый компонент
    public float timeWork;  // Стандартное время работы
    float timeStartWork;  // Время начала работы
    int perkLevel;  // Уровень прокачки
    public float horisontalSpeed; // Скорость горизонтального движения при управлении играком
    public float jumpSpeed; // Скорость прыжка
    [SerializeField]
    [SpineBone]
    string contactJackSlot; // Кость, которая контактирует с джеком
    string currentAnimation;  // Активная основная анимация
    [SpineAnimation(dataField: "skeletonAnimation")]
    public string runAnim = ""; // Анимация бега
    [SpineAnimation(dataField: "skeletonAnimation")]
    public string shopIdleAnim = "";  // Анимация в магазине
    bool isEnable;    // Флаг инициализации
    protected Vector3 velocity; // Вектор движения
    public float gravity; // Гравитация
    float horizontalKeyValue; // Значение контроллер горизонтального курсора
    protected bool jump;  // флаг выполнения прыжка
    protected bool canJump; // Флаг готовности выполнить прыжок
    protected bool thisJump;  // Прыжок выполняется
    float jumpDist; // Расстояние для выполнения прыжка
    [SerializeField]
    protected Phase phase;  // Текущая фаза пета
    RunnerPhase runnerPhase;  // Фаза забега
    public AudioClip touchClip; // Звук касания пета
    public LayerMask groundMask;  // Слой с поверхностью земли
    protected bool isGround;        // Флаг соприкосновения с землей
    protected GameObject playerObject;    // Объект плеера
    float barrierWait;        // Ожидание барьера
    public bool jumpKey;

    public virtual void OnEnable() {

      RunnerController.OnChangeRunnerPhase += ChangeState;
      runnerPhase = RunnerController.Instance.runnerPhase;
      InitAudio();
      isEnable = true;
      GetComponent<CapsuleCollider2D>().enabled = true;
      GetComponent<CapsuleCollider2D>().isTrigger = true;
      canJump = true;
      if (phase != Phase.shop) {
        rb.bodyType = RigidbodyType2D.Dynamic;
        phase = Phase.none;
      } else
        rb.bodyType = RigidbodyType2D.Kinematic;
      ShadowInit();

      perkLevel = Spawner.GetPetLevel(type);
    }

    public virtual void OnDisable() {
      RunnerController.OnChangeRunnerPhase -= ChangeState;
      ShadowDestroy();
    }

    public virtual void LateUpdate() {
      if (phase == Phase.shop) {
        SetAnimation(shopIdleAnim, true);
        isEnable = false;
        return;
      }

      if (isEnable) {
        SetAnimation(runAnim, true);
        isEnable = false;
      }
    }

    protected virtual void FixedUpdate() {
      if (phase != Phase.shop)
        UpdateGame();
    }

    public virtual void Update() {

      if (phase == Phase.shop)
        UpdateShop();
    }

    /// <summary>
    /// Кадровая обработка в игре
    /// </summary>
    public virtual void UpdateGame() {

      isGround = CheckGround();

      if (isGround) {
        OnGroundActive();
      } else {
        NotGroundActive();
      }

      // Обработка бега
      if (phase == Phase.uses)
        JackControl();
      else
        RunFree();

      // Отключение при выходе за игровую область
      if (transform.position.y > CameraController.displayDiff.topDif(2) || transform.position.y < CameraController.displayDiff.topDif(-2)
          || transform.position.x > CameraController.displayDiff.rightDif(2) || transform.position.x < CameraController.displayDiff.rightDif(-2)) {
        if (phase == Phase.uses)
          RunnerController.petActivate(Player.Jack.PetsTypes.none, false);
        gameObject.SetActive(false);
      }

      // Отключаем, если истекло время
      if (phase == Phase.uses && timeStartWork + timeWork + (perkLevel * 3) < Time.time)
        PetDisconnect();
    }

    protected virtual bool CheckGround() {
      return (Physics2D.OverlapCircle(groundPoint.position, 0.1f, groundMask) != null);
    }

    /// <summary>
    /// Кадровая обработка в магазине
    /// </summary>
    public virtual void UpdateShop() {
      SetAnimation(shopIdleAnim, true);
      audioComp.Pause();
    }

    /// <summary>
    /// Обьект соприкасается с землей
    /// </summary>
    protected virtual void OnGroundActive() {
      SetAnimation(runAnim, true);
      if (!audioComp.isPlaying)
        audioComp.UnPause();
    }

    /// <summary>
    /// Объект не соприкасается с землей
    /// </summary>
    protected virtual void NotGroundActive() {
      if (audioComp.isPlaying)
        audioComp.Pause();
    }


    /// <summary>
    /// Освобождение пета
    /// </summary>
    void PetDisconnect() {
      playerObject.GetComponent<PlayerPets>().PetDisconnect();
    }

    /// <summary>
    /// Активация пета
    /// </summary>
    public virtual void JackActivate(GameObject jack) {
      jack.GetComponent<PlayerPets>().EnablePet(this, skeletonAnimation, contactJackSlot, timeWork + (perkLevel * 3));

      if (phase == Phase.none) {
        AudioManager.PlayEffect(catchClip, AudioMixerTypes.runnerEffect);
        timeStartWork = Time.time;
        SetAnimation(runAnim, true);
        playerObject = jack;
        phase = Phase.uses;
        Spawner.Instance.ActivateParticles(transform.position + Vector3.up);
        AudioManager.PlayEffect(touchClip, AudioMixerTypes.runnerEffect);
      }
    }

    /// <summary>
    /// Деактивация пета
    /// </summary>
    /// <param name="damage"></param>
    public virtual void JackDisActive(bool damage = false) {
      if (phase == Phase.uses) {
        if (damage)
          AudioManager.PlayEffect(hitClip, AudioMixerTypes.runnerEffect);
        Spawner.Instance.ActivateParticles(transform.position + Vector3.up);
        AudioManager.PlayEffect(touchClip, AudioMixerTypes.runnerEffect);
        SetAnimation(runAnim, true);
        phase = Phase.free;
      }
    }

    protected virtual void OnTriggerEnter2D(Collider2D col) {

      if (phase == Phase.free) {

        if (col.tag == "jumpUp" && !thisJump) {
          JumpBreak();
        }

        if (col.tag == "BarrierUp" && !thisJump) {
          JumpBarrier();
        }

      }


      if (phase == Phase.uses) {

        // При контакте с монетой, добавляем
        if (col.tag == "Coins") {
          col.GetComponent<Coin>().AddCouns();
        }

        // Если столкнулись с камнем
        if (LayerMask.LayerToName(col.gameObject.layer) == "Barrier") {
          if (col.tag == "RollingStone") {
            if (velocity.y < 0) {
              col.GetComponent<BarrierController>().DestroyThis();
              velocity.y = 15f;
              rb.velocity = new Vector2(rb.velocity.x, 0);
              rb.AddForce(new Vector2(rb.velocity.x, 10f), ForceMode2D.Impulse);
              //Questions.QuestionManager.addJumpOnStone(transform.position);
              Questions.QuestionManager.ConfirmQuestion(Quest.jumpOnStone, 1, transform.position);
            } else {
              PetDisconnect();
              col.GetComponent<BarrierController>().DestroyThis();
            }
          }
        }
      }

      if (col.gameObject.tag == "Player" && phase == Phase.none && runnerPhase == RunnerPhase.run)
        JackActivate(col.gameObject);

    }

    /// <summary>
    /// Обработка атаки
    /// </summary>
    public virtual void Shoot() {
    }

    #region Свободное движение

    /// <summary>
    /// Изменение фазы забега
    /// </summary>
    /// <param name="newPhase">Новая фаза</param>
    public virtual void ChangeState(RunnerPhase newPhase) {
      runnerPhase = newPhase;
    }

    /// <summary>
    /// Свободное движение
    /// </summary>
    protected virtual void RunFree() {
      velocity = rb.velocity;

      if (isGround && velocity.y < 0)
        thisJump = false;

      if (phase == Phase.free)
        velocity.x = RunnerController.RunSpeed * 3;
      else
        velocity.x = RunnerController.RunSpeed * 0.2f;

      if (thisJump) {
        if (phase == Phase.free)
          velocity.x = RunnerController.RunSpeed * 3;
        else
          velocity.x = RunnerController.RunSpeed * jumpDist;
      }

      rb.velocity = velocity;

      //CalcGravity();

      // Проверка на возможность прыгнуть через препядствие
      //JumpBarrier();

      // Определение расстояния до конца конца обрыва
      //for(int i = 0; i < isGrounded.Length; i++) {
      //  if(isGrounded[i].tag == "jumpUp" && !thisJump) {
      //    JumpBreak();
      //  }
      //}

      //if (jumpNow && isGround) {
      //	velocity.y = jumpSpeed;
      //	thisJump = true;
      //	jumpNow = false;
      //}




      //if (isGround && velocity.y < 0) {
      //	velocity.y = 0;
      //	transform.position = new Vector3(transform.position.x, isGrounded[0].transform.position.y, transform.position.z);
      //}

      //transform.position += velocity * Time.deltaTime;
    }

    /// <summary>
    /// Рассчет гравитационного воздействия
    /// </summary>
    //protected virtual void CalcGravity() {
    //	velocity.y -= gravity * Time.deltaTime;
    //}

    Ray checkRayJump; // Луч проверки
    float distantionJump; // Дистанция для предполагаемого прыжка
                          /// <summary>
                          /// Проверка препятствий
                          /// </summary>
    protected void JumpBarrier() {
      if (Time.time <= barrierWait || thisJump)
        return;

      Vector3 checkVectorJumpHit = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
      if (Physics2D.Raycast(checkVectorJumpHit, Vector3.right, 2f, LayerMask.NameToLayer("Barrier"))) {
        barrierWait = Time.time + 0.7f;
        if (Random.value <= 0.99f) {
          jumpDist = 5f - RunnerController.RunSpeed * 0.7f;
          rb.velocity = new Vector2(rb.velocity.x, 0);
          rb.AddForce(new Vector2(rb.velocity.x, jumpSpeed), ForceMode2D.Impulse);
          if (jumpDist <= 0)
            jumpDist = 0;
        }
      }
    }

    /// <summary>
    /// Проверка ям
    /// </summary>
    protected void JumpBreak() {
      if (thisJump)
        return;

      Vector3 checkVectorJumpHit = new Vector3(transform.position.x, transform.position.y - 1.5f, transform.position.z);
      Collider2D[] checkHit = Physics2D.OverlapAreaAll(new Vector2(transform.position.x, transform.position.y - 4), new Vector2(transform.position.x + 7, transform.position.y + 4));

      distantionJump = 7f;

      foreach (Collider2D one in checkHit) {
        if (one.transform.tag == "jumpDown") {
          float dist = Vector3.Distance(one.transform.position, checkVectorJumpHit);
          if (dist < distantionJump)
            distantionJump = dist;
        }
      }

      if (distantionJump > 7f)
        distantionJump = 7f;
      distantionJump -= RunnerController.RunSpeed * 0.85f;
      if (distantionJump <= 0.1f)
        distantionJump = 0.1f;

      jumpDist = distantionJump;
      rb.velocity = new Vector2(rb.velocity.x, 0);
      rb.AddForce(new Vector2(rb.velocity.x, jumpSpeed), ForceMode2D.Impulse);
    }
    #endregion

    #region Управление

    public virtual float horizontalKey {
      set {
        horizontalKeyValue = value != 0 ? Mathf.Sign(value) : 0;
      }
    }

    /// <summary>
    /// Управление петом
    /// </summary>
    void JackControl() {
      velocity = rb.velocity;
      velocity.x = horizontalKeyValue * horisontalSpeed;

      // Ограничения горизонтального перемещения
      if ((transform.position.x < CameraController.displayDiff.transform.position.x + CameraController.displayDiff.left * 0.8f && velocity.x < 0)
          || (transform.position.x > CameraController.displayDiff.transform.position.x + CameraController.displayDiff.right * 0.8f && velocity.x > 0))
        velocity.x = 0;

      velocity.x += RunnerController.RunSpeed;

      //CalcGravity();

      rb.velocity = velocity;
      if (jumpKey)
        Jump();

      //CheckFixedGroung();

      //AlterChecking();

      //transform.position += velocity * Time.deltaTime;
    }

    /// <summary>
    /// Проверка соприкосновения с поверхностью
    /// </summary>
    //protected virtual void CheckFixedGroung() {
    //	if (isGround && velocity.y < 0) {
    //		velocity.y = 0;
    //		transform.position = new Vector3(transform.position.x, groundPoint.position.y, transform.position.z);
    //	}
    //}

    /// <summary>
    /// Дополнительные проверки
    /// </summary>
    //protected virtual void AlterChecking() {
    //}

    /// <summary>
    /// Обработка управляемого прыжка
    /// </summary>
    protected virtual void Jump() {
      jumpKey = false;
      if (isGround) {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(new Vector2(rb.velocity.x, jumpSpeed), ForceMode2D.Impulse);
      }
    }

    #endregion

    #region Animation

    /// <summary>
    /// Применение основной анимации
    /// </summary>
    /// <param name="spine">Кости</param>
    /// <param name="anim">Aнимация</param>
    /// <param name="loop">Зацикливание</param>
    public void SetAnimation(string anim, bool loop) {
      if (!gameObject)
        return; // Вероятно исправляет ошибку вылета

      if (currentAnimation != anim) {
        skeletonAnimation.state.SetAnimation(0, anim, loop);
        currentAnimation = anim;
      }
    }
    /// <summary>
    /// Сброс анимации
    /// </summary>
    public void ResetAnimation() {
      skeletonAnimation.Initialize(true);
      currentAnimation = null;
      skeletonAnimation.state.Event += AnimEvent;
    }
    /// <summary>
    /// Добавление анимации
    /// </summary>
    /// <param name="index">Слой</param>
    /// <param name="animName">Название анимации</param>
    /// <param name="loop">флаг зациклавания</param>
    /// <param name="delay">Задержка выполнения</param>
    public void AddAnimation(int index, string animName, bool loop, float delay) {
      skeletonAnimation.state.AddAnimation(index, animName, loop, delay);
    }

    protected virtual void AnimEvent(Spine.AnimationState state, int trackIndex, Spine.Event e) {
    }

    #endregion

    #region Тень
    /// <summary>
    /// Префаб тени
    /// </summary>
    public GameObject shadowPrefab;                                 // Объект тени
                                                                    /// <summary>
                                                                    /// Созданный экземпляр тени
                                                                    /// </summary>
    GameObject shadowInstance;                                      // Экземпляр тени
                                                                    /// <summary>
                                                                    /// Инициализации тени
                                                                    /// </summary>
    void ShadowInit() {
      shadowInstance = Instantiate(shadowPrefab, transform.position, Quaternion.identity) as GameObject;
      shadowInstance.GetComponent<ShadowBehaviour>().matherObject = gameObject.transform;
      shadowInstance.GetComponent<ShadowBehaviour>().diff = 0.3f;
      shadowInstance.transform.parent = transform;
    }

    void ShadowDestroy() {
      if (shadowInstance)
        Destroy(shadowInstance);
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
      if (shadowInstance)
        shadowInstance.SetActive(flag);
    }
    #endregion

    #region Звуки

    public AudioClip catchClip;
    public AudioClip hitClip;
    public AudioClip footStepClip;

    AudioSource audioComp;

    void InitAudio() {
      audioComp = GetComponent<AudioSource>();
      if (footStepClip != null) {
        audioComp.clip = footStepClip;
        audioComp.loop = true;
        audioComp.Play();
      }
    }

    #endregion

  }
}