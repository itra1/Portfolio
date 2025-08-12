using UnityEngine;
using System;
using Spine.Unity;

namespace Player.Jack {

  /// <summary>
  /// Управление играком в обычном режиме
  /// </summary>
  public class PlayerMoveOld: PlayerComponent {

    public static event Action OnPlayerGround;
    public event Action OnPlayerJump;

    private RunnerPhase playerState;
    private bool activDeadAnim;                                 // Флаг анимации смерти

    private bool jumpNow;                                       // Флаг, осначающий выполнения врыжка
    private float horizontalKey;                         // Значение мобильного контроллера, отвечающего за горизонтальное смещение

    [HideInInspector]
    public Vector3 velocity;             // Рассчет движения
    private Vector3 targetMove;

    public float runSpeed = 5;                         // Максимальная скорость горизонтального смещения

    private float horizontalSpeed;
    private float controlMoveX;

    public float jumpSpeed = 17;                        // Скорость прыжка

    [HideInInspector]
    public bool stopRun;
    [HideInInspector]
    public bool isSticky;             // Прилип

    private bool doubleJump;                                    // Разрешение на выполнение двойного прыжка

    [HideInInspector]
    public bool drag;                                   // Сопротивление
    [HideInInspector]
    public float dragTime;

    void OnEnable() {

      //horizontalSpeed = (PlayerPrefs.GetInt("speedPerk") + 1) * 0.3f;
      horizontalSpeed = 0.3f;

    }

    void Update() {

      if (pet.IsPet) return;

      // Поведение во время бустов, отдаем управление другому модулю
      if ((RunnerController.Instance.runnerPhase & (RunnerPhase.boost | RunnerPhase.preBoost | RunnerPhase.postBoost)) != 0)
        return;

      if (RunnerController.Instance.runnerPhase != playerState) {
        playerState = RunnerController.Instance.runnerPhase;
        if (RunnerController.Instance.runnerPhase == RunnerPhase.dead) {
          activDeadAnim = false;
          if (controller.isGround) {
            velocity.y = 12;
          }
        }
      }

      if (playerState != RunnerPhase.start) {
        rigidbody.gravityScale = controller.isGround ? 0 : controller.gravityNorm;
      }


      if ((playerState & (RunnerPhase.run | RunnerPhase.tutorial | RunnerPhase.boss | RunnerPhase.endRun | RunnerPhase.lowEnergy)) != 0) {
        PlayMove();
        GameAnimation();    // Анимация  
      }

      //CheckBorder();

      // Поведение ирока во время игрового процесса
      if ((playerState & (RunnerPhase.run | RunnerPhase.tutorial | RunnerPhase.boss | RunnerPhase.endRun | RunnerPhase.lowEnergy)) != 0) {
        //GameMovement();     // Движение
        //GameAttack();       // Атака    
      }
      // Поведение если убили
      if (playerState == RunnerPhase.dead) {
        //DeadMovement();     // Движение

        DeadAnimation();    // Анимация  
      }

      if (playerState == RunnerPhase.preBoost) {
        PreBoostMovement();     // Движение
      }
      if (playerState == RunnerPhase.dead) {
        DeadMovement();     // Движение
      }

      //if(playerState == RunnerPhase.preBoost) {
      //  PreBoostMovement();     // Движение
      //}

      if (useJump) {
        Questions.QuestionManager.ConfirmQuestion(Quest.jumped, 1, transform.position);
        useJump = false;
      }
      if (useDoubleJump) {
        RunnerController.doubleJumpCount++;
        check.ChechDoubleJump();
        useDoubleJump = false;
      }
      if (controller.isGround && jumpNow && rigidbody.velocity.y <= 0) {

        if (OnPlayerGround != null) OnPlayerGround();

        jumpNow = false;
        check.endJump = true;
        GameObject jumpFinish = Pooler.GetPooledObject("PlayerJumpFinish");
        jumpFinish.transform.position = new Vector3(transform.position.x + 0.08f, transform.position.y - 0.4f, -0.1f);
        jumpFinish.SetActive(true);
      }
    }


    //private void CheckBorder() {
    //	if (transform.position.x < CameraController.displayDiff.leftDif(0.8f))
    //		transform.position = new Vector3(CameraController.displayDiff.leftDif(0.8f), transform.position.y);

    //	if (transform.position.x > CameraController.displayDiff.rightDif(0.8f))
    //		transform.position = new Vector3(CameraController.displayDiff.rightDif(0.8f), transform.position.y);
    //}

    /// <summary>
    /// Атака игрока во время игрового процесса
    /// </summary>
    //void GameAttack() {
    //	// Финал анимации атаки 
    //	if (thisShoot) {
    //		if (_player.GetActiveWeapon() == WeaponTypes.ship) // Атака пушки
    //			_shootController.ShootEvent();
    //		thisShoot = false;
    //	}

    //	// Атака
    //	if (shoot && canShoot) {
    //		if (RunnerController.Instance.PlayerShoot()) {
    //			canShoot = false;
    //			//ShootNow();
    //		}
    //	}

    //}

    /// <summary>
    /// Анимация игрока во время игрового процесса
    /// </summary>
    void GameAnimation() {

      if (controller.isGround && rigidbody.velocity.y <= 0) {
        animation.timeScale = 1f;
        animation.SetAnimation(animation.runIdleAnim, true);
      } else if (useDoubleJump || useJump) {
        if (useDoubleJump) {
          animation.timeScale = 2.5f;
          animation.SetAnimation(animation.jumpDoubleIdleAnim, false, true);
        } else
          animation.SetAnimation(animation.jumpIdleAnim, true);
      }

    }

    public void MinJump() {
      //miniJump = val * (playerPets.playerPets == PlayerPetsTypes.spider ? -1 : 1);
      rigidbody.velocity = new Vector2(rigidbody.velocity.x, 0);
      jumpNow = true;
      rigidbody.AddForce(new Vector2(rigidbody.velocity.x, jumpSpeed * 0.8f), ForceMode2D.Impulse);
    }

    float x;

    bool useJump;
    bool useDoubleJump;

    public float RunSpeedToPlayer {
      get { return (playerState == RunnerPhase.endRun ? RunnerController.RunSpeed * 3 : RunnerController.RunSpeed)/* * 1.08f */* (drag ? 0.5f : 1); }
    }

    bool jumpActive;

    //private void LateUpdate() {
    //   if (transform.position.x < CameraController.displayDiff.leftDif(0.8f))
    //     velocity.x = RunSpeedToPlayer * 1.1f;

    //   if (transform.position.x > CameraController.displayDiff.rightDif(0.8f))
    //     velocity.x = RunSpeedToPlayer * 0.9f;


    //   if (transform.position.x <= CameraController.displayDiff.leftDif(0.8f) && (GameManager.activeLevelData.moveVector == MoveVector.left ? horizontalKey < 0 : horizontalKey > 0))
    //     transform.position = new Vector3(CameraController.displayDiff.leftDif(0.8f), transform.position.y, transform.position.z);
    //   if (transform.position.x >= CameraController.displayDiff.rightDif(0.8f) && (GameManager.activeLevelData.moveVector == MoveVector.left ? horizontalKey > 0 : horizontalKey < 0))
    //     transform.position = new Vector3(CameraController.displayDiff.rightDif(0.8f), transform.position.y, transform.position.z);

    // }

    void PlayMove() {

      if (stopRun) return;

      x = 0;
      x = horizontalKey * horizontalSpeed;

      velocity = rigidbody.velocity;
      velocity.x = RunSpeedToPlayer;
      controlMoveX = 0;

      if (horizontalKey < 0 && transform.position.x < CameraController.displayDiff.leftDif(0.8f))
        velocity.x = RunSpeedToPlayer;
      else if (horizontalKey > 0 && transform.position.x > CameraController.displayDiff.rightDif(0.8f))
        velocity.x = RunSpeedToPlayer;
      else {
        if (x != 0) controlMoveX = runSpeed * Mathf.Sign(x) * (drag ? 0.2f : 1);
        velocity.x += controlMoveX;
      }

      if (isSticky)
        velocity.x = RunSpeedToPlayer;


      //if (transform.position.x > CameraController.displayDiff.rightDif(0.8f))
      //	velocity.x = thisRunSpeedX * 1.035f * (drag ? 0.5f : 1);

      //if (GameManager.activeLevelData.moveVector == MoveVector.leftMove) {
      //	if (transform.position.x < CameraController.displayDiff.leftDif(0.8f))
      //		velocity.x = RunSpeedToPlayer * 1.1f;

      //	if (transform.position.x > CameraController.displayDiff.rightDif(0.8f))
      //		velocity.x = RunSpeedToPlayer * 0.9f;
      //}
      //else {
      //	if (transform.position.x < CameraController.displayDiff.leftDif(0.8f))
      //		velocity.x = RunSpeedToPlayer * 0.9f;

      //	if (transform.position.x > CameraController.displayDiff.rightDif(0.8f))
      //		velocity.x = RunSpeedToPlayer * 1.1f;
      //}


      if (!doubleJump && controller.isGround && rigidbody.velocity.y <= 0) doubleJump = true;

      if (isJump) {
        isJump = false;
        if (controller.isGround) {
          animation.SetAnimation(animation.jumpIdleAnim, true);
          audio.PlayEffect(audio.jumpAudio, AudioMixerTypes.runnerEffect);
          useJump = true;
        } else if ((doubleJump && !useDoubleJump) || GameManager.activeLevelData.gameFormat == GameMechanic.jetPack) {
          doubleJump = false;
          useDoubleJump = true;
          audio.PlayEffect(audio.jumpAudioAir, AudioMixerTypes.runnerEffect);
        } else
          return;

        jumpNow = true;

        GameObject jumpStart = Pooler.GetPooledObject("PlayerJumpStart");
        jumpStart.transform.position = new Vector2(controller.graundPoint.position.x, controller.graundPoint.position.y);
        jumpStart.SetActive(true);

        rigidbody.velocity = new Vector2(velocity.x, 0);

        rigidbody.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
      } else {
        rigidbody.velocity = velocity;
      }
    }

    /// <summary>
    /// Движение игрока во время забега
    /// </summary>
    //void GameMovement() {
    //  x = 0;
    //  controlMoveX = 0;

    //  if(drag && dragTime != 0 && dragTime <= Time.time) {
    //    dragTime = 0;
    //    drag = false;
    //  }

    //  if(lastGroundEvent != player.isGround) {
    //    lastGroundEvent = player.isGround;
    //    if(OnPlayerGround != null)
    //      OnPlayerGround(lastGroundEvent);
    //  }

    //  x = horizontalKey * horizontalSpeed;

    //  velocity.x = thisRunSpeedX * (drag ? 0.5f : 1);

    //  //при плиземлении на поверхность, обновляем флаг двойного прыжка
    //  if(player.isGround && canDoubleJump) doubleJump = true;

    //  if(isJump && player.isGround && !jumpNow) {

    //    if(OnPlayerJump != null) OnPlayerJump();

    //    jumpNow = true;

    //    if(!isSticky) {
    //      AudioManager.PlayEffects(jumpAudio, AudioMixerTypes.runnerEffect);

    //      velocity.y = jumpSpeed * (GameManager.isIPad ? 1.1f : 1);

    //      jumpThis = true;
    //      down = false;

    //      Questions.QuestionManager.ConfirmQuestion(Quest.jumped, 1, transform.position);
    //      // Взрыв
    //      if(player.isGround) {
    //        isNet = false;

    //        for(int i = 0; i < player.isGroungsArray.Length; i++)
    //          if(LayerMask.LayerToName(player.isGroungsArray[i].gameObject.layer) == "Net")
    //            isNet = true;

    //        // Эффект пыли из-под ног в начале прыжка, если не находимся в паутине
    //        if(!isNet) {
    //          GameObject jumpStart = Pooler.GetPooledObject("PlayerJumpStart");
    //          jumpStart.transform.position = new Vector3(transform.position.x - 0.3f, transform.position.y - 0.7f, -0.1f);
    //          jumpStart.SetActive(true);
    //        }
    //      }
    //    }
    //  } else if(isJump && !player.isGround && doubleJump && RunnerController.instance.doubleJump) {
    //    if(!isSticky) {
    //      AudioManager.PlayEffects(jumpAudio2, AudioMixerTypes.runnerEffect);


    //      doublejumpAnim = Time.time + 0.27f;
    //      velocity.y = jumpSpeed * (GameManager.isIPad ? 1.1f : 1);
    //      doubleJump = false;
    //      thisDoubleJump = true;
    //      afterJumpAnimYes = true;
    //      down = false;
    //      playerCheck.ChechDoubleJump();

    //      // Эффект пыли из-под ног в начале прыжка
    //      GameObject jumpStart = Pooler.GetPooledObject("PlayerJumpStart");
    //      jumpStart.transform.position = new Vector3(transform.position.x - 0.3f, transform.position.y - 0.7f, -0.1f);
    //      jumpStart.SetActive(true);
    //    }
    //  }

    //  if(x != 0) controlMoveX = runSpeed * Mathf.Sign(x) * (drag ? 0.2f : 1);

    //  // Графитационное действие
    //  velocity.y -= gravity * Time.deltaTime;

    //  if(miniJump > 0) {
    //    velocity.y = miniJump;
    //    miniJump = 0;
    //  }

    //  velocity.x += controlMoveX;

    //  // Ограничения горизонтального перемещения
    //  if((transform.position.x < CameraController.displayDiff.leftDif(0.8f) && controlMoveX < 0)
    //      || (transform.position.x > CameraController.displayDiff.rightDif(0.8f) && controlMoveX > 0))
    //    velocity.x = thisRunSpeedX;

    //  if((transform.position.y > CameraController.displayDiff.topDif(0.95f) && playerPets.playerPets != PlayerPetsTypes.none) && velocity.y > 0)
    //    velocity.y = 0;

    //  if(transform.position.x < CameraController.displayDiff.leftDif(0.9f))
    //    velocity.x += 5;


    //  // Если игрок падал, и у него остался запас спасений
    //  if(player.playerDowner) {
    //    if(transform.position.y < 8f)
    //      velocity.y = 800 * Time.deltaTime;
    //    else {
    //      player.playerDowner = false;
    //    }
    //  }

    //  // Первоначальное появление
    //  if(player.playerStart) {
    //    velocity.x = thisRunSpeedX * 1.5f;
    //  }

    //  // Запрет смещения при прилипании
    //  if(isSticky) {

    //    if(transform.position.x < CameraController.displayDiff.leftDif(0.8f) || transform.position.x > CameraController.displayDiff.rightDif(0.8f))
    //      velocity.x = thisRunSpeedX;
    //    else
    //      velocity.x = 0;
    //  }


    //  if(player.playerDownWait) velocity.y = 0;

    //  if(!stopRun) {
    //    PlayerMove(ref velocity);
    //  }

    //  deltaPosition = transform.position - lastPosition;

    //  // Анимация пыли при приземлении
    //  if(jumpThis && player.isGround /*&& velocity.y < 0*/) {
    //    jumpThis = false;
    //    velocity.y = 0;

    //    //allColliderGrount = Physics.OverlapSphere(groundChecker.position , 0.1f , player.groundLayer);
    //    isNet = false;

    //    for(int i = 0; i < player.isGroungsArray.Length; i++)
    //      if(LayerMask.LayerToName(player.isGroungsArray[i].gameObject.layer) == "Net")
    //        isNet = true;

    //    // Если после прыжка не приземлились в паутину, создаем эффект пыли из под ног
    //    if(!isNet) {
    //      GameObject jumpFinish = Pooler.GetPooledObject("PlayerJumpFinish");
    //      jumpFinish.transform.position = new Vector3(transform.position.x + 0.08f, transform.position.y - 0.4f, -0.1f);
    //      jumpFinish.SetActive(true);

    //    }
    //  }

    //  if(thisDoubleJump && player.isGround && !down && velocity.y < 0)
    //    thisDoubleJump = false;

    //}

    /// <summary>
    /// Двигаем плеера
    /// </summary>
    /// <param name="velocity">Вектор движения</param>
    /// <param name="playerSizeDiff">Спещение точки касания с зеплей относительно центра тяжести</param>
    //public void PlayerMove(ref Vector3 velocity, float playerSizeDiff = PLAYER_SIZE_DIFF_Y) {

    //rb.velocity = new Vector2(velocity.x, rb.velocity.y);

    //checkPosition = transform.position + velocity * Time.deltaTime + playerDiffSize;

    //player.isGroungsArray = Physics.OverlapSphere(checkPosition, player.groundRadius * 30f * Time.deltaTime * Vector3.Distance(lastPosition, checkPosition), player.groundLayer);
    //player.isGround = false;

    //if(player.isGroungsArray.Length > 0) {
    //  player.isGround = Vector3.Distance(new Vector3(checkPosition.x, player.isGroungsArray[0].transform.position.y, 0), checkPosition) <= player.groundRadius * 30f * Time.deltaTime * Vector3.Distance(lastPosition, checkPosition);
    //}

    //if(player.isGround && velocity.y <= 0) {
    //  velocity.y = 0;
    //  transform.position = new Vector3(transform.position.x + velocity.x * Time.deltaTime, player.isGroungsArray[0].transform.position.y + PLAYER_SIZE_DIFF_Y, transform.position.z);
    //} else if(!player.isGround && velocity.y <= 0) {
    //  groundHits = Physics.RaycastAll(new Ray(transform.position + playerDiffSize, checkPosition - (transform.position + playerDiffSize)), Vector3.Distance(checkPosition, transform.position + playerDiffSize / 2), player.groundLayer);
    //  player.isGround = groundHits.Length > 0;
    //  if(player.isGround && groundHits[0].transform.position.y <= checkPosition.y) {
    //    transform.position = new Vector3(checkPosition.x, groundHits[0].transform.position.y + PLAYER_SIZE_DIFF_Y, transform.position.z);
    //    velocity.y = 0;
    //  } else {
    //    transform.position += velocity * Time.deltaTime;
    //    //rb.MovePosition(transform.position + velocity * Time.deltaTime);
    //  }
    //} else {
    //  transform.position += velocity * Time.deltaTime;
    //  //rb.MovePosition(transform.position + velocity * Time.deltaTime);
    //}

    //}


    RaycastHit[] groundHits;
    /*
    void OnDrawGismos() {
        Gizmos.DrawRay(new Ray(transform.position + playerDiffSize , checkPosition - ( transform.position + playerDiffSize )));
    }
    */
    //int grountI;

    //bool isGround;
    //Vector3 checkPosition;
    //bool isNet;

    /// <summary>
    /// Анимация игрока во время смерти
    /// </summary>
    void DeadAnimation() {
      if (!activDeadAnim) {
        if (controller.isGround)
          animation.SetAnimation(animation.deadAnim, false);
        else
          animation.SetAnimation(animation.deadJumpAnim, false);
        activDeadAnim = true;
      }
    }

    void PreBoostMovement() {
      animation.SetAnimation(animation.runIdleAnim, true);
      velocity.x = RunSpeedToPlayer;
      //controller.Move(velocity * Time.deltaTime);
    }

    /// <summary>
    /// Движение игрока во время смерти
    /// </summary>
    void DeadMovement() {
      velocity.x = RunnerController.RunSpeed;
      rigidbody.velocity = new Vector2(velocity.x, rigidbody.velocity.y);
    }

    bool isJump;

    /// <summary>
    /// Подталкивание при падении на особые предметы
    /// </summary>
    /// <param name="flag"></param>
    public void Jump(bool flag) {
      isJump = flag;
    }

    /// <summary>
    /// Реакция на клавиши движения
    /// </summary>
    /// <param name="mov"></param>
    public void Movement(float mov) {
      horizontalKey = mov;
    }

  }
}