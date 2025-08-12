using UnityEngine;
using Spine.Unity;
using Player.Jack;

namespace Pet {

  /// <summary>
  /// Пет летучая мышь
  /// </summary>
  public class BatPet: Pet {

    /// <summary>
    /// Время последнего взмаха крыльев
    /// </summary>
    float lastJump;
    /// <summary>
    /// Фронмальное крыло
    /// </summary>
    public Animator frontWind;
    /// <summary>
    /// Крыло сзади
    /// </summary>
    public Animator backWind;
    /// <summary>
    /// Звук взмаха крыльев
    /// </summary>
    public AudioClip[] windClip;
    /// <summary>
    /// Анимация полета с джеком
    /// </summary>
    [SpineAnimation(dataField: "skeletonAnimation")]
    public string flyJackAnim = "";

    public override void OnEnable() {
      base.OnEnable();

      if (phase == Phase.shop) {
        ResetAnimation();
        return;
      }
    }

    protected override void FixedUpdate() {
      if (phase == Phase.shop) return;
      base.FixedUpdate();

      if (transform.position.y <= 2.3f)
        Jump();

    }

    /// <summary>
    /// Свободный полет
    /// </summary>
    protected override void RunFree() {

      if (phase == Phase.shop) return;

      velocity = rb.velocity;

      if (phase == Phase.free) {
        velocity.x = RunnerController.RunSpeed * 2;
        velocity.y = 2;
      } else {
        velocity.x = RunnerController.RunSpeed * 0.2f;
        velocity.y = 0;
      }

      if (lastJump + 0.5f < Time.time) {
        JumpFree();
        lastJump = Time.time;
      }

      rb.velocity = velocity;
      //transform.position += velocity * Time.deltaTime;
    }

    /// <summary>
    /// Анимация взмаха крыльев
    /// </summary>
    void JumpFree() {

      WindActive();
    }

    /// <summary>
    /// Обработка прыжка
    /// </summary>
    protected override void Jump() {

      jumpKey = false;
      rb.velocity = new Vector2(rb.velocity.x, 0);
      rb.AddForce(new Vector2(rb.velocity.x, jumpSpeed), ForceMode2D.Impulse);
      JumpFree();
      AudioManager.PlayEffect(windClip[Random.Range(0, windClip.Length)], AudioMixerTypes.runnerEffect);



      //if (canJump && jump) {
      //	velocity.y = jumpSpeed;
      //	canJump = false;
      //}
    }

    /// <summary>
    /// Активация джеком
    /// </summary>
    /// <param name="jack">Ссылка на самого джека</param>
    public override void JackActivate(GameObject jack) {
      base.JackActivate(jack);
      if (phase == Phase.uses)
        SetAnimation(flyJackAnim, true);
    }

    /// <summary>
    /// Событие анимации
    /// </summary>
    /// <param name="state">Анимация</param>
    /// <param name="trackIndex">Индекс</param>
    /// <param name="e">Событие анимации</param>
    protected override void AnimEvent(Spine.AnimationState state, int trackIndex, Spine.Event e) {
      WindActive();

      if (phase != Phase.shop) {
        AudioManager.PlayEffect(windClip[Random.Range(0, windClip.Length)], AudioMixerTypes.runnerEffect);
      }
    }

    public void WindActive() {
      frontWind.SetTrigger("jump");
      backWind.SetTrigger("jump");
    }

    //protected override void CheckFixedGroung() {
    //}

    /// <summary>
    /// Дополнительные проверки при управлении джеком
    /// </summary>
    //protected override void AlterChecking() {
    //	base.AlterChecking();
    //	if (transform.position.y > CameraController.displayDiff.topDif(0.9f) && velocity.y > 0)
    //		velocity.y = 0;
    //	if (transform.position.y < RunnerController.mapHeight + 1 && velocity.y < 0)
    //		velocity.y = 0;
    //}

  }
}