using Spine.Unity;
using System.Collections;
using UnityEngine;

namespace Player.Jack {

  public abstract class PlayerMove: PlayerComponent {

    public static event System.Action OnPlayerGround;
    public static event System.Action OnPlayerJump;

    public static bool isSticky;                          // Прилип
    public static bool isStopped;                         // Остановка
    protected bool isDead;                                 // Флаг анимации смерти

    [HideInInspector]
    public Vector3 velocity;                       // Рассчет движения
    protected FloatSpan boundary;
    //protected float vectorX;

    [HideInInspector]
    public bool isDrag;                                   // Сопротивление
    public float dragTime;

    public float RunSpeedToPlayer {
      get { return (RunnerController.Instance.runnerPhase == RunnerPhase.endRun ? RunnerController.RunSpeed * 3 : RunnerController.RunSpeed)/* * 1.08f */* (isDrag ? 0.5f : 1); }
    }

    protected abstract float horizontalSpeed { get; }
    public virtual float jumpSpeed { get { return 17; } }

    protected RunnerPhase runPhase;

    private void Start() {
      RunnerController.OnChangeRunnerPhase += ChangePhase;
    }

    protected override void OnDestroy() {
      base.OnDestroy();
      RunnerController.OnChangeRunnerPhase -= ChangePhase;
    }

    protected virtual void ChangePhase(RunnerPhase newPhase) {
      //if (newPhase != RunnerPhase.boost || newPhase != RunnerPhase.postBoost || newPhase != RunnerPhase.start) return;

      if (newPhase == RunnerPhase.boost) {
        //PlayBoostAudio();
        //AudioManager.PlayEffects(boosStartClip, AudioMixerTypes.runnerEffect);
      }

      runPhase = newPhase;
    }

    public abstract void Init();

    public virtual void DeInit() {
      GetComponent<ShadowController>().Fixed(false);
      GetComponent<ShadowController>().SetDeff();

      controller.boxDamageCollider.offset = Vector2.zero;
      controller.boxDamageCollider.size = new Vector2(0.44f, 1.7f);

      controller.boxGroundCollider.offset = new Vector3(0, 0f);
      controller.boxGroundCollider.size = new Vector2(0.44f, 0.005f);

      controller.graphic.transform.localPosition = new Vector3(0, 0, controller.graphic.transform.localPosition.z);
      controller.graphic.transform.localEulerAngles = new Vector3(0, 0, 0);
      animation.skeletonAnimation.transform.localPosition = new Vector3(0, -0.88f, animation.skeletonAnimation.transform.localPosition.z);
      animation.ResetAnimation();
    }

    protected virtual void Update() {

      boundary.min = CameraController.displayDiff.leftDif(0.85f);
      boundary.max = CameraController.displayDiff.rightDif(0.85f);

      Animation();
      Move();
    }
    
    protected virtual void Move() {

      CalcHorisontalVelocity();

      if (jumpKey) {
        jumpKey = false;
        CalcVerticalVelocity();

        rigidbody.velocity = new Vector2(velocity.x, 0);

        rigidbody.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
      } else {
        rigidbody.velocity = velocity;
      }
    }

    public float runSpeedPlayer {
      get { return (RunnerController.Instance.runnerPhase == RunnerPhase.endRun ? 10 : RunnerController.RunSpeed)/* * 1.08f * (drag ? 0.5f : 1)*/; }
    }

    protected virtual void CalcHorisontalVelocity() {

      velocity = rigidbody.velocity;
      velocity.x = runSpeedPlayer;

      if (horizontalKeyValue < 0 && transform.position.x < CameraController.displayDiff.leftDif(0.8f))
        velocity.x = runSpeedPlayer;
      else if (horizontalKeyValue > 0 && transform.position.x > CameraController.displayDiff.rightDif(0.8f))
        velocity.x = runSpeedPlayer;
      else {
        //if (horizontalKey != 0) controlMoveX = runSpeed * Mathf.Sign(x)/* * (drag ? 0.2f : 1)*/;
        velocity.x += (horizontalKeyValue != 0) ? horizontalSpeed * Mathf.Sign(horizontalKeyValue)/* * (drag ? 0.2f : 1)*/ : 0;
      }

      if (isSticky)
        velocity.x = runSpeedPlayer;
    }

    protected virtual void CalcVerticalVelocity() {

      doubleJumpReady = controller.isGround && rigidbody.velocity.y <= 0;

      if (controller.isGround) {
        animation.SetAnimation(animation.jumpIdleAnim, true);
        AudioManager.PlayEffect(audio.jumpAudio, AudioMixerTypes.runnerEffect);
        isJumped = true;
      } else if ((doubleJumpReady && !isDoubleJumped) || GameManager.activeLevelData.gameFormat == GameMechanic.jetPack) {
        doubleJumpReady = false;
        isDoubleJumped = true;
        AudioManager.PlayEffect(audio.jumpAudioAir, AudioMixerTypes.runnerEffect);
      } else
        return;

      isJumped = true;

      GameObject jumpStart = Pooler.GetPooledObject("PlayerJumpStart");
      jumpStart.transform.position = new Vector2(controller.graundPoint.position.x, controller.graundPoint.position.y);
      jumpStart.SetActive(true);
    }

    protected void OnGroundEmit() {
      if (OnPlayerGround != null) OnPlayerGround();
    }

    #region Animation

    protected virtual void Animation() {

      if (controller.isGround && rigidbody.velocity.y <= 0) {
        animation.skeletonAnimation.timeScale = 1f;
        animation.SetAnimation(animation.runIdleAnim, true);
      } else if (isDoubleJumped | isJumped) {
        if (isDoubleJumped) {
          animation.skeletonAnimation.timeScale = 2.5f;
          animation.SetAnimation(animation.jumpDoubleIdleAnim, false, true);
        } else
          animation.SetAnimation(animation.jumpIdleAnim, true);
      }

    }
    #endregion

    protected Coroutine InvokeMethod(System.Action method, float timeWait) {
      return StartCoroutine(InvokeMethodCoroutine(method, timeWait));
    }

    private IEnumerator InvokeMethodCoroutine(System.Action method, float timeWait) {
      yield return new WaitForSeconds(timeWait);
      method();
    }

    [ExEvent.ExEventHandler(typeof(ExEvent.RunEvents.RunPhaseChange))]
    private void RunPhaseChange(ExEvent.RunEvents.RunPhaseChange eventData) {
      RunChangePhase(eventData.oldPhase, eventData.newPhase);
    }

    protected virtual void RunChangePhase(RunnerPhase oldValue, RunnerPhase newValue) {

    }

    public void MinJump() {
      //miniJump = val * (playerPets.playerPets == PlayerPetsTypes.spider ? -1 : 1);
      rigidbody.velocity = new Vector2(rigidbody.velocity.x, 0);
      isJumped = true;
      rigidbody.AddForce(new Vector2(rigidbody.velocity.x, jumpSpeed * 0.8f), ForceMode2D.Impulse);
    }

    #region Управление
    [HideInInspector]
    public virtual bool jumpKey { get; set; }           // Запрос на совершения прыжка

    protected float horizontalKeyValue;
    public virtual float horizontalKey {
      set {
        horizontalKeyValue = value != 0 ? Mathf.Sign(value) : 0;
      }
    }      // Значение мобильного контроллера, отвечающего за горизонтальное смещение
    [HideInInspector]
    public bool isJumped;            // Флаг означающий выполнения врыжка
    [HideInInspector]
    public bool isDoubleJumped;      // Флаг выполнения второго прыжка
    [HideInInspector]
    public bool doubleJumpReady;     // Готовность выполнить второй прыжок
    
    #endregion

    #region Audio

    protected abstract AudioClip boostClip { get; }
    public AudioSource audioCompBoost;                  // Компонент аудио

    protected void PlayBoostAudio() {
      if (boostClip != null & audioCompBoost.clip != boostClip) {
        audioCompBoost.clip = boostClip;
        audioCompBoost.Play();
      }
    }

    protected void ResumePlayBoostAudio() {
      if (!audioCompBoost.isPlaying)
        audioCompBoost.UnPause();
    }

    protected void PausePlayBoostAudio() {
      if (audioCompBoost.isPlaying)
        audioCompBoost.Pause();
    }

    #endregion

  }
}